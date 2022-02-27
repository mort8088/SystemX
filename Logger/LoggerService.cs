using System;
using System.IO;

namespace SystemX.Logger {
    public class LoggerService : IDisposable, I_LoggerService {
        private readonly StreamWriter _logFile;
        private bool _newline = true;

        public LoggerService(string fileName) {
            try {
                _logFile = new StreamWriter(fileName);
                Enabled = true;
            }
            catch {
                Enabled = false;
            }
        }

        public bool Enabled { get; set; }

        #region IDisposable Members
        void IDisposable.Dispose() {
            Dispose();
        }
        #endregion

        public void Write(string line, params object[] args) {
            if (!Enabled) return;
            if (_newline)
                _logFile.Write("[{0}] - ", DateTime.Now.ToString("HH:mm:ss.ffff"));

            _logFile.Write(line, args);

            _newline = false;
        }

        public void WriteLine(string line, params object[] args) {
            if (!Enabled) return;
            if (_newline)
                _logFile.Write("[{0}] - ", DateTime.Now.ToString("HH:mm:ss.ffff"));

            _logFile.WriteLine(line, args);

            _newline = true;
        }

        public void Flush() {
            if (Enabled) _logFile.Flush();
        }

        public void Dispose() {
            if (!Enabled) return;

            Enabled = false;
            _logFile.Flush();
            _logFile.Close();
            _logFile.Dispose();
        }

        #region I_LoggerService Members
        void I_LoggerService.Dispose() {
            Dispose();
        }

        void I_LoggerService.Flush() {
            Flush();
        }

        void I_LoggerService.Write(string line, params object[] args) {
            Write(line, args);
        }

        void I_LoggerService.WriteLine(string line, params object[] args) {
            WriteLine(line, args);
        }
        #endregion
    }
}