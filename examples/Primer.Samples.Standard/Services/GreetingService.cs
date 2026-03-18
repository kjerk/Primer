namespace Primer.Samples.Standard.Services;

public class GreetingService : IGreetingService
{
	public string Greet(string name)
	{
		return $"Hello, {name}! Primer.Samples.Standard is running.";
	}
}
