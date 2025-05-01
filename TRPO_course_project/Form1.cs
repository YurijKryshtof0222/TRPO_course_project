using System.Windows.Forms.DataVisualization.Charting;
using TRPO_course_project.Models;

namespace TRPO_course_project
{
    public partial class Form1 : Form
    {
        private TesterManager _testerManager;
        private List<string> _logMessages = new List<string>();
        private Dictionary<int, Panel> _testerPanels = new Dictionary<int, Panel>();
        private Dictionary<int, Label> _testerStateLabels = new Dictionary<int, Label>();
        private System.Windows.Forms.Timer _uiUpdateTimer;
        
        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            InitializeTesterManager();
        }
        
        private void InitializeUI()
        {
            // Set up the UI update timer
            _uiUpdateTimer = new System.Windows.Forms.Timer();
            _uiUpdateTimer.Interval = 100; // Update every 100ms
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            _uiUpdateTimer.Start();
            
            // Set up the chart
            InitializeChart();
        }
        
        private void InitializeTesterManager()
        {
            _testerManager = new TesterManager();
            _testerManager.LogEvent += TesterManager_LogEvent;
            _testerManager.StatisticsUpdated += TesterManager_StatisticsUpdated;
            
            // Create UI elements for each tester
            foreach (var tester in _testerManager.Testers)
            {
                CreateTesterUI(tester);
            }
        }
        
        private void CreateTesterUI(Tester tester)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = testerFlowLayoutPanel.Width - 10,
                Height = 80,
                Margin = new Padding(5)
            };
            
            var nameLabel = new Label
            {
                Text = tester.Name,
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panel.Controls.Add(nameLabel);
            
            var stateLabel = new Label
            {
                Text = $"State: {tester.State}",
                Location = new Point(10, 40),
                AutoSize = true
            };
            panel.Controls.Add(stateLabel);
            
            _testerPanels[tester.Id] = panel;
            _testerStateLabels[tester.Id] = stateLabel;
            
            testerFlowLayoutPanel.Controls.Add(panel);
            
            tester.StateChanged += (sender, e) => {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateTesterUI(e.Tester)));
                }
                else
                {
                    UpdateTesterUI(e.Tester);
                }
            };
        }
        
        private void UpdateTesterUI(Tester tester)
        {
            if (_testerStateLabels.TryGetValue(tester.Id, out var stateLabel))
            {
                stateLabel.Text = $"State: {tester.State}";
                
                // Change background color based on state
                var panel = _testerPanels[tester.Id];
                switch (tester.State)
                {
                    case TesterState.Sleeping:
                        panel.BackColor = Color.LightBlue;
                        break;
                    case TesterState.Writing:
                        panel.BackColor = Color.LightGreen;
                        break;
                    case TesterState.Reviewing:
                        panel.BackColor = Color.LightYellow;
                        break;
                }
            }
        }
        
        private void InitializeChart()
        {
            statisticsChart.Series.Clear();
            
            // Create series for statistics
            var writtenSeries = new Series("Programs Written")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue
            };
            
            var reviewedSeries = new Series("Programs Reviewed")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green
            };
            
            var correctSeries = new Series("Correct Programs")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGreen
            };
            
            var incorrectSeries = new Series("Incorrect Programs")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red
            };
            
            statisticsChart.Series.Add(writtenSeries);
            statisticsChart.Series.Add(reviewedSeries);
            statisticsChart.Series.Add(correctSeries);
            statisticsChart.Series.Add(incorrectSeries);
            
            statisticsChart.ChartAreas[0].AxisX.Title = "Time";
            statisticsChart.ChartAreas[0].AxisY.Title = "Count";
            statisticsChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            statisticsChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            statisticsChart.ChartAreas[0].AxisX.Interval = 5;
        }
        
        private void TesterManager_LogEvent(object sender, LogEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddLogMessage(e.Message, e.Timestamp)));
            }
            else
            {
                AddLogMessage(e.Message, e.Timestamp);
            }
        }
        
        private void AddLogMessage(string message, DateTime timestamp)
        {
            string formattedMessage = $"[{timestamp:HH:mm:ss}] {message}";
            _logMessages.Add(formattedMessage);
            
            if (_logMessages.Count > 100)
            {
                _logMessages.RemoveAt(0);
            }
            
            logTextBox.Text = string.Join(Environment.NewLine, _logMessages);
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }
        
        private void TesterManager_StatisticsUpdated(object sender, StatisticsEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatistics(e)));
            }
            else
            {
                UpdateStatistics(e);
            }
        }
        
        private void UpdateStatistics(StatisticsEventArgs e)
        {
            try
            {
                // Update statistics labels
                lblProgramsWritten.Text = $"Programs Written: {e.TotalProgramsWritten}";
                lblProgramsReviewed.Text = $"Programs Reviewed: {e.TotalProgramsReviewed}";
                lblCorrectPrograms.Text = $"Correct Programs: {e.TotalCorrectPrograms}";
                lblIncorrectPrograms.Text = $"Incorrect Programs: {e.TotalIncorrectPrograms}";
                
                // Update chart with synchronization
                DateTime now = DateTime.Now;
                
                // Lock the chart while updating to prevent cross-thread issues
                lock (statisticsChart)
                {
                    if (statisticsChart.Series.Count >= 4)
                    {
                        statisticsChart.Series[0].Points.AddXY(now, e.TotalProgramsWritten);
                        statisticsChart.Series[1].Points.AddXY(now, e.TotalProgramsReviewed);
                        statisticsChart.Series[2].Points.AddXY(now, e.TotalCorrectPrograms);
                        statisticsChart.Series[3].Points.AddXY(now, e.TotalIncorrectPrograms);
                        
                        // Limit data points
                        foreach (var series in statisticsChart.Series)
                        {
                            if (series.Points.Count > 30)
                            {
                                series.Points.RemoveAt(0);
                            }
                        }
                        
                        // Request refresh
                        statisticsChart.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                Console.WriteLine($"Error updating statistics: {ex.Message}");
            }
        }
        
        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Update any UI elements that need frequent updates
        }
        
        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;
            _testerManager.Start();
        }
        
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;
            
            // Run the stop operation in a background task to prevent UI freezing
            Task.Run(() => 
            {
                _testerManager.Stop();
                // After stopping, update UI on the main thread
                if (!IsDisposed)
                {
                    Invoke(new Action(() => 
                    {
                        AddLogMessage("Simulation fully stopped", DateTime.Now);
                    }));
                }
            });
        }
    }
}
