using System;
using System.Numerics;
using System.Drawing;

namespace SPMouse
{
    public class RopeLogic
    {
        public float ropeLength = 32f;

        private bool initialized = false;
        private Vector2 cursorPos;
        private Vector2 pullPos;
        private Vector2 newIn;
        private Vector2 prevIn;
        private Vector2 mouseDelta;
        private Vector2 ropeDelta;
        private Vector2 ropeDir;
        private float ropeDist;
        private float ropeTension;

        public void update(Vector2 mouseIn)
        {
            if (!initialized)
            {
                cursorPos = mouseIn;
                pullPos = mouseIn;
                newIn = mouseIn;
                prevIn = mouseIn;
                initialized = true;
            }

            newIn = mouseIn;

            Vector2 mouseDelta = newIn - prevIn;

            pullPos += mouseDelta;
            ropeDelta = pullPos - cursorPos;

            ropeDist = ropeDelta.Length();
            ropeDir = Vector2.Zero; //normalization is not save and would fail for lenght = 0, so catch that case here
            if (ropeDist > 0.0)
            {
                ropeDir = Vector2.Normalize(ropeDelta);
            }

            ropeTension = (float)(Math.Max(0.0, ropeDist - ropeLength));

            cursorPos += Vector2.Multiply(ropeTension, ropeDir);
        }

        public static Point toPoint(Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static Vector2 toVec(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public Point getPullPos() {
            return toPoint(pullPos);
        }
        public Point getCursorPos()
        {
            return toPoint(cursorPos);
        }
    }
}