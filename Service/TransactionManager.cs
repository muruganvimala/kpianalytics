using System.Transactions;

namespace BusinessIntelligence_API.Service
{
	public static class TransactionManager
	{
		private static readonly TransactionScope _transactionScope;

		static TransactionManager()
		{
			_transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		}

		public static void Complete()
		{
			_transactionScope.Complete();
		}

		public static void Rollback()
		{
			_transactionScope.Dispose();
		}
	}
}
