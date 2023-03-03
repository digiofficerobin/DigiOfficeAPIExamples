using System.Globalization;

public class AuthenticationConfig
{
	/// <summary>
	/// instance of Azure AD
	/// </summary>
	public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";

	/// <summary>
	/// DigiOffice API Services App Url
	/// </summary>
	public string ApiUri { get; set; } = string.Empty;

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

	/// <summary>
	/// Client secret (application password)
	/// </summary>
	/// <remarks>Daemon applications can authenticate with AAD through two mechanisms: ClientSecret
	/// (which is a kind of application password: this property)
	/// or a certificate previously shared with AzureAD during the application registration 
	/// (and identified by the CertificateName property belows)
	/// <remarks> 
	public string ClientSecret { get; set; } = string.Empty;
	/// <summary>
	/// URL of the authority
	/// </summary>
	public string Authority => string.Format(CultureInfo.InvariantCulture, Instance, TenantID);
	
	public string DigiOfficeAPIGatewayUrl { get; set; } = string.Empty;
}