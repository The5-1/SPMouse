// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.

//from https://archive.codeplex.com/?p=uiautomationverify


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


public static partial class Win32Util
{
    [DllImport("user32.dll")]
    public static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);
    [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetCurrentObject(IntPtr hdc, uint objectType);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("ntdll.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern void NtClose(IntPtr hToken);
    [DllImport("ntdll.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int NtOpenProcessToken(IntPtr hProcess, uint accessMask, out IntPtr hToken);
    [DllImport("ntdll.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int NtQueryInformationToken(IntPtr hToken, uint tokenElevationType, out IntPtr elevationInfo, uint bufferSize, out uint tokensize);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int atom, uint fsModifiers, uint vk);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int atom);

    [StructLayout(LayoutKind.Sequential)]
    public struct TOKEN_ELEVATION_INFO
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint TokenIsElevated;
    }

}
