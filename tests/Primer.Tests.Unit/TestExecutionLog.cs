namespace Primer.Tests.Unit;

public static class TestExecutionLog
{
	public static List<string> Entries { get; } = new();
	
	public static void Clear() => Entries.Clear();
}
