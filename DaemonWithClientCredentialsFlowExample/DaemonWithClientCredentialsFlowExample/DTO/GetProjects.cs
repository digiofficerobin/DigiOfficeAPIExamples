using ServiceStack;

namespace DaemonWithClientCredentialsFlowExample.DTO
{
	[Route("/projects", "GET POST")]
	public class GetProjects : IReturn<List<Project>>
	{
		public string SearchCriteria { get; set; }
		public int PageSize { get; set; }
		public int PageNumber { get; set; }
		public string OrderBy { get; set; }
	}
}