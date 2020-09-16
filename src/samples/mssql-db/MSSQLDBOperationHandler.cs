using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sControllerSDK;

namespace mssql_db
{
	public class MSSQLDBOperationHandler : OperationHandler<MSSQLDB>
	{
		object _lockObject = new object();

		string Instance { get; set; }
		string DBUser { get; set; }
		string Password { get; set; }

		SqlConnection GetDBConnection(Kubernetes k8s, MSSQLDB db)
		{
			if (string.IsNullOrEmpty(Instance))
			{
				var configMap = k8s.ReadNamespacedConfigMap(db.Spec.Config, db.Namespace());
				Instance = configMap.Data["instance"];
			}

			if (string.IsNullOrEmpty(DBUser) || string.IsNullOrEmpty(Password))
			{
				var secret = k8s.ReadNamespacedSecret(db.Spec.Data, db.Namespace());
				DBUser = ASCIIEncoding.UTF8.GetString( secret.Data["userid"]);
				Password = ASCIIEncoding.UTF8.GetString(secret.Data["password"]);
			}

			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

			builder.DataSource = Instance;
			builder.UserID = DBUser;
			builder.Password = Password;
			builder.InitialCatalog = "master";

			return new SqlConnection(builder.ConnectionString);
		}

		public Task OnAdded(Kubernetes k8s, MSSQLDB crd)
		{
			lock (_lockObject)
			{

				Console.WriteLine($"DATABASE {crd.Spec.DBName} will be ADDED");
				
				using (SqlConnection connection = GetDBConnection(k8s, crd))
				{
					connection.Open();

					try
					{
						SqlCommand createCommand = new SqlCommand($"CREATE DATABASE {crd.Spec.DBName};", connection);
						int i = createCommand.ExecuteNonQuery();
					}
					catch (SqlException sex)
					{
						if (sex.Number == 1801) //Database already exists
						{
							Console.WriteLine(sex.Message);
							return Task.CompletedTask;
						}
					}

					Console.WriteLine($"DATABASE {crd.Spec.DBName} successfully created!");

				}

				return Task.CompletedTask;
			}
		}

		public Task OnBookmarked(MSSQLDB crd)
		{
			Console.WriteLine($"DATABASE {crd.Spec.DBName} was BOOKMARKED");

			return Task.CompletedTask;
		}

		public Task OnDeleted(Kubernetes k8s, MSSQLDB crd)
		{
			lock (_lockObject)
			{

				Console.WriteLine($"DATABASE {crd.Spec.DBName} will be DELETED!");

				using (SqlConnection connection = GetDBConnection(k8s, crd))
				{
					connection.Open();

					try
					{
						SqlCommand createCommand = new SqlCommand($"DROP DATABASE {crd.Spec.DBName};", connection);
						int i = createCommand.ExecuteNonQuery();
					}
					catch (SqlException sex)
					{
						if (sex.Number == 3701) //Already gone!
						{
							Console.WriteLine(sex.Message);
							return Task.CompletedTask;
						}
					}

					Console.WriteLine($"DATABASE {crd.Spec.DBName} successfully deleted!");

				}

				return Task.CompletedTask;
			}
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
