using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace MyGame
{
    /// <summary>
    /// Класс записей журнала.
    /// </summary>
    [Serializable]
    public class Note
    {
        public string Date { get; set; }

        public string Time { get; set; }

        public string Message { get; set; }

        public string Score { get; set; }

        public string Energy { get; set; }

        public Note()
        {
        }

        public Note(DateTime now, string message, string score, string energy)
        {
            this.Date = now.ToString("dd.MM.yyyy");
            this.Time = now.ToString("HH:mm:ss");
            this.Message = message;
            this.Score = score;
            this.Energy = energy;
        }
    }
    /// <summary>
    /// Класс для создания XML-файла журнала.
    /// </summary>
    class NotesToXML
    {
        public List<Note> List { get; private set; }

        public string FileName { set; private get; }

        public NotesToXML(string filePath)
        {
            this.FileName = Path.Combine(filePath,"log.XML");
            List = new List<Note>();
        }

        public void Add(DateTime now, string message, string score, string energy)
        {
            List.Add(new Note(now, message, score, energy));
        }

        public void Save()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<Note>));
            Stream fStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            xmlFormat.Serialize(fStream, List);
            fStream.Close();
        }
    }
}
