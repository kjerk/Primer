using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace Primer.Tests;

public abstract class PrimerTestBase<TProgram> : IDisposable where TProgram : class
{
	private WebApplication _app;
	private HttpClient _client;
	
	protected HttpClient Client => _client!;
	protected IServiceProvider Services => _app!.Services;
	
	// Must be implemented by derived classes to configure the WebApplicationBuilder
	protected abstract void ConfigureBuilder(WebApplicationBuilder builder);
	protected abstract void ConfigureApp(WebApplication app);
	
	[SetUp]
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
				EnvironmentName = "Testing",
				ApplicationName = targetAssembly.GetName().Name
			});
			
			builder.WebHost.UseTestServer();
			
			Primer.ApplyBuilderConfigs(builder);
			
			_app = builder.Build();
			Primer.ApplyAppConfigs(_app);
			
			await _app.StartAsync();
			
			var testServer = (TestServer)_app.Services.GetRequiredService<IServer>();
			_client = testServer.CreateClient();
		} catch {
			Primer.TestMode = false;
			Primer.TestBuilderConfig = null;
			Primer.TestAppConfig = null;
			throw;
		}
	}
	
	[TearDown]
	public async Task TearDown()
	{
		_client?.Dispose();
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
	
	public void Dispose()
	{
		_client?.Dispose();
		_app?.DisposeAsync().AsTask().Wait();
	}
}
