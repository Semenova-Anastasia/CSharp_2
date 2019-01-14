using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    /// <summary>
    /// Базовый класс объектов.
    /// </summary>
    /// <exception cref="GameObjectException"></exception>
    abstract class BaseObject : ICollision
    {
        public delegate void Message();
        //public Action<string> WriteMessage;

        protected Point Pos;
        protected Point Dir;
        protected Size Size;
        
        protected BaseObject(Point pos, Point dir, Size size)
        {
            if (pos.X < 0 || pos.Y < 0) throw new GameObjectException("Позиция объекта меньше нуля");
            if (dir.X > 50 || dir.Y > 50) throw new GameObjectException("Слишком большая скорость");
            if (size.Width < 0 || size.Height < 0) throw new GameObjectException("Размер объекта меньше нуля");
            Pos = pos;
            Dir = dir;
            Size = size;
        }

        protected BaseObject(Point pos, Point dir, int size)
        {
            if (pos.X < 0 || pos.Y < 0) throw new GameObjectException("Позиция объекта меньше нуля");
            if (dir.X > 50 || dir.Y > 50) throw new GameObjectException("Слишком большая скорость");
            if (size < 0) throw new GameObjectException("Размер объекта меньше нуля");
            Pos = pos;
            Dir = dir;
            Size = new Size(size, size);
        }

        protected BaseObject()
        {
        }
        /// <summary>
        /// Вывод объекта на экран.
        /// </summary>
        public abstract void Draw();
        /// <summary>
        /// Изменение состояния объекта.
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// Изменение позиции объекта на экране.
        /// </summary>
        public virtual void Regenerate()
        {
            Pos.Y = Game.rnd.Next(0, Game.Height - 40 + Size.Height);
        }
        /// <summary>
        /// Определение столкновения двух объектов.
        /// </summary>
        /// <param name="o">Объект, с которым проверяется столкновение.</param>
        /// <returns></returns>
        public bool Collision(ICollision o) => o.Rect.IntersectsWith(this.Rect);
        public Rectangle Rect => new Rectangle(Pos, Size);
    }
}
