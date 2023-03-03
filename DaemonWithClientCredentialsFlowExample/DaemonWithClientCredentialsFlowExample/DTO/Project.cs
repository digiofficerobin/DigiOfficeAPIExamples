namespace DaemonWithClientCredentialsFlowExample.DTO
{
	public class Project
	{
		public int ID { get; set; }
		public Guid GlobalID { get; set; }
		public string Number { get; set; }
		public string Name { get; set; }
		public string Description1 { get; set; }
		public string Description2 { get; set; }
		public string City { get; set; }
		public bool Active { get; set; }
	}
}