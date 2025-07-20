using System;
using System.Configuration;
using System.Windows.Forms;
using System.Xml;

namespace XiaoMiFlash.code.Utility;

public class MiAppConfig
{
	public static void Add(string key, string value)
	{
		//Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		//configuration.AppSettings.Settings.Add(key, value);
		//configuration.Save();
	}

	public static void SetValue(string AppKey, string AppValue)
	{
		//Console.WriteLine($"Appkey : {AppKey} -> {AppValue}");

		//Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		//configuration.AppSettings.Settings[AppKey].Value = AppValue;
		//configuration.Save(ConfigurationSaveMode.Modified);
		//ConfigurationManager.RefreshSection("appSettings");
	}

	public static string GetAppConfig(string appKey)
	{
		//XmlDocument xmlDocument = new XmlDocument();
		//xmlDocument.Load(Application.ExecutablePath + ".config");
		//XmlElement xmlElement = (XmlElement)xmlDocument.SelectSingleNode("//appSettings").SelectSingleNode("//add[@key='" + appKey + "']");
		//if (xmlElement != null)
		//{
		//	return xmlElement.Attributes["value"].Value;
		//}
		return string.Empty;
	}

	public static string Get(string key)
	{
		//if (ConfigurationManager.AppSettings[key] == null)
		//{
		//	Add(key, "");
		//}
		//if (ConfigurationManager.AppSettings[key] != null)
		//{
		//	return ConfigurationManager.AppSettings[key].ToString();
		//}
		return "";
	}
}
