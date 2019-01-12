using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    /// <summary>
    /// Исключение, которое появляется при попытке создать объект с неправильными характеристиками
    /// (например, отрицательные размеры, слишком большая скорость или неверная позиция).
    /// </summary>
    [Serializable]
    public class GameObjectException : ApplicationException
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса GameObjectException.
        /// </summary>
        public GameObjectException() { }
        /// <summary>
        /// Инициализирует новый экземпляр класса GameObjectException с указанным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение, описывающее ошибку.</param>
        public GameObjectException(string message) : base(message) { }
        /// <summary>
        /// Инициализирует новый экземпляр класса GameObjectException указанным сообщением
        /// об ошибке и ссылкой на внутреннее исключение, вызвавшее данное исключение.
        /// </summary>
        /// <param name="message">Сообщение об ошибке, указывающее причину создания исключения.</param>
        /// <param name="inner">Исключение, которое является причиной текущего исключения. Если параметр inner
        /// не является указателем NULL, текущее исключение возникло в блоке catch, обрабатывающем
        /// внутреннее исключение.</param>
        public GameObjectException(string message, ApplicationException inner) : base(message, inner) { }
        /// <summary>
        /// Инициализирует новый экземпляр класса System.ApplicationException с сериализованными данными.
        /// </summary>
        /// <param name="info">Объект, содержащий сериализованные данные объекта.</param>
        /// <param name="context">Контекстные сведения об источнике или назначении.</param>
        protected GameObjectException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
