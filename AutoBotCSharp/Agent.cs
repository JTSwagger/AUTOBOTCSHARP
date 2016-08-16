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
using System.ComponentModel;
using System.Windows;

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
        private bool started = false;
        public string Question;
        public const string INBETWEEN = "INBETWEEN";
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
        public ChromeDriver driver;


        public string[] dobInfo;
        private bool notInterestedFutureBool = false;

        WebRequest webRequest;
        WebResponse resp;
        StreamReader reader;
        //--------------------------------------------------------------------------------------------------------
        private double calltime = 0;
        public void doAgentStatusRequest()
        {

            while (LoggedIn)
            {
                
                StartWebRequest();
                string stats = reader.ReadToEnd();
                string[] tempstr = stats.Split(',');
                bool dead = false;
                try
                {
                    Dialer_Status = tempstr[0];
                    Agent_Name = tempstr[5];
                    try
                    {
                        foreach (string stat in tempstr)
                        {
                            if (stat.Contains("DEAD"))
                            {
                                dead = true;
                                break;
                            }
                        }
                    } catch (Exception)
                    {
                        Console.WriteLine("moo");
                    }
                    if (dead)
                    {
                        App.longDictationClient.EndMicAndRecognition();
                        autoDispo(calltime);
                    }
                    if (Dialer_Status == "READY")
                    {
                        newCall = true;
                        App.longDictationClient.EndMicAndRecognition();
                    } else if (Dialer_Status == "INCALL")
                    {
                        calltime += 0.5;
                        Console.WriteLine("calltime: " + calltime.ToString() + " seconds");
                        if (newCall)
                        {
                            setupBot();
                            notInterestedFutureBool = false;
                            calltime = 0;
                            newCall = false;
                        }
                        if (calltime >= 115)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                App.longDictationClient.EndMicAndRecognition();
                                App.longDictationClient.StartMicAndRecognition();
                            });
                        }

                    }

                }
                catch (IndexOutOfRangeException)
                {
                    if (stats.Contains("AGENT NOT LOGGED IN") && started)
                    {
                        MessageBox.Show("You're not logged in. K.");
                        try
                        {
                            driver.Quit();
                        } catch (Exception)
                        {
                            // letting go is important.
                        }
                        LoggedIn = false;
                    }   
                }
                setGlobals();
                Thread.Sleep(500);
            }
        }

        //------------------------------------------------------------------------------------------------------
        private /*static*/ void parseDOB(string response)
        {
            int month;
            // Tries to find the birth month as a word in the response.
            var possibleMonth = parseDOBWords(response);
            if (possibleMonth != 0)
            {
                month = possibleMonth;
            }
        }
        /*
         * Tries to find a word form of a month in the response string, and returns it in int form
         * Will return 0 if it can't find anything.
         */ 
        private static int parseDOBWords(string response)
        {
            int theMonth = 0;
            response = response.ToLower();
            string[] months = new string[12] 
            {
                "january","february","march",
                "april","may","june","july",
                "august","september","october","november","december"
            };
            foreach (string month in months)
            {
                if (response.Contains(month))
                {
                    theMonth = Array.IndexOf(months, month) + 1;
                }
            }
            
            return theMonth;
        }
        private static string[] parseDOBNums(string response, bool monthFound)
        {
            string[] jimmers = new string[2];
            int day, year;
            string numString = "";
            foreach (char c in response)
            {
                int temp;
                bool result = int.TryParse(c.ToString(), out temp);
                if (result)
                {
                    numString += temp.ToString();
                }
            }
            if (monthFound)
            {
                switch (numString.Length)
                {
                    // ex. 697
                    case 3:
                        int.TryParse(numString[0].ToString(), out day);
                        int.TryParse(numString.Substring(1, 2), out year);
                        break;
                    // ex. 0697
                    case 4:
                        int.TryParse(numString[1].ToString(), out day);
                        int.TryParse(numString.Substring(2, 2), out day);
                        break;
                    // ex 61997
                    case 5:
                        int.TryParse(numString[0].ToString(), out day);
                        int.TryParse(numString.Substring(1, 4), out year);
                        break;
                    // ex 061997
                    case 6:
                        int.TryParse(numString[1].ToString(), out day);
                        int.TryParse(numString.Substring(2, 4), out year);
                        break;
                }
            }
            

            return jimmers;
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
                    Console.WriteLine("Generic Exception");
                    Console.WriteLine("Inner exception: " + ex.InnerException);
                    Console.WriteLine("Message: " + ex.Message);
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
        //---------------------------------------------------------------------------------------------------
        public static string checkExp(string s)
        {
            List<string> Dates = new List<string>(2);
            string expMonth;
            string expyear;
            if (s.Contains("january"))
            {
                expMonth = "Jan";
                if (DateTime.Now.Month > 1)
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
                expMonth = "FALSE";
                expyear = "FALSE";
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
        }
        //--------------------------------------------------------------------------------------------------

        public string HowLong(string response)
        {
            string month = driver.FindElementById("frmPolicyExpiration_Month").Text;
            if (response.Contains("year"))
            {


                if (response.Contains("two"))
                {
                    return (month + " " + (DateTime.Now.Year - 2).ToString());
                }
                else if (response.Contains("three"))
                {
                    return (month + " " + (DateTime.Now.Year - 3).ToString());
                }
                else if (response.Contains("four"))
                {
                    return (month + " " + (DateTime.Now.Year - 4).ToString());
                }
                else if (response.Contains("five"))
                {
                    return (month + " " + (DateTime.Now.Year - 5).ToString());
                }
                if (response.Contains("six"))
                {
                    return (month + " " + (DateTime.Now.Year - 6).ToString());
                }
                else if (response.Contains("seven"))
                {
                    return (month + " " + (DateTime.Now.Year - 7).ToString());
                }
                else if (response.Contains("eight"))
                {
                    return (month + " " + (DateTime.Now.Year - 8).ToString());
                }
                else if (response.Contains("nine"))
                {
                    return (month + " " + (DateTime.Now.Year - 9).ToString());
                }
                if (response.Contains("ten"))
                {
                    return (month + " " + (DateTime.Now.Year - 10).ToString());
                }
                else if (response.Contains("eleven"))
                {
                    return (month + " " + (DateTime.Now.Year - 11).ToString());
                }
                else if (response.Contains("twelve"))
                {
                    return (month + " " + (DateTime.Now.Year - 12).ToString());
                }
                else if (response.Contains("thirteen"))
                {
                    return (month + " " + (DateTime.Now.Year - 13).ToString());
                }
                if (response.Contains("fourteen"))
                {
                    return (month + " " + (DateTime.Now.Year - 14).ToString());
                }
                else if (response.Contains("fiften"))
                {
                    return (month + " " + (DateTime.Now.Year - 15).ToString());
                }
                else if (response.Contains("sixteen"))
                {
                    return (month + " " + (DateTime.Now.Year - 16).ToString());
                }
                else if (response.Contains("seventeen"))
                {
                    return (month + " " + (DateTime.Now.Year - 17).ToString());
                }
                else if (response.Contains("eighteen"))
                {
                    return (month + " " + (DateTime.Now.Year - 18).ToString());
                }
                else if (response.Contains("nineteen"))
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
        public string getNumVehicles(string response)
        {
            if (response.Contains("1") || response.Contains("one"))
            { return "1"; }
            else if (response.Contains("2") || response.Contains("two"))
            { return "2"; }
            else if (response.Contains("3") || response.Contains("three"))
            { return "3"; }
            else if (response.Contains("4") || response.Contains("four"))
            { return "4"; }
            else { return "1"; }
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
            else if (response.Contains("2000")) { year = "2000"; }
            else if (response.Contains("2001")) { year = "2001"; }
            else if (response.Contains("2002")) { year = "2002"; }
            else if (response.Contains("2003")) { year = "2003"; }
            else if (response.Contains("2004")) { year = "2004"; }
            else if (response.Contains("2005")) { year = "2005"; }
            else if (response.Contains("2006")) { year = "2006"; }
            else if (response.Contains("2007")) { year = "2007"; }
            else if (response.Contains("2008")) { year = "2008"; }
            else if (response.Contains("2009")) { year = "2009"; }
            else if (response.Contains("2010")) { year = "2010"; }
            else if (response.Contains("2011")) { year = "2011"; }
            else if (response.Contains("2012")) { year = "2012"; }
            else if (response.Contains("2013")) { year = "2013"; }
            else if (response.Contains("2014")) { year = "2014"; }
            else if (response.Contains("2015")) { year = "2015"; }
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
                        App.getAgent().selectData("vehicle2-year", year);
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

                models = driver.FindElementById(Modelcontrol);
                IReadOnlyCollection<OpenQA.Selenium.IWebElement> theModels = models.FindElements(OpenQA.Selenium.By.TagName("option"));
                foreach (OpenQA.Selenium.IWebElement option in theModels)
                {
                    searcher = option.Text.Split(' ')[0];
                    if (response.Contains(searcher.ToLower())) { Console.WriteLine("FOUND MODEL!" + option.Text); model = option.Text; return (model); }
                }
            }
            return (year + " " + make + " " + model);
        }
        public static bool checkforData(string response)
        {
            Agent temp =  App.getAgent();

            bool mrMeseeks = true;
            string Data;
           
            string pos = temp.Question;
            switch (pos)
            {
                case Agent.INS_PROVIDER:
                    Console.WriteLine("INS_PROVIDER");
                    Data = CheckIProvider(response);
                    if (Data != "FALSE")
                    {
                        if (temp.selectData("frmInsuranceCarrier", Data))
                        {
                            temp.Callpos = Agent.INBETWEEN;
                        }
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.INS_EXP:
                    Data = checkExp(response);
                    string[] theDates = Data.Split(' ');
                    if (theDates.Length > 0)
                    {
                        if (temp.selectData("frmPolicyExpires_Month", theDates[0]) && temp.selectData("frmPolicyExpires_Year", theDates[1]))
                        {
                            temp.Callpos = Agent.INBETWEEN;
                        }
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.INST_START:
                    Data = temp.HowLong(response);
                    theDates = Data.Split(' ');
                    if (theDates.Length > 0) {
                        if (temp.selectData("frmPolicyStart_Month", theDates[0]) && temp.selectData("frmPolicyStart_Year", theDates[1]))
                        {
                            temp.Callpos = Agent.INBETWEEN;
                        }
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.NUM_VEHICLES:
                    Data = temp.getNumVehicles(response);
                    temp.Callpos = Agent.INBETWEEN;

                    break;
                case Agent.YMM1:
                    Data = temp.GETYMM(response, 1);
                    if (!Data.Contains("FALSE"))
                    {
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                        {
                            temp.selectData("vehicle-model", Data);
                            temp.Callpos = Agent.INBETWEEN;
                        });
                        bw.RunWorkerAsync();
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.YMM2:
                    Data = temp.GETYMM(response, 2);
                    if (!Data.Contains("FALSE"))
                    {
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                        {
                            temp.selectData("vehicle2-model", Data);
                            temp.Callpos = Agent.INBETWEEN;
                        });
                        bw.RunWorkerAsync();
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.YMM3:
                    Data = temp.GETYMM(response, 3);
                    if (!Data.Contains("FALSE"))
                    {
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                        {
                            temp.selectData("vehicle3-model", Data);
                            temp.Callpos = Agent.INBETWEEN;
                        });
                        bw.RunWorkerAsync();
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.YMM4:
                    Data = temp.GETYMM(response, 4);
                    if (!Data.Contains("FALSE"))
                    {
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                        {
                            temp.selectData("vehicle4-model", Data);
                            temp.Callpos = Agent.INBETWEEN;
                        });
                        bw.RunWorkerAsync();
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.DOB:
                    break;
                case Agent.MARITAL_STATUS:
                    var maritalStatus = temp.checkMaritalStatus(response);
                    if (maritalStatus.Length > 0)
                    {
                        temp.selectData("frmMaritalStatus", maritalStatus);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.SPOUSE_NAME:
                    break;
                case Agent.SPOUSE_DOB:
                    break;
                case Agent.OWN_OR_RENT:
                    var ownership = temp.checkOwnership(response);
                    if (ownership.Length > 0)
                    {
                       temp.selectData("frmResidenceType", ownership);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.RES_TYPE:
                    var resType = temp.checkResType(response);
                    if (resType.Length > 0)
                    {
                       temp.selectData("frmDwellingType", resType);
                    }
                    break;
                case Agent.CREDIT:
                    var credit = temp.checkCredit(response);
                    if (credit.Length > 0)
                    {
                        temp.selectData("frmCreditRating", credit);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    break;
                case Agent.ADDRESS:
                    string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3";
                    App.RollTheClip(clip);
                    temp.driver.FindElementById("btnValidate").Click();
                    break;
                case Agent.EMAIL:
                    break;
                case Agent.PHONE_TYPE:
                    var phoneType = temp.checkPhoneType(response);
                    if (phoneType.Length > 0)
                    {
                        temp.selectData("frmPhoneType1", phoneType);
                    }
                    break;
                case Agent.LAST_NAME:
                    clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Last Name.mp3";
                    App.RollTheClip(clip);
                    break;
                case Agent.TCPA:
                    if (temp.checkTCPAResponse(response))
                    {
                        temp.selectData("frmTcpaConsumerConsented", "Responded YES, said sure, I agree, that's okay, etc.");
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\ENDCALL.mp3");
                        //App.getAgent().driver.FindElementById("btnSubmit").Click();
                        temp.HangUpandDispo("Auto Lead");
                        temp.driver.FindElementById("btnSubmit").Click();
                    }
                    else
                    {
                        temp.selectData("frmTcpaConsumerConsented", "Responded NO, did not respond, hung up, etc.");
                        App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                        temp.HangUpandDispo("LOW");
                    }
                    break;
            }
            if (!mrMeseeks)
            {
                Console.WriteLine("\nMR MESEEKS FUCKING HATES YOU\n");
            }
            return mrMeseeks;
        }
        public bool checkTCPAResponse(string response)
        {
            if (response.Contains("yes") || response.Contains("sure") || response.Contains("alright"))
            {
                return true;
            }
            else if (response.Contains("no"))
            {
                return false;
            }
            return false;
        }
        //------------------------------------------------------------------
        public string checkPhoneType(string response)
        {
            if (response.Contains("cell") || response.Contains("mobile"))
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
            if (response.Contains("own"))
            {
                return "Own";
            }
            else if (response.Contains("rent"))
            {
                return "Rent";
            }
            return "";
        }
        //------------------------------------------------------------------
        public string checkMaritalStatus(string response)
        {
            if (response.Contains("married"))
            {
                return "Married";
            }
            else if (response.Contains("divorced"))
            {
                return "Divorced";
            }
            else if (response.Contains("separated") || response.Contains("not together"))
            {
                return "Separated";
            }
            else if (response.Contains("widow"))
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
        public void autoDispo(double calltime)
        {
            if (calltime < 10)
            {
                HangUpandDispo("Not Available");
            }
            else if (calltime > 10)
            {
                if (Callpos == "INTRO")
                {
                    HangUpandDispo("Not Interested");
                }
                if (notInterestedFutureBool)
                {
                    HangUpandDispo("Not Interested");
                }
            }
            else
            {
                HangUpandDispo("Not Available");
            }

        }
        //------------------------------------------------------------------------------------------------------------------------
        public static async Task<bool> checkForObjection(string response)
        {
            string resp = response;
            string clip;
            if (resp.Contains("don't want it"))
            {
                clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
                App.RollTheClip(clip);
                return true;
            }
            else if (resp.Contains("not interested") || resp.Contains("no interest"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\nothing to be interested in.mp3";
                App.RollTheClip(clip);
                App.getAgent().notInterestedFutureBool = true;
                return true;
            }
            else if (resp.Contains("take me off your list") || resp.Contains("take me off your list") || resp.Contains("put me on your"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\DNC.mp3";
                bool x = await App.RollTheClipAndWait(clip);
                try
                {
                    App.getAgent().HangUpandDispo("Do Not Call");
                }
                catch (Exception)
                {
                    Console.WriteLine("couldn't do it. I just. Couldn't. Do it.");
                }

                return true;
            }
            else if (resp.Contains("who are you") || resp.Contains("who is this"))
            {
                clip = @"C:\Soundboard\Cheryl\INTRO\CHERYLCALLING.mp3";
                App.RollTheClip(clip);
                return true;
            }
            else if (resp.Contains("what's lcn") || resp.Contains("what is lcn") || resp.Contains("what's LCN") || resp.Contains("what is LCN"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\What's LCN.mp3";
                App.RollTheClip(clip);
                return true;
            }
            else if (resp.Contains("already have") || resp.Contains("already got"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I already have insurance rebuttal.mp3";
                App.RollTheClip(clip);
                return true;
            }
            else if (resp.Contains("not giving you") || resp.Contains("none of your business") || resp.Contains("not giving that out"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Let me Just confirm a few things.mp3";
                App.RollTheClip(clip);
                return true;
            }
            else if (resp.Contains("wrong number"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
                bool x = await App.RollTheClipAndWait(clip);
                x = await App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                App.getAgent().HangUpandDispo("Wrong Number");
                return true;
            }
            else if (resp.Contains("don't have a car") || resp.Contains("don't own a vehicle") || resp.Contains("no car"))
            {
                clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
                bool x = await App.RollTheClipAndWait(clip);
                x = await App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                App.getAgent().HangUpandDispo("No Car");
                return true;
            }
            return false;
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
                WebRequest h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_hangup&value=1");
                WebResponse r = h.GetResponse();
                Thread.Sleep(250);
                r.Close();
                Thread.Sleep(250);
                switch (dispo)
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
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NoCar");
                        r = h.GetResponse();
                        break;
                    case "No English":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NoEng");
                        r = h.GetResponse();
                        break;
                    case "Auto Lead":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_hangup&value=1");
                        r = h.GetResponse();
                        PauseUnPause("PAUSE");
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "1Auto");
                        r = h.GetResponse();
                        break;
                    case "LOW":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "LOW");
                        r = h.GetResponse();
                        break;
                    case "Resource Not Found":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "ResNo");
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
                } else
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
            Task task = Task.Run((Action)getDob);
            started = true;
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
