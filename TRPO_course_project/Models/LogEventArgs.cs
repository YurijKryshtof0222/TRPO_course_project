namespace TRPO_course_project.Models
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; }
        public DateTime Timestamp { get; }
        
        public LogEventArgs(string message, DateTime timestamp)
        {
            Message = message;
            Timestamp = timestamp;
        }
    }
}
