using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XiaoMiFlash.code.bl;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.form;

public class ConfigurationFrm : Form
{
	private MainFrm mainFrm;

	private IContainer components;

	private CheckBox chkMD5;

	private CheckBox chkRbv;

	private CheckBox chkWriteDump;

	private CheckBox chkReadDump;

	private CheckBox chkVerbose;

	private CheckBox chkEraseAll;

	private Button btnOK;

	private CheckBox chkAutoDetect;

	private Label label1;

	private ComboBox cmbFactory;

	private Label label3;

	private ComboBox cmbMainProgram;

	private Button btnPatchSelect;

	private Button btnRawSelect;

	private TextBox txtPatch;

	private Label label4;

	private TextBox txtRaw;

	private Label label5;

	private OpenFileDialog openFileDialog;

	private GroupBox groupBox1;

	private Label label6;

	private TextBox txtCheckPoint;

	private CheckBox chkBackupOnly;

	public ConfigurationFrm()
	{
		InitializeComponent();
	}

	private void chkMD5_CheckedChanged(object sender, EventArgs e)
	{
		MiAppConfig.SetValue("checkMD5", chkMD5.Checked.ToString());
	}

	private void ConfigurationFrm_Load(object sender, EventArgs e)
	{
		chkMD5.Checked = MiAppConfig.GetAppConfig("checkMD5").ToLower() == "true";
		mainFrm = (MainFrm)base.Owner;
		chkRbv.Checked = mainFrm.ReadBackVerify;
		chkWriteDump.Checked = mainFrm.OpenWriteDump;
		chkReadDump.Checked = mainFrm.OpenReadDump;
		chkVerbose.Checked = mainFrm.Verbose;
		chkEraseAll.Checked = mainFrm.EraseAll;
		chkAutoDetect.Checked = mainFrm.AutoDetectDevice;
		cmbFactory.SelectedItem = MiAppConfig.Get("factory").ToString();
		cmbMainProgram.SelectedItem = MiAppConfig.Get("mainProgram").ToString();
		openFileDialog.InitialDirectory = mainFrm.SwPath;
		txtRaw.Text = MiAppConfig.Get("rawprogram").ToString();
		txtPatch.Text = MiAppConfig.Get("patch").ToString();
		txtCheckPoint.Text = MiAppConfig.Get("checkPoint").ToString();
	}

	private void chkRbv_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.ReadBackVerify = chkRbv.Checked;
		if (mainFrm.ReadBackVerify)
		{
			chkReadDump.Checked = true;
		}
	}

	private void chkWriteDump_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.OpenWriteDump = chkWriteDump.Checked;
	}

	private void chkReadDump_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.OpenReadDump = chkReadDump.Checked;
	}

	private void chkVerbose_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.Verbose = chkVerbose.Checked;
	}

	private void chkEraseAll_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.EraseAll = chkEraseAll.Checked;
		mainFrm.SetEraseAll(chkEraseAll.Checked);
	}

	private void chkBackupOnly_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.SetBackupOnly(chkBackupOnly.Checked);
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		MiAppConfig.SetValue("rawprogram", txtRaw.Text.Trim());
		MiAppConfig.SetValue("patch", txtPatch.Text.Trim());
		MiAppConfig.SetValue("checkPoint", txtCheckPoint.Text.Trim());
		Close();
		Dispose();
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.AutoDetectDevice = chkAutoDetect.Checked;
		mainFrm.AutoDetectUsb();
		Log.w("set AutoDetectDevice " + chkAutoDetect.Checked);
	}

	private void cmbFactory_SelectedValueChanged(object sender, EventArgs e)
	{
		string text = cmbFactory.SelectedItem.ToString();
		if (text != "please choose")
		{
			if (FactoryCtrl.SetFactory(text))
			{
				MiAppConfig.SetValue("factory", text);
				mainFrm.factory = text;
				mainFrm.SetFactory(text);
			}
			else
			{
				MessageBox.Show("set factory failed!");
			}
		}
		else
		{
			MiAppConfig.SetValue("factory", "");
			mainFrm.factory = "";
			mainFrm.SetFactory("");
		}
	}

	private void cmbMainProgram_SelectedValueChanged(object sender, EventArgs e)
	{
		string appValue = cmbMainProgram.SelectedItem.ToString();
		MiAppConfig.SetValue("mainProgram", appValue);
	}

	private void btnRawSelect_Click(object sender, EventArgs e)
	{
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			if (!string.IsNullOrEmpty(fileName))
			{
				txtRaw.Text = fileName;
				MiAppConfig.SetValue("rawprogram", fileName);
			}
		}
	}

	private void btnPatchSelect_Click(object sender, EventArgs e)
	{
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			if (!string.IsNullOrEmpty(fileName))
			{
				txtPatch.Text = fileName;
				MiAppConfig.SetValue("patch", fileName);
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
            this.chkMD5 = new System.Windows.Forms.CheckBox();
            this.chkRbv = new System.Windows.Forms.CheckBox();
            this.chkWriteDump = new System.Windows.Forms.CheckBox();
            this.chkReadDump = new System.Windows.Forms.CheckBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.chkEraseAll = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkAutoDetect = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFactory = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMainProgram = new System.Windows.Forms.ComboBox();
            this.btnPatchSelect = new System.Windows.Forms.Button();
            this.btnRawSelect = new System.Windows.Forms.Button();
            this.txtPatch = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRaw = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBackupOnly = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCheckPoint = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkMD5
            // 
            this.chkMD5.AutoSize = true;
            this.chkMD5.Location = new System.Drawing.Point(6, 22);
            this.chkMD5.Name = "chkMD5";
            this.chkMD5.Size = new System.Drawing.Size(140, 17);
            this.chkMD5.TabIndex = 0;
            this.chkMD5.Text = "check MD5 before flash";
            this.chkMD5.UseVisualStyleBackColor = true;
            this.chkMD5.CheckedChanged += new System.EventHandler(this.chkMD5_CheckedChanged);
            // 
            // chkRbv
            // 
            this.chkRbv.AutoSize = true;
            this.chkRbv.Location = new System.Drawing.Point(6, 94);
            this.chkRbv.Name = "chkRbv";
            this.chkRbv.Size = new System.Drawing.Size(109, 17);
            this.chkRbv.TabIndex = 1;
            this.chkRbv.Text = "Read Back Verify";
            this.chkRbv.UseVisualStyleBackColor = true;
            this.chkRbv.CheckedChanged += new System.EventHandler(this.chkRbv_CheckedChanged);
            // 
            // chkWriteDump
            // 
            this.chkWriteDump.AutoSize = true;
            this.chkWriteDump.Location = new System.Drawing.Point(6, 131);
            this.chkWriteDump.Name = "chkWriteDump";
            this.chkWriteDump.Size = new System.Drawing.Size(111, 17);
            this.chkWriteDump.TabIndex = 2;
            this.chkWriteDump.Text = "Open Write Dump";
            this.chkWriteDump.UseVisualStyleBackColor = true;
            this.chkWriteDump.CheckedChanged += new System.EventHandler(this.chkWriteDump_CheckedChanged);
            // 
            // chkReadDump
            // 
            this.chkReadDump.AutoSize = true;
            this.chkReadDump.Location = new System.Drawing.Point(6, 56);
            this.chkReadDump.Name = "chkReadDump";
            this.chkReadDump.Size = new System.Drawing.Size(112, 17);
            this.chkReadDump.TabIndex = 2;
            this.chkReadDump.Text = "Open Read Dump";
            this.chkReadDump.UseVisualStyleBackColor = true;
            this.chkReadDump.CheckedChanged += new System.EventHandler(this.chkReadDump_CheckedChanged);
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Location = new System.Drawing.Point(6, 168);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkVerbose.TabIndex = 3;
            this.chkVerbose.Text = "Verbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            this.chkVerbose.CheckedChanged += new System.EventHandler(this.chkVerbose_CheckedChanged);
            // 
            // chkEraseAll
            // 
            this.chkEraseAll.AutoSize = true;
            this.chkEraseAll.Location = new System.Drawing.Point(6, 205);
            this.chkEraseAll.Name = "chkEraseAll";
            this.chkEraseAll.Size = new System.Drawing.Size(64, 17);
            this.chkEraseAll.TabIndex = 4;
            this.chkEraseAll.Text = "EraseAll";
            this.chkEraseAll.UseVisualStyleBackColor = true;
            this.chkEraseAll.CheckedChanged += new System.EventHandler(this.chkEraseAll_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(53, 498);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkAutoDetect
            // 
            this.chkAutoDetect.AutoSize = true;
            this.chkAutoDetect.Location = new System.Drawing.Point(228, 22);
            this.chkAutoDetect.Name = "chkAutoDetect";
            this.chkAutoDetect.Size = new System.Drawing.Size(160, 17);
            this.chkAutoDetect.TabIndex = 5;
            this.chkAutoDetect.Text = "Detect Device Automatically";
            this.chkAutoDetect.UseVisualStyleBackColor = true;
            this.chkAutoDetect.Visible = false;
            this.chkAutoDetect.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(228, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Factory:";
            // 
            // cmbFactory
            // 
            this.cmbFactory.FormattingEnabled = true;
            this.cmbFactory.Items.AddRange(new object[] {
            "please choose",
            "Longcheer",
            "Foxconn",
            "Inventec",
            "Hipad",
            "PTSN",
            "Zowee",
            "FLEX",
            "HEG",
            "BYD",
            "factory",
            "MES",
            "X5MES",
            "test"});
            this.cmbFactory.Location = new System.Drawing.Point(287, 54);
            this.cmbFactory.Name = "cmbFactory";
            this.cmbFactory.Size = new System.Drawing.Size(121, 21);
            this.cmbFactory.TabIndex = 7;
            this.cmbFactory.SelectedValueChanged += new System.EventHandler(this.cmbFactory_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(198, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Main program:";
            // 
            // cmbMainProgram
            // 
            this.cmbMainProgram.FormattingEnabled = true;
            this.cmbMainProgram.Items.AddRange(new object[] {
            "xiaomi",
            "fh_loader"});
            this.cmbMainProgram.Location = new System.Drawing.Point(287, 92);
            this.cmbMainProgram.Name = "cmbMainProgram";
            this.cmbMainProgram.Size = new System.Drawing.Size(121, 21);
            this.cmbMainProgram.TabIndex = 11;
            this.cmbMainProgram.SelectedValueChanged += new System.EventHandler(this.cmbMainProgram_SelectedValueChanged);
            // 
            // btnPatchSelect
            // 
            this.btnPatchSelect.Location = new System.Drawing.Point(440, 418);
            this.btnPatchSelect.Name = "btnPatchSelect";
            this.btnPatchSelect.Size = new System.Drawing.Size(75, 25);
            this.btnPatchSelect.TabIndex = 16;
            this.btnPatchSelect.Text = "select";
            this.btnPatchSelect.UseVisualStyleBackColor = true;
            this.btnPatchSelect.Click += new System.EventHandler(this.btnPatchSelect_Click);
            // 
            // btnRawSelect
            // 
            this.btnRawSelect.Location = new System.Drawing.Point(440, 359);
            this.btnRawSelect.Name = "btnRawSelect";
            this.btnRawSelect.Size = new System.Drawing.Size(75, 25);
            this.btnRawSelect.TabIndex = 17;
            this.btnRawSelect.Text = "select";
            this.btnRawSelect.UseVisualStyleBackColor = true;
            this.btnRawSelect.Click += new System.EventHandler(this.btnRawSelect_Click);
            // 
            // txtPatch
            // 
            this.txtPatch.Location = new System.Drawing.Point(127, 420);
            this.txtPatch.Name = "txtPatch";
            this.txtPatch.Size = new System.Drawing.Size(293, 20);
            this.txtPatch.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 424);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Patch xml：";
            // 
            // txtRaw
            // 
            this.txtRaw.Location = new System.Drawing.Point(127, 361);
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.Size = new System.Drawing.Size(293, 20);
            this.txtRaw.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 364);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "RawProgram xml：";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "xml|*.xml";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBackupOnly);
            this.groupBox1.Controls.Add(this.chkMD5);
            this.groupBox1.Controls.Add(this.chkRbv);
            this.groupBox1.Controls.Add(this.chkWriteDump);
            this.groupBox1.Controls.Add(this.chkReadDump);
            this.groupBox1.Controls.Add(this.chkVerbose);
            this.groupBox1.Controls.Add(this.chkEraseAll);
            this.groupBox1.Controls.Add(this.chkAutoDetect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbFactory);
            this.groupBox1.Controls.Add(this.cmbMainProgram);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(40, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 224);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            // 
            // chkBackupOnly
            // 
            this.chkBackupOnly.AutoSize = true;
            this.chkBackupOnly.Location = new System.Drawing.Point(228, 131);
            this.chkBackupOnly.Name = "chkBackupOnly";
            this.chkBackupOnly.Size = new System.Drawing.Size(87, 17);
            this.chkBackupOnly.TabIndex = 12;
            this.chkBackupOnly.Text = "Backup Only";
            this.chkBackupOnly.UseVisualStyleBackColor = true;
            this.chkBackupOnly.CheckedChanged += new System.EventHandler(this.chkBackupOnly_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(51, 261);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "CheckPoint：";
            // 
            // txtCheckPoint
            // 
            this.txtCheckPoint.Location = new System.Drawing.Point(134, 258);
            this.txtCheckPoint.Name = "txtCheckPoint";
            this.txtCheckPoint.Size = new System.Drawing.Size(166, 20);
            this.txtCheckPoint.TabIndex = 22;
            // 
            // ConfigurationFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 597);
            this.Controls.Add(this.txtCheckPoint);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPatchSelect);
            this.Controls.Add(this.btnRawSelect);
            this.Controls.Add(this.txtPatch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRaw);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.Name = "ConfigurationFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.ConfigurationFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
