using Primer.AppInits;

namespace Primer.Tests.Unit.Initializers;

public class TestInitLow : IAppInitializer
{
	public RegistrationPriority GetPriority() => 
		RegistrationPriority.Low;

	public void ConfigureBuilder(WebApplicationBuilder builder) => 
		TestExecutionLog.Entries.Add("Low:Builder");

	public void ConfigureApp(WebApplication app) => 
		TestExecutionLog.Entries.Add("Low:App");
}
