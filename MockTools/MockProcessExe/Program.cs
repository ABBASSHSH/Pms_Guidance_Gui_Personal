using System.Globalization;

var waitMs = 5000;
var outcome = "pass";
var mode = "generic";
int? forcedExitCode = null;

for (var i = 0; i < args.Length; i++)
{
    var arg = args[i];

    if ((arg == "--wait-ms" || arg == "-w") && i + 1 < args.Length)
    {
        if (int.TryParse(args[++i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) && parsed >= 0)
        {
            waitMs = parsed;
        }
        continue;
    }

    if ((arg == "--result" || arg == "-r") && i + 1 < args.Length)
    {
        outcome = args[++i].Trim().ToLowerInvariant();
        continue;
    }

    if ((arg == "--mode" || arg == "-m") && i + 1 < args.Length)
    {
        mode = args[++i].Trim();
        continue;
    }

    if ((arg == "--exit-code" || arg == "-e") && i + 1 < args.Length)
    {
        if (int.TryParse(args[++i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedCode))
        {
            forcedExitCode = parsedCode;
        }
    }
}

Console.WriteLine($"[MockProcessExe] Mode={mode}; WaitMs={waitMs}; Result={outcome}");
Thread.Sleep(waitMs);

if (forcedExitCode.HasValue)
{
    Console.WriteLine($"[MockProcessExe] Returning forced exit code {forcedExitCode.Value}");
    return forcedExitCode.Value;
}

var exitCode = outcome switch
{
    "pass" => 1,
    "success" => 1,
    "1" => 1,
    "fail" => 0,
    "failed" => 0,
    "0" => 0,
    _ => 0,
};

Console.WriteLine($"[MockProcessExe] Returning exit code {exitCode}");
return exitCode;
