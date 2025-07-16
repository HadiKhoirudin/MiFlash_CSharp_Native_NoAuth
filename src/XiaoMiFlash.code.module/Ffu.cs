namespace XiaoMiFlash.code.module;

public class Ffu
{
	private string _name;

	private string _version;

	private string _number;

	private string _file;

	private string _size;

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public string Version
	{
		get
		{
			return _version;
		}
		set
		{
			_version = value;
		}
	}

	public string Number
	{
		get
		{
			return _number;
		}
		set
		{
			_number = value;
		}
	}

	public string File
	{
		get
		{
			return _file;
		}
		set
		{
			_file = value;
		}
	}

	public string Size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
		}
	}
}
