// Copyright (c) Sven Groot (Ookii.org) 2009
// BSD license; see LICENSE for details.

using System;
using System.Windows.Forms;

namespace WP_Tool_Pro_DXApplication.Gui.VistaDialogCore
{
    internal class WindowHandleWrapper : IWin32Window
    {
        public WindowHandleWrapper(IntPtr handle)
        {
            Handle = handle;
        }

        #region IWin32Window Members

        public IntPtr Handle { get; }

        #endregion
    }
}