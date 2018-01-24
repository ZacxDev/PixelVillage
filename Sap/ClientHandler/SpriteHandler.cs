using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PixelVillage.GameSprite;
using System.Drawing;
using System.Windows.Forms;
using PixelVillage.GameWorld;
using System.Diagnostics;
using PixelVillage.UI;
using PixelVillage.Main;

namespace PixelVillage.ClientHandler
{
    // This file gets all user input passed into it, as well as most game tick/render. And is used to delegate that work
    class SpriteHandler
    {

        public static List<Sprite> sprites = new List<Sprite>();
        public static List<Sprite> spriteRemoveQueue = new List<Sprite>();
        public static List<Sprite> VisableObstructiveSprites = new List<Sprite>();
        public static List<SentiantSprite> sentiants = new List<SentiantSprite>();

        public SpriteHandler()
        {
        }

        public static void tick()
        {

            // Dont't tick stuff on menu
            if (Game.GameState == GameState.MENU)
                return;

            for (int i = 0; i < sentiants.Count; i++)
            {
               // if (sprites[i] is SentiantSprite)
                    sentiants[i].tick();
            }

            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] is FunctionTile)
                    (sprites[i] as FunctionTile).Tick();
            }

            // If world builder enabled, render
            if (WorldBuilder.Enabled)
            {
                Game.WB.Tick();
                return;
            }

        }

        public static void render(ref Graphics g)
        {
            //Debug.WriteLine("Total sprites: " + sprites.Count);
            //Debug.WriteLine("Rendered sprites: " + VisableSprites.Count);
            //Debug.WriteLine("---------------------");

            // Clean up sprites
            for (var i = 0; i < spriteRemoveQueue.Count; i++)
            {
                sprites.Remove(spriteRemoveQueue[i]);
            }
            spriteRemoveQueue.Clear();

            // if not in world builder, Check which sprites are visable, if they are, add them to render list, else don't bother
            if (!WorldBuilder.Enabled)
            {
                var obsructives = new List<Sprite>();
                //VisableObstructiveSprites.Clear();
                for (int i = 0; i < sprites.Count; i++)
                {
                    if (sprites[i].GetBounds().IntersectsWith(new Rectangle((int)-g.Transform.OffsetX, (int)-g.Transform.OffsetY, Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT)))
                    {
                        sprites[i].Render(ref g);
                        if ((sprites[i] is FunctionTile && !(sprites[i] as FunctionTile).isHarvested()) || sprites[i] is StructureTile)
                            obsructives.Add(sprites[i]);
                    }
                }
                VisableObstructiveSprites = obsructives;
            }

            // Render the forcused sprite after tiles so that border will show properly (tile will be rendered twice)
            Tile.RenderFocusedTile(ref g);
            // Render sentiants last so they are above everything
            for (int i = 0; i < sentiants.Count; i++)
            {
                sentiants[i].Render(ref g);
            }

            // If world builder enabled, render
            if (WorldBuilder.Enabled)
            {
                Game.WB.Render(ref g);
                return;
            }
        }

        public static void clickDown(MouseEventArgs e)
        {
            Rectangle click = new Rectangle(e.X + Game.Camera.camX, e.Y + Game.Camera.camY, 1, 1);

            if (Game.GameState == GameState.MENU)
            {
                Game.MainMenu.mouseDown(click);
                return;
            }

            // If world builder enabled, pass click directly
            if (WorldBuilder.Enabled)
            {
                Game.WB.ClickedDown(click);
                return;
            }

            // pass into hud
            if (click.IntersectsWith(Game.Hud.GetCamRelBounds()))
            {
                Game.Hud.MouseDown(click);
                return;
            }

            //update player's movement target
            Game.P.SetTarget(click);

            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite s = SpriteHandler.sprites[i];
                if (s is UISprite)
                {
                    if (click.IntersectsWith(s.GetBounds()))
                    {
                        (s as UISprite).ClickedDown(click);
                        // return once we pass click to worldbuilder, if it's enabled
                        if (WorldBuilder.Enabled)
                            return;
                    }
                }
                else if ((!Game.P.GetIsWorking()) && s is FunctionTile)
                {
                    if (click.IntersectsWith(s.GetBounds()))
                    {
                        (s as FunctionTile).ClickedDown();
                        // move this sprite to the end of array so it is rendered last and harvest animation will be overtop other tiles
                        sprites.Add(sprites[i]);
                        sprites.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public static void clickUp(MouseEventArgs e)
        {
            // cancel player harvest when click up
            if (Game.P.GetIsWorking())
                Game.P.SetIsWorking(false);
            // Stop player tracking target
            Game.P.SetIsTracking(false);

            Rectangle click = new Rectangle(e.X + Game.Camera.camX, e.Y + Game.Camera.camY, 1, 1);

            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite s = SpriteHandler.sprites[i];
                if (s is UISprite)
                {
                    if (click.IntersectsWith(s.GetBounds()))
                        (s as UISprite).ClickedUp(click);
                    return;
                }
                else if (s is FunctionTile)
                {
                    if (click.IntersectsWith(s.GetBounds()))
                    {
                        (s as FunctionTile).ClickedUp();
                        return;
                    }
                }
            }
        }

        public static void mouseMove(MouseEventArgs e)
        {
            Rectangle click = new Rectangle(e.X + Game.Camera.camX, e.Y + Game.Camera.camY, 1, 1);

            _GlobalMouseMove(click);

            if (Game.GameState == GameState.MENU)
            {
                Game.MainMenu.mouseMove(click);
                return;
            }

            // If world builder enabled, pass click directly
            if (WorldBuilder.Enabled)
            {
                Game.WB.MouseOver(click);
                // !WARN: this return blocks hover from being passed to anything else :WARN!
                return;
            }

            // Check HUD after WB since it's disabled when WB enabled
            if (click.IntersectsWith(Game.Hud.GetCamRelBounds()))
            {
                Game.Hud.MouseOver(click);
                return;
            }

            //update player's movement target
            if (C.M_LEFTDOWN)
                Game.P.SetTarget(click);

            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite s = SpriteHandler.sprites[i];
                if (s is UISprite)
                {
                    if (click.IntersectsWith(s.GetBounds()))
                    {
                        (s as UISprite).MouseOver(click);
                        // once we find one, return if in worldbuilder, otherwise execute hover on underlying tiles
                        if (WorldBuilder.Enabled)
                            return;
                    }
                } else if (s is Tile)
                {
                    if (click.IntersectsWith(s.GetBounds()))
                    {
                        (s as Tile).MouseOver(click);
                        // once we find one, return
                        return;
                    }
                    // if we don't find one, make no tile focused
                    Tile.SetNoFocus();
                }
            }
        }

        // This function is for mouse move stuff that will remain the same regardless of gamestate
        private static void _GlobalMouseMove(Rectangle click)
        {
            // Update button focus
            foreach (var b in UI.Button.AllButtons)
            {
                if (click.IntersectsWith(b.GetCamRelBounds()))
                {
                    b.MouseOver();
                    return;
                }
            }
            UI.Button.UnFocusAll();
        }

    }
}
