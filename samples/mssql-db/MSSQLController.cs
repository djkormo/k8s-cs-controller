using System;
using System.Threading.Tasks;
using K8sControllerSDK;
using NLog;

namespace mssql_db
{
	class MSSQLController
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			try
			{
				Controller<MSSQLDB>.ConfigLogger();

				Log.Info($"=== {nameof(MSSQLController)} STARTING ===");

				MSSQLDBOperationHandler handler = new MSSQLDBOperationHandler();
				Controller<MSSQLDB> controller = new Controller<MSSQLDB>(new MSSQLDB(), handler);
				controller.SatrtAsync();
				Task reconciliation = handler.CheckCurrentState(controller.Kubernetes);

				Log.Info($"=== {nameof(MSSQLController)} STARTED ===");

				reconciliation.ConfigureAwait(false).GetAwaiter().GetResult();

			}
			catch (Exception ex)
			{
				Log.Fatal(ex);
			}
			finally
			{
				Log.Warn($"=== {nameof(MSSQLController)} TERMINATING ===");
			}
		}
	}
}
