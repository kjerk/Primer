using Primer.AppInits;
using Primer.Attribs;

namespace Primer.Tests.Unit.Initializers;

[PrimerDisabled]
public class TestInitDisabled : IAppInitializer
{
	public RegistrationPriority GetPriority() => 
		RegistrationPriority.Medium;
	
	public void ConfigureBuilder(WebApplicationBuilder builder) =>
		TestExecutionLog.Entries.Add("Disabled:Builder");
	
	public void ConfigureApp(WebApplication app) => 
		TestExecutionLog.Entries.Add("Disabled:App");
}
