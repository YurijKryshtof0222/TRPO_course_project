using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
        private Stopwatch _totalWaitingTime = new Stopwatch();
        private Stopwatch _totalServiceTime = new Stopwatch();
        
        // Базові метрики
        public TimeSpan AverageWaitingTime => _waitingTimes.Any() 
            ? TimeSpan.FromMilliseconds(_waitingTimes.Average(t => t.TotalMilliseconds))
            : TimeSpan.Zero;
        
        public TimeSpan AverageServiceTime => _serviceTimes.Any()
            ? TimeSpan.FromMilliseconds(_serviceTimes.Average(t => t.TotalMilliseconds))
            : TimeSpan.Zero;
            
        // Розширені метрики
        public TimeSpan MaxWaitingTime => _waitingTimes.Any()
            ? _waitingTimes.Max()
            : TimeSpan.Zero;
            
        public TimeSpan MaxServiceTime => _serviceTimes.Any()
            ? _serviceTimes.Max()
            : TimeSpan.Zero;
            
        public double EfficiencyScore => CalculateEfficiencyScore();
        
        public string PerformanceRecommendations => GenerateRecommendations();
        
        private double CalculateEfficiencyScore()
        {
            if (ProgramsReviewed == 0) return 0;
            
            // Базовий показник ефективності
            double baseScore = (double)CorrectReviews / ProgramsReviewed;
            
            // Корекція на основі часу очікування
            double waitingTimeFactor = 1.0;
            if (AverageWaitingTime.TotalMilliseconds > 5000) // Якщо середній час очікування > 5 секунд
            {
                waitingTimeFactor = 5000 / AverageWaitingTime.TotalMilliseconds;
            }
            
            // Корекція на основі часу обслуговування
            double serviceTimeFactor = 1.0;
            if (AverageServiceTime.TotalMilliseconds > 10000) // Якщо середній час обслуговування > 10 секунд
            {
                serviceTimeFactor = 10000 / AverageServiceTime.TotalMilliseconds;
            }
            
            return baseScore * waitingTimeFactor * serviceTimeFactor;
        }
        
        private string GenerateRecommendations()
        {
            var recommendations = new List<string>();
            
            // Аналіз часу очікування
            if (AverageWaitingTime.TotalMilliseconds > 5000)
            {
                recommendations.Add("Високий середній час очікування.\nРекомендується зменшити інтервали написання програм.");
            }
            
            // Аналіз часу обслуговування
            if (AverageServiceTime.TotalMilliseconds > 10000)
            {
                recommendations.Add("Високий середній час перевірки.\nРекомендується оптимізувати процес перевірки.");
            }
            
            // Аналіз ефективності
            if (EfficiencyScore < 0.5)
            {
                recommendations.Add("Низька ефективність роботи.\nРекомендується переглянути налаштування часу.");
            }
            
            // Аналіз балансу навантаження
            if (ProgramsReviewed > 0 && (double)ProgramsWritten / ProgramsReviewed > 2)
            {
                recommendations.Add("Дисбаланс між написанням та перевіркою програм. Рекомендується збалансувати навантаження.");
            }
            
            return recommendations.Any() 
                ? string.Join("\n", recommendations)
                : "Система працює оптимально.";
        }

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
            //_totalWaitingTime.Add(waitingTime);
        }

        public void AddServiceTime(TimeSpan serviceTime)
        {
            _serviceTimes.Add(serviceTime);
            //_totalServiceTime.Add(serviceTime);
        }
        
        public void StartWaitingTime()
        {
            _totalWaitingTime.Start();
        }
        
        public void StopWaitingTime()
        {
            _totalWaitingTime.Stop();
        }
        
        public void StartServiceTime()
        {
            _totalServiceTime.Start();
        }
        
        public void StopServiceTime()
        {
            _totalServiceTime.Stop();
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
