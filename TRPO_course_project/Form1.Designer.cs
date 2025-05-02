using System.Windows.Forms.DataVisualization.Charting;

namespace TRPO_course_project
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.testerChartsPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.testerFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statisticsChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblIncorrectPrograms = new System.Windows.Forms.Label();
            this.lblCorrectPrograms = new System.Windows.Forms.Label();
            this.lblProgramsReviewed = new System.Windows.Forms.Label();
            this.lblProgramsWritten = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statisticsChart)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 711);
            this.splitContainer1.SplitterDistance = 394;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.testerFlowLayoutPanel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(394, 711);
            this.panel1.TabIndex = 0;
            // 
            // testerFlowLayoutPanel
            // 
            this.testerFlowLayoutPanel.AutoScroll = true;
            this.testerFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testerFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.testerFlowLayoutPanel.Location = new System.Drawing.Point(0, 40);
            this.testerFlowLayoutPanel.Name = "testerFlowLayoutPanel";
            this.testerFlowLayoutPanel.Size = new System.Drawing.Size(394, 671);
            this.testerFlowLayoutPanel.TabIndex = 1;
            this.testerFlowLayoutPanel.WrapContents = false;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(394, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "Testers";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(786, 711);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.logTextBox);
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(778, 678);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // logTextBox
            // 
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Location = new System.Drawing.Point(3, 3);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(772, 622);
            this.logTextBox.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.statisticsChart);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(778, 678);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Statistics";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // statisticsChart
            // 
            chartArea1.Name = "ChartArea1";
            this.statisticsChart.ChartAreas.Add(chartArea1);
            this.statisticsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.statisticsChart.Legends.Add(legend1);
            this.statisticsChart.Location = new System.Drawing.Point(3, 153);
            this.statisticsChart.Name = "statisticsChart";
            this.statisticsChart.Size = new System.Drawing.Size(772, 522);
            this.statisticsChart.TabIndex = 1;
            this.statisticsChart.Text = "chart1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblIncorrectPrograms);
            this.panel2.Controls.Add(this.lblCorrectPrograms);
            this.panel2.Controls.Add(this.lblProgramsReviewed);
            this.panel2.Controls.Add(this.lblProgramsWritten);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(772, 150);
            this.panel2.TabIndex = 0;
            // 
            // lblIncorrectPrograms
            // 
            this.lblIncorrectPrograms.AutoSize = true;
            this.lblIncorrectPrograms.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblIncorrectPrograms.Location = new System.Drawing.Point(20, 120);
            this.lblIncorrectPrograms.Name = "lblIncorrectPrograms";
            this.lblIncorrectPrograms.Size = new System.Drawing.Size(177, 23);
            this.lblIncorrectPrograms.TabIndex = 4;
            this.lblIncorrectPrograms.Text = "Incorrect Programs: 0";
            // 
            // lblCorrectPrograms
            // 
            this.lblCorrectPrograms.AutoSize = true;
            this.lblCorrectPrograms.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCorrectPrograms.Location = new System.Drawing.Point(20, 90);
            this.lblCorrectPrograms.Name = "lblCorrectPrograms";
            this.lblCorrectPrograms.Size = new System.Drawing.Size(166, 23);
            this.lblCorrectPrograms.TabIndex = 3;
            this.lblCorrectPrograms.Text = "Correct Programs: 0";
            // 
            // lblProgramsReviewed
            // 
            this.lblProgramsReviewed.AutoSize = true;
            this.lblProgramsReviewed.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProgramsReviewed.Location = new System.Drawing.Point(20, 60);
            this.lblProgramsReviewed.Name = "lblProgramsReviewed";
            this.lblProgramsReviewed.Size = new System.Drawing.Size(181, 23);
            this.lblProgramsReviewed.TabIndex = 2;
            this.lblProgramsReviewed.Text = "Programs Reviewed: 0";
            // 
            // lblProgramsWritten
            // 
            this.lblProgramsWritten.AutoSize = true;
            this.lblProgramsWritten.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProgramsWritten.Location = new System.Drawing.Point(20, 30);
            this.lblProgramsWritten.Name = "lblProgramsWritten";
            this.lblProgramsWritten.Size = new System.Drawing.Size(168, 23);
            this.lblProgramsWritten.TabIndex = 1;
            this.lblProgramsWritten.Text = "Programs Written: 0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 28);
            this.label2.TabIndex = 0;
            this.label2.Text = "Statistics";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.stopButton);
            this.panel3.Controls.Add(this.startButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(3, 625);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(772, 50);
            this.panel3.TabIndex = 1;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(143, 9);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(94, 29);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(20, 9);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(94, 29);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 711);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Tester Simulation";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statisticsChart)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel panel1;
        private FlowLayoutPanel testerFlowLayoutPanel;
        private Label label1;
        private TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel testerChartsPanel;
        private Chart statisticsChart;
        private Panel panel2;
        private Label lblIncorrectPrograms;
        private Label lblCorrectPrograms;
        private Label lblProgramsReviewed;
        private Label lblProgramsWritten;
        private Label label2;
        private Panel panel3;
        private Button stopButton;
        private Button startButton;
    }
}
