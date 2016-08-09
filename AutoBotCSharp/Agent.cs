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
using OpenQA.Selenium.Support.UI;

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

        private bool newCall = false;
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


        public string[] dobInfo;

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
                    if (Dialer_Status == "READY")
                    {
                        newCall = true;
                    } else if (Dialer_Status == "INCALL")
                    {
                        if (newCall)
                        {
                            setupBot();
                            newCall = false;
                        }

                    }
                    //Console.WriteLine("Dialer Status: " + Dialer_Status);
                    //Console.WriteLine("Agent Name: " + Agent_Name);

                }
                catch
                {
                    for (int i = 0; i < tempstr.Length - 1; i++)

                    {
                        Console.WriteLine(tempstr[i]);
                    }
                }
                setGlobals();
                Thread.Sleep(500);
            }
        }
        //------------------------------------------------------------------------------------------------------
        private void setGlobals()
        {

            switch (Dialer_Status)
            {
                case "READY":
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().Background = Brushes.LightGoldenrodYellow;
                    }));

                    break;
                case "PAUSED":
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {

                        App.getWindow().Background = Brushes.IndianRed;
                    }));

                    break;
                case "INCALL":
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
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
                    Console.WriteLine("Generic Exception");
                    Console.WriteLine("Inner exception: " + ex.InnerException);
                    Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
            return false;
        }
        void StartWebRequest()
        {
            webRequest = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/non_agent.php?source=test&user=101&pass=API101IEpost&function=agent_status&agent_user=" + AgentNum + "&stage=csv&header=NO");
            resp = webRequest.GetResponse();
            reader = new StreamReader(resp.GetResponseStream());
        }
        public static string CheckIProvider(string s)
        {

            if (s.Contains("none") || s.Contains("no insurance") || s.Contains("don't have") || s.Contains("don't have insurance") || s.Contains("not insured") || s.Contains("not with anybody"))
            { return ("No current insurance"); }
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
            if (s.Contains("allstate") || s.Contains("all state") || s.Contains("ball state") || s.Contains("mall state"))
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
            if (s.Contains("geico") || s.Contains("gecko") || s.Contains("i go"))
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
            { return ("Nationwood Insurance Company"); }
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
            if (s.Contains("safeco") || s.Contains("safe co"))
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
            if (s.Contains("state farm") || s.Contains("statefarm") || s.Contains("haystack") || s.Contains("stay farm") || s.Contains("stayfarm"))
            { return ("State Farm General"); }
            if (s.Contains("state fund"))
            { return ("State Fund"); }
            if (s.Contains("state national"))
            { return ("State National Insurance"); }
            if (s.Contains("superior"))
            { return ("Superior Insurance"); }
            if (s.Contains("sure health") || s.Contains("sure"))
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
            if (s.Contains("USAA") || s.Contains("USA"))
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
            else { return ("FALSE"); }
        }

        public List<string> checkExp(string s)
        {
            string expMonth;
            string expyear;

            if(s.Contains("january"))
            {
                expMonth = "Jan";
            }
            else if(s.Contains("february"))
            {
                expMonth = "Feb";
            }

        }
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
        public static void checkforData(string response)
        {
            Agent temp = App.getAgent();
            string Data;
            string pos = temp.Callpos;
            switch (pos)
            {
                case Agent.INS_PROVIDER:
                    Data = CheckIProvider(response);
                    if(Data != "FALSE")
                    { if (temp.EnterData("frmInsuranceCarrier",Data)) { temp.Callpos = Agent.INS_EXP; }; }
                    break;
                case Agent.INS_EXP:
                    break;
                case Agent.INST_START:
                    break;
                case Agent.NUM_VEHICLES:
                    break;
                case Agent.YMM1:
                    break;
                case Agent.YMM2:
                    break;
                case Agent.YMM3:
                    break;
                case Agent.YMM4:
                    break;
                case Agent.DOB:
                    break;
                case Agent.MARITAL_STATUS:
                    break;
                case Agent.SPOUSE_NAME:
                    break;
                case Agent.SPOUSE_DOB:
                    break;
                case Agent.OWN_OR_RENT:
                    break;
                case Agent.RES_TYPE:
                    break;
                case Agent.CREDIT:
                    break;
                case Agent.ADDRESS:
                    break;
                case Agent.EMAIL:
                    break;
                case Agent.PHONE_TYPE:
                    break;
            }



        }
        public static bool checkForObjection(string response)
        {
            string resp = response;
            string clip;
            if (resp.Contains("not interested") || resp.Contains("no interest") || resp.Contains("don't want it"))
            {
                clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
                App.RollTheClip(clip);
                return true;
            }

            else if (resp.Contains("take me off your list") || resp.Contains("take me off your list"))
            {
                clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
                App.RollTheClip(clip);
                return true;
            }
            else { return false;
            }



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


        //------------------------------------------------------------------
        public void HangUpandDispo(string dispo)
        {
           
            try
            {
              WebRequest h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" +  AgentNum + "&function=external_hangup&value=1");
              WebResponse r = h.GetResponse();
                Thread.Sleep(250);
                r.Close();
                Thread.Sleep(250);
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
                Thread.Sleep(250);
                r.Close();
            }
            catch
            {
                Console.WriteLine("ERROR");
            }
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
        public void setupBot()
        {
            string firstName = "";
            while (driver.WindowHandles.Count < 2)
            {
                Console.WriteLine("shoop");
            }
            Console.WriteLine("count of driver.windowhandles: " + driver.WindowHandles.Count);
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            Console.WriteLine("driver title: " + driver.Title);
            firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
            
            try
            {
                string[] clips = App.findNameClips(firstName);
                System.Windows.Application.Current.Dispatcher.Invoke((() =>
                {
                    App.getWindow().setNameText(firstName);
                }));
                if (App.findNameClips(firstName)[0] == "no clip")
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().setNameBtns(false);
                    }));
                } else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
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
            Task.Run((Action)getDob);
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action) (() => App.getWindow().tabControlTop.SelectedIndex = 0));
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => App.getWindow().tabControlBottom.SelectedIndex = 0));
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
