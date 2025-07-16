using System;
using System.Runtime.InteropServices;
using MiUSB;

namespace XiaoMiFlash;

public class Test
{
	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
	public static extern bool DeviceIoControl(IntPtr hFile, int dwIoControlCode, ref byte[] lpInBuffer, int nInBufferSize, ref byte[] lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

	public static UsbNodeInformation[] GetUsbNodeInformation(string DevicePath)
	{
		if (string.IsNullOrEmpty(DevicePath))
		{
			return null;
		}
		IntPtr intPtr = Kernel32.CreateFile("\\\\.\\" + DevicePath, NativeFileAccess.GENERIC_WRITE, NativeFileShare.FILE_SHARE_WRITE, IntPtr.Zero, NativeFileMode.OPEN_EXISTING, IntPtr.Zero, IntPtr.Zero);
		if (intPtr == Kernel32.INVALID_HANDLE_VALUE)
		{
			return null;
		}
		byte[] lpInBuffer = new byte[76];
		DeviceIoControl(intPtr, 2229256, ref lpInBuffer, Marshal.SizeOf((object)lpInBuffer), ref lpInBuffer, Marshal.SizeOf((object)lpInBuffer), out var _, IntPtr.Zero);
		Marshal.GetLastWin32Error();
		Kernel32.CloseHandle(intPtr);
		return null;
	}
}
