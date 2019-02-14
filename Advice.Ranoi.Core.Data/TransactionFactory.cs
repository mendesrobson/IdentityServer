using Advice.Ranoi.Core.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Data
{
    public class TransactionFactory : ITransactionFactory
    {
        public TransactionFactory()
        {
        }

        public ITransaction CreateTransaction()
        {
            return new Transaction();
        }
    }
}
