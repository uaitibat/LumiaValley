using System;
using System.Diagnostics;
using System.Text;

internal static class BashRunner
{
	public static string Run(string commandLine)
	{
		StringBuilder errorBuilder = new StringBuilder();
		StringBuilder outputBuilder = new StringBuilder();
		string text = "-c \"" + commandLine + "\"";
		using Process process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "bash",
				Arguments = text,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false
			}
		};
		process.Start();
		process.OutputDataReceived += delegate(object _, DataReceivedEventArgs args)
		{
			outputBuilder.AppendLine(args.Data);
		};
		process.BeginOutputReadLine();
		process.ErrorDataReceived += delegate(object _, DataReceivedEventArgs args)
		{
			errorBuilder.AppendLine(args.Data);
		};
		process.BeginErrorReadLine();
		if (!process.DoubleWaitForExit())
		{
			string message = $"Process timed out. Command line: bash {text}.\nOutput: {outputBuilder}\nError: {errorBuilder}";
			throw new Exception(message);
		}
		if (process.ExitCode == 0)
		{
			return outputBuilder.ToString();
		}
		string message2 = $"Could not execute process. Command line: bash {text}.\nOutput: {outputBuilder}\nError: {errorBuilder}";
		throw new Exception(message2);
	}

	private static bool DoubleWaitForExit(this Process process)
	{
		bool flag = process.WaitForExit(500);
		if (flag)
		{
			process.WaitForExit();
		}
		return flag;
	}
}
