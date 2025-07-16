using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XiaoMiFlash.code.data;

namespace XiaoMiFlash.form;

public class SwDownloadProgress : Form
{
	private IContainer components;

	private ProgressBar progressBar;

	private Label label1;

	private Label lblPath;

	private Label label2;

	private Label lblSize;

	private Label lblPercent;

	private Timer dlTimer;

	private Label label3;

	private Label lblLocalPath;

	public SwDownloadProgress()
	{
		InitializeComponent();
	}

	private void DownloadProgress_Load(object sender, EventArgs e)
	{
		dlTimer.Interval = 1000;
		dlTimer.Enabled = true;
		progressBar.Value = 0;
		progressBar.Maximum = 100;
		lblPath.Text = MiFlashGlobal.Swdes.serverPath;
		lblSize.Text = MiFlashGlobal.Swdes.fileSize.ToString();
	}

	private void dlTimer_Tick(object sender, EventArgs e)
	{
		if (MiFlashGlobal.Swdes != null)
		{
			Uri uri = new Uri(MiFlashGlobal.Swdes.serverPath);
			lblPath.Text = uri.AbsoluteUri;
			lblSize.Text = MiFlashGlobal.Swdes.fileSize / 1024.0 + " kb";
			lblLocalPath.Text = MiFlashGlobal.Swdes.localPath;
			progressBar.Value = (int)MiFlashGlobal.Swdes.percent;
			lblPercent.Text = string.Format("{0}%", MiFlashGlobal.Swdes.percent.ToString("#0.000"));
			if (MiFlashGlobal.Swdes.percent >= 100.0)
			{
				progressBar.Value = progressBar.Maximum;
			}
			if (MiFlashGlobal.Swdes.isDone)
			{
				dlTimer.Enabled = false;
				Close();
				Dispose();
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.dlTimer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.lblLocalPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(46, 113);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(475, 35);
            this.progressBar.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Download:";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(97, 20);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(0, 13);
            this.lblPath.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Total:";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(99, 80);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(0, 13);
            this.lblSize.TabIndex = 4;
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(521, 126);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(0, 13);
            this.lblPercent.TabIndex = 5;
            // 
            // dlTimer
            // 
            this.dlTimer.Tick += new System.EventHandler(this.dlTimer_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "To :";
            // 
            // lblLocalPath
            // 
            this.lblLocalPath.AutoSize = true;
            this.lblLocalPath.Location = new System.Drawing.Point(97, 48);
            this.lblLocalPath.Name = "lblLocalPath";
            this.lblLocalPath.Size = new System.Drawing.Size(0, 13);
            this.lblLocalPath.TabIndex = 7;
            // 
            // SwDownloadProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 185);
            this.Controls.Add(this.lblLocalPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SwDownloadProgress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DownloadProgress";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DownloadProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
