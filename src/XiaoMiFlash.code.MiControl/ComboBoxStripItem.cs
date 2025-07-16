using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace XiaoMiFlash.code.MiControl;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
public class ComboBoxStripItem : ToolStripControlHost
{
	private ComboBox comboBox;

	public string SelectedText
	{
		get
		{
			if (comboBox.SelectedItem != null)
			{
				return comboBox.SelectedItem.ToString();
			}
			return "";
		}
	}

	public ComboBoxStripItem()
		: base(new ComboBox())
	{
		comboBox = base.Control as ComboBox;
	}

	public void SetItem(string[] items)
	{
		comboBox.Items.Clear();
		for (int i = 0; i < items.Length; i++)
		{
			comboBox.Items.Add(items[i]);
		}
	}

	public void SetText(string text)
	{
		bool flag = false;
		foreach (object item in comboBox.Items)
		{
			if (item.ToString() == text)
			{
				comboBox.SelectedItem = item;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			comboBox.SelectedItem = null;
		}
	}

	public void Enable(bool enable)
	{
		comboBox.Enabled = enable;
	}

	public void OnlyDrop()
	{
		comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
	}
}
