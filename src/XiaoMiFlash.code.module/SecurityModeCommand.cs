using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SecurityModeCommand
{
	public byte uCommand;

	public byte uMode;
}
