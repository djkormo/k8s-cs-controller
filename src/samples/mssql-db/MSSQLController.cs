using System;
using K8sControllerSDK;

namespace mssql_db
{
	class MSSQLController
	{
		static void Main(string[] args)
		{
			MSSQLDBOperationHandler handler = new MSSQLDBOperationHandler();
			Controller<MSSQLDB> controller = new Controller<MSSQLDB>(new MSSQLDB(), handler);
			controller.SatrtAsync(new System.Threading.CancellationToken());
			handler.CheckCurrentState(controller.Kubernetes);

			Console.WriteLine("Press <enter> to quit...");
			Console.ReadLine();
		}
	}
}
