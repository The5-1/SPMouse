using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

using System.Drawing;

/* The interop is used to call native windwos methods via DllImport and pInvoke
** https://docs.microsoft.com/en-us/dotnet/framework/interop/index
** https://docs.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions
*/
using System.Runtime.InteropServices;

namespace PreciseMouse
{
    //public class Highlighter
    //{
    //    ScreenBoundingRectangle _rectangle = new ScreenBoundingRectangle();
    //    public void DrawRectangle(Rectangle rect)
    //    {
    //        _rectangle.Color = System.Drawing.Color.Red;
    //        _rectangle.Opacity = 0.8;
    //        _rectangle.Location = rect;
    //        this._rectangle.Visible = true;
    //    }
    //}

    static class Interop
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        /* https://www.pinvoke.net/default.aspx/coredll/GetCursorPos.html
        ** Need to replace CoreDll with User32 though
        */
        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        public static bool setCursorPos(Point p)
        {
            return SetCursorPos(p.X, p.Y);
        }


        /*
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        */

    }

    static class SceenUtil{

        static Point lastPos_ = new Point();
        static Point newPos_ = new Point();

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        /* https://www.pinvoke.net/default.aspx/coredll/GetCursorPos.html
        ** Need to replace CoreDll with User32 though
        */
        [DllImport("User32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);



        public static void test()
        {
            Point pos = new Point();
            GetCursorPos(ref pos);

            Console.WriteLine("Pos: {0},{1}", pos.X,pos.Y);

            IntPtr desktopPtr = GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);


            SolidBrush sb = new SolidBrush(Color.DarkRed);
            Pen p = new Pen(sb);
            //g.FillRectangle(sb, new Rectangle(0, 0, 128, 128));

            g.DrawLine(p, lastPos_, newPos_);

            SetCursorPos(lastPos_.X, lastPos_.Y);

            g.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }


        public static bool updatePos()
        {
            GetCursorPos(ref newPos_);

            if(newPos_ != lastPos_){
                return true;
            }
            else{
                return false;
            }
        }

        public static void endFrame()
        {
            lastPos_ = newPos_;
        }

    }


    static class MathUtil
    {
        public static void toVec(Point p, ref Vector2 vec)
        {
            vec.X = (float)p.X;
            vec.Y = (float)p.Y;
        }

        public static void toPoint(Vector2 vec, ref Point p)
        {
            p.X = (int)Math.Floor(vec.X + 0.5f);
            p.Y = (int)Math.Floor(vec.Y + 0.5f);
        }

        public static void add(ref Point p, Vector2 vec)
        {
            p.X = p.X + (int)vec.X;
            p.Y = p.Y +(int)vec.Y;
        }

    }

    static class SPMouse
    {
        static double maxdist = 32.0f;

        static IntPtr desktopPtr;
        static Graphics g;

        static Pen penA = Pens.DarkGray;
        static Pen penB = Pens.DarkRed;
        //static SolidBrush brushA = new SolidBrush(Color.DarkGray);
        //static SolidBrush brushB = new SolidBrush(Color.DarkRed);
        //static Pen penA = new Pen(brushA);
        //static Pen penB = new Pen(brushB);

        public static Point previousMousePos_ = new Point();
        public static Point currentMousePos_ = new Point();
        public static Point overriddenPos_ = new Point();
        public static Point draggerPos_ = new Point();
        public static Vector2 delta = new Vector2();
        public static Vector2 dir = new Vector2();
        public static float draggerDist;
        public static float tension;
        public static Vector2 move = new Vector2();


        static SPMouse()
        {

        }

        public static void init()
        {
            desktopPtr = Interop.GetDC(IntPtr.Zero);
            g = Graphics.FromHdc(desktopPtr);

            Interop.GetCursorPos(ref previousMousePos_);
            Interop.GetCursorPos(ref currentMousePos_);
            Interop.GetCursorPos(ref overriddenPos_);
            Interop.GetCursorPos(ref draggerPos_);
        }

        public static bool getPos()
        {
            Interop.GetCursorPos(ref currentMousePos_);

            if (previousMousePos_ != currentMousePos_)
            {
                previousMousePos_ = currentMousePos_;
                draggerPos_ = currentMousePos_;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void updateCursor()
        {
            delta.X = (float)(draggerPos_.X - overriddenPos_.X);
            delta.Y = (float)(draggerPos_.Y - overriddenPos_.Y);
            draggerDist = delta.Length();
            dir = Vector2.Normalize(delta);
            tension = (float)(Math.Max(0.0,draggerDist - maxdist));

            move = Vector2.Multiply(tension, dir);
            MathUtil.add(ref overriddenPos_, move);
            Interop.setCursorPos(overriddenPos_);
        }

        public static void endFrame()
        {
            previousMousePos_ = currentMousePos_;
        }

        public static void draw()
        {
            g.DrawLine(penB, overriddenPos_, draggerPos_);
        }

        public static void debug()
        {
            Console.WriteLine("Mouse: {0},{1}", overriddenPos_.X, overriddenPos_.Y);
            Console.WriteLine("Dragr: {0},{1}", draggerPos_.X, draggerPos_.Y);
            Console.WriteLine("dist: {0}", draggerDist);
            Console.WriteLine("tension: {0}", tension);
        }

    }

    class App
    {
        static void Main(string[] args)
        {
            SPMouse.init();

            Console.WriteLine("Hello: {0}", "test");

            while (true)
            {
                if (SPMouse.getPos())
                {
                    //SceenUtil.test();
                    SPMouse.updateCursor();
                    SPMouse.debug();
                }
                SPMouse.draw();
                SPMouse.endFrame();
            }
        }
    }
}
