using System.Reflection;
using Primer.Tools;

namespace Primer.Advanced;

public static class PrimerCustom
{
	// Allows for testing without executing the auto-discovered initializers.
	public static bool TestMode { get; set; } = false;
	public static Action<WebApplicationBuilder> TestBuilderConfig { get; set; }
	public static Action<WebApplication> TestAppConfig { get; set; }
	
	public static Assembly TargetAssembly { get; set; } = null;
	
	public static void ApplyBuilderConfigs<T>(WebApplicationBuilder builder, Action<T, WebApplicationBuilder> configAction, Func<T, int> prioritySelector = null) where T : class
	{
		if (TestMode) {
			TestBuilderConfig?.Invoke(builder);
			return;
		}
		
		if (TargetAssembly == null)
			TargetAssembly = Assembly.GetCallingAssembly();
		
		foreach (var initializer in AutoCreateAppInits(prioritySelector)) {
			configAction(initializer, builder);
		}
	}
	
	public static void ApplyAppConfigs<T>(WebApplication app, Action<T, WebApplication> configAction, Func<T, int> prioritySelector = null) where T : class
	{
		if (TestMode) {
			TestAppConfig?.Invoke(app);
			return;
		}
		
		if (TargetAssembly == null) {
			throw new InvalidOperationException(
				"Primer.ApplyBuilderConfigs must be called before Primer.ApplyAppConfigs. " +
				"The typical pattern is:\n" +
				"  Primer.ApplyBuilderConfigs(builder);\n" +
				"  var app = builder.Build();\n" +
				"  Primer.ApplyAppConfigs(app);");
		}
		
		foreach (var initializer in AutoCreateAppInits(prioritySelector)) {
			configAction(initializer, app);
		}
	}
	
	private static List<T> AutoCreateAppInits<T>(Func<T, int> prioritySelector = null) where T : class
	{
		var types = PrimerReflects.DiscoverTypesImplementingThis(typeof(T), TargetAssembly);
		
		var instances = types.Select(t => Activator.CreateInstance(t) as T)
			.Where(b => b != null)
			.DistinctBy(b => b.GetType());
		
		if (prioritySelector != null)
			instances = instances.OrderByDescending(prioritySelector);
		
		return instances.ToList();
	}
}
