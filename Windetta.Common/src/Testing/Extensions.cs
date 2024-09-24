using System.Diagnostics;

namespace Windetta.Common.Testing;

public static class WindettaDebugger
{
    public static void AttachDebuggerIfPassedCorrespondingEnv()
    {
        var shouldAttachDebugger = Environment.GetEnvironmentVariable("ASK_DEBUGGING_ATTACH") == "Yes";

        if (shouldAttachDebugger)
            Debugger.Launch();
    }
}