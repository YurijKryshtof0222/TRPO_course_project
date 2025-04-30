using System;

namespace TRPO_course_project.Models
{
    public class TestProgram
    {
        public int Id { get; private set; }
        public int AuthorId { get; private set; }
        public int? ReviewerId { get; private set; }
        public bool? IsCorrect { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime? ReviewTime { get; private set; }
        public int RevisionCount { get; private set; }
        
        public TestProgram(int authorId)
        {
            Id = Guid.NewGuid().GetHashCode();
            AuthorId = authorId;
            CreationTime = DateTime.Now;
            RevisionCount = 0;
        }
        
        public void SetReviewResult(bool isCorrect, Tester reviewer)
        {
            IsCorrect = isCorrect;
            ReviewerId = reviewer.Id;
            ReviewTime = DateTime.Now;
        }
        
        public TestProgram CreateRevision()
        {
            var revision = new TestProgram(AuthorId)
            {
                RevisionCount = this.RevisionCount + 1
            };
            return revision;
        }
    }
}
