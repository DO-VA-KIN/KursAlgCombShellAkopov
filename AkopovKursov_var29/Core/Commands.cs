using Prism.Commands;
using System.Windows;

namespace AkopovKursov_var29.Core
{
    internal class Commands
    {
        public Commands()
        {
            ApplicationShutdown = new DelegateCommand(OnApplicationShutdown);
        }

        public DelegateCommand ApplicationShutdown { get; }
        private void OnApplicationShutdown() => Application.Current.Shutdown();
    }
}
