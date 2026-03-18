using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Primer.Tests;

[TestFixture]
public abstract class PrimerBrowserTestBase<TProgram> : PageTest where TProgram : class
{
	private WebApplication _app;
	protected IServiceProvider Services => _app!.Services;
	
	// Must be implemented by derived classes to configure the WebApplicationBuilder
	protected abstract void ConfigureBuilder(WebApplicationBuilder builder);
	protected abstract void ConfigureApp(WebApplication app);
	
	protected string ToTestUrl(string path)
	{
		var mainPort = _app.Urls.First().Split(":")[2].Split('/')[0];
		if (string.IsNullOrEmpty(path)) {
			// Everything runs on localhost, what are you, a monster?
			return $"http://127.0.0.1:{mainPort}/";
		}
		
		if (path.StartsWith("/")) {
			path = path.Substring(1);
		}
		
		return $"http://127.0.0.1:{mainPort}/{path}";
	}
	
	[OneTimeSetUp]
	public async Task SetUp()
	{
		var contentRoot = GetContentRoot();
		
		var targetAssembly = typeof(TProgram).Assembly;
		
		Primer.TestMode = true;
		Primer.TestBuilderConfig = ConfigureBuilder;
		Primer.TestAppConfig = ConfigureApp;
		Primer.TargetAssembly = targetAssembly;
		
		try {
			var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
				ContentRootPath = contentRoot,
				WebRootPath = contentRoot,
				EnvironmentName = "Testing",
				ApplicationName = targetAssembly.GetName().Name
			});
			
			// Do not use UseTestServer here, as it is not compatible with Playwright tests.
			
			Primer.ApplyBuilderConfigs(builder);
			_app = builder.Build();
			Primer.ApplyAppConfigs(_app);
			
			await _app.StartAsync();
		} catch {
			Primer.TestMode = false;
			Primer.TestBuilderConfig = null;
			Primer.TestAppConfig = null;
			throw;
		}
	}
	
	[OneTimeTearDown]
	public async Task TearDown()
	{
		if (_app != null) {
			await _app.StopAsync();
			await _app.DisposeAsync();
		}
		
		Primer.TestMode = false;
		Primer.TestBuilderConfig = null;
		Primer.TestAppConfig = null;
	}
	
	private string GetContentRoot()
	{
		var assemblyPath = typeof(TProgram).Assembly.Location;
		var directory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath)!);
		
		// Just walk up until we find a .csproj file
		while(directory != null && !directory.GetFiles("*.csproj").Any()) {
			directory = directory.Parent;
		}
		
		if (directory == null) {
			throw new InvalidOperationException($"Could not find project root from {assemblyPath}. Make sure you're running tests from a standard project structure.");
		}
		
		return directory.FullName;
	}
}
