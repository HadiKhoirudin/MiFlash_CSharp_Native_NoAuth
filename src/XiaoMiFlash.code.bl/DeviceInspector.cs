using System.Text.RegularExpressions;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class DeviceInspector
{
	public static bool checkLockStatus(string device, out string msg)
	{
		msg = "";
		Cmd cmd = new Cmd(device, "");
		string dosCommand = "fastboot -s " + device + " getvar token";
		string input = cmd.Execute(device, dosCommand);
		string[] array = Regex.Split(input, "\\r\\n");
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			string text2 = "token:";
			if (text.IndexOf("token:") >= 0)
			{
				if (!(text.Trim() == text2))
				{
					break;
				}
				msg = "token is null.";
				return false;
			}
		}
		dosCommand = "fastboot -s " + device + " oem device-info";
		input = cmd.Execute(device, dosCommand);
		string[] obj = new string[2] { "Device unlocked: false", " Device critical unlocked: false" };
		bool flag = true;
		array = obj;
		foreach (string value in array)
		{
			flag = flag && input.IndexOf(value) >= 0;
			if (!flag)
			{
				break;
			}
		}
		if (!flag)
		{
			msg = "device is unlocked!";
		}
		return flag;
	}
}
