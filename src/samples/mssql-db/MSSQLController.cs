using System;
using K8sControllerSDK;

namespace mssql_db
{
	class MSSQLController
	{
		static void Main(string[] args)
		{
			Controller<MSSQLDB> controller = new Controller<MSSQLDB>(new MSSQLDB(), new MSSQLDBOperationHandler());
			controller.SatrtAsync(new System.Threading.CancellationToken());

			Console.WriteLine("Press <enter> to quit...");
			Console.ReadLine();
		}
	}
}
