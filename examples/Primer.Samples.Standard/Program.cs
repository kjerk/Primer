namespace Primer.Samples.Standard;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		Primer.ApplyBuilderConfigs(builder);
		
		var app = builder.Build();
		Primer.ApplyAppConfigs(app);
		
		app.Run();
	}
}
