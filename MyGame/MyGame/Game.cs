﻿using System;
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
        public static event Action<string> WriteMessage;

        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        
        public static Bitmap bg;
        
        public static int Width { get; set; }
        public static int Height { get; set; }

        private static Timer timer = new Timer { Interval = 100 };
        public static Random rnd;

        public static Asteroid[] _asteroids;
        public static Medpack _medpack;
        public static BaseObject[] _stars;
        public static List<Bullet> _bullets;
        public static Starship _ship;

        static Game()
        {
            bg = Properties.Resources.Universe;
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
        /// <param name="form">Форма для вывода графики.</param>
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
            
            timer.Start();
            timer.Tick += Timer_Tick;

            form.KeyDown += Form_KeyDown;
            Starship.MessageDie += Finish;
            WriteMessage += LogToConsole;
        }
        /// <summary>
        /// Обработчик для управления кораблем.
        /// </summary>
        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullets.Add(new Bullet(new Point(_ship.Rect.Right + 10, _ship.Rect.Height / 2 + _ship.Rect.Y), new Point(4, 0), new Size(4, 1)));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
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
            Buffer.Graphics.Clear(Color.Transparent);
            
            Buffer.Graphics.DrawImage(bg, new Rectangle(0, 0, Width, Height));
            foreach (Asteroid a in _asteroids)
                a?.Draw();
            foreach (Star obj in _stars)
                obj.Draw();
            _ship?.Draw();
            _medpack?.Draw();
            foreach (Bullet b in _bullets)
            {
                b?.Draw();
            }
            if (_ship != null)
            {
                Buffer.Graphics.DrawString("Energy:" + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
                Buffer.Graphics.DrawString("Score:" + _ship.Score, SystemFonts.DefaultFont, Brushes.White, 0, 15);
            }
            Buffer.Render();
        }
        /// <summary>
        /// Инициализация объектов.
        /// </summary>
        public static void Load()
        {
            _bullets = new List<Bullet>();
            _medpack = new Medpack(new Point(rnd.Next(0, Width), rnd.Next(0, Height - 10)), new Point(rnd.Next(-10, -5), 0), rnd.Next(15, 48));
            _asteroids = new Asteroid[5];
            _stars = new Star[50];
            for (int i = 0; i < _asteroids.Length; i++)
                _asteroids[i] = new Asteroid(new Point(rnd.Next(0, Width), rnd.Next(0, Height - 10)), new Point(rnd.Next(-15, -10), 0), rnd.Next(15, 48));
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = new Star(new Point(rnd.Next(0, Width), rnd.Next(0, Height)), new Point(rnd.Next(-4, -1), 0), rnd.Next(4, 16));
            _ship = new Starship(new Point(100, Height / 2 - 50), new Point(0, 10), new Size(64, 30));
        }
        /// <summary>
        /// Завершение игры.
        /// </summary>
        public static void Finish()
        {
            timer.Stop();
            Buffer.Graphics.DrawString("Game Over", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.White, 200, 100);
            Buffer.Render();
        }
        /// <summary>
        /// Вывод сообщения в консоль.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }
        /// <summary>
        /// Изменение состояния объектов, проверка столкновений объектов.
        /// </summary>
        public static void Update()
        {
            foreach (Star obj in _stars)
                obj.Update();
            foreach (Bullet b in _bullets)
                b?.Update();
            for (var i = 0; i < _asteroids.Length; i++)
            {
                if (_asteroids[i] == null) continue;
                _asteroids[i].Update();
                for (var j = 0; j< _bullets.Count; j++)
                {
                    if (_bullets[j] != null && _bullets[j].Collision(_asteroids[i]))
                    {
                        System.Media.SystemSounds.Hand.Play();
                        _asteroids[i].Regenerate();
                        _ship.ScoreChange(1);
                        WriteMessage("Asteroid destroyed");
                        _bullets[j] = null;
                        continue;
                    }
                }
                if (_ship.Collision(_asteroids[i]))
                {
                    _ship?.EnergyChange(_asteroids[i].Power);
                    _asteroids[i].Regenerate();
                    WriteMessage("Ship collision");
                    System.Media.SystemSounds.Asterisk.Play();
                    if (_ship.Energy <= 0) _ship.Die();
                }
            }
            _medpack?.Update();
            if (_ship.Collision(_medpack))
            {
                _ship?.EnergyChange(_medpack.Power);
                _medpack = new Medpack(new Point(rnd.Next(0, Width), rnd.Next(0, Height - 10)), new Point(rnd.Next(-10, -5), 0), rnd.Next(15, 48));
                _medpack.Regenerate();
                System.Media.SystemSounds.Asterisk.Play();
                WriteMessage("Picked medpack");
            }
            _bullets.RemoveAll(item => item == null);
            _bullets.RemoveAll(item => item.Away == true);
        }
    }
}
