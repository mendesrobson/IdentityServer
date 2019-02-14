using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        #region IMDB
        private IContext IMDB { get; set; }
        #endregion

        private Dictionary<Guid, IEntity> FechadurasAdquiridas { get; set; }
        private Boolean TemFechaduras { get { return FechadurasAdquiridas.Count > 0; } }
        private Boolean EstaLiberado { get; set; }
        private String CommitStage { get; set; }
        public bool IsCommited { get; private set; }
        private ITransaction CurrentTransaction { get; set; }

        public UnitOfWork(IContext context, ITransaction transaction)
        {
            IMDB = context;
            CurrentTransaction = transaction;
            FechadurasAdquiridas = new Dictionary<Guid, IEntity>();
        }

        public void Delete(IEntity entity)
        {
            this.CurrentTransaction.RegisterRemove(entity);
        }

        public void Save(IEntity entity)
        {
            this.CurrentTransaction.RegisterSave(entity);
        }

        public void Commit()
        {
            if (this.IsCommited)
                return;

            this.IsCommited = true;

            try
            {
                CommitStage = "Pré-Locks";
                GetLocks();
                CommitStage = "Pós-Locks";
                SaveTransaction();
                CommitStage = "Pós-Transaction";
                Persist();
                CommitStage = "Persistido";
                CompleteTransaction();
                CommitStage = "Transaction Complete";
                ReleaseLocks();
                CommitStage = "Locks Liberado";
                PurgeTransaction();
                CommitStage = "Transaction Purged";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Dump(ex);

                if (TemFechaduras && EstaLiberado)
                    ReleaseLocks();

                throw;
            }
        }

        public void RegisterLoaded(IEntity entity)
        {
            this.CurrentTransaction.RegisterRollback(entity);
        }

        private void GetLocks()
        {
            foreach (var entity in CurrentTransaction.ToAdd)
            {
                entity.Value.TransactionId = CurrentTransaction.Id;
                FechadurasAdquiridas.Add(entity.Key, entity.Value);
                EstaLiberado = true;
            }

            foreach (var entity in CurrentTransaction.ToSave)
            {
                entity.Value.TransactionId = CurrentTransaction.Id;

                if (!this.IMDB.Lock(entity.Value, CurrentTransaction.Id))
                    throw new ApplicationException(String.Format("Commit ({0}) - GetLocks", this.CurrentTransaction.Id));

                FechadurasAdquiridas.Add(entity.Key, entity.Value);
                EstaLiberado = true;
            }

            foreach (var entity in CurrentTransaction.ToRemove)
            {
                entity.Value.TransactionId = CurrentTransaction.Id;

                if (!this.IMDB.Lock(entity.Value, CurrentTransaction.Id))
                    throw new ApplicationException(String.Format("Commit ({0}) - GetLocks", this.CurrentTransaction.Id));

                FechadurasAdquiridas.Add(entity.Key, entity.Value);
                EstaLiberado = true;
            }
        }

        private void SaveTransaction()
        {
            foreach (var transaction in this.CurrentTransaction.FullChain)
            {
                if (!IMDB.Add(transaction))
                    throw new ApplicationException(String.Format("Commit ({0}) - SaveTransaction", this.CurrentTransaction.Id));
            }

        }

        private void Persist()
        {
            foreach (var entity in CurrentTransaction.ToAdd)
            {
                if (!this.IMDB.Add(entity.Value))
                    throw new ApplicationException(String.Format("Commit ({0}) - Persist[Add]", this.CurrentTransaction.Id));

                EstaLiberado = false;
            }

            foreach (var entity in CurrentTransaction.ToSave)
            {
                if (!this.IMDB.Update(entity.Value, CurrentTransaction.Id))
                    throw new ApplicationException(String.Format("Commit ({0}) - Persist[Update]", this.CurrentTransaction.Id));

                EstaLiberado = false;
            }

            foreach (var entity in CurrentTransaction.ToRemove)
            {
                if (!this.IMDB.Remove(entity.Value, CurrentTransaction.Id))
                    throw new ApplicationException(String.Format("Commit ({0}) - Persist[Remove]", this.CurrentTransaction.Id));

                EstaLiberado = false;
            }
        }

        private void CompleteTransaction()
        {
            this.CurrentTransaction.Complete();

            foreach (var transaction in this.CurrentTransaction.FullChain)
            {
                if (!this.IMDB.Update(transaction, Guid.Empty))
                    throw new ApplicationException(String.Format("Commit ({0}) - CompleteTransaction", this.CurrentTransaction.Id));
            }
        }

        private void ReleaseLocks()
        {
            foreach (var entity in CurrentTransaction.ToAdd)
            {
                entity.Value.TransactionId = Guid.Empty;

                if (FechadurasAdquiridas.ContainsKey(entity.Key) && !EstaLiberado)
                    if (!this.IMDB.UnLock(entity.Value, CurrentTransaction.Id))
                        throw new ApplicationException(String.Format("Commit ({0}) - Liberar bloqueios", this.CurrentTransaction.Id));
            }

            foreach (var entity in CurrentTransaction.ToSave)
            {
                entity.Value.TransactionId = Guid.Empty;

                if (FechadurasAdquiridas.ContainsKey(entity.Key))
                    if (!this.IMDB.UnLock(entity.Value, CurrentTransaction.Id))
                        throw new ApplicationException(String.Format("Commit ({0}) - Liberar bloqueios", this.CurrentTransaction.Id));
            }

            foreach (var entity in CurrentTransaction.ToRemove)
            {
                entity.Value.TransactionId = Guid.Empty;

                if (FechadurasAdquiridas.ContainsKey(entity.Key))
                    if (!this.IMDB.UnLock(entity.Value, CurrentTransaction.Id))
                        throw new ApplicationException(String.Format("Commit ({0}) - Liberar bloqueios", this.CurrentTransaction.Id));
            }
        }

        private void PurgeTransaction()
        {
            foreach (var transaction in this.CurrentTransaction.FullChain)
            {
                if (!this.IMDB.Purge(transaction)) // Exibir historico de Trasactions
                    throw new ApplicationException(String.Format("Commit ({0}) - PurgeTransaction", this.CurrentTransaction.Id));
            }
        }

        private void Dump(Exception ex)
        {
            try
            {
                var fileName = this.CurrentTransaction.ParentId.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt";

                List<String> dumpLines = new List<string>();
                String entityJson = "";

                dumpLines.Add("----- Commit Dump -----" + Environment.NewLine);

                dumpLines.Add(String.Format("Transaction Id: {0}", CurrentTransaction.ParentId));
                dumpLines.Add(String.Format("Transaction Complete: {0}", CurrentTransaction.IsComplete));
                dumpLines.Add(String.Format("Commit Stage: {0}", this.CommitStage));

                dumpLines.Add("----- Entities To Add -----" + Environment.NewLine);

                foreach (var entity in CurrentTransaction.ToAdd)
                {
                    dumpLines.Add(String.Format("Id: {0}", entity.Value.Id));
                    dumpLines.Add(String.Format("Type: {0}", entity.Value.GetType()));
                    dumpLines.Add(String.Format("Entity JSON: {0}", JsonConvert.SerializeObject(entity.Value)));
                }

                dumpLines.Add("----- End Entities To Add -----" + Environment.NewLine);

                dumpLines.Add("----- Entities To Update -----" + Environment.NewLine);

                foreach (var entity in CurrentTransaction.ToSave)
                {
                    dumpLines.Add(String.Format("Id: {0}", entity.Value.Id));
                    dumpLines.Add(String.Format("Type: {0}", entity.Value.GetType()));
                    dumpLines.Add(String.Format("Entity JSON: {0}", JsonConvert.SerializeObject(entity.Value)) + Environment.NewLine);
                }

                dumpLines.Add("----- End Entities To Update -----" + Environment.NewLine);

                dumpLines.Add("----- Entities To Delete -----" + Environment.NewLine);

                foreach (var entity in CurrentTransaction.ToRemove)
                {
                    dumpLines.Add(String.Format("Id: {0}", entity.Value.Id));
                    dumpLines.Add(String.Format("Type: {0}", entity.Value.GetType()));
                    dumpLines.Add(String.Format("Entity JSON: {0}", JsonConvert.SerializeObject(entity.Value)));
                }

                dumpLines.Add("----- End Entities To Delete -----" + Environment.NewLine);

                dumpLines.Add("----- Exception -----" + Environment.NewLine);

                dumpLines.Add(String.Format("Type: {0}", ex.GetType()));
                dumpLines.Add(String.Format("Message: {0}", ex.Message));
                dumpLines.Add(String.Format("Stack Trace: {0}", ex.StackTrace));

                dumpLines.Add("----- End Exception -----" + Environment.NewLine);

                dumpLines.Add("----- End Commit Dump -----");

                System.IO.File.WriteAllLines(fileName, dumpLines);
            }
            catch (Exception e)
            {
                Console.WriteLine("Dump failed. Ex: " + e.Message.ToString());
            }
        }

    }
}