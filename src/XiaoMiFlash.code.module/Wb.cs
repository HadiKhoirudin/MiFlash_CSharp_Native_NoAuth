namespace XiaoMiFlash.code.module;

public class Wb
{
	private string _vendor;

	private string _partnumber;

	private string _wb_size_in_MB;

	private string _hpb_total_range_in_MB;

	public string Vendor
	{
		get
		{
			return _vendor;
		}
		set
		{
			_vendor = value;
		}
	}

	public string Partnumber
	{
		get
		{
			return _partnumber;
		}
		set
		{
			_partnumber = value;
		}
	}

	public string Wb_size_in_MB
	{
		get
		{
			return _wb_size_in_MB;
		}
		set
		{
			_wb_size_in_MB = value;
		}
	}

	public string Hpb_total_range_in_MB
	{
		get
		{
			return _hpb_total_range_in_MB;
		}
		set
		{
			_hpb_total_range_in_MB = value;
		}
	}
}
