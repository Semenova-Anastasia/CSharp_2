using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.ApplicationExit += HandleAppExit;
            Game.Resolution();
            Form form = new Form
            {
                Width = Game.Width,
                Height = Game.Height
            };
            Game.Init(form);
            form.Show();
            Game.Load();
            Game.Draw();
            Application.Run(form);
        }
        static void HandleAppExit(object sender, EventArgs e)
        {
            // Обработчик события Application.ApplicationExit
            Game.log.Save();
        }
    }
}