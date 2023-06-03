using Microsoft.Extensions.Logging;
using System;

namespace Assessment.Group.Phase.Repositories
{
    public interface IRepository
    {
        T LogExceptionIfFail<T>(ILogger logger, Func<T> func);
        T LogExceptionAndRollbackTransactionIfFail<T>(ILogger logger, Func<T> func);
    }
}
