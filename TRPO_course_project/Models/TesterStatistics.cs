using System;

namespace TRPO_course_project.Models
{
    public class TesterStatistics
    {
        public int TesterId { get; private set; }
        public string TesterName { get; private set; }
        public int ProgramsWritten { get; private set; }
        public int ProgramsReviewed { get; private set; }
        public int CorrectReviews { get; private set; }
        public int IncorrectReviews { get; private set; }
        public DateTime LastUpdateTime { get; private set; }

        public TesterStatistics(int testerId, string testerName)
        {
            TesterId = testerId;
            TesterName = testerName;
            ProgramsWritten = 0;
            ProgramsReviewed = 0;
            CorrectReviews = 0;
            IncorrectReviews = 0;
            LastUpdateTime = DateTime.Now;
        }

        public void IncrementProgramsWritten()
        {
            ProgramsWritten++;
            LastUpdateTime = DateTime.Now;
        }

        public void IncrementProgramsReviewed(bool wasCorrect)
        {
            ProgramsReviewed++;
            
            if (wasCorrect)
                CorrectReviews++;
            else
                IncorrectReviews++;

            LastUpdateTime = DateTime.Now;
        }
    }

    public class TesterStatisticsEventArgs : EventArgs
    {
        public TesterStatistics Statistics { get; }
        
        public TesterStatisticsEventArgs(TesterStatistics statistics)
        {
            Statistics = statistics;
        }
    }
}
