namespace mssql_db
{
	public class MSSQLDBSpec
	{
		public string DBName { get; set; }

		public string Config { get; set; }

		public string Data { get; set; }

		public override string ToString()
		{
			return $"{DBName}:{Config}:{Data}"; 
		}
	}
}
