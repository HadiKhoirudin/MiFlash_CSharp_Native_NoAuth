using System;
using System.IO;
using System.Linq;
using System.Threading;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class ScriptDevice : DeviceCtrl
{
	private X5Mes x5mes = new X5Mes();

	private Cmd mycmd;

	public override void flash()
	{
		try
		{
			string sFsn = "";
			string text = MiAppConfig.Get("factory");
			if (text == "MES")
			{
				string text2 = "";
				string msg = "";
				if (!Mes.Init("", out msg))
				{
					FlashingDevice.UpdateDeviceStatus(deviceName, null, "error:MesInit result err status " + msg, "factory ev error", isDone: true);
					Log.w(deviceName, "error:device " + deviceName + " MesInit result err status " + msg, throwEx: false);
					return;
				}
				Device device = FlashingDevice.flashDeviceList.Find((Device d) => d.Name.Equals(deviceName));
				bool flag = Mes.CheckSN(deviceName, device.Index, out text2, out sFsn, out msg);
				Log.w(deviceName, $"device {deviceName} Mes.CheckSN {sFsn} sswPath{text2} result{flag}", throwEx: false);
				if (!flag)
				{
					FlashingDevice.UpdateDeviceStatus(deviceName, null, "error:Mes CheckCPUID result err status " + msg, "factory ev error", isDone: true);
					Log.w(deviceName, "error:device " + deviceName + " CheckSN result err status " + msg, throwEx: false);
					return;
				}
				Log.w(deviceName, "device " + deviceName + " CheckSN success ", throwEx: false);
				swPath = text2;
				device.CheckCPUID = true;
				device.FSN = sFsn;
				Log.w(deviceName, $"device {deviceName} item.CheckCPUID{device.CheckCPUID},item.FSN{device.FSN}", throwEx: false);
			}
			else if (text == "X5MES")
			{
				string text3 = "";
				string text4 = "";
				if (!x5mes.Init(deviceName))
				{
					FlashingDevice.UpdateDeviceStatus(deviceName, null, "error:X5MesInit result err status " + text4, "factory ev error", isDone: true);
					Log.w(deviceName, "error:device " + deviceName + " X5MesInit result err status " + text4, throwEx: false);
					return;
				}
				Log.w(deviceName, "x5mes Init" + deviceName + " sucess", throwEx: false);
				Device device2 = FlashingDevice.flashDeviceList.Find((Device d) => d.Name.Equals(deviceName));
				bool flag2 = x5mes.CheckSN(deviceName, deviceName, out sFsn, out text3);
				Log.w(deviceName, $"device {deviceName} X5Mes.CheckSN {sFsn} sswPath{text3} result{flag2}", throwEx: false);
				if (!flag2)
				{
					FlashingDevice.UpdateDeviceStatus(deviceName, null, "error:X5CheckCPUID result err status " + text4, "factory ev error", isDone: true);
					Log.w(deviceName, "error:device " + deviceName + " X5CheckSN result err status " + text4, throwEx: false);
					return;
				}
				Log.w(deviceName, "device " + deviceName + " X5CheckSN success ", throwEx: false);
				swPath = text3;
				device2.CheckCPUID = true;
				device2.FSN = sFsn;
				Log.w(deviceName, $"device {deviceName} item.CheckCPUID{device2.CheckCPUID},item.FSN{device2.FSN}", throwEx: false);
			}
			if (new TimeOut
			{
				Do = fastbootflash
			}.DoWithTimeout(new TimeSpan(0, 0, 11, 40)))
			{
				FlashingDevice.UpdateDeviceStatus(deviceName, 1f, "flash timeout", "error", isDone: true);
				Log.w(deviceName, "error: flash timeout" + mycmd.mprocess.Id, throwEx: false);
				killCmdProcess();
				updateMesResult();
			}
			else
			{
				updateMesResult();
			}
		}
		catch (Exception ex)
		{
			FlashingDevice.UpdateDeviceStatus(deviceName, null, ex.Message, "error", isDone: true);
			Log.w(deviceName, ex, stopFlash: false);
		}
	}

	public void updateMesResult()
	{
		string text = MiAppConfig.Get("factory");
		if (text == "MES")
		{
			Log.w(deviceName, "do update MES", throwEx: false);
			string msg = "";
			bool? flag = null;
			Device device = FlashingDevice.flashDeviceList.Find((Device dex) => dex.Name.Equals(deviceName));
			if (device.Name == deviceName)
			{
				Log.w(deviceName, "flashDeviceList find device: " + deviceName);
				if (device.IsDone.HasValue && device.IsDone.Value && device.Status == "flash done")
				{
					flag = true;
				}
				if (device.IsDone.HasValue && device.IsDone.Value && device.Result.ToLower() == "success")
				{
					flag = true;
				}
				else if (device.Result.ToLower().IndexOf("error") >= 0 || device.Result.ToLower().IndexOf("fail") >= 0)
				{
					flag = false;
				}
				Log.w(device.Name, $"update flash result to facotry server devices id{device.Name} flashresult {flag.Value}", throwEx: false);
				if (Mes.SaveResult(device.Name, device.Index, flag.Value, out msg))
				{
					device.IsDone = true;
					Log.w(device.Name, "Mes.SaveResult success");
				}
				else
				{
					Log.w(device.Name, "Mes.SaveResult error: " + msg);
				}
			}
			else
			{
				Log.w(deviceName, "flashDeviceList  not find device: " + deviceName);
			}
		}
		else
		{
			if (!(text == "X5MES"))
			{
				return;
			}
			Log.w(deviceName, "do update MES", throwEx: false);
			bool? flag2 = null;
			Device device2 = FlashingDevice.flashDeviceList.Find((Device dex) => dex.Name.Equals(deviceName));
			if (device2.Name == deviceName)
			{
				Log.w(deviceName, "flashDeviceList find device: " + deviceName);
				if (device2.IsDone.HasValue && device2.IsDone.Value && device2.Status == "flash done")
				{
					flag2 = true;
				}
				if (device2.IsDone.HasValue && device2.IsDone.Value && device2.Result.ToLower() == "success")
				{
					flag2 = true;
				}
				else if (device2.Result.ToLower().IndexOf("error") >= 0 || device2.Result.ToLower().IndexOf("fail") >= 0)
				{
					flag2 = false;
				}
				Log.w(device2.Name, $"update flash result to facotry server devices id{device2.Name} flashresult {flag2.Value}", throwEx: false);
				if (x5mes.SaveResult(device2.Name, flag2.Value))
				{
					device2.IsDone = true;
					Log.w(device2.Name, "X5Mes.SaveResult success");
				}
				else
				{
					Log.w(device2.Name, "X5Mes.SaveResult error");
				}
			}
			else
			{
				Log.w(deviceName, "flashDeviceList  not find device: " + deviceName);
			}
		}
	}

	public void killCmdProcess()
	{
		if (mycmd.mprocess != null)
		{
			Log.w(deviceName, "timeout kill cmd: " + mycmd.mprocess.Id, throwEx: false);
			mycmd.mprocess.Kill();
		}
	}

	public bool fastbootflash()
	{
		try
		{
			Log.w(deviceName, "Thread id:" + Thread.CurrentThread.ManagedThreadId + " Thread name:" + Thread.CurrentThread.Name);
			_ = Script.fastboot;
			string text = "";
			string[] array = FileSearcher.SearchFiles(swPath, flashScript);
			if (array.Length == 0)
			{
				throw new Exception("can not found file " + flashScript);
			}
			text = array[0];
			string command = "pushd \"" + swPath + "\"&&prompt $$&&set PATH=\"" + Script.AndroidPath + "\";%PATH%&&\"" + text + "\" -s " + deviceName + "&&popd";
			Log.w(deviceName, "image path:" + swPath);
			Log.w(deviceName, "env android path:" + Script.AndroidPath);
			Log.w(deviceName, "script :" + text);
			Cmd cmd = (mycmd = new Cmd(deviceName, text));
			if (Convert.ToInt32(idproduct).ToString("x4") == "0a65" && Convert.ToInt32(idvendor).ToString("x4") == "8087")
			{
				string text2 = "DNX fastboot mode";
				Log.w(deviceName, text2);
				FlashingDevice.UpdateDeviceStatus(deviceName, null, text2, "boot into kernelflinger", isDone: false);
				string text3 = swPath + "\\images\\loader.efi";
				if (File.Exists(text3))
				{
					text2 = "Boot into kernelflinger " + text3;
					Log.w(deviceName, text2);
					FlashingDevice.UpdateDeviceStatus(deviceName, null, text2, text2, isDone: false);
					string dosCommand = string.Format("pushd \"{0}\"&&prompt $$&&set PATH=\"{1}\";%PATH%&&fastboot boot \"{2}\" -s {3}&&popd", swPath + "\\images", Script.AndroidPath, text3, deviceName);
					string msg = cmd.Execute(deviceName, dosCommand);
					Log.w(deviceName, msg);
					int num = 4;
					bool flag = false;
					while (num-- >= 0 && !flag)
					{
						if (UsbDevice.GetScriptDevice().ToList().IndexOf(deviceName) >= 0)
						{
							flag = true;
							break;
						}
						Thread.Sleep(1000);
					}
					if (!flag)
					{
						Log.w(deviceName, "error:device couldn't boot up ");
					}
				}
				else
				{
					Log.w(deviceName, "error:couldn't find " + swPath + "\\loader.efi");
				}
			}
			cmd.Execute_returnLine(deviceName, command, 1);
			return true;
		}
		catch (Exception ex)
		{
			FlashingDevice.UpdateDeviceStatus(deviceName, null, ex.Message, "error", isDone: true);
			Log.w(deviceName, ex, stopFlash: false);
			return false;
		}
	}

	public void GrapLog()
	{
		string text = "C:\\fastbootlog";
		if (!Directory.Exists(text.Replace("\"", "")))
		{
			Directory.CreateDirectory(text);
		}
		Cmd cmd = new Cmd(deviceName, "");
		string text2 = "lkmsg";
		FlashingDevice.UpdateDeviceStatus(deviceName, null, "start grab " + text2 + " log", "grabbing log", isDone: false);
		string text3 = string.Format("{0}_{1}@{2}.txt", deviceName, text2, DateTime.Now.ToString("yyyyMdHms"));
		cmd.Execute(dosCommand: string.Format("\"{0}\" -s {1} oem lkmsg > {2}", arg2: text + "\\" + text3, arg0: Script.LKMSG_FASTBOOT, arg1: deviceName), deviceName: deviceName);
		FlashingDevice.UpdateDeviceStatus(deviceName, 1f, string.Format("grab done", text2), "grabbing log", isDone: false);
		text2 = "lpmsg";
		FlashingDevice.UpdateDeviceStatus(deviceName, 0f, "start grab " + text2 + " log", "grabbing log", isDone: false);
		text3 = string.Format("{0}_{1}@{2}.txt", deviceName, text2, DateTime.Now.ToString("yyyyMdHms"));
		cmd.Execute(dosCommand: string.Format("\"{0}\" -s {1} oem lpmsg > {2}", arg2: text + "\\" + text3, arg0: Script.LKMSG_FASTBOOT, arg1: deviceName), deviceName: deviceName);
		FlashingDevice.UpdateDeviceStatus(deviceName, 1f, "flash done", "grab done", isDone: true);
	}

	public override string[] getDevice()
	{
		return UsbDevice.GetScriptDevice();
	}

	public override void CheckSha256()
	{
	}
}
