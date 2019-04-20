using System;
using System.Collections.Generic;
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
        private struct State
        {
            public bool LMB;
            public bool RMB;
            public Point pos;
        }

        private State previousState;
        private State newState;


        private IntPtr m_nativeHookPtr;
        private Win32Util.LowLevelMouseProc m_managedCallbackObject; //prevent callback from being garbage collected while native still uses it!

        public delegate void NotifyPosCallaback(Point cursorPos, Point pullPos, bool drawing);

        private List<NotifyPosCallaback> callbacks = new List<NotifyPosCallaback>();

        public RopeLogic ropeLogic = new RopeLogic();

        private bool m_LMB_held;
        private bool m_LMB_clicked;
        private bool m_RMB_held;
        private bool m_RMB_clicked;
        private Point m_pos = new Point();
        private Point m_pos_delta = new Point();

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
            if (m_nativeHookPtr == IntPtr.Zero)
            {
                m_managedCallbackObject = new Win32Util.LowLevelMouseProc(interceptInput); //keep reference to callback so its not garbage collected!
                m_nativeHookPtr = Win32Util.registerLowLevelMouseCallback(m_managedCallbackObject);
                succ = (m_nativeHookPtr != IntPtr.Zero);
                Debug.Assert(succ == true, "Error adding the input hook!");
            }
            else
            {
                succ = false;
                Debug.Assert(succ == true, "Trying to add hook while previous one still exists!");
            }
            Console.WriteLine("Input hook started.");
            return succ;
        }

        public bool stop()
        {
            bool succ = true;
            if (m_nativeHookPtr != IntPtr.Zero) {
                succ = Win32Util.UnhookWindowsHookEx(m_nativeHookPtr);
                Debug.Assert(succ == true,"Error removing the input hook!");
                m_nativeHookPtr = IntPtr.Zero;
                m_managedCallbackObject = null;
            }
            Console.WriteLine("Input hook stopped.");
            return succ;
        }

        //public void copyInput(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    Win32Util.INPUT input = new Win32Util.INPUT();
        //    input.Type = Win32Util.INPUT_TYPE.INPUT_MOUSE;
        //    input.Data.Mouse.Flags = (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) ? 0 : KEYEVENTF_KEYUP;
        //    Win32Util.SendInput(1,)
        //}

        private IntPtr interceptInput(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == Win32Util.HC_ACTION)
            {
                newState = previousState;

                Win32Util.MSLLHOOKSTRUCT hookStruct = (Win32Util.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32Util.MSLLHOOKSTRUCT));
                if ((Win32Util.MouseMessages)wParam == Win32Util.MouseMessages.WM_LBUTTONDOWN){
                    newState.LMB = true;                   
                }

                if ((Win32Util.MouseMessages)wParam == Win32Util.MouseMessages.WM_LBUTTONUP){
                    newState.LMB = false;
                    m_LMB_held = false;
                }
                else {
                    //if we do not release LMB and it was previously also pressed
                    if (previousState.LMB) m_LMB_held = true;
                }

                if ((Win32Util.MouseMessages)wParam == Win32Util.MouseMessages.WM_RBUTTONDOWN){
                    newState.RMB = true;
                }

                if ((Win32Util.MouseMessages)wParam == Win32Util.MouseMessages.WM_RBUTTONUP){
                    newState.RMB = false;
                    m_RMB_held = false;
                }
                else {
                    //if we do not release LMB and it was previously also pressed
                    if (previousState.RMB) m_LMB_held = true;
                }


                bool moves = false;
                if ((Win32Util.MouseMessages)wParam == Win32Util.MouseMessages.WM_MOUSEMOVE)
                {
                    m_pos.X = hookStruct.pt.x;
                    m_pos.Y = hookStruct.pt.y;

                    newState.pos = m_pos;

                    m_pos_delta.X = newState.pos.X - previousState.pos.X;
                    m_pos_delta.Y = newState.pos.Y - previousState.pos.Y;
                    //Console.WriteLine("delta: x{0} y{1}", m_pos_delta.X, m_pos_delta.Y);
                    moves = true;
                }
                else
                {
                    m_pos_delta.X = 0;
                    m_pos_delta.Y = 0;
                }

                if (newState.LMB && !moves)
                {
                    ropeLogic.start(VectorUtil.toVec(m_pos));
                }

                //painting happened, we need to intercept
                if (m_LMB_held && moves)
                {
                    ropeLogic.update(VectorUtil.toVec(m_pos),VectorUtil.toVec(m_pos_delta));

                    callCallbacks(ropeLogic.cursorPoint, ropeLogic.pullPoint, true);

                    Win32Util.SetCursorPos(ropeLogic.cursorPoint.X, ropeLogic.cursorPoint.Y);

                    //Console.WriteLine("intercepting: x{0} y{1}", ropeLogic.cursorPoint.X, ropeLogic.cursorPoint.Y);

                    previousState = newState;
                    return (IntPtr)1;
                }
                else 
                {
                    //Console.WriteLine("x{0} y{1} {2} {3}", m_pos.X, m_pos.Y, m_LMB_held ? "L" : " ", m_RMB_held ? "R" : " ");

                    ropeLogic.start(VectorUtil.toVec(m_pos));

                    callCallbacks(ropeLogic.cursorPoint, ropeLogic.pullPoint, false);

                    previousState = newState;
                    return Win32Util.CallNextHookEx(m_nativeHookPtr, nCode, wParam, lParam);
                }


                /* Both methods to directly modify lParam do not "work" but they do change the native value seemingly.
                ** So I guess "passing" modified values like this is just not how this API is meant to work.
                ** A) Write any modifications to hookStruct back to the lParam via marshal
                Marshal.StructureToPtr(hookStruct, lParam, true);

                ** B) unsafe editing the pointer directly
                unsafe
                {
                    Win32Util.MSLLHOOKSTRUCT* data = (Win32Util.MSLLHOOKSTRUCT*)lParam.ToPointer();
                    data->pt.x = 960;
                }

                ** So we Rather swallow the input by returning 1
                ** and use SetCursorPos() or SendInput() to emit new signals.
                ** https://stackoverflow.com/questions/21928956/how-do-i-modify-keys-in-a-keyboardproc-hooking-procedure
                ** LLKHF_INJECTED may be to prevent a infinite loop of sending and reacting to your own messages
                */

                //Console.WriteLine("Mouse hook: " + hookStruct.pt.x + ", " + hookStruct.pt.y);
                //Console.WriteLine("flag: " + hookStruct.flags);
                //if (hookStruct.flags == (uint)Win32Util.LLHookFLags.LLMHF_INJECTED)
                //{
                //    Console.WriteLine("is: " + hookStruct.flags + " LLMHF_INJECTED");
                //}
            }


            return Win32Util.CallNextHookEx(m_nativeHookPtr, nCode, wParam, lParam);
        }


        public void registerUpdateCallback(NotifyPosCallaback callback)
        {
            callbacks.Add(callback);
        }

        public void callCallbacks(Point cursorPos, Point pullPos, bool drawing)
        {
            foreach(var callback in callbacks)
            {
                callback(cursorPos, pullPos, drawing);
            }
        }
    }


}