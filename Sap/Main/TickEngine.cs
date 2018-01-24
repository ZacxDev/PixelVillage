using PixelVillage.ClientHandler;
using PixelVillage.GameSprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using PixelVillage.GameWorld;

namespace PixelVillage.Main
{
    class TickEngine
    {

        private Thread tickThread;

        public void Init()
        {
            tickThread = new Thread(new ThreadStart(tick));
            tickThread.Start();
        }

        public void stop()
        {
            tickThread.Abort();
        }

        public void tick()
        {
            while (true)
            {
                GEngine.waitHandle.WaitOne();
                //Debug.WriteLine("tockk");
                Game.Camera.tick();

                SpriteHandler.tick();
                
                //timeout between ticks
                Thread.Sleep(50);
            }
        }

    }
}
