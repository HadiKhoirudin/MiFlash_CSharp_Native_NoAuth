using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XiaoMiFlash.code.Utility;

internal class IniFile
{
	private string strIniFilePath;

	[DllImport("kernel32", CharSet = CharSet.Auto)]
	private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

	[DllImport("kernel32", CharSet = CharSet.Auto)]
	private static extern long GetPrivateProfileString(string section, string key, string strDefault, StringBuilder retVal, int size, string filePath);

	[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
	public static extern int GetPrivateProfileInt(string section, string key, int nDefault, string filePath);

	public IniFile()
	{
		string applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
		string text = applicationBase.Substring(0, applicationBase.IndexOf('\\') + 1);
		strIniFilePath = text + "MiFlashvcom.ini";
		if (!File.Exists(strIniFilePath))
		{
			using (FileStream fileStream = File.Create(strIniFilePath))
			{
				fileStream.Close();
			}
		}
	}

	public bool GetIniString(string section, string key, string strDefault, StringBuilder retVal, int size)
	{
		return GetPrivateProfileString(section, key, strDefault, retVal, size, strIniFilePath) >= 1;
	}

	public int GetIniInt(string section, string key, int nDefault)
	{
		return GetPrivateProfileInt(section, key, nDefault, strIniFilePath);
	}

	public bool WriteIniString(string section, string key, string val)
	{
		return WritePrivateProfileString(section, key, val, strIniFilePath) != 0;
	}

	public bool WriteIniInt(string section, string key, int val)
	{
		return WriteIniString(section, key, val.ToString());
	}

	public static string GetValue(string Section, string Key, string defaultText, string iniFilePath)
	{
		if (File.Exists(iniFilePath))
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			GetPrivateProfileString(Section, Key, defaultText, stringBuilder, 1024, iniFilePath);
			return stringBuilder.ToString();
		}
		return defaultText;
	}

	public static bool SetValue(string Section, string Key, string Value, string iniFilePath)
	{
		string directoryName = Path.GetDirectoryName(iniFilePath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if (!File.Exists(iniFilePath))
		{
			File.Create(iniFilePath).Close();
		}
		if (WritePrivateProfileString(Section, Key, Value, iniFilePath) == 0L)
		{
			return false;
		}
		return true;
	}
}
