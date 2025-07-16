namespace XiaoMiFlash.code.module;

public class StreamingArg
{
	public const int HELLO_COMMAND = 1;

	public const int HELLO_RESPONSE = 2;

	public const int READ_PACKET_COMMAND = 3;

	public const int READ_PACKET_RESPONSE = 4;

	public const int STREAM_WRITE_COMMAND = 7;

	public const int STREAM_WRITE_RESPONSE = 8;

	public const int RESET_COMMAND = 11;

	public const int RESET_RESPONSE = 12;

	public const int ERROR_RESPONSE = 13;

	public const int LOG_RESPONSE = 14;

	public const int CLOSE_COMMAND = 21;

	public const int CLOSE_RESPONSE = 22;

	public const int SECURITY_MODE_COMMAND = 23;

	public const int SECURITY_MODE_RESPONSE = 24;

	public const int OPEN_MULTI_IMAGE_COMMAND = 27;

	public const int OPEN_MULTI_IMAGE_RESPONSE = 28;

	public const int ASYNC_HDLC_FLAG = 126;

	public const int ASYNC_HDLC_ESC = 125;

	public const int ASYNC_HDLC_MASK = 32;

	public const int RCV_HUNT_FLAG = 0;

	public const int RCV_GOT_FLAG = 1;

	public const int RCV_GATHER_DATA = 2;

	public const int RCV_GOT_PACKET = 3;

	public const int CRC16_SEED = 0;

	public const int CRC16_OK = 3911;

	public const int CRC32_SEED = 0;
}
