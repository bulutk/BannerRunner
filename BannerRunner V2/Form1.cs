using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BannerRunner_V3
{
    public partial class Form1 : Form
    {

        System.Diagnostics.Stopwatch stopwatch;
        const int DEFAULT_RUNNER_COUNT = 10;
        Runner[] RunnerArray;
        int numberOfRunners;
        bool WeHaveAWinner = false;
        string UrlOfLogin;
        public Singer sing;
        string userID, PIN;
        int totalTryNumber;

        public Form1()
        {
            InitializeComponent();

            numberOfRunners = DEFAULT_RUNNER_COUNT;
            UrlOfLogin = "https://suis.sabanciuniv.edu/prod/twbkwbis.P_SabanciLogin";
            textBoxOfURL.Text = UrlOfLogin;
            textBox1.Text = DEFAULT_RUNNER_COUNT.ToString();
            sing = new Singer();
            totalTryNumber = 0;
        }

        public void PopMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxUserID.Text == "" || textBoxPIN.Text == "")
            {
                MessageBox.Show("You need to enter the UserID and the PIN to make the ninjas' succeed!");
            }
            else
            {
                textTime.Text = "0";
                TextCountNumber.Text = "0";
                userID = textBoxUserID.Text;
                PIN = textBoxPIN.Text;
                textBoxPIN.Text = "******";

                stopwatch = new System.Diagnostics.Stopwatch();
                WeHaveAWinner = false;
                stopwatch.Start();
                timer1.Start();
                RunnerArray = new Runner[numberOfRunners];
                RunnerArray[0] = new Runner(this, 0, UrlOfLogin, userID, PIN);
                RunnerArray[0].Show();
                for (int i = 1; i < numberOfRunners; i++)
                {
                    RunnerArray[i] = new Runner(this, i, UrlOfLogin, userID, PIN);
                    //  RunnerArray[i].Show();
                }
                this.Focus();
            }
        }

        public void FireInTheHole(int RunnerID)
        {
            if (!WeHaveAWinner) // check if any other thread win before
            {
                WeHaveAWinner = true;
                stopwatch.Stop();
                for (int i = 0; i < numberOfRunners; i++)
                {
                    if (i != RunnerID) //kazanan runner dýþýndakileri kapa
                    {
                        RunnerArray[i].Stop();
                        RunnerArray[i].Close();
                    }
                    else // kazanan runner ý göster
                    {
                        RunnerArray[i].Show();
                    }
                }
                timer1.Stop();

                // We don't want to lose our login becouse of some IO exception do we?
                try
                {
                    // doing the logging after finishing the real thing for a better life ( without race conditions )
                    int GlobalTryCount = 0;
                    for (int i = 0; i < numberOfRunners; i++)
                    {
                        GlobalTryCount += RunnerArray[i].NumberOFTries;
                    }
                    TextCountNumber.Text = GlobalTryCount.ToString();
                    textTime.Text = stopwatch.Elapsed.ToString();

                    //Log keeping
                    System.IO.StreamWriter log = new System.IO.StreamWriter("log.txt", true);
                    log.Write("------------------------------------------------------\r\n"
                            + "Date: " + System.DateTime.Now.ToString()
                            + "\r\nNumber of tries: " + GlobalTryCount.ToString() + "\r\nTime spend: " + stopwatch.Elapsed.ToString()
                            + "\r\nWinner runner: " + (RunnerID + 1).ToString()
                            + "\r\n------------------------------------------------------\r\n\r\n");
                    log.Close();
                }
                catch (Exception E)
                {
                    System.IO.StreamWriter debug = new System.IO.StreamWriter("debug_document.txt", true);
                    debug.Write(E.ToString());
                    debug.Close();
                }
            }            
        }

        public void SingIt()
        {
            if (radioButton1.Checked)
            {
                sing.StartWinner();
            }
            else if (radioButton2.Checked)
            {
                sing.StartShort();
            }
            else if (radioButton3.Checked)
            {
                sing.StartLong();
            }
        }

        public void startOverAll()
        {
            for (int t = 0; t < numberOfRunners; t++)
            {
               // if (t != AliveRunnerID)
                RunnerArray[t].Stop();
                RunnerArray[t].Close();
            }
            System.Threading.Thread.Sleep(100);
            RunnerArray = new Runner[numberOfRunners];
            for (int i = 0; i < numberOfRunners; i++)
            {
              //  if (i != AliveRunnerID)
                    RunnerArray[i] = new Runner(this, i, UrlOfLogin, userID, PIN);                
            }
            RunnerArray[0].Show();
            totalTryNumber = int.Parse(TextCountNumber.Text);
            WeHaveAWinner = false;
            timer1.Start();
            stopwatch.Start();
            try
            {
                // doing the logging after finishing the real thing for a better life ( without race conditions )
                int GlobalTryCount = 0;
                for (int i = 0; i < numberOfRunners; i++)
                {
                    GlobalTryCount += RunnerArray[i].NumberOFTries;
                }
                TextCountNumber.Text = GlobalTryCount.ToString();
                textTime.Text = stopwatch.Elapsed.ToString();

                //Log keeping
                System.IO.StreamWriter log = new System.IO.StreamWriter("log.txt", true);
                log.Write("------------------------------------------------------\r\n"
                        + "Date: " + System.DateTime.Now.ToString()
                        + "\r\nNumber of tries: " + (GlobalTryCount + totalTryNumber).ToString() + "\r\nTime spend: " + stopwatch.Elapsed.ToString()
                        + "\r\nFake login screen."
                        + "\r\n------------------------------------------------------\r\n\r\n");
                log.Close();
            }
            catch (Exception E)
            {
                System.IO.StreamWriter debug = new System.IO.StreamWriter("debug_document.txt", true);
                debug.Write(E.ToString());
                debug.Close();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out numberOfRunners))
            {
                MessageBox.Show("Invalid input!");
                textBox1.Text = DEFAULT_RUNNER_COUNT.ToString();
                numberOfRunners = DEFAULT_RUNNER_COUNT;
            }
            else
            {
               if (numberOfRunners <= 0)
                {
                    MessageBox.Show("Bannerweb e girmek istmiyorsun galiba?");
                    textBox1.Text = DEFAULT_RUNNER_COUNT.ToString();
                    numberOfRunners = DEFAULT_RUNNER_COUNT;
                }
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            UrlOfLogin = textBoxOfURL.Text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                int tryCount = 0;
                for (int i = 0; i < numberOfRunners; i++)
                {
                    tryCount += RunnerArray[i].NumberOFTries;
                }
                TextCountNumber.Text = (tryCount+totalTryNumber).ToString();
                textTime.Text = stopwatch.Elapsed.ToString();
            }
            catch (Exception)
            { }
        }

        private void textBoxOfURL_MouseUp(object sender, MouseEventArgs e)
        {
            textBoxOfURL.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < numberOfRunners; i++)
            {
                RunnerArray[i].Stop();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sing.ShutUP();
        }

        private void textBoxPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                button1_Click(sender, new EventArgs());
        }

        private void textBoxUserID_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.facebook.com/Bannerrunner");
            }
            catch (Exception)
            { }
        }
    }
}