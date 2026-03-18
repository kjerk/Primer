using Primer.AppInits;

namespace Primer.Tests.Unit.Initializers;

public class TestInitMedium : IAppInitializer
{
	public RegistrationPriority GetPriority() => 
		RegistrationPriority.Medium;

	public void ConfigureBuilder(WebApplicationBuilder builder) => 
		TestExecutionLog.Entries.Add("Medium:Builder");

	public void ConfigureApp(WebApplication app) => 
		TestExecutionLog.Entries.Add("Medium:App");
}
