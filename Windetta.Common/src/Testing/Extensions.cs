using System.Diagnostics;
using Windetta.Common.Configuration;

namespace Windetta.Common.Testing;

public static class WindettaDebugger
{
    public static void AttachDebuggerIfPassedCorrespondingEnv()
    {
        if (EnvVars.AskDebuggingAttachEnabled)
            Debugger.Launch();
    }
}