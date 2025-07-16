using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility;

public class Log
{
	public static readonly object _lock = new object();

	public static int GetLineNum()
	{
		return new StackTrace(1, fNeedFileInfo: true).GetFrame(0).GetFileLineNumber();
	}

	public static string GetCurSourceFileName()
	{
		return new StackTrace(1, fNeedFileInfo: true).GetFrame(0).GetFileName();
	}

	public static void moveLog(string deviceName, string filename, bool result)
	{
		if (string.IsNullOrEmpty(deviceName) || string.IsNullOrEmpty(filename))
		{
			return;
		}
		string text = "";
		string text2 = "";
		string text3 = "";
		foreach (Device flashDevice in FlashingDevice.flashDeviceList)
		{
			if (flashDevice.Name == deviceName)
			{
				text = string.Format("{0}@{1}.txt", flashDevice.Name, flashDevice.StartTime.ToString("yyyyMdHms"));
				text2 = string.Format("{0}@{1}.txt", filename, flashDevice.StartTime.ToString("yyyyMdHms"));
				text3 = flashDevice.StartTime.ToString("yyyyMd");
				break;
			}
		}
		string text4 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
		string text5 = "";
		text5 = ((!result) ? (AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text3 + "\\fail\\" + text2) : (AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text3 + "\\pass\\" + text2));
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(text5)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text5));
			}
			FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(text4)).GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				_ = files[i];
				File.Copy(text4, text5, overwrite: true);
			}
		}
		catch (Exception ex)
		{
			w(ex.Message);
		}
	}

	public static void w(string deviceName, Exception ex, bool stopFlash)
	{
		string text = ex.Message;
		string stackTrace = ex.StackTrace;
		if (stopFlash)
		{
			text = "error:" + text;
			stackTrace = "error" + stackTrace;
		}
		w(deviceName, text, stopFlash);
	}

	public static void w(string deviceName, string msg)
	{
		w(deviceName, msg, throwEx: true);
	}

	public static void w(string deviceName, string msg, bool throwEx)
	{
		if (string.IsNullOrEmpty(deviceName))
		{
			w(msg);
			return;
		}
		string text = "";
		foreach (Device flashDevice in FlashingDevice.flashDeviceList)
		{
			if (flashDevice.Name == deviceName)
			{
				text = string.Format("{0}@{1}.txt", flashDevice.Name, flashDevice.StartTime.ToString("yyyyMdHms"));
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			w(msg, throwEx: true);
			return;
		}
		string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
		try
		{
			if (!File.Exists(path))
			{
				File.Create(path).Close();
			}
		}
		catch (Exception)
		{
			w(msg);
			return;
		}
		string text2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");
		WriteFile("[" + text2 + "  " + deviceName + "]:" + msg, path);
		if (msg.ToLower().IndexOf("error") >= 0 || msg.ToLower().IndexOf("fail") >= 0 || msg.ToLower().IndexOf("找不到批处理文件") >= 0)
		{
			FlashingDevice.UpdateDeviceStatus(deviceName, null, msg, "error", isDone: true);
		}
	}

	public static void debugString(string deviceName, byte[] buffer)
	{
		if (string.IsNullOrEmpty(deviceName))
		{
			return;
		}
		string text = "";
		foreach (Device flashDevice in FlashingDevice.flashDeviceList)
		{
			if (flashDevice.Name == deviceName)
			{
				text = string.Format("{0}@{1}.txt", flashDevice.Name, flashDevice.StartTime.ToString("yyyyMdHms"));
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
		try
		{
			if (!File.Exists(path))
			{
				File.Create(path).Close();
			}
		}
		catch (Exception)
		{
			return;
		}
		string text2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff");
		string text3 = "";
		int num = buffer.Length;
		int num2 = 0;
		string text4 = "-----------------------------------------------------------------------------------------------------------";
		WriteFile("[" + text2 + "  " + deviceName + "]:" + text4, path);
		text3 = $"DEBUG: bufferLength{num}";
		WriteFile("[" + text2 + "  " + deviceName + "]:" + text3, path);
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int i = 0; i < buffer.Length; i++)
		{
			if (num2 == 0)
			{
				stringBuilder.Remove(0, stringBuilder.Length);
				stringBuilder2.Remove(0, stringBuilder2.Length);
			}
			stringBuilder.Append("0x" + buffer[i].ToString("X2") + " ");
			stringBuilder2.Append(Convert.ToChar(buffer[i]).ToString());
			num2++;
			if (num2 == 16)
			{
				text3 = "DEBUG: " + stringBuilder.ToString();
				WriteFile("[" + text2 + "  " + deviceName + "]:" + text3, path);
				num2 = 0;
			}
		}
		if (num2 > 0)
		{
			text3 = "DEBUG: " + stringBuilder.ToString();
			WriteFile("[" + text2 + "  " + deviceName + "]:" + text3, path);
		}
		WriteFile("[" + text2 + "  " + deviceName + "]:" + text4, path);
	}

	public static void w(string msg)
	{
		w(msg, throwEx: false);
	}

	public static void w(string msg, bool throwEx)
	{
		string text = "";
		text = string.Format("{0}@{1}.txt", "miflash", DateTime.Now.ToString("yyyyMd"));
		string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
		if (!File.Exists(path))
		{
			File.Create(path).Close();
		}
		msg = string.Format("[{0}]:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff:ffffff"), msg);
		WriteFile(msg, path);
		if ((msg.ToLower().IndexOf("error") >= 0 || msg.ToLower().IndexOf("fail") >= 0 || msg.ToLower().IndexOf("找不到批处理文件") >= 0) && throwEx)
		{
			throw new Exception(msg);
		}
	}

	public static void wFlashStatus(string msg)
	{
		try
		{
			string text = "";
			text = string.Format("{0}-Result@{1}.txt", "MiFlash", DateTime.Now.ToString("yyyyMd"));
			string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
			if (!File.Exists(path))
			{
				File.Create(path).Close();
			}
			StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.Default);
			streamWriter.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]:" + msg);
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			w("wFlashStatus " + msg + "  " + ex.Message + " " + ex.StackTrace);
		}
	}

	public static void wFlashDebug(string msg)
	{
		try
		{
			string text = "";
			text = string.Format("{0}-mesdebug@{1}.txt", "MiFlash", DateTime.Now.ToString("yyyyMd"));
			string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
			if (!File.Exists(path))
			{
				File.Create(path).Close();
			}
			StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.Default);
			streamWriter.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]:" + msg);
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			w("wFlashDebug " + msg + "  " + ex.Message + " " + ex.StackTrace);
		}
	}

	public static void Installw(string installationPath, string msg)
	{
		string text = "";
		text = string.Format("{0}@{1}.txt", "miflash", DateTime.Now.ToString("yyyyMd"));
		string path = installationPath + "log\\" + text;
		if (!File.Exists(path))
		{
			File.Create(path).Close();
		}
		StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.Default);
		streamWriter.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]:" + msg);
		streamWriter.Close();
	}

	public static void LogMsg(string deviceName, string msg, string suffix)
	{
		string text = string.Format("{0}_{1}@{2}.txt", deviceName, suffix, DateTime.Now.ToString("yyyyMdHms"));
		string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + text;
		try
		{
			if (!File.Exists(path))
			{
				File.Create(path).Close();
			}
		}
		catch (Exception)
		{
			w(msg);
			return;
		}
		WriteFile(msg, path);
	}

	public static void WriteFile(string log, string path)
	{
		lock (_lock)
		{
			if (!File.Exists(path))
			{
				using (new FileStream(path, FileMode.Create))
				{
				}
			}
			using FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
			using StreamWriter streamWriter = new StreamWriter(stream);
			string value = log.ToString();
			streamWriter.WriteLine(value);
			streamWriter.Flush();
		}
	}

	public static void WriteLog(string log, string logPath)
	{
		WriteFile(log, logPath);
	}

	public static void WriteErrorLog(string log, string logPath)
	{
		WriteFile(log, logPath);
	}
}
