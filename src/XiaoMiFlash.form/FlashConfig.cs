using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XiaoMiFlash.form;

public class FlashConfig : Form
{
	private MainFrm mainFrm;

	private IContainer components;

	private Label label1;

	private TextBox txtScript;

	private Button btnScriptSelect;

	private Label label2;

	private TextBox txtRaw;

	private Button btnRawSelect;

	private Label label3;

	private TextBox txtPatch;

	private Button btnPatchSelect;

	private OpenFileDialog openFileDialog;

	public FlashConfig()
	{
		InitializeComponent();
	}

	private void FlashConfig_Load(object sender, EventArgs e)
	{
		mainFrm = (MainFrm)base.Owner;
		openFileDialog.InitialDirectory = mainFrm.SwPath;
	}

	private void btnRawSelect_Click(object sender, EventArgs e)
	{
		openFileDialog.ShowDialog();
	}

	private void btnPatchSelect_Click(object sender, EventArgs e)
	{
		openFileDialog.ShowDialog();
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
		this.label1 = new System.Windows.Forms.Label();
		this.txtScript = new System.Windows.Forms.TextBox();
		this.btnScriptSelect = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.txtRaw = new System.Windows.Forms.TextBox();
		this.btnRawSelect = new System.Windows.Forms.Button();
		this.label3 = new System.Windows.Forms.Label();
		this.txtPatch = new System.Windows.Forms.TextBox();
		this.btnPatchSelect = new System.Windows.Forms.Button();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 40);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(107, 12);
		this.label1.TabIndex = 0;
		this.label1.Text = "Fastboot script：";
		this.txtScript.Location = new System.Drawing.Point(126, 37);
		this.txtScript.Name = "txtScript";
		this.txtScript.Size = new System.Drawing.Size(293, 21);
		this.txtScript.TabIndex = 1;
		this.btnScriptSelect.Location = new System.Drawing.Point(439, 35);
		this.btnScriptSelect.Name = "btnScriptSelect";
		this.btnScriptSelect.Size = new System.Drawing.Size(75, 23);
		this.btnScriptSelect.TabIndex = 2;
		this.btnScriptSelect.Text = "select";
		this.btnScriptSelect.UseVisualStyleBackColor = true;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(61, 85);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(59, 12);
		this.label2.TabIndex = 0;
		this.label2.Text = "Raw xml：";
		this.txtRaw.Location = new System.Drawing.Point(126, 82);
		this.txtRaw.Name = "txtRaw";
		this.txtRaw.Size = new System.Drawing.Size(293, 21);
		this.txtRaw.TabIndex = 1;
		this.btnRawSelect.Location = new System.Drawing.Point(439, 80);
		this.btnRawSelect.Name = "btnRawSelect";
		this.btnRawSelect.Size = new System.Drawing.Size(75, 23);
		this.btnRawSelect.TabIndex = 2;
		this.btnRawSelect.Text = "select";
		this.btnRawSelect.UseVisualStyleBackColor = true;
		this.btnRawSelect.Click += new System.EventHandler(btnRawSelect_Click);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(49, 140);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(71, 12);
		this.label3.TabIndex = 0;
		this.label3.Text = "Patch xml：";
		this.txtPatch.Location = new System.Drawing.Point(126, 137);
		this.txtPatch.Name = "txtPatch";
		this.txtPatch.Size = new System.Drawing.Size(293, 21);
		this.txtPatch.TabIndex = 1;
		this.btnPatchSelect.Location = new System.Drawing.Point(439, 135);
		this.btnPatchSelect.Name = "btnPatchSelect";
		this.btnPatchSelect.Size = new System.Drawing.Size(75, 23);
		this.btnPatchSelect.TabIndex = 2;
		this.btnPatchSelect.Text = "select";
		this.btnPatchSelect.UseVisualStyleBackColor = true;
		this.btnPatchSelect.Click += new System.EventHandler(btnPatchSelect_Click);
		this.openFileDialog.FileName = "openFileDialog1";
		this.openFileDialog.Filter = "xml|*.xml";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(541, 417);
		base.Controls.Add(this.btnPatchSelect);
		base.Controls.Add(this.btnRawSelect);
		base.Controls.Add(this.btnScriptSelect);
		base.Controls.Add(this.txtPatch);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.txtRaw);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.txtScript);
		base.Controls.Add(this.label1);
		base.Name = "FlashConfig";
		this.Text = "FlashConfig";
		base.Load += new System.EventHandler(FlashConfig_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
