using System.Collections;
using System.IO;
using System.Xml;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class ImageValidation
{
	public static string Validate(string path)
	{
		bool flag = true;
		string result = "md5 validate successfully.";
		new Hashtable();
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			if (directoryInfo2.Name.ToLower().IndexOf("images") >= 0)
			{
				directoryInfo = directoryInfo2;
				break;
			}
		}
		string fullName = directoryInfo.FullName;
		new Hashtable();
		string text = directoryInfo.Parent.FullName + "\\md5sum.xml";
		if (File.Exists(text))
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlReader reader = XmlReader.Create(text, new XmlReaderSettings
			{
				IgnoreComments = true
			});
			xmlDocument.Load(reader);
			{
				foreach (XmlNode childNode in xmlDocument.SelectSingleNode("root").FirstChild.ChildNodes)
				{
					foreach (XmlAttribute attribute in childNode.Attributes)
					{
						if (attribute.Name.ToLower() == "name")
						{
							string text2 = attribute.Value.ToLower();
							string innerText = childNode.InnerText;
							fullName = directoryInfo.FullName + "\\" + text2;
							if (XiaoMiFlash.code.Utility.Utility.GetMD5HashFromFile(fullName) != innerText)
							{
								result = fullName + " md5 validate failed!";
								flag = false;
								break;
							}
							Log.w(fullName + " md5 valide success.");
						}
					}
					if (!flag)
					{
						return result;
					}
				}
				return result;
			}
		}
		return "not found md5sum.xml.";
	}
}
