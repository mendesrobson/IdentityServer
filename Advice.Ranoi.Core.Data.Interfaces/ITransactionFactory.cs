using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Data.Interfaces
{
    public interface ITransactionFactory
    {
        ITransaction CreateTransaction();
    }
}
