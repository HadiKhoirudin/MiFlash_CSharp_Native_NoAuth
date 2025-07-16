using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XiaoMiFlash.code.module;

public class MiProperty
{
	private static Hashtable _dltable;

	private static Dictionary<string, int> _chksumtable;

	private static string[] _dalist;

	public static Hashtable DlTable
	{
		get
		{
			_dltable = new Hashtable();
			_dltable.Add("download_only", "dl_only");
			_dltable.Add("format_and_download", "fm_and_dl");
			_dltable.Add("firmware_upgrade", "firmware_upgrade");
			_dltable.Add("format_all", "fm");
			return _dltable;
		}
	}

	public static Dictionary<string, int> ChkSumTable
	{
		get
		{
			_chksumtable = new Dictionary<string, int>();
			_chksumtable.Add("None", 0);
			_chksumtable.Add("Usb+dram checksum", 1);
			_chksumtable.Add("Flash checksum", 2);
			_chksumtable.Add("All checksum", 3);
			return _chksumtable;
		}
	}

	public static string[] DaList
	{
		get
		{
			string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\da";
			if (Directory.Exists(path))
			{
				FileInfo[] files = new DirectoryInfo(path).GetFiles();
				_dalist = new string[files.Count()];
				for (int i = 0; i < files.Count(); i++)
				{
					_dalist[i] = files[i].Name;
				}
			}
			return _dalist;
		}
	}
}
