using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.data;

internal class SaveFfu
{
	public static List<Ffu> ffuList = new List<Ffu>();

	public static List<Wb> wbList = new List<Wb>();

	public static bool isNeedWBprovision = false;

	public static double VidToInt(string strVid)
	{
		if (strVid.Substring(0, 1) == "P")
		{
			return double.Parse(strVid.Substring(1, strVid.Length - 1));
		}
		if (strVid == "MP")
		{
			return 100.0;
		}
		return -1.0;
	}

	public static void GetFfu(string dlPath)
	{
		ffuList.Clear();
		string text = "";
		string text2 = dlPath + "\\MI_FFU\\ffu_list.txt";
		if (File.Exists(text2))
		{
			Log.w("dl has ffufilelist in ffuFile:" + text2);
			MiFlashGlobal.IsFirmwarewrite = true;
			string directoryName = Path.GetDirectoryName(text2);
			Log.w("ffuFilePath: " + directoryName);
			MiAppConfig.SetValue("ffuPath", directoryName.Trim());
			StreamReader streamReader = new StreamReader(text2, Encoding.Default);
			while ((text = streamReader.ReadLine()) != null)
			{
				if (text.ToLower().IndexOf("version") < 0)
				{
					if ("".Equals(text))
					{
						break;
					}
					string[] array = text.Split(' ');
					ffuList.Add(new Ffu
					{
						Name = array[0],
						Version = array[1],
						Number = array[2],
						File = array[3],
						Size = array[4]
					});
				}
			}
		}
		else
		{
			Log.w("dl not exit ffufilelist");
		}
	}

	public static string FileReadByLine(string filePath)
	{
		return new StreamReader(filePath, Encoding.Default).ReadLine().ToString();
	}

	public static void GetWb(string swPath)
	{
		wbList.Clear();
		string text = swPath + "\\provision.xml";
		if (File.Exists(text))
		{
			string vendor = "";
			string partnumber = "";
			string wb_size_in_MB = "";
			string hpb_total_range_in_MB = "";
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(text);
			{
				foreach (XmlNode childNode in xmlDocument.SelectSingleNode("data").ChildNodes)
				{
					if (childNode.Name.ToLower() == "start")
					{
						foreach (XmlAttribute attribute in childNode.Attributes)
						{
							if (attribute.Name.ToLower() == "strictly_ctrl")
							{
								if (attribute.Value != "1")
								{
									return;
								}
								isNeedWBprovision = true;
							}
						}
					}
					if (!(childNode.Name.ToLower() == "ufs"))
					{
						continue;
					}
					foreach (XmlAttribute attribute2 in childNode.Attributes)
					{
						switch (attribute2.Name.ToLower())
						{
						case "vendor":
							vendor = attribute2.Value;
							break;
						case "partnumber":
							partnumber = attribute2.Value;
							break;
						case "density_in_gb":
							_ = attribute2.Value;
							break;
						case "wb_size_in_mb":
							wb_size_in_MB = attribute2.Value;
							break;
						case "hpb_total_range_in_mb":
							hpb_total_range_in_MB = attribute2.Value;
							break;
						}
					}
					wbList.Add(new Wb
					{
						Vendor = vendor,
						Partnumber = partnumber,
						Wb_size_in_MB = wb_size_in_MB,
						Hpb_total_range_in_MB = hpb_total_range_in_MB
					});
				}
				return;
			}
		}
		Log.w("WbFile " + text + " not exist");
	}
}
