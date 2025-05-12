using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;
using TRPO_course_project.Models;

namespace TRPO_course_project
{
    // Class to store historical statistics data points
    public class StatisticsDataPoint
    {
        public DateTime Timestamp { get; set; }
        public int ProgramsWritten { get; set; }
        public int ProgramsReviewed { get; set; }
        public int CorrectPrograms { get; set; }
        public int IncorrectPrograms { get; set; }
    }
    
    public partial class MainForm : Form
    {
        private TesterManager _testerManager;
        private List<string> _logMessages = new List<string>();
        private Dictionary<int, Panel> _testerPanels = new Dictionary<int, Panel>();
        private Dictionary<int, Label> _testerStateLabels = new Dictionary<int, Label>();
        private System.Windows.Forms.Timer _uiUpdateTimer;
        
        // Historical data storage
        private List<StatisticsDataPoint> _historicalData = new List<StatisticsDataPoint>();
        private int _chartTimeWindowSeconds = 60; // Default 60 second window
        
        // Add new dictionaries for labels
        private Dictionary<int, Label> _testerQueueLengthLabels = new Dictionary<int, Label>();
        private Dictionary<int, Label> _testerAvgWaitingTimeLabels = new Dictionary<int, Label>();
        private Dictionary<int, Label> _testerAvgServiceTimeLabels = new Dictionary<int, Label>();
        
        public MainForm()
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
            
            // Set up the main chart
            InitializeChart();
            
            // Configure tester charts tab
            tabPage3.Text = "Tester Statistics";
            
            // Ensure testerChartsPanel is set up properly
            if (!tabPage3.Controls.Contains(testerChartsPanel))
            {
                // Clear existing controls
                tabPage3.Controls.Clear();
                
                // Configure testerChartsPanel
                testerChartsPanel.Dock = DockStyle.Fill;
                testerChartsPanel.AutoScroll = true;
                testerChartsPanel.Padding = new Padding(10);
                testerChartsPanel.BorderStyle = BorderStyle.None;
                
                // Add to tabPage3
                tabPage3.Controls.Add(testerChartsPanel);
            }
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
            
            // Initialize individual tester charts
            InitializeTesterCharts();
        }
        
        private void CreateTesterUI(Tester tester)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = testerFlowLayoutPanel.Width - 10,
                Height = 300, // Increased height for new controls
                Margin = new Padding(5)
            };
            
            // Tester name
            var nameLabel = new Label
            {
                Text = tester.Name,
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panel.Controls.Add(nameLabel);
            
            // State label
            var stateLabel = new Label
            {
                Text = $"State: {tester.State}",
                Location = new Point(10, 40),
                AutoSize = true
            };
            panel.Controls.Add(stateLabel);
            
            // Queue length label
            var queueLengthLabel = new Label
            {
                Text = $"Queue Length: {tester.ReviewQueue.Count}",
                Location = new Point(10, 70),
                AutoSize = true
            };
            panel.Controls.Add(queueLengthLabel);
            
            // Time interval configuration
            var timeLabel = new Label
            {
                Text = "Time Intervals (ms):",
                Location = new Point(10, 100),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };
            panel.Controls.Add(timeLabel);
            
            // Writing time - min
            var writeMinLabel = new Label
            {
                Text = "Min Writing Time:",
                Location = new Point(10, 125),
                AutoSize = true,
                Size = new Size(110, 20)
            };
            panel.Controls.Add(writeMinLabel);
            
            var writeMinInput = new NumericUpDown
            {
                Location = new Point(130, 123),
                Minimum = 100,
                Maximum = 100000,
                Increment = 100,
                Value = tester.MinWritingTime,
                Width = 80,
                Tag = "MinWritingTime"
            };
            writeMinInput.ValueChanged += (sender, e) => UpdateTesterTimeInterval(tester.Id, "MinWritingTime", (int)writeMinInput.Value);
            panel.Controls.Add(writeMinInput);
            
            // Writing time - max
            var writeMaxLabel = new Label
            {
                Text = "Max Writing Time:",
                Location = new Point(10, 150),
                AutoSize = true,
                Size = new Size(110, 20)
            };
            panel.Controls.Add(writeMaxLabel);
            
            var writeMaxInput = new NumericUpDown
            {
                Location = new Point(130, 148),
                Minimum = 100,
                Maximum = 100000,
                Increment = 100,
                Value = tester.MaxWritingTime,
                Width = 80,
                Tag = "MaxWritingTime"
            };
            writeMaxInput.ValueChanged += (sender, e) => UpdateTesterTimeInterval(tester.Id, "MaxWritingTime", (int)writeMaxInput.Value);
            panel.Controls.Add(writeMaxInput);
            
            // Review time - min
            var reviewMinLabel = new Label
            {
                Text = "Min Review Time:",
                Location = new Point(10, 175),
                AutoSize = true,
                Size = new Size(110, 20)
            };
            panel.Controls.Add(reviewMinLabel);
            
            var reviewMinInput = new NumericUpDown
            {
                Location = new Point(130, 173),
                Minimum = 100,
                Maximum = 100000,
                Increment = 100,
                Value = tester.MinReviewingTime,
                Width = 80,
                Tag = "MinReviewingTime"
            };
            reviewMinInput.ValueChanged += (sender, e) => UpdateTesterTimeInterval(tester.Id, "MinReviewingTime", (int)reviewMinInput.Value);
            panel.Controls.Add(reviewMinInput);
            
            // Review time - max
            var reviewMaxLabel = new Label
            {
                Text = "Max Review Time:",
                Location = new Point(10, 200),
                AutoSize = true,
                Size = new Size(110, 20)
            };
            panel.Controls.Add(reviewMaxLabel);
            
            var reviewMaxInput = new NumericUpDown
            {
                Location = new Point(130, 198),
                Minimum = 100,
                Maximum = 100000,
                Increment = 100,
                Value = tester.MaxReviewingTime,
                Width = 80,
                Tag = "MaxReviewingTime"
            };
            reviewMaxInput.ValueChanged += (sender, e) => UpdateTesterTimeInterval(tester.Id, "MaxReviewingTime", (int)reviewMaxInput.Value);
            panel.Controls.Add(reviewMaxInput);
            
            // Add statistics labels
            var statsLabel = new Label
            {
                Text = "Statistics:",
                Location = new Point(10, 220),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };
            panel.Controls.Add(statsLabel);
            
            var avgWaitingTimeLabel = new Label
            {
                Text = "Avg Waiting Time: 0ms",
                Location = new Point(10, 240),
                AutoSize = true
            };
            panel.Controls.Add(avgWaitingTimeLabel);
            
            var avgServiceTimeLabel = new Label
            {
                Text = "Avg Service Time: 0ms",
                Location = new Point(10, 260),
                AutoSize = true
            };
            panel.Controls.Add(avgServiceTimeLabel);
            
            _testerPanels[tester.Id] = panel;
            _testerStateLabels[tester.Id] = stateLabel;
            
            // Store references to new labels
            _testerQueueLengthLabels[tester.Id] = queueLengthLabel;
            _testerAvgWaitingTimeLabels[tester.Id] = avgWaitingTimeLabel;
            _testerAvgServiceTimeLabels[tester.Id] = avgServiceTimeLabel;
            
            testerFlowLayoutPanel.Controls.Add(panel);
            
            // Setup state change handler
            CreateTesterStateHandler(tester);
        }
        
        private void UpdateTesterTimeInterval(int testerId, string propertyName, int value)
        {
            var tester = _testerManager.Testers.FirstOrDefault(t => t.Id == testerId);
            if (tester == null)
                return;
                
            switch (propertyName)
            {
                case "MinWritingTime":
                    tester.MinWritingTime = value;
                    // Ensure min is less than max
                    if (tester.MinWritingTime > tester.MaxWritingTime)
                        tester.MaxWritingTime = value + 100;
                    break;
                    
                case "MaxWritingTime":
                    tester.MaxWritingTime = value;
                    // Ensure max is greater than min
                    if (tester.MaxWritingTime < tester.MinWritingTime)
                        tester.MinWritingTime = value - 100;
                    break;
                    
                case "MinReviewingTime":
                    tester.MinReviewingTime = value;
                    // Ensure min is less than max
                    if (tester.MinReviewingTime > tester.MaxReviewingTime)
                        tester.MaxReviewingTime = value + 100;
                    break;
                    
                case "MaxReviewingTime":
                    tester.MaxReviewingTime = value;
                    // Ensure max is greater than min
                    if (tester.MaxReviewingTime < tester.MinReviewingTime)
                        tester.MinReviewingTime = value - 100;
                    break;
            }
            
            // Update UI if needed
            if (_testerPanels.TryGetValue(testerId, out var panel))
            {
                foreach (Control control in panel.Controls)
                {
                    if (control is NumericUpDown numInput && numInput.Tag is string tag)
                    {
                        switch (tag)
                        {
                            case "MinWritingTime":
                                numInput.Value = tester.MinWritingTime;
                                break;
                            case "MaxWritingTime":
                                numInput.Value = tester.MaxWritingTime;
                                break;
                            case "MinReviewingTime":
                                numInput.Value = tester.MinReviewingTime;
                                break;
                            case "MaxReviewingTime":
                                numInput.Value = tester.MaxReviewingTime;
                                break;
                        }
                    }
                }
            }
            
            // Log the change
            AddLogMessage($"Updated {tester.Name}'s {propertyName} to {value}ms", DateTime.Now);
        }
        
        private void CreateTesterStateHandler(Tester tester)
        {
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
                Color = Color.Blue,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7
            };
            
            var reviewedSeries = new Series("Programs Reviewed")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Yellow,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7
            };
            
            var correctSeries = new Series("Correct Programs")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7
            };
            
            var incorrectSeries = new Series("Incorrect Programs")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7
            };
            
            statisticsChart.Series.Add(writtenSeries);
            statisticsChart.Series.Add(reviewedSeries);
            statisticsChart.Series.Add(correctSeries);
            statisticsChart.Series.Add(incorrectSeries);
            
            // Configure chart axes
            var chartArea = statisticsChart.ChartAreas[0];
            chartArea.AxisX.Title = "Time";
            chartArea.AxisY.Title = "Count";
            chartArea.AxisX.LabelStyle.Format = "HH:mm:ss";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Seconds;
            chartArea.AxisX.Interval = 10;
            
            // Enable scrolling and zooming
            chartArea.CursorX.AutoScroll = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.AxisX.ScaleView.Zoomable = true;
            
            // Set proper date/time mode for X-axis
            chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.WordWrap;
            
            // Initialize with a 60-second window
            DateTime now = DateTime.Now;
            chartArea.AxisX.Minimum = now.AddSeconds(-60).ToOADate();
            chartArea.AxisX.Maximum = now.AddSeconds(5).ToOADate();
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
                
                // Store the data point in our history
                var dataPoint = new StatisticsDataPoint
                {
                    Timestamp = now,
                    ProgramsWritten = e.TotalProgramsWritten,
                    ProgramsReviewed = e.TotalProgramsReviewed,
                    CorrectPrograms = e.TotalCorrectPrograms,
                    IncorrectPrograms = e.TotalIncorrectPrograms
                };
                _historicalData.Add(dataPoint);
                
                // Lock the chart while updating to prevent cross-thread issues
                lock (statisticsChart)
                {
                    if (statisticsChart.Series.Count >= 4)
                    {
                        statisticsChart.Series[0].Points.AddXY(now, e.TotalProgramsWritten);
                        statisticsChart.Series[1].Points.AddXY(now, e.TotalProgramsReviewed);
                        statisticsChart.Series[2].Points.AddXY(now, e.TotalCorrectPrograms);
                        statisticsChart.Series[3].Points.AddXY(now, e.TotalIncorrectPrograms);
                        
                        // Calculate the time window to display based on user setting
                        DateTime minTime = now.AddSeconds(-_chartTimeWindowSeconds);
                        
                        // Remove visible data points older than the time window from display (not from history)
                        foreach (var series in statisticsChart.Series)
                        {
                            // Remove points outside of the display window
                            while (series.Points.Count > 0 && 
                                   series.Points[0].XValue < minTime.ToOADate())
                            {
                                series.Points.RemoveAt(0);
                            }
                        }
                        
                        // Dynamically update the X-axis (time axis) to show only the current window
                        statisticsChart.ChartAreas[0].AxisX.Minimum = minTime.ToOADate();
                        statisticsChart.ChartAreas[0].AxisX.Maximum = now.AddSeconds(5).ToOADate();

                        // Enable proper auto-scaling for the Y-axis
                        statisticsChart.ChartAreas[0].AxisY.Minimum = double.NaN;
                        statisticsChart.ChartAreas[0].AxisY.Maximum = double.NaN;
                        statisticsChart.ChartAreas[0].RecalculateAxesScale();
                        
                        // Force the chart to use a dynamic range based on data
                        statisticsChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                        statisticsChart.ChartAreas[0].AxisY.IsMarginVisible = true;
                        statisticsChart.ChartAreas[0].AxisY.Interval = 0;

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
        
        private void RefreshChartDisplay()
        {
            try
            {
                lock (statisticsChart)
                {
                    // Clear current chart data
                    foreach (var series in statisticsChart.Series)
                    {
                        series.Points.Clear();
                    }
                    
                    // If we have no historical data, just return
                    if (_historicalData.Count == 0) return;
                    
                    // Get current time and calculate window
                    DateTime now = DateTime.Now;
                    DateTime minTime = now.AddSeconds(-_chartTimeWindowSeconds);
                    
                    // Add all points within the window
                    foreach (var dataPoint in _historicalData.Where(d => d.Timestamp >= minTime))
                    {
                        statisticsChart.Series[0].Points.AddXY(dataPoint.Timestamp, dataPoint.ProgramsWritten);
                        statisticsChart.Series[1].Points.AddXY(dataPoint.Timestamp, dataPoint.ProgramsReviewed);
                        statisticsChart.Series[2].Points.AddXY(dataPoint.Timestamp, dataPoint.CorrectPrograms);
                        statisticsChart.Series[3].Points.AddXY(dataPoint.Timestamp, dataPoint.IncorrectPrograms);
                    }
                    
                    // Update axes
                    statisticsChart.ChartAreas[0].AxisX.Minimum = minTime.ToOADate();
                    statisticsChart.ChartAreas[0].AxisX.Maximum = now.AddSeconds(5).ToOADate();
                    statisticsChart.ChartAreas[0].RecalculateAxesScale();
                    
                    // Refresh
                    statisticsChart.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing chart: {ex.Message}");
            }
        }
        
        private void ExportStatistics_Click(object sender, EventArgs e)
        {
            if (_historicalData.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту", "Експорт статистики", 
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            using (var saveDialog = new SaveFileDialog()
            {
                Filter = "CSV файли (*.csv)|*.csv|Всі файли (*.*)|*.*",
                Title = "Експорт даних статистики",
                FileName = $"StatisticsExport_{DateTime.Now:yyyy-MM-dd_HH-mm}.csv"
            })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Create CSV file
                        using (var writer = new System.IO.StreamWriter(saveDialog.FileName))
                        {
                            // Write header
                            writer.WriteLine("Timestamp,Programs Written,Programs Reviewed,Correct Programs,Incorrect Programs");
                            

                            // Write data
                            foreach (var dataPoint in _historicalData.OrderBy(d => d.Timestamp))
                            {
                                writer.WriteLine($"{dataPoint.Timestamp:yyyy-MM-dd HH:mm:ss},{dataPoint.ProgramsWritten},{dataPoint.ProgramsReviewed},{dataPoint.CorrectPrograms},{dataPoint.IncorrectPrograms}");
                            }
                        }
                        
                        MessageBox.Show($"Дані успішно експортовано в {saveDialog.FileName}", 
                                      "Експорт успішний", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при експорті: {ex.Message}", 
                                      "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void ClearStatisticsHistory()
        {
            if (MessageBox.Show("Ви впевнені, що хочете очистити всю історію статистики?", 
                              "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _historicalData.Clear();
                
                // Clear chart
                foreach (var series in statisticsChart.Series)
                {
                    series.Points.Clear();
                }
                
                AddLogMessage("Історію статистики очищено", DateTime.Now);
            }
        }
        
        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            foreach (var tester in _testerManager.Testers)
            {
                if (_testerQueueLengthLabels.TryGetValue(tester.Id, out var queueLabel))
                {
                    queueLabel.Text = $"Queue Length: {tester.ReviewQueue.Count}";
                }
                
                // Update average times if available in tester statistics
                if (_testerManager.GetTesterStatistics(tester.Id) is TesterStatistics stats)
                {
                    if (_testerAvgWaitingTimeLabels.TryGetValue(tester.Id, out var waitingLabel))
                    {
                        waitingLabel.Text = $"Avg Waiting Time: {stats.AverageWaitingTime.TotalMilliseconds:F0}ms";
                    }
                    
                    if (_testerAvgServiceTimeLabels.TryGetValue(tester.Id, out var serviceLabel))
                    {
                        serviceLabel.Text = $"Avg Service Time: {stats.AverageServiceTime.TotalMilliseconds:F0}ms";
                    }
                }
            }
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
        
        // Tester Charts Implementation
        private Dictionary<int, Chart> _testerCharts = new Dictionary<int, Chart>();
        private Dictionary<int, DateTime> _lastTesterUpdateTimes = new Dictionary<int, DateTime>();

        private void InitializeTesterCharts()
        {
            // Clear any existing charts
            testerChartsPanel.Controls.Clear();
            _testerCharts.Clear();
            
            // Create a chart for each tester
            foreach (var tester in _testerManager.Testers)
            {
                CreateTesterChart(tester.Id, tester.Name);
            }
            
            // Subscribe to individual tester statistics updates
            _testerManager.TesterStatisticsUpdated += TesterManager_TesterStatisticsUpdated;
        }
        
        private void CreateTesterChart(int testerId, string testerName)
        {
            // Create a chart container panel
            var containerPanel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = testerChartsPanel.Width - 25,
                Height = 250,
                Margin = new Padding(5),
                Padding = new Padding(5),
                Dock = DockStyle.Top
            };
            
            // Add title label
            var titleLabel = new Label
            {
                Text = testerName,
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30
            };
            containerPanel.Controls.Add(titleLabel);
            
            // Create chart
            var chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            
            // Add chart areas
            var chartArea = new ChartArea("Main");
            chartArea.AxisX.Title = "Time";
            chartArea.AxisY.Title = "Count";
            chartArea.AxisX.LabelStyle.Format = "HH:mm:ss";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Seconds;
            chartArea.AxisX.Interval = 10;
            chart.ChartAreas.Add(chartArea);
            
            // Add legend
            chart.Legends.Add(new Legend("Legend") 
            { 
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                LegendStyle = LegendStyle.Row
            });
            
            // Add series
            var writtenSeries = new Series("Written")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                BorderWidth = 2
            };
            
            var reviewedSeries = new Series("Reviewed")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Yellow,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                BorderWidth = 2
            };
            
            var correctSeries = new Series("Correct")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                BorderWidth = 2
            };
            
            var incorrectSeries = new Series("Incorrect")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                BorderWidth = 2
            };
            
            chart.Series.Add(writtenSeries);
            chart.Series.Add(reviewedSeries);
            chart.Series.Add(correctSeries);
            chart.Series.Add(incorrectSeries);
            
            // Initialize with a 60-second window
            DateTime now = DateTime.Now;
            chartArea.AxisX.Minimum = now.AddSeconds(-60).ToOADate();
            chartArea.AxisX.Maximum = now.AddSeconds(5).ToOADate();
            
            // Enable proper auto-scaling for the Y-axis
            chartArea.AxisY.Minimum = double.NaN;
            chartArea.AxisY.Maximum = double.NaN;
            chartArea.AxisY.IsStartedFromZero = true;
            chartArea.AxisY.IsMarginVisible = true;
            
            containerPanel.Controls.Add(chart);
            testerChartsPanel.Controls.Add(containerPanel);
            
            // Store reference to the chart
            _testerCharts[testerId] = chart;
            _lastTesterUpdateTimes[testerId] = now;
        }
        
        private void TesterManager_TesterStatisticsUpdated(object sender, TesterStatisticsEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateTesterChart(e.Statistics)));
            }
            else
            {
                UpdateTesterChart(e.Statistics);
            }
        }
        
        private void UpdateTesterChart(TesterStatistics stats)
        {
            try
            {
                if (!_testerCharts.TryGetValue(stats.TesterId, out var chart))
                    return;
                
                DateTime now = DateTime.Now;
                _lastTesterUpdateTimes[stats.TesterId] = now;
                
                // Update the chart with new data
                lock (chart)
                {
                    // Add data points
                    chart.Series[0].Points.AddXY(now, stats.ProgramsWritten);
                    chart.Series[1].Points.AddXY(now, stats.ProgramsReviewed);
                    chart.Series[2].Points.AddXY(now, stats.CorrectReviews);
                    chart.Series[3].Points.AddXY(now, stats.IncorrectReviews);
                    
                    // Set up the time window
                    DateTime minTime = now.AddSeconds(-60);
                    
                    // Remove old data points
                    foreach (var series in chart.Series)
                    {
                        if (series.Points.Count > 30)
                        {
                            series.Points.RemoveAt(0);
                        }
                    }
                    
                    // Update X-axis range
                    chart.ChartAreas[0].AxisX.Minimum = minTime.ToOADate();
                    chart.ChartAreas[0].AxisX.Maximum = now.AddSeconds(5).ToOADate();
                    
                    // Auto-scale Y-axis
                    chart.ChartAreas[0].RecalculateAxesScale();
                    
                    // Refresh the chart
                    chart.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating tester chart: {ex.Message}");
            }
        }
    }
}
