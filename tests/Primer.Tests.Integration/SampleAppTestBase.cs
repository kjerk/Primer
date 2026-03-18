using Primer.Samples.Standard.AppInits;

namespace Primer.Tests.Integration;

public abstract class SampleAppTestBase : PrimerTestBase<Program>
{
	protected override void ConfigureBuilder(WebApplicationBuilder builder)
	{
		new AppInitWebFeatures().ConfigureBuilder(builder);
		new AppInitServices().ConfigureBuilder(builder);
	}
	
	protected override void ConfigureApp(WebApplication app)
	{
		new AppInitWebFeatures().ConfigureApp(app);
		new AppInitServices().ConfigureApp(app);
	}
}
