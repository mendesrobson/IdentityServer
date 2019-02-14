using Advice.Ranoi.Core.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Advice.Ranoi.Core.Domain.Interfaces;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using System.Reflection;
using MongoDB.Bson.Serialization;
using Microsoft.Extensions.Options;

namespace Advice.Ranoi.Core.Data
{
    public class Context : IContext
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected static Boolean _mapped;

        public Context(IOptions<DbConfiguration> myConfiguration)
        {
            var database = myConfiguration.Value.Database;
            var mongodb = myConfiguration.Value.Path;

            if (String.IsNullOrEmpty(database))
                database = "defaultDB";

            if (String.IsNullOrEmpty(mongodb))
                mongodb = "mongodb://localhost:27017";

            _client = new MongoClient(mongodb);
            _database = _client.GetDatabase(database);
            Collections = new Dictionary<Type, List<IEntity>>();

            if (!_mapped)
            {
                ConventionRegistry.Register(
                                    "DictionaryRepresentationConvention",
                                    new ConventionPack { new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays) },
                                    _ => true);

                MapMongo();

                _mapped = true;
            }
        }

        private void MapMongo()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies.Where(x => x.FullName.Contains("Liera")))
            {
                Type[] assemblyTypes = assembly.GetTypes();

                foreach (Type t in assemblyTypes.Where(x => !x.IsInterface && (x.IsClass)))
                {
                    Type tipoFinal = null;
                    if (t.IsGenericType)
                    {
                        Type[] genericArguments = t.GetGenericArguments();
                        for (int i = 0; i < genericArguments.Length; i++)
                        {
                            //HACK: Se tiver mais de uma constraint, vai dar pau.
                            Type[] constraints = genericArguments[i].GetGenericParameterConstraints();
                            if (constraints != null && constraints.Length > 0)
                            {
                                genericArguments[i] = constraints[0];
                            }
                        }
                        tipoFinal = t.MakeGenericType(genericArguments);
                    }
                    else
                    {
                        tipoFinal = t;
                    }

                    try
                    {
                        MethodInfo method = typeof(Context).GetMethod("RegisterMongo");
                        MethodInfo generic = method.MakeGenericMethod(tipoFinal);
                        generic.Invoke(this, null);
                    }
                    catch (Exception ggwp)
                    {
                        Console.WriteLine("Não Registrou: " + t.FullName);
                    }
                }
            }
        }

        public void RegisterMongo<T>()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>();
            }
        }

        private Dictionary<Type, List<IEntity>> Collections { get; set; }

        public String FindCollectionName(Type entityType)
        {
            RootAttribute root = entityType.GetCustomAttribute<RootAttribute>(true);

            entityType = root == null ? entityType : root._rootType;

            var fullName = entityType.FullName;

            if (fullName.Contains("Liera"))
            {
                fullName = fullName.Replace("Liera.", "");
            }

            if (fullName.Contains("Interfaces"))
            {
                fullName = fullName.Replace("Interfaces.", "");
            }

            if (typeof(IXDomainEvent).IsAssignableFrom(entityType))
            {
                return "Publisher.XDomainEvent";
            }

            if (fullName.Contains("Domain"))
            {
                fullName = fullName.Replace("Domain.", "");
            }

            return fullName;
        }

        public IMongoDatabase DataBase { get { return Context._database; } }
        public IMongoClient Client { get { return Context._client; } }

        public IMongoCollection<BsonDocument> GetBsonCollection(Type entityType)
        {
            var collectioName = FindCollectionName(entityType);

            return _database.GetCollection<BsonDocument>(collectioName);
        }

        public Boolean Add(IEntity entity)
        {
            var collection = this.GetBsonCollection(entity.GetType());

            if (collection.Find(Builders<BsonDocument>.Filter.Eq("_id", entity.Id)).SingleOrDefault() == null)
            {
                collection.InsertOne(entity.ToBsonDocument());
                return true;
            }

            return false;
        }

        public Boolean Update(IEntity entity, Guid transactionId)
        {
            var oldVersion = entity.Version;

            var collection = this.GetBsonCollection(entity.GetType());

            FilterDefinition<BsonDocument> query = Builders<BsonDocument>.Filter.And(
                                                        Builders<BsonDocument>.Filter.Eq("_id", entity.Id),
                                                        Builders<BsonDocument>.Filter.Eq("Version", oldVersion),
                                                        Builders<BsonDocument>.Filter.Eq("TransactionId", transactionId),
                                                        Builders<BsonDocument>.Filter.Eq("Inactive", false)
                                                    );

            entity.Version++;

            var entityBson = entity.ToBsonDocument();

            entityBson.Remove("_id");

            BsonDocument updateDocument = new BsonDocument();
            updateDocument.Add("$set", entityBson);

            UpdateDefinition<BsonDocument> update = updateDocument;

            var result = collection.FindOneAndUpdate(query, update);

            if (result == null)
                return false;

            return true;
        }

        public Boolean Remove(IEntity entity, Guid transactionId)
        {
            var collection = this.GetBsonCollection(entity.GetType());

            FilterDefinition<BsonDocument> query = Builders<BsonDocument>.Filter.And(
                                                        Builders<BsonDocument>.Filter.Eq("_id", entity.Id),
                                                        Builders<BsonDocument>.Filter.Eq("Version", entity.Version),
                                                        Builders<BsonDocument>.Filter.Eq("TransactionId", transactionId),
                                                        Builders<BsonDocument>.Filter.Eq("Inactive", false)
                                                    );

            entity.Version++;

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("Inactive", true).Set("Version", entity.Version);

            var result = collection.FindOneAndUpdate(query, update);

            if (result == null)
                return false;

            return true;
        }

        public Boolean Purge(IEntity entity)
        {
            var collection = this.GetBsonCollection(entity.GetType());

            FilterDefinition<BsonDocument> query = Builders<BsonDocument>.Filter.Eq("_id", entity.Id);

            var result = collection.DeleteOne(query);

            if (result.DeletedCount.Equals(1))
                return true;
            else
                return false;
        }

        public Boolean Lock(IEntity entity, Guid transactionId)
        {
            var collection = this.GetBsonCollection(entity.GetType());

            FilterDefinition<BsonDocument> query = Builders<BsonDocument>.Filter.And(
                                                        Builders<BsonDocument>.Filter.Eq("_id", entity.Id),
                                                        Builders<BsonDocument>.Filter.Eq("Version", entity.Version),
                                                        Builders<BsonDocument>.Filter.Eq("TransactionId", Guid.Empty),
                                                        Builders<BsonDocument>.Filter.Eq("Inactive", false)
                                                    );

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("TransactionId", transactionId);

            var result = collection.FindOneAndUpdate(query, update);

            if (result == null)
                return false;

            return true;
        }

        public Boolean UnLock(IEntity entity, Guid transactionId)
        {
            var collection = this.GetBsonCollection(entity.GetType());

            FilterDefinition<BsonDocument> query = Builders<BsonDocument>.Filter.And(
                                                        Builders<BsonDocument>.Filter.Eq("_id", entity.Id),
                                                        Builders<BsonDocument>.Filter.Eq("Version", entity.Version),
                                                        Builders<BsonDocument>.Filter.Eq("TransactionId", transactionId)
                                                    );

            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("TransactionId", Guid.Empty);

            var result = collection.FindOneAndUpdate(query, update);

            if (result == null)
                return false;

            return true;
        }
    }
}
