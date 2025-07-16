using System;
using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.module;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct UserInfo
{
	public IntPtr user_id;

	public IntPtr user_name;

	public IntPtr user_icon;

	public int user_icon_len;
}
