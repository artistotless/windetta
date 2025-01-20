namespace Windetta.Common.Configuration;

public class EnvVars
{
    public static bool FakeAuthEnabled => Environment.GetEnvironmentVariable("FAKE_AUTH") == "Enabled";
    public static bool AskDebuggingAttachEnabled => Environment.GetEnvironmentVariable("ASK_DEBUGGING_ATTACH") == "Yes";
    public static string? InstanceID => Environment.GetEnvironmentVariable("InstanceId");
}
