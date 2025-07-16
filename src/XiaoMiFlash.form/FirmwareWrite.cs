using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.form;

public class FirmwareWrite : Form
{
	private MainFrm mainFrm;

	private IContainer components;

	private Button btnRawSelect;

	private TextBox txtFfu;

	private Label label5;

	private CheckBox chkFirmwarewrite;

	private OpenFileDialog openFileDialog;

	private Button btnOK;

	public FirmwareWrite()
	{
		InitializeComponent();
	}

	private void label5_Click(object sender, EventArgs e)
	{
	}

	private void FirmwareWrite_Load(object sender, EventArgs e)
	{
		mainFrm = (MainFrm)base.Owner;
	}

	private void chkFirmwarewrite_CheckedChanged(object sender, EventArgs e)
	{
		mainFrm.SetFirmwarewrite(chkFirmwarewrite.Checked);
	}

	private void btnRawSelect_Click(object sender, EventArgs e)
	{
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			if (!string.IsNullOrEmpty(fileName))
			{
				txtFfu.Text = fileName;
				MiAppConfig.SetValue("ffuList", fileName);
			}
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		MiAppConfig.SetValue("ffuList", txtFfu.Text.Trim());
		Close();
		Dispose();
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
		this.btnRawSelect = new System.Windows.Forms.Button();
		this.txtFfu = new System.Windows.Forms.TextBox();
		this.label5 = new System.Windows.Forms.Label();
		this.chkFirmwarewrite = new System.Windows.Forms.CheckBox();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.btnOK = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.btnRawSelect.Location = new System.Drawing.Point(596, 127);
		this.btnRawSelect.Margin = new System.Windows.Forms.Padding(4);
		this.btnRawSelect.Name = "btnRawSelect";
		this.btnRawSelect.Size = new System.Drawing.Size(100, 29);
		this.btnRawSelect.TabIndex = 20;
		this.btnRawSelect.Text = "select";
		this.btnRawSelect.UseVisualStyleBackColor = true;
		this.btnRawSelect.Click += new System.EventHandler(btnRawSelect_Click);
		this.btnRawSelect.Visible = false;
		this.txtFfu.Location = new System.Drawing.Point(178, 129);
		this.txtFfu.Margin = new System.Windows.Forms.Padding(4);
		this.txtFfu.Name = "txtFfu";
		this.txtFfu.Size = new System.Drawing.Size(389, 25);
		this.txtFfu.TabIndex = 19;
		this.txtFfu.Visible = false;
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(36, 133);
		this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(71, 15);
		this.label5.TabIndex = 18;
		this.label5.Text = "ffu_list";
		this.label5.Click += new System.EventHandler(label5_Click);
		this.label5.Visible = false;
		this.chkFirmwarewrite.AutoSize = true;
		this.chkFirmwarewrite.Location = new System.Drawing.Point(38, 54);
		this.chkFirmwarewrite.Margin = new System.Windows.Forms.Padding(4);
		this.chkFirmwarewrite.Name = "chkFirmwarewrite";
		this.chkFirmwarewrite.Size = new System.Drawing.Size(133, 19);
		this.chkFirmwarewrite.TabIndex = 21;
		this.chkFirmwarewrite.Text = "not do firmwarewrite";
		this.chkFirmwarewrite.UseVisualStyleBackColor = true;
		this.chkFirmwarewrite.CheckedChanged += new System.EventHandler(chkFirmwarewrite_CheckedChanged);
		this.btnOK.Location = new System.Drawing.Point(38, 190);
		this.btnOK.Margin = new System.Windows.Forms.Padding(4);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(100, 29);
		this.btnOK.TabIndex = 22;
		this.btnOK.Text = "OK";
		this.btnOK.UseVisualStyleBackColor = true;
		this.btnOK.Click += new System.EventHandler(btnOK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(731, 246);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.chkFirmwarewrite);
		base.Controls.Add(this.btnRawSelect);
		base.Controls.Add(this.txtFfu);
		base.Controls.Add(this.label5);
		base.Name = "FirmwareWrite";
		this.Text = "FirmwareWrite";
		base.Load += new System.EventHandler(FirmwareWrite_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
