using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.authFlash;

public class EDL_SLA_Challenge
{
	private static readonly object locker1 = new object();

	private static int chip_type;

	[DllImport("SLA_Challenge.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int StartLoginProcess();

	[DllImport("SLA_Challenge.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int get_user_info(IntPtr usr_arg, IntPtr user_info);

	[DllImport("SLA_Challenge.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int free_user_info(IntPtr usr_arg, IntPtr user_info);

	[DllImport("SLA_Challenge.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int SLA_Challenge(IntPtr obj, byte[] challenge_in, int in_len, out IntPtr challenge_out, ref int out_len);

	[DllImport("SLA_Challenge.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int SLA_ChallengeEx(IntPtr obj, byte[] challenge_in, int in_len, out IntPtr challenge_out, ref int out_len, ref int chip_type);

	[DllImport("SLA_Challenge.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int SLA_Challenge_End(IntPtr obj, IntPtr challenge_out);

	[DllImport("SLA_Challenge.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int can_flash(IntPtr obj);

	private static string decodeOut(string str)
	{
		char[] array = str.ToCharArray();
		Encoder encoder = Encoding.Unicode.GetEncoder();
		byte[] array2 = new byte[encoder.GetByteCount(array, 0, array.Length, flush: true)];
		encoder.GetBytes(array, 0, array.Length, array2, 0, flush: true);
		Decoder decoder = Encoding.UTF8.GetDecoder();
		char[] array3 = new char[decoder.GetCharCount(array2, 0, array2.Length)];
		decoder.GetChars(array2, 0, array2.Length, array3, 0);
		return new string(array3);
	}

	public static PInvokeResultArg GetUserInfo(MiUserInfo miUser)
	{
		IntPtr zero = IntPtr.Zero;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UserInfo)));
		int num = -1;
		lock (locker1)
		{
			num = get_user_info(zero, intPtr);
		}
		UserInfo userInfo = (UserInfo)Marshal.PtrToStructure(intPtr, typeof(UserInfo));
		miUser.id = Marshal.PtrToStringAnsi(userInfo.user_id);
		string str = Marshal.PtrToStringUni(userInfo.user_name);
		miUser.name = decodeOut(str);
		PInvokeResultArg pInvokeResultArg = new PInvokeResultArg();
		pInvokeResultArg.result = num;
		pInvokeResultArg.lastErrCode = Marshal.GetLastWin32Error();
		pInvokeResultArg.lastErrMsg = new Win32Exception(pInvokeResultArg.lastErrCode).Message;
		if (num != 0)
		{
			return pInvokeResultArg;
		}
		num = free_user_info(zero, intPtr);
		pInvokeResultArg.result = num;
		pInvokeResultArg.lastErrCode = Marshal.GetLastWin32Error();
		pInvokeResultArg.lastErrMsg = new Win32Exception(pInvokeResultArg.lastErrCode).Message;
		return pInvokeResultArg;
	}

	public static PInvokeResultArg AuthFlash()
	{
		IntPtr zero = IntPtr.Zero;
		int result = -1;
		lock (locker1)
		{
			result = can_flash(zero);
		}
		PInvokeResultArg obj = new PInvokeResultArg
		{
			result = result,
			lastErrCode = Marshal.GetLastWin32Error()
		};
		obj.lastErrMsg = new Win32Exception(obj.lastErrCode).Message;
		return obj;
	}

	private static void UpdateChipType(string chip = "Qualcomm")
	{
		switch (chip)
		{
			case "Qualcomm":
				chip_type = 1;
				break;
			default:
				chip_type = 1;
				break;
		}
	}

	public static PInvokeResultArg SignEdl(string orignKey, out string signedKey)
	{
		IntPtr zero = IntPtr.Zero;
		IntPtr challenge_out = default(IntPtr);
		int out_len = 0;
		UpdateChipType();
		int num = -1;
		lock (locker1)
		{
			num = SLA_ChallengeEx(zero, Encoding.Default.GetBytes(orignKey), orignKey.Length, out challenge_out, ref out_len, ref chip_type);
		}
		Log.w($"vboytest: out_size{out_len}");
		new StringBuilder();
		byte[] array = new byte[out_len];
		for (int i = 0; i < out_len; i++)
		{
			IntPtr ptr = new IntPtr(challenge_out.ToInt64() + Marshal.SizeOf(typeof(byte)) * i);
			array[i] = Marshal.ReadByte(ptr);
		}
		PInvokeResultArg pInvokeResultArg = new PInvokeResultArg();
		pInvokeResultArg.result = num;
		pInvokeResultArg.lastErrCode = Marshal.GetLastWin32Error();
		pInvokeResultArg.lastErrMsg = new Win32Exception(pInvokeResultArg.lastErrCode).Message;
		if (num != 0)
		{
			signedKey = string.Empty;
			return pInvokeResultArg;
		}
		byte[] array2 = array;
		StringBuilder stringBuilder = new StringBuilder();
		for (int j = 0; j < array2.Length; j++)
		{
			stringBuilder.Append(array2[j].ToString("X2"));
		}
		signedKey = stringBuilder.ToString();
		num = SLA_Challenge_End(zero, challenge_out);
		pInvokeResultArg.result = num;
		pInvokeResultArg.lastErrCode = Marshal.GetLastWin32Error();
		pInvokeResultArg.lastErrMsg = new Win32Exception(pInvokeResultArg.lastErrCode).Message;
		return pInvokeResultArg;
	}

	public static MiUserInfo authEDl(string devicename, out bool canFlash)
	{
		string text = "";
		MiUserInfo miUserInfo = new MiUserInfo();
		canFlash = false;
		try
		{
			PInvokeResultArg pInvokeResultArg = new PInvokeResultArg();
			Log.w(devicename, "GetUserInfo");
			pInvokeResultArg = GetUserInfo(miUserInfo);
			if (pInvokeResultArg.result == 0)
			{
				Log.w(devicename, "AuthFlash");
				pInvokeResultArg = AuthFlash();
				if (pInvokeResultArg.result == 1)
				{
					canFlash = true;
					return miUserInfo;
				}
				text = "Authentication required.";
				MessageBox.Show(text);
				Log.w($"{text} errcode:{pInvokeResultArg.lastErrCode} lasterr:{pInvokeResultArg.lastErrMsg}");
				return miUserInfo;
			}
			text = "Login failed.";
			MessageBox.Show(text);
			Log.w($"{text} errcode:{pInvokeResultArg.lastErrCode} lasterr:{pInvokeResultArg.lastErrMsg}");
			return miUserInfo;
		}
		catch (Exception ex)
		{
			text = ex.Message;
			Log.w("authentication edl error:" + text);
			return miUserInfo;
		}
	}
}
