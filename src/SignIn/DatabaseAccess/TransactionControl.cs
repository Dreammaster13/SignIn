using System;
using Starcounter;

namespace SignIn.DatabaseAccess
{
    public class TransactionControl:ITransactionControl
    {
        public T Transact<T>(Func<T> action)
        {
            return Db.Transact(action);
        }
    }
}