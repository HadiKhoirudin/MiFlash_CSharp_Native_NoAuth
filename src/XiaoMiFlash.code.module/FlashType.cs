using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct FlashType
{
	public static string CleanAll = "flash_all.bat";

	public static string SaveUserData = "flash_all_except.*\\.bat";

	public static string CleanAllAndLock = "flash_all_lock.bat";

	public static string flash_all_lock_crc = "flash_all_lock_crc.bat";
}
