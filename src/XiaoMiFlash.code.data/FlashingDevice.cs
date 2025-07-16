using System;
using System.Collections.Generic;
using System.Linq;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.data;

public static class FlashingDevice
{
	public static bool consoleMode_UsbInserted = false;

	public static List<Device> flashDeviceList = new List<Device>();

	public static void UpdateDeviceStatus(string deviceName, float? progress, string status, string result, bool isDone)
	{
		try
		{
			foreach (Device flashDevice in flashDeviceList)
			{
				if (flashDevice.Name == deviceName)
				{
					if (!string.IsNullOrEmpty(status))
					{
						flashDevice.StatusList.Add(status);
					}
					if (progress.HasValue)
					{
						flashDevice.Progress = progress.Value;
					}
					if (!string.IsNullOrEmpty(status))
					{
						flashDevice.Status = status;
					}
					if (!string.IsNullOrEmpty(result))
					{
						flashDevice.Result = result;
					}
					flashDevice.IsDone = isDone;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message + "\r\n" + ex.StackTrace);
		}
	}

	public static void UpdateDeviceAllStop(string status, string result)
	{
		try
		{
			using List<Device>.Enumerator enumerator = flashDeviceList.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Device current = enumerator.Current;
				if (!string.IsNullOrEmpty(status))
				{
					current.StatusList.Add(status);
				}
				if (!string.IsNullOrEmpty(status))
				{
					current.Status = status;
				}
				if (!string.IsNullOrEmpty(result))
				{
					current.Result = result;
				}
				current.IsDone = true;
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message + "\r\n" + ex.StackTrace);
		}
	}

	public static List<Device> GetFlashDoneDs()
	{
		return flashDeviceList.Where((Device d) => !d.IsDone.HasValue || (d.IsDone.HasValue && d.IsDone.Value && !d.IsUpdate)).ToList();
	}

	public static bool IsAllDone()
	{
		return GetFlashDoneDs().Count == flashDeviceList.Count;
	}
}
