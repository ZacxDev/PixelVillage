using PixelVillage.GameSprite;
using PixelVillage.Main;
using System.Drawing;
using System.Diagnostics;
using PixelVillage.GameWorld;
using PixelVillage.UI;

namespace PixelVillage
{
    public enum GameState { GAME, MENU, LOADING }

    class Game
    {

        //CONSTANTS
        public static int CANVAS_WIDTH = 1200;
        public static int CANVAS_HEIGHT = 700;
        public static int WORLD_WIDTH = 2400;
        public static int WORLD_HEIGHT = 1400;
        public static int X_MID = CANVAS_WIDTH / 2;
        public static int Y_MID = CANVAS_HEIGHT / 2;

        public static Player P;
        public static World World;
        public static WorldBuilder WB;
        public static Camera Camera;
        public static MainMenu MainMenu;
        public static HUD Hud;
        public static GameState GameState = GameState.MENU;

        public GEngine gEngine;
        private TickEngine tEngine;

        public Game()
        {
            //FontFamily[] ffArray = FontFamily.Families;
            //foreach (FontFamily ff in ffArray)
            //{
            //    Debug.WriteLine(ff.Name);
            //}
            new WorldHelper();
            World = new World("temp_world");
            P = new Player();
            WB = new WorldBuilder(new Bitmap(PVResources.DEAD_IMAGE));
            Camera = new Camera(P);
            MainMenu = new MainMenu();
            Hud = new HUD();
        }

        public void startGraphics(Graphics g) {
            gEngine = new GEngine(g);
            gEngine.init();

            tEngine = new TickEngine();
            tEngine.Init();
        }

        public void stopGame()
        {
            gEngine.stop();
            tEngine.stop();
        }

    }
}
