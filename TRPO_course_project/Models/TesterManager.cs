using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace TRPO_course_project.Models
{
    public class TesterManager
    {
        private readonly object _lockObject = new object();
        private readonly List<Tester> _testers = new List<Tester>();
        private readonly Dictionary<int, Task> _testerTasks = new Dictionary<int, Task>();
        private readonly Dictionary<int, TesterStatistics> _testerStatistics = new Dictionary<int, TesterStatistics>();
        
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Random _random = new Random();
        
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
                _testerStatistics[tester.Id] = new TesterStatistics(tester.Id, tester.Name);
                tester.StateChanged += Tester_StateChanged;
            }
        }
        
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            
            foreach (var tester in _testers)
            {
                var testerId = tester.Id;
                _testerTasks[testerId] = Task.Run(() => TesterWorkflow(tester, token), token);
            }
            
            LogMessage("Tester simulation started");
        }
        
        private async Task TesterWorkflow(Tester tester, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Якщо є програма на перевірку — перевіряємо її
                    if (tester.ReviewQueue.TryDequeue(out var programToReview))
                    {
                        await ReviewProgramAsync(tester, programToReview, token);
                        continue;
                    }

                    // Якщо чекає на результат — чекаємо, але не блокуємо цикл повністю
                    if (tester.WaitingForResult != null)
                    {
                        // Task.WhenAny: або результат, або знову перевіряємо чергу через короткий час
                        var completed = await Task.WhenAny(
                            tester.WaitingForResult.Task,
                            Task.Delay(100, token)
                        );
                        if (completed == tester.WaitingForResult.Task)
                        {
                            var result = tester.WaitingForResult.Task.Result;
                            tester.WaitingForResult = null;
                            tester.LastReviewerId = result.ReviewerId;
                            if (result.IsCorrect)
                            {
                                LogMessage($"{tester.Name} отримав підтвердження від {result.ReviewerName}, починає нову програму");
                                continue; // Переходимо до написання нової програми
                            }
                            else
                            {
                                LogMessage($"{tester.Name} отримав відмову від {result.ReviewerName}, виправляє програму");
                                // Виправляє і знову віддає тому ж тестеру
                                await WriteAndSendProgramAsync(tester, token, result.ReviewerId);
                                continue;
                            }
                        }
                        // Якщо не завершено — цикл повториться і перевірить чергу ще раз
                        continue;
                    }

                    // Якщо нічого не треба перевіряти і не чекає — пише нову програму
                    await WriteAndSendProgramAsync(tester, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальне завершення
            }
            catch (Exception ex)
            {
                LogMessage($"Error in tester {tester.Id} workflow: {ex.Message}");
            }
        }
        
        private async Task WriteAndSendProgramAsync(Tester tester, CancellationToken token, int? reviewerId = null)
        {
            tester.ChangeState(TesterState.Writing);
            LogMessage($"{tester.Name} почав писати програму");
            int writeTime = _random.Next(tester.MinWritingTime, tester.MaxWritingTime);
            await Task.Delay(writeTime, token);
            var program = new TestProgram(tester.Id);
            lock (_lockObject)
            {
                _totalProgramsWritten++;
                if (_testerStatistics.TryGetValue(tester.Id, out var stats))
                {
                    stats.IncrementProgramsWritten();
                    TesterStatisticsUpdated?.Invoke(this, new TesterStatisticsEventArgs(stats));
                }
                UpdateStatistics();
            }
            // Визначаємо, кому віддати на перевірку
            int reviewerIndex;
            if (reviewerId.HasValue)
            {
                reviewerIndex = _testers.FindIndex(t => t.Id == reviewerId.Value);
            }
            else
            {
                reviewerIndex = (_testers.IndexOf(tester) + 1) % _testers.Count;
            }
            var reviewer = _testers[reviewerIndex];
            reviewer.ReviewQueue.Enqueue(program);
            LogMessage($"{tester.Name} відправив програму на перевірку {reviewer.Name}");
            // Готуємося чекати результату
            tester.WaitingForResult = new TaskCompletionSource<ReviewResult>();
        }
        
        private async Task ReviewProgramAsync(Tester reviewer, TestProgram program, CancellationToken token)
        {
            reviewer.ChangeState(TesterState.Reviewing);
            LogMessage($"{reviewer.Name} перевіряє програму від Tester {program.AuthorId}");
            int reviewTime = _random.Next(reviewer.MinReviewingTime, reviewer.MaxReviewingTime);
            await Task.Delay(reviewTime, token);
            bool isCorrect = _random.Next(100) < 70;
            program.SetReviewResult(isCorrect, reviewer);
            lock (_lockObject)
            {
                _totalProgramsReviewed++;
                if (isCorrect)
                    _totalCorrectPrograms++;
                else
                    _totalIncorrectPrograms++;
                if (_testerStatistics.TryGetValue(reviewer.Id, out var stats))
                {
                    stats.IncrementProgramsReviewed(isCorrect);
                    TesterStatisticsUpdated?.Invoke(this, new TesterStatisticsEventArgs(stats));
                }
                UpdateStatistics();
            }
            LogMessage(isCorrect
                ? $"{reviewer.Name} підтвердив програму Tester {program.AuthorId}"
                : $"{reviewer.Name} відхилив програму Tester {program.AuthorId}");
            // Знаходимо автора і повертаємо результат
            var author = _testers.First(t => t.Id == program.AuthorId);
            author.WaitingForResult?.SetResult(new ReviewResult
            {
                ProgramId = program.Id,
                IsCorrect = isCorrect,
                ReviewerId = reviewer.Id,
                ReviewerName = reviewer.Name
            });
            reviewer.ChangeState(TesterState.Sleeping);
        }
        
        public void Stop()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                try
                {
                    Task.WaitAll(_testerTasks.Values.ToArray(), 3000);
                }
                catch (AggregateException) { }
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                LogMessage("Tester simulation stopped");
            }
        }
        
        private void Tester_StateChanged(object sender, TesterEventArgs e)
        {
            LogMessage($"{e.Tester.Name} is now {e.State}");
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
    
    // Клас для зберігання результату перевірки
    public class ReviewResult
    {
        public int ProgramId { get; set; }
        public bool IsCorrect { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; }
    }
}
