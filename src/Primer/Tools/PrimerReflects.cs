using System.Reflection;

namespace Primer.Tools;

public static class PrimerReflects
{
	public static List<Type> DiscoverTypesImplementingThis(Type thisInterface, Assembly inThisAssembly = null)
	{
		if (!thisInterface.IsInterface)
			throw new ArgumentException($"The type '{thisInterface.FullName}' is not an interface type.", nameof(thisInterface));
		
		inThisAssembly ??= Assembly.GetCallingAssembly();
		
		return inThisAssembly.GetTypes()
			.Where(t => thisInterface.IsAssignableFrom(t) && IsNormalClass(t))
			.Distinct()
			.ToList();
	}
	
	public static bool IsNormalClass(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, nameof(type));
		
		if (typeof(Attribute).IsAssignableFrom(type))
			return false;
		
		return type.IsClass && !type.IsInterface && !type.IsAbstract && !type.IsGenericTypeDefinition;
	}
}
