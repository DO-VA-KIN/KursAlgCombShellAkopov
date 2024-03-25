using AkopovKursov_var29.Core;
using AkopovKursov_var29.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows;
using Prism.Dialogs;

namespace AkopovKursov_var29.ViewModels
{
    internal class MainView : BindableBase
    {
        #region переменные
        /// <summary>
        /// Режим(размерность) окна
        /// </summary>
        private WindowState _currentWindowState;
        public WindowState CurrentWindowState
        {
            get => _currentWindowState;
            set
            {
                _currentWindowState = value;
                RaisePropertyChanged(nameof(CurrentWindowState));
            }
        }

        private bool _canSort = true;
        /// <summary>
        /// Доступность интерфейса
        /// </summary>
        public bool CanSort
        {
            get => _canSort;
            set
            {
                _canSort = value;
                RaisePropertyChanged(nameof(CanSort));
            }
        }

        /// <summary>
        /// Количество элементов
        /// </summary>
        public int Count
        {
            get => Settings.Values.Count;
            set
            {
                if (value > 0 && value < 10e+6)
                    Settings.Values.Count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        private int _progress = 0;
        /// <summary>
        /// Прогресс сортировки
        /// </summary>
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }

        public IEnumerable<DataTypes> GetDataTypes
        {
            get => Enum.GetValues(typeof(DataTypes)).Cast<DataTypes>();
        }
       
        /// <summary>
        /// Выбранный тип данных
        /// </summary>
        public DataTypes CurrentDataType
        {
            get => Settings.Values.CurrentDataType;
            set
            {
                Settings.Values.CurrentDataType = value;
                RaisePropertyChanged(nameof(CurrentDataType));
            }
        }

        public IEnumerable<SortTypes> GetSortTypes
        {
            get => Enum.GetValues(typeof(SortTypes)).Cast<SortTypes>();
        }

        /// <summary>
        /// Выбранный способ сортировки
        /// </summary>
        public SortTypes CurrentSortType
        {
            get => Settings.Values.CurrentSortType;
            set
            {
                Settings.Values.CurrentSortType = value;
                RaisePropertyChanged(nameof(CurrentSortType));
            }
        }


        private IComparable[] _values = new IComparable[0];
        /// <summary>
        /// Элементы массива
        /// </summary>
        public IComparable[] Values
        {
            get => _values;
            set
            {
                UpadteChart(value);
                _values = value;
                RaisePropertyChanged(nameof(Values));
            }
        }


        //так как размер и данные могут быть неприемлемыми для графика математически обходим проблемную зону
        /// <summary>
        /// Обновление графика
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private async Task UpadteChart(IComparable[] values)
        {
            await Task.Run(async  () => 
            {
                double[] list = new double[values.Length];
                try
                {
                for (int i = 0; i < list.Length; i++)
                        list[i] = (Convert.ToDouble(values[i]));
                }
                catch
                {
                    //здесь можно написать что-то, но нет смысла строки показывать на графиках
                    //for (int i = 0; i < list.Length; i++)
                    //    list[i] = 0;
                }

                if (list.Length > 100)//в случае большого размера делим на 100 равных частей и берем среднии значения в каждой области
                {
                    double[] newList = new double[100];
                    int subArrayLength = list.Length / 100;

                    for (int i = 0; i < 100; i++)
                        newList[i] = new Memory<double>(list, i * subArrayLength, subArrayLength).ToArray().Average();

                    list = newList;
                }

                ChartValues<double> collection = new ChartValues<double>(list);
                ValuesChart = collection;
            }); 
        }
        private ChartValues<double> _valuesChart;
        public ChartValues<double> ValuesChart
        {
            get => _valuesChart;
            set
            {
                _valuesChart = value;
                RaisePropertyChanged(nameof(ValuesChart));
            }
        }
        #endregion


        #region Команды
        /// <summary>
        /// Свернуть окно
        /// </summary>
        public DelegateCommand WindowMinimizeCommand { get; }
        private void OnWindowMinimizeCommand() => CurrentWindowState = WindowState.Minimized;

        /// <summary>
        /// Во весь экран и обратно
        /// </summary>
        public DelegateCommand WindowMaximizeCommand { get; }
        private void OnWindowMaximizeCommand()
        {
            if(CurrentWindowState == WindowState.Maximized)
                CurrentWindowState = WindowState.Normal;
            else CurrentWindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Завершение приложения
        /// </summary>
        public DelegateCommand WindowCloseCommand { get; }
        private void OnWindowCloseCommand()
        {
            Settings.SaveSettings();
            new Commands().ApplicationShutdown.Execute();
        }

        /// <summary>
        /// Выполнение сортировки
        /// </summary>
        public DelegateCommand SortStopCommand { get; }
        private bool CanSortStopCommandExecute() => CanSort;
        private async void OnSortStopCommand()
        {
            if (Values.Count() < 2) return;

            Loger.StartLog("Начата сортировка - " + CurrentSortType + "[" + Count + "]");
            Loger.LogIndent(1, 10 * 10, '=');
            Loger.MessageLog("Данные в начале:\n");
            Loger.LogTable(Values, 10, 10);

            CanSort = false;
            int swapCount = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            switch (CurrentSortType)
            {
                case SortTypes.Расчёска:
                    await Task.Run(() => Sorter.CombSort(Values, ref swapCount));
                    break;
                case SortTypes.Шелл:
                    await Task.Run(() => Sorter.ShellSort(Values, ref swapCount));
                    break;
                default:
                    break;
            }
            stopwatch.Stop();
            var val = new IComparable[0];
            Values = val.Concat(Values).ToArray();
            CanSort = true;

            Loger.LogIndent(1, 10 * 10, '_');
            Loger.MessageLog("Данные после сортировки:\n");
            Loger.LogTable(Values, 10, 10);
            Loger.LogIndent(1, 10 * 10, '=');
            Loger.MessageLog("Количество перестановок: " + swapCount + "\tвремя: " + stopwatch.Elapsed.ToString());
            Loger.LogIndent(3, 0, ' ');
        }

        /// <summary>
        /// Выполнение генерации
        /// </summary>
        public DelegateCommand GenerateCommand { get; }
        private async void OnGenerateCommand()
        {
            CanSort = false;
            switch (CurrentDataType)
            {                
                case DataTypes.целые:
                    await Task.Run(() => Values = new Generator().GenerateInts(Count));
                    break;
                case DataTypes.дробные:
                    await Task.Run(() => Values = new Generator().GenerateDoubles(Count));
                    break;
                case DataTypes.строки:
                    await Task.Run(() => Values = new Generator().GenerateStrings(Count));
                    break;
                default:
                    break;
            }
            CanSort = true;
            Progress = 0;
        }

        /// <summary>
        /// Открыть лог
        /// </summary>
        public DelegateCommand OpenLogCommand { get; }
        private void OnOpenLogCommand()
        {
            try
            {
                Process logFile = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        Arguments = Loger.Directory + Loger.FileName
                    }
                };
                logFile.Start();
            }
            catch { }
        }

        /// <summary>
        /// Очистить лог
        /// </summary>
        public DelegateCommand DeleteLogCommand { get; }
        private void OnDeleteLogCommand() => Loger.ClearLog(); 
        #endregion


        // Не, ну это очевидно - тут старт программы 
        // (не счтитая нижнех уровней программы, которые выполняются до прогрузки интерфейса)
        public MainView()
        {
            Settings.Initialize();//иницализируем настройки
            Sorter.ReportProgress = (int progress) => Progress = progress;//Связываем прогресс с классом сортировки

            WindowMinimizeCommand = new DelegateCommand(OnWindowMinimizeCommand);//привязываем кнопку свернуть окно
            WindowMaximizeCommand = new DelegateCommand(OnWindowMaximizeCommand);//привязываем кнопку развернуть окно
            WindowCloseCommand = new DelegateCommand(OnWindowCloseCommand);//привязываем кнопку закрытия окна
            SortStopCommand = new DelegateCommand(OnSortStopCommand, CanSortStopCommandExecute);//привязываем кнопку сортировки
            GenerateCommand = new DelegateCommand(OnGenerateCommand);//привязываем кнопку генерации
            OpenLogCommand = new DelegateCommand(OnOpenLogCommand);//привязываем меню открыть лог
            DeleteLogCommand = new DelegateCommand(OnDeleteLogCommand);//привязываем меню удалить лог
        }

    }
}
