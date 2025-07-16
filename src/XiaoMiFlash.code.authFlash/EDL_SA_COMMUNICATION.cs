using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace XiaoMiFlash.code.authFlash;

public class EDL_SA_COMMUNICATION
{
	private static readonly object locker1 = new object();

	[DllImport("iacj_interprocess_dll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern void IACJ_Interp_Init(string pThis, string pEventProc, string pDataProc);

	[DllImport("iacj_interprocess_dll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern void IACJ_Interp_Finz();

	[DllImport("iacj_interprocess_dll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int IACJ_Interp_SendMsg2Peer(byte[] pBuf);

	public static void DaConnect()
	{
		IACJ_Interp_Init(null, null, null);
	}

	public static void DaDisConnect()
	{
		IACJ_Interp_Finz();
	}

	public static PInvokeResultArg RspEdlResult(string deivce, bool reuslt)
	{
		string text = null;
		string text2 = null;
		int num = -1;
		text2 = ((!reuslt) ? "fail" : "pass");
		text = "DLRESULT=" + deivce.Substring(3, deivce.Length - 3) + ":" + text2;
		num = IACJ_Interp_SendMsg2Peer(Encoding.Default.GetBytes(text));
		PInvokeResultArg obj = new PInvokeResultArg
		{
			result = num,
			lastErrCode = Marshal.GetLastWin32Error()
		};
		obj.lastErrMsg = new Win32Exception(obj.lastErrCode).Message;
		return obj;
	}
}
