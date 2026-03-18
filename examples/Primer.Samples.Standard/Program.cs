var builder = WebApplication.CreateBuilder(args);
Primer.Primer.ApplyBuilderConfigs(builder);

var app = builder.Build();
Primer.Primer.ApplyAppConfigs(app);

app.Run();
