using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.form;

public class ComPortConfig : Form
{
	private MainFrm mainFrm;

	private IContainer components;

	private GroupBox groupBoxComs;

	private Button btnOK;

	private TextBox textBox7;

	private TextBox textBox5;

	private TextBox textBox3;

	private TextBox textBox1;

	private TextBox textBox8;

	private Label label8;

	private TextBox textBox6;

	private Label label6;

	private TextBox textBox4;

	private Label label7;

	private Label label4;

	private Label label5;

	private TextBox textBox2;

	private Label label3;

	private Label label2;

	private Label label1;

	public ComPortConfig()
	{
		InitializeComponent();
	}

	private void ComPortConfig_Load(object sender, EventArgs e)
	{
		mainFrm = (MainFrm)base.Owner;
		string[] array = MiAppConfig.Get("mtkComs").Split(',');
		int num = 0;
		foreach (Control control in groupBoxComs.Controls)
		{
			if (num > array.Length - 1)
			{
				break;
			}
			if (control is TextBox)
			{
				TextBox textBox = control as TextBox;
				if (string.IsNullOrEmpty(textBox.Text))
				{
					textBox.Text = array[num];
					num++;
				}
			}
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		List<string> list = new List<string>();
		foreach (Control control in groupBoxComs.Controls)
		{
			if (control is TextBox)
			{
				TextBox textBox = control as TextBox;
				if (!string.IsNullOrEmpty(textBox.Text))
				{
					list.Add(textBox.Text.Trim());
				}
			}
		}
		list.Sort();
		string appValue = string.Join(",", list.ToArray());
		MiAppConfig.SetValue("mtkComs", appValue);
		Close();
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
            this.groupBoxComs = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxComs.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxComs
            // 
            this.groupBoxComs.Controls.Add(this.btnOK);
            this.groupBoxComs.Controls.Add(this.textBox1);
            this.groupBoxComs.Controls.Add(this.textBox2);
            this.groupBoxComs.Controls.Add(this.textBox3);
            this.groupBoxComs.Controls.Add(this.textBox4);
            this.groupBoxComs.Controls.Add(this.textBox5);
            this.groupBoxComs.Controls.Add(this.label8);
            this.groupBoxComs.Controls.Add(this.textBox6);
            this.groupBoxComs.Controls.Add(this.label6);
            this.groupBoxComs.Controls.Add(this.textBox7);
            this.groupBoxComs.Controls.Add(this.label7);
            this.groupBoxComs.Controls.Add(this.label4);
            this.groupBoxComs.Controls.Add(this.label5);
            this.groupBoxComs.Controls.Add(this.textBox8);
            this.groupBoxComs.Controls.Add(this.label3);
            this.groupBoxComs.Controls.Add(this.label2);
            this.groupBoxComs.Controls.Add(this.label1);
            this.groupBoxComs.Location = new System.Drawing.Point(31, 31);
            this.groupBoxComs.Name = "groupBoxComs";
            this.groupBoxComs.Size = new System.Drawing.Size(626, 347);
            this.groupBoxComs.TabIndex = 0;
            this.groupBoxComs.TabStop = false;
            this.groupBoxComs.Text = "ComPortConfig";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(116, 290);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(116, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(118, 20);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(116, 57);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(118, 20);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(116, 89);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(118, 20);
            this.textBox3.TabIndex = 3;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(116, 115);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(118, 20);
            this.textBox4.TabIndex = 4;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(116, 148);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(118, 20);
            this.textBox5.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(79, 236);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "COM";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(116, 174);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(118, 20);
            this.textBox6.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 177);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "COM";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(116, 208);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(118, 20);
            this.textBox7.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(79, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "COM";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "COM";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(79, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "COM";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(116, 234);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(118, 20);
            this.textBox8.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "COM";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "COM";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "COM";
            // 
            // ComPortConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 403);
            this.Controls.Add(this.groupBoxComs);
            this.Name = "ComPortConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ComPortConfig";
            this.Load += new System.EventHandler(this.ComPortConfig_Load);
            this.groupBoxComs.ResumeLayout(false);
            this.groupBoxComs.PerformLayout();
            this.ResumeLayout(false);

	}
}
