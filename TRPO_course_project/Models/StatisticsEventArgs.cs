namespace TRPO_course_project.Models
{
    public class StatisticsEventArgs : EventArgs
    {
        public int TotalProgramsWritten { get; }
        public int TotalProgramsReviewed { get; }
        public int TotalCorrectPrograms { get; }
        public int TotalIncorrectPrograms { get; }
        
        public StatisticsEventArgs(int written, int reviewed, int correct, int incorrect)
        {
            TotalProgramsWritten = written;
            TotalProgramsReviewed = reviewed;
            TotalCorrectPrograms = correct;
            TotalIncorrectPrograms = incorrect;
        }
    }
}
