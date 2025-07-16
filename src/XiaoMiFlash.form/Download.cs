using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XiaoMiFlash.form;

public class Download : Form
{
	private IContainer components;

	private ComboBox cmbVersionCat;

	private RadioButton rdoNoRoot;

	private RadioButton rdoRoot;

	private Panel panel1;

	private ComboBox cmbVersions;

	private Label label1;

	private TextBox txtServer;

	public Download()
	{
		InitializeComponent();
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
            this.cmbVersionCat = new System.Windows.Forms.ComboBox();
            this.rdoNoRoot = new System.Windows.Forms.RadioButton();
            this.rdoRoot = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbVersions = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbVersionCat
            // 
            this.cmbVersionCat.FormattingEnabled = true;
            this.cmbVersionCat.Location = new System.Drawing.Point(91, 157);
            this.cmbVersionCat.Name = "cmbVersionCat";
            this.cmbVersionCat.Size = new System.Drawing.Size(121, 21);
            this.cmbVersionCat.TabIndex = 0;
            // 
            // rdoNoRoot
            // 
            this.rdoNoRoot.AutoSize = true;
            this.rdoNoRoot.Location = new System.Drawing.Point(17, 13);
            this.rdoNoRoot.Name = "rdoNoRoot";
            this.rdoNoRoot.Size = new System.Drawing.Size(55, 17);
            this.rdoNoRoot.TabIndex = 1;
            this.rdoNoRoot.TabStop = true;
            this.rdoNoRoot.Text = "非root";
            this.rdoNoRoot.UseVisualStyleBackColor = true;
            // 
            // rdoRoot
            // 
            this.rdoRoot.AutoSize = true;
            this.rdoRoot.Location = new System.Drawing.Point(103, 13);
            this.rdoRoot.Name = "rdoRoot";
            this.rdoRoot.Size = new System.Drawing.Size(43, 17);
            this.rdoRoot.TabIndex = 1;
            this.rdoRoot.TabStop = true;
            this.rdoRoot.Text = "root";
            this.rdoRoot.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdoNoRoot);
            this.panel1.Controls.Add(this.rdoRoot);
            this.panel1.Location = new System.Drawing.Point(91, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(265, 50);
            this.panel1.TabIndex = 2;
            // 
            // cmbVersions
            // 
            this.cmbVersions.FormattingEnabled = true;
            this.cmbVersions.Location = new System.Drawing.Point(235, 157);
            this.cmbVersions.Name = "cmbVersions";
            this.cmbVersions.Size = new System.Drawing.Size(121, 21);
            this.cmbVersions.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Server:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(156, 112);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(200, 20);
            this.txtServer.TabIndex = 5;
            // 
            // Download
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 411);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbVersions);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cmbVersionCat);
            this.Name = "Download";
            this.Text = "Download";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
