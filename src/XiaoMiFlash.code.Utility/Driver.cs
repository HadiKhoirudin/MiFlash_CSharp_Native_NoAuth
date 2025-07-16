using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XiaoMiFlash.code.Utility;

public class Driver
{
	[DllImport("setupapi.dll", SetLastError = true)]
	private static extern bool SetupCopyOEMInf(string SourceInfFileName, string OEMSourceMediaLocation, OemSourceMediaType OEMSourceMediaType, OemCopyStyle CopyStyle, StringBuilder DestinationInfFileName, int DestinationInfFileNameSize, int RequiredSize, out string DestinationInfFileNameComponent);

	[DllImport("setupapi.dll", SetLastError = true)]
	private static extern bool SetupUninstallOEMInf(string InfFileName, SetupUOInfFlags Flags, IntPtr Reserved);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern uint GetWindowsDirectory(StringBuilder path, int pathLen);

	public static string SetupOEMInf(string infPath, out string destinationInfFileName, out string destinationInfFileNameComponent, out bool success)
	{
		string result = "";
		StringBuilder stringBuilder = new StringBuilder(260);
		success = SetupCopyOEMInf(infPath, "", OemSourceMediaType.SPOST_PATH, OemCopyStyle.SP_COPY_NEWER, stringBuilder, stringBuilder.Capacity, 0, out destinationInfFileNameComponent);
		if (!success)
		{
			result = new Win32Exception(Marshal.GetLastWin32Error()).Message;
		}
		destinationInfFileName = stringBuilder.ToString();
		return result;
	}

	public static string UninstallInf(string infFileName, out bool success)
	{
		string result = "";
		success = SetupUninstallOEMInf(infFileName, SetupUOInfFlags.SUOI_FORCEDELETE, IntPtr.Zero);
		if (!success)
		{
			result = new Win32Exception(Marshal.GetLastWin32Error()).Message;
		}
		return result;
	}

	public static string UninstallInfByText(string text, out bool success)
	{
		success = false;
		StringBuilder stringBuilder = new StringBuilder(256);
		if (GetWindowsDirectory(stringBuilder, stringBuilder.Capacity) == 0)
		{
			return "UninstallInfByText: GetWindowsDirectory failed with system error code " + Marshal.GetLastWin32Error();
		}
		string[] files = Directory.GetFiles(stringBuilder.ToString() + "\\inf", "*.inf");
		string text2 = "";
		string[] array = files;
		foreach (string text3 in array)
		{
			if (File.ReadAllText(text3).Contains(text))
			{
				string text4 = text3.Remove(0, text3.LastIndexOf('\\') + 1);
				if (!SetupUninstallOEMInf(text4, SetupUOInfFlags.SUOI_FORCEDELETE, IntPtr.Zero))
				{
					text2 = text2 + "UninstallInfByText: SetupUninstallOEMInf failed with code " + Marshal.GetLastWin32Error() + " for file " + text4;
				}
				else
				{
					success = true;
				}
			}
		}
		if (text2.Length > 0)
		{
			return text2;
		}
		return null;
	}
}
