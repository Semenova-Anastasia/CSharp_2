using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

namespace MyGame
{
    /// <summary>
    /// Класс, описывающий логику игры.
    /// </summary>
    static class Game
    {
        /// <summary>
        /// Событие, инициирующее запись в журнал.
        /// </summary>
        public static event Action<string> WriteMessage;

        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        /// <summary>
        /// Изображения фона.
        /// </summary>
        public static Bitmap bg;
        /// <summary>
        /// Изображение сообщения Game Over.
        /// </summary>
        public static Bitmap gameOver;
        /// <summary>
        /// Изображение сообщения Next Level.
        /// </summary>
        public static Bitmap nextLevel;
        /// <summary>
        /// Свойство, задающее ширину игрового поля.
        /// </summary>
        public static int Width { get; private set; }
        /// <summary>
        /// Свойство, задающее высоту игрового поля.
        /// </summary>
        public static int Height { get; private set; }
        /// <summary>
        /// Свойство, задающее количество астероидов на каждом уровне.
        /// </summary>
        public static int Quantity { get; private set; } = 5;
        /// <summary>
        /// Свойство, показывающее уровень игры в данный момент.
        /// </summary>
        public static int Level { get; private set; } = 1;
        /// <summary>
        /// Таймер.
        /// </summary>
        private static Timer timer = new Timer { Interval = 100 };
        /// <summary>
        /// Время, когда произошло событие.
        /// </summary>
        private static DateTime now;
        /// <summary>
        /// Поле для генерации рандомных чисел.
        /// </summary>
        public static Random rnd;
        /// <summary>
        /// Коллекция астероидов.
        /// </summary>
        public static List<Asteroid> _asteroids;
        /// <summary>
        /// Аптечка.
        /// </summary>
        public static Medpack _medpack;
        /// <summary>
        /// Массив астероидов.
        /// </summary>
        public static BaseObject[] _stars;
        /// <summary>
        /// Коллекция снарядов.
        /// </summary>
        public static List<Bullet> _bullets;
        /// <summary>
        /// Корабль.
        /// </summary>
        public static Starship _ship;
        /// <summary>
        /// Путь к текущей директории.
        /// </summary>
        private static string filePath;
        /// <summary>
        /// Журнал.
        /// </summary>
        public static NotesToXML log;
        /// <summary>
        /// Конструктор класса Game.
        /// </summary>
        static Game()
        {
            bg = Properties.Resources.Universe;
            gameOver = Properties.Resources.GameOver;
            nextLevel = Properties.Resources.NextLevel;
            filePath = Directory.GetCurrentDirectory();
            log = new NotesToXML(filePath);
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
            WriteMessage += LogToXML;
        }
        /// <summary>
        /// Обработчик клавиш управления кораблем.
        /// </summary>
        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullets.Add(new Bullet(new Point(_ship.Rect.Right + 10, _ship.Rect.Height / 2 + _ship.Rect.Y), new Point(4, 0), new Size(4, 1)));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
        }
        /// <summary>
        /// Обработчик таймера.
        /// </summary>
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
                Buffer.Graphics.DrawString("Level:" + Level, SystemFonts.DefaultFont, Brushes.White, 0, 30);
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
            _asteroids = new List<Asteroid>();
            _stars = new Star[50];
            for (int i = 0; i < Quantity; i++)
                _asteroids.Add(new Asteroid(new Point(rnd.Next(0, Width), rnd.Next(0, Height - 10)), new Point(rnd.Next(-15, -10), 0), rnd.Next(15, 48)));
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
            DrawMessage(gameOver);
        }
        /// <summary>
        /// Вывод сообщения на экран.
        /// </summary>
        /// <param name="im">Изображение с сообщением.</param>
        public static void DrawMessage(Bitmap im)
        {
            int w = Game.Width / 2;
            int h = w / 5;
            int x = (Game.Width / 2) - (w / 2);
            int y = (Game.Height / 2) - (h / 2) - 40;
            Buffer.Graphics.DrawImage(im, x, y, w, h);
            Buffer.Render();
        }
        /// <summary>
        /// Вывод сообщения в консоль.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void LogToConsole(string message)
        {
            now = DateTime.Now;
            Console.WriteLine($"{now} : {message}");
        }
        public static void LogToXML(string message)
        {
            now = DateTime.Now;
            log.Add(now, message, _ship.Score.ToString(), _ship.Energy.ToString());
        }
        /// <summary>
        /// Проверка и переход на новый уровень.
        /// </summary>
        public static void NextLevel()
        {
            int n = 0;
            foreach (Asteroid a in _asteroids)
                if (a.Power == 0) n++;
            if (n == _asteroids.Count)
            {
                _asteroids.RemoveAll(item => item.Power == 0);
                Quantity++;
                Level++;
                for (int i = 0; i < Quantity; i++)
                    _asteroids.Add(new Asteroid(new Point(rnd.Next(0, Width), rnd.Next(0, Height - 10)), new Point(rnd.Next(-15, -10), 0), rnd.Next(15, 48)));
                WriteMessage("Next Level!");
                DrawMessage(nextLevel);
                System.Threading.Thread.Sleep(3000);
            }
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
            for (var i = 0; i < _asteroids.Count; i++)
            {
                if (_asteroids[i] == null) continue;
                _asteroids[i].Update();
                for (var j = 0; j< _bullets.Count; j++)
                {
                    if (_bullets[j] != null && _bullets[j].Collision(_asteroids[i]))
                    {
                        System.Media.SystemSounds.Hand.Play();
                        //_asteroids[i].Regenerate();
                        _asteroids[i] = new Asteroid();
                        _ship.ScoreChange(1);
                        WriteMessage("Asteroid destroyed");
                        _bullets[j] = null;
                        continue;
                    }
                }
                if (_ship.Collision(_asteroids[i]))
                {
                    _ship?.EnergyChange(_asteroids[i].Power);
                    //_asteroids[i].Regenerate();
                    _asteroids[i] = new Asteroid();
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
            if (_ship.Energy > 0) NextLevel();
        }
    }
}
