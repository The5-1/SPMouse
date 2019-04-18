using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    static class SceenUtil{

        static Point lastPos = new Point();

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

            SolidBrush b = new SolidBrush(Color.DarkRed);
            g.FillRectangle(b, new Rectangle(0, 0, 128, 128));

            g.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }

        public static bool updatePos()
        {
            Point newPos = new Point();
            GetCursorPos(ref newPos);

            if(newPos != lastPos)
            {
                lastPos = newPos;
                return true;
            }
            else
            {
                return false;
            }
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello: {0}", "test");

            while (true)
            {
                if (SceenUtil.updatePos())
                {
                    SceenUtil.test();
                }
            }
        }
    }
}
