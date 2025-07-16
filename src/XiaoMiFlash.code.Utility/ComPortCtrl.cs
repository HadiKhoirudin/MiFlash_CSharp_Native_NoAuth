using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility;

public class ComPortCtrl
{
	private static string[] getDevices()
	{
		List<string> list = new List<string>();
		try
		{
			foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\" and Name LIKE '%Qualcomm HS-USB QDLoader 9008%'").Get())
			{
				string text = item.GetPropertyValue("Name").ToString();
				string oldValue = item.GetPropertyValue("Description").ToString();
				string text2 = text.Replace(oldValue, "").Replace('(', ' ').Replace(')', ' ');
				list.Add(text2.Trim());
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
			Log.w(ex.Message);
		}
		return list.ToArray();
	}

	public static string[] getDevicesQc()
	{
		List<string> list = new List<string>();
		Regex regex = new Regex("COM\\d+");
		string qcLsUsb = Script.QcLsUsb;
		Log.w("lsusb path:" + qcLsUsb);
		if (File.Exists(qcLsUsb.Replace("\"", "")))
		{
			string text = new Cmd("", "").Execute(null, qcLsUsb);
			if (text != string.Empty)
			{
				Log.w("ls ubs :" + text);
			}
			string[] array = Regex.Split(text, "\r\n");
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]) && array[i].IndexOf("9008") > 0)
				{
					string value = regex.Match(array[i]).Groups[0].Value;
					list.Add(value.Trim());
				}
			}
			return list.ToArray();
		}
		throw new Exception("no lsusb.");
	}

	public static bool getDevicesMtk(out int com_port)
	{
		new List<string>();
		com_port = 0;
		Regex regex = new Regex("COM\\d+");
		string qcLsUsb = Script.QcLsUsb;
		if (File.Exists(qcLsUsb.Replace("\"", "")))
		{
			string text = new Cmd("", "").Execute(null, qcLsUsb);
			if (text != string.Empty)
			{
				Log.w("ls ubs :" + text);
			}
			string[] array = Regex.Split(text, "\r\n");
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]) && (array[i].IndexOf("PreLoader") > 0 || array[i].ToLower().IndexOf("mediatek") >= 0))
				{
					Match match = regex.Match(array[i]);
					string value = match.Groups[0].Value;
					regex = new Regex("\\d+");
					match = regex.Match(value);
					com_port = Convert.ToInt32(match.Groups[0].Value);
					if (com_port > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		throw new Exception("no lsusb.");
	}

	public static bool getSpecialDevice(int bootrom_port, int preloader_port, out int com_port)
	{
		Regex regex = new Regex("COM\\d+");
		com_port = 0;
		string qcLsUsb = Script.QcLsUsb;
		if (File.Exists(qcLsUsb.Replace("\"", "")))
		{
			string text = new Cmd("", "").Execute(null, qcLsUsb);
			if (text != string.Empty)
			{
				Log.w("ls ubs " + text);
			}
			string[] array = Regex.Split(text, "\r\n");
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					if (array[i].IndexOf(bootrom_port.ToString()) > 0)
					{
						com_port = bootrom_port;
						return true;
					}
					if (array[i].IndexOf(preloader_port.ToString()) > 0)
					{
						com_port = preloader_port;
						return true;
					}
					Match match = regex.Match(array[i]);
					string value = match.Groups[0].Value;
					regex = new Regex("\\d+");
					match = regex.Match(value);
					com_port = Convert.ToInt32(match.Groups[0].Value);
					if (com_port > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		throw new Exception("no lsusb.");
	}

	public static string[] getDevicesJLQ()
	{
		List<string> list = new List<string>();
		Regex regex = new Regex("COM\\d+");
		string qcLsUsb = Script.QcLsUsb;
		Log.w("lsusb path:" + qcLsUsb);
		if (File.Exists(qcLsUsb.Replace("\"", "")))
		{
			string text = new Cmd("", "").Execute(null, qcLsUsb);
			if (text != string.Empty)
			{
				Log.w("ls ubs :" + text);
			}
			string[] array = Regex.Split(text, "\r\n");
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]) && array[i].IndexOf("JLQ") >= 0 && (array[i].IndexOf("BOOTROM") >= 0 || array[i].IndexOf("TL") >= 0))
				{
					string value = regex.Match(array[i]).Groups[0].Value;
					list.Add(value.Trim());
				}
			}
			return list.ToArray();
		}
		throw new Exception("no lsusb.");
	}
}
