﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using LibgpiodV2 = Interop.LibgpiodV2;

namespace System.Device.Gpio.Libgpiod.V2;

internal class LineRequestSafeHandle : SafeHandle
{
    public LineRequestSafeHandle()
        : base(IntPtr.Zero, true)
    {
    }

    protected override bool ReleaseHandle()
    {
        LibgpiodV2.gpiod_line_request_release(handle);
        return true;
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override void Dispose(bool disposing)
    {
        ReleaseHandle();
    }
}
