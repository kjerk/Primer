using Primer.AppInits;

namespace Primer.Tests.Unit.Initializers;

public class TestInitHigh : IAppInitializer
{
	public RegistrationPriority GetPriority() => 
		RegistrationPriority.High;

	public void ConfigureBuilder(WebApplicationBuilder builder) => 
		TestExecutionLog.Entries.Add("High:Builder");

	public void ConfigureApp(WebApplication app) => 
		TestExecutionLog.Entries.Add("High:App");
}
