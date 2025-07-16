using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace XiaoMiFlash.code.Utility;

public class DeviceClasses
{
	[StructLayout(LayoutKind.Sequential)]
	private class SP_DEVINFO_DATA
	{
		public int cbSize;

		public Guid ClassGuid;

		public int DevInst;

		public ulong Reserved;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class SP_CLASSIMAGELIST_DATA
	{
		public int cbSize;

		public ImageList ImageList;

		public ulong Reserved;
	}

	public struct RECT
	{
		private long left;

		private long top;

		private long right;

		private long bottom;
	}

	public static Guid ClassesGuid;

	public const int MAX_SIZE_DEVICE_DESCRIPTION = 1000;

	public const int CR_SUCCESS = 0;

	public const int CR_NO_SUCH_VALUE = 37;

	public const int CR_INVALID_DATA = 31;

	private const int DIGCF_PRESENT = 2;

	private const int DIOCR_INSTALLER = 1;

	private const int MAXIMUM_ALLOWED = 33554432;

	public const int DMI_MASK = 1;

	public const int DMI_BKCOLOR = 2;

	public const int DMI_USERECT = 4;

	public const int DIGCF_DEFAULT = 1;

	public const int DIGCF_ALLCLASSES = 4;

	public const int DIGCF_PROFILE = 8;

	public const int DIGCF_DEVICEINTERFACE = 16;

	[DllImport("cfgmgr32.dll")]
	private static extern uint CM_Enumerate_Classes(uint ClassIndex, ref Guid ClassGuid, uint Params);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiClassNameFromGuidA(ref Guid ClassGuid, StringBuilder ClassName, uint ClassNameSize, ref uint RequiredSize);

	[DllImport("setupapi.dll")]
	private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr hwndParent, uint Flags);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

	[DllImport("setupapi.dll")]
	private static extern IntPtr SetupDiOpenClassRegKeyExA(ref Guid ClassGuid, uint samDesired, int Flags, IntPtr MachineName, uint Reserved);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, SP_DEVINFO_DATA DeviceInfoData);

	[DllImport("advapi32.dll")]
	private static extern uint RegQueryValueA(IntPtr KeyClass, uint SubKey, StringBuilder ClassDescription, ref uint sizeB);

	[DllImport("user32.dll")]
	public static extern int LoadBitmapW(int hInstance, ulong Reserved);

	[DllImport("setupapi.dll")]
	public static extern bool SetupDiGetClassImageList(out SP_CLASSIMAGELIST_DATA ClassImageListData);

	[DllImport("setupapi.dll")]
	public static extern int SetupDiDrawMiniIcon(Graphics hdc, RECT rc, int MiniIconIndex, int Flags);

	[DllImport("setupapi.dll")]
	public static extern bool SetupDiGetClassBitmapIndex(Guid ClassGuid, out int MiniIconIndex);

	[DllImport("setupapi.dll")]
	public static extern int SetupDiLoadClassIcon(ref Guid classGuid, out IntPtr hIcon, out int index);

	public static int EnumerateClasses(uint ClassIndex, StringBuilder ClassName, StringBuilder ClassDescription, ref bool DevicePresent)
	{
		Guid ClassGuid = Guid.Empty;
		SP_DEVINFO_DATA sP_DEVINFO_DATA = new SP_DEVINFO_DATA();
		uint RequiredSize = 0u;
		uint num = CM_Enumerate_Classes(ClassIndex, ref ClassGuid, 0u);
		DevicePresent = false;
		new SP_CLASSIMAGELIST_DATA();
		if (num != 0)
		{
			return (int)num;
		}
		SetupDiClassNameFromGuidA(ref ClassGuid, ClassName, RequiredSize, ref RequiredSize);
		if (RequiredSize != 0)
		{
			ClassName.Capacity = (int)RequiredSize;
			SetupDiClassNameFromGuidA(ref ClassGuid, ClassName, RequiredSize, ref RequiredSize);
		}
		IntPtr deviceInfoSet = SetupDiGetClassDevs(ref ClassGuid, 0u, IntPtr.Zero, 2u);
		if (deviceInfoSet.ToInt32() == -1)
		{
			DevicePresent = false;
			return 0;
		}
		uint memberIndex = 0u;
		sP_DEVINFO_DATA.cbSize = 28;
		sP_DEVINFO_DATA.DevInst = 0;
		sP_DEVINFO_DATA.ClassGuid = Guid.Empty;
		sP_DEVINFO_DATA.Reserved = 0uL;
		if (!SetupDiEnumDeviceInfo(deviceInfoSet, memberIndex, sP_DEVINFO_DATA))
		{
			DevicePresent = false;
			return 0;
		}
		SetupDiDestroyDeviceInfoList(deviceInfoSet);
		IntPtr keyClass = SetupDiOpenClassRegKeyExA(ref ClassGuid, 33554432u, 1, IntPtr.Zero, 0u);
		if (keyClass.ToInt32() == -1)
		{
			DevicePresent = false;
			return 0;
		}
		uint sizeB = 1000u;
		ClassDescription.Capacity = 1000;
		if (RegQueryValueA(keyClass, 0u, ClassDescription, ref sizeB) != 0)
		{
			ClassDescription = new StringBuilder("");
		}
		DevicePresent = true;
		ClassesGuid = sP_DEVINFO_DATA.ClassGuid;
		return 0;
	}
}
