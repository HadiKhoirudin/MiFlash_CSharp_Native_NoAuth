using System;
using System.Collections.Generic;
using System.Threading;
using XiaoMiFlash.code.bl;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.module;

public class Device
{
	private int _index;

	private string _name;

	private string _fsn;

	private float _progress;

	private DateTime _startTime;

	private float _elapse;

	private string _status;

	private List<string> _statuslist = new List<string>();

	private string _result = "";

	private bool? _isdone;

	private bool _isupdate;

	private DeviceCtrl _devicectrl;

	private Thread _thread;

	private Cmd _cmd;

	private Comm _comm;

	private bool _checkcpuid;

	private string _devicetype;

	private ushort _idproduct;

	private ushort _idvendor;

	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

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

	public string FSN
	{
		get
		{
			return _fsn;
		}
		set
		{
			_fsn = value;
		}
	}

	public float Progress
	{
		get
		{
			return _progress;
		}
		set
		{
			_progress = value;
		}
	}

	public DateTime StartTime
	{
		get
		{
			return _startTime;
		}
		set
		{
			_startTime = value;
		}
	}

	public float Elapse
	{
		get
		{
			return _elapse;
		}
		set
		{
			_elapse = value;
		}
	}

	public string Status
	{
		get
		{
			return _status;
		}
		set
		{
			_status = value;
		}
	}

	public List<string> StatusList
	{
		get
		{
			return _statuslist;
		}
		set
		{
			_statuslist = value;
		}
	}

	public string Result
	{
		get
		{
			return _result;
		}
		set
		{
			_result = value;
		}
	}

	public bool? IsDone
	{
		get
		{
			return _isdone;
		}
		set
		{
			_isdone = value;
		}
	}

	public bool IsUpdate
	{
		get
		{
			return _isupdate;
		}
		set
		{
			_isupdate = value;
		}
	}

	public DeviceCtrl DeviceCtrl
	{
		get
		{
			return _devicectrl;
		}
		set
		{
			_devicectrl = value;
		}
	}

	public Thread DThread
	{
		get
		{
			return _thread;
		}
		set
		{
			_thread = value;
		}
	}

	public Cmd DCmd
	{
		get
		{
			return _cmd;
		}
		set
		{
			_cmd = value;
		}
	}

	public Comm DComm
	{
		get
		{
			return _comm;
		}
		set
		{
			_comm = value;
		}
	}

	public bool CheckCPUID
	{
		get
		{
			return _checkcpuid;
		}
		set
		{
			_checkcpuid = value;
		}
	}

	public string DeviceType
	{
		get
		{
			return _devicetype;
		}
		set
		{
			_devicetype = value;
		}
	}

	public ushort IdProduct
	{
		get
		{
			return _idproduct;
		}
		set
		{
			_idproduct = value;
		}
	}

	public ushort IdVendor
	{
		get
		{
			return _idvendor;
		}
		set
		{
			_idvendor = value;
		}
	}
}
