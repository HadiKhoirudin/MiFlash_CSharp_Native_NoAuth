using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.module;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

public class SerialPortDevice : DeviceCtrl
{
	public Comm comm = new Comm();

	private int BUFFER_SECTORS = 256;

	private int programmerType = 1;

	private int programmerName;

	private string storageType = "ufs";

	private bool isLite;

	private int m_iSkipStorageInit;

	private bool needFirmwarewrite;

	private bool isUploadDebugpolicy;

	private bool needRestoreRom;

	private int TIME_WAIT_FIREHOSE_START = 1000;

	public static bool isOpen;

	public override void flash()
	{
		if (string.IsNullOrEmpty(deviceName))
		{
			return;
		}
		Log.w(deviceName, "flash in thread name:" + Thread.CurrentThread.Name + ",id:" + Thread.CurrentThread.ManagedThreadId);
		if (!Directory.Exists(swPath))
		{
			throw new Exception("sw path is not valid");
		}
		comm.isReadDump = openReadDump;
		comm.isWriteDump = openWriteDump;
		DirectoryInfo[] directories = new DirectoryInfo(swPath).GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (directoryInfo.Name.ToLower().IndexOf("images") >= 0)
			{
				swPath = directoryInfo.FullName;
				Log.w(deviceName, "sw in images");
				break;
			}
		}
		if (NeedProvision(swPath))
		{
			m_iSkipStorageInit = 1;
		}
		if (string.IsNullOrEmpty(MiAppConfig.Get("mainProgram")) || MiAppConfig.Get("mainProgram").ToString() == "xiaomi")
		{
			if (MiFlashGlobal.IsBackupOnly)
			{
				BackupImage();
			}
			else
			{
				XiaomiFlashNew();
			}
		}
		else
		{
			QcmFlash();
		}
	}

	private void BackupImage()
	{
		if (string.IsNullOrEmpty(deviceName))
		{
			return;
		}
		try
		{
			SerialPort port = new SerialPort(deviceName, 9600);
			registerPort(port);
			SaharaDownloadProgrammer(programmerName);
			Thread.Sleep(1000);
			PropareFirehose();
			ConfigureDDR(comm.intSectorSize, BUFFER_SECTORS, storageType, m_iSkipStorageInit);
			BackupRom();
			FlashingDevice.UpdateDeviceStatus(deviceName, 1f, "BackupRom done", "success", isDone: true);
		}
		catch (Exception ex)
		{
			FlashingDevice.UpdateDeviceStatus(deviceName, null, ex.Message, "error", isDone: true);
			Log.w(deviceName, string.Concat(ex, "  ", ex.StackTrace), throwEx: false);
		}
		finally
		{
			comm.Close();
		}
	}

	private void XiaomiFlashNew()
	{
		if (string.IsNullOrEmpty(deviceName))
		{
			return;
		}
		try
		{
			string text = "";
			SerialPort port = new SerialPort(deviceName, 9600);
			registerPort(port);
			comm.StartReading();
			SaharaDownloadProgrammer(programmerName);
			comm.StopReading();
			Thread.Sleep(1000);
			while (true)
			{
				PropareFirehose();
				ConfigureDDR(comm.intSectorSize, BUFFER_SECTORS, storageType, m_iSkipStorageInit);
				if (!needRestoreRom)
				{
					needRestoreRom = true;
					BackupRom();
				}
				if (!NeedProvision(swPath))
				{
					break;
				}
				Provision(swPath);
				Thread.Sleep(5000);
				if (comm.isSupportPartialReset)
				{
					Log.w(comm.serialPort.PortName, "Partial_Reset target");
					FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "Partial_Reset target", "flashing", isDone: false);
					text = string.Format(Firehose.Partial_Reset, verbose ? "1" : "0");
					comm.SendCommand(text, checkAck: true);
					Thread.Sleep(1000);
				}
				else
				{
					Log.w(comm.serialPort.PortName, "Reset_To_Edl target");
					FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "Reset_To_Edl target", "flashing", isDone: false);
					text = string.Format(Firehose.Reset_To_Edl, verbose ? "1" : "0");
					comm.SendCommand(text, checkAck: true);
					Thread.Sleep(2000);
					comm.Close();
					Thread.Sleep(2000);
					findComPort();
					SaharaDownloadProgrammer(programmerName);
					Thread.Sleep(1000);
				}
				PropareFirehose();
				m_iSkipStorageInit = 0;
				uploadDebugpolicy();
				ConfigureDDR(comm.intSectorSize, BUFFER_SECTORS, storageType, m_iSkipStorageInit);
				if (!MiFlashGlobal.IsFirmwarewrite)
				{
					break;
				}
				Log.w(comm.serialPort.PortName, "do firmwarewrite");
				Firmwarewrite();
				if (!needFirmwarewrite)
				{
					break;
				}
				if (comm.isSupportPartialReset)
				{
					Log.w(comm.serialPort.PortName, "Partial_Reset target");
					FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "Partial_Reset target", "flashing", isDone: false);
					text = string.Format(Firehose.Partial_Reset, verbose ? "1" : "0");
					comm.SendCommand(text, checkAck: true);
					Thread.Sleep(1000);
					continue;
				}
				Log.w(comm.serialPort.PortName, "Reset_To_Edl target");
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "Reset_To_Edl target", "flashing", isDone: false);
				text = string.Format(Firehose.Reset_To_Edl, verbose ? "1" : "0");
				comm.SendCommand(text, checkAck: true);
				Thread.Sleep(2000);
				comm.Close();
				Thread.Sleep(2000);
				findComPort();
				SaharaDownloadProgrammer(programmerName);
				Thread.Sleep(1000);
			}
			if (storageType == Storage.ufs)
			{
				SetBootPartition();
			}
			FirehoseDownloadImg(swPath);
			if (NeedRestoreRom(swPath))
			{
				string text2 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "restore\\" + startTime.ToString("yyyyMdHms") + "_" + comm.chipNum;
				Log.w(comm.serialPort.PortName, "restoreImagePath:" + text2);
				string[] array = FileSearcher.SearchFiles(swPath, SoftwareImage.RawRestorePattern);
				for (int i = 0; i < array.Length; i++)
				{
					WriteFilesToDevice(comm.serialPort.PortName, text2, array[i]);
				}
			}

            Log.w(comm.serialPort.PortName, "Partial_Reset target");
            FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "Partial_Reset target", "flashing", isDone: false);
            text = string.Format(Firehose.Partial_Reset, verbose ? "1" : "0");
            comm.SendCommand(text, checkAck: true);
            Thread.Sleep(1000);

            FlashingDevice.UpdateDeviceStatus(deviceName, 1f, "flash done", "success", isDone: true);
		}
		catch (Exception ex)
		{
			FlashingDevice.UpdateDeviceStatus(deviceName, null, ex.Message, "error", isDone: true);
			Log.w(deviceName, string.Concat(ex, "  ", ex.StackTrace), throwEx: false);
		}
		finally
		{
			comm.Close();
		}
	}

	private void QcmFlash()
	{
		FHloader();
	}

	private void findComPort()
	{
		SerialPort serialPort = new SerialPort(deviceName, 9600);
		comm.serialPort = serialPort;
		bool flag = false;
		Thread.Sleep(5000);
		List<string> list = getDevice().ToList();
		int num = 100;
		string text = "";
		while (num-- > 0 && list.IndexOf(deviceName) < 0)
		{
			Thread.Sleep(100);
			list = getDevice().ToList();
			text = "waiting for " + deviceName + " restart";
			Log.w(comm.serialPort.PortName, text);
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, text, "restart", isDone: false);
		}
		if (list.IndexOf(deviceName) >= 0)
		{
			flag = true;
			text = deviceName + " restart successfully";
			Log.w(comm.serialPort.PortName, text);
			Thread.Sleep(800);
			bool flag2 = false;
			while (num-- > 0 && !flag2)
			{
				try
				{
					comm.serialPort.Open();
					flag2 = true;
					Log.w(comm.serialPort.PortName, " serial port " + deviceName + " opend successfully");
				}
				catch (Exception)
				{
					Log.w(comm.serialPort.PortName, "open serial port " + deviceName + " ");
					Thread.Sleep(800);
				}
			}
			flag = flag && comm.IsOpen;
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, text, "restart", isDone: false);
			return;
		}
		text = deviceName + " restart failed";
		Log.w(comm.serialPort.PortName, text);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, text, "restart", isDone: false);
		flag = false;
		throw new Exception(text);
	}

	public override void CheckSha256()
	{
		if (string.IsNullOrEmpty(deviceName))
		{
			return;
		}
		try
		{
			if (!Directory.Exists(swPath))
			{
				throw new Exception("sw path is not valid");
			}
			DirectoryInfo[] directories = new DirectoryInfo(swPath).GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				if (directoryInfo.Name.ToLower().IndexOf("images") >= 0)
				{
					swPath = directoryInfo.FullName;
					break;
				}
			}
			FlashingDevice.UpdateDeviceStatus(deviceName, 0f, "start flash", "flashing", isDone: false);
			SaharaDownloadProgrammer(programmerName);
			PropareFirehose();
			ConfigureDDR(comm.intSectorSize, BUFFER_SECTORS, storageType, 0);
			GetSha256(swPath);
			FlashingDevice.UpdateDeviceStatus(deviceName, 1f, "check done", "success", isDone: true);
		}
		catch (Exception ex)
		{
			FlashingDevice.UpdateDeviceStatus(deviceName, null, ex.Message, "error", isDone: true);
			Log.w(deviceName, ex, stopFlash: true);
		}
		finally
		{
			comm.serialPort.Close();
			comm.serialPort.Dispose();
		}
	}

	private void registerPort(SerialPort port)
	{
		comm.serialPort = port;
		if (!isOpen)
		{
			Log.w(comm.serialPort.PortName, string.Format("vboytest do open :", deviceName));
			int num = 100;
			bool flag = false;
			while (num-- > 0 && !flag)
			{
				try
				{
					comm.serialPort.Open();
					flag = true;
					Log.w(comm.serialPort.PortName, " serial port " + deviceName + " opend successfully");
				}
				catch (Exception ex)
				{
					Log.w(comm.serialPort.PortName, "open serial port " + deviceName + " ");
					Log.w(comm.serialPort.PortName, string.Concat(ex, "  ", ex.StackTrace));
					Thread.Sleep(800);
				}
			}
		}
		foreach (Device flashDevice in FlashingDevice.flashDeviceList)
		{
			if (flashDevice.Name == deviceName)
			{
				flashDevice.DComm = comm;
			}
		}
	}

	private void SaharaDownloadProgrammer(int programmertype)
	{
		if (comm.IsOpen)
		{
			string msg = string.Format("[{0}]:{1}", comm.serialPort.PortName, "start flash.");
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "read hello packet", "flashing", isDone: false);
			Log.w(comm.serialPort.PortName, msg);
			sahara_switch_Mode_packet sahara_switch_Mode_packet = default(sahara_switch_Mode_packet);
			sahara_switch_Mode_packet.Command = 19u;
			sahara_switch_Mode_packet.Length = 8u;
			byte[] array = new byte[12];
			comm.getRecDataIgnoreExcep();
			if (comm.recData == null || comm.recData.Length == 0)
			{
				comm.recData = new byte[48];
			}
			sahara_packet sahara_packet = default(sahara_packet);
			sahara_hello_packet sahara_hello_packet = (sahara_hello_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_hello_packet));
			sahara_hello_packet.Reserved = new uint[6];
			sahara_hello_response sahara_hello_response = default(sahara_hello_response);
			sahara_hello_response.Reserved = new uint[6];
			sahara_readdata_packet sahara_readdata_packet = default(sahara_readdata_packet);
			sahara_64b_readdata_packet sahara_64b_readdata_packet = default(sahara_64b_readdata_packet);
			sahara_end_transfer_packet sahara_end_transfer_packet = default(sahara_end_transfer_packet);
			sahara_done_response sahara_done_response = default(sahara_done_response);
			int num = 10;
			while (num-- > 0 && sahara_hello_packet.Command != 1)
			{
				msg = "cannot receive hello packet,MiFlash is trying to reset status!";
				Log.w(comm.serialPort.PortName, msg);
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, msg, "flashing", isDone: false);
				comm.getRecDataIgnoreExcep();
				if (comm.recData == null || comm.recData.Length == 0)
				{
					comm.recData = new byte[48];
				}
				sahara_hello_packet = (sahara_hello_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_hello_packet));
				Thread.Sleep(500);
				if (sahara_hello_packet.Command == 1)
				{
					continue;
				}
				msg = "try to reset status.";
				Log.w(comm.serialPort.PortName, msg);
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, msg, "flashing", isDone: false);
				if (!comm.serialPort.IsOpen)
				{
					if (comm.serialPort != null)
					{
						comm.serialPort.Close();
						Log.w(comm.serialPort.PortName, "端口被关闭尝试释放");
					}
					Log.w(comm.serialPort.PortName, "端口被关闭重新打开端口");
					comm.serialPort.Open();
				}
				if (comm.IsOpen)
				{
					Log.w(comm.serialPort.PortName, "清理端口数据，准备发送SAHARA_SWITCH_MODE_PACKET");
					comm.serialPort.DiscardInBuffer();
					comm.serialPort.DiscardOutBuffer();
				}
				sahara_hello_response = default(sahara_hello_response);
				sahara_hello_response.Reserved = new uint[6];
				sahara_hello_response.Command = 2u;
				sahara_hello_response.Length = 48u;
				sahara_hello_response.Version = 2u;
				sahara_hello_response.Version_min = 1u;
				sahara_hello_response.Mode = 3u;
				byte[] array2 = CommandFormat.StructToBytes(sahara_hello_response);
				comm.WritePort(array2, 0, array2.Length);
				comm.getRecDataIgnoreExcep();
				msg = "Switch mode back";
				Log.w(comm.serialPort.PortName, msg);
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, msg, "flashing", isDone: false);
				sahara_switch_Mode_packet.Command = 12u;
				sahara_switch_Mode_packet.Length = 12u;
				sahara_switch_Mode_packet.Mode = 0u;
				array2 = CommandFormat.StructToBytes(sahara_switch_Mode_packet);
				Array.ConstrainedCopy(array2, 0, array, 0, 12);
				comm.WritePort(array, 0, array.Length);
			}
			if (sahara_hello_packet.Command == 1)
			{
				msg = "received hello packet";
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, msg, "flashing", isDone: false);
				Log.w(comm.serialPort.PortName, msg);
				sahara_hello_response = default(sahara_hello_response);
				sahara_hello_response.Reserved = new uint[6];
				sahara_hello_response.Command = 2u;
				sahara_hello_response.Length = 48u;
				sahara_hello_response.Version = 2u;
				sahara_hello_response.Version_min = 1u;
				byte[] array3 = CommandFormat.StructToBytes(sahara_hello_response);
				comm.WritePort(array3, 0, array3.Length);
				Log.w(comm.serialPort.PortName, "programmertype: " + programmertype);
				string[] array4;
				switch (programmertype)
				{
				case 1:
					array4 = FileSearcher.SearchFiles(swPath, SoftwareImage.ProgrammerLite);
					break;
				case 2:
					array4 = FileSearcher.SearchFiles(swPath, SoftwareImage.ProgrammerDDR4);
					break;
				case 3:
					array4 = FileSearcher.SearchFiles(swPath, SoftwareImage.ProgrammerDDR5);
					break;
				default:
					Log.w(comm.serialPort.PortName, "swPath:" + swPath);
					array4 = FileSearcher.SearchFiles(swPath, SoftwareImage.ProgrammerPattern);
					break;
				}
				Log.w(comm.serialPort.PortName, "download programmer file: " + array4[0]);
				string text = "";
				if (array4.Length != 0)
				{
					text = array4[0];
					FileInfo fileInfo = new FileInfo(text);
					if (fileInfo.Name.ToLower().IndexOf("firehose") >= 0)
					{
						programmerType = Programmer.firehose;
					}
					if (fileInfo.Name.ToLower().IndexOf("ufs") >= 0)
					{
						storageType = Storage.ufs;
					}
					else if (fileInfo.Name.ToLower().IndexOf("emmc") >= 0)
					{
						storageType = Storage.emmc;
					}
					if (fileInfo.Name.ToLower().IndexOf("lite") >= 0)
					{
						isLite = true;
					}
					comm.intSectorSize = ((storageType == Storage.ufs) ? comm.SECTOR_SIZE_UFS : comm.SECTOR_SIZE_EMMC);
					Log.w(comm.serialPort.PortName, "download programmer " + text);
					FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "download programmer " + text, "flashing", isDone: false);
					FileTransfer fileTransfer = new FileTransfer(comm.serialPort.PortName, text);
					bool flag;
					do
					{
						flag = false;
						comm.getRecData();
						_ = comm.recData;
						sahara_packet = (sahara_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_packet));
						switch (sahara_packet.Command)
						{
						case 3u:
							sahara_readdata_packet = (sahara_readdata_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_readdata_packet));
							msg = $"sahara read data:imgID {sahara_readdata_packet.Image_id}, offset {sahara_readdata_packet.Offset},length {sahara_readdata_packet.SLength}";
							fileTransfer.transfer(comm.serialPort, (int)sahara_readdata_packet.Offset, (int)sahara_readdata_packet.SLength);
							Log.w(comm.serialPort.PortName, msg);
							break;
						case 18u:
							sahara_64b_readdata_packet = (sahara_64b_readdata_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_64b_readdata_packet));
							msg = $"sahara read 64b data:imgID {sahara_64b_readdata_packet.Image_id},offset {sahara_64b_readdata_packet.Offset},length {sahara_64b_readdata_packet.SLength}";
							fileTransfer.transfer(comm.serialPort, (int)sahara_64b_readdata_packet.Offset, (int)sahara_64b_readdata_packet.SLength);
							Log.w(comm.serialPort.PortName, msg);
							break;
						case 4u:
							sahara_end_transfer_packet = (sahara_end_transfer_packet)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_end_transfer_packet));
							msg = $"sahara read end  imgID:{sahara_end_transfer_packet.Image_id} status:{sahara_end_transfer_packet.Status}";
							if (sahara_end_transfer_packet.Status != 0)
							{
								Log.w(comm.serialPort.PortName, $"sahara read end error with status:{sahara_end_transfer_packet.Status}");
							}
							flag = true;
							break;
						default:
							msg = $"invalid command:{sahara_packet.Command}";
							Log.w(comm.serialPort.PortName, msg);
							break;
						}
					}
					while (!flag);
					Log.w(comm.serialPort.PortName, "Send done packet");
					sahara_packet.Command = 5u;
					sahara_packet.Length = 8u;
					byte[] array5 = CommandFormat.StructToBytes(sahara_packet, 8);
					for (int i = 8; i < array5.Length; i++)
					{
						array5[i] = 0;
					}
					comm.WritePort(array5, 0, array5.Length);
					comm.getRecData();
					if (comm.recData.Length == 0)
					{
						comm.recData = new byte[48];
					}
					sahara_done_response = (sahara_done_response)CommandFormat.BytesToStuct(comm.recData, typeof(sahara_done_response));
					if (sahara_done_response.Command == 6)
					{
						msg = "file " + text + " transferred successfully";
						Log.w(comm.serialPort.PortName, msg);
						FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, msg, "flashing", isDone: false);
						fileTransfer.closeTransfer();
						Thread.Sleep(TIME_WAIT_FIREHOSE_START);
						return;
					}
					msg = "programmer transfer error " + sahara_done_response.Command;
					throw new Exception(msg);
				}
				throw new Exception("can not found programmer file.");
			}
			msg = "cannot receive hello packet";
			comm.serialPort.Close();
			comm.serialPort.Dispose();
			throw new Exception(msg);
		}
		Log.w(comm.serialPort.PortName, "port " + comm.serialPort.PortName + " is not open.");
		throw new Exception("port " + comm.serialPort.PortName + " is not open.");
	}

	private void FHloader()
	{
		Cmd cmd = new Cmd(deviceName, "");
		string text = "";
		int num = int.Parse(deviceName.ToLower().Replace("com", ""));
		string[] array = FileSearcher.SearchFiles(swPath, SoftwareImage.ProgrammerPattern);
		string text2 = "";
		if (array.Length != 0)
		{
			text2 = array[0];
			text = $"QSaharaServer.exe  -u {num} -s 13:{text2}";
			string[] array2 = FileSearcher.SearchFiles(swPath, SoftwareImage.RawProgramPattern);
			string[] array3 = FileSearcher.SearchFiles(swPath, SoftwareImage.PatchPattern);
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = Path.GetFileName(array2[i]);
			}
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j] = Path.GetFileName(array3[j]);
			}
			FileInfo fileInfo = new FileInfo(text2);
			if (fileInfo.Name.ToLower().IndexOf("ufs") >= 0)
			{
				storageType = Storage.ufs;
			}
			else if (fileInfo.Name.ToLower().IndexOf("emmc") >= 0)
			{
				storageType = Storage.emmc;
			}
			verbose = true;
			string text3 = "";
			if (verbose)
			{
				text3 = " --verbose";
			}
			string text4 = string.Format(" & fh_loader.exe --port=\\\\.\\{0} --sendxml={1} --search_path={2} --noprompt --showpercentagecomplete --memoryname={3} {4} --convertprogram2read", deviceName, string.Join(",", array2), swPath, storageType, text3);
			string.Format(" & fh_loader.exe --port=\\\\.\\{0} --sendxml={1} --search_path={2} --noprompt --showpercentagecomplete --maxpayloadsizeinbytes={3} --zlpawarehost=1 --memoryname={4} {5} {6}", deviceName, string.Join(",", array3), swPath, comm.intSectorSize * BUFFER_SECTORS, storageType, (m_iSkipStorageInit == 1) ? " --skipstorageinit " : "", text3);
			string.Format(" & fh_loader.exe --port=\\\\.\\{0} --setactivepartition={1} --noprompt --showpercentagecomplete --maxpayloadsizeinbytes={2} --zlpawarehost=1 --memoryname={3} {4} {5}", deviceName, (storageType.ToLower() == "ufs") ? 1 : 0, comm.intSectorSize * BUFFER_SECTORS, (m_iSkipStorageInit == 1) ? " --skipstorageinit " : "", storageType, text3);
			string.Format(" & fh_loader.exe --port=\\\\.\\{0} --reset --noprompt --showpercentagecomplete --maxpayloadsizeinbytes={1} --zlpawarehost=1 --memoryname={2} {3} {4}", deviceName, comm.intSectorSize * BUFFER_SECTORS, storageType, (m_iSkipStorageInit == 1) ? " --skipstorageinit " : "", text3);
			text += text4;
			Log.w(deviceName, text);
			cmd.Execute_returnLine(deviceName, text, 1);
			return;
		}
		throw new Exception("can not found programmer file.");
	}

	private void PropareFirehose()
	{
		ping();
	}

	private void ping()
	{
		Log.w(comm.serialPort.PortName, "send nop command");
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "ping target via firehose", "flashing", isDone: false);
		string command = string.Format(Firehose.Nop, verbose ? "1" : "0");
		if (!comm.SendCommand(command, checkAck: true))
		{
			throw new Exception("ping target failed");
		}
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, "ping target via firehose", "flashing", isDone: false);
	}

	private bool uploadDebugpolicy()
	{
		Log.w(comm.serialPort.PortName, "uploadDebugpolicy begin.");
		bool result = false;
		string text = "";
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, "uploadDebugpolicy begin", "uploadDebugpolicy", isDone: false);
		if (MiFlashGlobal.IsFactory)
		{
			if (!isUploadDebugpolicy && comm.chipNum != null)
			{
				Log.w(comm.serialPort.PortName, "Upload Debugpolicy");
				text = comm.chipNum;
				Log.w(comm.serialPort.PortName, "uploadDebugpolicy cpuid:[" + text + "]");
				CheckCPUIDResult checkCPUIDResult = FactoryCtrl.FactoryUploadDebugpolicy(deviceName, text);
				if (!checkCPUIDResult.Result)
				{
					Log.w(comm.serialPort.PortName, "factory authentication failed result " + checkCPUIDResult.Result + ", msg:" + checkCPUIDResult.Msg);
					throw new Exception("authentication failed " + checkCPUIDResult.Msg);
				}
				result = true;
				isUploadDebugpolicy = true;
			}
		}
		else
		{
			Log.w(comm.serialPort.PortName, "uploadDebugpolicy not factory.");
			result = true;
		}
		return result;
	}

	private bool dlAuth()
	{
		Log.w(comm.serialPort.PortName, "authentication edl.");
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, "等待用户输入", "授权中", isDone: false);
		string aUTHP = Firehose.AUTHP;
		byte[] array = new byte[] { 0xBF, 0x35, 0xD6, 0x01, 0x3A, 0x39, 0xD6, 0x16, 0x6B, 0xE0, 0x38, 0x7E, 0x6B, 0x9B, 0x00, 0xFD, 0x0E, 0x09, 0x62, 0x83, 0xF8, 0x11, 0xED, 0xE8, 0x15, 0x94, 0x86, 0x6C, 0xF6, 0x76, 0xB4, 0x1B, 0x1A, 0x32, 0xEA, 0x67, 0xFB, 0xAB, 0x4F, 0x6D, 0x90, 0xE4, 0x5C, 0x94, 0x4B, 0x53, 0x30, 0x2A, 0x1D, 0xA3, 0x2D, 0x94, 0xF3, 0x0A, 0x68, 0xE1, 0xEB, 0x11, 0x66, 0x72, 0xB0, 0x29, 0x20, 0x08, 0x9A, 0xA9, 0x38, 0xF9, 0x14, 0x64, 0xD6, 0x92, 0x6C, 0x42, 0xA9, 0x3D, 0x0E, 0xAE, 0x88, 0xE5, 0x49, 0xA4, 0x9C, 0x00, 0xFC, 0xF9, 0xB1, 0xB8, 0x9E, 0xF6, 0x8A, 0x7C, 0xD2, 0x3D, 0xEB, 0xEB, 0x88, 0xC0, 0x1D, 0x85, 0x0A, 0xCD, 0x52, 0xA8, 0x32, 0xBB, 0x80, 0x13, 0x4C, 0x4B, 0x0E, 0x2A, 0x7A, 0x14, 0x22, 0xE2, 0x53, 0x0C, 0x19, 0xB3, 0x09, 0xEB, 0xA1, 0xFF, 0x7E, 0x12, 0x3A, 0x34, 0xDD, 0x3B, 0x83, 0xDC, 0xFA, 0xCD, 0xCE, 0x45, 0xF3, 0x03, 0xD1, 0x35, 0xFE, 0x58, 0x89, 0x9E, 0x53, 0x1E, 0x1C, 0xF7, 0x15, 0x5D, 0x48, 0xBF, 0xF1, 0x8A, 0xB3, 0xE5, 0xFC, 0x1A, 0x2E, 0x85, 0xFB, 0xB0, 0x15, 0xDE, 0x2A, 0x3C, 0xFC, 0x8E, 0xE5, 0x1A, 0xA4, 0x53, 0xF7, 0xDE, 0xBC, 0x4A, 0x09, 0x58, 0x61, 0xDA, 0x16, 0x37, 0xC8, 0xDF, 0x4D, 0x9C, 0xF6, 0x4E, 0xC4, 0xA5, 0xF4, 0x54, 0x86, 0xAD, 0x73, 0xFB, 0x03, 0x69, 0x65, 0xB9, 0x4E, 0x1E, 0xE8, 0xF4, 0x07, 0x7F, 0xFB, 0x54, 0xE9, 0x0A, 0xF0, 0xAB, 0x52, 0xBF, 0x02, 0xE4, 0x99, 0x51, 0x7F, 0xB7, 0xD1, 0x02, 0x8A, 0xBC, 0xBA, 0x1B, 0x98, 0x95, 0x18, 0x43, 0xB2, 0xA8, 0xC9, 0x64, 0xB4, 0xD9, 0x48, 0x01, 0xBA, 0xF6, 0x30, 0xC6, 0x17, 0x9F, 0xA6, 0xF8, 0x63, 0x71, 0x83, 0x0A, 0x48, 0x4F, 0x27, 0x92, 0xD4, 0x91 };
		string command = string.Format(aUTHP, array.Length);
		bool flag = comm.SendCommand(command, checkAck: true);
		if (!flag)
		{
			throw new Exception("authentication failed");
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		byte[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			byte b = array2[j];
			stringBuilder.Append(b + " ");
			stringBuilder2.Append("0x" + b.ToString("X2") + " ");
		}
		comm.WritePort(array, 0, array.Length);
		if (!comm.GetResponse(waiteACK: true))
		{
			throw new Exception("authentication failed");
		}
		return flag;
	}

	private void ConfigureDDR(int intSectorSize, int buffer_sectors, string ddrType, int m_iSkipStorageInit)
	{
		Log.w(comm.serialPort.PortName, "send configure command");
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "send configure command", "flashing", isDone: false);
		string command = string.Format(Firehose.Configure, verbose ? "1" : "0", intSectorSize * buffer_sectors, ddrType, m_iSkipStorageInit);
		bool flag = comm.SendCommand(command, checkAck: true);
		if (!flag)
		{
			if (!comm.needEdlAuth)
			{
				Log.w(comm.serialPort.PortName, "configure failed!!!");
				throw new Exception("configure failed!!!");
			}
			foreach (Device flashDevice in FlashingDevice.flashDeviceList)
			{
				if (flashDevice.Name == comm.serialPort.PortName)
				{
					flashDevice.IsUpdate = true;
					break;
				}
			}
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, "等待用户输入", "授权中", isDone: false);
			Log.w(comm.serialPort.PortName, "edl authentication");
			if (!dlAuth())
			{
				Log.w(comm.serialPort.PortName, "authorize failed!!!");
				throw new Exception("authorize failed!!!");
			}
			flag = comm.SendCommand(command, checkAck: true);
		}
		if (Storage.ufs.ToLower() == ddrType && !isLite && !flag)
		{
			throw new Exception("send configure command failed");
		}
		Log.w(comm.serialPort.PortName, "max buffer sector is " + comm.m_dwBufferSectors);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, "send command command", "flashing", isDone: false);
	}

	private bool NeedBackupRom(string swpath)
	{
		bool result = false;
		if (FileSearcher.SearchFiles(swpath, SoftwareImage.RawBackupPattern).Length != 0)
		{
			Log.w(deviceName, "need backup rom");
			result = true;
		}
		return result;
	}

	private bool NeedRestoreRom(string swpath)
	{
		bool result = false;
		if (FileSearcher.SearchFiles(swpath, SoftwareImage.RawRestorePattern).Length != 0)
		{
			Log.w(deviceName, "need restore rom");
			result = true;
		}
		return result;
	}

	private bool NeedProvision(string swpath)
	{
		bool result = false;
		if (FileSearcher.SearchFiles(swpath, SoftwareImage.ProvisionPattern).Length != 0)
		{
			Log.w(deviceName, "need provision");
			result = true;
		}
		return result;
	}

	private bool NeedDifferentiateDdr(string swpath)
	{
		bool result = false;
		if (FileSearcher.SearchFiles(swpath, SoftwareImage.ProgrammerLite).Length != 0)
		{
			Log.w(deviceName, "sw has prog_firehose_lite.elf");
			if (FileSearcher.SearchFiles(swpath, SoftwareImage.ProgrammerDDR4).Length != 0)
			{
				Log.w(deviceName, "sw has prog_ufs_firehose_sdm855_ddr_4.elf");
				if (FileSearcher.SearchFiles(swpath, SoftwareImage.ProgrammerDDR5).Length != 0)
				{
					Log.w(deviceName, "need differentiate ddr");
					result = true;
				}
			}
		}
		return result;
	}

	private bool Provision(string swpath)
	{
		string[] array = FileSearcher.SearchFiles(swpath, SoftwareImage.ProvisionPattern);
		if (array.Length == 0)
		{
			return false;
		}
		string text = array[0];
		string msg = "start provision:" + text;
		Log.w(comm.serialPort.PortName, msg);
		XmlDocument xmlDocument = new XmlDocument();
		XmlReader xmlReader = XmlReader.Create(text, new XmlReaderSettings
		{
			IgnoreComments = true
		});
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.SelectSingleNode("data").ChildNodes;
		string text2 = "";
		int num = 0;
		try
		{
			foreach (XmlNode item in childNodes)
			{
				if (item.Name.ToLower() != "ufs")
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder("<ufs ");
				foreach (XmlAttribute attribute in item.Attributes)
				{
					if (!(attribute.Name.ToLower() == "desc"))
					{
						stringBuilder.Append(attribute.Name + "=\"" + attribute.Value + "\" ");
					}
				}
				if (verbose)
				{
					stringBuilder.Append(" verbose=\"1\" ");
				}
				stringBuilder.Append("/>");
				text2 = "<?xml version=\"1.0\" ?>\n<data>\n" + stringBuilder.ToString() + "\n</data>";
				if (!comm.SendCommand(text2, checkAck: true))
				{
					Log.w(comm.serialPort.PortName, "Provision failed :" + text2);
					throw new Exception("Provision failed");
				}
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, (float)num / (float)childNodes.Count, text, "provisioning", isDone: false);
				num++;
			}
			Log.w(comm.serialPort.PortName, "Provision done.");
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, "provisiong done", "provisioning", isDone: false);
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			xmlReader.Close();
		}
		return true;
	}

	private bool Firmwarewrite()
	{
		needFirmwarewrite = false;
		string command = string.Format(Firehose.GETSTORAGEINFO);
		string text = "";
		if (!comm.SendCommand(command, checkAck: true))
		{
			throw new Exception("ffu getstorageinfo fail");
		}
		Log.w(comm.serialPort.PortName, "storageInfo:" + comm.storageInfo);
		string text2 = comm.storageInfo.Split(':')[2].Trim();
		string text3 = text2.Substring(0, 8);
		text3 = text3.Trim();
		string text4 = text2.Substring(8, 16);
		text4 = text4.Trim();
		string text5 = text2.Substring(24, 4);
		text5 = text5.Trim();
		Log.w(comm.serialPort.PortName, "name:" + text3 + " version:" + text4 + " number:" + text5);
		try
		{
			foreach (Ffu ffu in SaveFfu.ffuList)
			{
				if (ffu.Name == text3 && ffu.Version == text4 && ffu.Number == text5)
				{
					text = ffu.File;
					needFirmwarewrite = true;
					break;
				}
			}
			Log.w(comm.serialPort.PortName, "needFirmwarewrite: " + needFirmwarewrite);
			if (needFirmwarewrite && !string.IsNullOrEmpty(text))
			{
				string text6 = MiAppConfig.Get("ffuPath").ToString() + "\\" + text;
				string strFileStartSector = "0";
				string text7 = "0";
				string strPartitionSectorNumber = "0";
				string physical_partition_number = "0";
				string sector_size = "1";
				string addtionalFirehose = "";
				if (readBackVerify)
				{
					addtionalFirehose = "read_back_verify=\"1\"";
				}
				if (!File.Exists(text6))
				{
					throw new Exception("file " + text6 + " not found.");
				}
				Log.w(comm.serialPort.PortName, "Write file " + text6 + "  sector " + text7);
				FileTransfer fileTransfer = new FileTransfer(comm.serialPort.PortName, text6);
				fileTransfer.WriteFfuFile(this, text7, strPartitionSectorNumber, text, strFileStartSector, "0", sector_size, physical_partition_number, addtionalFirehose, chkAck: true, 1);
				fileTransfer.closeTransfer();
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		return true;
	}

	private bool Reboot(string portName)
	{
		bool flag = false;
		Log.w(comm.serialPort.PortName, "restart target");
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, "restart target", "flashing", isDone: false);
		string command = string.Format(Firehose.Reset_To_Edl, verbose ? "1" : "0");
		if (!comm.SendCommand(command, checkAck: true))
		{
			throw new Exception("restart target failed");
		}
		comm.serialPort.DiscardInBuffer();
		comm.serialPort.DiscardOutBuffer();
		comm.serialPort.Close();
		comm.serialPort.Dispose();
		Thread.Sleep(5000);
		List<string> list = getDevice().ToList();
		int num = 100;
		string text = "";
		while (num-- > 0 && list.IndexOf(portName) < 0)
		{
			Thread.Sleep(100);
			list = getDevice().ToList();
			text = "waiting for " + portName + " restart";
			Log.w(comm.serialPort.PortName, text);
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, text, "restart", isDone: false);
		}
		if (list.IndexOf(portName) >= 0)
		{
			flag = true;
			text = portName + " restart successfully";
			Log.w(comm.serialPort.PortName, text);
			Thread.Sleep(800);
			bool flag2 = false;
			while (num-- > 0 && !flag2)
			{
				try
				{
					comm.serialPort.Open();
					flag2 = true;
					Log.w(comm.serialPort.PortName, " serial port " + portName + " opend successfully");
				}
				catch (Exception)
				{
					Log.w(comm.serialPort.PortName, "open serial port " + portName + " ");
					Thread.Sleep(800);
				}
			}
			flag = flag && comm.IsOpen;
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, text, "restart", isDone: false);
			return flag;
		}
		text = portName + " restart failed";
		Log.w(comm.serialPort.PortName, text);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, text, "restart", isDone: false);
		flag = false;
		throw new Exception(text);
	}

	private void SetBootPartition()
	{
		string text = "Set Boot Partition ";
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, text, "flashing", isDone: false);
		string command = string.Format(Firehose.SetBootPartition, verbose ? "1" : "0");
		if (!comm.SendCommand(command, checkAck: true))
		{
			throw new Exception("set boot partition failed");
		}
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, text, "flashing", isDone: false);
		Log.w(comm.serialPort.PortName, text);
	}

	private void FirehoseDownloadImg(string swPath)
	{
		string text = MiAppConfig.Get("rawprogram").ToString();
		string text2 = MiAppConfig.Get("patch").ToString();
		string[] array;
		string[] array2;
		if (string.IsNullOrEmpty(text))
		{
			array = FileSearcher.SearchFiles(swPath, SoftwareImage.RawProgramPattern);
			array2 = FileSearcher.SearchFiles(swPath, SoftwareImage.PatchPattern);
		}
		else
		{
			array = new string[1] { text };
			array2 = new string[1] { text2 };
		}
		if (NeedProvision(swPath) && MiFlashGlobal.IsEraseAll)
		{
			Log.w(comm.serialPort.PortName, "need erase target!");
			for (int i = 0; i < array.Length; i++)
			{
				if (storageType == Storage.ufs)
				{
					Log.w(comm.serialPort.PortName, "erase target");
					string command = string.Format(Firehose.ERASE, "4096", i);
					comm.SendCommand(command, checkAck: true);
				}
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (WriteFilesToDevice(comm.serialPort.PortName, swPath, array[j]))
			{
				ApplyPatchesToDevice(comm.serialPort.PortName, array2[j]);
			}
		}
	}

	private void GetSha256(string swPath)
	{
		if (string.IsNullOrEmpty(sha256Path))
		{
			string[] array = FileSearcher.SearchFiles(swPath, SoftwareImage.RawProgramPattern);
			for (int i = 0; i < array.Length; i++)
			{
				Log.w(comm.serialPort.PortName, "sha256 " + array[i]);
				GetSha256Digest(comm.serialPort.PortName, swPath, array[i]);
			}
		}
		else
		{
			Log.w(comm.serialPort.PortName, "sha256 " + sha256Path);
			GetSha256Digest(comm.serialPort.PortName, swPath, sha256Path);
		}
	}

	private bool ReadFilesFromDevice(string portName, string swPath, string rawFilePath)
	{
		bool result = true;
		Log.w(comm.serialPort.PortName, "open program file " + rawFilePath);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, rawFilePath, "flashing", isDone: false);
		XmlDocument xmlDocument = new XmlDocument();
		XmlReader xmlReader = XmlReader.Create(rawFilePath, new XmlReaderSettings
		{
			IgnoreComments = true
		});
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.SelectSingleNode("data").ChildNodes;
		try
		{
			bool is_sparse = false;
			string filename = "";
			string file_sector_offset = "0";
			string start_sector = "0";
			string num_partition_sectors = "0";
			string physical_partition_number = "0";
			string sector_size_in_bytes = "512";
			string label = "";
			string size_in_kb = "0";
			foreach (XmlNode item in childNodes)
			{
				if (!(item.Name.ToLower() == "program"))
				{
					continue;
				}
				foreach (XmlAttribute attribute in item.Attributes)
				{
					switch (attribute.Name.ToLower())
					{
					case "file_sector_offset":
						file_sector_offset = attribute.Value;
						break;
					case "filename":
						filename = attribute.Value;
						break;
					case "num_partition_sectors":
						num_partition_sectors = attribute.Value;
						break;
					case "start_sector":
						start_sector = attribute.Value;
						break;
					case "sparse":
						is_sparse = attribute.Value == "true";
						break;
					case "sector_size_in_bytes":
						sector_size_in_bytes = attribute.Value;
						break;
					case "physical_partition_number":
						physical_partition_number = attribute.Value;
						break;
					case "label":
						label = attribute.Value;
						break;
					case "size_in_kb":
						size_in_kb = attribute.Value;
						break;
					}
				}
				//comm.writeCount = 0;
				comm.CleanBuffer();
				string addtionalFirehose = "";
				if (readBackVerify)
				{
					addtionalFirehose = "read_back_verify=\"1\"";
				}
				DateTime now = DateTime.Now;
				filename = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "restore\\" + startTime.ToString("yyyyMdHms") + "_" + comm.chipNum + "\\" + filename;
				if (is_sparse)
				{
					Log.w(comm.serialPort.PortName, "nou support read sparse file " + filename);
					throw new Exception("nou support read sparse file " + filename);
				}
				Log.w(comm.serialPort.PortName, "Read file " + filename + " from partition[" + label + "] sector " + start_sector);
				FileTransfer fileTransfer = new FileTransfer(comm.serialPort.PortName, filename, isWriteFile: true);
				fileTransfer.ReadFile(this, start_sector, num_partition_sectors, filename, file_sector_offset, size_in_kb, "0", sector_size_in_bytes, physical_partition_number, addtionalFirehose, chkAck: true, 1);
				fileTransfer.closeTransfer();
				string text4 = (DateTime.Now - now).ToString();
				Log.w(comm.serialPort.PortName, "Image " + filename + " transferred successfully,elapse " + text4);
				//comm.writeCount = 0;
			}
			return result;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			xmlReader.Close();
		}
	}

    #region Sparse
    #region Deklarasi Sparse
#pragma warning disable
    private static CHUNK_HEADER chunkheader;
    private static SPARSE_HEADER sparseheader;
    private const Int64 SPARSE_MAGIC = unchecked((int)0xEED26FF3A);
    private const Int64 SPARSE_RAW_CHUNK = 0xECAC1;
    private const Int64 SPARSE_FILL_CHUNK = 0xECAC2;
    private const Int64 SPARSE_DONT_CARE = 0xECAC3;
    private static int totalchunk;
    private static int blocksize;
    #endregion
    private struct CHUNK_HEADER
    {
        public Int16 wChunkType;
        public Int16 wReserved;
        public Int32 dwChunkSize;
        public Int32 dwTotalSize;
    }

    private struct SPARSE_HEADER
    {
        public Int32 dwMagic; //4
        public Int16 wVerMajor; //2
        public Int16 wVerMinor; //2
        public Int16 wSparseHeaderSize; //2
        public Int16 wChunkHeaderSize; //2
        public Int32 dwBlockSize; //4
        public Int32 dwTotalBlocks; //4
        public Int32 dwTotalChunks;
        public Int32 dwImageChecksum;
    }

#pragma warning enable
    private static SPARSE_HEADER parsingheader(byte[] bytes)
    {
        SPARSE_HEADER stuff = new SPARSE_HEADER();
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            stuff = (SPARSE_HEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SPARSE_HEADER));
        }
        finally
        {
            handle.Free();
        }
        return stuff;
    }

    private static bool CekSparse(string fileName)
    {
        long header_magic = 0;
        byte[] buffer = new byte[28];
        using (var stream = File.OpenRead(fileName))
        using (var reader = new BinaryReader(stream))
        {
            reader.Read(buffer, 0, 28);
            sparseheader = parsingheader(buffer);
            var magic = sparseheader.dwMagic;
            header_magic = Convert.ToInt64(magic);

            if (header_magic == SPARSE_MAGIC)
            {
                totalchunk = sparseheader.dwTotalChunks;
                blocksize = sparseheader.dwBlockSize;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

            #endregion
        
	private bool WriteFilesToDevice(string portName, string swPath, string rawFilePath)
	{
		bool result = true;
		Log.w(comm.serialPort.PortName, "open program file " + rawFilePath);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, rawFilePath, "flashing", isDone: false);
		XmlDocument xmlDocument = new XmlDocument();
		XmlReader xmlReader = XmlReader.Create(rawFilePath, new XmlReaderSettings
		{
			IgnoreComments = true
		});
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.SelectSingleNode("data").ChildNodes;
		try
		{
			bool is_sparse = false;
			string filename = "";
			string file_sector_offset = "0";
			string start_sector = "0";
			string num_partition_sectors = "0";
			string physical_partition_number = "0";
			string sector_size_in_bytes = "512";
			string label = "";
			string text8 = "";
			foreach (XmlNode item in childNodes)
			{
				if (item.Name.ToLower() == "xblgpt")
				{
					foreach (XmlAttribute attribute in item.Attributes)
					{
						if (attribute.Name.ToLower() == "lun")
						{
							text8 = attribute.Value;
							string command = string.Format(Firehose.XBLGPT, text8);
							comm.SendCommand(command, checkAck: true);
						}
					}
				}
				if (!(item.Name.ToLower() == "program"))
				{
					continue;
				}
				foreach (XmlAttribute attribute2 in item.Attributes)
				{
					switch (attribute2.Name.ToLower())
					{
					case "file_sector_offset":
						file_sector_offset = attribute2.Value;
						break;
					case "filename":
						filename = attribute2.Value;
						break;
					case "num_partition_sectors":
						num_partition_sectors = attribute2.Value;
						break;
					case "start_sector":
						start_sector = attribute2.Value;
						break;
					case "sparse":
						is_sparse = attribute2.Value == "true";
						break;
					case "sector_size_in_bytes":
						sector_size_in_bytes = attribute2.Value;
						break;
					case "physical_partition_number":
						physical_partition_number = attribute2.Value;
						break;
					case "label":
						label = attribute2.Value;
						break;
					}
				}
				if (!string.IsNullOrEmpty(filename))
				{
					//comm.writeCount = 0;
					comm.CleanBuffer();
					filename = swPath + "\\" + filename;
					if (!File.Exists(filename))
					{
						throw new Exception("file " + filename + " not found.");
					}
					if (filename.IndexOf("gpt_main1") >= 0 || filename.IndexOf("gpt_main2") >= 0)
					{
						Thread.Sleep(1000);
					}
					string addtionalFirehose = "";
					if (readBackVerify)
					{
						addtionalFirehose = "read_back_verify=\"1\"";
					}
					DateTime now = DateTime.Now;

                    is_sparse = CekSparse(filename);

					if (is_sparse)
					{
						Log.w(comm.serialPort.PortName, "Write sparse file " + filename + " to partition[" + label + "] sector " + start_sector);
						FileTransfer fileTransfer = new FileTransfer(comm.serialPort.PortName, filename);
						fileTransfer.WriteSparseFileToDevice(this, start_sector, num_partition_sectors, filename, file_sector_offset, sector_size_in_bytes, physical_partition_number, addtionalFirehose);
						fileTransfer.closeTransfer();
					}
					else
					{
						Log.w(comm.serialPort.PortName, "Write file " + filename + " to partition[" + label + "] sector " + start_sector);
						FileTransfer fileTransfer2 = new FileTransfer(comm.serialPort.PortName, filename);
						fileTransfer2.WriteFile(this, start_sector, num_partition_sectors, filename, file_sector_offset, "0", sector_size_in_bytes, physical_partition_number, addtionalFirehose, chkAck: true, 1);
						fileTransfer2.closeTransfer();
					}
					string text9 = (DateTime.Now - now).ToString();
					Log.w(comm.serialPort.PortName, "Image " + filename + " transferred successfully,elapse " + text9);
					//comm.writeCount = 0;
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			xmlReader.Close();
		}
	}

	private void GetSha256Digest(string portName, string swPath, string rawFilePath)
	{
		Log.w(comm.serialPort.PortName, "open file " + rawFilePath);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, rawFilePath, "flashing", isDone: false);
		XmlDocument xmlDocument = new XmlDocument();
		XmlReader xmlReader = XmlReader.Create(rawFilePath, new XmlReaderSettings
		{
			IgnoreComments = true
		});
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.SelectSingleNode("data").ChildNodes;
		try
		{
			string text = "";
			string value = "0";
			string text2 = "0";
			string value2 = "0";
			string value3 = "0";
			string value4 = "512";
			foreach (XmlNode item in childNodes)
			{
				if (item.Name.ToLower() != "program")
				{
					continue;
				}
				foreach (XmlAttribute attribute in item.Attributes)
				{
					switch (attribute.Name.ToLower())
					{
					case "file_sector_offset":
						value = attribute.Value;
						break;
					case "filename":
						text = attribute.Value;
						break;
					case "num_partition_sectors":
						value2 = attribute.Value;
						break;
					case "start_sector":
						text2 = attribute.Value;
						break;
					case "sparse":
						_ = attribute.Value == "true";
						break;
					case "sector_size_in_bytes":
						value4 = attribute.Value;
						break;
					case "physical_partition_number":
						value3 = attribute.Value;
						break;
					case "label":
						_ = attribute.Value;
						break;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				long num = Convert.ToInt64(value2);
				if (num == 0L)
				{
					num = 2147483647L;
				}
				Convert.ToInt64(value);
				long num2 = Convert.ToInt64(value4);
				long num3 = Convert.ToInt64(value3);
				Log.w(comm.serialPort.PortName, "checking sha256 " + text);
				FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, null, "checking sha256 " + text, "checking sha256", isDone: false);
				string command = string.Format(Firehose.FIREHOSE_SHA256DIGEST, num2, num, text2, num3);
				if (comm.SendCommand(command, checkAck: true))
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				while (!flag2 && !flag)
				{
					foreach (XmlDocument item2 in comm.GetResponseXml(waiteACK: true))
					{
						foreach (XmlNode childNode in item2.SelectSingleNode("data").ChildNodes)
						{
							foreach (XmlAttribute attribute2 in ((XmlElement)childNode).Attributes)
							{
								if (attribute2.Value.ToLower() == "ack")
								{
									flag = true;
								}
								else if (attribute2.Value.ToLower() == "nak")
								{
									flag2 = true;
								}
							}
						}
					}
					Thread.Sleep(50);
					comm.GetResponseXml(waiteACK: false);
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			xmlReader.Close();
		}
	}

	private bool ApplyPatchesToDevice(string portName, string patchFilePath)
	{
		bool result = true;
		Log.w(comm.serialPort.PortName, "open patch file " + patchFilePath);
		FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 0f, patchFilePath, "flashing", isDone: false);
		XmlDocument xmlDocument = new XmlDocument();
		XmlReader xmlReader = XmlReader.Create(patchFilePath, new XmlReaderSettings
		{
			IgnoreComments = true
		});
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.SelectSingleNode("patches").ChildNodes;
		string text = "";
		string pszPatchSize = "0";
		string pszPatchValue = "0";
		string pszDiskOffsetSector = "0";
		string pszSectorOffsetByte = "0";
		string pszPhysicalPartitionNumber = "0";
		string pszSectorSizeInBytes = "512";
		try
		{
			foreach (XmlNode item in childNodes)
			{
				if (item.Name.ToLower() != "patch")
				{
					continue;
				}
				foreach (XmlAttribute attribute in item.Attributes)
				{
					switch (attribute.Name.ToLower())
					{
					case "byte_offset":
						pszSectorOffsetByte = attribute.Value;
						break;
					case "filename":
						text = attribute.Value;
						break;
					case "size_in_bytes":
						pszPatchSize = attribute.Value;
						break;
					case "start_sector":
						pszDiskOffsetSector = attribute.Value;
						break;
					case "value":
						pszPatchValue = attribute.Value;
						break;
					case "sector_size_in_bytes":
						pszSectorSizeInBytes = attribute.Value;
						break;
					case "physical_partition_number":
						pszPhysicalPartitionNumber = attribute.Value;
						break;
					}
				}
				if (text.ToLower() == "disk")
				{
					ApplyPatch(pszDiskOffsetSector, pszSectorOffsetByte, pszPatchValue, pszPatchSize, pszSectorSizeInBytes, pszPhysicalPartitionNumber);
				}
			}
			FlashingDevice.UpdateDeviceStatus(comm.serialPort.PortName, 1f, patchFilePath, "flashing", isDone: false);
			return result;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			xmlReader.Close();
		}
	}

	private void ApplyPatch(string pszDiskOffsetSector, string pszSectorOffsetByte, string pszPatchValue, string pszPatchSize, string pszSectorSizeInBytes, string pszPhysicalPartitionNumber)
	{
		Log.w(comm.serialPort.PortName, "ApplyPatch sector " + pszDiskOffsetSector + ", offset " + pszSectorOffsetByte + ", value " + pszPatchValue + ", size " + pszPatchSize);
		string text = "";
		if (readBackVerify)
		{
			text = "read_back_verify=\"1\"";
		}
		string command = string.Format(Firehose.FIREHOSE_PATCH, pszSectorSizeInBytes, pszSectorOffsetByte, pszPhysicalPartitionNumber, pszPatchSize, pszDiskOffsetSector, pszPatchValue, text);
		comm.SendCommand(command);
	}

	public override string[] getDevice()
	{
		return ComPortCtrl.getDevicesQc();
	}

	private void BackupRom()
	{
		if (NeedBackupRom(swPath))
		{
			string[] array = FileSearcher.SearchFiles(swPath, SoftwareImage.RawBackupPattern);
			for (int i = 0; i < array.Length; i++)
			{
				ReadFilesFromDevice(comm.serialPort.PortName, swPath, array[i]);
			}
		}
	}
}
