using System;
using System.IO;
using System.Net;
using System.Text;

namespace XiaoMiFlash.code.bl;

public class Authentication
{
	private static string ACCOUNT_LOGIN_URL_BASE = "http://account.preview.n.xiaomi.net/";

	private string ACCOUNT_LOGIN_URL = ACCOUNT_LOGIN_URL_BASE + "pass/serviceLogin";

	private string ACCOUNT_LOGIN_AUTH_URL = ACCOUNT_LOGIN_URL_BASE + "pass/serviceLoginAuth2";

	private string ACCOUNT_LOGIN_STEP2 = ACCOUNT_LOGIN_URL_BASE + "pass/loginStep2";

	private string ACCOUNT_LOGOUT_URL = ACCOUNT_LOGIN_URL_BASE + "pass/logout";

	private string ACCOUNT_USERCARD_URL = "http://api.account.xiaomi.com/pass/usersCard";

	private string ACCOUNT_REGISTER_URL = ACCOUNT_LOGIN_URL_BASE + "pass/register";

	private string ACCOUNT_FORGETPASSWORD_URL = ACCOUNT_LOGIN_URL_BASE + "pass/forgetPassword";

	private string ACCOUT_GET_VERIFY_CODE_URL = ACCOUNT_LOGIN_URL_BASE + "pass/getCode?icodeType?=login?";

	private string SendDataByPost(string Url, string postDataStr, ref CookieContainer cookie)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
		if (cookie.Count == 0)
		{
			httpWebRequest.CookieContainer = new CookieContainer();
			cookie = httpWebRequest.CookieContainer;
		}
		else
		{
			httpWebRequest.CookieContainer = cookie;
		}
		httpWebRequest.Method = "POST";
		httpWebRequest.ContentType = "application/x-www-form-urlencoded";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
		httpWebRequest.ContentLength = postDataStr.Length;
		StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream(), Encoding.GetEncoding("gb2312"));
		streamWriter.Write(postDataStr);
		streamWriter.Close();
		Stream responseStream = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
		StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
		string result = streamReader.ReadToEnd();
		streamReader.Close();
		responseStream.Close();
		return result;
	}

	private string SendDataByGET(string Url, string postDataStr, ref CookieContainer cookie)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url + ((postDataStr == "") ? "" : "?") + postDataStr);
		if (cookie.Count == 0)
		{
			httpWebRequest.CookieContainer = new CookieContainer();
			cookie = httpWebRequest.CookieContainer;
		}
		else
		{
			httpWebRequest.CookieContainer = cookie;
		}
		httpWebRequest.Method = "GET";
		httpWebRequest.ContentType = "text/html;charset=UTF-8";
		Stream responseStream = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
		StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
		string result = streamReader.ReadToEnd();
		streamReader.Close();
		responseStream.Close();
		return result;
	}

	public string Login(string username, string pwd)
	{
		try
		{
			SignInfo signInfo = default(SignInfo);
			CookieContainer cookie = new CookieContainer();
			signInfo.sid = "passport";
			string url = ACCOUNT_LOGIN_URL + "?sid=" + signInfo.sid + "&_json=true&passive=true&hidden=false";
			string result = SendDataByGET(url, "", ref cookie);
			url = ACCOUNT_LOGIN_AUTH_URL + "?_json=true";
			return result;
		}
		catch (Exception)
		{
			return "";
		}
	}
}
