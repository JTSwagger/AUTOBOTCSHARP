using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace AutoBotCSharp
{

    public class Agent
    {
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
       
 
        public bool Login()
        {
            ChromeDriverService cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;
            driver = new ChromeDriver(cds);
            try
            {        
                driver.Navigate().GoToUrl("http://loudcloud9.ytel.com");
                driver.FindElementById("");
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
        //---------------------------------------------------------------
        public bool AskQuestion()
        {
            try
            {
                switch (Callpos)
                {
                    case "INTRO":
                        App.RollTheClip(@"C:\Soundboard\Cheryl\INTRO\Intro2.mp3");
                        break;
                    case "INS_PROVIDER":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3");                      
                        break;
                    case "INS_EXPIRATION":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3");
                        break;
                    case "INS_START":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3");
                        break;
                    case "NUM_VEHICLES":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\How many vehicles do you have");
                            break;
                    case "YMM1":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\First Vehicle.mp3");
                        break;
                    case "YMM2":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\2nd Vehicle.mp3");
                        break;
                    case "YMM3":
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
