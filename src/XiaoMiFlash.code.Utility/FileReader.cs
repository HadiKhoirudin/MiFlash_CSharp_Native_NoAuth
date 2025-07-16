using System;
using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.Utility;

internal class FileReader
{
	private const uint GENERIC_READ = 2147483648u;

	private const uint OPEN_EXISTING = 3u;

	private IntPtr handle;

	[DllImport("kernel32", SetLastError = true)]
	public static extern IntPtr CreateFile(string FileName, uint DesiredAccess, uint ShareMode, uint SecurityAttributes, uint CreationDisposition, uint FlagsAndAttributes, int hTemplateFile);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool CloseHandle(IntPtr hObject);

	public IntPtr Open(string FileName)
	{
		handle = CreateFile(FileName, 2147483648u, 0u, 0u, 3u, 0u, 0);
		if (handle != IntPtr.Zero)
		{
			return handle;
		}
		throw new Exception("打开文件失败");
	}

	public bool Close()
	{
		return CloseHandle(handle);
	}
}
