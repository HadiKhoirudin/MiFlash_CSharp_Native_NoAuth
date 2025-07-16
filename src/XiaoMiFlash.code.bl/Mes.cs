using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class Mes
{
	public static string kWIPPathName = "C:/eBook_Test";

	public static string kWIPHandleFileName = "C:/eBook_Test/Handle.txt";

	public static int m_hMonitorProcessHandle = 0;

	[DllImport("User32.dll")]
	private static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

	private static bool GetEqualString(string szFileName, string szKeyName, ref string szReturnedString)
	{
		if (File.Exists(szFileName))
		{
			StreamReader streamReader = new StreamReader(szFileName, Encoding.Default);
			bool flag = false;
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (text.Contains(szKeyName))
				{
					flag = true;
					string[] array = text.Split('=');
					szReturnedString = array[1];
				}
			}
			streamReader.Close();
			if (!flag)
			{
				Log.wFlashDebug("MES GetEqualString: " + szKeyName + " FAIL");
				return false;
			}
			return true;
		}
		Log.wFlashDebug("not exist： " + szFileName);
		return false;
	}

	public static bool Init(string sParams, out string msg)
	{
		msg = "";
		string szReturnedString = "";
		if (File.Exists(kWIPHandleFileName))
		{
			GetEqualString(kWIPHandleFileName, "Handle", ref szReturnedString);
			m_hMonitorProcessHandle = Convert.ToInt32(szReturnedString);
			return true;
		}
		msg = "MES " + kWIPHandleFileName + " not create";
		return false;
	}

	public static bool CheckSN(string sDeviceID, int idex, out string swPath, out string sFsn, out string msg)
	{
		string text = "";
		swPath = "";
		sFsn = "";
		msg = "";
		string path = kWIPPathName + $"\\{idex}_WIP.txt";
		text = kWIPPathName + $"\\{idex}_WIP_INFO.txt";
		FileStream fileStream = new FileStream(path, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		streamWriter.Write("WIP_NO=" + sDeviceID);
		streamWriter.Flush();
		streamWriter.Close();
		fileStream.Close();
		SendMessage(m_hMonitorProcessHandle, 2050, 0, idex);
		bool flag = false;
		int num = 10;
		for (int i = 0; i < num; i++)
		{
			if (File.Exists(text))
			{
				flag = true;
				break;
			}
			Thread.Sleep(1000);
		}
		if (!flag)
		{
			msg = $"MES {idex}_WIP_INFO.txt NOT EXIST";
			return false;
		}
		string szReturnedString = "";
		if (!GetEqualString(text, "DB_PSN", ref szReturnedString))
		{
			msg = string.Format("GetEqualString {0} FAIL", "DB_PSN");
			return false;
		}
		sFsn = szReturnedString;
		if (!GetEqualString(text, "PERMISSION", ref szReturnedString))
		{
			msg = string.Format("GetEqualString {0} FAIL", "PERMISSION");
			return false;
		}
		if (!szReturnedString.Contains("TRUE"))
		{
			msg = "MES " + text + " CHECKSN FAIL";
			if (File.GetAttributes(text).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(text, FileAttributes.Normal);
			}
			File.Delete(text);
			return false;
		}
		if (!GetEqualString(text, "DB_SWPATH", ref szReturnedString))
		{
			msg = string.Format("GetEqualString {0} FAIL", "DB_SWPATH");
			return false;
		}
		swPath = szReturnedString;
		if (File.GetAttributes(text).ToString().IndexOf("ReadOnly") != -1)
		{
			File.SetAttributes(text, FileAttributes.Normal);
		}
		File.Delete(text);
		return true;
	}

	public bool SignEdl(int idex, int handle, string originKey, out string sigKey, out string msg)
	{
		sigKey = "";
		msg = "";
		string text = $"C:/eBook_Test/{idex}_DL_DATA.txt";
		string text2 = $"C:/eBook_Test/{idex}_DL_KEY.txt";
		FileStream fileStream = new FileStream(text, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		if (File.Exists(text))
		{
			streamWriter.Write(string.Format("CL_ID=qocmFlash\r\n", idex));
			streamWriter.Write("DL_Data=" + originKey + "\r\n");
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
			SendMessage(handle, 2057, 0, idex);
			bool flag = false;
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if (File.Exists(text2))
				{
					flag = true;
					break;
				}
				Thread.Sleep(1000);
			}
			if (!flag)
			{
				msg = "MES " + text2 + " NOT EXIST";
				return false;
			}
			string szReturnedString = "";
			if (!GetEqualString(text2, "Authority", ref szReturnedString))
			{
				msg = string.Format("GetEqualString {0} FAIL", "Authority");
				return false;
			}
			if (!szReturnedString.Contains("TRUE"))
			{
				msg = "MES " + text2 + " SignEdl NOT TRUE";
				if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
				{
					File.SetAttributes(text2, FileAttributes.Normal);
				}
				File.Delete(text2);
				return false;
			}
			if (!GetEqualString(text2, "DL_KEY", ref szReturnedString))
			{
				msg = string.Format("GetEqualString {0} FAIL", "DL_KEY");
				return false;
			}
			sigKey = szReturnedString;
			if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(text2, FileAttributes.Normal);
			}
			File.Delete(text2);
			return true;
		}
		msg = "MES " + text + " not create";
		return false;
	}

	public static bool SaveResult(string sDeviceID, int idex, bool sParams, out string msg)
	{
		Log.w(sDeviceID, "Mes.SaveResult begin");
		string text = "";
		string text2 = "";
		text = $"C:/eBook_Test/{idex}.txt";
		text2 = $"C:/eBook_Test/{idex}_RES_INFO.txt";
		msg = "";
		FileStream fileStream = new FileStream(text, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		if (File.Exists(text))
		{
			streamWriter.Write("[TEST_ITEMS]\r\n");
			streamWriter.Write("FUNCTION_NAME\tTEST_ITEM\tHI_LIMIT\tLOW_LIMIT\tVALUE\tSTATUS\tTEST_TIME\tERROR_CODE\r\n");
			streamWriter.Write(sParams);
			streamWriter.Write("[TEST_HEADER]\r\n");
			streamWriter.Write($"\r\nWIP_ID={idex}\r\n");
			streamWriter.Write("WIP_NO=" + sDeviceID + "\r\n");
			streamWriter.Write($"IS_PASSED={(sParams ? 1 : 0)}\r\n");
			streamWriter.Write("[END]");
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
			SendMessage(m_hMonitorProcessHandle, 2048, idex, sParams ? 1 : 0);
			bool flag = false;
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if (File.Exists(text2))
				{
					flag = true;
					break;
				}
				Thread.Sleep(1000);
			}
			if (!flag)
			{
				msg = "MES " + text2 + " not exit";
				Log.w(sDeviceID, "MES " + text2 + " not exit");
				return false;
			}
			string szReturnedString = "";
			GetEqualString(text2, "PERMISSION", ref szReturnedString);
			bool num2 = szReturnedString.Contains("TRUE");
			Log.w(sDeviceID, "Mes get PERMISSION :" + szReturnedString);
			if (!num2)
			{
				msg = "MES: PERMISSION not TRUE " + szReturnedString;
				if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
				{
					File.SetAttributes(text2, FileAttributes.Normal);
				}
				File.Delete(text2);
				return false;
			}
			if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(text2, FileAttributes.Normal);
			}
			File.Delete(text2);
			return true;
		}
		msg = "MES " + text + " not create";
		return false;
	}
}
