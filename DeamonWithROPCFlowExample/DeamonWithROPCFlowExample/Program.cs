using DeamonWithROPCFlowExample.DTO;
using Microsoft.Identity.Client;
using ServiceStack;
using System.Net.Http.Headers;

// Example of how a daemon application can authenticate (using then Resource Owner Password Owner auth 2.0 flow), authenticate at the DigiOffice API and get some projects 

// Configuration
var config = new AuthenticationConfig()
{
	ClientID = "<guid>",      // Client ID. When invalid, you get error: AADSTS700016: Application with identifier '<guid>' was not found in the directory '<tenantname>'. This can happen if the application has not been installed by the administrator of the tenant or consented to by any user in the tenant. You may have sent your authentication request to the wrong tenant.
	Username = "<usernamne>",                  // Username
	Password = "<password>",                                // Password. When password should be renewed. You get may get: Exception: There was an error parsing the WS-Trust response from the endpoint. This may occur if there are issues with your ADFS configuration. See https://aka.ms/msal-net-iwa-troubleshooting for more details. This can be solve to log in manually at portal.azure.com.
	TenantID = "<tenantid>",        // Tenant ID. When invalid, you get error: AADSTS90002: Tenant '<guid>' not found. Check to make sure you have the correct tenant ID and are signing into the correct cloud. Check with your subscription administrator, this may happen if there are no active subscriptions for the tenant.
	DigiOfficeAPIGatewayUrl = @"<digiofficeapigatewayurl>",  // DigiOffice API Gateway URL. When invalid, you get error: No such host is known. (<url>:443)
	Scopes = new string[]{ $"user.read" }						// Scopes. For DigiOffice only identification is needed.
};

try
{
	System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
		
	// Build the MSAL public client application
	var app = PublicClientApplicationBuilder.Create(config.ClientID)
	.WithAuthority(new Uri(config.Authority))
	.WithTenantId(config.TenantID)
	.Build();

	Console.WriteLine("Trying to aquire token for DigiOffice API by using the ROPC flow in Microsoft Azure Active Directory");

	// Acquire a ID token for the Web API (daemon)
	var result = await app.AcquireTokenByUsernamePassword(config.Scopes, config.Username, config.Password).ExecuteAsync();

	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("ID Token acquired: " + result.IdToken);
	Console.ResetColor();

	if (result != null)
	{
		// Create a new instance of the ServiceStack JsonHttpClient, but you could use youre own favorite Http client
		var jsonHttpClient = new JsonHttpClient(config.DigiOfficeAPIGatewayUrl);

		var authRequest = new Authenticate
		{
			provider = "msalauth",
			oauth_token = result.IdToken   // Do not confuse with result.AccessToken !!
		};

		Console.WriteLine("Trying to authenticate on DigiOffice with the ROPC ID token");

		// Authenticate with the DigiOffice Web API (daemon)
		var authResponse = jsonHttpClient.Post<AuthenticateResponse>(authRequest);

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Bearer Token of DigiOffice acquired: " + authResponse.BearerToken);
		Console.ResetColor();

		// Add bearer token to default request headers.
		jsonHttpClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.BearerToken);

		var reqProjects = new GetProjects
		{
			PageSize = 10, // get 10 projects
			PageNumber = 1 // get page 1
		};

		Console.WriteLine("Calling GetProjects of DigiOffice API");
		// Get projects from the DigiOffice Web API (daemon)
		var resProjects = jsonHttpClient.Post(reqProjects);

		// Display projects
		foreach (var project in resProjects)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{project.ID}\t{project.Name}");
			Console.ResetColor();
		}
	}
}
catch (Exception ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("Exception: " + ex.Message);
	Console.ResetColor();
}