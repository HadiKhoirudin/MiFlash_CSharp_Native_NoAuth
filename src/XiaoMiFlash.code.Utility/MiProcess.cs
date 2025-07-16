using System;
using System.Diagnostics;
using System.Linq;

namespace XiaoMiFlash.code.Utility;

public class MiProcess
{
	public static void KillProcess(string processName)
	{
		try
		{
			foreach (Process item in Process.GetProcessesByName(processName).ToList())
			{
				item.Kill();
				item.Dispose();
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}
}
