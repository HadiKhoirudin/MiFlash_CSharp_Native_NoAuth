using System;

namespace XiaoMiFlash.code.bl;

public abstract class DeviceCtrl
{
	public string swPath = "D:\\SW\\A1\\FDL153I\\images\\";

	public string scatter = "";

	public ushort idproduct;

	public ushort idvendor;

	public string da = "";

	public string dl_type = "";

	public string sha256Path = "";

	public string deviceName = "";

	public string flashScript;

	public bool readBackVerify;

	public bool openWriteDump;

	public bool openReadDump;

	public bool verbose;

	public DateTime startTime;

	public abstract void flash();

	public abstract string[] getDevice();

	public abstract void CheckSha256();
}
