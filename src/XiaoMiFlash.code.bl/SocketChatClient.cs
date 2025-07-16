using System;
using System.Net.Sockets;
using XiaoMiFlash.code.Utility;

namespace XiaoMiFlash.code.bl;

internal class SocketChatClient
{
	private Socket m_sock;

	private byte[] m_byBuff = new byte[50];

	public Socket Sock => m_sock;

	public SocketChatClient(Socket sock)
	{
		m_sock = sock;
	}

	public void SetupRecieveCallback(MainFrm app)
	{
		try
		{
			AsyncCallback callback = app.OnRecievedData;
			m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, callback, this);
		}
		catch (Exception ex)
		{
			Log.w("Recieve callback setup failed! " + ex.Message);
		}
	}

	public byte[] GetRecievedData(IAsyncResult ar)
	{
		int num = 0;
		try
		{
			num = m_sock.EndReceive(ar);
		}
		catch
		{
		}
		byte[] array = new byte[num];
		Array.Copy(m_byBuff, array, num);
		return array;
	}
}
