using Microsoft.AspNetCore.Mvc;
using Primer.Samples.Standard.Services;

namespace Primer.Samples.Standard.Controllers;

[ApiController]
[Route("api/sample")]
public class SampleController : ControllerBase
{
	private readonly IGreetingService _greetingService;
	private readonly IWebHostEnvironment _env;
	
	public SampleController(IGreetingService greetingService, IWebHostEnvironment env)
	{
		_greetingService = greetingService;
		_env = env;
	}
	
	[HttpGet("hello")]
	public IActionResult Hello([FromQuery] string name = "World")
	{
		return Ok(_greetingService.Greet(name));
	}
	
	[HttpGet("info")]
	public IActionResult Info()
	{
		return Ok(new {
			app = "Primer.Samples.Standard",
			environment = _env.EnvironmentName,
			time = DateTime.UtcNow
		});
	}
}
