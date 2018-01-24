using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PixelVillage.Main
{
    class C
    {

        public static int MX, MY;
        public static bool M_RIGHTDOWN, M_LEFTDOWN;

        public const int PLAYER_WIDTH = 64;
        public const int PLAYER_HEIGHT = 64;
        public const int PLAYER_SPEED = 7;
        public const int PLAYER_JUMP = 15;
        public const int PLAYER_INTERACT_RANGE = 64;

        // These mean player can only move X percent across/down/up the canvas before camera starts following
        public const double PLAYER_MAX_ACROSS = 0.7;
        public const double PLAYER_MIN_ACROSS = 0.2;
        public const double PLAYER_MAX_DOWN = 0.6;
        public const double PLAYER_MIN_DOWN = 0.4;

        public const int TILE_WIDTH = 64;
        public const int TILE_HEIGHT = 64;

        public const int WORLD_WIDTH = 1200;
        public const int WORLD_HEIGHT = 700;

        public const int GRAVITY = 5;
        public const int GRAVITY_MULTI = 1;

        public static Font TFont = new Font("ariel", 8);
        public static Font SFont = new Font("ariel", 12);
        public static Font NFont = new Font("ariel", 16);
        public static Font MFont = new Font("ariel", 24);
        public static Font LFont = new Font("ariel", 32);

        public static Random RDM;

        static C()
        {
            RDM = new Random();
        }

        public static int round(int i, int m)
        {
            return m * (i / m);
        }

        public static decimal round(decimal i, decimal m)
        {
            return m * (i / m);
        }

        private static StringFormat cformat = new StringFormat();
        public static StringFormat GetCenterFormat()
        {
            cformat.LineAlignment = StringAlignment.Center;
            cformat.Alignment = StringAlignment.Center;
            return cformat;
        }

        private static StringFormat cyrxformat = new StringFormat();
        public static StringFormat GetCenterYRightXFormat()
        {
            cyrxformat.LineAlignment = StringAlignment.Center;
            cyrxformat.Alignment = StringAlignment.Near;
            return cyrxformat;
        }

        private static StringFormat cxformat = new StringFormat();
        public static StringFormat GetCenterXFormat()
        {
            cxformat.Alignment = StringAlignment.Center;
            return cxformat;
        }

    }
}
