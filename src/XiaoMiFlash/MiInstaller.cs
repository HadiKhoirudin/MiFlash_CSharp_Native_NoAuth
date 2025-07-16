using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using XiaoMiFlash.code.bl;

namespace XiaoMiFlash;

[RunInstaller(true)]
public class MiInstaller : Installer
{
	private IContainer components;

	public MiInstaller()
	{
		InitializeComponent();
		base.BeforeInstall += MiInstaller_BeforeInstall;
		base.AfterInstall += MiInstaller_AfterInstall;
	}

	private void MiInstaller_AfterInstall(object sender, InstallEventArgs e)
	{
		MiDriver miDriver = new MiDriver();
		miDriver.CopyFiles(base.Context.Parameters["assemblypath"]);
		miDriver.InstallAllDriver(base.Context.Parameters["assemblypath"], uninstallOld: false);
	}

	public override void Install(IDictionary savedState)
	{
		try
		{
			base.Install(savedState);
		}
		catch (Exception)
		{
		}
	}

	private void MiInstaller_BeforeInstall(object sender, InstallEventArgs e)
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
		components = new Container();
	}
}
