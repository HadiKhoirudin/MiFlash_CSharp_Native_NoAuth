using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using MiUSB;
using XiaoMiFlash.code.bl;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility;

public class UsbDevice
{
	private static object obj_lock = new object();

	private static List<string> _mtkdevice = new List<string>();

	private static int _flashmode = 0;

	public static int FlashMode
	{
		get
		{
			return _flashmode;
		}
		set
		{
			_flashmode = value;
		}
	}

	public static List<string> MtkDevice
	{
		get
		{
			return _mtkdevice;
		}
		set
		{
			_mtkdevice = value;
		}
	}

	public static List<Device> GetDevice()
	{
		List<Device> list = new List<Device>();
		string text = "Qualcomm";
		int num = 0;

		string[] devicesQc = ComPortCtrl.getDevicesQc();
		int num2 = 0;
        foreach (string text3 in devicesQc)
        {
            num2 = int.Parse(text3.ToLower().Replace("com", ""));
            list.Add(new Device
            {
                Index = num2,
                Name = text3,
                DeviceCtrl = new SerialPortDevice()
            });
        }
        List<UsbNodeConnectionInformation> scriptDevices = GetScriptDevices(TreeViewUsbItem.AllUsbDevices);
		List<string> list2 = GetScriptDevice().ToList();
		int num3 = 0;
		foreach (string item2 in list2)
		{
			foreach (UsbNodeConnectionInformation item3 in scriptDevices)
			{
				if (!string.IsNullOrEmpty(item2) && !string.IsNullOrEmpty(item3.DeviceDescriptor.SerialNumber) && item2.ToLower() == item3.DeviceDescriptor.SerialNumber.ToLower())
				{
					num3++;
					num2 = num3;
					list.Add(new Device
					{
						Index = num2,
						Name = item2,
						IdProduct = item3.DeviceDescriptor.idProduct,
						IdVendor = item3.DeviceDescriptor.idVendor,
						DeviceCtrl = new ScriptDevice()
					});
				}
			}
		}
		foreach (UsbNodeConnectionInformation item4 in scriptDevices)
		{
			if (string.IsNullOrEmpty(item4.DeviceDescriptor.SerialNumber) || list2.IndexOf(item4.DeviceDescriptor.SerialNumber) >= 0)
			{
				continue;
			}
			num2 = GetDeviceIndex(item4.DeviceDescriptor.SerialNumber);
			int num4 = 10;
			while (num2 <= 0 && num4-- >= 0)
			{
				if (num4 < 9)
				{
					Thread.Sleep(200);
				}
				num2 = GetDeviceIndex(item4.DeviceDescriptor.SerialNumber);
			}
			list.Add(new Device
			{
				Index = num2,
				Name = item4.DeviceDescriptor.SerialNumber,
				DeviceCtrl = new ScriptDevice()
			});
		}
		return list;
	}

	public static List<Device> GetFastbootDevice()
	{
		List<Device> list = new List<Device>();
		List<UsbNodeConnectionInformation> scriptDevices = GetScriptDevices(TreeViewUsbItem.AllUsbDevices);
		string text = "";
		foreach (UsbNodeConnectionInformation item in scriptDevices)
		{
			if (!string.IsNullOrEmpty(item.DeviceDescriptor.SerialNumber))
			{
				Log.w("devicepath:" + item.DevicePath);
				Log.w("hubpath:" + item.HubPath);
				Log.w("SerialNumber:" + item.DeviceDescriptor.SerialNumber);
				if (item.HubPath != null)
				{
					text = item.HubPath.Split('&')[0].Split('#')[2];
					string text2 = text;
					int connectionIndex = item.ConnectionIndex;
					text = text2 + connectionIndex;
				}
				else
				{
					text = item.DevicePath.Split('&')[0].Split('#')[2];
					string text3 = text;
					int connectionIndex = item.ConnectionIndex;
					text = text3 + connectionIndex;
				}
				list.Add(new Device
				{
					Index = int.Parse(text),
					Name = item.DeviceDescriptor.SerialNumber,
					IdProduct = item.DeviceDescriptor.idProduct,
					IdVendor = item.DeviceDescriptor.idVendor,
					DeviceCtrl = new ScriptDevice()
				});
			}
		}
		return list;
	}

	public static UsbNodeConnectionInformationForPDL GetFastbootDeviceByIndex(string index)
	{
		new List<Device>();
		int num = 0;
		int pciIndex = int.Parse(index.Substring(0, 1));
		num = int.Parse(index.Substring(1));
		return TreeViewUsbItem.GetPDLUsbDevice(pciIndex, num);
	}

	public static string[] GetScriptDevice()
	{
		List<string> list = new List<string>();
		string fastboot = Script.fastboot;
		if (File.Exists(fastboot.Replace("\"", "")))
		{
			string[] array = Regex.Split(new Cmd("", "").Execute(null, fastboot + " devices"), "\r\n");
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					list.Add(Regex.Split(array[i], "\t")[0]);
				}
			}
			return list.ToArray();
		}
		throw new Exception("no fastboot.");
	}

	public static int GetQcDeviceIndex(string comName)
	{
		int num = 10;
		int num2 = 10;
		_ = (int.Parse(comName.ToLower().Replace("com", "")) - num) / num2;
		return 0;
	}

	public static int GetBaseNum(string comName)
	{
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\XiaoMi\\MiFlash\\");
		registryKey.GetValue("test").ToString();
		registryKey.Close();
		return 0;
	}

	public static int GetDeviceIndex(string devicesSn)
	{
		lock (obj_lock)
		{
			int result = -1;
			if (File.Exists(Script.qcCoInstaller.Replace("\"", "")))
			{
				string text = "";
				string text2 = "";
				try
				{
					Cmd cmd = new Cmd("", "");
					text2 = string.Format("rundll32.exe {0},qcGetDeviceIndex {1}", "qcCoInstaller.dll", devicesSn);
					text = cmd.Execute(null, text2);
					if (!string.IsNullOrEmpty(text))
					{
						result = Convert.ToInt32(text);
					}
				}
				catch (Exception ex)
				{
					Log.w(text2 + ":" + text);
					Log.w(ex.Message + ":" + ex.StackTrace);
				}
			}
			return result;
		}
	}

	private static List<UsbNodeConnectionInformation> GetScriptDevices(List<TreeViewUsbItem> UsbItems)
	{
		List<UsbNodeConnectionInformation> outItems = new List<UsbNodeConnectionInformation>();
		foreach (TreeViewUsbItem UsbItem in UsbItems)
		{
			GetAndroidDevices(UsbItem, ref outItems);
		}
		return outItems;
	}

	private static void GetAndroidDevices(TreeViewUsbItem item, ref List<UsbNodeConnectionInformation> outItems)
	{
		try
		{
			if (item.Children != null && item.Children.Count > 0)
			{
				foreach (TreeViewUsbItem child in item.Children)
				{
					GetAndroidDevices(child, ref outItems);
				}
				return;
			}
			UsbNodeConnectionInformation item2 = (UsbNodeConnectionInformation)item.Data;
			if (item2.DeviceDescriptor.Manufacturer != null && (item2.DeviceDescriptor.Product.ToLower() == "android" || item2.DeviceDescriptor.Product.ToLower() == "fastboot" || item2.DeviceDescriptor.Product.ToLower() == "intel android ad" || item2.DeviceDescriptor.Manufacturer.ToLower().IndexOf("xiaomi inc") >= 0 || Convert.ToInt32(item2.DeviceDescriptor.idVendor).ToString("x4") == "8087" || Convert.ToInt32(item2.DeviceDescriptor.idVendor).ToString("x4") == "0955" || Convert.ToInt32(item2.DeviceDescriptor.idVendor).ToString("x4").ToLower() == "05c6" || Convert.ToInt32(item2.DeviceDescriptor.idVendor).ToString("x4").ToLower() == "18d1" || Convert.ToInt32(item2.DeviceDescriptor.idVendor).ToString("x4") == "2717" || Convert.ToInt32(item2.DeviceDescriptor.idVendor).ToString("x4").ToLower() == "31ef"))
			{
				outItems.Add(item2);
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message + " " + ex.StackTrace);
		}
	}

	private static TreeViewUsbItem GetLastChild(TreeViewUsbItem item, string[] deviceSn, ref List<UsbNodeConnectionInformation> outItems)
	{
		if (item.Children != null && item.Children.Count > 0)
		{
			foreach (TreeViewUsbItem child in item.Children)
			{
				GetLastChild(child, deviceSn, ref outItems);
			}
			return item;
		}
		try
		{
			UsbNodeConnectionInformation item2 = (UsbNodeConnectionInformation)item.Data;
			if (item.Data != null)
			{
				if (deviceSn.ToList().Contains(item2.DeviceDescriptor.SerialNumber))
				{
					outItems.Add(item2);
					return item;
				}
				return item;
			}
			return item;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
			Log.w(ex.Message + " " + ex.StackTrace);
			return item;
		}
	}
}
