using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using PixelVillage.ClientHandler;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using PixelVillage.Main;
using PixelVillage.UI;
using PixelVillage.GameSprite;

namespace PixelVillage
{
    class GEngine
    {
        public static EventWaitHandle waitHandle = new ManualResetEvent(initialState: true);
        public static bool shouldRecalc;

        //Members
        public Graphics drawHandle;
        public Bitmap frame;
        private Thread renderThread;


        public GEngine(Graphics g) {
            update(g);
        }

        public void init()
        {
            renderThread = new Thread(new ThreadStart(render));
            renderThread.Start();
        }

        public void stop()
        {
            renderThread.Abort();
        }

        public static void pause()
        {
            waitHandle.Reset();
        }

        public static void resume()
        {
            waitHandle.Set();
        }

        public void update(Graphics g)
        {
            drawHandle = g;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            frame = new Bitmap(Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT);
        }

        // recalculate all :scalable w/h/x/y
        public static void TRIGGER_GRAPHICS_RECALC()
        {
            shouldRecalc = true;
        }

        private void render()
        {

            int framesRendered = 0;
            long startTime = Environment.TickCount;
            long endTime = 0;

            frame = new Bitmap(Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT);

            
            while (true)
            {

                if (shouldRecalc)
                {
                    Scalable.UpdateAllScaleAndPosition();
                    shouldRecalc = false;
                }

                //Debug.WriteLine("tick");
                waitHandle.WaitOne();
                Graphics frameGr = Graphics.FromImage(frame);

                if (Game.GameState == GameState.GAME)
                {
                    //background
                    frameGr.FillRectangle(Brushes.Aqua, 0, 0, Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT);

                    frameGr.TranslateTransform(-Game.Camera.camX, -Game.Camera.camY);

                    //render world
                    Game.World.render(ref frameGr);

                    //render sprites
                    SpriteHandler.render(ref frameGr);

                    //render HUD
                    Game.Hud.render(ref frameGr);

                    //frameGr.TranslateTransform(Game.Camera.camX, Game.Camera.camY);
                }
                else if (Game.GameState == GameState.MENU)
                {
                    _RenderMainMenu(ref frameGr);
                }
                else if (Game.GameState == GameState.LOADING)
                {
                    frameGr.FillRectangle(Brushes.White, 0, 0, Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT);
                    frameGr.DrawString("Loading...", C.MFont, Brushes.Black, Game.X_MID - 32, Game.Y_MID);
                }
                //draw frame
                drawHandle.DrawImage(frame, 0, 0);
                
                frameGr.Dispose();
                
                //benchamarking
                framesRendered++;
                if (Environment.TickCount >= startTime + 1000)
                {
                    //Debug.WriteLine("GEngine: " + framesRendered + " fps");
                    framesRendered = 0;
                    startTime = Environment.TickCount;
                }

            }
        }

        
        private void _RenderMainMenu(ref Graphics g)
        {                
            Game.MainMenu.render(ref g);
        }

    }
}
