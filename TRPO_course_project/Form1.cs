using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;
using TRPO_course_project.Models;

namespace TRPO_course_project
{
    public partial class MainForm : Form
    {
        private TesterManager _testerManager;
        private List<string> _logMessages = new List<string>();
        private Dictionary<int, Panel> _testerPanels = new Dictionary<int, Panel>();
        private Dictionary<int, Label> _testerStateLabels = new Dictionary<int, Label>();
        private System.Windows.Forms.Timer _uiUpdateTimer;
        
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
                
                // Lock the chart while updating to prevent cross-thread issues
                lock (statisticsChart)
                {
                    if (statisticsChart.Series.Count >= 4)
                    {
                        statisticsChart.Series[0].Points.AddXY(now, e.TotalProgramsWritten);
                        statisticsChart.Series[1].Points.AddXY(now, e.TotalProgramsReviewed);
                        statisticsChart.Series[2].Points.AddXY(now, e.TotalCorrectPrograms);
                        statisticsChart.Series[3].Points.AddXY(now, e.TotalIncorrectPrograms);
                        
                        // Calculate the time window to display - last 60 seconds
                        DateTime minTime = now.AddSeconds(-60);
                        
                        // Remove data points older than the time window
                        foreach (var series in statisticsChart.Series)
                        {
                            if (series.Points.Count > 30)
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
                Width = testerChartsPanel.Width - 15,
                Height = 350, // Increased height to accommodate stats panel
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
            
            // Create statistics panel
            var statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(10)
            };
            
            // Add statistics labels
            var lblProgramsWritten = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                Text = "Programs Written: 0",
                Tag = "written"
            };
            
            var lblProgramsReviewed = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                Text = "Programs Reviewed: 0",
                Tag = "reviewed"
            };
            
            var lblCorrectPrograms = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 60),
                Text = "Correct Programs: 0",
                Tag = "correct"
            };
            
            var lblIncorrectPrograms = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 85),
                Text = "Incorrect Programs: 0",
                Tag = "incorrect"
            };
            
            statsPanel.Controls.Add(lblProgramsWritten);
            statsPanel.Controls.Add(lblProgramsReviewed);
            statsPanel.Controls.Add(lblCorrectPrograms);
            statsPanel.Controls.Add(lblIncorrectPrograms);
            
            // Store references to labels in the tag of the panel
            statsPanel.Tag = new Dictionary<string, Label>
            {
                { "written", lblProgramsWritten },
                { "reviewed", lblProgramsReviewed },
                { "correct", lblCorrectPrograms },
                { "incorrect", lblIncorrectPrograms }
            };
            
            containerPanel.Controls.Add(statsPanel);
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
                
                // Find the container panel that holds the chart
                Control containerPanel = chart.Parent;
                if (containerPanel == null)
                    return;
                
                // Find the stats panel (should be the first panel in the container)
                Panel statsPanel = null;
                foreach (Control control in containerPanel.Controls)
                {
                    if (control is Panel panel && panel.Tag is Dictionary<string, Label>)
                    {
                        statsPanel = panel;
                        break;
                    }
                }
                
                // Update the stats labels if found
                if (statsPanel != null && statsPanel.Tag is Dictionary<string, Label> labelsDict)
                {
                    if (labelsDict.TryGetValue("written", out var lblWritten))
                        lblWritten.Text = $"Programs Written: {stats.ProgramsWritten}";
                        
                    if (labelsDict.TryGetValue("reviewed", out var lblReviewed))
                        lblReviewed.Text = $"Programs Reviewed: {stats.ProgramsReviewed}";
                        
                    if (labelsDict.TryGetValue("correct", out var lblCorrect))
                        lblCorrect.Text = $"Correct Programs: {stats.CorrectReviews}";
                        
                    if (labelsDict.TryGetValue("incorrect", out var lblIncorrect))
                        lblIncorrect.Text = $"Incorrect Programs: {stats.IncorrectReviews}";
                }
                
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
