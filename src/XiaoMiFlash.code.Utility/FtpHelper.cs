using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;

namespace XiaoMiFlash.code.Utility;

public class FtpHelper
{
	private string ftpServerIP;

	public string ftpRemotePath;

	private string ftpUserID;

	private string ftpPassword;

	public string ftpURI;

	public string localPath;

	public string downloadFile;

	public string swFileName;

	public FtpHelper(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
	{
		ftpServerIP = FtpServerIP;
		ftpRemotePath = FtpRemotePath;
		ftpUserID = FtpUserID;
		ftpPassword = FtpPassword;
		ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
	}

	public void Upload(string filename)
	{
		FileInfo fileInfo = new FileInfo(filename);
		FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + fileInfo.Name));
		ftpWebRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
		ftpWebRequest.Method = "STOR";
		ftpWebRequest.UseBinary = true;
		ftpWebRequest.UsePassive = false;
		ftpWebRequest.ContentLength = fileInfo.Length;
		int num = 2048;
		byte[] buffer = new byte[num];
		FileStream fileStream = fileInfo.OpenRead();
		try
		{
			Stream requestStream = ftpWebRequest.GetRequestStream();
			for (int num2 = fileStream.Read(buffer, 0, num); num2 != 0; num2 = fileStream.Read(buffer, 0, num))
			{
				requestStream.Write(buffer, 0, num2);
			}
			requestStream.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("Ftphelper Upload Error --> " + ex.Message);
		}
	}

	public void Download(string filePath, string fileName)
	{
		try
		{
			double num = GetFileSize(fileName);
			MiFlashGlobal.Swdes.fileSize = (long)num;
			FileStream fileStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + fileName));
			obj.Method = "RETR";
			obj.UseBinary = true;
			obj.UsePassive = false;
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			FtpWebResponse ftpWebResponse = (FtpWebResponse)obj.GetResponse();
			Stream responseStream = ftpWebResponse.GetResponseStream();
			_ = ftpWebResponse.ContentLength;
			int num2 = 2048;
			byte[] buffer = new byte[num2];
			int num3 = 0;
			int num4 = responseStream.Read(buffer, 0, num2);
			num3 += num4;
			while (num4 > 0)
			{
				fileStream.Write(buffer, 0, num4);
				num4 = responseStream.Read(buffer, 0, num2);
				num3 += num4;
				MiFlashGlobal.Swdes.serverPath = ftpURI + fileName;
				MiFlashGlobal.Swdes.localPath = fileStream.Name;
				MiFlashGlobal.Swdes.percent = (double)num3 / num * 100.0;
			}
			responseStream.Close();
			fileStream.Close();
			ftpWebResponse.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper Download Error --> " + ex.Message);
		}
	}

	public void ThreadDownload()
	{
		string filePath = localPath;
		string fileName = downloadFile;
		Download(filePath, fileName);
	}

	public void Delete(string fileName)
	{
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + fileName));
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			obj.Method = "DELE";
			obj.UsePassive = false;
			_ = string.Empty;
			FtpWebResponse obj2 = (FtpWebResponse)obj.GetResponse();
			_ = obj2.ContentLength;
			Stream responseStream = obj2.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			streamReader.ReadToEnd();
			streamReader.Close();
			responseStream.Close();
			obj2.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper Delete Error --> " + ex.Message + "  文件名:" + fileName);
		}
	}

	public void RemoveDirectory(string folderName)
	{
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + folderName));
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			obj.Method = "RMD";
			obj.UsePassive = false;
			_ = string.Empty;
			FtpWebResponse obj2 = (FtpWebResponse)obj.GetResponse();
			_ = obj2.ContentLength;
			Stream responseStream = obj2.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			streamReader.ReadToEnd();
			streamReader.Close();
			responseStream.Close();
			obj2.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper Delete Error --> " + ex.Message + "  文件名:" + folderName);
		}
	}

	public string[] GetFilesDetailList()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI));
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			obj.Method = "LIST";
			obj.UsePassive = false;
			WebResponse response = obj.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default);
			for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
			{
				stringBuilder.Append(text);
				stringBuilder.Append("\n");
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				stringBuilder.Remove(stringBuilder.ToString().LastIndexOf("\n"), 1);
			}
			streamReader.Close();
			response.Close();
			return stringBuilder.ToString().Split('\n');
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper  Error --> " + ex.Message);
		}
	}

	public string[] GetFileList(string mask)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI));
			obj.UseBinary = true;
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			obj.Method = "NLST";
			obj.UsePassive = false;
			WebResponse response = obj.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default);
			for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
			{
				if (mask.Trim() != string.Empty && mask.Trim() != "*.*")
				{
					string text2 = mask.Substring(0, mask.IndexOf("*"));
					if (text.Substring(0, text2.Length) == text2)
					{
						stringBuilder.Append(text);
						stringBuilder.Append("\n");
					}
				}
				else
				{
					stringBuilder.Append(text);
					stringBuilder.Append("\n");
				}
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				stringBuilder.Remove(stringBuilder.ToString().LastIndexOf('\n'), 1);
			}
			streamReader.Close();
			response.Close();
			return stringBuilder.ToString().Split('\n');
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper GetFileList Error --> " + ex.Message.ToString());
		}
	}

	public string[] GetDirectoryList()
	{
		string[] filesDetailList = GetFilesDetailList();
		string text = string.Empty;
		string[] array = filesDetailList;
		foreach (string text2 in array)
		{
			if (string.IsNullOrEmpty(text2))
			{
				continue;
			}
			int num = text2.IndexOf("<DIR>");
			if (num > 0)
			{
				text = text + text2.Substring(num + 5).Trim() + "\n";
			}
			else if (text2.Trim().Substring(0, 1).ToUpper() == "D")
			{
				string text3 = text2.Substring(54).Trim();
				if (text3 != "." && text3 != "..")
				{
					text = text + text3 + "\n";
				}
			}
		}
		char[] separator = new char[1] { '\n' };
		return text.Split(separator);
	}

	public bool DirectoryExist(string RemoteDirectoryName)
	{
		string[] directoryList = GetDirectoryList();
		for (int i = 0; i < directoryList.Length; i++)
		{
			if (directoryList[i].Trim() == RemoteDirectoryName.Trim())
			{
				return true;
			}
		}
		return false;
	}

	public bool FileExist(string RemoteFileName)
	{
		string[] fileList = GetFileList("*.*");
		for (int i = 0; i < fileList.Length; i++)
		{
			if (fileList[i].Trim() == RemoteFileName.Trim())
			{
				return true;
			}
		}
		return false;
	}

	public void MakeDir(string dirName)
	{
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + dirName));
			obj.Method = "MKD";
			obj.UseBinary = true;
			obj.UsePassive = false;
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			FtpWebResponse obj2 = (FtpWebResponse)obj.GetResponse();
			obj2.GetResponseStream().Close();
			obj2.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper MakeDir Error --> " + ex.Message);
		}
	}

	public long GetFileSize(string filename)
	{
		long num = 0L;
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + filename));
			obj.Method = "SIZE";
			obj.UseBinary = true;
			obj.UsePassive = false;
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			FtpWebResponse obj2 = (FtpWebResponse)obj.GetResponse();
			Stream responseStream = obj2.GetResponseStream();
			num = obj2.ContentLength;
			responseStream.Close();
			obj2.Close();
			return num;
		}
		catch (WebException ex)
		{
			_ = ((FtpWebResponse)ex.Response).StatusDescription;
			return 1024L;
		}
	}

	public void ReName(string currentFilename, string newFilename)
	{
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI + currentFilename));
			obj.Method = "RENAME";
			obj.RenameTo = newFilename;
			obj.UseBinary = true;
			obj.UsePassive = false;
			obj.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			FtpWebResponse obj2 = (FtpWebResponse)obj.GetResponse();
			obj2.GetResponseStream().Close();
			obj2.Close();
		}
		catch (Exception ex)
		{
			throw new Exception("FtpHelper ReName Error --> " + ex.Message);
		}
	}

	public void MovieFile(string currentFilename, string newDirectory)
	{
		ReName(currentFilename, newDirectory);
	}

	public void GotoDirectory(string DirectoryName, bool IsRoot)
	{
		if (IsRoot)
		{
			ftpRemotePath = DirectoryName;
		}
		else
		{
			ftpRemotePath = ftpRemotePath + DirectoryName + "/";
		}
		ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
	}

	public void DeleteOrderDirectory(string ftpServerIP, string folderToDelete, string ftpUserID, string ftpPassword)
	{
		try
		{
			if (!string.IsNullOrEmpty(ftpServerIP) && !string.IsNullOrEmpty(folderToDelete) && !string.IsNullOrEmpty(ftpUserID) && !string.IsNullOrEmpty(ftpPassword))
			{
				FtpHelper ftpHelper = new FtpHelper(ftpServerIP, folderToDelete, ftpUserID, ftpPassword);
				ftpHelper.GotoDirectory(folderToDelete, IsRoot: true);
				string[] directoryList = ftpHelper.GetDirectoryList();
				foreach (string text in directoryList)
				{
					if (string.IsNullOrEmpty(text) && !(text != ""))
					{
						continue;
					}
					string directoryName = folderToDelete + "/" + text;
					ftpHelper.GotoDirectory(directoryName, IsRoot: true);
					string[] fileList = ftpHelper.GetFileList("*.*");
					if (fileList != null)
					{
						string[] array = fileList;
						foreach (string fileName in array)
						{
							ftpHelper.Delete(fileName);
						}
					}
					ftpHelper.GotoDirectory(folderToDelete, IsRoot: true);
					ftpHelper.RemoveDirectory(text);
				}
				string directoryName2 = folderToDelete.Remove(folderToDelete.LastIndexOf('/'));
				string folderName = folderToDelete.Substring(folderToDelete.LastIndexOf('/') + 1);
				ftpHelper.GotoDirectory(directoryName2, IsRoot: true);
				ftpHelper.RemoveDirectory(folderName);
				return;
			}
			throw new Exception("FTP 及路径不能为空！");
		}
		catch (Exception ex)
		{
			throw new Exception("删除订单时发生错误，错误信息为：" + ex.Message);
		}
	}

	public void DownloadAllInDic(string dic, string destinationPath)
	{
		dic = dic.Substring(0, dic.IndexOf(swFileName)) + destinationPath.Substring(destinationPath.IndexOf(swFileName));
		dic = dic.Replace("\\", "/");
		GotoDirectory(dic, IsRoot: true);
		Directory.CreateDirectory(destinationPath);
		new List<string>();
		List<string> list = GetFileList("").ToList();
		List<string> list2 = GetDirectoryList().ToList();
		foreach (string item in list)
		{
			if (list2.IndexOf(item) < 0)
			{
				double fileSize = GetFileSize(item);
				MiFlashGlobal.Swdes = new SwDes
				{
					fileSize = fileSize,
					serverPath = ftpURI + "\\" + item
				};
				Download(destinationPath, item);
			}
		}
		foreach (string item2 in list2)
		{
			_ = item2 == "ota";
			if (!string.IsNullOrEmpty(item2))
			{
				DownloadAllInDic(ftpRemotePath + "\\" + item2, destinationPath + "/" + item2);
			}
		}
	}
}
