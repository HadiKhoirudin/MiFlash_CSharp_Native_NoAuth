using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StreamWriteResponse
{
	public byte uResponse;

	public int uAddress;
}
