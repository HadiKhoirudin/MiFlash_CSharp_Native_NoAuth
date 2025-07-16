using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using XiaoMiFlash.code.bl;

namespace XiaoMiFlash.form;

public class DriverFrm : Form
{
	private IContainer components;

	private GroupBox DriverBox;

	private Label lblDriver;

	private Button btnInstall;

	private Label lblMsg;

	public DriverFrm()
	{
		InitializeComponent();
	}

	private void DriverFrm_Load(object sender, EventArgs e)
	{
		bool flag = CultureInfo.InstalledUICulture.Name.ToLower().IndexOf("zh") >= 0;
		lblMsg.Text = (flag ? "请安装驱动" : "Please install driver");
		btnInstall.Text = (flag ? "安装" : "Install");
		MiDriver miDriver = new MiDriver();
		for (int i = 0; i < miDriver.infFiles.Length; i++)
		{
			lblDriver.Text += $"({i + 1}) {miDriver.infFiles[i]}\r\n";
		}
	}

	private void btnInstall_Click(object sender, EventArgs e)
	{
		string text = btnInstall.Text;
		btnInstall.Text = "Wait......";
		btnInstall.Enabled = false;
		string applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
		MiDriver miDriver = new MiDriver();
		miDriver.CopyFiles(applicationBase);
		miDriver.InstallAllDriver(applicationBase, uninstallOld: true);
		btnInstall.Text = text;
		btnInstall.Enabled = true;
		MessageBox.Show("安装完成");
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
            this.DriverBox = new System.Windows.Forms.GroupBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.lblDriver = new System.Windows.Forms.Label();
            this.lblMsg = new System.Windows.Forms.Label();
            this.DriverBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // DriverBox
            // 
            this.DriverBox.Controls.Add(this.btnInstall);
            this.DriverBox.Controls.Add(this.lblDriver);
            this.DriverBox.Location = new System.Drawing.Point(72, 56);
            this.DriverBox.Name = "DriverBox";
            this.DriverBox.Size = new System.Drawing.Size(534, 246);
            this.DriverBox.TabIndex = 0;
            this.DriverBox.TabStop = false;
            this.DriverBox.Text = "Driver";
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(44, 208);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 25);
            this.btnInstall.TabIndex = 1;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // lblDriver
            // 
            this.lblDriver.AutoSize = true;
            this.lblDriver.Location = new System.Drawing.Point(42, 46);
            this.lblDriver.Name = "lblDriver";
            this.lblDriver.Size = new System.Drawing.Size(0, 13);
            this.lblDriver.TabIndex = 0;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(114, 25);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 13);
            this.lblMsg.TabIndex = 1;
            // 
            // DriverFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 443);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.DriverBox);
            this.Name = "DriverFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Driver";
            this.Load += new System.EventHandler(this.DriverFrm_Load);
            this.DriverBox.ResumeLayout(false);
            this.DriverBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
