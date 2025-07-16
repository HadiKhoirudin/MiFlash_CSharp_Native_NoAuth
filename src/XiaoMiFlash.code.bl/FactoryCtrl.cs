using System;
using System.IO;
using PUB_TEST_FUNC_DLL;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class FactoryCtrl
{
	private static string funcDll = "PUB_TEST_FUNC_DLL.dll";

	private static string routingDll = "RoutingObject.dll";

	private static string thirdParty = "Source\\ThirdParty\\";

	private static bool firstCall = true;

	private static readonly object locker1 = new object();

	public static bool SetFactory(string factory)
	{
		bool result = false;
		string applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
		string text = thirdParty + factory;
		try
		{
			if (Directory.Exists(applicationBase + text))
			{
				string[] files = Directory.GetFiles(applicationBase + text);
				string text2 = "";
				string[] array = files;
				foreach (string text3 in array)
				{
					text2 = Path.GetFileName(text3);
					if (File.Exists(applicationBase + text2))
					{
						File.Delete(applicationBase + text2);
					}
					File.Copy(text3, applicationBase + text2, overwrite: true);
					Log.w("Factory:" + factory + " switch dll " + text3);
				}
				result = true;
				return result;
			}
			Log.w("file not exit " + text);
			return result;
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			return result;
		}
	}

	public static FlashResult SetFlashResultD(string deivce, bool reuslt)
	{
		Log.w(deivce, "start SetFlashResultD");
		FlashResult flashResult = new FlashResult();
		flashResult.Result = false;
		flashResult.Msg = "upload result failed";
		try
		{
			string msg = "";
			if (string.IsNullOrEmpty(deivce))
			{
				flashResult.Result = false;
				flashResult.Msg = "device is null";
				return flashResult;
			}
			Log.w(deivce, "upload flash result");
			TPUB_TEST_FUNC_DLL factoryObject = GetFactoryObject(deivce, out msg);
			if (factoryObject == null)
			{
				flashResult.Result = false;
				flashResult.Msg = "error:couldn't get TPUB_TEST_FUNC_DLL";
			}
			else
			{
				bool flag = factoryObject.SaveDataByCPUID(deivce, reuslt, out msg);
				msg = "SaveDataByCPUID result " + flag + " status " + msg;
				flashResult.Result = flag;
				flashResult.Msg = msg;
				Log.w(deivce, msg);
				if (flag)
				{
					Log.w(deivce, "upload result success");
				}
				else
				{
					Log.w(deivce, "upload result failed");
				}
			}
		}
		catch (Exception ex)
		{
			Log.w(deivce + " " + ex.Message + "  " + ex.StackTrace);
			flashResult.Msg = flashResult.Msg + " " + ex.Message;
		}
		Log.w(deivce, flashResult.Msg);
		return flashResult;
	}

	public static CheckCPUIDResult GetSearchPathD(string deivce, string swPath)
	{
		CheckCPUIDResult checkCPUIDResult = new CheckCPUIDResult();
		Log.w(deivce, "GetSearchPath");
		bool flag = false;
		string text = "";
		string msg = "";
		try
		{
			TPUB_TEST_FUNC_DLL factoryObject = GetFactoryObject(deivce, out msg);
			if (factoryObject == null)
			{
				if (string.IsNullOrEmpty(msg))
				{
					msg = "can not GetFactoryObject";
				}
				Log.w(deivce, "CheckCPUID failed!");
			}
			else
			{
				flag = factoryObject.CheckCPUID(deivce, out text, out msg);
				foreach (Device flashDevice in FlashingDevice.flashDeviceList)
				{
					if (flashDevice.Name == deivce)
					{
						flashDevice.CheckCPUID = flag;
					}
				}
				Log.w(deivce, "CheckCPUID result " + flag + " status " + msg + " imgPath " + text, throwEx: false);
			}
			checkCPUIDResult.Msg = msg;
		}
		catch (Exception ex)
		{
			checkCPUIDResult.Msg = ex.Message;
		}
		checkCPUIDResult.Device = deivce;
		checkCPUIDResult.Result = flag;
		checkCPUIDResult.Path = text;
		Log.w(deivce, "CheckCPUID result " + checkCPUIDResult.Result + " status " + checkCPUIDResult.Msg);
		return checkCPUIDResult;
	}

	public static TPUB_TEST_FUNC_DLL GetFactoryObject(string deivce, out string msg)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		TPUB_TEST_FUNC_DLL val = new TPUB_TEST_FUNC_DLL();
		msg = string.Empty;
		bool flag = val.InitCheck(out msg);
		Log.w(deivce, "InitCheck result " + flag + " status " + msg, throwEx: false);
		if (flag)
		{
			return val;
		}
		FlashingDevice.UpdateDeviceStatus(deivce, null, "can't init factory ev " + msg, "factory ev error", isDone: true);
		Log.w(deivce, "InitCheck err result " + flag + " status " + msg, throwEx: false);
		return null;
	}

	public static CheckCPUIDResult FactorySignEdl(string deivce, string orignKey, out string signedKey, string clientId)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		signedKey = "";
		CheckCPUIDResult checkCPUIDResult = new CheckCPUIDResult();
		bool result = false;
		string msg = "";
		try
		{
			TPUB_TEST_FUNC_DLL val = new TPUB_TEST_FUNC_DLL();
			lock (locker1)
			{
				result = val.SLA_Challenge(clientId, orignKey, out signedKey, out msg);
			}
			checkCPUIDResult.Msg = msg;
		}
		catch (Exception ex)
		{
			checkCPUIDResult.Msg = ex.Message;
		}
		checkCPUIDResult.Device = deivce;
		checkCPUIDResult.Result = result;
		Log.w(deivce, "device " + deivce + " SLA_Challenge result " + checkCPUIDResult.Result + " status " + checkCPUIDResult.Msg);
		return checkCPUIDResult;
	}

	public static CheckCPUIDResult FactoryUploadDebugpolicy(string deivce, string cpuid)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		CheckCPUIDResult checkCPUIDResult = new CheckCPUIDResult();
		bool result = false;
		string msg = "";
		try
		{
			new TPUB_TEST_FUNC_DLL();
			checkCPUIDResult.Msg = msg;
		}
		catch (Exception ex)
		{
			checkCPUIDResult.Msg = ex.Message;
		}
		checkCPUIDResult.Device = deivce;
		checkCPUIDResult.Result = result;
		Log.w(deivce, "device " + deivce + " UploadDebugpolicy result " + checkCPUIDResult.Result + " status " + checkCPUIDResult.Msg);
		return checkCPUIDResult;
	}
}
