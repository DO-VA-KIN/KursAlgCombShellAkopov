using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkopovKursov_var29.Models
{
    internal class Generator
    {
        private const string chars = "йцукенгшщзхфывапролджэячсмитбю";
        private const int length = 5;

        private Random _rand = new Random();
        public Random Rand
        {
            private get => _rand;
            set => _rand = value;
        }

        /// <summary>
        /// Генерация массива строк. Принимает кол-во генерируемых данных.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IComparable[] GenerateStrings(int count)
        {
            string[] strings = new string[count];
            for (int i = 0; i < count; i++)
            {
                string str = null;
                for (int j = 0; j < length; j++)
                    str += chars[Rand.Next(chars.Length)];
                strings[i] = str;
            }

            return strings;
        }

        /// <summary>
        /// Генерация массива целочисленных значений. Принимает кол-во генерируемых данных.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IComparable[] GenerateInts(int count)
        {
            IComparable[] ints = new IComparable[count];
            for (int i = 0; i < count; i++)
                ints[i] = (Rand.Next((int)Math.Pow(10, length)));

            return ints;
        }

        /// <summary>
        /// Генерация дробных значений. Принимает кол-во генерируемых данных.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IComparable[] GenerateDoubles(int count)
        {
            IComparable[] doubles = new IComparable[count];
            for (int i = 0; i < count; i++)
                doubles[i] = ((double)Rand.Next((int)Math.Pow(10, length))/100);

            return doubles;
        }
    }
}
