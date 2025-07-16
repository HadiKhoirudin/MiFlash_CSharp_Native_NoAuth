using System.Diagnostics;

namespace XiaoMiFlash.code.Utility;

public class MiCmdProcess : Process
{
	public new void Kill()
	{
		Log.w(base.ProcessName, "kill process:" + base.ProcessName);
		Log.w("kill process:" + base.ProcessName);
		base.Kill();
	}
}
