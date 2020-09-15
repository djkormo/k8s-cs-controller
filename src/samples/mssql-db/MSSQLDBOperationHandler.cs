using System;
using System.Threading.Tasks;
using controller_sdk;

namespace mssql_db
{
	public class MSSQLDBOperationHandler : OperationHandler<MSSQLDB>
	{
		public Task OnAdded(MSSQLDB crd)
		{
			Console.WriteLine($"DATABASE {crd.Spec.DBName} was ADDED");

			return Task.CompletedTask;
		}

		public Task OnBookmarked(MSSQLDB crd)
		{
			Console.WriteLine($"DATABASE {crd.Spec.DBName} was BOOKMARKED");

			return Task.CompletedTask;
		}

		public Task OnDeleted(MSSQLDB crd)
		{
			Console.WriteLine($"DATABASE {crd.Spec.DBName} was DELETED");

			return Task.CompletedTask;
		}

		public Task OnError(MSSQLDB crd)
		{
			Console.WriteLine($"ERROR on {crd.Spec.DBName}");

			return Task.CompletedTask;
		}

		public Task OnUpdated(MSSQLDB crd)
		{
			Console.WriteLine($"DATABASE {crd.Spec.DBName} was UPDATED");

			return Task.CompletedTask;
		}
	}
}
