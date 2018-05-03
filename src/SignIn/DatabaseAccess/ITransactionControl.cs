using System;

namespace SignIn.DatabaseAccess
{
    public interface ITransactionControl
    {
        T Transact<T>(Func<T> action);
    }
}