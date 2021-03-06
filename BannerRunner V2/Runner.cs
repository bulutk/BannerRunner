using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mshtml;


namespace BannerRunner_V3
{
    public partial class Runner : Form
    {
        const string LOGINED_URL = "suis.sabanciuniv.edu/prod/twbkwbis.P_GenMenu?name=bmenu.P_MainMnu";
        const string WRONG_PIN_URL = "suis.sabanciuniv.edu/prod/twbkwbis.P_ValLogin";
        Form1 motherNature;
        int id;
        bool WeAreIn;
        int numberOfTries;
        string url;
        string userID, PIN;
        static bool TRY_TO_LOGIN = true;
        enum state
        {
            initial,
            test_logined,
            logined
        }
        state currentState;

        public Runner(Form1 Mother,int ID,string URL,string User_ID,string Pin)
        {
            userID = User_ID;
            PIN = Pin;
            url = URL;
            numberOfTries = 0;
            WeAreIn = false;
            id = ID;
            motherNature = Mother;
            InitializeComponent();
            this.Text = "Runner " + (id + 1).ToString();
            currentState = state.initial;
            
            timer1.Interval = id * 300 + 1;  //this part is for async start of the pages
            timer1.Start();
        }

        public void Stop()
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            webBrowser1.Stop();            
        }

        public int NumberOFTries
        {
            get 
            {
                return numberOfTries;
            }
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            switch (currentState)
            {
                case state.initial:
                    timer2.Stop();
                    if (!WeAreIn)
                    {
                        if (webBrowser1.Url.ToString() == url)
                        {
                            System.Threading.Thread.Sleep(200);
                            if (!WeAreIn && webBrowser1.DocumentTitle != "Error Message" 
                                && webBrowser1.DocumentTitle != "Internet Explorer cannot display the webpage" 
                                && !webBrowser1.DocumentText.Contains("Check your Internet connection. Try visiting another website to make sure you are connected.") 
                                && !webBrowser1.DocumentText.Contains("<title>Navigation Canceled</title>") 
                                && !webBrowser1.DocumentText.Contains("<title>Cannot find server</title>") 
                                && !webBrowser1.DocumentText.Contains("<title>No page to display </title>") 
                                && !webBrowser1.DocumentText.Contains("<TITLE>503 Service Temporarily Unavailable</TITLE>"))
                            {
                                if (!webBrowser1.DocumentText.Contains("<TITLE>User Login</TITLE>"))
                                {
                                    System.IO.StreamWriter debug = new System.IO.StreamWriter("debug_document.txt", false);
                                    debug.Write(webBrowser1.DocumentText);
                                    debug.Close();
                                    // MessageBox.Show("An unknown error has occured please send \"debug_document.txt\" and retry.");
                                }
                                motherNature.FireInTheHole(id);
                                WeAreIn = true;
                                if (TRY_TO_LOGIN)
                                    timer3.Start(); // the login function. it requires some time to load everything so we wait.
                            }
                            else
                            {
                                webBrowser1.Navigate(url);
                                numberOfTries++;
                                timer2.Start();
                            }
                        }
                        else
                        {
                            webBrowser1.Navigate(url);
                            numberOfTries++;
                            timer2.Start();
                        }
                    }
                break;
                case state.test_logined:
                    if (webBrowser1.Url.ToString().Contains(LOGINED_URL))
                    {
                        currentState = state.logined;
                        button3_Click(this, new EventArgs());
                        motherNature.SingIt();
                    }
                    else
                    {
                        if (webBrowser1.Url.ToString().Contains(WRONG_PIN_URL))
                        {
                            motherNature.PopMessageBox("The User Id or the PIN is wrong. Please fix that and rerun!");
                        }
                        else
                        {
                            motherNature.startOverAll();
                            currentState = state.initial;
                            webBrowser1.Navigate(url);
                        }
                    }
                break;
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (!WeAreIn)
            {
                webBrowser1.Navigate(url);
                numberOfTries++;
                timer2.Start();
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!WeAreIn)
            {
                webBrowser1.Navigate(url);
                numberOfTries++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://suis.sabanciuniv.edu/prod/twbkwbis.P_GenMenu?name=bmenu.P_RegMnu");
        }

        private void button3_Click(object sender, EventArgs e) // add drop course site
        {
            webBrowser1.Navigate("https://suis.sabanciuniv.edu/prod/bwskfreg.P_AltPin");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://suis.sabanciuniv.edu/prod/bwskfcls.p_sel_crse_search");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://suis.sabanciuniv.edu/prod/bwskfshd.P_CrseSchd");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://suis.sabanciuniv.edu/prod/bwskflib.P_SelDefTerm");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            motherNature.sing.ShutUP();
            if (WeAreIn)
                button8.Hide();
        }

        private void Runner_FormClosing(object sender, FormClosingEventArgs e)
        {
            motherNature.sing.ShutUP();
        }

        private void timer3_Tick(object sender, EventArgs e) // user login part
        {
            timer3.Stop();
            try
            {

                webBrowser1.Document.Forms[0].Document.All["UserID"].SetAttribute("value", userID.ToString());
                webBrowser1.Document.Forms[0].Document.All["PIN"].FirstChild.SetAttribute("value", PIN.ToString());

                // click the button
                HtmlElement el = webBrowser1.Document.Forms[0].Document.All["PIN"].Parent.Parent.Parent.NextSibling.FirstChild;
                mshtml.HTMLButtonElement qwe = (HTMLButtonElement)el.DomElement;
                qwe.click();
                currentState = state.test_logined;
            }
            catch (System.Exception E)
            {
                motherNature.startOverAll();
                TRY_TO_LOGIN = false;

                System.IO.StreamWriter debug = new System.IO.StreamWriter("debug_document.txt", true);
                debug.Write(E.ToString());
                debug.Close();
            }
        }

       
    }
}