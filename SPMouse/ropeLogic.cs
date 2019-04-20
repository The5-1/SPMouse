using System;
using System.Numerics;
using System.Drawing;

namespace SPMouse
{
    public class RopeLogic
    {
        private Vector2 cursorPos;
        private Vector2 pullPos;

        public Point cursorPoint;
        public Point pullPoint;

        private Vector2 ropeDelta;
        private Vector2 ropeDir;
        private float ropeDist;
        private float ropeTension;


        public void update(int x, int y, int dx, int dy)
        {
            update(new Vector2(x, y), new Vector2(dx,dy));
        }

        public void start(Vector2 mouseIn)
        {
            cursorPos = mouseIn;
            pullPos = mouseIn;
            //Console.WriteLine("rope reset");
        }

        public void update(Vector2 mouseIn, Vector2 mouseDelta)
        {
            Console.WriteLine("delta: x{0} y{1}", mouseDelta.X, mouseDelta.Y);

            pullPos += mouseDelta;
            ropeDelta = pullPos - cursorPos;

            ropeDist = ropeDelta.Length();
            ropeDir = Vector2.Zero; //normalization is not save and would fail for lenght = 0, so catch that case here
            if (ropeDist > 0.0)
            {
                ropeDir = Vector2.Normalize(ropeDelta);
            }

            ropeTension = (float)(Math.Max(0.0, ropeDist - SPMouse.settings.sRopeLength));
            //Console.WriteLine("dist: " + ropeDist + " tension: " + ropeTension);

            cursorPos += Vector2.Multiply(ropeTension, ropeDir);

            cursorPoint.X = (int)cursorPos.X;
            cursorPoint.Y = (int)cursorPos.Y;

            pullPoint.X = (int)pullPos.X;
            pullPoint.Y = (int)pullPos.Y;
        }
    }
}