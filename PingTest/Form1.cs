using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PingTest
{
    public partial class Form1 : Form
    {
        private List<int> averagePing = new List<int>();

        public int PingInterval
        {
            get
            {
                return pingInterval;
            }
            set
            {
                pingInterval = value;
            }
        }
        private int pingInterval = 250;

        private Thread pingThread;
        private bool pingThreadRunning = false;

        public int AverageSamples
        {
            get
            {
                return averageSamples;
            }
            set
            {
                averageSamples = value;
            }
        }
        private int averageSamples = 5;

        public int OkPing
        {
            get
            {
                return okPing;
            }
            set
            {
                okPing = value;
            }
        }
        private int okPing = 50;
        public int GoodPing
        {
            get
            {
                return goodPing;
            }
            set
            {
                goodPing = value;
            }
        }
        private int goodPing = 25;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = PingInterval;
            numericUpDown2.Value = AverageSamples;
            numericUpDown3.Value = OkPing;
            numericUpDown4.Value = GoodPing;

            pingThreadRunning = true;
            pingThread = new Thread(new ThreadStart(PingThread));
            pingThread.IsBackground = true;
            pingThread.Start();           
        }

        public void PingThread()
        {
            while (pingThreadRunning)
            {
                
                Thread.Sleep(pingInterval);

                Ping pinger = new Ping();
                PingReply pingReply;

                long ping = 0;

                try
                {
                    IPHostEntry iPHostEntry = Dns.GetHostEntry(textBox1.Text);
                    pingReply = pinger.Send(iPHostEntry.AddressList[0], pingInterval / 2);
                    ping = pingReply.RoundtripTime;

                    while (averagePing.Count > averageSamples)
                        averagePing.RemoveAt(0);

                    averagePing.Add((int)ping);

                    setUiLabel(lblPingVal, ping);
                    setUiLabel(lblPingValAvg, averagePing.Average());
                }
                catch
                {
                    ping = -1;
                }

                
            }
        }

        private void setUiLabel(Label ui, double ping)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                try
                {
                    if (ping > 0)
                        ui.Text = ping.ToString("0");
                    else
                        ui.Text = "N/A";

                    if (ping <= 0)
                        ui.ForeColor = Color.Red;
                    else if (ping > goodPing)
                        ui.ForeColor = Color.DarkOrange;
                    else if (ping > okPing)
                        ui.ForeColor = Color.Red;
                    else
                        ui.ForeColor = Color.Green;
                }
                catch
                {
                    // form is closed
                }
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            pingThreadRunning = false;
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            PingInterval = (int)numericUpDown1.Value;
        }
        
        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            AverageSamples = (int)numericUpDown2.Value;
        }
        
        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {            
            OkPing = (int)numericUpDown3.Value;
            checkPing();
        }

        private void NumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            GoodPing = (int)numericUpDown4.Value;
            checkPing();
        }

        private void checkPing()
        {
            if (okPing <= goodPing)
            {
                if (goodPing >= 100)
                {
                    goodPing = okPing - 1;
                    numericUpDown4.Value = goodPing;
                }
                else
                {
                    okPing = goodPing + 1;
                    numericUpDown3.Value = okPing;
                }
            }
        }
    }
}