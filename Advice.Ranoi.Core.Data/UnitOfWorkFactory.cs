using Advice.Ranoi.Core.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Data
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IUnitOfWork instance;
        private IContext Context;
        private ITransactionFactory TransactionFactory;

        public UnitOfWorkFactory(IContext context, ITransactionFactory transactionFactory)
        {
            this.Context = context;
            this.TransactionFactory = transactionFactory;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            instance = new UnitOfWork(this.Context, this.TransactionFactory.CreateTransaction());

            return instance;
        }

        public IUnitOfWork CurrentUnitOfWork()
        {
            if (instance == null)
            {
                return CreateUnitOfWork();
            }
            return instance;
        }

        public IUnitOfWork TemCertezaQueQuerUmaUnitOfWorkCompletamenteSeparada()
        {
            return new UnitOfWork(this.Context, this.TransactionFactory.CreateTransaction());
        }

        public void ResetUnitOfWork()
        {
            instance = null;
        }
    }
}
