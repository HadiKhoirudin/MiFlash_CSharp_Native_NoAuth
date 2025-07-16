using System;
using System.Xml;

namespace XiaoMiFlash.code.lan;

public class LanguageProvider
{
	private string languageType = "";

	public LanguageProvider(string lanType)
	{
		languageType = lanType;
	}

	public string GetLanguage(string ctrlID)
	{
		XmlDocument xmlDocument = new XmlDocument();
		new XmlReaderSettings().IgnoreComments = true;
		XmlReader reader = XmlReader.Create(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Source\\LanguageLibrary.xml");
		xmlDocument.Load(reader);
		XmlNodeList childNodes = xmlDocument.SelectSingleNode("LanguageLibrary").ChildNodes;
		string result = "";
		foreach (XmlNode item in childNodes)
		{
			if (!(item.Name.ToLower() != "lan") && item.Attributes["CTRLID"].Value == ctrlID)
			{
				return item.Attributes[languageType].Value;
			}
		}
		return result;
	}
}
