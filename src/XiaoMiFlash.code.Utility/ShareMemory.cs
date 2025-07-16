using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace XiaoMiFlash.code.Utility;

public class ShareMemory
{
	public struct SYSTEM_INFO
	{
		public ushort processorArchitecture;

		private ushort reserved;

		public uint pageSize;

		public IntPtr minimumApplicationAddress;

		public IntPtr maximumApplicationAddress;

		public IntPtr activeProcessorMask;

		public uint numberOfProcessors;

		public uint processorType;

		public uint allocationGranularity;

		public ushort processorLevel;

		public ushort processorRevision;
	}

	private const int ERROR_ALREADY_EXISTS = 183;

	private const int FILE_MAP_COPY = 1;

	private const int FILE_MAP_WRITE = 2;

	private const int FILE_MAP_READ = 4;

	private const int FILE_MAP_ALL_ACCESS = 6;

	private const int PAGE_READONLY = 2;

	private const int PAGE_READWRITE = 4;

	private const int PAGE_WRITECOPY = 8;

	private const int PAGE_EXECUTE = 16;

	private const int PAGE_EXECUTE_READ = 32;

	private const int PAGE_EXECUTE_READWRITE = 64;

	private const int SEC_COMMIT = 134217728;

	private const int SEC_IMAGE = 16777216;

	private const int SEC_NOCACHE = 268435456;

	private const int SEC_RESERVE = 67108864;

	private IntPtr m_fHandle;

	private IntPtr m_hSharedMemoryFile = IntPtr.Zero;

	private IntPtr m_pwData = IntPtr.Zero;

	private bool m_bAlreadyExist;

	private bool m_bInit;

	private uint m_MemSize = 20971520u;

	private long m_offsetBegin;

	public long m_FileSize;

	private FileReader File = new FileReader();

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

	[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);

	[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr OpenFileMapping(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

	[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr MapViewOfFile(IntPtr hFileMapping, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

	[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
	public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);

	[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
	public static extern bool CloseHandle(IntPtr handle);

	[DllImport("kernel32")]
	public static extern int GetLastError();

	[DllImport("kernel32.dll")]
	private static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

	public static uint GetPartitionsize()
	{
		GetSystemInfo(out var lpSystemInfo);
		return lpSystemInfo.allocationGranularity;
	}

	public ShareMemory(string filename, uint memSize)
	{
		m_MemSize = memSize;
		Init(filename);
	}

	public ShareMemory(string filename)
	{
		m_MemSize = 20971520u;
		Init(filename);
	}

	~ShareMemory()
	{
		Close();
	}

	protected void Init(string strName)
	{
		if (!System.IO.File.Exists(strName))
		{
			throw new Exception("未找到文件");
		}
		FileInfo fileInfo = new FileInfo(strName);
		m_FileSize = fileInfo.Length;
		m_fHandle = File.Open(strName);
		if (strName.Length > 0)
		{
			m_hSharedMemoryFile = CreateFileMapping(m_fHandle, IntPtr.Zero, 2u, 0u, (uint)m_FileSize, "mdata");
			if (m_hSharedMemoryFile == IntPtr.Zero)
			{
				m_bAlreadyExist = false;
				m_bInit = false;
				throw new Exception("CreateFileMapping失败LastError=" + GetLastError());
			}
			m_bInit = true;
			if (m_MemSize > m_FileSize)
			{
				m_MemSize = (uint)m_FileSize;
			}
			m_pwData = MapViewOfFile(m_hSharedMemoryFile, 4u, 0u, 0u, m_MemSize);
			if (m_pwData == IntPtr.Zero)
			{
				m_bInit = false;
				throw new Exception("m_hSharedMemoryFile失败LastError=" + GetLastError() + "  " + new Win32Exception(GetLastError()).Message);
			}
		}
	}

	private static uint GetHighWord(ulong intValue)
	{
		return Convert.ToUInt32(intValue >> 32);
	}

	private static uint GetLowWord(ulong intValue)
	{
		return Convert.ToUInt32(intValue & 0xFFFFFFFFu);
	}

	public uint GetNextblock()
	{
		if (!m_bInit)
		{
			throw new Exception("文件未初始化。");
		}
		uint memberSize = GetMemberSize();
		if (memberSize == 0)
		{
			return memberSize;
		}
		m_MemSize = memberSize;
		m_pwData = MapViewOfFile(m_hSharedMemoryFile, 4u, GetHighWord((ulong)m_offsetBegin), GetLowWord((ulong)m_offsetBegin), memberSize);
		if (m_pwData == IntPtr.Zero)
		{
			m_bInit = false;
			throw new Exception("映射文件块失败" + GetLastError());
		}
		m_offsetBegin += memberSize;
		return memberSize;
	}

	private uint GetMemberSize()
	{
		if (m_offsetBegin >= m_FileSize)
		{
			return 0u;
		}
		if (m_offsetBegin + m_MemSize >= m_FileSize)
		{
			return (uint)(m_FileSize - m_offsetBegin);
		}
		return m_MemSize;
	}

	public void Close()
	{
		if (m_bInit)
		{
			UnmapViewOfFile(m_pwData);
			CloseHandle(m_hSharedMemoryFile);
			File.Close();
		}
	}

	public void Read(ref byte[] bytData, int lngAddr, int lngSize, bool Unmap)
	{
		if (lngAddr + lngSize > m_MemSize)
		{
			throw new Exception("Read操作超出数据区");
		}
		if (m_bInit)
		{
			Marshal.Copy(m_pwData, bytData, lngAddr, lngSize);
			if (Unmap && UnmapViewOfFile(m_pwData))
			{
				m_pwData = IntPtr.Zero;
			}
			return;
		}
		throw new Exception("文件未初始化");
	}

	public void Read(ref byte[] bytData, int lngAddr, int lngSize)
	{
		if (lngAddr + lngSize > m_MemSize)
		{
			throw new Exception("Read操作超出数据区");
		}
		if (m_bInit)
		{
			Marshal.Copy(m_pwData, bytData, lngAddr, lngSize);
			return;
		}
		throw new Exception("文件未初始化");
	}

	public uint ReadBytes(int lngAddr, ref byte[] byteData, int StartIndex, uint intSize)
	{
		if (lngAddr >= m_MemSize)
		{
			throw new Exception("起始数据超过缓冲区长度");
		}
		if (lngAddr + intSize > m_MemSize)
		{
			intSize = m_MemSize - (uint)lngAddr;
		}
		if (m_bInit)
		{
			Marshal.Copy(new IntPtr((long)m_pwData + lngAddr), byteData, StartIndex, (int)intSize);
			return intSize;
		}
		throw new Exception("文件未初始化");
	}

	private int Write(byte[] bytData, int lngAddr, int lngSize)
	{
		if (lngAddr + lngSize > m_MemSize)
		{
			return 2;
		}
		if (m_bInit)
		{
			Marshal.Copy(bytData, lngAddr, m_pwData, lngSize);
			return 0;
		}
		return 1;
	}
}
