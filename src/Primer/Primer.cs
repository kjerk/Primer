using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Primer.AppInits;
using Primer.Attribs;
using Primer.Tools;

namespace Primer;

public static class Primer
{
	// Allows for testing without executing the auto-discovered initializers.
	public static bool TestMode { get; set; } = false;
	public static Action<WebApplicationBuilder> TestBuilderConfig { get; set; }
	public static Action<WebApplication> TestAppConfig { get; set; }
	
	public static Assembly TargetAssembly { get; set; } = null;
	
	private static List<IAppInitializer> CachedAppInits { get; set; } = null;
	
	public static void ApplyBuilderConfigs(WebApplicationBuilder builder)
	{
		if (TestMode) {
			TestBuilderConfig?.Invoke(builder);
			return;
		}
		
		CachedAppInits = null;
		
		if (TargetAssembly == null)
			TargetAssembly = Assembly.GetCallingAssembly();
		
		foreach (var bootstrap in GetOrCreateAppInits(TargetAssembly)) {
			bootstrap.ConfigureBuilder(builder);
		}
	}
	
	public static void ApplyAppConfigs(WebApplication app)
	{
		if (TestMode) {
			TestAppConfig?.Invoke(app);
			return;
		}
		
		if (TargetAssembly == null) {
			throw new InvalidOperationException(
				"Primer.ApplyBuilderConfigs must be called before Primer.ApplyAppConfigs. \n" +
				"The typical pattern is:\n" +
				"  Primer.ApplyBuilderConfigs(builder);\n" +
				"  var app = builder.Build();\n" +
				"  Primer.ApplyAppConfigs(app);");
		}
		
		foreach (var bootstrap in GetOrCreateAppInits(TargetAssembly)) {
			bootstrap.ConfigureApp(app);
		}
		
		CachedAppInits = null;
	}
	
	private static List<IAppInitializer> GetOrCreateAppInits(Assembly assembly)
	{
		if (CachedAppInits != null)
			return CachedAppInits;
		
		var types = PrimerReflects.DiscoverTypesImplementingThis(typeof(IAppInitializer), assembly);
		
		// Filter out any types marked with [PrimerDisabled].
		types = types.Where(t => !t.IsDefined(typeof(PrimerDisabledAttribute), inherit: true)).ToList();
		
		CachedAppInits = types.Select(t => Activator.CreateInstance(t) as IAppInitializer)
			.Where(b => b != null)
			.DistinctBy(b => b.GetType())
			.OrderByDescending(b => b.GetPriority())
			.ToList();
		
		return CachedAppInits;
	}
}
