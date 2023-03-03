using Microsoft.VisualBasic;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeamonWithROPCFlowExample.DTO;

[Route("/projects", "GET POST")]
public class GetProjects : IReturn<List<Project>>
{
	public string SearchCriteria { get; set; }
	public int PageSize { get; set; }
	public int PageNumber { get; set; }
	public string OrderBy { get; set; }
}