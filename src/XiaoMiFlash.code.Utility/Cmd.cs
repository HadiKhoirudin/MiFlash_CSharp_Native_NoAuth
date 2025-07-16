using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility;

public class Cmd
{
	private string devicename;

	public string errMsg = "";

	private string scriptPath = "";

	public Process mprocess;

	public string logType = "";

	private IniFile ini = new IniFile();

	public Cmd(string _deivcename, string _scriptPath)
	{
		devicename = _deivcename;
		scriptPath = _scriptPath;
	}

	public string Execute(string deviceName, string dosCommand)
	{
		return Execute(deviceName, dosCommand, 0);
	}

	public string Execute_returnLine(string deviceName, string dosCommand)
	{
		return Execute_returnLine(deviceName, dosCommand, 1);
	}

	public string Execute(string deviceName, string command, int seconds)
	{
		string text = "";
		if (command != null && !command.Equals(""))
		{
			Process process = initCmdProcess(command);
			try
			{
				if (process.Start())
				{
					if (seconds == 0)
					{
						process.WaitForExit();
					}
					else
					{
						process.WaitForExit(seconds);
					}
					text = process.StandardOutput.ReadToEnd();
					text += process.StandardError.ReadToEnd();
					return text;
				}
				return text;
			}
			catch (Exception ex)
			{
				Log.w(deviceName, ex.Message);
				return text;
			}
			finally
			{
				if (process != null)
				{
					process.Close();
					process.Dispose();
				}
			}
		}
		return text;
	}

	public string Exec_returnLine(string deviceName, string command, int seconds)
	{
		devicename = deviceName;
		StringBuilder stringBuilder = new StringBuilder();
		if (command != null && !command.Equals(""))
		{
			try
			{
				Process process = initCmdProcess(command);
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.Arguments = "/C " + command;
				mprocess = process;
				foreach (Device flashDevice in FlashingDevice.flashDeviceList)
				{
					if (flashDevice.Name == devicename)
					{
						flashDevice.DCmd = this;
					}
				}
				StreamReader streamReader = null;
				StreamReader streamReader2 = null;
				process.Refresh();
				if (process.Start())
				{
					streamReader = process.StandardOutput;
					streamReader2 = process.StandardError;
					string text = streamReader.ReadLine();
					string text2 = streamReader2.ReadLine();
					while (!streamReader.EndOfStream && !streamReader2.EndOfStream)
					{
						if (text.Length > 0)
						{
							Log.w(devicename, text);
							if ((!string.IsNullOrEmpty(errMsg) || text.ToLower().IndexOf("error") >= 0 || text.ToLower().IndexOf("fail") >= 0 || text.ToLower() == "missmatching image and device" || text.ToLower() == "missmatching board version") && string.IsNullOrEmpty(errMsg))
							{
								errMsg = text;
							}
						}
						if (text2.Length > 0)
						{
							Log.w(devicename, text2);
							if ((!string.IsNullOrEmpty(errMsg) || text2.ToLower().IndexOf("error") >= 0 || text2.ToLower().IndexOf("fail") >= 0 || text2.ToLower() == "missmatching image and device" || text2.ToLower() == "missmatching board version") && string.IsNullOrEmpty(errMsg))
							{
								errMsg = text2;
							}
						}
						text = streamReader.ReadLine();
						text2 = streamReader2.ReadLine();
						float? percent = GetPercent(text);
						FlashingDevice.UpdateDeviceStatus(devicename, percent, text, "flashing", isDone: false);
					}
				}
			}
			catch (Exception ex)
			{
				Log.w(deviceName, ex, stopFlash: false);
				FlashingDevice.UpdateDeviceStatus(devicename, null, ex.Message, "error", isDone: true);
			}
			finally
			{
				FlashDone();
				if (mprocess != null)
				{
					mprocess.Close();
					mprocess.Dispose();
				}
				Log.w(devicename, "process exit.");
			}
		}
		return stringBuilder.ToString();
	}

	public string Execute_returnLine(string deviceName, string command, int seconds)
	{
		devicename = deviceName;
		StringBuilder stringBuilder = new StringBuilder();
		if (command != null && !command.Equals(""))
		{
			try
			{
				Process process = initCmdProcess(command);
				process.StartInfo.Arguments = "/C " + command;
				mprocess = process;
				foreach (Device flashDevice in FlashingDevice.flashDeviceList)
				{
					if (flashDevice.Name == devicename)
					{
						flashDevice.DCmd = this;
					}
				}
				process.EnableRaisingEvents = true;
				process.OutputDataReceived += process_OutputDataReceived;
				process.ErrorDataReceived += process_ErrorDataReceived;
				if (process.Start())
				{
					Log.w(devicename, "Physical Memory Usage:" + process.WorkingSet64 + " Byte");
					Log.w(devicename, "start process id " + process.Id + " name " + process.ProcessName);
					Log.w("start process id " + process.Id + " name " + process.ProcessName);
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				Log.w(deviceName, ex, stopFlash: false);
				FlashingDevice.UpdateDeviceStatus(devicename, null, ex.Message, "error", isDone: true);
			}
			finally
			{
				FlashDone();
				if (mprocess != null)
				{
					mprocess.Close();
					mprocess.Dispose();
				}
				Log.w(devicename, "process exit.");
			}
		}
		return stringBuilder.ToString();
	}

	private void process_Exited(object sender, EventArgs e)
	{
		try
		{
			Thread.Sleep(1000);
			Process process = (Process)sender;
			Log.w(devicename, "process exit id " + process.Id + " name " + process.ProcessName);
			Log.w("process exit id " + process.Id + " name " + process.ProcessName);
			process.CancelErrorRead();
			process.CancelOutputRead();
			process.Refresh();
		}
		catch (Exception ex)
		{
			Log.w(devicename, ex.Message, throwEx: false);
		}
		finally
		{
			FlashDone();
		}
	}

	private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.Data))
		{
			return;
		}
		try
		{
			string data = e.Data;
			Log.w(devicename, "info2:" + e.Data, throwEx: false);
			data = data.Replace(devicename, "devicename");
			if (!string.IsNullOrEmpty(errMsg) || data.ToLower().IndexOf("error") >= 0 || data.ToLower().IndexOf("fail") >= 0 || data.ToLower() == "missmatching image and device")
			{
				Log.w(devicename, "info2:" + e.Data, throwEx: false);
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = data;
				}
			}
		}
		catch (Exception ex)
		{
			Log.w(devicename, ex.Message);
			try
			{
				Log.w(devicename, "exit process");
				_ = (Process)sender;
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex.Message + "\r\n" + ex.StackTrace;
				}
			}
			catch (Exception ex2)
			{
				Log.w(devicename, ex2.Message);
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex2.Message + "\r\n" + ex2.StackTrace;
				}
			}
		}
	}

	private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.Data))
		{
			return;
		}
		try
		{
			string data = e.Data;
			float? percent = GetPercent(data);
			FlashingDevice.UpdateDeviceStatus(devicename, percent, data, "flashing", isDone: false);
			Log.w(devicename, "info1:" + data, throwEx: false);
			data = data.Replace(devicename, "devicename");
			if (!string.IsNullOrEmpty(errMsg) || data.ToLower().IndexOf("error") >= 0 || data.ToLower().IndexOf("fail") >= 0 || data.ToLower() == "missmatching image and device" || data.ToLower() == "missmatching board version")
			{
				Log.w(devicename, "info1:" + data, throwEx: false);
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = data;
				}
			}
		}
		catch (Exception ex)
		{
			Log.w(devicename, ex.Message);
			try
			{
				Log.w(devicename, "exit process");
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex.Message + "\r\n" + ex.StackTrace;
				}
			}
			catch (Exception ex2)
			{
				Log.w(devicename, ex2.Message);
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex2.Message + "\r\n" + ex2.StackTrace;
				}
			}
		}
	}

	public string consoleMode_Execute_returnLine(string deviceName, string command, int seconds)
	{
		devicename = deviceName;
		StringBuilder stringBuilder = new StringBuilder();
		if (command != null && !command.Equals(""))
		{
			try
			{
				Process process = (mprocess = initCmdProcess(command));
				foreach (Device flashDevice in FlashingDevice.flashDeviceList)
				{
					if (flashDevice.Name == devicename)
					{
						flashDevice.DCmd = this;
					}
				}
				process.OutputDataReceived += consoleMode_process_OutputDataReceived;
				process.ErrorDataReceived += consoleMode_process_ErrorDataReceived;
				process.EnableRaisingEvents = true;
				process.Exited += consoleMode_process_Exited;
				if (process.Start())
				{
					Log.w(devicename, "Physical Memory Usage:" + process.WorkingSet64 + " Byte");
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				Log.w(deviceName, ex, stopFlash: false);
				FlashingDevice.UpdateDeviceStatus(devicename, null, ex.Message, "error", isDone: true);
			}
			finally
			{
				if (mprocess != null)
				{
					mprocess.Close();
					mprocess.Dispose();
					Log.w(devicename, "process exit.");
				}
				FlashDone();
			}
		}
		return stringBuilder.ToString();
	}

	private void consoleMode_process_Exited(object sender, EventArgs e)
	{
		try
		{
			Process obj = (Process)sender;
			obj.CancelErrorRead();
			obj.CancelOutputRead();
			obj.Refresh();
		}
		catch (Exception ex)
		{
			Log.w(devicename, ex.Message, throwEx: false);
		}
		finally
		{
			FlashDone();
		}
	}

	private void consoleMode_process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.Data))
		{
			return;
		}
		try
		{
			string data = e.Data;
			if ((!string.IsNullOrEmpty(errMsg) || data.ToLower().IndexOf("error") >= 0 || data.ToLower().IndexOf("fail") >= 0 || data == "Missmatching image and device") && string.IsNullOrEmpty(errMsg))
			{
				errMsg = data;
			}
			Log.w(devicename, e.Data, throwEx: false);
			FlashingDevice.UpdateDeviceStatus(devicename, null, e.Data, "flashing", isDone: false);
		}
		catch (Exception ex)
		{
			try
			{
				_ = (Process)sender;
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex.Message + "\r\n" + ex.StackTrace;
				}
			}
			catch (Exception ex2)
			{
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex2.Message + "\r\n" + ex2.StackTrace;
				}
			}
		}
	}

	private void consoleMode_process_OutputDataReceived(object sender, DataReceivedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.Data))
		{
			return;
		}
		try
		{
			string data = e.Data;
			if (data.ToLower().IndexOf("no insert comport") >= 0)
			{
				try
				{
					_ = (Process)sender;
					errMsg = "no device insert";
				}
				catch (Exception ex)
				{
					Log.w(ex.Message, throwEx: true);
				}
			}
			if (!string.IsNullOrEmpty(errMsg) || data.ToLower().IndexOf("error") >= 0 || data.ToLower().IndexOf("fail") >= 0)
			{
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = data;
				}
				_ = (Process)sender;
				Log.w(devicename, data, throwEx: false);
				return;
			}
			if (data.ToLower().IndexOf("insert comport: com") >= 0)
			{
				string text = data.ToLower().Replace("insert comport:", "").Trim();
				Device device = new Device();
				device.Index = int.Parse(text.Replace("com", ""));
				device.Name = text;
				device.StartTime = DateTime.Now;
				devicename = device.Name;
				device.Status = "flashing";
				device.Progress = 0f;
				device.IsDone = false;
				device.IsUpdate = true;
				if (devicename.ToLower() == device.Name.ToLower() && UsbDevice.MtkDevice.IndexOf(device.Name) < 0)
				{
					UsbDevice.MtkDevice.Add(device.Name);
					FlashingDevice.consoleMode_UsbInserted = true;
				}
				Log.w(devicename, data);
			}
			else if (data.ToLower().IndexOf("da usb vcom") >= 0)
			{
				Match match = new Regex("\\(com.*\\)").Match(data.ToLower());
				if (match.Groups.Count > 0)
				{
					string s = match.Groups[0].Value.Replace("com", "").Replace("(", "").Replace(")", "")
						.Trim();
					if (ini.GetIniInt("bootrom", devicename, 0) == 0 && !ini.WriteIniInt("bootrom", devicename, int.Parse(s)))
					{
						MessageBox.Show("couldn't write vcom");
					}
				}
			}
			else if (data.ToLower().IndexOf("download success") >= 0)
			{
				try
				{
					Log.w(devicename, e.Data, throwEx: false);
					FlashDone();
					return;
				}
				catch (Exception ex2)
				{
					Log.w(devicename, ex2.Message, throwEx: false);
					return;
				}
			}
			string status = data;
			float? progress = null;
			if (data.ToLower().IndexOf("percent") >= 0)
			{
				string text2 = data.ToLower();
				string text3 = "percent";
				int num = text2.IndexOf(text3);
				status = text2.Substring(0, num);
				string value = text2.Substring(num + text3.Length, text2.Length - text3.Length - num).Trim();
				progress = (float)Convert.ToInt32(value) / 100f;
			}
			else
			{
				Log.w(devicename, e.Data, throwEx: true);
			}
			FlashingDevice.UpdateDeviceStatus(devicename, progress, status, "flashing", isDone: false);
		}
		catch (Exception ex3)
		{
			try
			{
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = ex3.Message + "\r\n" + ex3.StackTrace;
				}
				_ = (Process)sender;
			}
			catch (Exception ex4)
			{
				Log.w(ex4.Message, throwEx: true);
			}
		}
	}

	private void FlashDone()
	{
		Log.w(devicename, "begin FlashDone");
		if (string.IsNullOrEmpty(errMsg))
		{
			Log.w(devicename, "errMsg is null");
			bool flag = false;
			if (!string.IsNullOrEmpty(MiAppConfig.Get("checkPoint")))
			{
				Log.w(devicename, "begin checkPoint");
				foreach (Device flashDevice in FlashingDevice.flashDeviceList)
				{
					if (!(flashDevice.Name == devicename))
					{
						continue;
					}
					foreach (string item in new List<string>(flashDevice.StatusList))
					{
						if (new Regex(MiAppConfig.Get("checkPoint")).IsMatch(item.ToLower()) && item.ToLower().IndexOf("bootloader") < 0)
						{
							flag = true;
							Log.w(devicename, string.Format("catch checkpoint ({0})", MiAppConfig.Get("checkPoint")));
							break;
						}
					}
					break;
				}
			}
			else
			{
				Log.w(devicename, "no need checkPoint");
				flag = true;
			}
			if (flag)
			{
				FlashingDevice.UpdateDeviceStatus(devicename, 1f, "flash done", "success", isDone: true);
				Log.w(devicename, "flash done");
			}
			else
			{
				if (string.IsNullOrEmpty(errMsg))
				{
					errMsg = "Not catch checkpoint (" + MiAppConfig.Get("checkPoint") + "),flash is not done";
				}
				FlashingDevice.UpdateDeviceStatus(devicename, 1f, errMsg, "error", isDone: true);
				Log.w(devicename, "error:" + errMsg, throwEx: false);
			}
		}
		else
		{
			FlashingDevice.UpdateDeviceStatus(devicename, 1f, errMsg, "error", isDone: true);
			Log.w(devicename, "error:" + errMsg, throwEx: false);
		}
		UsbDevice.MtkDevice.Remove(devicename);
	}

	private Process initCmdProcess(string command)
	{
		return new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = "/C (" + command + ")",
				UseShellExecute = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			}
		};
	}

	private float? GetPercent(string line)
	{
		Hashtable dummyProgress = SoftwareImage.DummyProgress;
		float? result = null;
		foreach (string key in dummyProgress.Keys)
		{
			if (line.IndexOf(key) >= 0)
			{
				return (float)Convert.ToInt32(dummyProgress[key]) / 50f;
			}
		}
		return result;
	}

	public void KillProcessAndChildrens(string devicename, int pid)
	{
		Log.w(devicename, "Kill Process " + pid + " And Child Process");
		ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid).Get();
		try
		{
			Process processById = Process.GetProcessById(pid);
			if (!processById.HasExited)
			{
				processById.Kill();
			}
		}
		catch (ArgumentException)
		{
		}
		if (managementObjectCollection == null)
		{
			return;
		}
		foreach (ManagementObject item in managementObjectCollection)
		{
			KillProcessAndChildrens(devicename, Convert.ToInt32(item["ProcessID"]));
		}
	}
}
