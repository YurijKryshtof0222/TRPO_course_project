using System;
using System.Collections.Generic;
using System.Linq;

namespace TRPO_course_project.Models
{
    public class TesterStatistics
    {
        public int TesterId { get; }
        public string TesterName { get; }
        public int ProgramsWritten { get; private set; }
        public int ProgramsReviewed { get; private set; }
        public int CorrectReviews { get; private set; }
        public int IncorrectReviews { get; private set; }
        public DateTime LastUpdateTime { get; private set; }
        
        private List<TimeSpan> _waitingTimes = new List<TimeSpan>();
        private List<TimeSpan> _serviceTimes = new List<TimeSpan>();
        
        public TimeSpan AverageWaitingTime => _waitingTimes.Any() 
            ? TimeSpan.FromMilliseconds(_waitingTimes.Average(t => t.TotalMilliseconds))
            : TimeSpan.Zero;
        
        public TimeSpan AverageServiceTime => _serviceTimes.Any()
            ? TimeSpan.FromMilliseconds(_serviceTimes.Average(t => t.TotalMilliseconds))
            : TimeSpan.Zero;

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

        public void AddWaitingTime(TimeSpan waitingTime)
        {
            _waitingTimes.Add(waitingTime);
        }

        public void AddServiceTime(TimeSpan serviceTime)
        {
            _serviceTimes.Add(serviceTime);
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
