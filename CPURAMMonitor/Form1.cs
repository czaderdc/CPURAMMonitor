using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Management;

namespace CPURAMMonitor
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        bool ostrzeglem = false;
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
        PerformanceCounter upTimeCounter;
        SpeechSynthesizer synth;
        PerformanceCounter uzywanyRAMCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            upTimeCounter = new PerformanceCounter("System", "System Up Time");
            upTimeCounter.NextValue();
            timer.Tick += timer_Tick;
            synth = new SpeechSynthesizer();
            TimeSpan upTime = TimeSpan.FromSeconds(upTimeCounter.NextValue());
            Task.Run(() =>
            {
                synth.Speak($"Your computer is up for {(int)upTime.TotalDays} days {(int)upTime.Hours} hours {(int)upTime.Minutes} minutes");
            });


        }

        private void timer_Tick(object sender, EventArgs e)
        {
            double daneCPU = cpuCounter.NextValue();
            double daneRAM = ramCounter.NextValue();
            
            daneCPULabel.Text = string.Format("{0:0.00}%", daneCPU);
            daneRamLabel.Text = daneRAM.ToString() + "MB";
            this.metroProgressBar1.Value = (int)daneCPU;
            this.metroProgressBar2.Value = (int)daneRAM/100;
            chart1.Series["CPU"].Points.AddY(daneCPU);
            chart1.Series["RAM"].Points.AddY(uzywanyRAMCounter.NextValue());//w gb
            if ((int)daneRAM < 1000 && !ostrzeglem)
            {
                synth.Speak("Your free RAM is under one gigabyte!");
                ostrzeglem = true;
            }

        }

      
    }
}
