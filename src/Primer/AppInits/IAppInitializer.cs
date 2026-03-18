namespace Primer.AppInits;

public interface IAppInitializer
{
	/// <summary>
	/// Gets initialization priority. Higher values run first.
	/// For fine-grained control, cast any integer: (RegistrationPriority)12
	/// </summary>
	public RegistrationPriority GetPriority();
	
	public void ConfigureBuilder(WebApplicationBuilder builder);
	
	public void ConfigureApp(WebApplication app);
}
