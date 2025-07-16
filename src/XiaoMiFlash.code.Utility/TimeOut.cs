using System;
using System.Threading;

namespace XiaoMiFlash.code.Utility;

public class TimeOut
{
	private ManualResetEvent mTimeoutObject;

	private bool mBoTimeout;

	public bool mResult;

	public int processID;

	public DoHandler Do;

	public DoHandlerScript DoScript;

	public TimeOut()
	{
		mTimeoutObject = new ManualResetEvent(initialState: true);
	}

	public bool DoWithTimeout(TimeSpan timeSpan)
	{
		if (Do == null)
		{
			return false;
		}
		mTimeoutObject.Reset();
		mBoTimeout = true;
		Do.BeginInvoke(DoAsyncCallBack, null);
		if (!mTimeoutObject.WaitOne(timeSpan, exitContext: false))
		{
			mBoTimeout = true;
		}
		return mBoTimeout;
	}

	public bool DoWithTimeoutScript(TimeSpan timeSpan)
	{
		if (DoScript == null)
		{
			return false;
		}
		mTimeoutObject.Reset();
		mBoTimeout = true;
		DoScript.BeginInvoke(DoAsyncCallBackScript, null);
		if (!mTimeoutObject.WaitOne(timeSpan, exitContext: false))
		{
			mBoTimeout = true;
		}
		return mBoTimeout;
	}

	private void DoAsyncCallBack(IAsyncResult result)
	{
		try
		{
			mResult = Do.EndInvoke(result);
			mBoTimeout = false;
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			mBoTimeout = true;
		}
		finally
		{
			mTimeoutObject.Set();
		}
	}

	private void DoAsyncCallBackScript(IAsyncResult result)
	{
		try
		{
			processID = DoScript.EndInvoke(result);
			mBoTimeout = false;
		}
		catch (Exception ex)
		{
			Log.w(ex.Message);
			mBoTimeout = true;
		}
		finally
		{
			mTimeoutObject.Set();
		}
	}
}
