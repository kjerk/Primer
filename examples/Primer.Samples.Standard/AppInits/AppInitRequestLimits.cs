using Microsoft.AspNetCore.Server.Kestrel.Core;
using Primer.AppInits;

namespace Primer.Samples.Standard.AppInits;

public class AppInitRequestLimits : IAppInitializer
{
	public RegistrationPriority GetPriority() => RegistrationPriority.High;
	
	// 10MB
	private const long MaxRequestBodySize = 1024 * 1024 * 1024;

	public void ConfigureBuilder(WebApplicationBuilder builder)
	{
		builder.Services.Configure<KestrelServerOptions>(options => {
			options.Limits.MaxRequestBodySize = MaxRequestBodySize;
		});
	}

	public void ConfigureApp(WebApplication app)
	{
		app.Use(async (context, next) => {
			context.Response.Headers.XContentTypeOptions = "nosniff";
			context.Response.Headers.XFrameOptions = "DENY";
			context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
			await next();
		});
	}
}
