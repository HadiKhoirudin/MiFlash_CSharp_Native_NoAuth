using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OpenMultiImageResponse
{
	public byte uResponse;

	public byte uStatus;
}
