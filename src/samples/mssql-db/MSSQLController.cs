using System.Threading.Tasks;
using K8sControllerSDK;

namespace mssql_db
{
	class MSSQLController
	{
		static void Main(string[] args)
		{
			MSSQLDBOperationHandler handler = new MSSQLDBOperationHandler();
			Controller<MSSQLDB> controller = new Controller<MSSQLDB>(new MSSQLDB(), handler);
			Task t1 = controller.SatrtAsync(new System.Threading.CancellationToken());
			Task t2 = handler.CheckCurrentState(controller.Kubernetes);

			Task.WaitAll(new Task[] { t1, t2 });
		}
	}
}
