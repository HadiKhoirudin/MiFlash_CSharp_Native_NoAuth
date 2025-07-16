using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StreamWriteCommand
{
	public byte uCommand;

	public int uAddress;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
	public byte[] uData;
}
