using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TRPO_course_project.Models
{
    public class TesterManager
    {
        private readonly List<Tester> _testers = new List<Tester>();
        private readonly Dictionary<int, Task> _testerTasks = new Dictionary<int, Task>();
        private readonly Dictionary<int, TesterStatistics> _testerStatistics = new Dictionary<int, TesterStatistics>();
        private readonly object _lockObject = new object();
        private CancellationTokenSource _cancellationTokenSource;
        
        public event EventHandler<LogEventArgs> LogEvent;
        public event EventHandler<StatisticsEventArgs> StatisticsUpdated;
        public event EventHandler<TesterStatisticsEventArgs> TesterStatisticsUpdated;
        
        private int _totalProgramsWritten = 0;
        private int _totalProgramsReviewed = 0;
        private int _totalCorrectPrograms = 0;
        private int _totalIncorrectPrograms = 0;
        
        public IReadOnlyList<Tester> Testers => _testers.AsReadOnly();
        
        public TesterManager()
        {
            InitializeTesters();
        }
        
        private void InitializeTesters()
        {
            _testers.Add(new Tester(1, "Tester 1"));
            _testers.Add(new Tester(2, "Tester 2"));
            _testers.Add(new Tester(3, "Tester 3"));
            
            foreach (var tester in _testers)
            {
                // Create statistics tracking for each tester
                _testerStatistics[tester.Id] = new TesterStatistics(tester.Id, tester.Name);
                
                // Subscribe to tester events
                tester.StateChanged += Tester_StateChanged;
                tester.ProgramCompleted += Tester_ProgramCompleted;
                tester.ReviewCompleted += Tester_ReviewCompleted;
            }
        }
        
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            
            foreach (var tester in _testers)
            {
                var testerId = tester.Id;
                _testerTasks[testerId] = Task.Run(() => RunTesterWorkflow(tester, token), token);
            }
            
            LogMessage("Tester simulation started");
        }
        
        public void Stop()
        {
            if (_cancellationTokenSource != null)
            {
                try
                {
                    // Signal cancellation
                    _cancellationTokenSource.Cancel();
                    
                    // Wait for tasks to complete but with a timeout to prevent deadlocks
                    bool allTasksCompleted = Task.WaitAll(_testerTasks.Values.ToArray(), 3000);
                    
                    if (!allTasksCompleted)
                    {
                        LogMessage("Some tester tasks are taking longer to stop. Continuing shutdown.");
                    }
                    
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                    
                    LogMessage("Tester simulation stopped");
                }
                catch (Exception ex)
                {
                    LogMessage($"Error stopping simulation: {ex.Message}");
                }
            }
        }
        
        private void RunTesterWorkflow(Tester tester, CancellationToken token)
        {
            try
            {
                // Start by writing a program
                tester.StartWritingProgram();
                
                while (!token.IsCancellationRequested)
                {
                    tester.WaitForWork();
                    
                    if (token.IsCancellationRequested)
                        break;
                        
                    if (tester.State == TesterState.Sleeping && tester.ProgramToReview == null)
                    {
                        tester.StartWritingProgram();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
            }
            catch (Exception ex)
            {
                LogMessage($"Error in tester {tester.Id} workflow: {ex.Message}");
            }
        }
        
        private void Tester_StateChanged(object sender, TesterEventArgs e)
        {
            var tester = e.Tester;
            LogMessage($"{tester.Name} is now {e.State}");
        }
        
        private void Tester_ProgramCompleted(object sender, TestProgramEventArgs e)
        {
            var tester = (Tester)sender;
            var program = e.Program;
            
            lock (_lockObject)
            {
                _totalProgramsWritten++;
                
                // Update individual tester statistics
                if (_testerStatistics.TryGetValue(tester.Id, out var stats))
                {
                    stats.IncrementProgramsWritten();
                    // Notify listeners about the updated statistics
                    TesterStatisticsUpdated?.Invoke(this, new TesterStatisticsEventArgs(stats));
                }
                
                // Assign the program to another tester for review
                var reviewer = GetAvailableReviewer(tester.Id);
                if (reviewer != null)
                {
                    LogMessage($"{tester.Name} completed a program and sent it to {reviewer.Name} for review");
                    reviewer.AssignProgramToReview(program);
                }
                else
                {
                    LogMessage($"{tester.Name} completed a program but no reviewers are available");
                    // Queue the program for later review
                    // In a real implementation, we would have a queue here
                }
                
                UpdateStatistics();
            }
        }
        
        private void Tester_ReviewCompleted(object sender, TestProgramEventArgs e)
        {
            var reviewer = (Tester)sender;
            var program = e.Program;
            
            lock (_lockObject)
            {
                _totalProgramsReviewed++;
                bool isCorrect = program.IsCorrect == true;
                
                // Update reviewer statistics
                if (_testerStatistics.TryGetValue(reviewer.Id, out var reviewerStats))
                {
                    reviewerStats.IncrementProgramsReviewed(isCorrect);
                    // Notify listeners about the updated statistics
                    TesterStatisticsUpdated?.Invoke(this, new TesterStatisticsEventArgs(reviewerStats));
                }
                
                if (isCorrect)
                {
                    _totalCorrectPrograms++;
                    LogMessage($"{reviewer.Name} approved the program from Tester {program.AuthorId}");
                    
                    // The author can now write a new program
                    var author = _testers.Find(t => t.Id == program.AuthorId);
                    if (author.State == TesterState.Sleeping)
                    {
                        Task.Run(() => author.StartWritingProgram());
                    }
                }
                else
                {
                    _totalIncorrectPrograms++;
                    LogMessage($"{reviewer.Name} rejected the program from Tester {program.AuthorId}");
                    
                    // The author needs to fix and resubmit
                    var author = _testers.Find(t => t.Id == program.AuthorId);
                    var revisedProgram = program.CreateRevision();
                    
                    if (author.State == TesterState.Sleeping)
                    {
                        Task.Run(() => {
                            author.StartWritingProgram();
                            // After writing, resubmit to the same reviewer
                            reviewer.AssignProgramToReview(revisedProgram);
                        });
                    }
                }
                
                UpdateStatistics();
            }
        }
        
        private Tester GetAvailableReviewer(int authorId)
        {
            // Find a tester who is not the author and is either sleeping or will be done soon
            return _testers.Find(t => t.Id != authorId && 
                                    (t.State == TesterState.Sleeping || 
                                     (t.State == TesterState.Reviewing && t.ProgramToReview == null)));
        }
        
        private void LogMessage(string message)
        {
            LogEvent?.Invoke(this, new LogEventArgs(message, DateTime.Now));
        }
        
        private void UpdateStatistics()
        {
            StatisticsUpdated?.Invoke(this, new StatisticsEventArgs(
                _totalProgramsWritten,
                _totalProgramsReviewed,
                _totalCorrectPrograms,
                _totalIncorrectPrograms
            ));
        }
    }
}
