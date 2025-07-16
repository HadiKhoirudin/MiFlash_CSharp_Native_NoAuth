using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XiaoMiFlash.code.Utility;

public class Utility
{
	public static string GetMD5HashFromFile(string fileName)
	{
		try
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
			byte[] array = new MD5CryptoServiceProvider().ComputeHash(fileStream);
			fileStream.Close();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
		}
	}
}
