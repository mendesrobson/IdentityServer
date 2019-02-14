using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advice.Ranoi.Core.Data.Factories
{
    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        private Dictionary<Type, IDeclaredFactory<IRepository<IEntity>>> Repositories { get; set; }

        public GenericRepositoryFactory(IEnumerable<IDeclaredFactory<IRepository<IEntity>>> factories)
        {
            Repositories = new Dictionary<Type, IDeclaredFactory<IRepository<IEntity>>>();

            factories.ToList().ForEach(factory =>
            {
                Repositories.Add(factory.GetCreatedType(), factory);
            });
        }

        public T CreateRepository<T>()
        {
            var type = typeof(T);

            return (T)Repositories[type].CreateRepository();
        }
    }
}
