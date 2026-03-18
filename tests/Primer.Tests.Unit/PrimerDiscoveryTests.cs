namespace Primer.Tests.Unit;

[TestFixture]
public class PrimerDiscoveryTests
{
	[SetUp]
	public void SetUp()
	{
		TestExecutionLog.Clear();
		Primer.TargetAssembly = typeof(PrimerDiscoveryTests).Assembly;
		Primer.TestMode = false;
	}
	
	[TearDown]
	public void TearDown()
	{
		TestExecutionLog.Clear();
		Primer.TargetAssembly = null;
		Primer.TestMode = false;
	}
	
	[Test]
	public void DiscoversAllEnabledInitializers()
	{
		var builder = WebApplication.CreateBuilder();
		Primer.ApplyBuilderConfigs(builder);
		
		Assert.That(TestExecutionLog.Entries, Has.Count.EqualTo(3));
	}
	
	[Test]
	public void ExecutesInPriorityOrder()
	{
		var builder = WebApplication.CreateBuilder();
		Primer.ApplyBuilderConfigs(builder);
		
		Assert.That(TestExecutionLog.Entries[0], Is.EqualTo("High:Builder"));
		Assert.That(TestExecutionLog.Entries[1], Is.EqualTo("Medium:Builder"));
		Assert.That(TestExecutionLog.Entries[2], Is.EqualTo("Low:Builder"));
	}
	
	[Test]
	public void AppConfigsReusesSameInstances_InCorrectOrder()
	{
		var builder = WebApplication.CreateBuilder();
		Primer.ApplyBuilderConfigs(builder);
		var app = builder.Build();
		Primer.ApplyAppConfigs(app);
		
		Assert.That(TestExecutionLog.Entries, Has.Count.EqualTo(6));
		Assert.That(TestExecutionLog.Entries[0], Is.EqualTo("High:Builder"));
		Assert.That(TestExecutionLog.Entries[1], Is.EqualTo("Medium:Builder"));
		Assert.That(TestExecutionLog.Entries[2], Is.EqualTo("Low:Builder"));
		Assert.That(TestExecutionLog.Entries[3], Is.EqualTo("High:App"));
		Assert.That(TestExecutionLog.Entries[4], Is.EqualTo("Medium:App"));
		Assert.That(TestExecutionLog.Entries[5], Is.EqualTo("Low:App"));
	}
	
	[Test]
	public void DisabledInitializerIsExcluded()
	{
		var builder = WebApplication.CreateBuilder();
		Primer.ApplyBuilderConfigs(builder);
		var app = builder.Build();
		Primer.ApplyAppConfigs(app);
		
		Assert.That(TestExecutionLog.Entries, Has.None.Contains("Disabled"));
	}
}
