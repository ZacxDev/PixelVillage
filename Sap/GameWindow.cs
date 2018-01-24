using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PixelVillage.Main;

namespace PixelVillage
{
    public partial class GameWindow : Form
    {
        private Game game = new Game();
        private bool _Running;

        public GameWindow()
        {
            InitializeComponent();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (_Running)
                return;
            Graphics g = canvas.CreateGraphics();
            //Graphics g = Graphics.FromImage(new Bitmap(Game.CANVAS_WIDTH, Game.CANVAS_HEIGHT));
            game.startGraphics(g);
            _Running = true;
        }

        private void GameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            game.stopGame();
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            //broken
            //AllocConsole();
            Console.WriteLine("dddd");
        }

        //allows cmd line to be seen
        [DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAsAttribute(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
//            System.Diagnostics.Debug.WriteLine("die");
            KeyInput.HandleKeyDown(e);
        }

        private void GameWindow_KeyUp(object sender, KeyEventArgs e)
        {
            KeyInput.HandleKeyUp(e);
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            MouseInput.HandleMouseDown(e);
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            MouseInput.HandleMouseUp(e);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            MouseInput.HandleMouseMove(e);
        }

        private void GameWindow_ResizeEnd(object sender, EventArgs e)
        {
            MouseInput.HandleResize(this);
            game.gEngine.update(canvas.CreateGraphics());
        }
    }
}
