using System.IO;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class SwDownloader
{
	private FtpHelper ftp;

	public string dlReturnPath;

	public SwDownloader()
	{
		ftp = new FtpHelper("10.237.107.75", "SW", "daemon", "xampp");
	}

	public void ThreadDownloadSw()
	{
		DownloadSw(dlReturnPath);
	}

	public void DownloadSw(string dllReturnPath)
	{
		string text = dllReturnPath.Substring(dllReturnPath.IndexOf("\\"));
		string swFileName = dllReturnPath.Substring(dllReturnPath.LastIndexOf("\\")).Replace("\\", "");
		ftp.swFileName = swFileName;
		string dic = ftp.ftpRemotePath + "/" + text;
		Directory.CreateDirectory(dllReturnPath);
		ftp.DownloadAllInDic(dic, dllReturnPath);
		MiFlashGlobal.Swdes.isDone = true;
	}
}
