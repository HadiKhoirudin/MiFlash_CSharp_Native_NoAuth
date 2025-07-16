using System.Collections;

namespace XiaoMiFlash.code.module;

public class SoftwareImage
{
	public static string ProgrammerPattern => "prog_.*_firehose_.*.*|prog_.*firehose_.*.*|prog_firehose_*.elf|xbl_.*devprg_.*.*";

	public static string MTKDAPattern => "MTK_AllInOne_DA+\\w*.bin";

	public static string ProgrammerLite => "prog_firehose_lite.elf";

	public static string ProgrammerDDR4 => "prog_ufs_firehose_sm8250_ddr_4.elf";

	public static string ProgrammerDDR5 => "prog_ufs_firehose_sm8250_ddr_5.elf";

	public static string BootImage => "*_msimage.mbn";

	public static string ProvisionPattern => "provision.*\\.xml";

	public static string RawBackupPattern => "rawprogram_backup.xml";

	public static string RawRestorePattern => "rawprogram_restore.xml";

	public static string RawProgramPattern => "rawprogram[0-9]{1,20}\\.xml";

	public static string PatchPattern => "patch[0-9]{1,20}\\.xml";

	public static Hashtable DummyProgress => new Hashtable
	{
		{ "xbl", 1 },
		{ "tz", 2 },
		{ "hyp", 3 },
		{ "rpm", 4 },
		{ "emmc_appsboot", 5 },
		{ "pmic", 6 },
		{ "devcfg", 7 },
		{ "BTFM", 8 },
		{ "cmnlib", 9 },
		{ "cmnlib64", 10 },
		{ "NON-HLOS", 11 },
		{ "adspso", 12 },
		{ "mdtp", 13 },
		{ "keymaster", 14 },
		{ "misc", 15 },
		{ "system", 16 },
		{ "cache", 30 },
		{ "userdata", 34 },
		{ "recovery", 35 },
		{ "splash", 36 },
		{ "logo", 37 },
		{ "boot", 38 },
		{ "cust", 45 }
	};
}
