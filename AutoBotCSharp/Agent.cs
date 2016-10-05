using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Media;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;
using Ionic.Zip;
using System.Diagnostics;

namespace AutoBotCSharp
{

    public class Agent
    {
        
        public string version = "Version 66";
        public string verToCheck = "";
        public List<string> maleNames = new List<string>();
        public bool endcall = false;
        public string BDAYHOLDER = "";
        public bool AskingBDay = false;
        public bool doHome = false;
        public bool doLife = false;
        public bool doHealth = false;
        public bool doMedicare = false;
        public bool doRenters = false;
        public bool isTalking = false;
        public bool testing = false;
        public double SilenceTimer = 0;
        public bool currentlyRebuttaling = false;
        public bool inCall = false;
        public bool custObjected = false;
        public bool isListening = false;
        public bool hasAsked = false;
        public int waitTime = 0;
        public bool LoggedIn = false;
        public string Campaign;
        public int CallsToday;
        public string Data = "FALSE";
        public string AgentNum;
        public string Agent_Name;
        public string Dialer_Status;
        public string Callpos;

        private bool started = false;
        private bool newCall = false;
        public string Question;

        public const string STARTYMCSTARTFACE = "START";
        public const string INBETWEEN = "INBETWEEN";
        public const string INTRO = "INTRO";
        public const string PROVIDER = "INS_PROVIDER";
        public const string INS_EXP = "INS_EXP";
        public const string INST_START = "INS_START";
        public const string NUM_VEHICLES = "NUM_VEHICLES";
        public const string YMM_ONLY_ONE = "YMM ONLY ONE";
        public const string YMM1 = "YMM1";
        public const string YMM2 = "YMM2";
        public const string YMM3 = "YMM3";
        public const string YMM4 = "YMM4";
        public const string DOB = "DOB";
        public const string BDAYMONTH = "BDAYMONTH";
        public const string SPOUSEBDAYMONTH = "SPOUSEBDAYMONTH";
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
        public const string SECONDARIES = "SECONDARIES";
        public const string TCPA = "TCPA";
        public const string WHICHSECONDARIES = "WHICH SECONDARIES";
        public const string YEARBUILT = "YEAR BUILT";
        public const string PPC = "PERSONAL PROPERTY COVERAGE";
        public const string SQFT = "SQUARE FOOTAGE";
        public const string FIXING = "FIXING LEAD";


        public ChromeDriver driver;

        public static Agent temp = App.getAgent();
        public class Customer
        {
           
            public string IMPORT_ID { get; set; }
            public string LEADGUID { get; set; }
            public string LeadID { get; set; }
            public string speech { get; set; }
            public string phone { get; set; }
            public int numVehicles { get; set; }
            public string maritalStatus { get; set; }
            public string firstName { get; set; }
            public bool isNameEnabled { get; set; }
            public string expMonth { get; set; }
            public string expYear { get; set; }
            public string lastName { get; set; }
        }
        public Customer cust = new Customer { numVehicles = 0, maritalStatus = "Single", speech = "", LeadID = "", lastName="" };

        public string[] dobInfo;
        private bool notInterestedFutureBool = false;

        WebRequest webRequest;
        WebResponse resp;
        StreamReader reader;
        //--------------------------------------------------------------------------------------------------------
        private double calltime = 0;

        public void CheckForContact(double time)
        {
            Console.WriteLine("was called successfully");
            if (!isTalking)
            {
                switch ((int)time)
                {
                    case 4:
                        Application.Current.Dispatcher.Invoke((() => App.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello 1.mp3")));
                        SilenceTimer = 6;
                        break;
                    case 7:
                        Application.Current.Dispatcher.Invoke((() => App.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello 2.mp3")));
                        SilenceTimer = 10;
                        break;

                    case 11:
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\bad connection.mp3")));
                        Application.Current.Dispatcher.Invoke((() => HangUpandDispo("Not Available")));
                        break;
                }
            }
        }
           //---------------------------------------------

        public string CheckLead()
        {
            int i = 0;
            IReadOnlyCollection<OpenQA.Selenium.IWebElement> field = driver.FindElementsByClassName("error");
            foreach (OpenQA.Selenium.IWebElement item in field)
            {
                Console.WriteLine("MISSED"+ i + ": " + item.GetAttribute("id"));
                if (item.GetAttribute("id") != "") { return item.GetAttribute("id"); }
            }
            return "";           
        }
        //-----------------------------------------------------------------------
        public void INPUTDEFAULT()
        {
            Console.WriteLine("CUSTOMER SPEECH FOR DEFAULT: " + cust.speech);
            if (cust.speech != "")
            {
                switch (temp.Question)
                {
                    case Agent.INTRO:

                    case Agent.PROVIDER:

                        selectData("frmInsuranceCarrier", "Progressive Auto Pro");
                        temp.Question = Agent.INS_EXP;
                        temp.Callpos = "INBETWEEN";
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;
                    case Agent.INS_EXP:
                        selectData("frmPolicyExpires_Month", MonthFromNumeral((DateTime.Now.Month + 1).ToString()));
                        selectData("frmPolicyExpires_Year", (DateTime.Now.Year).ToString());
                        temp.Callpos = "INBETWEEN";
                        temp.Question = Agent.INST_START;
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;
                    case Agent.INST_START:
                        selectData("frmPolicyStart_Month", MonthFromNumeral((DateTime.Now.Month + 1).ToString()));
                        selectData("frmPolicyStart_Year", (DateTime.Now.Year - 1).ToString());
                        temp.Callpos = "INBETWEEN";
                        temp.Question = Agent.NUM_VEHICLES;
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;
                    //   case Agent.NUM_VEHICLES:
                    //       if (Int32.Parse(NUM_VEHICLES) == 1)
                    //       {
                    //           temp.Callpos = "INBETWEEN";
                    //           temp.Question = Agent.YMM_ONLY_ONE;
                    //           SilenceTimer = 0;
                    //           Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                    //           break;
                    //       }else
                    //       {
                    //           temp.Callpos = "INBETWEEN";
                    //           temp.Question = Agent.YMM1;
                    //           SilenceTimer = 0;
                    //           Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                    //           break;
                    //       }
                    case NUM_VEHICLES:
                        temp.Callpos = "INBETWEEN";
                        temp.Question = Agent.YMM_ONLY_ONE;
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;
                    case Agent.YMM1:
                    case YMM2:
                    case YMM3:
                    case YMM4:
                              temp.Callpos = "INBETWEEN";
                              temp.Question = Agent.DOB;
                              SilenceTimer = 0;
                              Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                             break;
                    case DOB:
                        SilenceTimer = 0;
                        temp.Question = Agent.BDAYMONTH;
                        App.getAgent().Question = Agent.BDAYMONTH;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;
                    case BDAYMONTH:
                       
                        temp.Question = Agent.MARITAL_STATUS;
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;
                    case SPOUSEBDAYMONTH:
                        temp.Question = Agent.OWN_OR_RENT;
                        SilenceTimer = 0;
                        Application.Current.Dispatcher.Invoke((() => temp.AskQuestion()));
                        break;

                }
            }

        }
        //--------------------------------------------------------------------------
        public static async Task<bool> doAgentStatusRequest()
        {

            while (App.getAgent().LoggedIn || App.getAgent().testing ==true)
            {
               
                 
                App.getAgent().StartWebRequest();
                string stats = App.getAgent().reader.ReadToEnd();
                Console.WriteLine(stats);
                App.getAgent().reader.Close();
                    
                string[] tempstr = stats.Split(',');
               
                try
                {
                    App.getAgent().Dialer_Status = tempstr[0];
                    App.getAgent().Agent_Name = tempstr[5];
                    try
                    {
                        if (stats.Contains("DEAD") || stats.Contains("DISPO"))
                        {

                            MySqlConnection myConnection = new MySqlConnection();
                            myConnection.ConnectionString =
                            "Server=sql9.freemysqlhosting.net;" +
                            "Database=sql9136099;" +
                            "Uid=sql9136099;" +
                            "Pwd=HvsN6cVwbx;";
                            myConnection.Open();
                            MySqlCommand Add = new MySqlCommand("INSERT INTO `DROPPEDCALLS` (`SPOT`) VALUES('" + App.getAgent().Question + "')", myConnection);
                            Add.ExecuteNonQuery();
                            myConnection.Close();
                            if (App.getAgent().calltime > 120)
                            {
                                Thread.Sleep(250);
                                myConnection.Open();
                                string name = App.getAgent().cust.firstName + " " + App.getAgent().cust.lastName;
                                Add = new MySqlCommand("INSERT INTO `LONGCALLS` (`AGENT`, `NAME`, `PHONE`, `LEAD_ID`, `LEAD_GUID`, `IMPORT_ID`) VALUES ('" + App.getAgent().AgentNum + "','" + name + "','" + App.getAgent().cust.phone + "','" + App.getAgent().cust.LeadID + "','" + App.getAgent().cust.LEADGUID + "','" + App.getAgent().cust.IMPORT_ID + "','" + App.getAgent().calltime.ToString() + "')", myConnection);
                                Add.ExecuteNonQuery();
                                myConnection.Close();
                            }

                            App.getAgent().HangUpandDispo("hangup");
                            App.getAgent().inCall = false;
                        }

                    } catch (Exception ex)
                    {
                        App.getWindow().speechTxtBox.Text += Environment.NewLine +   ex;
                        Console.WriteLine(ex);
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine("Problem getting stats");
                    }
                    if (App.getAgent().Dialer_Status == "READY")
                    {
                        App.getAgent().newCall = true;
                        App.longDictationClient.EndMicAndRecognition();
                      
                    } else if (App.getAgent().Dialer_Status == "INCALL" || App.getAgent().testing == true)

                    {
                        
                        if (App.getAgent().isTalking == false ) { App.getAgent().SilenceTimer += .2; Console.WriteLine("Silence is " + App.getAgent().SilenceTimer + " seconds"); }
                        if (App.getAgent().SilenceTimer >= 2) { App.getAgent().INPUTDEFAULT(); }                             
                        if (App.getAgent().SilenceTimer >= 4) {  App.getAgent().CheckForContact(App.getAgent().SilenceTimer); }
                        App.getAgent().calltime += 0.2;
                       App.totalTimer += 0.2;
                        Thread.Sleep(200);
                        if (App.getAgent().newCall)
                        {
                           
                            App.getAgent().inCall = true;
                            App.getAgent().currentlyRebuttaling = false;
                            App.getAgent().custObjected = false;
                            App.getAgent().setupBot();
                            App.getAgent().endcall = false;
                            App.getAgent().Callpos = STARTYMCSTARTFACE;
                            App.getAgent().Question = STARTYMCSTARTFACE;
                            App.getAgent().notInterestedFutureBool = false;
                            App.getAgent().calltime = 0;
                            App.getAgent().SilenceTimer = 0;
                            App.getAgent().newCall = false;
                        }                       
                    }
                    //Console.WriteLine("Dialer Status: " + Dialer_Status);
                    //Console.WriteLine("Agent Name: " + Agent_Name);
                    //Console.WriteLine("dead? " + dead);
                }
                catch (IndexOutOfRangeException)
                {
                    if (App.getAgent().started)
                    {
                        MessageBox.Show("You're not logged in anymore");
                        App.getAgent().driver.Quit();
                    }
 
                }
                App.getAgent().setGlobals();
              
                    

            }
            return true;
        }
        //------------------------------------------------------------------------------------------------------
        private void setGlobals()
        {

            switch (Dialer_Status)
            {
                case "READY":
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().Background = Brushes.LightGoldenrodYellow;
                    }));

                    break;
                case "PAUSED":
                    Application.Current.Dispatcher.Invoke((() =>
                    {

                        App.getWindow().Background = Brushes.IndianRed;
                    }));

                    break;
                case "INCALL":
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().Background = Brushes.ForestGreen;
                    }));

                    break;
            }

        }
        //----------------------------------------------------------------------------------------------------

        public bool EnterData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;

            while (retry)
            {
                try
                {
                    driver.FindElementById(elementId).SendKeys(data);
                    return true;
                }
                catch (OpenQA.Selenium.ElementNotVisibleException)
                {
                    unhideElement(elementId);
                    Console.WriteLine("Element has been unhidden, retrying...");
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    Console.WriteLine(elementId + " does not exist on the current form. Try a different ID?");
                    retry = false;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    if (staleRefCount == 2)
                    {
                        Console.WriteLine("Two stale references, ending");
                        retry = false;
                    }
                    Thread.Sleep(1000);
                    staleRefCount += 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Generic Exception");
                    Console.WriteLine("Inner exception: " + ex.InnerException);
                    Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
            return false;
        }
        public bool selectData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            while (retry)
            {
                try
                {
                    var select = new SelectElement(driver.FindElementById(elementId));
                    select.SelectByText(data);
                    return true;
                }
                catch (OpenQA.Selenium.ElementNotVisibleException)
                {
                    unhideElement(elementId);
                    Console.WriteLine("Element has been unhidden, retrying...");
                }
                catch (OpenQA.Selenium.NoSuchElementException ex)
                {
                    string message = ex.Message;
                    if (message.Contains(data))
                    {
                        Console.WriteLine(data + " is not a valid option for " + elementId + "; try a different option.");
                    }
                    else
                    {
                        Console.WriteLine(elementId + " does not exist on the current form. Try a different ID?");
                    }
                    retry = false;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    if (staleRefCount == 2)
                    {
                        Console.WriteLine("Two stale references, ending");
                        retry = false;
                    }
                    Thread.Sleep(1000);
                    staleRefCount += 1;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Generic Exception");
                    //Console.WriteLine("Inner exception: " + ex.InnerException);
                    //Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
            return false;
        }

        public bool CheckBox(string elementId)
        {
            bool retry = true;
            int staleRefCount = 0;

            while (retry)
            {
                try
                {
                    if (!driver.FindElement(OpenQA.Selenium.By.Id(elementId)).Selected)
                    {
                        driver.FindElementById(elementId).Click();
                        Console.WriteLine("CHECKED BOX");
                        return true;
                    }
                }
                catch (OpenQA.Selenium.ElementNotVisibleException)
                {
                    unhideElement(elementId);
                    Console.WriteLine("Element has been unhidden, retrying...");
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    Console.WriteLine(elementId + " does not exist on the current form. Try a different ID?");
                    retry = false;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    if (staleRefCount == 2)
                    {
                        Console.WriteLine("Two stale references, ending");
                        retry = false;
                    }
                    Thread.Sleep(1000);
                    staleRefCount += 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Generic Exception");
                    Console.WriteLine("Inner exception: " + ex.InnerException);
                    Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
            return false;
        }


        //===============================================================================
        public async void async_selectData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            while (retry)
            {
                try
                {
                    var select = new SelectElement(driver.FindElementById(elementId));
                    await Task.Run(() => select.SelectByText(data));
                }
                catch (OpenQA.Selenium.ElementNotVisibleException)
                {
                    unhideElement(elementId);
                    Console.WriteLine("Element has been unhidden, retrying...");
                }
                catch (OpenQA.Selenium.NoSuchElementException ex)
                {
                    string message = ex.Message;
                    if (message.Contains(data))
                    {
                        Console.WriteLine(data + " is not a valid option for " + elementId + "; try a different option.");
                    }
                    else
                    {
                        Console.WriteLine(elementId + " does not exist on the current form. Try a different ID?");
                    }
                    retry = false;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    if (staleRefCount == 2)
                    {
                        Console.WriteLine("Two stale references, ending");
                        retry = false;
                    }
                    Thread.Sleep(1000);
                    staleRefCount += 1;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Generic Exception");
                    //Console.WriteLine("Inner exception: " + ex.InnerException);
                    //Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
        }
        //================================================================================================================
        void StartWebRequest()
        {
            webRequest = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/non_agent.php?source=test&user=101&pass=API101IEpost&function=agent_status&agent_user=" + AgentNum + "&stage=csv&header=NO");
            resp = webRequest.GetResponse();
            reader = new StreamReader(resp.GetResponseStream());
            
            
        }
        public static string CheckIProvider(string s)
        {

            
            if (s.Contains("twenty first") || s.Contains("21st") || s.Contains("twenty first century") || s.Contains("21st century") || s.Contains("twenty first century insurance") || s.Contains("21st century insurance") || s.Contains("first century"))
            { return ("21st Century Insurance"); }
            if (s.Contains("AAA") || s.Contains("triple A") || s.Contains("triple a") || s.Contains("aaa"))
            { return ("AAA Insurance Co."); }
            if (s.Contains("aarp") || s.Contains("AARP"))
            { return ("AARP"); }
            if (s.Contains("etna") || s.Contains("edna") || s.Contains("aetna") || s.Contains("AETNA") || s.Contains("Edna"))
            { return ("AETNA"); }
            if (s.Contains("aflac") || s.Contains("affleck") || s.Contains("afleck") || s.Contains("AFLAC"))
            { return ("AFLAC"); }
            if (s.Contains("aig") || s.Contains("AIG"))
            { return ("AIG"); }
            if (s.Contains("AIU") || s.Contains("eye you"))
            { return ("AIU"); }
            if (s.Contains("allied") || s.Contains("ally"))
            { return ("Allied"); }
            if (s.Contains("allstate") || s.Contains("all state") || s.Contains("ball state") || s.Contains("mall state") || s.Contains("I'll say it") || s.Contains("I'll stay"))
            { return ("Allstate Insurance"); }
            if (s.Contains("american"))
            { return ("American Insurance"); }
            if (s.Contains("ameriplan"))
            { return ("AmeriPlan"); }
            if (s.Contains("amica") || s.Contains("amiga") || s.Contains("amigo") || s.Contains("omika") || s.Contains("amika"))
            { return ("Amica Insurance"); }
            if (s.Contains("answer") || s.Contains("answer financial"))
            { return ("Answer Financial"); }
            if (s.Contains("arbella") || s.Contains("bella"))
            { return ("Arbella"); }
            if (s.Contains("associated"))
            { return ("Associated Indemnity"); }
            if (s.Contains("atlanta casualty"))
            { return ("Atlanta Casualty"); }
            if (s.Contains("atlantic") || s.Contains("atlantic indemnity"))
            { return ("Atlantic Indemnity"); }
            if (s.Contains("auto club"))
            { return ("Auto Club Insurance Company"); }
            if (s.Contains("auto owners"))
            { return ("Auto-Owners Insurance"); }
            if (s.Contains("axa") || s.Contains("axe"))
            { return ("AXA Advisors"); }
            if (s.Contains("bankers"))
            { return ("Bankers Life and Casualty"); }
            if (s.Contains("banner"))
            { return ("Banner Life"); }
            if (s.Contains("best"))
            { return ("Best Agency USA"); }
            if (s.Contains("blue cross") || s.Contains("blue cross blue shield") || s.Contains("bcbs"))
            { return ("Blue Cross and Blue Shield"); }
            if (s.Contains("brooke"))
            { return ("Brooke Insurance"); }
            if (s.Contains("cal farm") || s.Contains("call form") || s.Contains("cal form") || s.Contains("call farm"))
            { return ("Cal Farm Insurance"); }
            if (s.Contains("california state") || s.Contains("cal state"))
            { return ("California State Automobile Association"); }
            if (s.Contains("chub"))
            { return ("chub"); }
            if (s.Contains("citizen") || s.Contains("citizens"))
            { return ("Citizens"); }
            if (s.Contains("clarendon"))
            { return ("Clarendon American Insurance"); }
            if (s.Contains("cna") || s.Contains("see na"))
            { return ("CNA"); }
            if (s.Contains("colonial"))
            { return ("Colonial Insurance"); }
            if (s.Contains("comparison"))
            { return ("Comparison Market"); }
            if (s.Contains("continental"))
            { return ("Continental Insurance"); }
            if (s.Contains("cotton") || s.Contains("cotton states"))
            { return ("Cotton States Insurance"); }
            if (s.Contains("country insurance"))
            { return ("Country Insurance and Financial Services"); }
            if (s.Contains("countrywide"))
            { return ("Countrywide Insurance"); }
            if (s.Contains("cse") || s.Contains("easy"))
            { return ("CSE Insurance Group"); }
            if (s.Contains("dairy"))
            { return ("Dairyland Insurance"); }
            if (s.Contains("e health") || s.Contains("ehealth"))
            { return ("eHealthInsurance Services"); }
            if (s.Contains("electric"))
            { return ("Electric Insurance"); }
            if (s.Contains("erie") || s.Contains("eerie") || s.Contains("hear ye"))
            { return ("Erie Insurance Company"); }
            if (s.Contains("esurance"))
            { return ("Esurance"); }
            if (s.Contains("farm bureau"))
            { return ("Farm Bureau/Farm Family/Rural"); }
            if (s.Contains("farmers"))
            { return ("Farmers Insurance"); }
            if (s.Contains("finance box"))
            { return ("FinanceBox.com"); }
            if (s.Contains("fire and casualty"))
            { return ("Fire and Casualty Insurance Co of CT"); }
            if (s.Contains("fireman") || s.Contains("firemens") || s.Contains("firemens fund"))
            { return ("Fireman's Fund"); }
            if (s.Contains("foremost"))
            { return ("Foremost"); }
            if (s.Contains("forester"))
            { return ("Foresters"); }
            if (s.Contains("frank") || s.Contains("frankenstein") || s.Contains("frankenmuth"))
            { return ("Frankenmuth Insurance"); }
            if (s.Contains("geico") || s.Contains("gecko") || s.Contains("i go") || s.Contains("gotta go"))
            { return ("Geico General Insurance"); }
            if (s.Contains("gmac") || s.Contains("gee em"))
            { return ("GMAC Insurance"); }
            if (s.Contains("golden rule"))
            { return ("Golden Rule Insurance"); }
            if (s.Contains("government"))
            { return ("Government Employees Insurance"); }
            if (s.Contains("i am a panda hear me moo"))
            { return ("Progressive"); }
            if (s.Contains("guaranty") || s.Contains("guarantee"))
            { return ("Guaranty National Insurance"); }
            if (s.Contains("guide") || s.Contains("guide one"))
            { return ("Guide One Insurance"); }
            if (s.Contains("hanover") || s.Contains("lloyd"))
            { return ("Hanover Lloyd's Insurance Company"); }
            if (s.Contains("hartford"))
            { return ("Hartfod Insurance Co of the Southeast"); }
            if (s.Contains("hastings") || s.Contains("hasting mutual"))
            { return ("Hastings Mutual Insurance Company"); }
            if (s.Contains("health benefits"))
            { return ("Health Benefits Direct"); }
            if (s.Contains("health plus"))
            { return ("Health Plus of America"); }
            if (s.Contains("health share"))
            { return ("HealthShare American"); }
            if (s.Contains("human") || s.Contains("humana"))
            { return ("Humana"); }
            if (s.Contains("ifa") || s.Contains("eye f a"))
            { return ("IFA Auto Insurance"); }
            if (s.Contains("igf"))
            { return ("IGF Insurance"); }
            if (s.Contains("infinity") || s.Contains("infinite"))
            { return ("Infinity Insurance"); }
            if (s.Contains("insurance insight"))
            { return ("Insurance Insight"); }
            if (s.Contains("insurance dot com") || s.Contains("insurance.com"))
            { return ("Insurance.com"); }
            if (s.Contains("insurance leads dot com") || s.Contains("insuranceleads.com"))
            { return ("InsuranceLeaders.com"); }
            if (s.Contains("insweb") || s.Contains("in web"))
            { return ("Insweb"); }
            if (s.Contains("integon") || s.Contains("pentagon"))
            { return ("Integon"); }
            if (s.Contains("hancock") || s.Contains("john hancock"))
            { return ("John Hancock"); }
            if (s.Contains("kaiser") || s.Contains("kayser") || s.Contains("permanent"))
            { return ("Kaiser Permanente"); }
            if (s.Contains("kemper") || s.Contains("camper") || s.Contains("lloyd"))
            { return ("Kemper Lloyds Insurance"); }
            if (s.Contains("landmark"))
            { return ("Landmark American Insurance"); }
            if (s.Contains("leader"))
            { return ("Leader National Insurance"); }
            if (s.Contains("liberty") || s.Contains("liberty mutual"))
            { return ("Liberty Mutual Insurance"); }
            if (s.Contains("lumber"))
            { return ("Lumbermens Mutual"); }
            if (s.Contains("maryland"))
            { return ("Maryland Casualty"); }
            if (s.Contains("mass mutual"))
            { return ("Mass Mutual"); }
            if (s.Contains("mega") || s.Contains("midwest") || s.Contains("mega midwest"))
            { return ("Mega/Midwest"); }
            if (s.Contains("mercury"))
            { return ("Mercury"); }
            if (s.Contains("met life") || s.Contains("metlife"))
            { return ("MetLife Auto and Home"); }
            if (s.Contains("metropolitan"))
            { return ("Metropolitan Insurance Co."); }
            if (s.Contains("mid century"))
            { return ("Mid Century Insurance"); }
            if (s.Contains("mid continent"))
            { return ("Mid-Continent Casualty"); }
            if (s.Contains("middlesex"))
            { return ("Middlesex Insurance"); }
            if (s.Contains("midland national") || s.Contains("midland"))
            { return ("Midland National Life"); }
            if (s.Contains("mutual of new york"))
            { return ("Mutual of New York"); }
            if (s.Contains("mutual of omaha") || s.Contains("omaha"))
            { return ("Mutual of Omaha"); }
            if (s.Contains("national ben franklin") || s.Contains("ben franklin"))
            { return ("National Ben Franklin Insurance"); }
            if (s.Contains("national casualty"))
            { return ("National Casualty"); }
            if (s.Contains("national continental"))
            { return ("National Continental Insurance"); }
            if (s.Contains("national fire"))
            { return ("National Fire Insurance Company of Hartford"); }
            if (s.Contains("national health"))
            { return ("National Health Insurance"); }
            if (s.Contains("national indemnity"))
            { return ("National Indemnity"); }
            if (s.Contains("national union fire of los angeles") || s.Contains("national union fire insurance of los angeles") || s.Contains("national union fire of la") || s.Contains("national union fire insurance of los angeles"))
            { return ("National Union Fire Insurance of LA"); }
            if (s.Contains("national union fire of pennsylvania") || s.Contains("national union fire insurance of pennsylvania"))
            { return ("National Union Fire Insurance of PA"); }
            if (s.Contains("nationwide"))
            { return ("Nationwide Insurance Company"); }
            if (s.Contains("new england financial"))
            { return ("New England Financial"); }
            if (s.Contains("new york life"))
            { return ("New York Life Insurance"); }
            if (s.Contains("northwestern"))
            { return ("Northwestern Mutual Life"); }
            if (s.Contains("omni"))
            { return ("Omni Insruance"); }
            if (s.Contains("orion"))
            { return ("Orion Insurance"); }
            if (s.Contains("pacific insurance"))
            { return ("Pacific Insurance"); }
            if (s.Contains("pafco"))
            { return ("Pafco General Insurance"); }
            if (s.Contains("patriot"))
            { return ("Patriot General Insurance"); }
            if (s.Contains("peak property"))
            { return ("Peak Property and Casualty Insurance"); }
            if (s.Contains("pemco"))
            { return ("PEMCO Insurance"); }
            if (s.Contains("physicians"))
            { return ("Physicians"); }
            if (s.Contains("pioneer"))
            { return ("Pioneer State Mutual Insurance Company"); }
            if (s.Contains("preferred"))
            { return ("Preferred Mutual"); }
            if (s.Contains("progressive") || s.Contains("progress") || s.Contains("aggressive"))
            { return ("Progressive"); }
            if (s.Contains("prudential"))
            { return ("Prudential Insurance Co."); }
            if (s.Contains("reliance") || s.Contains("reliant"))
            { return ("Reliance Insurance"); }
            if (s.Contains("response"))
            { return ("Response Insurance"); }
            if (s.Contains("safeco") || s.Contains("safe co") || s.Contains("safe auto"))
            { return ("SAFECO"); }
            if (s.Contains("safeway") || s.Contains("safe way"))
            { return ("Safeway Insurance"); }
            if (s.Contains("security insurance"))
            { return ("Security Insurance Co of Hartford"); }
            if (s.Contains("security national"))
            { return ("Security National Insurance Co of FL"); }
            if (s.Contains("sentinel"))
            { return ("Sentinel Insurance"); }
            if (s.Contains("sentry"))
            { return ("Sentry Insurance Group"); }
            if (s.Contains("shelter"))
            { return ("Shelter Insurance Co."); }
            if (s.Contains("saint paul") || s.Contains("st. paul"))
            { return ("St. Paul"); }
            if (s.Contains("standard fire"))
            { return ("Standard Fire Insurance Company"); }
            if (s.Contains("state and county"))
            { return ("State and County Mutual Fire Insurance"); }
            if (s.Contains("state farm") || s.Contains("statefarm") || s.Contains("haystack") || s.Contains("stay farm") || s.Contains("stayfarm") || s.Contains("state park") || s.Contains("say farm") || s.Contains("has a farm"))
            { return ("State Farm General"); }
            if (s.Contains("state fund"))
            { return ("State Fund"); }
            if (s.Contains("state national"))
            { return ("State National Insurance"); }
            if (s.Contains("superior"))
            { return ("Superior Insurance"); }
            if (s.Contains("sure health"))
            { return ("Sure Health Plans"); }
            if (s.Contains("abe group") || s.Contains("ah be group"))
            { return ("The Ahbe Group"); }
            if (s.Contains("general") || s.Contains("the general"))
            { return ("The General"); }
            if (s.Contains("tico") || s.Contains("tye co") || s.Contains("tie co"))
            { return ("TICO Insruance"); }
            if (s.Contains("tig countrywide") || s.Contains("tig"))
            { return ("TIG Countrywide Insurance"); }
            if (s.Contains("the hartford"))
            { return ("The Hartford"); }
            if (s.Contains("titan"))
            { return ("Titan"); }
            if (s.Contains("trans") || s.Contains("transamerica") || s.Contains("trans america"))
            { return ("TransAmerica"); }
            if (s.Contains("travelers"))
            { return ("Travelers Insurance Company"); }
            if (s.Contains("tri-state") || s.Contains("tri state"))
            { return ("Tri-State Consumer Insurance"); }
            if (s.Contains("twin city"))
            { return ("Twin City Fire Insurance"); }
            if (s.Contains("unicare") || s.Contains("unicorn"))
            { return ("UniCare"); }
            if (s.Contains("united american"))
            { return ("United American/Farm and Ranch"); }
            if (s.Contains("united pacific"))
            { return ("United Pacific Insurance"); }
            if (s.Contains("united security"))
            { return ("United Security"); }
            if (s.Contains("united services"))
            { return ("United Serviecs Automobile Association"); }
            if (s.Contains("unitrin"))
            { return ("Unitrin Direct"); }
            if (s.Contains("universal"))
            { return ("Universal Underwriters Insurance"); }
            if (s.Contains("US financial"))
            { return ("US Financial"); }
            if (s.Contains("USA"))
            { return ("USA Benefits/Continental General"); }
            if (s.Contains("USAA") || s.Contains("USA") || s.Contains("usaa"))
            { return ("USAA"); }
            if (s.Contains("usf and g"))
            { return ("USF and G"); }
            if (s.Contains("viking"))
            { return ("Viking Insurance Co of WI"); }
            if (s.Contains("western and"))
            { return ("Western and Southern Life"); }
            if (s.Contains("western"))
            { return ("Western Mutual"); }
            if (s.Contains("windsor"))
            { return ("Windsor Insurance"); }
            if (s.Contains("woodland"))
            { return ("Woolands Financial Group"); }
            if (s.Contains("zurich"))
            { return ("Zurich North America"); }
            return "FALSE";
        }

        public int getAge()
        {

            int Month; int Day; int Year;
            int.TryParse(driver.FindElementById("frmDOB_Month").GetAttribute("value"),  out Month);
            int.TryParse(driver.FindElementById("frmDOB_Day").GetAttribute("value"), out Day);
            int.TryParse(driver.FindElementById("frmDOB_Year").GetAttribute("value"), out Year);
            Console.WriteLine(Month.ToString()+ Day.ToString()+ Year.ToString());
            DateTime now = DateTime.Now;
            DateTime birthDate = new DateTime(Year, Month, Day);
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) { age--; }
            Console.WriteLine(age);
            return age;


        }

        //------------------------------------------------------------------------------------------
        public string checkForSecondaries()
        {
            string secondaries = "";
            string HomeInsurance = "";
            string LifeInsurance = "";           //These will define what secondaries to offer
            string HealthInsurance = "";
            try
            {
                int age = getAge();
                if(driver.FindElementById("frmResidenceType").GetAttribute("value") == "Own") { HomeInsurance = "Home"; }else if(driver.FindElementById("frmResidenceType").GetAttribute("value") == "Rent")  { HomeInsurance = "Rent"; }else { HomeInsurance = ""; }
                if(age <=80 && age >= 25) { LifeInsurance = "Life"; }
                if(age >= 64) { HealthInsurance = "Medicare"; }else { HealthInsurance = "Health"; }
                secondaries = (HomeInsurance + " " + LifeInsurance + " " + HealthInsurance);
                return (secondaries);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return ("ERROR");
            }
        }
        //-----------------------------------------------------------------------------------------
        public string getSecondaryClip()
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\More More More Cheryl\More More More Cheryl\";
            switch (checkForSecondaries())
            {
                case "Home  Medicare":
                    clip += "Home and Medicare.mp3";
                    break;
                case "Home  Health":
                    clip += "Home and Health.mp3";
                    break;
                case "Home Life Health":
                    clip += "Home Life and Health.mp3";
                    break;
                case "Home Life Medicare":
                    clip += "Home Life Medicare.mp3";
                    break;
                case "Rent  Medicare":
                    clip += "Rental and Medicare.mp3";
                    break;
                case "Rent  Health":
                    clip += "Renters Health.mp3";
                        break;
                case "Rent Life Health":
                    clip += "Renters Health and Life.mp3";
                    break;
                case "Rent Life Medicare":
                    clip += "Rental Life Medicare.mp3";
                    break;
                case " Life Health":
                    clip += "Life and Health.mp3";
                    break;
                case " Life Medicare":
                    clip += "Life and Medicare.mp3";
                    break;
                case "  Medicare":
                    clip += "Medicare.mp3";
                    break;

            }
            Console.WriteLine("SECONDARIES " + clip);
            return clip;
        }
        //---------------------------------------------------------------------------------------------------
        public static string checkExp(string s)
        {
            List<string> Dates = new List<string>(2);
            string expMonth;
            string expyear;
            s = s.ToLower();


            if (s.Contains("january"))
            {
                expMonth = "Jan";
                if (DateTime.Now.Month > 1)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("just renewed") || s.Contains("this month") || s.Contains("just did"))
            {
                expMonth = App.getAgent().MonthFromNumeral((DateTime.Now.Month).ToString());
                if (DateTime.Now.Month == 12)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("in a month") || s.Contains("next month"))
            {
                expMonth = App.getAgent().MonthFromNumeral((DateTime.Now.Month + 1).ToString());
                if (DateTime.Now.Month == 12)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("february"))
            {
                expMonth = "Feb";
                if (DateTime.Now.Month > 2)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("march"))
            {
                expMonth = "Mar";
                if (DateTime.Now.Month > 3)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("april"))
            {
                expMonth = "Apr";
                if (DateTime.Now.Month > 4)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("may"))
            {
                expMonth = "May";
                if (DateTime.Now.Month > 5)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("june"))
            {
                expMonth = "Jun";
                if (DateTime.Now.Month > 6)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("july"))
            {
                expMonth = "Jul";
                if (DateTime.Now.Month > 7)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("august"))
            {
                expMonth = "Aug";
                if (DateTime.Now.Month > 8)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("september"))
            {
                expMonth = "Sep";
                if (DateTime.Now.Month > 9)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }

            }
            else if (s.Contains("october"))
            {
                expMonth = "Oct";
                if (DateTime.Now.Month > 10)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }

            }
            else if (s.Contains("november"))
            {
                expMonth = "Nov";
                if (DateTime.Now.Month > 11)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }

            }
            else if (s.Contains("december"))
            {
                expMonth = "Dec";
                if (DateTime.Now.Month > 12)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            
            else
            {
                return "NULL";
            }

            return (expMonth + " " + expyear);
        }
        //-----------------------------------------------------------------------------------------------------
        public bool unhideElement(string elementId)
        {
            try
            {
                driver.ExecuteScript("$('" + elementId + "').removeClass('hide')");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //--------------------------------------------------------------------------------------------------
        public void openTestPage()
        {
            var cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;
            driver = new ChromeDriver(cds);
            driver.Navigate().GoToUrl("https://forms.lead.co/auto/?key=e2869270-7c7a-11e1-b0c4-0800200c9a66");
            EnterData("frmFirstName", "Spencer");
   
        }
        //--------------------------------------------------------------------------------------------------

        public string HowLong(string response)
        {
            string month = cust.expMonth;

            if (response.Contains("year"))
            {
                if (response.Contains("two") || response.Contains("2"))
                {
                    return (month + " " + (DateTime.Now.Year - 2).ToString());
                }
                else if (response.Contains("three") || response.Contains("3"))
                {
                    return (month + " " + (DateTime.Now.Year - 3).ToString());
                }
                else if (response.Contains("four") || response.Contains("4"))
                {
                    return (month + " " + (DateTime.Now.Year - 4).ToString());
                }
                else if (response.Contains("five") || response.Contains("5"))
                {
                    return (month + " " + (DateTime.Now.Year - 5).ToString());
                }
                if (response.Contains("six") || response.Contains("6") || response.Contains("sex"))
                {
                    return (month + " " + (DateTime.Now.Year - 6).ToString());
                }
                else if (response.Contains("seven") || response.Contains("7"))
                {
                    return (month + " " + (DateTime.Now.Year - 7).ToString());
                }
                else if (response.Contains("eight") || response.Contains("8"))
                {
                    return (month + " " + (DateTime.Now.Year - 8).ToString());
                }
                else if (response.Contains("nine") || response.Contains("9"))
                {
                    return (month + " " + (DateTime.Now.Year - 9).ToString());
                }
                if (response.Contains("ten") || response.Contains("10"))
                {
                    return (month + " " + (DateTime.Now.Year - 10).ToString());
                }
                else if (response.Contains("eleven") || response.Contains("11"))
                {
                    return (month + " " + (DateTime.Now.Year - 11).ToString());
                }
                else if (response.Contains("twelve") || response.Contains("12"))
                {
                    return (month + " " + (DateTime.Now.Year - 12).ToString());
                }
                else if (response.Contains("thirteen") || response.Contains("13"))
                {
                    return (month + " " + (DateTime.Now.Year - 13).ToString());
                }
                if (response.Contains("fourteen") || response.Contains("14"))
                {
                    return (month + " " + (DateTime.Now.Year - 14).ToString());
                }
                else if (response.Contains("fiften") || response.Contains("15"))
                {
                    return (month + " " + (DateTime.Now.Year - 15).ToString());
                }
                else if (response.Contains("sixteen") || response.Contains("16"))
                {
                    return (month + " " + (DateTime.Now.Year - 16).ToString());
                }
                else if (response.Contains("seventeen") || response.Contains("17"))
                {
                    return (month + " " + (DateTime.Now.Year - 17).ToString());
                }
                else if (response.Contains("eighteen") || response.Contains("18"))
                {
                    return (month + " " + (DateTime.Now.Year - 18).ToString());
                }
                else if (response.Contains("nineteen") || response.Contains("19"))
                {
                    return (month + " " + (DateTime.Now.Year - 19).ToString());
                }
                else
                {
                    return (month + " " + (DateTime.Now.Year - 20).ToString());
                }
            }
            else if (response.Contains("just started") || response.Contains("last month"))
            {
                return ((DateTime.Now.Month - 1).ToString() + " " + DateTime.Now.Year.ToString());
            }

            else
            {
                return "FALSE";
            }



        }
        //===================================================================================================
        public int getNumVehicles(string response)
          
        {

            
            if (response.Contains("1") || response.Contains("one") || response.Contains("won") || response.Contains("want"))
            { return 1; }
            else if (response.Contains("2") || response.Contains("two") || response.Contains("too") || response.Contains("take") || response.Contains("two"))
            { return 2; }
            else if (response.Contains("3") || response.Contains("three"))
            { return 3; }
            else if (response.Contains("4") || response.Contains("four") || response.Contains("for"))
            { return 4; }
            else if (response.Contains("5") || response.Contains("five"))
            { return 5; }

            else { return -1; }
        }
        //==================================================================================================


        public string GETYMM(string response, int vehicleNum)
        {
          
            List<string> VModels = new List<string>();
            string Modelcontrol = "1";
            string year;
            string make = "";
            string model = "FALSE";
            string searcher = "";
            OpenQA.Selenium.IWebElement models;
            switch (vehicleNum)
            {
                case 1:
                    Modelcontrol = "vehicle-model";
                    break;
                case 2:
                    Modelcontrol = "vehicle2-model";
                    break;
                case 3:
                    Modelcontrol = "vehicle3-model";
                    break;
                case 4:
                    Modelcontrol = "vehicle4-model";
                    break;
            }
            if (response.Contains("1981")) { year = "1981"; }
            else if (response.Contains("1982") || response.Contains("82")) { year = "1982"; }
            else if (response.Contains("1983") || response.Contains("83")) { year = "1983"; }
            else if (response.Contains("1984") || response.Contains("84")) { year = "1984"; }
            else if (response.Contains("1985") || response.Contains("85")) { year = "1985"; }
            else if (response.Contains("1986") || response.Contains("86")) { year = "1986"; }
            else if (response.Contains("1987") || response.Contains("87")) { year = "1987"; }
            else if (response.Contains("1988") || response.Contains("88")) { year = "1988"; }
            else if (response.Contains("1989") || response.Contains("89")) { year = "1989"; }
            else if (response.Contains("1990") || response.Contains("90")) { year = "1990"; }
            else if (response.Contains("1991") || response.Contains("91")) { year = "1991"; }
            else if (response.Contains("1992") || response.Contains("92")) { year = "1992"; }
            else if (response.Contains("1993") || response.Contains("93")) { year = "1993"; }
            else if (response.Contains("1994") || response.Contains("94")) { year = "1994"; }
            else if (response.Contains("1995") || response.Contains("95")) { year = "1995"; }
            else if (response.Contains("1996") || response.Contains("96")) { year = "1996"; }
            else if (response.Contains("1997") || response.Contains("97")) { year = "1997"; }
            else if (response.Contains("1998") || response.Contains("98")) { year = "1998"; }
            else if (response.Contains("1999") || response.Contains("99")) { year = "1999"; }
            else if (response.Contains("2000") ) { year = "2000"; }
            else if (response.Contains("2001") || response.Contains("01")) { year = "2001"; }
            else if (response.Contains("2002") || response.Contains("02")) { year = "2002"; }
            else if (response.Contains("2003") || response.Contains("03")) { year = "2003"; }
            else if (response.Contains("2004") || response.Contains("04")) { year = "2004"; }
            else if (response.Contains("2005") || response.Contains("05")) { year = "2005"; }
            else if (response.Contains("2006") || response.Contains("06")) { year = "2006"; }
            else if (response.Contains("2007") || response.Contains("07")) { year = "2007"; }
            else if (response.Contains("2008") || response.Contains("08")) { year = "2008"; }
            else if (response.Contains("2009") || response.Contains("09")) { year = "2009"; }
            else if (response.Contains("2010") || response.Contains("10")) { year = "2010"; }
            else if (response.Contains("2011") || response.Contains("11")) { year = "2011"; }
            else if (response.Contains("2012") || response.Contains("12")) { year = "2012"; }
            else if (response.Contains("2013") || response.Contains("13")) { year = "2013"; }
            else if (response.Contains("2014") || response.Contains("14")) { year = "2014"; }
            else if (response.Contains("2015") || response.Contains("15")) { year = "2015"; }
            else if (response.Contains("2016") || response.Contains("16")) { year = "2016"; }
            else if (response.Contains("2017") || response.Contains("17")) { year = "2016"; }
            else { year = "FALSE"; }

            if (response.Contains("acura")) { make = "ACURA"; }
            else if (response.ToUpper().Contains("ALFA ROMEO")) { make = "ALFA ROMEO"; }
            else if (response.ToUpper().Contains("ASTON MARTIN")) { make = "ASTON MARTIN"; }
            else if (response.ToUpper().Contains("AUDI")) { make = "AUDI"; }
            else if (response.ToUpper().Contains("BENTLEY")) { make = "BENTLEY"; }
            else if (response.ToUpper().Contains("BMW")) { make = "BMW"; }
            else if (response.ToUpper().Contains("BUICK")) { make = "BUICK"; }
            else if (response.ToUpper().Contains("CADILLAC")) { make = "CADILLAC"; }
            else if (response.ToUpper().Contains("CHEVROLET")) { make = "CHEVROLET"; }
            else if (response.ToUpper().Contains("CHRYSLER")) { make = "CHRYSLER"; }
            else if (response.ToUpper().Contains("DODGE")) { make = "DODGE"; }
            else if (response.ToUpper().Contains("FERRARI")) { make = "FERRARI"; }
            else if (response.ToUpper().Contains("FIAT")) { make = "FIAT"; }
            else if (response.ToUpper().Contains("FORD")) { make = "FORD"; }
            else if (response.ToUpper().Contains("GMC")) { make = "GMC"; }
            else if (response.ToUpper().Contains("HONDA")) { make = "HONDA"; }
            else if (response.ToUpper().Contains("HYUNDAI")) { make = "HYUNDAI"; }
            else if (response.ToUpper().Contains("INFINITI")) { make = "INFINITI"; }
            else if (response.ToUpper().Contains("ISUZU")) { make = "ISUZU"; }
            else if (response.ToUpper().Contains("JAGUAR")) { make = "JAGUAR"; }
            else if (response.ToUpper().Contains("JEEP")) { make = "JEEP"; }
            else if (response.ToUpper().Contains("KAWASAKI")) { make = "KAWASAKI"; }
            else if (response.ToUpper().Contains("KIA")) { make = "KIA"; }
            else if (response.ToUpper().Contains("LAMBORGHINI")) { make = "LAMBORGHINI"; }
            else if (response.ToUpper().Contains("LAND ROVER")) { make = "LAND ROVER"; }
            else if (response.ToUpper().Contains("LEXUS")) { make = "LEXUS"; }
            else if (response.ToUpper().Contains("LINCOLN")) { make = "LINCOLN"; }
            else if (response.ToUpper().Contains("LOTUS")) { make = "LOTUS"; }
            else if (response.ToUpper().Contains("MASERATI")) { make = "MASERATI"; }
            else if (response.ToUpper().Contains("MAZDA")) { make = "MAZDA"; }
            else if (response.ToUpper().Contains("MCLAREN AUTOMOTIVE")) { make = "MCLAREN AUTOMOTIVE"; }
            else if (response.ToUpper().Contains("MERCEDES-BENZ")) { make = "MERCEDES-BENZ"; }
            else if (response.ToUpper().Contains("MINI")) { make = "MINI"; }
            else if (response.ToUpper().Contains("MITSUBISHI")) { make = "MITSUBISHI"; }
            else if (response.ToUpper().Contains("NISSAN")) { make = "NISSAN"; }
            else if (response.ToUpper().Contains("PORSCHE")) { make = "PORSCHE"; }
            else if (response.ToUpper().Contains("RAM")) { make = "RAM"; }
            else if (response.ToUpper().Contains("ROLLS-ROYCE")) { make = "ROLLS-ROYCE"; }
            else if (response.ToUpper().Contains("SMART")) { make = "SMART"; }
            else if (response.ToUpper().Contains("SUBARU")) { make = "SUBARU"; }
            else if (response.ToUpper().Contains("SUZUKI")) { make = "SUZUKI"; }
            else if (response.ToUpper().Contains("TESLA")) { make = "TESLA"; }
            else if (response.ToUpper().Contains("TOYOTA")) { make = "TOYOTA"; }
            else if (response.ToUpper().Contains("VOLKSWAGEN")) { make = "VOLKSWAGEN"; }
            else if (response.ToUpper().Contains("VOLVO")) { make = "VOLVO"; }
            else if (response.ToUpper().Contains("YAMAHA")) { make = "YAMAHA"; }
            else { make = "FALSE"; }

            if (year != "FALSE" && make != "FALSE")
            {              
                switch (vehicleNum)
                {
                    case 1:
                      
                        App.getAgent().selectData("vehicle-year", year);
                       
                         App.getAgent().selectData("vehicle-make", make); 
                        break;
                    case 2:
                        App.getAgent().selectData("vehiqcle2-year", year);
                       
                        App.getAgent().selectData("vehicle2-make", make); 
                        break;
                    case 3:
                        App.getAgent().selectData("vehicle3-year", year);
                        App.getAgent().selectData("vehicle3-make", make); 
                        break;
                    case 4:
                        App.getAgent().selectData("vehicle4-year", year);
                        App.getAgent().selectData("vehicle4-make", make); 
                        break;
                }
                Thread.Sleep(300);
                Console.WriteLine(Modelcontrol);

                models = driver.FindElementById(Modelcontrol);
                IReadOnlyCollection<OpenQA.Selenium.IWebElement> theModels = models.FindElements(OpenQA.Selenium.By.TagName("option"));
                foreach (OpenQA.Selenium.IWebElement option in theModels)
                {
                    Console.WriteLine("Searching...." + option.Text);
                    searcher = option.Text.Split(' ')[0];
                    if (response.Contains(searcher.ToLower()))
                    {
                        Console.WriteLine("FOUND MODEL!" + option.Text);                    
                        model = option.Text;
                        switch(vehicleNum)
                        {
                            case 1:
                                 temp.selectData("vehicle-model", model);
                                
                               
                         
                                break;
                            case 2:
                                temp.selectData("vehicle2-model", model);
                                
                                break;
                            case 3:
                                temp.selectData("vehicle3-model", model);
                              
                                break;
                            case 4:
                                temp.selectData("vehicle4-model", model);

                                break;
                        }
                        
                        return (model);
                    }
                }
            }
            return (year + " " + make + " " + model);
        }


        public string checkIfSecondaries(string phrase)
        {

            if (phrase.Contains("yes") || phrase.Contains("sure")) { return "YES"; }
            else   { return "NONE"; }
        }
        public bool ParseDOB(string response,int spouse)
        {
            string month = "";
            string respo = response.ToLower();
            string DayYear = BDAYHOLDER;
            Console.WriteLine(BDAYHOLDER);
            string day = "";
            string year = "";
            if (respo.Contains("jan") || respo.Contains("feb") || respo.Contains("mar") || respo.Contains("apr") || respo.Contains("may")
                || respo.Contains("jun") || respo.Contains("jul") || respo.Contains("aug") || respo.Contains("sep"))
            {
                month = DayYear.Substring(0, 1);
                DayYear = DayYear.Substring(1);
                Console.WriteLine("MONTH WAS 1-9");
            }

            else
            {
                month = DayYear.Substring(0, 2);
                DayYear = DayYear.Substring(2);
                    Console.WriteLine("MONTH WAS 10-12");
                                
            }
            switch(DayYear.Length)
            {
                case 3:
                case 5:
                    day = DayYear.Substring(0, 1);
                    year = DayYear.Substring(1);
                    break;
                case 4:
                case 6:
                    day = DayYear.Substring(0, 2);
                    year = "19" + DayYear.Substring(2);
                    break;

            }
            Console.WriteLine("DAY: " + day);
            Console.WriteLine("YEAR: " + year);
            Console.WriteLine(spouse);
            if(day != "" && year != "") {
                if (spouse == 0)
                {

                    selectData("frmDOB_Day", day);
                    selectData("frmDOB_Month", MonthFromNumeral(month));
                    selectData("frmDOB_Year", year);
                    return true;
                }
                else
                {
                    selectData("frmSpouseDOB_Day", day);
                    selectData("frmSpouseDOB_Month", MonthFromNumeral(month));
                    selectData("frmSpouseDOB_Year", year);
                    return true;

                }
            }
            else { return false; }
        }
         public bool ParseEmail(string response)
        {
            string boogabooga = response.TrimEnd('.', '?', '!');
            string[] temp = boogabooga.Split(' ');
            
            string email = "";
            for(int i = temp.Length-1;i>0;i--)
            {
                if(temp[i] == "at") { temp[i] = "@";break; }         
            }
            for(int j = 0; j < temp.Length; j++) { email += temp[j]; }
            Console.WriteLine(email);
            WebRequest req = WebRequest.Create("http://apilayer.net/api/check?access_key=c6176c5026425635d9eb177b9989de66&email=" + email + "&smtp=1&format=1");
            req.ContentType = "application/json";
            req.Method = "POST";
            WebResponse resp = req.GetResponse();
            StreamReader r = new StreamReader(resp.GetResponseStream());
            String results = r.ReadToEnd();
            r.Close();
            Console.WriteLine(results);
            if (results.Contains("smtp_check\":true,")) { EnterData("frmEmailAddress", email); return true; }
            else { return false; }

            
          

        }
        public string MonthFromNumeral(string monthNum)
        {
            switch(monthNum)
            {
                case "1":return "Jan";
                case "2": return "Feb";
                case "3": return "Mar"; 
                case "4": return "Apr"; 
                case "5": return "May"; 
                case "6": return "Jun"; 
                case "7": return "Jul"; 
                case "8": return "Aug"; 
                case "9": return "Sep"; 
                case "10": return "Oct"; 
                case "11": return "Nov"; 
                case "12": return "Dec";
                default:
                    return "Dec";
            }
        }
        public  static async Task<bool> checkforData(string response)
        {
            string Data;
            bool mrMeseeks = true;
            bool isrebuttaling = false;
            App.getAgent().cust.speech = response;
            Console.WriteLine(App.getAgent().cust.speech);
            Console.WriteLine("CHECKING FOR DATAS");
            Console.WriteLine("QUESTION: " + temp.Question);
            string raw = response;
            response = response.ToLower();
           
            switch (temp.Question)
            {
                case Agent.STARTYMCSTARTFACE:

                    if (temp.cust.isNameEnabled)
                    {
                        if (response.Contains("yes") || response.Contains("speaking") || response.Contains("this is") || response.Contains("yeah") || response.Contains("hi") || response.Contains("yup") || response.Contains("sure is") || response.Contains("you've got him") )
                        {
                            Thread.Sleep(300);
                            App.getAgent().custObjected = false;
                            temp.Question = Agent.INTRO;
                            App.getAgent().AskQuestion();
                        }
                        
                        else if (response.Contains("hello"))
                        {
                            if (!isrebuttaling)
                            {
                                isrebuttaling = true;

                                App.RollTheClip(App.findNameClips(App.getWindow().btnTheirName.Content.ToString())[1]);
                            }
                        }
                        else if (response.Contains("no this is not") || response.Contains("no it isn't") || response.Contains("no it is not") || response.Contains("ain't") || response.Contains("aint"))
                        {
                            Thread.Sleep(300);
                            if (!isrebuttaling)
                            {
                                isrebuttaling = true;
                                string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Is This the Spouse.mp3";
                                bool x = await App.RollTheClipAndWait(clip);
                                temp.Question = "SPOUSE?";
                            }

                        }
                       
                    }
                    else
                    {
                        temp.Question = INTRO;
                    }
                    break;
                case "SPOUSE?":
                    if (response.Contains("yes") || response.Contains("speaking") || response.Contains("yup") || response.Contains("yeah") || response.Contains("this is") || response.Contains("yep") || response.Contains("it is"))
                    {
                        App.getAgent().custObjected = false;
                        temp.Question = Agent.INTRO;
                        App.getAgent().AskQuestion();
                    }
                    else if (response.Contains("no"))
                    {

                        bool x =  await App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                        temp.HangUpandDispo("Not Available");

                    }
                    break;
                case Agent.INTRO: // fall through
                case Agent.PROVIDER:
                    Console.WriteLine(Agent.PROVIDER);

                    Data = CheckIProvider(response);
                    if (Data != "FALSE")
                    {
                        if (temp.selectData("frmInsuranceCarrier", Data))
                        {
                            
                            Console.WriteLine("Val is: " + temp.driver.FindElementById("frmInsuranceCarrier").GetAttribute("value"));                                                
                            if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                            Console.WriteLine("put stuff in, current question is: " + temp.Question);
                            App.RESULTS.Add(Agent.PROVIDER, true);
                        }

                    } else
                    {
                        mrMeseeks = false;
                        App.RESULTS.Add(Agent.PROVIDER, false);
                    }

                    if (temp.driver.FindElementById("frmInsuranceCarrier").GetAttribute("value") != "")
                    {

                        App.failedEntry = false;
                    }
                    else
                    {
                        App.failedEntry = true;

                    }
                    break;
                case Agent.INS_EXP:

                    Data = checkExp(response);
                    string[] theDates = Data.Split(' ');
                    if (theDates.Length > 0)
                    {
                        if (temp.selectData("frmPolicyExpires_Month", theDates[0]) && temp.selectData("frmPolicyExpires_Year", theDates[1]))
                        {
                            temp.cust.expMonth = theDates[0];
                            temp.cust.expYear = theDates[1];

                            if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                        }
                        else { Console.WriteLine("error entering data EXP"); }
                    }
                    else
                    {
                        mrMeseeks = false;
                    }

                    break;
                case Agent.INST_START:
                    //Console.WriteLine("omg wtf bbq");
                    Data = temp.HowLong(response);
                    if (Data != "FALSE")
                    {
                        theDates = Data.Split(' ');
                        temp.selectData("frmPolicyStart_Month", theDates[0]);
                        temp.selectData("frmPolicyStart_Year", theDates[1]);
                        temp.Callpos = Agent.INBETWEEN;
                        //Console.WriteLine("\n BEYBLADE \n");                      
                    } 
                  
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.NUM_VEHICLES:
                    int data = temp.getNumVehicles(response);
                    if (data > 0 ) { if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; } temp.cust.numVehicles = data; }             
                    break;
                case Agent.YMM_ONLY_ONE:

                case Agent.YMM1:
                    BackgroundWorker bg = new BackgroundWorker();
                    bg.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        temp.Data = temp.GETYMM(response, 1);
                    });
                    bg.RunWorkerAsync();                   
                    break;
                case Agent.YMM2:
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        temp.Data = temp.GETYMM(response, 2);
                    });
                    bw.RunWorkerAsync();
                    break;
                case Agent.YMM3:
                    BackgroundWorker bz = new BackgroundWorker();
                    bz.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        temp.Data = temp.GETYMM(response, 3);
                    });
                    bz.RunWorkerAsync();
                    break;
                case Agent.YMM4:
                    BackgroundWorker bx = new BackgroundWorker();
                    bx.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        temp.Data = temp.GETYMM(response, 4);
                    });
                    bx.RunWorkerAsync();
                    break;
                case Agent.DOB:


                case Agent.MARITAL_STATUS:
                    var maritalStatus = App.getAgent().checkMaritalStatus(response);
                    if (maritalStatus.Length > 0)
                    {

                        temp.selectData("frmMaritalStatus", maritalStatus);
                        
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    temp.cust.maritalStatus = maritalStatus;
                    break;
                case Agent.SPOUSE_NAME:
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.SPOUSE_DOB:
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.OWN_OR_RENT:
                    var ownership = App.getAgent().checkOwnership(response);
                    if (ownership.Length > 0)
                    {
                        temp.selectData("frmResidenceType", ownership);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.RES_TYPE:
                    var resType = App.getAgent().checkResType(response);
                    if (resType.Length > 0)
                    {
                        temp.selectData("frmDwellingType", resType);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    break;

                case Agent.CREDIT:
                    var credit = App.getAgent().checkCredit(response);
                    if (credit.Length > 0)
                    {
                        temp.selectData("frmCreditRating", credit);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    break;
 
            
                case Agent.PHONE_TYPE:
                    var phoneType = App.getAgent().checkPhoneType(response);
                    if (phoneType.Length > 0)
                    {
                        temp.selectData("frmPhoneType1", phoneType);
                       
                    }

                    else
                    {
                        mrMeseeks = false;
                    }
                    if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.LAST_NAME:                      
                    break;
                case Agent.SECONDARIES:
                    if(temp.checkIfSecondaries(response) != "NONE") { if (temp.Callpos != Agent.FIXING) { temp.Callpos = Agent.INBETWEEN; } }
                              
                    break;
                case Agent.YEARBUILT:
                    if(temp.returnNumeric(response) != ""){ }
                    break;
                case Agent.SQFT:
                    if (temp.returnNumeric(response) != "") { }
                    break;                
                case Agent.PPC:
                    if (temp.returnNumeric(response) != "") { }
                    break;
                case Agent.TCPA:
                    
                    break;
            }
     
            return mrMeseeks;

        }
        public void FixLead()
        {
           string bad = App.getAgent().CheckLead();
            Console.WriteLine("FIXING::::" + bad);
                    switch(bad)
                    {
                          
                        case "frmLastName":
                            Console.WriteLine("recognized");
                             App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Last Name.mp3");
                             App.getAgent().Question = Agent.LAST_NAME;
                            
                              break;
                        case "frmPhoneType1":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\PHONETYPE.mp3");
                            App.getAgent().Question = Agent.PHONE_TYPE;
                             
               
                    break;
                        case "frmEmailAddress":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\EMAIL.mp3");
                            break;
                        case "frmAddress":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3");
                            break;
                        case "frmCity":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3");
                            break;
                        case "frmState":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3");
                            break;
                        case "frmPostalCode":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3");
                            break;
                        case "frmDwellingType":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\HOMETYPE.mp3");
                            break;
                        case "frmInsuranceCarrier":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3");
                            break;
                        case "frmPolicyExpires_Month":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3");
                            break;
                        case "frmPolicyExpires_Year":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3");
                            break;
                        case "frmPolicyStart_Month":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3");
                            break;
                        case "frmPolicyStart_Year":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3");
                            break;
                        case "vehicle-year":

                        case "vehicle-make":

                        case "vehicle-model":

                        case "vehicle-submodel":
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\YMMYV.mp3");
                            break;
                        case "frmDOB_Month":

                        case "frmDOB_Day":

                        case "frmDOB_Year":

                        case "frmGender":

                        case "frmTcpaConsumerConsented":
                     
                            break;
                    

                    }
            hasAsked = true;
            App.getAgent().Callpos = Agent.FIXING;


        }

        public bool ParseAddress(string response)
        {
            int i = 0;
            string[] addy = response.Split(' ');
            string AddressLine1 = "";
            for(  i = 0; i<addy.Length-1;i++)
            {       
                AddressLine1 += addy[i];
                if (addy[i].ToLower() == "street" || addy[i].ToLower() == "st" || addy[i].ToLower() == "road" || addy[i].ToLower() == "avenue" || addy[i].ToLower() == "boulevard" || addy[i].ToLower() == "terrace" || addy[i].ToLower() == "circle" || addy[i].ToLower() == "lane" || addy[i].ToLower() == "court") { break; }
            }
            string zip = addy[addy.Length-1];
            driver.FindElementById("frmAddress").Clear();
            driver.FindElementById("frmPostalCode").Clear();
            EnterData("frmAddress", AddressLine1);
            EnterData("frmPostalCode", zip);
            driver.FindElementById("btnValidate").Click();
            Thread.Sleep(250);
            try
            {
                Thread.Sleep(250);
                if (driver.FindElementByXPath("//*[@id=\"address_info\"]/tbody/tr[4]/td[2]/span[2]").GetAttribute("class") == "validation_result checkmark")
                {
                    Console.WriteLine("address is good");
                    return true;
                }
                else {
                    Console.WriteLine("address is bad");
                    hasAsked = true;
                    App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\can you repeat that.mp3");
                    return false;

                }
            }
            
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                Console.WriteLine("address is bad");
                hasAsked = true;
                App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\can you repeat that.mp3"); 
                return false;
                
            }
        
   

        }
        
       public bool CheckForMonth(string response)
        {
            if (response.Contains("jan")) { App.getAgent().selectData("frmDOB_Month","Jan"); return true; }
            if (response.Contains("feb")) { App.getAgent().selectData("frmDOB_Month", "Feb"); return true; }
            if (response.Contains("mar")) { App.getAgent().selectData("frmDOB_Month", "Mar"); return true; }
            if (response.Contains("apr")) { App.getAgent().selectData("frmDOB_Month", "Apr"); return true; }
            if (response.Contains("may")) { App.getAgent().selectData("frmDOB_Month", "May"); return true; }
            if (response.Contains("jun")) { App.getAgent().selectData("frmDOB_Month", "Jun"); return true; }
            if (response.Contains("july")) { App.getAgent().selectData("frmDOB_Month", "Jul"); return true; }
            if (response.Contains("aug")) { App.getAgent().selectData("frmDOB_Month", "Aug"); return true; }
            if (response.Contains("sep")) { App.getAgent().selectData("frmDOB_Month", "Sep"); return true; }
            if (response.Contains("oct")) { App.getAgent().selectData("frmDOB_Month", "Oct"); return true; }
            if (response.Contains("nov")) { App.getAgent().selectData("frmDOB_Month", "Nov"); return true; }
            if (response.Contains("dec")) { App.getAgent().selectData("frmDOB_Month", "Dec"); return true; }
            else { return false;}

        }

        public String returnNumeric(string response)
        {
            string resultString;
            resultString = System.Text.RegularExpressions.Regex.Match(response, @"\d+").Value;
            Console.WriteLine(resultString);
            return resultString;
        }
        public string GETPPC(string response)
        {
            if (response.Contains("15,000")) { return ("$15,000"); }
            else if (response.Contains("10,000")) { return ("$10,000"); }
            else if (response.Contains("15,000")) { return ("$15,000"); }
            else if (response.Contains("20,000")) { return ("$20,000"); }
            else if (response.Contains("15,000")) { return ("$15,000"); }
            else if (response.Contains("20,000")) { return ("$20,000"); }
            else if (response.Contains("25,000")) { return ("$25,000"); }
            else if (response.Contains("15,000")) { return ("$15,000"); }
            else if (response.Contains("20,000")) { return ("$20,000"); }
            else if (response.Contains("25,000")) { return ("$25,000"); }
            else if (response.Contains("30,000")) { return ("$30,000"); }
            else if (response.Contains("35,000")) { return ("$35,000"); }
            else if (response.Contains("40,000")) { return ("$40,000"); }
            else if (response.Contains("45,000")) { return ("$45,000"); }
            else if (response.Contains("50,000")) { return ("$50,000"); }
            else if (response.Contains("55,000")) { return ("$55,000"); }
            else if (response.Contains("60,000")) { return ("$60,000"); }
            else if (response.Contains("65,000")) { return ("$65,000"); }
            else if (response.Contains("70,000")) { return ("$70,000"); }
            else if (response.Contains("75,000")) { return ("$75,000"); }
            else if (response.Contains("80,000")) { return ("$80,000"); }
            else if (response.Contains("85,000")) { return ("$85,000"); }
            else if (response.Contains("90,000")) { return ("$90,000"); }
            else if (response.Contains("95,000")) { return ("$95,000"); }
            else if (response.Contains("100,000")) { return ("$100,000"); }
            else { return "$5,000"; }

        }




        //------------------------------------------------------------------
        public string checkPhoneType(string response)
        {
            if (response.Contains("cell") || response.Contains("mobile") || response.Contains("that's awesome"))
            {
                return "Mobile/Cell";
            }
            else if (response.Contains("work") || response.Contains("office"))
            {
                return "Work";
            }
            else if (response.Contains("home") || response.Contains("landline"))
            {
                return "Home";
            }
            return "";
        }
        //------------------------------------------------------------------
        public string checkCredit(string response)
        {
            if (response.Contains("fair") || response.Contains("poor"))
            {
                return "Fair";
            }
            else if (response.Contains("good"))
            {
                return "Good";
            }
            else if (response.Contains("excellent"))
            {
                return "Excellent";
            }
            return "";
        }
        //------------------------------------------------------------------
        public string checkResType(string response)
        {
            if (response.Contains("single family") || response.Contains("a house") || response.Contains("single"))
            {
                return "Single Family";
            }
            else if (response.Contains("apartment"))
            {
                return "Apartment";
            }
            else if (response.Contains("duplex"))
            {
                return "Duplex";
            }
            else if (response.Contains("condo") || response.Contains("condominium"))
            {
                return "Condominium";
            }
            else if (response.Contains("townhome") || response.Contains("townhouse") || response.Contains("town house") || response.Contains("town home"))
            {
                return "Townhome";
            }
            else if (response.Contains("mobile home") || response.Contains("trailer"))
            {
                return "Mobile Home";
            }
            return "";
        }
        //------------------------------------------------------------------
        public string checkOwnership(string response)
        {
            if (response.Contains("own") || response.Contains("hone") || response.Contains("home"))
            {
                return "Own";
            }
            else if (response.Contains("rent") || response.Contains("went") || response.Contains("rant"))
            {
                return "Rent";
            }
            else if (response.Contains("live there"))
            {
                return "Other";
            }
            return "";
        }
        //------------------------------------------------------------------
        public string checkMaritalStatus(string response)
        {
            
            if (response.Contains("single") || response.Contains("bingo") || response.Contains("not together") || response.Contains("thing go"))
            {
                return "Single";
            }
            if (response.Contains("married") || response.Contains("marry") || response.Contains("bury") || response.Contains("buried") || response.Contains("mary") || response.Contains("merry") || response.Contains("larry"))
            {
                return "Married";
            }
            else if (response.Contains("divorce") || response.Contains("even worse") || response.Contains("did worse"))
            {
                return "Divorced";
            }
            else if (response.Contains("separate") || response.Contains("not together"))
            {
                return "Separated";
            }
            else if (response.Contains("widow") || response.Contains("window"))
            {
                return "Widowed";
            }
            else if (response.Contains("domestic"))
            {
                return "Domestic Partner";
            }

            return "";
        }
        //------------------------------------------------------------------
      
        //------------------------------------------------------------------------------------------------------------------------
        public static async Task <bool> checkForObjection(string response)
        {
            string resp = response;
            string clip;
            if (App.getAgent().currentlyRebuttaling == false)
            {
              

                if (resp.Contains("don't want it") || resp.Contains("no thank you") || resp.Contains("no thank you"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;

                }
                else if(resp.Contains("not here right now") || resp.Contains("leave a message") ||  resp.Contains("record your message") || resp.Contains("voicemail") || resp.Contains("mailbox") || resp.Contains("mail box") || resp.Contains("is full") || resp.Contains("press 2") || resp.Contains("satisfied with the message") || resp.Contains("the tone"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    App.getAgent().custObjected = true;
                    App.getAgent().notInterestedFutureBool = true;
                    App.getAgent().currentlyRebuttaling = true;

                    App.getAgent().HangUpandDispo("Not Available");

                }
                else if (resp.Contains("why do you need that"))
                {
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Why do you need my info.mp3";
                    App.getAgent().currentlyRebuttaling = true;
                    App.getAgent().custObjected = true;
                    App.getAgent().notInterestedFutureBool = true;
                    App.getAgent().currentlyRebuttaling = true;
                    bool x = await App.RollTheClipAndWait(clip);
                }
                else if (resp.Contains("what is this about") || resp.Contains("what's this about") || resp.Contains("why are you calling") || resp.Contains("what are you calling for") || resp.Contains("what's this 14") || resp.Contains("why you callin") || resp.Contains("what's this all about") || resp.Contains("purpose of your call"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    App.getAgent().custObjected = true;
                    App.getAgent().notInterestedFutureBool = true;
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
                    bool x = await App.RollTheClipAndWait(clip);

                }
                else if (resp.Contains("real busy") || resp.Contains("i'm at work") || resp.Contains("going to work") || resp.Contains("call back") || resp.Contains("the middle of something") || resp.Contains("can't right now") || resp.Contains("can't talk") || resp.Contains("busy now") || resp.Contains("busy right now"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\This Will be Real quick.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().notInterestedFutureBool = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;

                }
                else if (resp.Contains("not interested") || resp.Contains("no interest") || resp.Contains("don't need"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\nothing to be interested in.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().notInterestedFutureBool = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("take me off your") || resp.Contains("take me off your list") || resp.Contains("put me on your") || resp.Contains("do not call list") || resp.Contains("no call list") || resp.Contains("don't call me again") || resp.Contains("don't ever call me again"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\DNC.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    try
                    {
                        App.getAgent().HangUpandDispo("Do Not Call");
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("couldn't do it. I just. Couldn't. Do it.");
                    }
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("what did you say") || resp.Contains("what was that") || resp.Contains("could you repeat that") || resp.Contains("come again"))
                {
                    return true;
                }
                else if (resp.Contains("who are you") || resp.Contains("who is this") || resp.Contains("who is calling") || resp.Contains("whose calling") || resp.Contains("who's calling") || resp.Contains("who is calling") || resp.Contains("who's this"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\Soundboard\Cheryl\INTRO\CHERYLCALLING.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    bool y = await App.RollTheClipAndWait(App.findNameClips(App.getWindow().btnTheirName.Content.ToString())[1]);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;

                }
                else if (resp.Contains("what's lcn") || resp.Contains("what is lcn") || resp.Contains("I'll see you then") || resp.Contains("whats LCN") || resp.Contains("what is LCN") || resp.Contains("else in"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\What's LCN.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("already have") || resp.Contains("already got"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I already have insurance rebuttal.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("not giving you") || resp.Contains("none of your business") || resp.Contains("not giving that out") || resp.Contains("not getting that out") || resp.Contains("that's personal"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Let me Just confirm a few things.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("where'd you get") || resp.Contains("how did you get my number") || resp.Contains("get my info") || resp.Contains("where did you get"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    App.getAgent().custObjected = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\where did you get my info.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("happy with") || resp.Contains("fine with my insurance") || resp.Contains("i'm good") || resp.Contains("all set") || resp.Contains("satisfied"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    App.getAgent().custObjected = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\ThisIsJustAQuickProccess.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    App.getAgent().custObjected = true;
                    App.getAgent().currentlyRebuttaling = true;
                    return true;
                }

                else if (resp.Contains("wrong number"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    x = await App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                    App.getAgent().HangUpandDispo("Wrong Number");                   
                    return true;
                }
                else if (resp.Contains("don't have insurance") || resp.Contains("don't have the car") || resp.Contains("don't have a car") ||  resp.Contains("don't have a vehicle") || resp.Contains("don't own a vehicle") || resp.Contains("don't own a car") || resp.Contains("no car") || resp.Contains("no vehicle"))
                {
                    App.getAgent().currentlyRebuttaling = true;
                    App.getAgent().custObjected = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
                    bool x = await App.RollTheClipAndWait(clip);
                    x = await App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                    App.getAgent().HangUpandDispo("No Car");
                    return true;
                }

                else
                {
                    return false;
                }               
            }
            return true;                      
        }
    
        //-----------------------------------------------------------------------------------------------------------
        public bool Login(string AgentNumber)
        {
            AgentNum = AgentNumber;
          
            ChromeDriverService cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;
            driver = new  ChromeDriver(cds);
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
                Thread.Sleep(750);
                driver.FindElementById("select-campaign").Click();
                driver.FindElementById("select-campaign").FindElements(OpenQA.Selenium.By.TagName("option")).Last().Click(); 
                Thread.Sleep(250);
                driver.FindElementById("btn-submit").Click();
                LoggedIn = true;
                Task task = Task.Run(doAgentStatusRequest);               
            }
            catch
            {
                return false;
            }
            return true;
        }
        //---------------------------------------------------------------


        //------------------------------------------------------------------
        public void HangUpandDispo(string dispo)
        {
            
            MySqlConnection myConnection;
            MySqlCommand Add;
            Console.WriteLine("got called");
            try
            {
                App.getAgent().AskingBDay = false;
                App.getAgent().isListening = false;
                App.getAgent().isTalking = false;
                App.getAgent().SilenceTimer = 0;
                App.getAgent().calltime = 0;
                App.getAgent().inCall = false;
                calltime = 0;
                if (!App.waveOutIsStopped) { Application.Current.Dispatcher.Invoke((() => { App.StopTheClip(); })); }
                if (App.getAgent().isListening) { App.longDictationClient.EndMicAndRecognition(); }
                string hangupDisp = "http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_hangup&value=1";
                Console.WriteLine("API CALL TO YTEL: " + hangupDisp);
                WebRequest h = WebRequest.Create(hangupDisp);
                WebResponse r = h.GetResponse();
                Thread.Sleep(300);
                r.Close();
                switch (dispo)
                {
                    case "hangup":

                        if (!endcall)
                        {
                            if (notInterestedFutureBool)
                            {

                                h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NI");
                                r = h.GetResponse();
                                myConnection = new MySqlConnection();
                                myConnection.ConnectionString =
                                "Server=sql9.freemysqlhosting.net;" +
                                "Database=sql9136099;" +
                                "Uid=sql9136099;" +
                                "Pwd=HvsN6cVwbx;";
                                myConnection.Open();
                                Add = new MySqlCommand("UPDATE `DISPO` SET `NI`=`NI` + 1", myConnection);
                                Add.ExecuteNonQuery();
                                myConnection.Close();
                                break;
                            }
                            else
                            {
                                h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NotAvl");
                                r = h.GetResponse();

                                myConnection = new MySqlConnection();
                                myConnection.ConnectionString =
                                "Server=sql9.freemysqlhosting.net;" +
                                "Database=sql9136099;" +
                                "Uid=sql9136099;" +
                                "Pwd=HvsN6cVwbx;";
                                myConnection.Open();
                                Add = new MySqlCommand("UPDATE `DISPO` SET `NA`=`NA` + 1", myConnection);
                                Add.ExecuteNonQuery();
                                myConnection.Close();
                                break;



                            }

                        }
                        break;
                    case "Not Available":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NotAvl");
                        r = h.GetResponse();

                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("UPDATE `DISPO` SET `NA`=`NA` + 1", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();
                        break;

                    case "Not Interested":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NI");
                        r = h.GetResponse();
                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("UPDATE `DISPO` SET `NI`=`NI` + 1", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();


                        break;
                    case "No Insurance":
                    case "NO Ins Transfer Unsuccessful":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NITU");
                        r = h.GetResponse();
                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("UPDATE `DISPO` SET `NO INS`=`NO INS` + 1", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();

                        break;
                    case "Do Not Call":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "DNC");
                        r = h.GetResponse();
                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("UPDATE `DISPO` SET `DNC`=`DNC` + 1", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();

                        break;
                    case "Wrong Number":
                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("UPDATE `DISPO` SET `WRONG NUM`=`WRONG NUM` + 1", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "Wrong");
                        r = h.GetResponse();
                        break;
                    case "No Car":
                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("UPDATE `DISPO` SET `NO CAR`=`NO CAR` + 1", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NoCar");
                        r = h.GetResponse();
                        break;
                    case "No English":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NoEng");
                        r = h.GetResponse();
                        break;
                    case "Auto Lead":

                        string name = cust.firstName + " " + driver.FindElementById("frmLastName").GetAttribute("value");
                        string phone = cust.phone;
                        myConnection = new MySqlConnection();
                        myConnection.ConnectionString =
                        "Server=sql9.freemysqlhosting.net;" +
                        "Database=sql9136099;" +
                        "Uid=sql9136099;" +
                        "Pwd=HvsN6cVwbx;";
                        myConnection.Open();
                        Add = new MySqlCommand("INSERT INTO `LEADS` (`AGENT`, `NAME`, `PHONE`, `LEAD_ID`, `LEAD_GUID`, `IMPORT_ID`) VALUES ('" + AgentNum + "','" + name + "','" + phone + "','" + cust.LeadID + "','" + cust.LEADGUID + "','" + cust.IMPORT_ID + "')", myConnection);
                        Add.ExecuteNonQuery();
                        myConnection.Close();

                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "1Auto");
                        r = h.GetResponse();
                        break;
                    case "LOW":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "LOW");
                        r = h.GetResponse();
                        break;
                        
                }
                r.Close();
                App.getAgent().Question = STARTYMCSTARTFACE;
            }
            

            // try

            //  {
            //   GHOST.Navigate().GoToUrl("https://rink.hockeyapp.net/apps/27b5c4930f7d4e52a31542739e6fda99");
            //  GHOST.FindElementById("user_email").SendKeys("JTSwagger@gmail.com");
            // GHOST.FindElementById("user_password").SendKeys("Jt55153910");
            // GHOST.FindElementByName("commit").Click();

            //Thread.Sleep(300);
            //  Console.WriteLine("CURRENT VERSION: " + version + ". SHOW ME WHAT YOU GOT!");
            //  drawHead();
            //  string versionToCheck = GHOST.FindElementByClassName("app-body").FindElement(OpenQA.Selenium.By.TagName("h3")).GetAttribute("innerHTML");
            //  versionToCheck = System.Text.RegularExpressions.Regex.Match(versionToCheck, @"\d+").Value;
            //  verToCheck = versionToCheck;
            //  if ( version !=  versionToCheck)
            //  {
            //     App.getWindow().speechTxtBox.AppendText(versionToCheck + " IS AVAILABLE! I LIKE WHAT YOU GOT");

            //     WebClient wc = new WebClient();
            //      string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BEST_APP_IN_THE_WORLD.zip";
            //     string path2 = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + '\\' ;
            //  try
            // {
            //   string dl = GHOST.FindElementByClassName("app-body").FindElement(OpenQA.Selenium.By.TagName("a")).GetAttribute("href");
            // App.getWindow().speechTxtBox.AppendText(dl);
            //wc.DownloadFileAsync(new Uri(dl), path);

            //   wc.DownloadFileCompleted += delegate
            //  {
            //      using (var zip = Ionic.Zip.ZipFile.Read(path))
            //     {
            //          Console.WriteLine("extracting " + path + " to: " + path2 + "AutoBotCSharpV" + App.getAgent().verToCheck);
            //          zip.ExtractProgress += zip_extract_progress;
            //         zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
            //          zip.ExtractAll(path2 + "AutoBotCSharpV" + App.getAgent().verToCheck, ExtractExistingFileAction.OverwriteSilently);

            //      }
            //   };
            //    version = GHOST.FindElementByClassName("icon-md5").GetAttribute("innerHTML");
            //    GHOST.Quit();               
            // }
            // catch(Exception e)
            // {
            //      App.getWindow().speechTxtBox.AppendText(e.StackTrace);

            // }



            // }
            // else
            //   {

            //  GHOST.Quit();
            //  }
            //  }
            //   catch(Exception e)
            //  {
            //      App.getWindow().speechTxtBox.AppendText(e.StackTrace);
            //  }
            //Thread.Sleep(400);
            //   r.Close();

            // }
            catch (Exception ex)
            {
                App.getWindow().speechTxtBox.AppendText(ex.StackTrace);
                Thread.Sleep(250);
                HangUpandDispo(dispo);
            }

         
        }
        static void zip_extract_progress(object sender, ExtractProgressEventArgs e)
        {
            Console.WriteLine("extracting..." );
            string path2 = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + '\\';
            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractAll)
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.UseShellExecute = true;
                start.Arguments = "version" + App.getAgent().verToCheck + " agent" + App.getAgent().AgentNum;
                Console.WriteLine(start.Arguments);
                start.FileName = "\"" + path2 + "AutoBotCSharpV" + App.getAgent().verToCheck + "\\" + "AutoBotCSharp.exe" + "\"";
                Console.WriteLine(start.FileName);
                Process.Start(start);
                App.getAgent().driver.Quit();
                Application.Current.Shutdown();
            }
        }
        public void drawHead()
        {
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNmdddhhdddmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMNdmMMMMMMMMMMMMMMMMMMMMMMMMMMNmhhso +/:-----------::+ shmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNdMMMMMMMMMMMM");
            Console.WriteLine("MMMmhMMMMMMMMNNMMMMMMMMMMMMMMMMmNMMMMNds +:-------------------------/ ohmNMMMMMMMMMMMMMMMMMMMMMMNmMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNdo: -----------.-------------------- -/ oydNMMMMMMMMMMMMMMMMMMNmMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMNNMMMMMMMMmy / ---:----------------------------------.-:+ymMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNs: --::--------:::::::://///////:::://///::::--:/smMMMmyMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMh / --::----:::::---------------------------:::::::--/ oymMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMNy-- -::----:---------------:::::::----------------------:+hNMMMMMMMMMMMMMMMMMMMMMMMMdmMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMNo--::::--.---- -:://////////////////////::::::::::::::/::::/odNNNMMMMMMMMMMMMMMMMMMMMdhNMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMd / --::::---- -:///:::--------------------:::::::::::::::-------::/+oydNMMMMMMMMMMMMMMMMMNMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMdNMMMMNy: --:::/:--:/:::----------------.................-------------------- -/ odNMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMNMMMMmo-- -:/://:-------------::::::------....................-------------------/dMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMd: --::/::/::------ -:/::::::::::::://///:::::-----------------::::///////:::::/shdNMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMNo-- -:::/ -/:::--://::-....................---::::://::::::::::::--................-:sNMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMNd: --::::+ -/:::://:......----:::://////////:::----.................--------:::::-----:/mMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMs--:::::/:-+:::/ +...--://///////:::::-:::::////////////:::::////////////////::::/:::/mMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMNs--:::::/:--+::::///::::-----::::--:::::--------------:::::::::::::::::/::::::--------sMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMNo--:::://:-.-//:::-------.-:::.````  ``..-::::-.-------------------/:--.``````.-:/:----:NMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMs--::///::::---//:--------::.``  `       `  ``-/:----------------:/-```           `-/----yMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMd--://:////:://--::--.----/.`                   `-/:------------:/-`   `             `::--/NMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMN / -:::///:::::://---------+`````                   `:/----------/-`                    `/:--hMMMMMMMMMMMmMMM");
            Console.WriteLine("MMMMMMMMMMMMh--::::::::::::::-------- / -               `       `  -/ --------+``                      `+--/ MMMMMMMMMMMmMMM");
            Console.WriteLine("MMMMMMMMMMMN: -:::::::::::::---------.+.             `//-          /:------::`             -o+        /:-:mMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMy--::::::::::::----------.+`           ` `oo.          /:.-----/ -              `+:        -/ --sMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMN / --:::::::::::-----------.+`                           /:----.-/:                 ``      -/ --+NMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMd--:::::::::::/:-----------/.                          `+-----..- +`              `  `      -/ --:NMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMh--:::::::::::+:-----------/ - -/ --..----:/` `                     +----mMMMMMMMMMMMNM");
            Console.WriteLine("hmMMMMMMMMy.-:::::::::::+-------------+`                        ./ --.../:--./ -`                     -/ ----hMMMMMMMMMMMMM");
            Console.WriteLine("NMMMMMMMMMo--:::::::::::+-------------:/```                    -/ --....- +:---/:`          `       `-/ ---:-oMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMM + --::::/:::::://------------.:/-``            ` ```.:/----....-+----:/.`             ``./:----::/MMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMNN / -:::::+--//:::+:--------------:/:.```   `````` `.::------...../:-----::-..`````...--:::------::/MMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMmmo--:://:-//::::/+-----------------::::::-------:/:--------.....-+:--------:::::::::-----------::/NMNmMMMmNMMMM");
            Console.WriteLine("MMMMMMMMMMy.-://---+:::::://-------------------------:----.---------......:+.---------------------------::/mMMMMMMmNMMMM");
            Console.WriteLine("MMMMMMMMMMd -:/:-..- +::::::://---------------------------------------.......+----------------------------:::mMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMN +:--...-/:::::::://:-------------------------------------.......-+--------------------------::::dMMMMMNMMMMMM");
            Console.WriteLine("MMMMMMMMMMm / -----..:/::::::::://:-----------------------------------........+:-------------------------::::mMMMMMsmMMMMM");
            Console.WriteLine("MMMMMMMMMMm: ---------::::::::::://::--------------------------------........:/---------------------------:/NMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMy--::///:----::::::::::::///:-----------------------------.......:+---------------------------:/NMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMN + -:::::///--:::::::::::::::::::-------------------::::::::::::://:----------------------------/MNdMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMh--::::::///:::::::::::::::::::::------------------::::::::--------------------------:::----::oMNmMMMMMMMMMM");
            Console.WriteLine("MMMMMNNMMMMMN:-:::::::://::::::::::::::::::::--------------------------------------------------::::::::::-yMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMs.-::::::::+:::::::::::::::::::--------------------------------------------------::::::::::::mMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMN / --:::::::/::::::::::::::::::----::--------------------------------------------::::::::::::/ NMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMd: --:::::::::::::::::::::::::::/:::----------------------------------------------://::::::-");
            Console.WriteLine("MMMMMMMNMMMMNNMh: --::::::::::::::::::::::::/:---------------------:::::/::::::::::::://:::--------//::::-dMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMmsmMMdhMMm: -:::::::::::::::::::::://-----------://///////::::--------------------:::////:---//::-+MMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMNdMMMMMMMMd:-:::::::::::::::::::::/:------:///::-------------------------------------.--:---:/:-/mMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMm + --::::::::::::::::::://-----:-------------------------------------------------:/:-:mMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMy: -::::::::::::::::::://::------------------------------------------------::///:-+mMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMm +::::::::::::::::::::::://::::------------------------------------------:::::-oNMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMNMMMh: -:::::::::::::::-----------------------------------------------------::::- oMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMm / -::::::::::::::::-------------------------------------------------- -:::-oNMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMN + --:::::::::::::::-------------------------------------------------:::-:MMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMs--:::::::::::::::------------------------------------------------:::-yMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMo--:::::::::::::::-----------------------------------------------::-:dMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMN + --::::::::::::::::-------------------------------------------- -::-/ NMMMMMMMMMMNMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMNhsNMMMMMMMMN + ---::::::::::::::::------------------------------------------ -:--yMMMMMMMMMMMNMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMNdMMMMMMMMMMMN + --:::::::::::::::::--------------------------------------------:mMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMm:.-:::::::::::::::::-------------------------------------------+NMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMNNMMMMMMMMMMMMMMMMMMMMMMMm / --:::::::::::::::::------------------------------------------sMMMMMMMMMMMNMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm / --:::::::::::::::-------------------------------------------hMMMMMMMMMMMNMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMd / --::::::::::::::------------------------------------------:mMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMNMMMMMMMMMMMMMMMMMMMMMMNd / --::::::::::::::-------------------------------------- -.- +MMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMNMMMMMMMMMMMMMMMMMMMMMMMMm / --::::::::::::::--------------------------------------.- sMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMm + --:::::::::::::----------------------------------------hMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMNyNMMMMMMMMNo--::::::::::::-------------------------------------- -:hMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMNMMMMMMMMMMNy-- -:::::::::::-------------------------------------:/ mMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh: --:::::::::::------------------------------------:oNMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmo--:::::::::::----------------------------------::yMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh + ---::::::::::------------------------------ -::/ mMMMMMMMMMMMMMMMMMMMMMMNmMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMdo / --:::::::::-----------------------------::+ dMMMMMMMMMMMMMMMMMMMMMMMNNMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNy: --::::::::::------------------------::+ hNMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNdo /:::::::::::::-----------------::/ ohNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmmMMMMMMNmhso//:::::::::----------::::/ohmMMMMMMMMMMMMMMMMMMMMMMMMMNNMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNdhys +///:::::::::::/+shNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Console.WriteLine("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNhMMMMMMMMMMMMNNmddhhhhhhdmNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void getDob()
        {
            string[] dob = new string[3]
            {
                new SelectElement(driver.FindElementById("frmDOB_Month")).SelectedOption.GetAttribute("value"),
                new SelectElement(driver.FindElementById("frmDOB_Day")).SelectedOption.GetAttribute("value"),
                new SelectElement(driver.FindElementById("frmDOB_Year")).SelectedOption.GetAttribute("value"),
            };

            dobInfo = dob;

            while(testing == true)
            {
                if (isTalking == false) { SilenceTimer += .2; Console.WriteLine("Silence is " + SilenceTimer + " seconds"); }
                if (SilenceTimer >= 2.4) { INPUTDEFAULT(); }
                if (SilenceTimer >= 4) { CheckForContact(SilenceTimer); }
                Thread.Sleep(200);
            }
           
        }
        public void killDriver()
        {
            try
            {
                driver.Quit();
            } catch (Exception)
            {
                // okey
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public async void setupBot()
        {
            cust = new Customer();
            int x = 0;
            Question = STARTYMCSTARTFACE;
            string firstName = "";
            while (driver.WindowHandles.Count < 2)
            {
                Console.WriteLine("shoop");
            }
            try
            {
            Console.WriteLine("count of driver.windowhandles: " + driver.WindowHandles.Count);
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            Console.WriteLine("driver title: " + driver.Title); 
            firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
                cust.LeadID = driver.Url.Split('&')[1].Replace("lead_id=","");
                cust.LEADGUID = driver.Url.Split('&')[2].Replace("lead_guid=", "");
                cust.IMPORT_ID = driver.Url.Split('&')[3].Replace("import_id=", "");
                Console.WriteLine("LEAD ID: " + driver.Url.Split('&')[1]);
            cust.firstName = firstName;
            }
            catch (Exception ex)
            {
                x += 1;
                if (x < 3)
                {
                    try
                    {
                        if (driver.PageSource.Contains("resource") || driver.PageSource.Contains("respectfully")) { HangUpandDispo("Not Available");return; }
                        else
                        {
                            Thread.Sleep(50);
                            firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
                            cust.phone = driver.FindElementById("frmPhone1").GetAttribute("value");
                            cust.firstName = firstName;
                        }
                    }
                    catch { }

                }
               else { HangUpandDispo("Not Available");}
                Console.WriteLine(ex.StackTrace);
            }
            try
            {
                if (maleNames.Contains(firstName)) { selectData("frmGender", "Male"); } else { EnterData("frmGender", "Female"); }
                string[] clips = App.findNameClips(firstName);
                Application.Current.Dispatcher.Invoke((() =>
                {
                    App.getWindow().setNameText(firstName);
                }));
                if (App.findNameClips(firstName)[0] == "no clip")
                {
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().setNameBtns(false);
                        cust.isNameEnabled = false;
                    }));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().setNameBtns(true);
                        cust.isNameEnabled = true;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
            }
            AskQuestion();
            await Task.Run((Action)getDob);
            App.longDictationClient.StartMicAndRecognition();
            started = true;
        }
        public void setupTesting()
        {
            string firstName = "";

            firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
            cust.firstName = firstName;
            cust.phone = "123-456-7890";
            AgentNum = "1198";
            try
            {
                if (maleNames.Contains(firstName)){ selectData("frmGender", "Male"); }else { EnterData("frmGender", "Female"); }
                string[] clips = App.findNameClips(firstName);
                Application.Current.Dispatcher.Invoke((() =>
                {
                    App.getWindow().setNameText(firstName);
                }));
                if (App.findNameClips(firstName)[0] == "no clip")
                {
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().setNameBtns(false);
                    }));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().setNameBtns(true);
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);

            }

          

            Question = STARTYMCSTARTFACE;
            cust.firstName = firstName;
            cust.isNameEnabled = true;
            
            AskQuestion();
            App.longDictationClient.StartMicAndRecognition();
            Task t = Task.Run((Action)getDob);

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
                cust.speech = "";
                App.longDictationClient.EndMicAndRecognition();
                isTalking = true;
                SilenceTimer = 0;
             
                switch (Question)
                {
                    case STARTYMCSTARTFACE:
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (App.findNameClips(cust.firstName)[0] != "no clip")
                            {
                                App.RollTheClip(App.findNameClips(cust.firstName)[0]);
                            }
                            else
                            {
                                Question = INTRO;
                                AskQuestion();
                            }
                        });
                        break;
                    case INTRO:
                        if (!App.getAgent().custObjected)
                            { App.RollTheClip(@"C:\Soundboard\Cheryl\INTRO\Intro2.mp3"); }
                            else { App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3"); }
                       
                        break;
                    case PROVIDER:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3");
                        break;
                    case INS_EXP:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3");
                        break;
                    case INST_START:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3");
                        break;
                    case NUM_VEHICLES:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\How many vehicles do you have.mp3");
                        break;
                    case YMM_ONLY_ONE:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\YMMYV.mp3");
                        break;
                    case YMM1:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\First Vehicle.mp3");
                        break;
                    case YMM2:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\2nd Vehicle.mp3");
                        break;
                    case YMM3:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\Third Vehicle.mp3");
                        break;
                    case YMM4:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\Fourth Vehicle.mp3");
                        break;
                    case DOB:
                        App.playDobClips();
                        break;
                    case BDAYMONTH:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\REBUTTALS\Can You Just Verify the month.mp3");
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
                    case SPOUSEBDAYMONTH:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\REBUTTALS\Can You Just Verify the month.mp3");
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
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3");
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
                    case SECONDARIES:
                        try
                        {
                            if (!App.RollTheClip(getSecondaryClip())) { Question = Agent.TCPA;  goto TCPA; };
                            break;
                        }
                        catch { goto TCPA; }
                    case TCPA:
                        TCPA:
                        
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\TCPA.mp3");                  
                        break;
                    case Agent.WHICHSECONDARIES:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Which secondaries.mp3");
                        break;
                    case YEARBUILT:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\YearBuilt.mp3");
                        break;               
                    case SQFT:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Square footage.mp3");
                        break;
                    case PPC:
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\PPCoverage.mp3");
                        break;
                    case "REPEAT":
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Can you repeat that.mp3");                  
                        break;
                }
                Callpos = INBETWEEN;
                hasAsked = true;
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
        
}
