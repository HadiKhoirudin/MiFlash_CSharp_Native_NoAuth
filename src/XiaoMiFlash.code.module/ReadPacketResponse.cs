using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ReadPacketResponse
{
	public byte uResponse;

	public int uAddress;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
	public byte[] uData;
}
