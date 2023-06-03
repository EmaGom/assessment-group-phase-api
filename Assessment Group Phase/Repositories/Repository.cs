using Microsoft.Extensions.Logging;
using System.Transactions;
using System;

namespace Assessment.Group.Phase.Repositories
{
    public class Repository : IRepository
    {
        public T LogExceptionAndRollbackTransactionIfFail<T>(ILogger logger, Func<T> func)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    T response = func();
                    scope.Complete();
                    return response;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message} for {ex.Data}");
                throw;
            }
        }

        public T LogExceptionIfFail<T>(ILogger logger, Func<T> func)
        {
            try
            {
                T response = func();
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message} for {ex.Data}");
                throw;
            }
        }
    }
}
