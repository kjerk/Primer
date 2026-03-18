using Microsoft.AspNetCore.CookiePolicy;
using Primer.AppInits;

namespace Primer.Samples.Standard.AppInits;

public class AppInitCookiePolicy : IAppInitializer
{
	public RegistrationPriority GetPriority() => RegistrationPriority.Medium;
	
	public void ConfigureBuilder(WebApplicationBuilder builder)
	{
		builder.Services.Configure<CookiePolicyOptions>(options => {
			options.Secure = CookieSecurePolicy.Always;
			options.HttpOnly = HttpOnlyPolicy.Always;
			options.MinimumSameSitePolicy = SameSiteMode.Strict;
		});
	}
	
	public void ConfigureApp(WebApplication app)
	{
		app.UseCookiePolicy();
	}
}
