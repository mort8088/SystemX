namespace SystemX.Logger {
    public interface I_LoggerService {
        bool Enabled { get; set; }
        
        void Dispose();
        void Flush();
        void Write(string line, params object[] args);
        void WriteLine(string line, params object[] args);
    }
}