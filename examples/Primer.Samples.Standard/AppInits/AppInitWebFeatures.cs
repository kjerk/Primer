using Primer.AppInits;

namespace Primer.Samples.Standard.AppInits;

public class AppInitWebFeatures : IAppInitializer
{
	public RegistrationPriority GetPriority() => RegistrationPriority.Medium;
	
	public void ConfigureBuilder(WebApplicationBuilder builder)
	{
		builder.Services.AddControllers();
		builder.Services.AddRazorPages();
		
		builder.Services.AddRouting(options => {
			options.LowercaseUrls = true;
		});
	}
	
	public void ConfigureApp(WebApplication app)
	{
		// app.UseHttpsRedirection();
		app.UseDefaultFiles();
		app.UseStaticFiles();
		app.UseRouting();
		
		app.MapControllers();
		app.MapRazorPages().WithStaticAssets();
	}
}
