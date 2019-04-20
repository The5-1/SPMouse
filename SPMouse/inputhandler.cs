using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;
using System.Diagnostics;


namespace SPMouse
{
    public class InputHandler
    {
        private IntPtr m_HookCallbackPtr;

        public delegate void NotifyPosCallaback(Point p);

        NotifyPosCallaback originalPos;
        NotifyPosCallaback draggerPos;

        public InputHandler()
        {

        }

        ~InputHandler()
        {
            stop();
        }

        public bool start()
        {
            bool succ = true;
            if (m_HookCallbackPtr == IntPtr.Zero)
            {
                m_HookCallbackPtr = Win32Util.registerLowLevelMouseCallback(interceptInput);
                succ = (m_HookCallbackPtr != IntPtr.Zero);
                Debug.Assert(succ == true, "Error adding the input hook!");
            }
            else
            {
                succ = false;
                Debug.Assert(succ == true, "Trying to add hook while previous one is not null!");
            }
            Console.WriteLine("Input hook started.");
            return succ;
        }

        public bool stop()
        {
            bool succ = true;
            if (m_HookCallbackPtr != IntPtr.Zero) {
                succ = Win32Util.UnhookWindowsHookEx(m_HookCallbackPtr);
                Debug.Assert(succ == true,"Error removing the input hook!");
                m_HookCallbackPtr = IntPtr.Zero;
            }
            Console.WriteLine("Input hook stopped.");
            return succ;
        }

        private IntPtr interceptInput(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == Win32Util.HC_ACTION && (Win32Util.MouseMessages)wParam == Win32Util.MouseMessages.WM_MOUSEMOVE)
            {
                Win32Util.MSLLHOOKSTRUCT hookStruct = (Win32Util.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32Util.MSLLHOOKSTRUCT));
                Console.WriteLine("Mouse hook: " + hookStruct.pt.x + ", " + hookStruct.pt.y);

                Point current = new Point(hookStruct.pt.x, hookStruct.pt.y);

                //DOES NOT WORK: Write any modifications to hookStruct back to the lParam
                Marshal.StructureToPtr(hookStruct, lParam, true);
            }

            //return (IntPtr)1;

            return Win32Util.CallNextHookEx(m_HookCallbackPtr, nCode, wParam, lParam);

        }

    }


}