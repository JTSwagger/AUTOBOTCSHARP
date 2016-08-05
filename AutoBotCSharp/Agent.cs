using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support;
using System.Net;
using System.IO;
using System.Windows.Media;

namespace AutoBotCSharp
{

    public class Agent
    {
        public bool LoggedIn = false;
        public string Campaign;
        public int CallsToday;

        public string AgentNum;
        public string Agent_Name;
        public string Dialer_Status;
        public string Callpos;
        public const string INTRO = "INTRO";
        public const string INS_PROVIDER = "INS_PROVIDER";
        public const string INS_EXP = "INS_EXP";
        public const string INST_START = "INS_START";
        public const string NUM_VEHICLES = "NUM_VEHICLES";
        public const string YMM1 = "YMM1";
        public const string YMM2 = "YMM2";
        public const string YMM3 = "YMM3";
        public const string YMM4 = "YMM4";
        public const string DOB = "DOB";
        public const string MARITAL_STATUS = "MARITAL STATUS";
        public const string SPOUSE_NAME = "SPOUSE_NAME";
        public const string SPOUSE_DOB = "SPOUSE_DOB";
        public const string OWN_OR_RENT = "OWN OR RENT";
        public const string RES_TYPE = "RESIDENCE TYPE";
        public const string CREDIT = "CREDIT";
        public const string ADDRESS = "ADDRESS";
        public const string EMAIL = "EMAIL";
        public const string PHONE_TYPE = "PHONE TYPE";
        public const string LAST_NAME = "LAST NAME";
        public const string TCPA = "TCPA";
        private ChromeDriver driver;


        WebRequest webRequest;
        WebResponse resp;
        StreamReader reader;
        //--------------------------------------------------------------------------------------------------------
        public void doAgentStatusRequest()
        {
            
            while (LoggedIn)
            {
                StartWebRequest();
                string stats = reader.ReadToEnd();
                string[] tempstr = stats.Split(',');
                try
                {
                    Dialer_Status = tempstr[0];
                    Agent_Name = tempstr[5];
                    Console.WriteLine("Dialer Status: " + Dialer_Status);
                    Console.WriteLine("Agent Name: " + Agent_Name);
                }
                catch
                {
                    for(int i = 0; i < tempstr.Length-1;i++)

                    {
                        Console.WriteLine(tempstr[i]);
                    }
                }
                setGlobals();
                
            }
        }
        //------------------------------------------------------------------------------------------------------
        private void setGlobals()
        {

           switch(Dialer_Status)
            {
                case "READY":
                    App.Current.Resources["Background"] = new SolidColorBrush();
                    break;
                case "PAUSED":
                    App.Current.Resources["Background"] = new SolidColorBrush(Colors.Red);
                    break;
                case "CONNECTED":               
                    App.Current.Resources["Background"] = new SolidColorBrush(Colors.Green);
                    break;
            }

        }
        //----------------------------------------------------------------------------------------------------
        void StartWebRequest()
        {
            webRequest= WebRequest.Create("http://loudcloud9.ytel.com/x5/api/non_agent.php?source=test&user=101&pass=API101IEpost&function=agent_status&agent_user=" + AgentNum + "&stage=csv&header=NO");
            resp = webRequest.GetResponse();
            reader = new StreamReader(resp.GetResponseStream());

        }

     

  //-----------------------------------------------------------------------------------------------------------
        public bool Login(string AgentNumber)
        {
            AgentNum = AgentNumber;

            ChromeDriverService cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;
            driver = new ChromeDriver(cds);
            try
            {
                driver.Navigate().GoToUrl("http://loudcloud9.ytel.com");
                driver.SwitchTo().Frame("top");
                Thread.Sleep(500);
                driver.FindElementById("login-agent").Click();
                Thread.Sleep(250);
                driver.FindElementById("agent-login").SendKeys(AgentNum);
                Thread.Sleep(500);
                driver.FindElementById("agent-password").SendKeys("y" + AgentNum + "IE");
                Thread.Sleep(500);
                driver.FindElementById("btn-get-campaign").Click();
                Thread.Sleep(500);
                driver.FindElementById("select-campaign").Click();
                driver.FindElementById("select-campaign").FindElements(OpenQA.Selenium.By.TagName("option")).Last().Click(); 
                Thread.Sleep(250);
                driver.FindElementById("btn-submit").Click();
                LoggedIn = true;
                Task task = Task.Run((Action)doAgentStatusRequest);
               
            }
            catch
            {
                return false;
            }
            return true;
        }
        //---------------------------------------------------------------
        public bool EnterData(string Element, string Data)
        {
            try
            {
                driver.FindElementById(Element).SendKeys(Data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //------------------------------------------------------------------
        public void HangUpandDispo(string dispo)
        {
           
            try
            {
              WebRequest h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" +  AgentNum + "&function=external_hangup&value=1");
              WebResponse r = h.GetResponse();
                switch(dispo)
                {
                case "Not Available":
                    h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NotAvl");
                         r = h.GetResponse();
                        break;
               case "Not Interested":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NI");
                         r = h.GetResponse();
                        break;
                case "Do Not Call":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "DNC");
                         r = h.GetResponse();
                        break;
                    case "Wrong Number":
                h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "Wrong");
                         r = h.GetResponse();
                        break;
                    case "No Car":
                h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum+ "&function=external_status&value=" + "NoCar");
                         r = h.GetResponse();
                        break;
                    case "No English":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value="  +"NoEng");
                         r = h.GetResponse();
                        break;

                }
            }
            catch
            {
                Console.WriteLine("ERROR");
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void setupNameButtons()
        {
            var window = App.getWindow();
            string firstName = driver.FindElementById("frmFirstName").Text;
            //string firstName = "James";
            string[] clips = App.findNameClips(firstName);

            if (clips[0] != "no clip")
            {
                window.setNameText(firstName);
                window.btnTheirName.IsEnabled = true;
            } else
            {
                window.setNameText(firstName);
                window.btnTheirName.IsEnabled = false;
            }
            if (clips[1] != "no clip")
            {
                window.btnLookingFor.IsEnabled = true;
            } else
            {
                window.btnLookingFor.IsEnabled = false;
            }
            if (clips[2] != "no clip")
            {
                window.btnHi.IsEnabled = true;
            } else
            {
                window.btnHi.IsEnabled = false;
            }
        }

        //---------------------------------------------------------------

        public void PauseUnPause(string pauseAction)
        {
            WebRequest Pause;
            WebResponse resp;

            switch(pauseAction)
            {
                case "PAUSE":
                Pause = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_pause&value=" + "PAUSE");
                resp = Pause.GetResponse();
                resp.Close();
                        break;
                case "UNPAUSE":
                Pause = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_pause&value=" + "RESUME");
                    resp = Pause.GetResponse();
                    resp.Close();
                    break;
            }

        }
        public bool AskQuestion()
        {
            try
            {
                switch (Callpos)
                {
                    case INTRO:
                        App.RollTheClip(@"C:\Soundboard\Cheryl\INTRO\Intro2.mp3");
                        break;
                    case INS_PROVIDER:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3");                      
                        break;
                    case INS_EXP:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3");
                        break;
                    case INST_START:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3");
                        break;
                    case NUM_VEHICLES:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\How many vehicles do you have");
                            break;
                    case YMM1:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\First Vehicle.mp3");
                        break;
                    case YMM2:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\2nd Vehicle.mp3");
                        break;
                    case YMM3:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\3rd Vehicle.mp3");
                        break;
                    case YMM4:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\4th Vehicle.mp3");
                        break;
                    case DOB:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\DOB1.mp3");
                        break;
                    case MARITAL_STATUS:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\Marital Status.mp3");
                        break;
                    case SPOUSE_NAME:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses First name.mp3");
                        break;
                    case SPOUSE_DOB:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses Date of Birth.mp3");
                        break;
                    case OWN_OR_RENT:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Do You Own Or Rent the Home.mp3");
                        break;
                    case RES_TYPE:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\HOMETYPE.mp3");
                        break;
                    case CREDIT:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Credit.mp3");
                        break;
                    case ADDRESS:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\ADDRESS.mp3");
                        break;
                    case EMAIL:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\EMAIL.mp3");
                        break;
                    case PHONE_TYPE:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\PHONETYPE.mp3");
                        break;
                    case LAST_NAME:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Last Name.mp3");
                        break;
                    case TCPA:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\TCPA.mp3");
                        break;

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
        
}
