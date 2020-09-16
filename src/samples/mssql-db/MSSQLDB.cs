using K8sControllerSDK;

namespace mssql_db
{
	public class MSSQLDB : BaseCRD
	{
		public MSSQLDB() :
			base("samples.k8s-cs-controller", "v1", "mssqldbs", "mssqldb")
		{ }

		public MSSQLDBSpec Spec { get; set; }
	}
}
