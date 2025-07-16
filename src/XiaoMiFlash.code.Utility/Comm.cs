using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using XiaoMiFlash.code.data;

namespace XiaoMiFlash.code.Utility;

public class Comm
{
	public bool isReadDump = true;

	public bool isWriteDump;

	public bool ignoreResponse = true;

	public SerialPort serialPort;

	private bool _keepReading;

	public byte[] recData;

	private long received_count;

	public int MAX_SECTOR_STR_LEN = 20;

	public int SECTOR_SIZE_UFS = 4096;

	public int SECTOR_SIZE_EMMC = 512;

	public int m_dwBufferSectors;

	public int intSectorSize;

	public string auth = "";

	public string edlAuthErr { get; set; } = "error: only nop and sig tag can be recevied before authentication";

	public bool needEdlAuth = true;

	public bool isSupportPartialReset;

	public bool isDdr4;

	public bool isDdr5;

	public string storageInfo = "";

	public bool isGetChipNum;

	public string chipNum = "";

	public bool IsOpen
	{
		get
		{
			int num = 20;
			while (num-- > 0 && !serialPort.IsOpen)
			{
				Log.w(serialPort.PortName, "wait for port open.");
				Thread.Sleep(50);
			}
			return serialPort.IsOpen;
		}
	}

	public Comm()
	{
		_keepReading = false;
	}

	public bool isKeepReading()
	{
		return _keepReading;
	}

	public void StartReading()
	{
		if (!_keepReading)
		{
			_keepReading = true;
		}
	}

	public void StopReading()
	{
		if (_keepReading)
		{
			_keepReading = false;
		}
	}

	public byte[] ReadPortData()
	{
		byte[] result = null;
		if (serialPort.IsOpen)
		{
			int bytesToRead = serialPort.BytesToRead;
			if (bytesToRead > 0)
			{
				result = new byte[bytesToRead];
				try
				{
					serialPort.Read(result, 0, bytesToRead);
					return result;
				}
				catch (TimeoutException ex)
				{
					Log.w(serialPort.PortName, ex, stopFlash: false);
					return result;
				}
			}
		}
		return result;
	}

	public byte[] ReadPortData(int offset, int count)
	{
		byte[] array = new byte[count];
		try
		{
			serialPort.Read(array, offset, count);
			return array;
		}
		catch (TimeoutException ex)
		{
			Log.w(serialPort.PortName, ex, stopFlash: false);
			return array;
		}
	}

	public void Open()
	{
		Close();
		serialPort.Open();
		if (!serialPort.IsOpen)
		{
			string text = "open serial port failed!";
			Log.w(serialPort.PortName, text);
			FlashingDevice.UpdateDeviceStatus(serialPort.PortName, null, text, "error", isDone: true);
		}
	}

	private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		int bytesToRead = serialPort.BytesToRead;
		recData = new byte[bytesToRead];
		received_count += bytesToRead;
		serialPort.Read(recData, 0, bytesToRead);
	}

	public void Close()
	{
		serialPort.Close();
	}

	public void CleanBuffer()
	{
		serialPort.DiscardOutBuffer();
		serialPort.DiscardInBuffer();
	}

	public void WritePort(byte[] send, int offSet, int count)
	{
		if (IsOpen)
		{
			_ = _keepReading;
			int num = 0;
			Exception ex = new TimeoutException();
			bool flag = false;
			while (num++ <= 6 && ex != null && ex.GetType() == typeof(TimeoutException))
			{
				try
				{
					serialPort.WriteTimeout = 2000;
					serialPort.Write(send, offSet, count);
					flag = true;
					if (isWriteDump)
					{
						Log.w(serialPort.PortName, "write to port:");
						Dump(send);
					}
					ex = null;
				}
				catch (TimeoutException ex2)
				{
					ex = ex2;
					Log.w(serialPort.PortName, "write time out try agian " + num);
					Thread.Sleep(500);
				}
				catch (Exception ex3)
				{
					Log.w(serialPort.PortName, "write failed:" + ex3.Message);
				}
			}
			if (!flag)
			{
				Log.w(serialPort.PortName, ex, stopFlash: true);
				throw new Exception("write time out,maybe device was disconnected.");
			}
		}
	}

	public bool SendCommand(string command)
	{
		return SendCommand(command, checkAck: false);
	}

	public bool SendCommand(string command, bool checkAck)
	{
		byte[] bytes = Encoding.Default.GetBytes(command);
		if (_keepReading || checkAck)
		{
			Log.w(serialPort.PortName, "send command:" + command);
		}
		WritePort(bytes, 0, bytes.Length);
		if (checkAck)
		{
			return GetResponse(checkAck);
		}
		return false;
	}

	private int SubstringCount(string str, string substring)
	{
		if (str.Contains(substring))
		{
			string text = str.Replace(substring, "");
			return (str.Length - text.Length) / substring.Length;
		}
		return 0;
	}

	public bool chkRspAck(out string msg)
	{
		msg = null;
		byte[] binary = ReadDataFromPort();
		string[] array = Dump(binary, waitACK: true);
		string value = "<response value=\"ACK\"";
		int num = 10;
		while ((array.Length != 2 || array[1].IndexOf(value) < 0) && num-- >= 0)
		{
			Thread.Sleep(10);
			binary = ReadDataFromPort();
			array = Dump(binary, waitACK: true);
		}
		if (array.Length == 2 && array[1].IndexOf(value) >= 0)
		{
			CleanBuffer();
			return true;
		}
		msg = "did not detect ACK from target.";
		return false;
	}

	public bool chkRspAck(out string msg, int chunkCount)
	{
		msg = null;
		byte[] binary = ReadDataFromPort();
		string[] array = Dump(binary, waitACK: true);
		string text = "<response value=\"ACK\"";
		int num = 10;
		while ((array.Length != 2 || array[1].IndexOf(text) < 0) && num-- >= 0)
		{
			Thread.Sleep(10);
			binary = ReadDataFromPort();
			array = Dump(binary, waitACK: true);
		}
		if (array.Length == 2 && array[1].IndexOf(text) >= 0)
		{
			int i = SubstringCount(array[1], text);
			num = 10;
			for (; i < chunkCount * 2; i += SubstringCount(array[1], text))
			{
				if (num-- <= 0)
				{
					break;
				}
				Thread.Sleep(10);
				binary = ReadDataFromPort();
				array = Dump(binary, waitACK: true);
			}
			if (chunkCount * 2 > i)
			{
				Log.w(serialPort.PortName, "ACK count don't match!");
				throw new Exception("ACK count don't match!");
			}
			Log.w(serialPort.PortName, $"{chunkCount} chunks match {i} ack");
			CleanBuffer();
			return true;
		}
		msg = array[1];
		return false;
	}

	public byte[] getRecDataIgnoreExcep()
	{
		byte[] array = ReadDataFromPort();
		if (array != null && array.Length != 0 && isReadDump)
		{
			Log.w(serialPort.PortName, "read from port:");
			Dump(array);
		}
		return array;
	}

	public byte[] getRecData()
	{
		byte[] array = ReadDataFromPort();
		if (array == null)
		{
			throw new Exception("can not read from port " + serialPort.PortName);
		}
		if (array.Length != 0 && isReadDump)
		{
			Log.w(serialPort.PortName, "read from port:");
			Dump(array);
		}
		return array;
	}

	private byte[] ReadDataFromPort()
	{
		int num = 10;
		recData = null;
		recData = ReadPortData();
		while (num-- >= 0 && recData == null)
		{
			Thread.Sleep(50);
			recData = ReadPortData();
		}
		return recData;
	}

	public bool GetResponse(bool waiteACK)
	{
		bool flag = false;
		Log.w(serialPort.PortName, "get response from target");
		if (!waiteACK)
		{
			return ReadDataFromPort() != null;
		}
		int num = 2;
		if (waiteACK)
		{
			num = 32;
		}
		while (num-- > 0 && !flag)
		{
			List<XmlDocument> responseXml = GetResponseXml(waiteACK);
			_ = responseXml.Count;
			foreach (XmlDocument item in responseXml)
			{
				foreach (XmlNode childNode in item.SelectSingleNode("data").ChildNodes)
				{
					if (childNode.Name.ToLower() == "sig")
					{
						auth = childNode.OuterXml.Replace("blob", "sig");
					}
					foreach (XmlAttribute attribute in childNode.Attributes)
					{
						if (attribute.Name.ToLower() == "maxpayloadsizetotargetinbytes")
						{
							m_dwBufferSectors = Convert.ToInt32(attribute.Value) / intSectorSize;
						}
						if (attribute.Value.ToLower() == "ack")
						{
							flag = true;
						}
						if (attribute.Value == "WARN: NAK: MaxPayloadSizeToTargetInBytes sent by host 1048576 larger than supported 16384")
						{
							flag = true;
						}
						if (attribute.Value.Contains("0xC3 0x00 0x40 0x10 "))
						{
							Log.w(serialPort.PortName, "peek 0xc3 0x00 0x40 0x10,chose ddr4");
							isDdr4 = true;
						}
						if (attribute.Value.Contains("0xC3 0x00 0x31 0x10 "))
						{
							Log.w(serialPort.PortName, "peek 0xc3 0x00 0x31 0x10,chose ddr4");
							isDdr4 = true;
						}
						if (attribute.Value.Contains("0xC3 0x00 0x02 0x10 "))
						{
							Log.w(serialPort.PortName, "peek 0xc3 0x00 0x02 0x10,chose ddr4");
							isDdr4 = true;
						}
						if (attribute.Value.Contains("0xC3 0x00 0x00 0x10 "))
						{
							Log.w(serialPort.PortName, "peek 00xC3\u00a00x00 0x00 0x10,chose dd5");
							isDdr5 = true;
						}
						if (attribute.Value.Contains("0xC3 0x00 0x51 0x10 "))
						{
							Log.w(serialPort.PortName, "peek 00xC3\u00a00x00 0x51 0x10,chose dd5");
							isDdr5 = true;
						}
						if (attribute.Value.Contains("0xC3 0x00 0x32 0x10 "))
						{
							Log.w(serialPort.PortName, "peek 00xC3\u00a00x00 0x32 0x10,chose dd5");
							isDdr5 = true;
						}
						if (attribute.Value.Contains("UFS Inquiry Command Output"))
						{
							Log.w(serialPort.PortName, "StorageInfo: " + attribute.Value);
							storageInfo = attribute.Value;
						}
						if (attribute.Value.Contains("INFO: quick_reset") && !isSupportPartialReset)
						{
							Log.w(serialPort.PortName, "quick_reset: " + attribute.Value);
							isSupportPartialReset = true;
						}
						if (attribute.Value.Contains("Chip serial num") && !isGetChipNum)
						{
							Log.w(serialPort.PortName, "Chip serial num: " + attribute.Value);
							isGetChipNum = true;
							int num2 = attribute.Value.IndexOf('(') + 1;
							int num3 = attribute.Value.IndexOf(')');
							string text = attribute.Value.Substring(num2, num3 - num2);
							if (text.Length < 10)
							{
								int length = 10 - text.Length;
								string text2 = "00000000";
								text = text.Substring(0, 2) + text2.Substring(0, length) + text.Substring(2, text.Length - 2);
							}
							chipNum = text;
						}
					}
				}
			}
			if (waiteACK)
			{
				Thread.Sleep(50);
			}
		}
		return flag;
	}

	public List<XmlDocument> GetResponseXml(bool waiteACK)
	{
		List<XmlDocument> list = new List<XmlDocument>();
		byte[] binary = ReadDataFromPort();
		string[] array = Dump(binary, waiteACK);
		if (array.Length >= 2)
		{
			foreach (string item in Regex.Split(array[1], "\\<\\?xml").ToList())
			{
				if (!string.IsNullOrEmpty(item))
				{
					if (item.ToLower().IndexOf(edlAuthErr) >= 0)
					{
						needEdlAuth = true;
					}
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml("<?xml " + item);
					list.Add(xmlDocument);
				}
			}
			return list;
		}
		return list;
	}

	private string[] Dump(byte[] binary)
	{
		return Dump(binary, waitACK: false);
	}

	private string[] Dump(byte[] binary, bool waitACK)
	{
		if (binary == null)
		{
			Log.w(serialPort.PortName, "no Binary dump");
			return new string[2] { "", "" };
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		new StringBuilder();
		new StringBuilder();
		for (int i = 0; i < binary.Length; i++)
		{
			stringBuilder2.Append(Convert.ToChar(binary[i]).ToString());
		}
		if (waitACK)
		{
			var str = stringBuilder2.ToString();
			if (str.Contains("nop and sig tag"))
            {
                Log.w(serialPort.PortName, "bypassing authentication: please wait [3 min]...\r\n\r\n", throwEx: false);
            }
			else
			{
				Log.w(serialPort.PortName, "resdump:" + str + "\r\n\r\n", throwEx: false);
			}
		}
		if (_keepReading)
		{
			Log.debugString(serialPort.PortName, binary);
		}
		return new string[2]
		{
			stringBuilder.ToString(),
			stringBuilder2.ToString()
		};
	}
}
