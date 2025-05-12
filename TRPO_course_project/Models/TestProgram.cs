using System;
using System.Diagnostics;

namespace TRPO_course_project.Models
{
    public class TestProgram
    {
        public int Id { get; }
        public int AuthorId { get; }
        public int? ReviewerId { get; private set; }
        public bool? IsCorrect { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime? ReviewTime { get; private set; }
        public int RevisionCount { get; private set; }
        public DateTime EnqueueTime { get; set; }
        public DateTime DequeueTime { get; set; }
        public DateTime ReviewEndTime { get; set; }
        public TimeSpan WaitingTime => DequeueTime - EnqueueTime;
        public TimeSpan ServiceTime => ReviewEndTime - DequeueTime;

        public Stopwatch WaitingStopwatch { get; } = new Stopwatch();
        public Stopwatch ServiceStopwatch { get; } = new Stopwatch();

        public TestProgram(int authorId)
        {
            Id = Guid.NewGuid().GetHashCode();
            AuthorId = authorId;
            CreationTime = DateTime.Now;
            RevisionCount = 0;
            EnqueueTime = DateTime.Now;
        }

        public TestProgram(int authorId, DateTime creationTime)
        {
            Id = Guid.NewGuid().GetHashCode();
            AuthorId = authorId;
            CreationTime = creationTime;
            RevisionCount = 0;
            EnqueueTime = DateTime.Now;
        }
        
        public void SetReviewResult(bool isCorrect, Tester reviewer)
        {
            IsCorrect = isCorrect;
            ReviewerId = reviewer.Id;
            ReviewTime = DateTime.Now;
            ReviewEndTime = DateTime.Now;
        }
        
        public TestProgram CreateRevision()
        {
            var revision = new TestProgram(AuthorId)
            {
                RevisionCount = this.RevisionCount + 1
            };
            return revision;
        }

        public void StartWaiting()
        {
            WaitingStopwatch.Start();
        }

        public void StopWaiting()
        {
            WaitingStopwatch.Stop();
        }

        public void StartService()
        {
            ServiceStopwatch.Start();
        }

        public void StopService()
        {
            ServiceStopwatch.Stop();
        }
    }
}
