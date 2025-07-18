// Copyright (c) Sven Groot (Ookii.org) 2009
// BSD license; see LICENSE for details.

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace WP_Tool_Pro_DXApplication.Gui.VistaDialogCore
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class ActivationContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ActivationContextSafeHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            NativeMethods.ReleaseActCtx(handle);
            return true;
        }
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeGDIHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeGDIHandle()
            : base(true)
        {
        }

        internal SafeGDIHandle(IntPtr existingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.DeleteObject(handle);
        }
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeDeviceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeDeviceHandle()
            : base(true)
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SafeDeviceHandle(IntPtr existingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.DeleteDC(handle);
        }
    }

    internal class SafeModuleHandle : SafeHandle
    {
        public SafeModuleHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.FreeLibrary(handle);
        }
    }
}