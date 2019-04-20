// Most Win32 calls based on
// https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-keyboard-hook-in-c/
// https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-mouse-hook-in-c/
// other sources appear at the individual members

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

public static partial class Win32Util
{
    //Codes for installing a hook to the low level (_LL) MouseInput
    //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowshookexa
    public const int WH_MOUSE_LL = 14;
    // nCode returned to mouse callback, for mouses its only HC_ACTION or <0
    public const int HC_ACTION = 0;

    // wParam returned to mouse callback: https://msdn.microsoft.com/en-us/library/ms644986(v=VS.85).aspx
    public enum MouseMessages
    {
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_MOUSEWHEEL = 0x020A
    }

    //Signature for the event callback: https://msdn.microsoft.com/en-us/library/ms644986(v=VS.85).aspx
    //nCode: 0 means HC_ACTION, so something happened, <0 would mean "ignore and pass to next hook"
    //wParam: is the MouseMessage type enum
    //lParam: pointer to a MSLLHOOKSTRUCT containing e.g Position: https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmsllhookstruct
    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    /*SetWindowsHookEx to install a hook to an event: https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowshookexa
    **"proc" is the callback when the event happens, the signature is described here: https://msdn.microsoft.com/en-us/library/ms644986(v=VS.85).aspx
    ** Returns:
    **      Hook pointer neded for releasing the hook or handling callbacks
    */
    public static IntPtr registerLowLevelMouseCallback(LowLevelMouseProc proc)
    {
        return SetWindowsHookEx(
            WH_MOUSE_LL, //Hook Location ID
            proc, //a callback to register
            GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), //in what dll the callback can be found, so the current one
            0 //the thread where the procedure shall be called, 0 for all threads
            );
    }

    public static bool releaseLowLevelMouseCallback(IntPtr hookptr)
    {
        return UnhookWindowsHookEx(hookptr);
    }

    public static IntPtr exsampleHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode == HC_ACTION && (MouseMessages)wParam == MouseMessages.WM_MOUSEMOVE)
        {
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            Console.WriteLine("Mouse hook: " + hookStruct.pt.x + ", " + hookStruct.pt.y);

            //DOES NOT WORK: Write any modifications to hookStruct back to the lParam
            Marshal.StructureToPtr(hookStruct, lParam, true);
        }

#if false
        //A) forward the event to the next hook, recommended by the documentation: https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-callnexthookex#remarks
        //return CallNextHookEx(_hookID, nCode, wParam, lParam);
#elif false
        //B) swallow the event and do not forward it, see: https://stackoverflow.com/questions/2067397/can-i-change-a-users-keyboard-input
        //return (IntPtr)1;//nonzero value = Swallow the Input!  
#elif true
        //C) 0 also passes it along, NULL may be an error that is then gracefully handled
        return (IntPtr)0;
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {

        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-taginput
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmouseinput
    [StructLayout(LayoutKind.Explicit)] // LayoutKind.Explicit = Union
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint Msg;
        public ushort ParamL;
        public ushort ParamH;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MSLLHOOKSTRUCT lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern long SetCursorPos(int x, int y);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

}