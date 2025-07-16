using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using MiUSB;
using WP_Tool_Pro_DXApplication.Gui;
using XiaoMiFlash.code.authFlash;
using XiaoMiFlash.code.bl;
using XiaoMiFlash.code.data;
using XiaoMiFlash.code.lan;
using XiaoMiFlash.code.MiControl;
using XiaoMiFlash.code.module;
//using XiaoMiFlash.code.mtkFlashTool;
using XiaoMiFlash.code.Utility;
using XiaoMiFlash.form;

namespace XiaoMiFlash;

public class MainFrm : MiBaseFrm
{
	private USB miUsb = new USB();

	private SynchronizationContext m_SyncContext;

	private string validateSpecifyXml;

	private bool readbackverify;

	private bool openwritedump;

	private bool openreaddump;

	private bool verbose;

	private bool eraseall;

	public string chip;

	private int downloadEclipse;

	private System.Windows.Forms.Timer eclipseTimer;

	private List<string> comportList = new List<string>();

	private bool autodetectdevice;

	private bool jlqReboot;

	private bool jlqEraseWholeFlash;

	private bool jlqNoTimeout;

	private bool jlqUpdateGpt;

	private bool jlqNonsecuredTL;

	private bool jlqRebootToBootrom;

	private bool jlqVerifyData;

	private bool jlqVerifyPartitions;

	private bool jlqHash;

	private bool jlqBackupQcn;

	private string script = "";

	private List<Device> deviceArr = new List<Device>();

	private List<Ffu> ffuArr = new List<Ffu>();

	private byte[] result = new byte[1024];

	private int myPort = 6002;

	public bool isAutoFlash;

	private bool isFactory;

	public string factory = string.Empty;

	private Socket serverSocket;

	private Socket clientSocket;

	private bool canFlash = true;

	private bool isAutomate;

	private bool isDaConnect;

	private bool isAutoPDL;

	private List<Thread> threads = new List<Thread>();

	private List<SocketChatClient> m_aryClients = new List<SocketChatClient>();

	private static readonly object poslocker = new object();

	private static readonly object flashingdevicelocker = new object();

	private ProcessFrm frm = new ProcessFrm();

	//private List<MTK_DL.ROM_INFO> rom_info_list = new List<MTK_DL.ROM_INFO>();

	private static string dlFile = null;

	private static string daFile = null;

	private static string authFile = null;

	public static List<PdlPos> posList = new List<PdlPos>();

	private bool iswork = true;

	public static List<PdlPos> posListbak = new List<PdlPos>();

	private bool isbakwork;

	private int maxnum = 10;

	private int maxwaittime = 60;

	private string currentTime = "0";

	private IContainer components;

	private TextBox txtPath;

	private Button btnBrwDic;

	private VistaFolderBrowserDialog fbdSelect;

	private Button btnRefresh;

	private Button btnFlash;

	private System.Windows.Forms.Timer timer_updateStatus;

	private StatusStrip statusStrp;

	private ToolStripStatusLabel statusTab;

	private RadioStripItem rdoCleanAll;

	private RadioStripItem rdoSaveUserData;

	private RadioStripItem rdoCleanAllAndLock;

	private Label lblMD5;

	private MenuStrip mnsAuth;

	private ToolStripMenuItem miConfiguration;

	private ToolStripMenuItem miFlashConfigurationToolStripMenuItem;

	private ToolStripMenuItem driverTsmItem;

	private ToolStripMenuItem otherToolStripMenuItem;

	private ToolStripMenuItem checkSha256ToolStripMenuItem;

	private ToolStripMenuItem logToolStripMenuItem;

	private ComboBoxStripItem cmbScriptItem;

	private Panel pnlQcom;

	private Button btnAutoFlash;

	private ToolStripMenuItem flashLogToolStripMenuItem;

	private ToolStripMenuItem fastbootLogToolStripMenuItem;

	private ToolStripStatusLabel lblAccount;

	private RichTextBox txtLog;
    private ListView devicelist;
    private ColumnHeader clnID;
    private ColumnHeader clnDevice;
    private ColumnHeader clnProgress;
    private ColumnHeader clnTime;
    private ColumnHeader clnStatus;
    private ColumnHeader clnResult;

	public string ValidateSpecifyXml
	{
		get
		{
			return validateSpecifyXml;
		}
		set
		{
			validateSpecifyXml = value;
		}
	}

	public string SwPath => txtPath.Text;

	public bool ReadBackVerify
	{
		get
		{
			return readbackverify;
		}
		set
		{
			readbackverify = value;
		}
	}

	public bool OpenWriteDump
	{
		get
		{
			return openwritedump;
		}
		set
		{
			openwritedump = value;
		}
	}

	public bool OpenReadDump
	{
		get
		{
			return openreaddump;
		}
		set
		{
			openreaddump = value;
		}
	}

	public bool Verbose
	{
		get
		{
			return verbose;
		}
		set
		{
			verbose = value;
		}
	}

	public bool EraseAll
	{
		get
		{
			return eraseall;
		}
		set
		{
			eraseall = value;
		}
	}

	public bool AutoDetectDevice
	{
		get
		{
			return autodetectdevice;
		}
		set
		{
			autodetectdevice = value;
		}
	}

	public bool JLQreboot
	{
		get
		{
			return jlqReboot;
		}
		set
		{
			jlqReboot = value;
		}
	}

	public bool JLQeraseWholeFlash
	{
		get
		{
			return jlqEraseWholeFlash;
		}
		set
		{
			jlqEraseWholeFlash = value;
		}
	}

	public bool JLQnoTimeout
	{
		get
		{
			return jlqNoTimeout;
		}
		set
		{
			jlqNoTimeout = value;
		}
	}

	public bool JLQupdateGpt
	{
		get
		{
			return jlqUpdateGpt;
		}
		set
		{
			jlqUpdateGpt = value;
		}
	}

	public bool JLQnonsecuredTL
	{
		get
		{
			return jlqNonsecuredTL;
		}
		set
		{
			jlqNonsecuredTL = value;
		}
	}

	public bool JLQrebootToBootrom
	{
		get
		{
			return jlqRebootToBootrom;
		}
		set
		{
			jlqRebootToBootrom = value;
		}
	}

	public bool JLQverifyData
	{
		get
		{
			return jlqVerifyData;
		}
		set
		{
			jlqVerifyData = value;
		}
	}

	public bool JLQverifyPartitions
	{
		get
		{
			return jlqVerifyPartitions;
		}
		set
		{
			jlqVerifyPartitions = value;
		}
	}

	public bool JLQhash
	{
		get
		{
			return jlqHash;
		}
		set
		{
			jlqHash = value;
		}
	}

	public bool JLQbackupQcn
	{
		get
		{
			return jlqBackupQcn;
		}
		set
		{
			jlqBackupQcn = value;
		}
	}

	public List<string> ComportList
	{
		get
		{
			string empty = string.Empty;
			if (!string.IsNullOrEmpty(MiAppConfig.Get("mtkComs")))
			{
				comportList = MiAppConfig.Get("mtkComs").Split(',').ToList();
			}
			else
			{
				comportList.Clear();
				UsbDevice.MtkDevice.Clear();
			}
			UsbDevice.MtkDevice.Clear();
			foreach (string comport in comportList)
			{
				if (!string.IsNullOrEmpty(comport))
				{
					empty = "com" + comport;
					if (UsbDevice.MtkDevice.IndexOf(empty) < 0)
					{
						UsbDevice.MtkDevice.Add(empty);
					}
				}
			}
			return comportList;
		}
	}

	public MainFrm()
	{
		InitializeComponent();
		m_SyncContext = SynchronizationContext.Current;
	}

	public void SetFactory(string factory)
	{
		if (!string.IsNullOrEmpty(factory))
		{
			statusTab.Text = factory;
			isFactory = true;
			ForceLockCrc();
		}
		else
		{
			statusTab.Text = factory;
			isFactory = false;
		}
		MiFlashGlobal.IsFactory = isFactory;
	}

	public void SetEraseAll(bool isEraseAll)
	{
		MiFlashGlobal.IsEraseAll = isEraseAll;
	}

	public void SetBackupOnly(bool isBackupOnly)
	{
		MiFlashGlobal.IsBackupOnly = isBackupOnly;
	}

	public void SetFirmwarewrite(bool isFirmwarewrite)
	{
		MiFlashGlobal.IsFirmwarewrite = isFirmwarewrite;
	}

	public void SetChip(string chip)
	{
		if (!string.IsNullOrEmpty(chip))
		{
			switch (chip)
			{
			case "Qualcomm":
				SetChip_Qualcomm();
				break;
			case "MTK":
				SetChip_MTK();
				break;
			case "JLQ":
				SetChip_JLQ();
				break;
			default:
				SetChip_Qualcomm();
				break;
			}
		}
	}

	private void MainFrm_Load(object sender, EventArgs e)
	{
		Directory.CreateDirectory(Application.StartupPath + "\\Log");
		string name = "Software\\XiaoMi\\MiFlash\\";
		if (Registry.LocalMachine.OpenSubKey(name, writable: true) == null)
		{
			DriverFrm driverFrm = new DriverFrm();
			driverFrm.TopMost = true;
			driverFrm.Show();
		}
		SetLanguage();
		string text = MiAppConfig.Get("swPath");
		txtPath.Text = text;
		SetFactory(MiAppConfig.Get("factory"));
		factory = MiAppConfig.Get("factory");
		if (Directory.Exists(txtPath.Text))
		{
			SetScriptItems(txtPath.Text);
			cmbScriptItem.SetText(MiAppConfig.Get("script"));
		}
		script = MiAppConfig.Get("script");
		SetChip("Qualcomm");
		MiFlashGlobal.Version = Text;
	}

	public void AutoDetectUsb()
	{
		RefreshDevice();
		if (AutoDetectDevice)
		{
			miUsb.AddUSBEventWatcher(USBInsertHandler, null, new TimeSpan(0, 0, 3));
		}
	}

	private void USBInsertHandler(object sender, EventArrivedEventArgs e)
	{
		if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
		{
			btnAutoFlash.BeginInvoke((Action<string>)delegate
			{
				btnAutoFlash_Click(btnAutoFlash, new EventArgs());
			}, "");
		}
		else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
		{
			btnAutoFlash.BeginInvoke((Action<string>)delegate
			{
				btnAutoFlash_Click(btnAutoFlash, new EventArgs());
			}, "");
		}
	}

	private void USBRemoveHandler(object sender, EventArrivedEventArgs e)
	{
	}

	private void SetText(string text)
	{
		if (btnRefresh.InvokeRequired)
		{
			btnRefresh.BeginInvoke((Action<string>)delegate
			{
				btnRefresh_Click(btnRefresh, new EventArgs());
			}, text);
		}
		else
		{
			btnRefresh_Click(btnRefresh, new EventArgs());
		}
	}

	private void SetLogText(string text)
	{
		try
		{
			if (txtLog.InvokeRequired)
			{
				txtLog.BeginInvoke((Action<string>)delegate(string msg)
				{
					if (txtLog.Text.Length >= 575488)
					{
						txtLog.Text = "";
					}
					txtLog.AppendText(msg);
					txtLog.AppendText("\r\n");
					txtLog.Select(txtLog.TextLength, 0);
					txtLog.ScrollToCaret();
				}, text);
			}
			else
			{
				if (txtLog.Text.Length >= 575488)
				{
					txtLog.Text = "";
				}
				txtLog.AppendText(text);
				txtLog.AppendText("\r\n");
				txtLog.Select(txtLog.TextLength, 0);
				txtLog.ScrollToCaret();
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message + ":" + ex.StackTrace);
		}
	}

	private void Valide()
	{
		try
		{
			string validateMsg = ImageValidation.Validate(txtPath.Text);
			if (validateMsg.IndexOf("md5 validate successfully") < 0)
			{
				lblMD5.ForeColor = Color.Red;
			}
			Invoke((EventHandler)delegate
			{
				lblMD5.Text = validateMsg;
				frm.TopMost = false;
				frm.Hide();
			});
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}

	private void ShowMsg(string msg, bool openOrClose)
	{
		try
		{
			Invoke((EventHandler)delegate
			{
				lblMD5.Text = msg;
				if (openOrClose)
				{
					frm.TopMost = true;
					frm.Show();
				}
				else
				{
					frm.TopMost = false;
					frm.Hide();
				}
			});
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}

	private void btnBrwDic_Click(object sender, EventArgs e)
	{
		if (fbdSelect.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		MemImg.distory();
		txtPath.Text = fbdSelect.SelectedPath;
		SetScriptItems(txtPath.Text);
		if (MiAppConfig.GetAppConfig("checkMD5").ToLower() != "true")
		{
			return;
		}
		try
		{
			frm.TopMost = true;
			frm.Show();
			Thread thread = new Thread(Valide);
			thread.IsBackground = true;
			thread.Start();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
			Log.w(ex.Message + " " + ex.StackTrace);
		}
	}

	private void SetScriptItems(string path)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		List<string> list = new List<string>();
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Name.LastIndexOf(".bat") >= 0)
			{
				list.Add(fileInfo.Name);
			}
		}
		if (list.Count() == 0)
		{
			MessageBox.Show("couldn't find flash script.");
			return;
		}
		cmbScriptItem.SetItem(list.ToArray());
		if (Directory.Exists(txtPath.Text))
		{
			string[] array = FileSearcher.SearchFiles(txtPath.Text, FlashType.SaveUserData);
			if (rdoCleanAll.IsChecked)
			{
				script = FlashType.CleanAll;
			}
			else if (rdoSaveUserData.IsChecked)
			{
				script = Path.GetFileName(array[0]);
			}
			else if (rdoCleanAllAndLock.IsChecked)
			{
				script = FlashType.CleanAllAndLock;
			}
			cmbScriptItem.SetText(script);
		}
	}

	private void btnRefresh_Click(object sender, EventArgs e)
	{
		RefreshDevice();
	}

	public void CheckIsFactory()
	{
		try
		{
			if (MiAppConfig.Get("isFactory") == "1" && (string.IsNullOrEmpty(MiAppConfig.Get("factory")) || MiAppConfig.Get("factory") == "" || MiAppConfig.Get("factory").Length == 0))
			{
				MessageBox.Show("Please set factory mes");
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			Log.w(ex.StackTrace);
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshDevice()
	{
		try
		{
			btnRefresh.Enabled = false;
			btnFlash.Enabled = false;
			btnRefresh.Cursor = Cursors.WaitCursor;
			btnFlash.Cursor = Cursors.WaitCursor;
			CheckIsFactory();
			deviceArr = UsbDevice.GetDevice();
			new List<Thread>(threads);
			IEnumerable<string> dr = GetFlashDoneDevice();
			DrInit(ref dr);
			foreach (Device d in deviceArr)
			{
				dr = from fd in FlashingDevice.flashDeviceList
					where fd.Name == d.Name
					select fd.Name;
				if (dr.Count() == 0)
				{
					d.Progress = 0f;
					d.IsDone = null;
					d.IsUpdate = false;
					if (d.Name.Contains("?"))
					{
						Log.w(d.Name + "device is err Because it contains '?'");
						continue;
					}
					Log.w("add device " + d.Name + " index " + d.Index);
					FlashingDevice.flashDeviceList.Add(d);
					float num = 0f;
					DrawCln(d.Index, d.Name, num);
				}
				else
				{
					Log.w("Dulicate device " + d.Name);
				}
			}
			reDrawProgress();
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			Log.w(ex.StackTrace);
			MessageBox.Show(ex.Message);
		}
		finally
		{
			btnRefresh.Enabled = true;
			btnRefresh.Cursor = Cursors.Default;
			btnFlash.Enabled = true;
			btnFlash.Cursor = Cursors.Default;
		}
	}

	private void DrawCln(int deviceIndex, string deviceName, double progress)
	{
		ListViewItem listViewItem = new ListViewItem(new string[6]
		{
			deviceIndex.ToString(),
			deviceName,
			"",
			"0s",
			"",
			""
		});
		devicelist.Items.Add(listViewItem);
		Rectangle rectangle = default(Rectangle);
		ProgressBar progressBar = new ProgressBar();
		rectangle = listViewItem.SubItems[2].Bounds;
		rectangle.Width = devicelist.Columns[2].Width;
		progressBar.Parent = devicelist;
		progressBar.SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		progressBar.Value = (int)progress;
		progressBar.Visible = true;
		progressBar.Name = deviceName + "progressbar";
	}

	private void btnFlash_Click(object sender, EventArgs e)
	{
		if (!isAutoFlash)
		{
			RefreshDevice();
		}
		if (string.IsNullOrEmpty(txtPath.Text))
		{
			MessageBox.Show("Please select sw");
			return;
		}
		SaveFfu.GetFfu(txtPath.Text.Trim() + "\\images");
		File.Exists(txtPath.Text + "\\" + script);
		if (FlashingDevice.flashDeviceList.Count == 0)
		{
			return;
		}
		try
		{
			if (!timer_updateStatus.Enabled)
			{
				timer_updateStatus.Enabled = true;
			}
			foreach (Device flashDevice in FlashingDevice.flashDeviceList)
			{
				if (flashDevice.StatusList.Count > 0)
				{
					Log.w(flashDevice.Name + " already in flashing");
					continue;
				}
				if (flashDevice.StartTime > DateTime.MinValue && flashDevice.IsDone.HasValue && !flashDevice.IsDone.Value && flashDevice.IsUpdate)
				{
					Log.w(flashDevice.Name + " already in flashing");
					continue;
				}
				flashDevice.StartTime = DateTime.Now;
				flashDevice.Status = "flashing";
				flashDevice.Progress = 0f;
				flashDevice.IsDone = false;
				flashDevice.IsUpdate = true;
				Log.w(flashDevice.Name, MiFlashGlobal.Version);
				FlashingDevice.UpdateDeviceStatus(flashDevice.Name, null, "start flash", "flashing", isDone: false);
				Log.w(flashDevice.Name, "vboytest index:" + flashDevice.Index);
				if (isFactory)
				{
					if (flashDevice.DeviceCtrl.GetType() == typeof(ScriptDevice))
					{
						ForceLockCrc();
						if (factory == "MES")
						{
							Log.w(flashDevice.Name, "MES check in flash thread");
						}
						else if (factory == "X5MES")
						{
							Log.w(flashDevice.Name, "X5MES check in flash thread");
						}
						else
						{
							Log.w(flashDevice.Name, "factory dll log start");
							CheckCPUIDResult searchPathD = FactoryCtrl.GetSearchPathD(flashDevice.Name, txtPath.Text.Trim());
							if (!searchPathD.Result)
							{
								FlashingDevice.UpdateDeviceStatus(flashDevice.Name, null, "error:CheckCPUID result " + searchPathD.Result + " status " + searchPathD.Msg, "factory ev error", isDone: true);
								Log.w(flashDevice.Name, "error:device " + flashDevice.Name + " CheckCPUID result " + searchPathD.Result + " status " + searchPathD.Msg, throwEx: false);
								continue;
							}
							txtPath.Text = searchPathD.Path;
							Log.w(flashDevice.Name, "factory dll log end");
						}
					}
					else if (flashDevice.DeviceCtrl.GetType() == typeof(SerialPortDevice))
					{
						Log.w(flashDevice.Name, "factory:" + factory);
						if (factory == "Inventec")
						{
							isAutomate = true;
						}
						if (isAutomate && !isDaConnect)
						{
							EDL_SA_COMMUNICATION.DaConnect();
							isDaConnect = true;
						}
					}
					Log.w(flashDevice.Name, "factory env");
				}
				DeviceCtrl deviceCtrl = flashDevice.DeviceCtrl;
				deviceCtrl.deviceName = flashDevice.Name;
				deviceCtrl.swPath = txtPath.Text.Trim();
				deviceCtrl.flashScript = script;
				deviceCtrl.readBackVerify = ReadBackVerify;
				deviceCtrl.openReadDump = OpenReadDump;
				deviceCtrl.openWriteDump = OpenWriteDump;
				deviceCtrl.verbose = Verbose;
				deviceCtrl.idproduct = flashDevice.IdProduct;
				deviceCtrl.idvendor = flashDevice.IdVendor;
				deviceCtrl.startTime = flashDevice.StartTime;
				Thread thread = new Thread(deviceCtrl.flash);
				thread.Name = flashDevice.Name;
				thread.IsBackground = true;
				thread.Start();
				flashDevice.DThread = thread;
				threads.Add(thread);
				Log.w($"Thread start,thread id {thread.ManagedThreadId},thread name {thread.Name}");
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
			Log.w(ex.Message + "  " + ex.StackTrace);
		}
	}

	private void ForceLockCrc()
	{
		script = FlashType.CleanAllAndLock;
		rdoCleanAll.IsChecked = false;
		rdoCleanAll.Enabled = false;
		rdoCleanAllAndLock.IsChecked = false;
		rdoCleanAllAndLock.Enabled = false;
		rdoSaveUserData.IsChecked = false;
		rdoSaveUserData.Enabled = false;
		cmbScriptItem.OnlyDrop();
		cmbScriptItem.SetItem(new string[1] { FlashType.CleanAllAndLock });
		cmbScriptItem.SetText(FlashType.CleanAllAndLock);
		cmbScriptItem.Enabled = false;
		cmbScriptItem.Enable(enable: false);
	}

	private void btnAutoFlash_Click(object sender, EventArgs e)
	{
		RefreshDevice();
		btnFlash.BeginInvoke((Action<string>)delegate
		{
			btnFlash_Click(btnFlash, new EventArgs());
		}, "");
	}

	private void timer_updateStatus_Tick(object sender, EventArgs e)
	{
		try
		{
			foreach (Thread thread in threads)
			{
				_ = thread;
			}
			if (FlashingDevice.consoleMode_UsbInserted)
			{
				FlashingDevice.consoleMode_UsbInserted = false;
				RefreshDevice();
			}
			bool? flag = null;
			string text = "";
			foreach (ListViewItem item in devicelist.Items)
			{
				if (item.UseItemStyleForSubItems)
				{
					item.UseItemStyleForSubItems = false;
				}
				text = item.SubItems[1].Text.ToLower();
				foreach (Device flashDevice in FlashingDevice.flashDeviceList)
				{
					if (!flashDevice.IsUpdate)
					{
						continue;
					}
					if (flashDevice.DeviceCtrl.GetType() == typeof(ScriptDevice))
					{
						foreach (string item2 in new List<string>(flashDevice.StatusList))
						{
							string.IsNullOrEmpty(item2);
						}
					}
					if (!(flashDevice.Name.ToLower() == text))
					{
						continue;
					}
					flag = true;
					item.SubItems[2].Text = flashDevice.Progress * 100f + "%";
					foreach (Control control in devicelist.Controls)
					{
						if (control.Name == flashDevice.Name + "progressbar")
						{
							ProgressBar obj = (ProgressBar)control;
							if (obj.Value == (int)(flashDevice.Progress * 100f) && (int)(flashDevice.Progress * 100f) < 100)
							{
								flashDevice.Progress += 0.001f;
							}
							obj.Value = (int)(flashDevice.Progress * 100f);
						}
					}
					if (flashDevice.StartTime > DateTime.MinValue)
					{
						int num = (int)DateTime.Now.Subtract(flashDevice.StartTime).TotalSeconds;
						item.SubItems[3].Text = num + "s";
					}
					item.SubItems[4].Text = flashDevice.Status;
					item.SubItems[5].Text = flashDevice.Result;
					if (flashDevice.Status.ToLower() == "wait for device insert")
					{
						continue;
					}
					bool? flag2 = null;
					if (flashDevice.IsDone.HasValue && flashDevice.IsDone.Value && flashDevice.Status == "flash done")
					{
						flashDevice.IsDone = true;
						item.SubItems[5].BackColor = Color.LightGreen;
						flag2 = true;
					}
					if (flashDevice.IsDone.HasValue && flashDevice.IsDone.Value && flashDevice.Result.ToLower() == "success")
					{
						flashDevice.IsDone = true;
						item.SubItems[5].BackColor = Color.LightGreen;
						flag2 = true;
					}
					else if (flashDevice.Result.ToLower().IndexOf("error") >= 0 || flashDevice.Result.ToLower().IndexOf("fail") >= 0)
					{
						flashDevice.IsDone = true;
						item.SubItems[5].BackColor = Color.Red;
						flag2 = false;
					}
					if (!flag2.HasValue)
					{
						item.SubItems[5].BackColor = Color.White;
					}
					string text2 = "#USB";
					string text3 = "";
					if (!flashDevice.IsDone.HasValue || !flashDevice.IsDone.Value || !flag2.HasValue)
					{
						break;
					}
					try
					{
						if (flag2.Value)
						{
							text2 = text2 + flashDevice.Index + "OK$";
							text3 = "Pass," + flashDevice.Index + ";";
						}
						else
						{
							text2 = text2 + flashDevice.Index + "NG$";
							text3 = "Fail," + flashDevice.Index + ";";
						}
						if (!isAutoPDL)
						{
							TimeSpan timeSpan = DateTime.Now.Subtract(flashDevice.StartTime);
							Log.wFlashStatus(flashDevice.Index + "    " + flashDevice.Name + "     " + timeSpan.TotalSeconds + "s     " + flashDevice.Status);
						}
					}
					catch (Exception ex)
					{
						Log.w(ex.Message + " " + ex.StackTrace);
					}
					if (isFactory && flashDevice.CheckCPUID && flag2.HasValue && flashDevice.DeviceCtrl.GetType() == typeof(ScriptDevice))
					{
						try
						{
							string text4 = " update flash result to facotry server……";
							statusTab.Text += text4;
							if (!(factory == "MES") && !(factory == "X5MES"))
							{
								Log.w(flashDevice.Name, $"update flash result to facotry server devices id{flashDevice.Name} flashresult {flag2.Value}", throwEx: false);
								FlashResult flashResult = FactoryCtrl.SetFlashResultD(flashDevice.Name, flag2.Value);
								statusTab.Text = MiAppConfig.Get("factory");
								if (!flashResult.Result)
								{
									flashDevice.IsDone = true;
									item.SubItems[4].Text = flashResult.Msg;
									item.SubItems[5].Text = "error";
									item.SubItems[5].BackColor = Color.Red;
								}
								else
								{
									flashDevice.IsDone = true;
									Log.w(flashDevice.Name, "flashResult.Result is true");
								}
							}
						}
						catch (Exception ex2)
						{
							item.SubItems[4].Text = ex2.Message;
							item.SubItems[5].Text = "error";
							item.SubItems[5].BackColor = Color.Red;
							Log.w(ex2.Message + " " + ex2.StackTrace);
						}
					}
					if (isAutomate && flag2.HasValue)
					{
						try
						{
							string text5 = " update flash result to facotry server……";
							statusTab.Text += text5;
							PInvokeResultArg pInvokeResultArg = EDL_SA_COMMUNICATION.RspEdlResult(flashDevice.Name, flag2.Value);
							statusTab.Text = MiAppConfig.Get("factory");
							if (pInvokeResultArg.result <= 0)
							{
								flashDevice.IsDone = true;
								item.SubItems[4].Text = pInvokeResultArg.lastErrMsg;
								item.SubItems[5].Text = "error";
								item.SubItems[5].BackColor = Color.Red;
							}
							else
							{
								flashDevice.IsDone = true;
								Log.w(flashDevice.Name, "flashResult.Result is true");
							}
						}
						catch (Exception ex3)
						{
							item.SubItems[4].Text = ex3.Message;
							item.SubItems[5].Text = "error";
							item.SubItems[5].BackColor = Color.Red;
							Log.w(ex3.Message + " " + ex3.StackTrace);
						}
					}
					if (flag2.HasValue)
					{
						flashDevice.IsUpdate = false;
						flashDevice.IsDone = true;
					}
					if (m_aryClients.Count > 0 && m_aryClients[0].Sock != null)
					{
						if (isAutoPDL)
						{
							Log.w("device " + flashDevice.Name + " reply result" + text3);
							SetLogText("send :" + text3 + "\r\n");
							m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text3));
						}
						else
						{
							SetLogText("send :" + text2 + "\r\n");
							m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text2));
							if (FlashingDevice.IsAllDone())
							{
								string text6 = "OPEN\r\n";
								m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text6));
								SetLogText(text6);
							}
						}
					}
					if (clientSocket != null)
					{
					}
					break;
				}
				if (flag.HasValue && !flag.Value)
				{
					Log.w("couldn't find " + item.SubItems[1].Text.ToLower() + " in FlashingDevice.flashDeviceList");
				}
			}
		}
		catch (Exception ex4)
		{
			Log.w(ex4.Message + " " + ex4.StackTrace);
		}
		UpdateThreadStatus();
	}

	private void UpdateThreadStatus()
	{
		foreach (Thread item in new List<Thread>(threads))
		{
			if (!item.IsAlive)
			{
				Log.w($"Thread stopped, thread id {item.ManagedThreadId}, thread name {item.Name}");
				threads.Remove(item);
			}
		}
	}

	private void cleanMiFlashTmp()
	{
		try
		{
			if (!Directory.Exists(txtPath.Text))
			{
				return;
			}
			string[] files = Directory.GetFiles(txtPath.Text);
			foreach (string text in files)
			{
				if (File.Exists(text) && new FileInfo(text).Name.IndexOf("miflashTmp_") == 0)
				{
					File.Delete(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}

	private void cleanMiFlashTmp(string prefix)
	{
		try
		{
			if (!Directory.Exists(txtPath.Text))
			{
				return;
			}
			string[] files = Directory.GetFiles(txtPath.Text);
			foreach (string text in files)
			{
				if (File.Exists(text) && new FileInfo(text).Name.IndexOf(prefix) == 0)
				{
					File.Delete(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}

	private void devicelist_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
	{
		Rectangle rectangle = default(Rectangle);
		_ = e.NewWidth;
		foreach (Control control in devicelist.Controls)
		{
			if (control.Name.IndexOf("progressbar") >= 0)
			{
				ProgressBar obj = (ProgressBar)control;
				rectangle = obj.Bounds;
				rectangle.Width = devicelist.Columns[2].Width;
				obj.SetBounds(devicelist.Items[0].SubItems[2].Bounds.X, rectangle.Y, rectangle.Width, rectangle.Height);
			}
		}
	}

	private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			MiAppConfig.SetValue("swPath", txtPath.Text.ToString());
			if (serverSocket != null)
			{
				serverSocket.Close();
			}
			if (clientSocket != null)
			{
				clientSocket.Close();
			}
			if (m_aryClients.Count > 0 && m_aryClients[0].Sock != null)
			{
				m_aryClients[0].Sock.Close();
			}
			MiProcess.KillProcess("fh_loader");
			cleanMiFlashTmp();
			miUsb.RemoveUSBEventWatcher();
			MemImg.distory();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
			Log.w(ex.Message + " " + ex.StackTrace);
		}
	}

	private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
	{
		System.Environment.Exit(System.Environment.ExitCode);
		Dispose();
		Close();
	}

	public override void SetLanguage()
	{
		base.SetLanguage();
		if (CultureInfo.CurrentUICulture.Name.ToLower().IndexOf("zh") >= 0)
		{
			base.LanID = LanguageType.chn_s;
		}
		else
		{
			base.LanID = LanguageType.eng;
		}
		LanguageProvider languageProvider = new LanguageProvider(base.LanID);
		btnBrwDic.Text = languageProvider.GetLanguage("MainFrm.btnBrwDic");
		btnRefresh.Text = languageProvider.GetLanguage("MainFrm.btnRefresh");
		btnFlash.Text = languageProvider.GetLanguage("MainFrm.btnFlash");
		devicelist.Columns[0].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln0");
		devicelist.Columns[1].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln1");
		devicelist.Columns[2].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln2");
		devicelist.Columns[3].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln3");
		devicelist.Columns[4].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln4");
		devicelist.Columns[5].Text = languageProvider.GetLanguage("MainFrm.devicelist.cln5");
		rdoCleanAll.Text = languageProvider.GetLanguage("MainFrm.rdoCleanAll");
		rdoSaveUserData.Text = languageProvider.GetLanguage("MainFrm.rdoSaveUserData");
		rdoCleanAllAndLock.Text = languageProvider.GetLanguage("MainFrm.rdoCleanAllAndLock");
	}

	private void miFlashConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ConfigurationFrm configurationFrm = new ConfigurationFrm();
		configurationFrm.Owner = this;
		configurationFrm.Show();
	}

	private void firmwarewriteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		FirmwareWrite firmwareWrite = new FirmwareWrite();
		firmwareWrite.Owner = this;
		firmwareWrite.Show();
	}

	private void driverTsmItem_Click(object sender, EventArgs e)
	{
		new DriverFrm().Show();
	}

	private void comportToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ComPortConfig comPortConfig = new ComPortConfig();
		comPortConfig.Owner = this;
		comPortConfig.Show();
	}

	private void reDrawProgress()
	{
		Rectangle rectangle = default(Rectangle);
		foreach (ListViewItem item in devicelist.Items)
		{
			string text = item.SubItems[1].Text;
			foreach (Control control in devicelist.Controls)
			{
				if (control.Name.IndexOf("progressbar") >= 0)
				{
					ProgressBar progressBar = (ProgressBar)control;
					if (progressBar.Name == text + "progressbar")
					{
						rectangle = progressBar.Bounds;
						rectangle.Width = devicelist.Columns[2].Width;
						rectangle.Y = item.Bounds.Y;
						progressBar.SetBounds(devicelist.Items[0].SubItems[2].Bounds.X, rectangle.Y, rectangle.Width, rectangle.Height);
					}
				}
			}
		}
	}

	private void checkSha256ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ValidationFrm validationFrm = new ValidationFrm();
		validationFrm.Owner = this;
		validationFrm.Show();
	}

	public void CheckSha256()
	{
		timer_updateStatus.Enabled = true;
		foreach (Device flashDevice in FlashingDevice.flashDeviceList)
		{
			if (!flashDevice.IsDone.HasValue || flashDevice.IsDone.Value)
			{
				flashDevice.StartTime = DateTime.Now;
				flashDevice.Status = "flashing";
				flashDevice.Progress = 0f;
				flashDevice.IsDone = false;
				flashDevice.IsUpdate = true;
				DeviceCtrl deviceCtrl = flashDevice.DeviceCtrl;
				deviceCtrl.deviceName = flashDevice.Name;
				deviceCtrl.swPath = txtPath.Text.Trim();
				deviceCtrl.sha256Path = ValidateSpecifyXml;
				Thread thread = new Thread(deviceCtrl.CheckSha256);
				thread.IsBackground = true;
				thread.Start();
			}
		}
	}

	private void flashLogToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string text = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\";
		if (Directory.Exists(text))
		{
			Process.Start("Explorer.exe", text);
		}
	}

	private void fastbootLogToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (!timer_updateStatus.Enabled)
		{
			timer_updateStatus.Enabled = true;
		}
		deviceArr = UsbDevice.GetDevice();
		foreach (Device flashDevice in FlashingDevice.flashDeviceList)
		{
			flashDevice.IsDone = false;
			flashDevice.IsUpdate = true;
			flashDevice.Status = "grabbing log";
			flashDevice.Progress = 0f;
			flashDevice.StartTime = DateTime.Now;
			Thread thread = new Thread(new ScriptDevice
			{
				deviceName = flashDevice.Name
			}.GrapLog);
			thread.Start();
			thread.IsBackground = true;
			FlashingDevice.UpdateDeviceStatus(flashDevice.Name, null, "start grab log", "grabbing log", isDone: false);
		}
	}

	private void authenticationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		MiUserInfo miUserInfo = EDL_SLA_Challenge.authEDl("sunny", out canFlash);
		lblAccount.Text = miUserInfo.name.Trim();
	}

	private void rdoCleanAll_Click(object sender, EventArgs e)
	{
		if (rdoCleanAll.IsChecked)
		{
			script = FlashType.CleanAll;
			MiAppConfig.SetValue("script", script);
		}
		cmbScriptItem.SetText(script);
		if (Directory.Exists(txtPath.Text))
		{
			SetScriptItems(txtPath.Text);
		}
	}

	private void rdoSaveUserData_Click(object sender, EventArgs e)
	{
		if (rdoSaveUserData.IsChecked)
		{
			if (!Directory.Exists(txtPath.Text))
			{
				return;
			}
			string[] array = FileSearcher.SearchFiles(txtPath.Text, FlashType.SaveUserData);
			if (array.Length == 0)
			{
				MessageBox.Show("couldn't find script.");
				return;
			}
			script = Path.GetFileName(array[0]);
			MiAppConfig.SetValue("script", script);
		}
		cmbScriptItem.SetText(script);
		if (Directory.Exists(txtPath.Text))
		{
			SetScriptItems(txtPath.Text);
		}
	}

	private void rdoCleanAllAndLock_Click(object sender, EventArgs e)
	{
		if (rdoCleanAllAndLock.IsChecked)
		{
			script = FlashType.CleanAllAndLock;
			MiAppConfig.SetValue("script", script);
		}
		cmbScriptItem.SetText(script);
		if (Directory.Exists(txtPath.Text))
		{
			SetScriptItems(txtPath.Text);
		}
	}

	private void cmbScriptItem_TextChanged(object sender, EventArgs e)
	{
		script = cmbScriptItem.SelectedText;
		if (script == FlashType.CleanAll)
		{
			rdoCleanAll.IsChecked = true;
		}
		else if (script.IndexOf("flash_all_except") >= 0)
		{
			rdoSaveUserData.IsChecked = true;
		}
		else if (script == FlashType.CleanAllAndLock)
		{
			rdoCleanAllAndLock.IsChecked = true;
		}
		else
		{
			rdoCleanAll.IsChecked = false;
			rdoSaveUserData.IsChecked = false;
			rdoCleanAllAndLock.IsChecked = false;
		}
		MiAppConfig.SetValue("script", script);
	}

	private void txtPath_TextChanged(object sender, EventArgs e)
	{
	}

	private void StartAutoFlash()
	{
		try
		{
			IPAddress any = IPAddress.Any;
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(any, myPort));
			serverSocket.Listen(10);
			SetLogText("start listen " + serverSocket.LocalEndPoint.ToString() + " successfully");
			serverSocket.BeginAccept(OnConnectRequest, serverSocket);
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}

	private void StartNewAutoFlash()
	{
		try
		{
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(MiAppConfig.Get("hostIpAddress").ToString()), 8901);
			clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			SetLogText("client is start!");
			clientSocket.BeginConnect(remoteEP, DoConnect, clientSocket);
			Thread thread = new Thread(DoReconnect);
			thread.Name = "DoReconnect";
			thread.IsBackground = true;
			thread.Start();
			threads.Add(thread);
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
		}
	}

	private void RecMsg(string recStr)
	{
		string[] array = recStr.Replace("$", "#").Split('#');
		for (int i = 0; i < array.Length; i++)
		{
			recStr = array[i];
			if (string.IsNullOrEmpty(recStr))
			{
				continue;
			}
			if (recStr.IndexOf("READY") >= 0)
			{
				SetLogText("start load devices");
				btnRefresh.BeginInvoke((Action<string>)delegate
				{
					btnRefresh_Click(btnRefresh, new EventArgs());
				}, "");
			}
			else if (recStr.IndexOf("START") >= 0)
			{
				SetLogText("start load devices");
				deviceArr = UsbDevice.GetDevice();
				btnRefresh.BeginInvoke((Action<string>)delegate
				{
					btnRefresh_Click(btnRefresh, new EventArgs());
				}, "");
				string text = recStr.Replace("START", "");
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				SetLogText("check usb " + text);
				int num = 0;
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						num = int.Parse(text);
					}
					catch (Exception ex)
					{
						SetLogText(ex.Message);
					}
				}
				bool flag = false;
				foreach (Device item in deviceArr)
				{
					if (item.Index == num)
					{
						flag = true;
						break;
					}
				}
				string text2 = "#USB" + num;
				text2 = ((!flag) ? (text2 + "OFF$") : (text2 + "ON$"));
				SetLogText("send :" + text2);
				m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text2));
			}
			else if (recStr.IndexOf("LOAD") >= 0)
			{
				SetLogText("start flash");
				btnFlash.BeginInvoke((Action<string>)delegate
				{
					btnFlash_Click(btnFlash, new EventArgs());
				}, "");
			}
		}
	}

	private void DoRecMsg(object message)
	{
		string text = (string)message;
		if (text.IndexOf("Ready") >= 0)
		{
			Log.w("recStr:" + text);
			CheckIsFactory();
			SetLogText("start load devices");
			int startIndex = text.IndexOf("Ready");
			text = text.Substring(startIndex);
			string text2 = text.Replace(";", ",").Split(',')[1];
			if (string.IsNullOrEmpty(text2))
			{
				return;
			}
			SetLogText("check usb " + text2);
			int index = 0;
			if (!string.IsNullOrEmpty(text2))
			{
				try
				{
					index = int.Parse(text2);
				}
				catch (Exception ex)
				{
					SetLogText(ex.Message);
				}
			}
			bool flag = false;
			UsbNodeConnectionInformationForPDL info = UsbDevice.GetFastbootDeviceByIndex(text2);
			if (!string.IsNullOrEmpty(info.SerialNumber))
			{
				flag = true;
			}
			string text3 = "Ready," + index;
			text3 = ((!flag) ? (text3 + ",Fail;") : (text3 + ",Pass;"));
			SetLogText("send :" + text3);
			m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text3));
			if (flag)
			{
				BeginInvoke((Action<string>)delegate
				{
					FreshDevice(index, info.SerialNumber);
				}, "");
				SetLogText("start flash " + text2);
				btnFlash.BeginInvoke((Action<string>)delegate
				{
					btnFlash_Click(btnFlash, new EventArgs());
				}, "");
			}
		}
		else if (text.IndexOf("Stop") >= 0)
		{
			Log.w("recStr:" + text);
			FlashingDevice.UpdateDeviceAllStop("flash Stop", "error");
			string s = "Stop,Ok;";
			m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(s));
		}
	}

	public string GetTimeStamp()
	{
		return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
	}

	private void DoLoadPosDevice()
	{
		try
		{
			string text = "";
			string text2 = "";
			bool flag = false;
			string text3 = "0";
			Log.w("Thread DoLoadPosDevice begin========");
			while (true)
			{
				Thread.Sleep(2000);
				flag = false;
				text3 = GetTimeStamp();
				lock (poslocker)
				{
					if (posList.Count() >= maxnum || (int.Parse(text3) - int.Parse(currentTime) >= maxwaittime && posList.Count() > 0))
					{
						Log.w($"*** posList.Count{posList.Count()}");
						SetLogText("start load devices");
						deviceArr = UsbDevice.GetFastbootDevice();
						foreach (PdlPos pos in posList)
						{
							text = "Ready," + pos.Posindex;
							text2 = text + ",Fail;";
							foreach (Device item in deviceArr)
							{
								if (item.Index == pos.Posindex)
								{
									Log.w($"*** FreshDevice{pos.Posindex}");
									flag = true;
									text2 = text + ",Pass;";
								}
							}
							SetLogText("send :" + text2);
							m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text2));
						}
						if (flag)
						{
							BeginInvoke((Action<string>)delegate
							{
								FreshDevice(0);
							}, "");
							btnFlash.BeginInvoke((Action<string>)delegate
							{
								btnFlash_Click(btnFlash, new EventArgs());
							}, "");
						}
						iswork = false;
						isbakwork = true;
						posList.Clear();
						Log.w("*** posList.Clear");
					}
					if (posListbak.Count() < maxnum && (int.Parse(text3) - int.Parse(currentTime) < maxwaittime || posListbak.Count() <= 0))
					{
						continue;
					}
					Log.w($"*** posListbak.Count{posListbak.Count()}");
					SetLogText("start load devices");
					deviceArr = UsbDevice.GetFastbootDevice();
					foreach (PdlPos item2 in posListbak)
					{
						text = "Ready," + item2.Posindex;
						text2 = text + ",Fail;";
						foreach (Device item3 in deviceArr)
						{
							if (item3.Index == item2.Posindex)
							{
								Log.w($"*** FreshDevice{item2.Posindex}");
								flag = true;
								text2 = text + ",Pass;";
							}
						}
						SetLogText("send :" + text2);
						m_aryClients[0].Sock.Send(Encoding.ASCII.GetBytes(text2));
					}
					if (flag)
					{
						BeginInvoke((Action<string>)delegate
						{
							FreshDevice(0);
						}, "");
						btnFlash.BeginInvoke((Action<string>)delegate
						{
							btnFlash_Click(btnFlash, new EventArgs());
						}, "");
					}
					posListbak.Clear();
					iswork = true;
					isbakwork = false;
					Log.w("*** posList.Clear");
				}
			}
		}
		catch (Exception ex)
		{
			SetLogText("DoReconnect error exit");
			Log.w("DoReconnect error exit");
			Log.w(ex.Message + "\r\n" + ex.StackTrace);
		}
	}

	public void FreshDevice(int index, string deviceid)
	{
		try
		{
			btnRefresh.Enabled = false;
			btnFlash.Enabled = false;
			btnRefresh.Cursor = Cursors.WaitCursor;
			btnFlash.Cursor = Cursors.WaitCursor;
			new List<Thread>(threads);
			IEnumerable<string> dr = GetFlashDoneDevice();
			DrInitWithLock(ref dr);
			dr = from fd in FlashingDevice.flashDeviceList
				where fd.Name == deviceid
				select fd.Name;
			if (dr.Count() == 0)
			{
				if (deviceid.Contains("?"))
				{
					Log.w(deviceid + "device is err Because it contains '?'");
				}
				else
				{
					Log.w("add device " + deviceid + " index " + index);
					lock (flashingdevicelocker)
					{
						FlashingDevice.flashDeviceList.Add(new Device
						{
							Index = index,
							Name = deviceid,
							Progress = 0f,
							IsDone = null,
							IsUpdate = false,
							DeviceCtrl = new ScriptDevice()
						});
					}
					float num = 0f;
					DrawCln(index, deviceid, num);
				}
			}
			else
			{
				Log.w("Dulicate device " + deviceid);
			}
			reDrawProgress();
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			Log.w(ex.StackTrace);
			MessageBox.Show(ex.Message);
		}
		finally
		{
			btnRefresh.Enabled = true;
			btnRefresh.Cursor = Cursors.Default;
			btnFlash.Enabled = true;
			btnFlash.Cursor = Cursors.Default;
		}
	}

	public void FreshDevice(int index)
	{
		try
		{
			btnRefresh.Enabled = false;
			btnFlash.Enabled = false;
			btnRefresh.Cursor = Cursors.WaitCursor;
			btnFlash.Cursor = Cursors.WaitCursor;
			new List<Thread>(threads);
			IEnumerable<string> dr = GetFlashDoneDevice();
			DrInitWithLock(ref dr);
			foreach (string deviceid in UsbDevice.GetScriptDevice().ToList())
			{
				dr = from fd in FlashingDevice.flashDeviceList
					where fd.Name == deviceid
					select fd.Name;
				if (dr.Count() == 0)
				{
					if (deviceid.Contains("?"))
					{
						Log.w(deviceid + "device is err Because it contains '?'");
						continue;
					}
					Log.w("add device " + deviceid + " index " + index);
					lock (flashingdevicelocker)
					{
						FlashingDevice.flashDeviceList.Add(new Device
						{
							Index = index,
							Name = deviceid,
							Progress = 0f,
							IsDone = null,
							IsUpdate = false,
							DeviceCtrl = new ScriptDevice()
						});
					}
					float num = 0f;
					DrawCln(index, deviceid, num);
				}
				else
				{
					Log.w("Dulicate device " + deviceid);
				}
			}
			reDrawProgress();
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			Log.w(ex.StackTrace);
			MessageBox.Show(ex.Message);
		}
		finally
		{
			btnRefresh.Enabled = true;
			btnRefresh.Cursor = Cursors.Default;
			btnFlash.Enabled = true;
			btnFlash.Cursor = Cursors.Default;
		}
	}

	private void DrawClnForAuto(object a)
	{
		Device device = (Device)a;
		ListViewItem listViewItem = new ListViewItem(new string[6]
		{
			device.Index.ToString(),
			device.Name,
			"",
			"0s",
			"",
			""
		});
		devicelist.Items.Add(listViewItem);
		Rectangle rectangle = default(Rectangle);
		ProgressBar progressBar = new ProgressBar();
		rectangle = listViewItem.SubItems[2].Bounds;
		rectangle.Width = devicelist.Columns[2].Width;
		progressBar.Parent = devicelist;
		progressBar.SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		progressBar.Value = 0;
		progressBar.Visible = true;
		progressBar.Name = device.Name + "progressbar";
	}

	private void MainFrm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.A)
		{
			devicelist.Height = 270;
			txtLog.Visible = true;
			if (serverSocket == null)
			{
				isAutoFlash = true;
				StartAutoFlash();
			}
		}
		else if (e.KeyCode == Keys.C)
		{
			devicelist.Height = 270;
			txtLog.Visible = true;
			if (serverSocket == null)
			{
				isAutoFlash = true;
				StartNewAutoFlash();
			}
		}
		else if (e.KeyCode == Keys.R && e.Alt)
		{
			e.Handled = true;
			btnRefresh.PerformClick();
		}
		else if (e.KeyCode == Keys.F && e.Alt)
		{
			e.Handled = true;
			btnFlash.PerformClick();
		}
	}

	public void OnConnectRequest(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			NewConnection(socket.EndAccept(ar));
			socket.BeginAccept(OnConnectRequest, socket);
		}
		catch (Exception)
		{
		}
	}

	public void DoConnect(IAsyncResult ar)
	{
		try
		{
			Socket obj = (Socket)ar.AsyncState;
			obj.EndConnect(ar);
			SocketChatClient socketChatClient = new SocketChatClient(obj);
			m_aryClients.Add(socketChatClient);
			string text = "Connect Ok;";
			byte[] bytes = Encoding.ASCII.GetBytes(text.ToCharArray());
			socketChatClient.Sock.Send(bytes, bytes.Length, SocketFlags.None);
			string text2 = "Timeout,700;";
			bytes = Encoding.ASCII.GetBytes(text2.ToCharArray());
			socketChatClient.Sock.Send(bytes, bytes.Length, SocketFlags.None);
			Thread thread = new Thread(DoHeartBeat);
			thread.Name = "DoHeartBeat";
			thread.IsBackground = true;
			thread.Start();
			threads.Add(thread);
			isAutoPDL = true;
			socketChatClient.SetupRecieveCallback(this);
		}
		catch (Exception)
		{
		}
	}

	public bool Reconnect()
	{
		m_aryClients[0].Sock.Shutdown(SocketShutdown.Both);
		m_aryClients[0].Sock.Disconnect(reuseSocket: true);
		m_aryClients[0].Sock.Close();
		StartNewAutoFlash();
		return true;
	}

	private bool IsSocketConnected(Socket s)
	{
		if (!s.Poll(1000, SelectMode.SelectRead) || s.Available != 0)
		{
			return s.Connected;
		}
		return false;
	}

	private void DoReconnect()
	{
		try
		{
			while (true)
			{
				Thread.Sleep(10000);
				if (m_aryClients.Count() == 0)
				{
					SetLogText("beging reconncet");
					IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(MiAppConfig.Get("hostIpAddress").ToString()), 8901);
					clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					SetLogText("a client is start!");
					clientSocket.BeginConnect(remoteEP, DoConnect, clientSocket);
				}
				else if (m_aryClients[0].Sock == null)
				{
					IPEndPoint remoteEP2 = new IPEndPoint(IPAddress.Parse(MiAppConfig.Get("hostIpAddress").ToString()), 8901);
					clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					SetLogText("b client is start!");
					clientSocket.BeginConnect(remoteEP2, DoConnect, clientSocket);
				}
				else if (!IsSocketConnected(m_aryClients[0].Sock))
				{
					m_aryClients[0].Sock.Shutdown(SocketShutdown.Both);
					m_aryClients[0].Sock.Disconnect(reuseSocket: true);
					m_aryClients[0].Sock.Close();
					m_aryClients.RemoveAt(0);
					IPEndPoint remoteEP3 = new IPEndPoint(IPAddress.Parse(MiAppConfig.Get("hostIpAddress").ToString()), 8901);
					clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					SetLogText("c client is start!");
					clientSocket.BeginConnect(remoteEP3, DoConnect, clientSocket);
				}
			}
		}
		catch (Exception ex)
		{
			SetLogText("DoReconnect error exit");
			Log.w("DoReconnect error exit");
			Log.w(ex.Message + "\r\n" + ex.StackTrace);
		}
	}

	private void DoHeartBeat()
	{
		try
		{
			SetLogText("beging DoHeartBeat");
			while (true)
			{
				Thread.Sleep(10000);
				if (m_aryClients[0].Sock != null)
				{
					string text = "HeartBeat;";
					byte[] bytes = Encoding.ASCII.GetBytes(text.ToCharArray());
					m_aryClients[0].Sock.Send(bytes, bytes.Length, SocketFlags.None);
				}
			}
		}
		catch (Exception ex)
		{
			SetLogText("DoHeartBeat error exit");
			Log.w("DoHeartBeat error exit");
			Log.w(ex.Message + "\r\n" + ex.StackTrace);
		}
	}

	public void NewConnection(Socket sockClient)
	{
		SocketChatClient socketChatClient = new SocketChatClient(sockClient);
		m_aryClients.Add(socketChatClient);
		SetLogText($"Client {socketChatClient.Sock.RemoteEndPoint}, joined");
		string text = "Welcome " + DateTime.Now.ToString("G") + "\n\r";
		byte[] bytes = Encoding.ASCII.GetBytes(text.ToCharArray());
		socketChatClient.Sock.Send(bytes, bytes.Length, SocketFlags.None);
		socketChatClient.SetupRecieveCallback(this);
	}

	public void OnRecievedData(IAsyncResult ar)
	{
		SocketChatClient socketChatClient = (SocketChatClient)ar.AsyncState;
		byte[] recievedData = socketChatClient.GetRecievedData(ar);
		if (recievedData.Length < 1)
		{
			SetLogText($"Client {socketChatClient.Sock.RemoteEndPoint}, disconnected");
			socketChatClient.Sock.Shutdown(SocketShutdown.Both);
			socketChatClient.Sock.Disconnect(reuseSocket: true);
			socketChatClient.Sock.Close();
			m_aryClients.Remove(socketChatClient);
		}
		else
		{
			string @string = Encoding.ASCII.GetString(recievedData);
			if (isAutoPDL)
			{
				DoRecMsg(@string);
			}
			else
			{
				RecMsg(@string);
			}
			socketChatClient.SetupRecieveCallback(this);
		}
	}

	private void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upgrader.exe"));
		System.Environment.Exit(0);
	}

	public static DataTable GetExcelTableByOleDB(string strExcelPath, string tableName)
	{
		try
		{
			new DataTable();
			DataSet dataSet = new DataSet();
			string extension = Path.GetExtension(strExcelPath);
			Path.GetFileName(strExcelPath);
			OleDbConnection oleDbConnection = null;
			oleDbConnection = ((extension == ".xls") ? new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strExcelPath + ";Extended Properties=\"Excel 8.0;HDR=yes;IMEX=1;\"") : ((!(extension == ".xlsx")) ? null : new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strExcelPath + ";Extended Properties=\"Excel 12.0;HDR=yes;IMEX=1;\"")));
			if (oleDbConnection == null)
			{
				return null;
			}
			oleDbConnection.Open();
			string obj = "select * from [" + tableName + "$]";
			new OleDbCommand(obj, oleDbConnection);
			new OleDbDataAdapter(obj, oleDbConnection).Fill(dataSet, tableName);
			oleDbConnection.Close();
			return dataSet.Tables[tableName];
		}
		catch (Exception ex)
		{
			Log.w(ex.Message + "\r\n" + ex.StackTrace);
			return null;
		}
	}

	public static bool IsIncludeData(DataTable dt, string columnName, string fieldData)
	{
		if (dt == null)
		{
			return false;
		}
		DataRow[] array = dt.Select(columnName + "='" + fieldData + "'");
		if (array.Length != 0)
		{
			string text = "";
			DataRow[] array2 = array;
			foreach (DataRow dataRow in array2)
			{
				text += dataRow["版本"];
				Log.w("vboytest  version：" + text);
			}
			return true;
		}
		return false;
	}

	public static DataTable OpenCSV(string filePath)
	{
		Encoding type = GetType(filePath);
		DataTable dataTable = new DataTable();
		FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, type);
		string text = "";
		string[] array = null;
		string[] array2 = null;
		int num = 0;
		bool flag = true;
		while ((text = streamReader.ReadLine()) != null)
		{
			if (flag)
			{
				array2 = text.Split(',');
				flag = false;
				num = array2.Length;
				for (int i = 0; i < num; i++)
				{
					DataColumn column = new DataColumn(array2[i]);
					dataTable.Columns.Add(column);
				}
			}
			else
			{
				array = text.Split(',');
				DataRow dataRow = dataTable.NewRow();
				for (int j = 0; j < num; j++)
				{
					dataRow[j] = array[j];
				}
				dataTable.Rows.Add(dataRow);
			}
		}
		if (array != null && array.Length != 0)
		{
			dataTable.DefaultView.Sort = array2[0] + " asc";
		}
		streamReader.Close();
		fileStream.Close();
		return dataTable;
	}

	public static Encoding GetType(string FILE_NAME)
	{
		FileStream fileStream = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
		Encoding type = GetType(fileStream);
		fileStream.Close();
		return type;
	}

	public static Encoding GetType(FileStream fs)
	{
		_ = new byte[3] { 255, 254, 65 };
		_ = new byte[3] { 254, 255, 0 };
		_ = new byte[3] { 239, 187, 191 };
		Encoding encoding = Encoding.Default;
		BinaryReader binaryReader = new BinaryReader(fs, Encoding.Default);
		int.TryParse(fs.Length.ToString(), out var count);
		byte[] array = binaryReader.ReadBytes(count);
		if (IsUTF8Bytes(array) || (array[0] == 239 && array[1] == 187 && array[2] == 191))
		{
			encoding = Encoding.UTF8;
		}
		else if (array[0] == 254 && array[1] == byte.MaxValue && array[2] == 0)
		{
			encoding = Encoding.BigEndianUnicode;
		}
		else if (array[0] == byte.MaxValue && array[1] == 254 && array[2] == 65)
		{
			encoding = Encoding.Unicode;
		}
		binaryReader.Close();
		return encoding;
	}

	private static bool IsUTF8Bytes(byte[] data)
	{
		int num = 1;
		for (int i = 0; i < data.Length; i++)
		{
			byte b = data[i];
			if (num == 1)
			{
				if (b >= 128)
				{
					while (((b <<= 1) & 0x80u) != 0)
					{
						num++;
					}
					if (num == 1 || num > 6)
					{
						return false;
					}
				}
			}
			else
			{
				if ((b & 0xC0) != 128)
				{
					return false;
				}
				num--;
			}
		}
		if (num > 1)
		{
			throw new Exception("非预期的byte格式");
		}
		return true;
	}

	private void SetChip_Qualcomm()
	{
		pnlQcom.Visible = true;
		devicelist.Location = new Point(21, 113);
		MiAppConfig.SetValue("chip", "Qualcomm");
	}

	private void SetChip_MTK()
	{
		pnlQcom.Visible = false;
		devicelist.Location = new Point(21, 154);
		if (ComportList.Count == 0)
		{
			string text = MiAppConfig.Get("mtkComs");
			if (!string.IsNullOrEmpty(text))
			{
				text.Split(',');
			}
		}
	}

	private void SetChip_JLQ()
	{
		pnlQcom.Visible = false;
		devicelist.Location = new Point(21, 113);
		MiAppConfig.SetValue("chip", "JLQ");
	}

	private IEnumerable<string> GetFlashDoneDevice()
	{
		return from d in FlashingDevice.flashDeviceList
			where !d.IsDone.HasValue || (d.IsDone.HasValue && d.IsDone.Value && !d.IsUpdate)
			select d.Name;
	}

	private void DrInit(ref IEnumerable<string> dr)
	{
		foreach (string item in dr.ToList())
		{
			foreach (Device flashDevice in FlashingDevice.flashDeviceList)
			{
				if (flashDevice.Name == item.ToString())
				{
					Log.w("FlashingDevice.flashDeviceList.Remove " + flashDevice.Name);
					flashDevice.StatusList.Clear();
					FlashingDevice.flashDeviceList.Remove(flashDevice);
					break;
				}
			}
			ListView.ListViewItemCollection items = devicelist.Items;
			foreach (ListViewItem item2 in items)
			{
				if (item2.SubItems[1].Text == item.ToString())
				{
					items.Remove(item2);
					break;
				}
			}
			foreach (Control control in devicelist.Controls)
			{
				if (control.Name == item.ToString() + "progressbar")
				{
					devicelist.Controls.Remove(control);
					break;
				}
			}
		}
	}

	private void DrInitWithLock(ref IEnumerable<string> dr)
	{
		foreach (string item in dr.ToList())
		{
			foreach (Device flashDevice in FlashingDevice.flashDeviceList)
			{
				if (flashDevice.Name == item.ToString())
				{
					Log.w("FlashingDevice.flashDeviceList.Remove " + flashDevice.Name);
					flashDevice.StatusList.Clear();
					lock (flashingdevicelocker)
					{
						FlashingDevice.flashDeviceList.Remove(flashDevice);
					}
					break;
				}
			}
			ListView.ListViewItemCollection items = devicelist.Items;
			foreach (ListViewItem item2 in items)
			{
				if (item2.SubItems[1].Text == item.ToString())
				{
					items.Remove(item2);
					break;
				}
			}
			foreach (Control control in devicelist.Controls)
			{
				if (control.Name == item.ToString() + "progressbar")
				{
					devicelist.Controls.Remove(control);
					break;
				}
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnBrwDic = new System.Windows.Forms.Button();
            this.fbdSelect = new WP_Tool_Pro_DXApplication.Gui.VistaFolderBrowserDialog();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnFlash = new System.Windows.Forms.Button();
            this.timer_updateStatus = new System.Windows.Forms.Timer(this.components);
            this.statusStrp = new System.Windows.Forms.StatusStrip();
            this.lblAccount = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusTab = new System.Windows.Forms.ToolStripStatusLabel();
            this.rdoCleanAll = new XiaoMiFlash.code.MiControl.RadioStripItem();
            this.rdoSaveUserData = new XiaoMiFlash.code.MiControl.RadioStripItem();
            this.rdoCleanAllAndLock = new XiaoMiFlash.code.MiControl.RadioStripItem();
            this.cmbScriptItem = new XiaoMiFlash.code.MiControl.ComboBoxStripItem();
            this.lblMD5 = new System.Windows.Forms.Label();
            this.mnsAuth = new System.Windows.Forms.MenuStrip();
            this.miConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.miFlashConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.driverTsmItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSha256ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flashLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fastbootLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlQcom = new System.Windows.Forms.Panel();
            this.btnAutoFlash = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.devicelist = new System.Windows.Forms.ListView();
            this.clnID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnDevice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnResult = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrp.SuspendLayout();
            this.mnsAuth.SuspendLayout();
            this.pnlQcom.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(86, 25);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(776, 20);
            this.txtPath.TabIndex = 0;
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // btnBrwDic
            // 
            this.btnBrwDic.Location = new System.Drawing.Point(5, 23);
            this.btnBrwDic.Name = "btnBrwDic";
            this.btnBrwDic.Size = new System.Drawing.Size(75, 25);
            this.btnBrwDic.TabIndex = 1;
            this.btnBrwDic.Text = "select";
            this.btnBrwDic.UseVisualStyleBackColor = true;
            this.btnBrwDic.Click += new System.EventHandler(this.btnBrwDic_Click);
            // 
            // fbdSelect
            // 
            this.fbdSelect.Description = "Please select sw path";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(868, 23);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 25);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnFlash
            // 
            this.btnFlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlash.Location = new System.Drawing.Point(980, 20);
            this.btnFlash.Name = "btnFlash";
            this.btnFlash.Size = new System.Drawing.Size(84, 25);
            this.btnFlash.TabIndex = 3;
            this.btnFlash.Text = "刷机(F)";
            this.btnFlash.UseVisualStyleBackColor = true;
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
            // 
            // timer_updateStatus
            // 
            this.timer_updateStatus.Interval = 800;
            this.timer_updateStatus.Tick += new System.EventHandler(this.timer_updateStatus_Tick);
            // 
            // statusStrp
            // 
            this.statusStrp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblAccount,
            this.statusTab,
            this.rdoCleanAll,
            this.rdoSaveUserData,
            this.rdoCleanAllAndLock,
            this.cmbScriptItem});
            this.statusStrp.Location = new System.Drawing.Point(0, 573);
            this.statusStrp.Name = "statusStrp";
            this.statusStrp.Size = new System.Drawing.Size(1008, 30);
            this.statusStrp.TabIndex = 7;
            this.statusStrp.Text = "statusStrip1";
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = false;
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(100, 25);
            // 
            // statusTab
            // 
            this.statusTab.AutoSize = false;
            this.statusTab.Name = "statusTab";
            this.statusTab.Size = new System.Drawing.Size(410, 25);
            this.statusTab.Spring = true;
            // 
            // rdoCleanAll
            // 
            this.rdoCleanAll.IsChecked = false;
            this.rdoCleanAll.Name = "rdoCleanAll";
            this.rdoCleanAll.Size = new System.Drawing.Size(68, 28);
            this.rdoCleanAll.Text = "clean all";
            this.rdoCleanAll.Click += new System.EventHandler(this.rdoCleanAll_Click);
            // 
            // rdoSaveUserData
            // 
            this.rdoSaveUserData.IsChecked = false;
            this.rdoSaveUserData.Name = "rdoSaveUserData";
            this.rdoSaveUserData.Size = new System.Drawing.Size(99, 28);
            this.rdoSaveUserData.Text = "save user data";
            this.rdoSaveUserData.Click += new System.EventHandler(this.rdoSaveUserData_Click);
            // 
            // rdoCleanAllAndLock
            // 
            this.rdoCleanAllAndLock.IsChecked = true;
            this.rdoCleanAllAndLock.Name = "rdoCleanAllAndLock";
            this.rdoCleanAllAndLock.Size = new System.Drawing.Size(116, 28);
            this.rdoCleanAllAndLock.Text = "clean all and lock";
            this.rdoCleanAllAndLock.Click += new System.EventHandler(this.rdoCleanAllAndLock_Click);
            // 
            // cmbScriptItem
            // 
            this.cmbScriptItem.Name = "cmbScriptItem";
            this.cmbScriptItem.Size = new System.Drawing.Size(200, 28);
            this.cmbScriptItem.TextChanged += new System.EventHandler(this.cmbScriptItem_TextChanged);
            // 
            // lblMD5
            // 
            this.lblMD5.AutoSize = true;
            this.lblMD5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMD5.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMD5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblMD5.Location = new System.Drawing.Point(84, 59);
            this.lblMD5.Name = "lblMD5";
            this.lblMD5.Size = new System.Drawing.Size(2, 14);
            this.lblMD5.TabIndex = 8;
            // 
            // mnsAuth
            // 
            this.mnsAuth.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mnsAuth.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.mnsAuth.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miConfiguration,
            this.driverTsmItem,
            this.otherToolStripMenuItem,
            this.logToolStripMenuItem});
            this.mnsAuth.Location = new System.Drawing.Point(0, 0);
            this.mnsAuth.Name = "mnsAuth";
            this.mnsAuth.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnsAuth.Size = new System.Drawing.Size(1008, 24);
            this.mnsAuth.TabIndex = 9;
            this.mnsAuth.Text = "Authentication";
            // 
            // miConfiguration
            // 
            this.miConfiguration.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFlashConfigurationToolStripMenuItem});
            this.miConfiguration.Name = "miConfiguration";
            this.miConfiguration.Size = new System.Drawing.Size(93, 20);
            this.miConfiguration.Text = "Configuration";
            // 
            // miFlashConfigurationToolStripMenuItem
            // 
            this.miFlashConfigurationToolStripMenuItem.Name = "miFlashConfigurationToolStripMenuItem";
            this.miFlashConfigurationToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.miFlashConfigurationToolStripMenuItem.Text = "MiFlash Configuration";
            this.miFlashConfigurationToolStripMenuItem.Click += new System.EventHandler(this.miFlashConfigurationToolStripMenuItem_Click);
            // 
            // driverTsmItem
            // 
            this.driverTsmItem.Name = "driverTsmItem";
            this.driverTsmItem.Size = new System.Drawing.Size(50, 20);
            this.driverTsmItem.Text = "Driver";
            this.driverTsmItem.Click += new System.EventHandler(this.driverTsmItem_Click);
            // 
            // otherToolStripMenuItem
            // 
            this.otherToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkSha256ToolStripMenuItem});
            this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
            this.otherToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.otherToolStripMenuItem.Text = "Other";
            // 
            // checkSha256ToolStripMenuItem
            // 
            this.checkSha256ToolStripMenuItem.Name = "checkSha256ToolStripMenuItem";
            this.checkSha256ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.checkSha256ToolStripMenuItem.Text = "Check Sha256";
            this.checkSha256ToolStripMenuItem.Click += new System.EventHandler(this.checkSha256ToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flashLogToolStripMenuItem,
            this.fastbootLogToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.logToolStripMenuItem.Text = "Log";
            // 
            // flashLogToolStripMenuItem
            // 
            this.flashLogToolStripMenuItem.Name = "flashLogToolStripMenuItem";
            this.flashLogToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.flashLogToolStripMenuItem.Text = "Flash log";
            this.flashLogToolStripMenuItem.Click += new System.EventHandler(this.flashLogToolStripMenuItem_Click);
            // 
            // fastbootLogToolStripMenuItem
            // 
            this.fastbootLogToolStripMenuItem.Name = "fastbootLogToolStripMenuItem";
            this.fastbootLogToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.fastbootLogToolStripMenuItem.Text = "Fastboot log";
            this.fastbootLogToolStripMenuItem.Visible = false;
            this.fastbootLogToolStripMenuItem.Click += new System.EventHandler(this.fastbootLogToolStripMenuItem_Click);
            // 
            // pnlQcom
            // 
            this.pnlQcom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlQcom.Controls.Add(this.btnAutoFlash);
            this.pnlQcom.Controls.Add(this.txtPath);
            this.pnlQcom.Controls.Add(this.lblMD5);
            this.pnlQcom.Controls.Add(this.btnBrwDic);
            this.pnlQcom.Controls.Add(this.btnRefresh);
            this.pnlQcom.Controls.Add(this.btnFlash);
            this.pnlQcom.Location = new System.Drawing.Point(18, 30);
            this.pnlQcom.Name = "pnlQcom";
            this.pnlQcom.Size = new System.Drawing.Size(969, 108);
            this.pnlQcom.TabIndex = 10;
            // 
            // btnAutoFlash
            // 
            this.btnAutoFlash.Location = new System.Drawing.Point(92, 52);
            this.btnAutoFlash.Name = "btnAutoFlash";
            this.btnAutoFlash.Size = new System.Drawing.Size(75, 25);
            this.btnAutoFlash.TabIndex = 9;
            this.btnAutoFlash.Text = "start flash";
            this.btnAutoFlash.UseVisualStyleBackColor = true;
            this.btnAutoFlash.Click += new System.EventHandler(this.btnAutoFlash_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(18, 496);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(969, 34);
            this.txtLog.TabIndex = 6;
            this.txtLog.Text = "";
            this.txtLog.Visible = false;
            // 
            // devicelist
            // 
            this.devicelist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.devicelist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnID,
            this.clnDevice,
            this.clnProgress,
            this.clnTime,
            this.clnStatus,
            this.clnResult});
            this.devicelist.GridLines = true;
            this.devicelist.HideSelection = false;
            this.devicelist.Location = new System.Drawing.Point(20, 144);
            this.devicelist.Name = "devicelist";
            this.devicelist.Size = new System.Drawing.Size(969, 426);
            this.devicelist.TabIndex = 11;
            this.devicelist.UseCompatibleStateImageBehavior = false;
            this.devicelist.View = System.Windows.Forms.View.Details;
            // 
            // clnID
            // 
            this.clnID.Text = "id";
            // 
            // clnDevice
            // 
            this.clnDevice.Text = "device";
            this.clnDevice.Width = 90;
            // 
            // clnProgress
            // 
            this.clnProgress.Text = "progress";
            this.clnProgress.Width = 107;
            // 
            // clnTime
            // 
            this.clnTime.Text = "elapsed";
            // 
            // clnStatus
            // 
            this.clnStatus.Text = "status";
            this.clnStatus.Width = 500;
            // 
            // clnResult
            // 
            this.clnResult.Text = "result";
            this.clnResult.Width = 126;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 603);
            this.Controls.Add(this.devicelist);
            this.Controls.Add(this.pnlQcom);
            this.Controls.Add(this.statusStrp);
            this.Controls.Add(this.mnsAuth);
            this.Controls.Add(this.txtLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnsAuth;
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MiFlash - Qualcomm No Auth | iReverse Qualcomm Native C# Re-work";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFrm_FormClosed);
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFrm_KeyDown);
            this.statusStrp.ResumeLayout(false);
            this.statusStrp.PerformLayout();
            this.mnsAuth.ResumeLayout(false);
            this.mnsAuth.PerformLayout();
            this.pnlQcom.ResumeLayout(false);
            this.pnlQcom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	private void 打开Log文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start(Application.StartupPath + "\\Log");
	}
}
