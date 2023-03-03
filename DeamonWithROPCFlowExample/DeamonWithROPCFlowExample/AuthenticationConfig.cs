using System.Globalization;

public class AuthenticationConfig
{
	/// <summary>
	/// Instance of Azure AD
	/// </summary>
	public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";

	/// <summary>
	/// DigiOffice API Services App Url
	/// </summary>
	public string ApiUrl { get; set; } = string.Empty;

	/// <summary>
	/// The Tenant is:
	/// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
	/// or a domain name associated with the tenant
	/// - or 'organizations' (for a multi-tenant application)
	/// </summary>
	public string TenantID { get; set; } = string.Empty;

	/// <summary>
	/// Guid used by the application to uniquely identify itself to Azure AD
	/// </summary>
	public string ClientID { get; set; } = string.Empty;

	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	/// <summary>
	/// URL of the authority
	/// </summary>
	public string Authority => string.Format(CultureInfo.InvariantCulture, Instance, TenantID);
	
	public string DigiOfficeAPIGatewayUrl { get; set; } = string.Empty;
	public string[] Scopes = new string[] { $"user.read" };
}