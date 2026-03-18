using Microsoft.Extensions.DependencyInjection;

namespace Primer.Attribs;

// TODO: Not implemented yet.
[AttributeUsage(AttributeTargets.Class)]
internal class ServiceLifetimeAttribute : Attribute
{
	public ServiceLifetime Lifetime { get; }
	
	public ServiceLifetimeAttribute(ServiceLifetime lifetime)
	{
		Lifetime = lifetime;
	}
}
