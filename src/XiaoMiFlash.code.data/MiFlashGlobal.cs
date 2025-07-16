using System.Collections.Generic;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.data;

public class MiFlashGlobal
{
	private static string _version;

	private static bool _isfactory;

	private static bool _isFirmwarewrite;

	private static bool _isEraseAll;

	private static bool _isBackupOnly;

	private static List<string> _downloadTag;

	private static SwDes _swdes;

	public static string Version
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

	public static bool IsFactory
	{
		get
		{
			return _isfactory;
		}
		set
		{
			_isfactory = value;
		}
	}

	public static bool IsFirmwarewrite
	{
		get
		{
			return _isFirmwarewrite;
		}
		set
		{
			_isFirmwarewrite = value;
		}
	}

	public static bool IsEraseAll
	{
		get
		{
			return _isEraseAll;
		}
		set
		{
			_isEraseAll = value;
		}
	}

	public static bool IsBackupOnly
	{
		get
		{
			return _isBackupOnly;
		}
		set
		{
			_isBackupOnly = value;
		}
	}

	public static List<string> DownloadTag
	{
		get
		{
			return _downloadTag;
		}
		set
		{
			_downloadTag = value;
		}
	}

	public static SwDes Swdes
	{
		get
		{
			return _swdes;
		}
		set
		{
			_swdes = value;
		}
	}
}
