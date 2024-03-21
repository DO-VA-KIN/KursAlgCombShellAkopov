using System;
using System.IO;
using System.Windows.Threading;
using System.Xml.Serialization;
using AkopovKursov_var29.Core;

namespace AkopovKursov_var29.ViewModels
{
    [Serializable]
    public struct SettingsStruct
    {
        public int Count { get; set; }
        public DataTypes CurrentDataType { get; set; }
        public SortTypes CurrentSortType { get; set; }
    }


    internal static class Settings
    {
        /// <summary>
        /// Имя для файла настроек.
        /// </summary>
        private const string settingsFileName = "Settings.xml";
        /// <summary>
        /// Таймер автосохранений.
        /// </summary>
        public static DispatcherTimer TimerAutosave = new DispatcherTimer { Interval = new TimeSpan(0, 0, 5) };

        /// <summary>
        /// Сохраняемые значения.
        /// </summary>
        public static SettingsStruct Values;

        private static Exception LastException;
        /// <summary>
        /// Последнее сообщение об ошибке
        /// </summary>
        public static Exception GetException()
        { return LastException; }


        /// <summary>
        /// Инициализация.
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            if (TimerAutosave != null)
                TimerAutosave.Tick += TimerAutosave_Tick;

            if (File.Exists(settingsFileName))
            {
                ReadSettings();
                return true;
            }
            else
            {
                SetDefault();
                SaveSettings();
                return false;
            }
        }

        /// <summary>
        /// Прекращение автоматических сохранений. Возобновить можно переинициализировав.
        /// </summary>
        public static void AutoSaveStop()
        {
            TimerAutosave.Stop();
            SaveSettings();
        }

        private static void TimerAutosave_Tick(object sender, EventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// Установить базовые значения.
        /// </summary>
        public static void SetDefault()
        {
            Values = new SettingsStruct
            {
                Count = 1000,
                CurrentDataType = DataTypes.целые,
                CurrentSortType = SortTypes.Расчёска
            };
        }

        /// <summary>
        /// Сохранить настройки.
        /// </summary>
        /// <returns></returns>
        public static bool SaveSettings()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(SettingsStruct));
                using (FileStream fs = new FileStream(settingsFileName, FileMode.Create))
                    ser.Serialize(fs, Values);

                return true;
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
        }

        /// <summary>
        /// Считать записанные настройки.
        /// </summary>
        /// <returns></returns>
        public static bool ReadSettings()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(SettingsStruct));
                using (FileStream fs = new FileStream(settingsFileName, FileMode.Open))
                    Values = (SettingsStruct)ser.Deserialize(fs);

                return true;
            }
            catch (Exception ex)
            {
                SetDefault();
                LastException = ex;
                return false;
            }
        }
    }

}




