using System;
using System.Collections.Generic;
using System.IO;

namespace XiaoMiFlash.code.data;

public static class MemImg
{
	private static object obj_lock = new object();

	public static bool isHighSpeed = false;

	public static Dictionary<string, MemoryStream> shareMemTable = new Dictionary<string, MemoryStream>();

	public static long mapImg(string filePath)
	{
		lock (obj_lock)
		{
			try
			{
				if (!shareMemTable.ContainsKey(filePath))
				{
					FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
					int num = (int)fileStream.Length;
					byte[] buffer = new byte[num];
					fileStream.Read(buffer, 0, num);
					MemoryStream value = new MemoryStream(buffer);
					shareMemTable[filePath] = value;
					return num;
				}
				return shareMemTable[filePath].Length;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}

	public static byte[] GetBytesFromFile(string filePath, long offset, int size, out float percent)
	{
		lock (obj_lock)
		{
			MemoryStream memoryStream = shareMemTable[filePath];
			byte[] array = new byte[size];
			memoryStream.Position = offset;
			memoryStream.Read(array, 0, size);
			percent = (float)offset / (float)memoryStream.Length;
			return array;
		}
	}

	public static void distory()
	{
		if (!isHighSpeed)
		{
			return;
		}
		foreach (KeyValuePair<string, MemoryStream> item in shareMemTable)
		{
			item.Value.Close();
			item.Value.Dispose();
		}
		shareMemTable.Clear();
	}
}
