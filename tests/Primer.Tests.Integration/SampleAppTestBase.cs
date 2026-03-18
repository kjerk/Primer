using Primer.Samples.Standard;
using Primer.Samples.Standard.AppInits;

namespace Primer.Tests.Integration;

public abstract class SampleAppTestBase : PrimerTestBase<Program>
{
	protected override void ConfigureBuilder(WebApplicationBuilder builder)
	{
		// Borrowing from Primer.Samples.Standard , any mocked or other impl here.
		new AppInitWebFeatures().ConfigureBuilder(builder);
		new AppInitServices().ConfigureBuilder(builder);
	}
	
	protected override void ConfigureApp(WebApplication app)
	{
		// Borrowing from Primer.Samples.Standard , any mocked or other impl here.
		new AppInitWebFeatures().ConfigureApp(app);
		new AppInitServices().ConfigureApp(app);
	}
}
