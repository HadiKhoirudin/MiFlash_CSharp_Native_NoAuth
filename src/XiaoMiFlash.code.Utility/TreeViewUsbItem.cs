using System;
using System.Collections.Generic;
using MiUSB;

namespace XiaoMiFlash.code.Utility;

internal class TreeViewUsbItem
{
	public static int ConnectedHubs;

	public static int ConnectedDevices;

	public string Name { get; set; }

	public object Data { get; set; }

	public List<TreeViewUsbItem> Children { get; set; }

	public static List<TreeViewUsbItem> AllUsbDevices
	{
		get
		{
			ConnectedHubs = 0;
			ConnectedDevices = 0;
			TreeViewUsbItem treeViewUsbItem = new TreeViewUsbItem();
			treeViewUsbItem.Name = "Computer";
			treeViewUsbItem.Data = "Machine Name:" + System.Environment.MachineName;
			HostControllerInfo[] allHostControllers = USB.AllHostControllers;
			if (allHostControllers != null)
			{
				List<TreeViewUsbItem> list = new List<TreeViewUsbItem>(allHostControllers.Length);
				HostControllerInfo[] array = allHostControllers;
				for (int i = 0; i < array.Length; i++)
				{
					HostControllerInfo hostControllerInfo = array[i];
					TreeViewUsbItem treeViewUsbItem2 = new TreeViewUsbItem();
					treeViewUsbItem2.Name = hostControllerInfo.Name;
					treeViewUsbItem2.Data = hostControllerInfo;
					string usbRootHubPath = USB.GetUsbRootHubPath(hostControllerInfo.PNPDeviceID);
					treeViewUsbItem2.Children = AddHubNode(usbRootHubPath, "RootHub");
					list.Add(treeViewUsbItem2);
				}
				treeViewUsbItem.Children = list;
			}
			return new List<TreeViewUsbItem>(1) { treeViewUsbItem };
		}
	}

	public static UsbNodeConnectionInformationForPDL GetPDLUsbDevice(int PciIndex, int ConnectionIndex)
	{
		UsbNodeConnectionInformationForPDL result = default(UsbNodeConnectionInformationForPDL);
		HostControllerInfo[] allHostControllers = USB.AllHostControllers;
		if (allHostControllers != null)
		{
			HostControllerInfo[] array = allHostControllers;
			for (int i = 0; i < array.Length; i++)
			{
				string usbRootHubPath = USB.GetUsbRootHubPath(array[i].PNPDeviceID);
				if (int.Parse(usbRootHubPath.Split('&')[0].Split('#')[2]) == PciIndex)
				{
					UsbNodeInformation[] usbNodeInformation = USB.GetUsbNodeInformation(usbRootHubPath);
					if (usbNodeInformation != null && usbNodeInformation[0].NodeType == USB_HUB_NODE.UsbHub)
					{
						result = USB.GetUsbNodeConnectionInformationForPDL(usbRootHubPath, usbNodeInformation[0].NumberOfPorts, ConnectionIndex);
					}
				}
			}
		}
		return result;
	}

	private static List<TreeViewUsbItem> AddHubNode(string HubPath, string HubNodeName)
	{
		UsbNodeInformation[] usbNodeInformation = USB.GetUsbNodeInformation(HubPath);
		if (usbNodeInformation != null)
		{
			TreeViewUsbItem treeViewUsbItem = new TreeViewUsbItem();
			if (string.IsNullOrEmpty(usbNodeInformation[0].Name))
			{
				treeViewUsbItem.Name = HubNodeName;
			}
			else
			{
				treeViewUsbItem.Name = usbNodeInformation[0].Name;
			}
			treeViewUsbItem.Data = usbNodeInformation[0];
			if (usbNodeInformation[0].NodeType == USB_HUB_NODE.UsbHub)
			{
				treeViewUsbItem.Children = AddPortNode(HubPath, usbNodeInformation[0].NumberOfPorts);
			}
			else
			{
				treeViewUsbItem.Children = null;
			}
			return new List<TreeViewUsbItem>(1) { treeViewUsbItem };
		}
		return null;
	}

	private static List<TreeViewUsbItem> AddPortNode(string HubPath, int NumberOfPorts)
	{
		UsbNodeConnectionInformation[] usbNodeConnectionInformation = USB.GetUsbNodeConnectionInformation(HubPath, NumberOfPorts);
		if (usbNodeConnectionInformation != null)
		{
			List<TreeViewUsbItem> list = new List<TreeViewUsbItem>(NumberOfPorts);
			UsbNodeConnectionInformation[] array = usbNodeConnectionInformation;
			for (int i = 0; i < array.Length; i++)
			{
				UsbNodeConnectionInformation usbNodeConnectionInformation2 = array[i];
				TreeViewUsbItem treeViewUsbItem = new TreeViewUsbItem();
				treeViewUsbItem.Name = "[Port" + usbNodeConnectionInformation2.ConnectionIndex + "]" + usbNodeConnectionInformation2.ConnectionStatus;
				treeViewUsbItem.Data = usbNodeConnectionInformation2;
				treeViewUsbItem.Children = null;
				if (usbNodeConnectionInformation2.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
				{
					ConnectedDevices++;
					if (!string.IsNullOrEmpty(usbNodeConnectionInformation2.DeviceDescriptor.Product))
					{
						treeViewUsbItem.Name = treeViewUsbItem.Name + ": " + usbNodeConnectionInformation2.DeviceDescriptor.Product;
					}
					if (usbNodeConnectionInformation2.DeviceIsHub)
					{
						string externalHubPath = USB.GetExternalHubPath(usbNodeConnectionInformation2.DevicePath, usbNodeConnectionInformation2.ConnectionIndex);
						UsbNodeInformation[] usbNodeInformation = USB.GetUsbNodeInformation(externalHubPath);
						if (usbNodeInformation != null)
						{
							treeViewUsbItem.Data = new ExternalHubInfo
							{
								NodeInfo = usbNodeInformation[0],
								NodeConnectionInfo = usbNodeConnectionInformation2
							};
							if (usbNodeInformation[0].NodeType == USB_HUB_NODE.UsbHub)
							{
								treeViewUsbItem.Children = AddPortNode(externalHubPath, usbNodeInformation[0].NumberOfPorts);
								foreach (TreeViewUsbItem child in treeViewUsbItem.Children)
								{
									try
									{
										if (child != null && child.Data != null)
										{
											UsbNodeConnectionInformation usbNodeConnectionInformation3 = (UsbNodeConnectionInformation)child.Data;
											usbNodeConnectionInformation3.HubPath = HubPath;
											int connectionIndex = usbNodeConnectionInformation2.ConnectionIndex;
											usbNodeConnectionInformation3.ConnectionIndex = Convert.ToInt32(connectionIndex.ToString() + usbNodeConnectionInformation3.ConnectionIndex);
											child.Data = usbNodeConnectionInformation3;
											child.Name = "[Port" + usbNodeConnectionInformation3.ConnectionIndex + "]" + usbNodeConnectionInformation2.ConnectionStatus;
										}
									}
									catch (Exception ex)
									{
										Log.w(ex.Message + ":" + ex.StackTrace);
									}
								}
							}
							if (string.IsNullOrEmpty(usbNodeConnectionInformation2.DeviceDescriptor.Product) && !string.IsNullOrEmpty(usbNodeInformation[0].Name))
							{
								treeViewUsbItem.Name = treeViewUsbItem.Name + ": " + usbNodeInformation[0].Name;
							}
						}
						ConnectedHubs++;
					}
				}
				list.Add(treeViewUsbItem);
			}
			return list;
		}
		return null;
	}
}
