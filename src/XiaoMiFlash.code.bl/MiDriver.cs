using System;
using System.IO;
using Microsoft.Win32;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class MiDriver
{
	public string[] infFiles = new string[5] { "Google\\Driver\\android_winusb.inf", "Nvidia\\Driver\\NvidiaUsb.inf", "Microsoft\\Driver\\tetherxp.inf", "Microsoft\\Driver\\wpdmtphw.inf", "Qualcomm\\Driver\\qcser.inf" };

	public void CopyFiles(string installationPath)
	{
		installationPath = installationPath.Substring(0, installationPath.LastIndexOf('\\') + 1);
		try
		{
			string systemDirectory = Environment.SystemDirectory;
			string[] array = new string[1] { "Qualcomm\\Driver\\serial\\i386\\qcCoInstaller.dll" };
			(new string[1])[0] = systemDirectory + "\\qcCoInstaller.dll";
			_ = installationPath + "Source\\ThirdParty\\";
			for (int i = 0; i < array.Length; i++)
			{
			}
		}
		catch (Exception ex)
		{
			Log.Installw(installationPath, "copy file failed," + ex.Message);
		}
	}

	public void InstallAllDriver(string installationPath, bool uninstallOld)
	{
		installationPath = installationPath.Substring(0, installationPath.LastIndexOf('\\') + 1);
		string text = installationPath + "Source\\ThirdParty\\";
		if (new DirectoryInfo(text).Exists)
		{
			for (int i = 0; i < infFiles.Length; i++)
			{
				InstallDriver(text + infFiles[i], installationPath, uninstallOld);
			}
		}
		else
		{
			Log.Installw(installationPath, "dic " + text + " not exists.");
		}
	}

	public void InstallDriver(string infPath, string installationPath, bool uninstallOld)
	{
		try
		{
			string text = "Software\\XiaoMi\\MiFlash\\";
			FileInfo fileInfo = new FileInfo(infPath);
			RegistryKey localMachine = Registry.LocalMachine;
			RegistryKey registryKey = localMachine.OpenSubKey(text, writable: true);
			Log.Installw(installationPath, "open RegistryKey " + text);
			if (registryKey == null)
			{
				registryKey = localMachine.CreateSubKey(text, RegistryKeyPermissionCheck.ReadWriteSubTree);
				Log.Installw(installationPath, "create RegistryKey " + text);
			}
			registryKey.GetValueNames();
			object value = registryKey.GetValue(fileInfo.Name);
			bool success = true;
			string text2 = "";
			if (value != null && uninstallOld)
			{
				text2 = Driver.UninstallInf(value.ToString(), out success);
				Log.Installw(installationPath, "driver " + value.ToString() + " exists,uninstall,reuslt " + success + ",GetLastWin32Error" + text2);
			}
			string destinationInfFileNameComponent = "";
			string destinationInfFileName = "";
			text2 = Driver.SetupOEMInf(fileInfo.FullName, out destinationInfFileName, out destinationInfFileNameComponent, out success);
			Log.Installw(installationPath, "install driver " + fileInfo.FullName + " to " + destinationInfFileName + ",result " + success + ",GetLastWin32Error " + text2);
			if (success)
			{
				registryKey.SetValue(fileInfo.Name, destinationInfFileNameComponent);
				Log.Installw(installationPath, "set RegistryKey value:" + fileInfo.Name + "--" + destinationInfFileNameComponent);
			}
			registryKey.Close();
			if (infPath.IndexOf("android_winusb.inf") >= 0)
			{
				string environmentVariable = Environment.GetEnvironmentVariable("USERPROFILE");
				Cmd cmd = new Cmd("", "");
				string text3 = "mkdir \"" + environmentVariable + "\\.android\"";
				string text4 = cmd.Execute(null, text3);
				Log.Installw(installationPath, text3);
				Log.Installw(installationPath, "output:" + text4);
				text3 = " echo 0x2717 >>\"" + environmentVariable + "\\.android\\adb_usb.ini\"";
				text4 = cmd.Execute(null, text3);
				Log.Installw(installationPath, text3);
				Log.Installw(installationPath, "output:" + text4);
			}
		}
		catch (Exception ex)
		{
			Log.Installw(installationPath, "install driver " + infPath + ", exception:" + ex.Message);
		}
	}
}
