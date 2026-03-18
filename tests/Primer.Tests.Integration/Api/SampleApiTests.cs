using System.Net;
using System.Text.Json;

namespace Primer.Tests.Integration.Api;

[TestFixture]
[Category("Integration")]
public class SampleApiTests : SampleAppTestBase
{
	[TestCase("World")]
	[TestCase("Primer")]
	public async Task Hello_ReturnsGreetingContainingName(string name)
	{
		var response = await Client.GetAsync($"/api/sample/hello?name={name}");
		
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadAsStringAsync();
		
		Assert.That(content, Does.StartWith($"Hello, {name}!"));
	}
	
	[Test]
	public async Task Hello_DefaultName_ReturnsWorld()
	{
		var response = await Client.GetAsync("/api/sample/hello");
		
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadAsStringAsync();
		
		Assert.That(content, Does.StartWith("Hello, World!"));
	}
	
	[Test]
	public async Task Info_ReturnsJsonWithExpectedFields()
	{
		var response = await Client.GetAsync("/api/sample/info");
		
		response.EnsureSuccessStatusCode();
		Assert.That(response.Content.Headers.ContentType?.ToString(), Contains.Substring("application/json"));
		
		var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
		var root = json.RootElement;
		
		Assert.That(root.GetProperty("app").GetString(), Is.EqualTo("Primer.Samples.Standard"));
		Assert.That(root.TryGetProperty("environment", out _), Is.True);
		Assert.That(root.TryGetProperty("time", out _), Is.True);
	}
	
	[Test]
	public async Task NonExistentRoute_Returns404()
	{
		var response = await Client.GetAsync("/api/does-not-exist");
		
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}
}
