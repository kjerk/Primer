# Primer

**Stop fighting merge conflicts in `Program.cs`.**

Primer discovers your startup configuration classes automatically and runs them in priority order. Each one is a small, focused file. Your `Program.cs` stays minimal, your configuration stays organized, and adding a new service never means resolving a three-way merge in a 200-line startup file.

## What It Looks Like

Your `Program.cs` becomes this and then you (hopefully) never touch it again:

```csharp
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
```

Your actual configuration lives in self-contained files:

```
YourApp/
  AppInits/
  ├── AppInitRequestLimits.cs      // Priority: High — Kestrel limits, security headers
  ├── AppInitDatabase.cs           // Priority: Medium — EF Core setup, migrations
  ├── AppInitCookiePolicy.cs       // Priority: Medium — Cookie security
  ├── AppInitWebFeatures.cs        // Priority: Medium — MVC, routing, static files
  ├── AppInitServices.cs           // Priority: Low — Business service registration
  └── AppInitRateLimiting.cs       // Priority: Low — Rate limit policies
```

Need to understand rate limiting? Open `AppInitRateLimiting.cs`. Database config? `AppInitDatabase.cs`. No scrolling, no guessing which block of middleware belongs to what.

## Install

```
dotnet add package Primer
```

Targets net8.0, net9.0, and net10.0. Zero external dependencies.

## Writing an Initializer

Implement `IAppInitializer`. Primer finds it automatically — no registration, no attribute wiring, nothing.

```csharp
using Primer.AppInits;

public class AppInitDatabase : IAppInitializer
{
    public RegistrationPriority GetPriority() => RegistrationPriority.Medium;

    public void ConfigureBuilder(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
    }

    public void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment()) {
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        }
    }
}
```

`ConfigureBuilder` runs during service registration. `ConfigureApp` runs after the app is built — middleware, endpoints, anything that needs a built container. Both methods are called on every initializer, in priority order. The same instances are used for both phases.

## Priority

Higher values run first. The enum gives you named tiers:

```csharp
public enum RegistrationPriority
{
    Highest = 30,    // Request constraints, fundamental limits
    High = 25,       // Core infrastructure
    MediumHigh = 22,
    Medium = 20,     // Database, caching, web features
    Low = 15,        // Business services
    Lowest = 10      // Optional features
}
```

Need something between tiers? Cast any integer: `(RegistrationPriority)18`. The enum is a convenience — it's the number that determines order.

## Disabling an Initializer

Mark a class with `[PrimerDisabled]` to exclude it from discovery without deleting it:

```csharp
[PrimerDisabled]
public class AppInitExperimental : IAppInitializer { ... }
```

Useful for feature-flagging during development or temporarily pulling something out while debugging.

## Testing

Primer ships with a companion package for integration testing:

```
dotnet add package Primer.Tests.Base
```

The key idea: test mode bypasses auto-discovery entirely. You choose exactly which initializers to wire up per test fixture — no surprise middleware, no ambient configuration leaking between tests.

`PrimerTestBase<TProgram>` handles the rest of the painful parts: finding the content root, managing the test server lifecycle, and wiring up Primer's test mode. You just override what to configure:

```csharp
public abstract class MyAppTestBase : PrimerTestBase<Program>
{
    protected override void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase($"Test_{Guid.CreateVersion7()}"));

        new AppInitWebFeatures().ConfigureBuilder(builder);
        new AppInitServices().ConfigureBuilder(builder);
    }

    protected override void ConfigureApp(WebApplication app)
    {
        new AppInitWebFeatures().ConfigureApp(app);
        new AppInitServices().ConfigureApp(app);
    }
}
```

Then write tests against it:

```csharp
[TestFixture]
[Category("Integration")]
public class ApiTests : MyAppTestBase
{
    [Test]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await Client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
    }
}
```

`Client` is an `HttpClient` wired to the test server. `Services` gives you the app's `IServiceProvider`.

## PrimerCustom

The standard `IAppInitializer` covers most cases. But if you have your own initializer interface — maybe one that's async, or carries additional context — `PrimerCustom` lets you use Primer's discovery engine with any type:

```csharp
using Primer.Advanced;

public interface IPluginInitializer
{
    int Order { get; }
    void Register(WebApplicationBuilder builder, PluginContext ctx);
}

var ctx = new PluginContext();
PrimerCustom.ApplyBuilderConfigs<IPluginInitializer>(
    builder,
    configAction: (plugin, b) => plugin.Register(b, ctx),
    prioritySelector: plugin => plugin.Order
);
```

`PrimerCustom` discovers all concrete classes implementing your type, instantiates them, and calls your action delegate in priority order. The priority selector is optional — omit it if ordering doesn't matter.

## How It Works

There's no magic. Primer scans your assembly for classes implementing `IAppInitializer`, filters out anything marked `[PrimerDisabled]`, instantiates them via `Activator.CreateInstance`, deduplicates by type, sorts by priority descending, and calls each one. The discovery runs once at startup and the result is cached across both phases. The runtime cost is negligible — you're reflecting over your own assembly to find a handful of classes. After that, it's just method calls.

## When Not to Use Primer

- Your app has a 20-line `Program.cs` and that's fine. Not everything needs modularity.
- Your team prefers singular centralized registration.
- You're not cool enough.
- You need async initialization. `IAppInitializer` methods are synchronous. (Though `PrimerCustom` can work around this with a custom interface.)

## Sample App

The repository includes a working example at [`examples/Primer.Samples.Standard`](examples/Primer.Samples.Standard/) with four initializers covering request limits, cookie policy, routing, and service registration. Clone it, `dotnet run`, and browse to `http://localhost:5100`.

---

MIT License
