using DaemonWithClientCredentialsFlowExample.DTO;
using Microsoft.Identity.Client;
using ServiceStack;
using System.Net.Http.Headers;

// Example of how a daemon application can authenticate (using then Client Credentials auth 2.0 flow), authenticate at the DigiOffice API and get some projects 

// Client Credentials Configuration
var config = new AuthenticationConfig()
{
	ClientID = "<guid>",								// Client ID. When invalid, you get error: AADSTS700016: Application with identifier '<guid>' was not found in the directory '<tenantname>'. This can happen if the application has not been installed by the administrator of the tenant or consented to by any user in the tenant. You may have sent your authentication request to the wrong tenant.
	ClientSecret = "<secretstring>",					// Client Secret. When invalid, you get error: A configuration issue is preventing authentication - check the error message from the server for details. You can modify the configuration in the application registration portal. See https://aka.ms/msal-net-invalid-client for details.  Original exception: AADSTS7000215: Invalid client secret provided. Ensure the secret being sent in the request is the client secret value, not the client secret ID, for a secret added to app '<guid>'.
	TenantID = "<guid>",								// Tenant ID. When invalid, you get error: AADSTS90002: Tenant '<guid>' not found. Check to make sure you have the correct tenant ID and are signing into the correct cloud. Check with your subscription administrator, this may happen if there are no active subscriptions for the tenant.
	DigiOfficeAPIGatewayUrl = @"<digiofficeapigatewayurl>",  // DigiOffice API Gateway URL. When invalid, you get error: No such host is known. (<url>:443)
	ApiUri = "<appuri>"									// Resource App URI of API Services. When invalid you get error: AADSTS500011: The resource principal named <uri> was not found in the tenant named <tenantname>. This can happen if the application has not been installed by the administrator of the tenant or consented to by any user in the tenant. You might have sent your authentication request to the wrong tenant.
};

try
{
	// Build the MSAL confidential client application
	var app = ConfidentialClientApplicationBuilder.Create(config.ClientID)
	.WithClientSecret(config.ClientSecret)
	.WithAuthority(new Uri(config.Authority))
	.Build();

	// With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
	// application permissions need to be set statically (in the portal or by PowerShell), and then granted by
	// a tenant administrator. 
	var scopes = new string[] { $"{config.ApiUri}/.default" };

	Console.WriteLine("Trying to aquire token for DigiOffice API by using the Client-Credentials flow in Microsoft Azure Active Directory");

	// Acquire a access token for the Web API (daemon)
	var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("Access Token acquired: " + result.AccessToken);
	Console.ResetColor();

	if (result != null)
	{
		// Create a new instance of the ServiceStack JsonHttpClient, but you could use youre own favorite Http client
		var jsonHttpClient = new JsonHttpClient(config.DigiOfficeAPIGatewayUrl);

		var authRequest = new Authenticate
		{
			provider = "msalauth",
			oauth_token = result.AccessToken
		};

		Console.WriteLine("Trying to authenticate on DigiOffice with the Client-Credentials accesstoken");

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