using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TRPO_course_project.Models
{
    public class Tester
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public TesterState State { get; private set; }
        
        // Configurable time intervals (in milliseconds)
        public int MinWritingTime { get; set; } = 1000;
        public int MaxWritingTime { get; set; } = 3000;
        public int MinReviewingTime { get; set; } = 1000;
        public int MaxReviewingTime { get; set; } = 2000;
        
        private readonly object _stateLock = new object();
        
        public event EventHandler<TesterEventArgs> StateChanged;
        
        // Черга програм, які треба перевірити цим тестером
        public ConcurrentQueue<TestProgram> ReviewQueue { get; } = new ConcurrentQueue<TestProgram>();
        
        // TaskCompletionSource для очікування результату перевірки власної програми
        public TaskCompletionSource<ReviewResult> WaitingForResult { get; set; }
        public int? LastReviewerId { get; set; } // Для повторної перевірки

        public Tester(int id, string name)
        {
            Id = id;
            Name = name;
            State = TesterState.Sleeping;
        }
        
        // Публічний метод для зміни стану тестера
        public void ChangeState(TesterState newState)
        {
            lock (_stateLock)
            {
                if (State != newState)
                {
                    State = newState;
                    StateChanged?.Invoke(this, new TesterEventArgs(this, newState));
                }
            }
        }
    }
    
    public enum TesterState
    {
        Sleeping,
        Writing,
        Reviewing
    }
    
    public class TesterEventArgs : EventArgs
    {
        public Tester Tester { get; }
        public TesterState State { get; }
        
        public TesterEventArgs(Tester tester, TesterState state)
        {
            Tester = tester;
            State = state;
        }
    }
    
    public class TestProgramEventArgs : EventArgs
    {
        public TestProgram Program { get; }
        
        public TestProgramEventArgs(TestProgram program)
        {
            Program = program;
        }
    }
}
