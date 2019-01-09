using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MyGame
{
    /// <summary>
    /// Класс, описывающий логику игры.
    /// </summary>
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        
        public static Bitmap bg;
        
        public static int Width { get; set; }
        public static int Height { get; set; }

        public static Random rnd;

        public static Asteroid[] _asteroids;
        public static BaseObject[] _stars;
        public static Bullet _bullet;
        public static Starship _ship;

        static Game()
        {
        }
        /// <summary>
        /// Позволяет пользователю задать разрешение экрана.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Resolution()
        {
            Regex regexResolution = new Regex(@"[-]*\d{3,4}");
            string userWidth, userHeight;
            bool correct = false;
            do
            {
                Console.WriteLine("Задайте размер экрана");
                Console.Write("Ширина:");
                userWidth = Console.ReadLine();
                Console.Write("Высота:");
                userHeight = Console.ReadLine();
                if (regexResolution.IsMatch(userWidth) && regexResolution.IsMatch(userHeight))
                {
                    Width = int.Parse(userWidth);
                    Height = int.Parse(userHeight);
                    if (Width > 1000 || Height > 1000 || Width <= 0 || Height <= 0)
                        throw new ArgumentOutOfRangeException();
                    else correct = true;
                }
                else Console.WriteLine("Некорректный ввод!");
            }
            while (!correct);
        }
        /// <summary>
        /// Инициализация графики и связывание её с формой.
        /// </summary>
        /// <param name="form">Форма для вывода графики</param>
        public static void Init(Form form)
        {
            Graphics g;

            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();

            rnd = new Random();

            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));
            Load();

            Timer timer = new Timer { Interval = 100 };
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }
        /// <summary>
        /// Вывод всех объектов на экран.
        /// </summary>
        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            bg = Properties.Resources.Universe;
            Buffer.Graphics.DrawImage(bg, new Rectangle(0, 0, Width, Height));
            foreach (Asteroid obj in _asteroids)
                obj.Draw();
            foreach (Star obj in _stars)
                obj.Draw();
            _ship.Draw();
            _bullet.Draw();
            Buffer.Render();
        }
        /// <summary>
        /// Инициализация объектов.
        /// </summary>
        public static void Load()
        {
            _bullet = new Bullet(new Point(0, 200), new Point(5, 0), new Size(4, 1));
            _asteroids = new Asteroid[5];
            _stars = new Star[50];
            for (int i = 0; i < _asteroids.Length; i++)
                _asteroids[i] = new Asteroid(new Point(rnd.Next(0, Width), rnd.Next(0, Height - 10)), new Point(rnd.Next(-15, -10), 0), rnd.Next(15, 48));
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = new Star(new Point(rnd.Next(0, Width), rnd.Next(0, Height)), new Point(rnd.Next(-4, -1), 0), rnd.Next(4, 16));
            _ship = new Starship(new Point(100, Height / 2 - 50), new Point(0, 10), 100);
        }
        /// <summary>
        /// Изменение состояния объектов.
        /// </summary>
        public static void Update()
        {
            foreach (Star obj in _stars)
                obj.Update();
            foreach (Asteroid a in _asteroids)
            {
                a.Update();
                if (a.Collision(_bullet))
                {
                    System.Media.SystemSounds.Hand.Play();
                    a.Regenerate();
                    _bullet.Regenerate();
                }
            }
            _ship.Update();
            _bullet.Update();
        }
    }
}
