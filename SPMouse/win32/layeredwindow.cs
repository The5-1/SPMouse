using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/* From: https://blogs.msdn.microsoft.com/mswanson/2005/07/07/transparent-window-sample/
** Uses the "UpdateLayeredWindow" command to draw with propper, antialiased alpha blending
*/


public static partial class Win32Util
{

    // Required constants
    //public const Int32 WS_EX_LAYERED = 0x80000;
    public const Int32 HTCAPTION = 0x02;
    public const Int32 WM_NCHITTEST = 0x84;
    public const Int32 ULW_ALPHA = 0x02;
    public const byte AC_SRC_OVER = 0x00;
    public const byte AC_SRC_ALPHA = 0x01;

    public enum Bool
    {
        False = 0,
        True = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public Int32 x;
        public Int32 y;

        public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public Int32 cx;
        public Int32 cy;

        public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ARGB
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    //[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    //public static extern IntPtr GetDC(IntPtr hWnd);

    //[DllImport("user32.dll", ExactSpelling = true)]
    //public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteObject(IntPtr hObject);

}