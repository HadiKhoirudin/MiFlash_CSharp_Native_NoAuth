using System;

namespace XiaoMiFlash.code.module;

public class Script
{
	public static string AndroidPath => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Google\\Android\"";

	public static string fastboot => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Google\\Android\\fastboot.exe\"";

	public static string QcLsUsb => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Qualcomm\\fh_loader\\lsusb.exe\"";

	public static string SP_Download_Tool_PATH => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "SP_Download_tool\"";

	public static string LKMSG_FASTBOOT => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Mi\\fastboot.exe\"";

	public static string qcCoInstaller => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Mi\\qcCoInstaller.dll\"";

	public static string msiexec => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Mi\\msiexec.exe\"";

	public static string pythonMsi => "\"" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\Mi\\python-2.7.13.msi\"";

	public static string NslPath => AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\JR\\NSL\\";

	public static string JrFlashLoader => AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\ThirdParty\\JR\\NSL\\flash-loader.exe";
}
