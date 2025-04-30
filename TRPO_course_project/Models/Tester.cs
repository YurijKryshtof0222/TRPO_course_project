using System;
using System.Collections.Generic;
using System.Threading;

namespace TRPO_course_project.Models
{
    public class Tester
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public TesterState State { get; private set; }
        public TestProgram CurrentProgram { get; private set; }
        public TestProgram ProgramToReview { get; private set; }
        
        private readonly object _stateLock = new object();
        private readonly ManualResetEvent _wakeupEvent = new ManualResetEvent(false);
        private readonly Random _random = new Random();
        
        public event EventHandler<TesterEventArgs> StateChanged;
        public event EventHandler<TestProgramEventArgs> ProgramCompleted;
        public event EventHandler<TestProgramEventArgs> ReviewCompleted;
        
        public Tester(int id, string name)
        {
            Id = id;
            Name = name;
            State = TesterState.Sleeping;
        }
        
        public void StartWritingProgram()
        {
            lock (_stateLock)
            {
                CurrentProgram = new TestProgram(Id);
                ChangeState(TesterState.Writing);
            }
            
            // Simulate writing time
            Thread.Sleep(_random.Next(1000, 3000));
            
            lock (_stateLock)
            {
                var program = CurrentProgram;
                CurrentProgram = null;
                ProgramCompleted?.Invoke(this, new TestProgramEventArgs(program));
                
                if (ProgramToReview == null)
                {
                    ChangeState(TesterState.Sleeping);
                    _wakeupEvent.Reset();
                }
                else
                {
                    StartReviewingProgram();
                }
            }
        }
        
        public void AssignProgramToReview(TestProgram program)
        {
            lock (_stateLock)
            {
                ProgramToReview = program;
                
                if (State == TesterState.Sleeping)
                {
                    _wakeupEvent.Set();
                    StartReviewingProgram();
                }
            }
        }
        
        private void StartReviewingProgram()
        {
            lock (_stateLock)
            {
                if (ProgramToReview == null)
                    return;
                    
                ChangeState(TesterState.Reviewing);
            }
            
            // Simulate review time
            Thread.Sleep(_random.Next(1000, 2000));
            
            lock (_stateLock)
            {
                // Decide if program is correct (70% chance)
                bool isCorrect = _random.Next(100) < 70;
                ProgramToReview.SetReviewResult(isCorrect, this);
                
                var program = ProgramToReview;
                ProgramToReview = null;
                ReviewCompleted?.Invoke(this, new TestProgramEventArgs(program));
                
                if (CurrentProgram == null)
                {
                    ChangeState(TesterState.Sleeping);
                    _wakeupEvent.Reset();
                }
            }
        }
        
        public void WaitForWork()
        {
            _wakeupEvent.WaitOne();
        }
        
        private void ChangeState(TesterState newState)
        {
            State = newState;
            StateChanged?.Invoke(this, new TesterEventArgs(this, newState));
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
