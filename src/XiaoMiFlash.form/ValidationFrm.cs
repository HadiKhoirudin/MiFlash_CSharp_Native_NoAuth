using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XiaoMiFlash.form;

public class ValidationFrm : Form
{
	private IContainer components;

	private Button btnCheckAll;

	private Button btnSpecify;

	private OpenFileDialog openXmlFile;

	public ValidationFrm()
	{
		InitializeComponent();
	}

	private void btnCheckAll_Click(object sender, EventArgs e)
	{
		MainFrm obj = (MainFrm)base.Owner;
		obj.ValidateSpecifyXml = "";
		obj.RefreshDevice();
		obj.CheckSha256();
		Close();
		Dispose();
	}

	private void btnSpecify_Click(object sender, EventArgs e)
	{
		MainFrm mainFrm = (MainFrm)base.Owner;
		openXmlFile.InitialDirectory = mainFrm.SwPath;
		if (openXmlFile.ShowDialog() == DialogResult.OK)
		{
			string fileName = openXmlFile.FileName;
			mainFrm.ValidateSpecifyXml = fileName;
			mainFrm.RefreshDevice();
			mainFrm.CheckSha256();
			Close();
			Dispose();
		}
		else
		{
			MessageBox.Show("Please select a xml file.");
		}
	}

	private void ValidationFrm_Load(object sender, EventArgs e)
	{
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
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnSpecify = new System.Windows.Forms.Button();
            this.openXmlFile = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(63, 77);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 25);
            this.btnCheckAll.TabIndex = 0;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnSpecify
            // 
            this.btnSpecify.Location = new System.Drawing.Point(63, 133);
            this.btnSpecify.Name = "btnSpecify";
            this.btnSpecify.Size = new System.Drawing.Size(75, 25);
            this.btnSpecify.TabIndex = 1;
            this.btnSpecify.Text = "Specify";
            this.btnSpecify.UseVisualStyleBackColor = true;
            this.btnSpecify.Click += new System.EventHandler(this.btnSpecify_Click);
            // 
            // openXmlFile
            // 
            this.openXmlFile.FileName = "*.xml";
            // 
            // ValidationFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 226);
            this.Controls.Add(this.btnSpecify);
            this.Controls.Add(this.btnCheckAll);
            this.Name = "ValidationFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Check Sha256";
            this.Load += new System.EventHandler(this.ValidationFrm_Load);
            this.ResumeLayout(false);

	}
}
