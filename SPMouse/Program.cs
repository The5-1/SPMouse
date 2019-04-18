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
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("User32.dll")]
        static extern bool ClientToScreen(IntPtr hwnd, ref Point lpPoint);

        [DllImport("User32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        public static bool getCursorPos(ref Vector2 vec)
        {
            Point p = new Point();
            bool suc = GetCursorPos(ref p);
            if (suc)
            {
                vec.X = (float)p.X;
                vec.Y = (float)p.Y;
            }
            return suc;
        }

        public static bool setCursorPos(Vector2 vec)
        {
            return SetCursorPos((int)vec.X, (int)vec.Y);
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
            p.Y = p.Y + (int)vec.Y;
        }


        public static Point toPoint(Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }
    }


    static class Drawer
    {
        static IntPtr desktopPtr;
        public static Graphics g;
        public static System.Drawing.Drawing2D.GraphicsContainer ctnr;

        static Pen pen = Pens.Red;
        static Brush brush = Brushes.White;
        static Pen penB = Pens.DarkRed;
        //static SolidBrush brushA = new SolidBrush(Color.DarkGray);
        //static SolidBrush brushB = new SolidBrush(Color.DarkRed);
        //static Pen penA = new Pen(brushA);
        //static Pen penB = new Pen(brushB);

        public static void init()
        {
            desktopPtr = Interop.GetDC(IntPtr.Zero);
            g = Graphics.FromHdc(desktopPtr);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        public static void start()
        {
            //desktopPtr = Interop.GetDC(IntPtr.Zero);
            //g = Graphics.FromHdc(desktopPtr);
            //ctnr = g.BeginContainer();
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.Clear(Color.Transparent);
        }

        public static void end()
        {
            //g.EndContainer(ctnr);
            //g.Dispose();
        }

        public static void drawLine(Vector2 start, Vector2 end)
        {
            g.DrawLine(pen, MathUtil.toPoint(start), MathUtil.toPoint(end));
        }

        public static void drawCircle(Vector2 pos, float radius)
        {
            g.DrawEllipse(pen, pos.X - radius/2.0f, pos.Y - radius/2.0f, radius, radius);
        }

        public static void drawDot(Vector2 pos, float radius)
        {
            g.FillEllipse(brush, pos.X - radius / 2.0f, pos.Y - radius / 2.0f, radius, radius);
        }

        public static void drawRope(Vector2 start, Vector2 end, float length)
        {
            /* Bezier length approximation https://stackoverflow.com/questions/29438398/cheap-way-of-calculating-cubic-bezier-length
            ** Bezier bezier = Bezier (p0, p1, p2, p3);
            ** chord = (p3-p0).Length;
            ** cont_net = (p0 - p1).Length + (p2 - p1).Length + (p3 - p2).Length;
            ** app_arc_length = (cont_net + chord) / 2
            ** --> rope   = spanning quad / 2
            ** --> rope*2 = spanning quad
            **
            ** Inverse problem: find controll points for given length:
            ** our spanning quad is a rectangle, we know the width = dist, we seek the height:
            ** --> spanning rectangle = (2*width + 2*height)
            ** --> rope*2 = spanning rectangle
            ** --> rope*2 = (2*dist + 2*height)
            ** --> (rope*2 - dist*2) /2 = height
            ** --> rope - dist = height
            */

            float diff = Vector2.Distance(start, end);
            float hangtrough = (float)Math.Max(0.0,length - diff)*0.666f; //empiric factor that makes it work
            g.DrawBezier(pen, start.X, start.Y, start.X, start.Y + hangtrough, end.X, end.Y + hangtrough, end.X, end.Y);
        }

        public static void draw()
        {

        }

        public static void redrawBounds(Vector2 TL, Vector2 BR)
        {
            //g.FillRectangle(Brushes.Transparent, TL.X, TL.Y, BR.X - TL.X, BR.Y - TL.Y);
        }

        public static void redrawBounds(float X, float Y, float width, float height)
        {
            //g.FillRectangle(Brushes.Transparent, X, Y, width, height);
            g.Flush();
        }
    }

    static class SPMouse
    {
        static float maxdist = 128.0f;

        public static Vector2 previousMousePos = new Vector2();
        public static Vector2 currentMousePos = new Vector2();
        public static Vector2 mouseDelta = new Vector2();

        public static Vector2 cursorPos = new Vector2();
        public static Vector2 draggerPos = new Vector2();
        public static Vector2 towDelta = new Vector2();
        public static Vector2 towDir = new Vector2();
        public static float towDist;
        public static float towTension;


        static SPMouse()
        {

        }

        public static void init()
        {
            Drawer.init();

            Interop.getCursorPos(ref previousMousePos);
            Interop.getCursorPos(ref currentMousePos);
            Interop.getCursorPos(ref cursorPos);
            Interop.getCursorPos(ref draggerPos);
        }

        public static bool updatePositions()
        {       
            Interop.getCursorPos(ref currentMousePos);
            bool changed = (previousMousePos != currentMousePos);
            mouseDelta = currentMousePos - previousMousePos;

            draggerPos += mouseDelta;

            return changed;
        }

        public static void applyCursorManipulation()
        {
            Interop.getCursorPos(ref currentMousePos);
            mouseDelta = currentMousePos - previousMousePos;

            draggerPos += mouseDelta;

            towDelta = draggerPos - cursorPos;
            towDist = towDelta.Length();
            if (towDist > 0.0)
            {
                towDir = Vector2.Normalize(towDelta);
            }
            else
            {

                towDir = Vector2.Zero;
            }
            towTension = (float)(Math.Max(0.0,towDist - maxdist));

            cursorPos += Vector2.Multiply(towTension, towDir);

            Interop.setCursorPos(cursorPos);
            Interop.getCursorPos(ref previousMousePos);
        }

        public static void draw()
        {
            Drawer.start();
            //Drawer.redrawBounds(0, 0, 1000, 1000);
            //Drawer.drawLine(cursorPos, draggerPos);
            Drawer.drawRope(cursorPos, draggerPos, maxdist);
            Drawer.drawCircle(cursorPos, 9.0f);
            Drawer.drawCircle(draggerPos, 12.0f);
            Drawer.drawDot(draggerPos, 6.0f);
            Drawer.redrawBounds(0, 0, 1000, 1000);
            Drawer.end();

        }

        public static void debug()
        {
            Console.WriteLine("Mouse: {0},{1}", cursorPos.X, cursorPos.Y);
            Console.WriteLine("Dragr: {0},{1}", draggerPos.X, draggerPos.Y);
            Console.WriteLine("dist: {0}", towDist);
            Console.WriteLine("tension: {0}", towTension);
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
                if (SPMouse.updatePositions())
                {
                    //SceenUtil.test();
                    SPMouse.applyCursorManipulation();
                    //SPMouse.debug();
                }
                SPMouse.draw();
            }
        }
    }
}
