using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkopovKursov_var29.Models
{
    internal static class Loger
    {
        public const string FileName = "Log.txt";
        private static string _directory = null;
        /// <summary>
        /// Директория в которой находится файл логирования.
        /// </summary>
        public static string Directory
        {
            get => _directory;
            set => _directory = value;
        }

        /// <summary>
        /// Начальное сообщение логирования. Указывает текущее время и добавляет переданное сообщение
        /// </summary>
        /// <param name="message"></param>
        public static void StartLog(string message)
        {
            message = "\t" + message;
            using (StreamWriter writer = File.AppendText(Directory + FileName))
                writer.WriteLine("---" + DateTime.Now + message + "---");
        }

        /// <summary>
        /// Логирует переданную строку.
        /// </summary>
        /// <param name="message"></param>
        public static void MessageLog(string message)
        {
            using (StreamWriter writer = File.AppendText(Directory + FileName))
                writer.Write(message);
        }

        /// <summary>
        /// Делает отступ. Принимает количество строк, количество символов в строке, используемый при заполнении символ.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="charsInRow"></param>
        /// <param name="character"></param>
        public static void LogIndent(int rows, int charsInRow, char character)
        {
            using (StreamWriter writer = File.AppendText(Directory + FileName))
            {
                for (int i = 0; i < rows; i++)
                    writer.WriteLine(new string(character, charsInRow));
            }
        }

        /// <summary>
        /// Логирует в виде таблицы. Принимает массив данных, количество значений в строке, количество выделенных символов под значение.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="valuesInRow"></param>
        /// <param name="charsInValue"></param>
        public static void LogTable<T>(T[] data, int valuesInRow, int charsInValue)
        {
            using (StreamWriter writer = File.AppendText(Directory + FileName))
            {
                string str = null;
                int j = 0;

                for (int i = 0; i < data.Length; i++)
                {
                    if (j == valuesInRow)
                    {
                        writer.WriteLine(str);
                        str = null;
                        j = 0;
                    }

                    str += data[i].ToString().PadRight(charsInValue);
                    j++;
                }

                writer.WriteLine(str);
            }
        }


        /// <summary>
        /// Очистка лога.
        /// </summary>
        /// <returns></returns>
        public static bool ClearLog()
        {
            try
            {
                File.Create(Directory + FileName);
                return true;
            }
            catch { return false; }
        }
    }
}
