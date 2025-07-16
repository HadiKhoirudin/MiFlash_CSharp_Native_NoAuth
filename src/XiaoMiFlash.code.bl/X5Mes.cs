using System;
using System.Runtime.InteropServices;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

internal class X5Mes
{
	private IntPtr mhHandle = IntPtr.Zero;

	private IntPtr ErrorMessage = IntPtr.Zero;

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr CreateObject(int type);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr ReleaseObject(IntPtr mhHandle);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr Init(IntPtr mhHandle, IntPtr szDumpLogFolder);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr VerifySN(IntPtr mhHandle, IntPtr pSN);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr WriteTestResult(IntPtr mhHandle, bool bAllTestPassed, int nUploadingFileNum = 0);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr Final(IntPtr mhHandle);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr SetEQPID(IntPtr mhHandle, IntPtr szEQP_ID);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr GetDownLoadKEY_PARMS(IntPtr mhHandle);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr GetWIPX5Data(IntPtr mhHandle, IntPtr key);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr AddTestResult(IntPtr mhHandle, IntPtr szTestItemLog);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr SetMiflashTokenClientID(IntPtr mhHandle, IntPtr szMiflashClient_ID, IntPtr szMiflashTOKEN);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr GetIoMiFlashData(IntPtr mhHandle);

	[DllImport("X5MesLibrary.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
	public static extern IntPtr GetPDLData(IntPtr mhHandle);

	public void MyTest()
	{
		Init("vboytest");
		string swPath = "";
		string sFsn = "";
		CheckSN("vboytest", "0x7660c87a", out sFsn, out swPath);
	}

	public bool Init(string deviceName)
	{
		mhHandle = CreateObject(0);
		ErrorMessage = Init(mhHandle, Marshal.StringToHGlobalAnsi("D:\\TestX5\\test.rec"));
		if (ErrorMessage != IntPtr.Zero)
		{
			Log.w(deviceName, "InitFactory Fail" + Marshal.PtrToStringAnsi(ErrorMessage));
			return false;
		}
		string text = MiAppConfig.Get("EQPID");
		Log.w(deviceName, "EQPID：" + text);
		ErrorMessage = SetEQPID(mhHandle, Marshal.StringToHGlobalAnsi(text));
		if (ErrorMessage != IntPtr.Zero)
		{
			Log.w(deviceName, "SetEQPID Fail" + Marshal.PtrToStringAnsi(ErrorMessage));
			return false;
		}
		return true;
	}

	public bool CheckSN(string deviceName, string cpuId, out string sFsn, out string swPath)
	{
		swPath = "";
		sFsn = "";
		ErrorMessage = VerifySN(mhHandle, Marshal.StringToHGlobalAnsi(cpuId));
		if (ErrorMessage != IntPtr.Zero)
		{
			Log.w(deviceName, "VerifySN err：" + Marshal.PtrToStringAnsi(ErrorMessage));
			return false;
		}
		Marshal.AllocHGlobal(IntPtr.Zero);
		string text = Marshal.PtrToStringAnsi(GetDownLoadKEY_PARMS(mhHandle));
		Log.w(deviceName, "strKeyRes " + text);
		string[] array = text.Split(',');
		IntPtr zero = IntPtr.Zero;
		for (int i = 0; i < array.Length - 1; i++)
		{
			zero = GetWIPX5Data(mhHandle, Marshal.StringToHGlobalAnsi(array[i]));
			Log.w(deviceName, "valueString " + Marshal.PtrToStringAnsi(zero));
		}
		zero = GetWIPX5Data(mhHandle, Marshal.StringToHGlobalAnsi("DB_PSN"));
		sFsn = Marshal.PtrToStringAnsi(zero);
		zero = GetWIPX5Data(mhHandle, Marshal.StringToHGlobalAnsi("DB_SWPATH"));
		swPath = Marshal.PtrToStringAnsi(zero);
		return true;
	}

	public bool SaveResult(string deviceName, bool result)
	{
		string s = string.Format("PDL^PDL^30.000^0.000^60.000^{0}^-^-", result ? "PASS" : "FAIL");
		ErrorMessage = AddTestResult(mhHandle, Marshal.StringToHGlobalAnsi(s));
		if (ErrorMessage != IntPtr.Zero)
		{
			Log.w(deviceName, "AddTestResult err" + Marshal.PtrToStringAnsi(ErrorMessage));
			return false;
		}
		ErrorMessage = WriteTestResult(mhHandle, result);
		if (ErrorMessage != IntPtr.Zero)
		{
			Log.w(deviceName, "WriteTestResult err" + Marshal.PtrToStringAnsi(ErrorMessage));
			return false;
		}
		return true;
	}

	public void Release(string sParams)
	{
		try
		{
			ErrorMessage = ReleaseObject(mhHandle);
			if (ErrorMessage != IntPtr.Zero)
			{
				throw new Exception(Marshal.PtrToStringAnsi(ErrorMessage));
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
}
