using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FTPc
{
    internal static class ExecutionLog
    {
        private static readonly object SyncRoot = new object();
        private static bool _enabled;
        private static bool _initialized;
        private static string _logFilePath;

        private static void EnsureLogFileCreated()
        {
            if (!_enabled || !string.IsNullOrEmpty(_logFilePath))
                return;

            string exeDir = Application.StartupPath;
            string fileName = "FTPc_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + Process.GetCurrentProcess().Id + ".log";
            _logFilePath = Path.Combine(exeDir, fileName);
            File.WriteAllText(_logFilePath, "Inicio do log: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine, Encoding.UTF8);
        }

        public static string CurrentLogPath
        {
            get { return _logFilePath; }
        }

        public static bool Enabled
        {
            get { return _enabled; }
        }

        public static void Initialize()
        {
            lock (SyncRoot)
            {
                if (_initialized)
                    return;

                _initialized = true;
                INI ini = new INI();
                _enabled = ini.ReadBool("Config", "LogAtivo", false);
                if (!_enabled)
                    return;

                EnsureLogFileCreated();
            }
        }

        public static void Write(string message)
        {
            if (!_initialized)
                Initialize();
            if (!_enabled)
                return;

            string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + message;

            lock (SyncRoot)
            {
                EnsureLogFileCreated();
                File.AppendAllText(_logFilePath, line + Environment.NewLine, Encoding.UTF8);
            }

            Debug.WriteLine(line);
            Console.WriteLine(line);
        }

        public static bool OpenCurrentLog()
        {
            if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
                return false;

            Process.Start(new ProcessStartInfo
            {
                FileName = _logFilePath,
                UseShellExecute = true
            });
            return true;
        }

        public static void SetEnabled(bool enabled)
        {
            lock (SyncRoot)
            {
                _enabled = enabled;
                if (_enabled)
                    EnsureLogFileCreated();
            }
        }
    }
}
