using System;
using controller_sdk;

namespace mssql_db
{
	public class MSSQLDB : BaseCRD
	{

		public MSSQLDB() :
			base("samples.k8s-cs-controller", "v1","dbs","db")
		{ }

		

		public MSSQLDBSpec Spec { get; set; }
	}
}
