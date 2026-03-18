using Primer.AppInits;
using Primer.Samples.Standard.Services;

namespace Primer.Samples.Standard.AppInits;

public class AppInitServices : IAppInitializer
{
	public RegistrationPriority GetPriority() => RegistrationPriority.Low;
	
	public void ConfigureBuilder(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IGreetingService, GreetingService>();
	}
	
	public void ConfigureApp(WebApplication app) { }
}
