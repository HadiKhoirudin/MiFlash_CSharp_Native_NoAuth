using System.Windows.Forms;
using XiaoMiFlash.code.lan;

namespace XiaoMiFlash;

public class MiBaseFrm : Form, ILanguageSupport
{
	private string lanid = "";

	public string LanID
	{
		get
		{
			return lanid;
		}
		set
		{
			lanid = value;
		}
	}

	public virtual void SetLanguage()
	{
	}

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // MiBaseFrm
            // 
            this.ClientSize = new System.Drawing.Size(283, 261);
            this.Name = "MiBaseFrm";
            this.ResumeLayout(false);

    }
}
