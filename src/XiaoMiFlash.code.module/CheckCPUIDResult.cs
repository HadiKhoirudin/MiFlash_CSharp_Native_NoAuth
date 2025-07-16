namespace XiaoMiFlash.code.module;

public class CheckCPUIDResult
{
	private string _device;

	private bool _bool;

	private string _path;

	private string _msg;

	public string Device
	{
		get
		{
			return _device;
		}
		set
		{
			_device = value;
		}
	}

	public bool Result
	{
		get
		{
			return _bool;
		}
		set
		{
			_bool = value;
		}
	}

	public string Path
	{
		get
		{
			return _path;
		}
		set
		{
			_path = value;
		}
	}

	public string Msg
	{
		get
		{
			return _msg;
		}
		set
		{
			_msg = value;
		}
	}
}
