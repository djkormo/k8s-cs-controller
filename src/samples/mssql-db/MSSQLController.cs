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
			Log.Info($"=== {nameof(MSSQLController)} STARTING ===");

			MSSQLDBOperationHandler handler = new MSSQLDBOperationHandler();
			Controller<MSSQLDB> controller = new Controller<MSSQLDB>(new MSSQLDB(), handler);
			Task t1 = controller.SatrtAsync(new System.Threading.CancellationToken());
			Task t2 = handler.CheckCurrentState(controller.Kubernetes);

			Log.Info($"=== {nameof(MSSQLController)} STARTED ===");

			Task.WaitAll(new Task[] { t1, t2 });

			Log.Warn($"=== {nameof(MSSQLController)} TERMINATING ===");
		}
	}
}
