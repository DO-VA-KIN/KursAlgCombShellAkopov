using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkopovKursov_var29.Models
{
    internal class Sorter
    {
        static private Action<int> _reportProgress;
        /// <summary>
        /// Задает функцию сообщения о прогрессе
        /// </summary>
        static public Action<int> ReportProgress
        {
            private get => _reportProgress;
            set => _reportProgress = value;
        }

        /// <summary>
        /// Сортировка расчёской. Принимает массив данных и ссылку на счётчик перстановок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="swapCount"></param>
        public static void CombSort<T>(T[] array, ref int swapCount) where T : IComparable
        {
            const double factor = 1.247;
            int gap = array.Length;

            int maxProgress = (int)Math.Log(array.Length, factor);//сколько внешних итераций нам понадобится
            int currentProgress = 0;//счётчик этих итераций

            while (gap != 1)
            {
                if (gap > 1)
                {
                    gap = (int)(gap / factor);
                    ReportProgress?.Invoke(++currentProgress * 100 / maxProgress);//прогресс в %
                }

                for (int i = 0; i + gap < array.Length; i++)
                {
                    if (array[i].CompareTo(array[i + gap]) > 0)
                    {
                        Swap(array, i, i + gap);
                        swapCount++;//счётчик перестановок для логирования
                    }
                }
            }

            ReportProgress?.Invoke(100);
        }

        private static void Swap<T>(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }


        /// <summary>
        ///Сортировка Шелла. Принимает массив данных и ссылку на счётчик перстановок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="swapCount"></param>
        public static void ShellSort<T>(T[] array, ref int swapCount) where T : IComparable
        {
            int maxProgress = (int)Math.Log(array.Length, 2);//сколько внешних итераций нам понадобится
            int currentProgress = 0;//счётчик этих итераций

            for (int gap = array.Length / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < array.Length; i++)
                {
                    T temp = array[i];
                    int j;

                    if (i >= gap && array[i - gap].CompareTo(temp) > 0)
                        swapCount++;

                    for (j = i; j >= gap && array[j - gap].CompareTo(temp) > 0; j -= gap)
                        array[j] = array[j - gap];

                    array[j] = temp;
                }
                ReportProgress(++currentProgress * 100 / maxProgress);//прогресс в %
            }

            ReportProgress?.Invoke(100);
        }
    }
}
