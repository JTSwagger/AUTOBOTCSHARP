using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium;
using System.Net;
using System.IO;
using System.Windows.Media;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;
using Ionic.Zip;
using System.Diagnostics;
using System.Net.Sockets;
using NAudio.Wave;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace AutoBotCSharp
{
    public class Agent
    {
        public void setAutoFormQuestion(string Question)
        {
            AutoFormQuestion = Question;
        }
        public void setCallPos(string CPos)
        {
            Callpos = CPos;
        }
        public Customer getCustomer()
        {
            return (CurrentCustomer);
        }
        public class Conversation
        {
            private static WaveOut waveOut = new WaveOut();
            public static bool waveOutIsStopped = true;
            public async static  Task<bool> PlayHumanism()
            {
                bool b;
                string clip;
                switch (AutoFormQuestion)
                {

                    case Agent.INS_EXP:


                        clip = @"C:\SoundBoard\Cheryl\REACTIONS\OTAGC.mp3";
                        b = await Conversation.RollTheClipAndWait(clip);

                        break;
                    case Agent.RES_TYPE:
                        clip = @"C:\SoundBoard\Cheryl\REBUTTALS\we're almost done.mp3";
                        b = await Conversation.RollTheClipAndWait(clip);
                        break;

                }
                numrepeats = 0;
                Thread.Sleep(200);
                AskAutoFormQuestion();
                return true;
            }
            public static async Task<bool> RollTheClipAndWait(string Clip)
            {
                SilenceTimer = 0;
                //if (Clip == "no clip")
                //{
                //    return false;
                //}
                Console.WriteLine("PLAYING:---->" + Clip);
                try
                {

                    StopTheClip();
                    waveOut = new WaveOut();
                    waveOut.PlaybackStopped += onPlaybackStopped;
                    Mp3FileReader Reader = new Mp3FileReader(Clip);

                    waveOutIsStopped = false;
                    waveOut.Init(Reader);
                    waveOut.Play();
                    do
                    {
                        await Task.Delay(25);
                    } while (waveOutIsStopped == false);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }
            public static void StopTheClip()
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        waveOut.Stop();
                        waveOut.Dispose();
                        waveOutIsStopped = true;
                    });
                }
                catch
                {
                    Console.WriteLine("couldn't stop...probably not a clip playing.");
                }
            }

            public static bool RollTheClip(string Clip)
            {
                try
                {
                    StopTheClip();
                    waveOut = new WaveOut();
                    waveOut.PlaybackStopped += onPlaybackStopped;
                    Mp3FileReader Reader = new Mp3FileReader(Clip);
                    waveOut.Init(Reader);

                    Application.Current.Dispatcher.Invoke(async () => waveOut.Play());
                    waveOutIsStopped = false;
                    Console.WriteLine("CLIP: " + Clip + " CHERYL IS TALKING---> " + isTalking);
                    isTalking = true;
                    SilenceTimer = 0;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            public static void onPlaybackStopped(object sender, StoppedEventArgs e)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.getWindow().reco.clearSpeech();
                    App.getWindow().speechTxtBox.Clear();
                    Console.WriteLine(Callpos);
                    Console.WriteLine(AutoFormQuestion);
                    if (endcall == true) { endcall = false; HangUpandDispo("Auto Lead"); }
                    //if (getAgent().low_blow_bro) { getAgent().low_blow_bro = false; getAgent().HangUpandDispo("LOW"); }
                    App.getWindow().reco.Final_Speech = "";
                    App.getWindow().reco.partial_speech = "";
                    Console.WriteLine("CHERYL JUST REBUTTALED " + currentlyRebuttaling);
                    isTalking = false;
                    Console.WriteLine("PLAYBACK STOPPED. CHERYL TALKING: " + isTalking);
                    //Console.WriteLine(user.Callpos);
                    //Console.WriteLine(user.AutoFormQuestion);
                    waveOutIsStopped = true;
                    if (currentlyRebuttaling == true)
                    {
                        if (App.getWindow().cmbDispo.SelectedIndex == 4) { HangUpandDispo("Do Not Call"); }
                        currentlyRebuttaling = false;
                        currentlyRebuttaling = false;
                        Console.WriteLine(AutoFormQuestion);
                        numrepeats += 1;
                        if (AutoFormQuestion == "INTRO") { AutoFormQuestion = Agent.PROVIDER; }
                        AskAutoFormQuestion();
                        return;
                    }
                    if (Callpos == "INBETWEEN")
                    {
                        // hidden below is a massive switch statement...
                        switch (AutoFormQuestion)
                        {
                            case Agent.CALL_START:
                                Callpos = Agent.INTRO;
                                break;
                            case Agent.INTRO:
                                Callpos = Agent.INTRO;
                                break;
                            case "INS_PROVIDER":
                                Callpos = Agent.PROVIDER;
                                break;
                            case "INS_EXP":
                                Callpos = Agent.INS_EXP;
                                break;
                            case "INS_START":
                                Callpos = Agent.INS_START;
                                break;
                            case "NUM_VEHICLES":
                                Callpos = Agent.NUM_VEHICLES;
                                break;
                            case "YMM1":
                                Callpos = Agent.YMM1;
                                break;
                            case "YMM2":
                                Callpos = Agent.YMM2;
                                break;
                            case "YMM3":
                                Callpos = Agent.YMM3;
                                break;
                            case "YMM4":
                                Callpos = Agent.YMM4;
                                break;
                            case "DOB":
                                Callpos = Agent.DOB;
                                break;
                            case Agent.BDAYMONTH:
                                Callpos = Agent.BDAYMONTH;
                                break;
                            case "MARITAL_STATUS":
                                Callpos = Agent.MARITAL_STATUS;
                                break;
                            case "SPOUSE_NAME":
                                Callpos = Agent.SPOUSE_NAME;
                                break;
                            case "SPOUSE_DOB":
                                Callpos = Agent.SPOUSE_DOB;
                                break;
                            case Agent.SPOUSEBDAYMONTH:
                                Callpos = Agent.SPOUSEBDAYMONTH;
                                break;
                            case "OWN OR RENT":
                                Callpos = Agent.OWN_OR_RENT;
                                break;
                            case "RESIDENCE TYPE":
                                Callpos = Agent.RES_TYPE;
                                break;
                            case "CREDIT":
                                Callpos = Agent.CREDIT;
                                break;
                            case "ADDRESS":
                                Callpos = Agent.ADDRESS;
                                break;
                            case "EMAIL":
                                Callpos = Agent.EMAIL;
                                break;
                            case "PHONE TYPE":
                                Callpos = Agent.PHONE_TYPE;
                                break;
                            case "LAST NAME":
                                Callpos = Agent.LAST_NAME;
                                break;
                            case "SECONDARIES":
                                Callpos = Agent.SECONDARIES;
                                break;
                            //case Agent.WHICHSECONDARIES:
                            //    if (getAgent().doHome || getAgent().doRenters) { user.Callpos = Agent.YEARBUILT; }
                            //    else { user.Callpos = Agent.TCPA; break; }
                            //    break;
                            case "TCPA":
                                Callpos = Agent.TCPA;
                                break;
                            case "REPEAT":
                                Callpos = "REPEAT";
                                break;
                        }
                    }
                });
            }
        }

        public static int numrepeats = 0; // Times AutoFormQuestion has been asked
        public static int NIReps = 0; // Times a rebuttal has been used on that objection
                                      // public double timeout = 0; // timeout pushes unfinalized speech as emulated final speech
        public static int port = 6000;  //connects to python wrapper
        public static Socket sock;
        public string version = "Version 67";
        public static List<string> maleNames = new List<string>();  // List of names to assign gender
        public static bool endcall = false;
        public string BDAYHOLDER = "";
        public static bool AskingBDay = false;   //bool to determine whether we already have the bday
        public static bool isTalking = false;          //is Cheryl Talking?
        public static bool testing = false;
        public static double SilenceTimer = 0;
        public static bool currentlyRebuttaling = false;
        public static bool inCall = false;
        public static bool isListening = false;
        public static bool hasAsked = false;
        public static bool LoggedIn = false;
        public static string Campaign;
        public static string Understood_Speech = "";
        public static string Data = "FALSE";
        public static string AgentNum;
        public static string Agent_Name;
        public static string Dialer_Status;
        public static string Callpos;

        private static bool started = false;
        private static bool newCall = false;
        public static string AutoFormQuestion;

        //BELOW ARE CONSTANTS USED TO SET CALLPOS OR AutoFormQuestion
        public const string CALL_START = "START";
        public const string INBETWEEN = "INBETWEEN";
        public const string INTRO = "INTRO";
        public const string PROVIDER = "INS_PROVIDER";
        public const string INS_EXP = "INS_EXP";
        public const string INS_START = "INS_START";
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

        //ChromeDriver things
        public static ChromeDriver driver;
        //Customer Class to handle each call
        public class AutoForm
        {
            public const string FIRST_NAME = "frmFirstName";
            public const string LAST_NAME = "frmLastName";
            public const string PHONE_NUMBER = "frmPhone1";
            public const string PHONE_TYPE = "frmPhoneType1";
            public const string EMAIL = "frmEmail";
            public const string ADDRESS = "frmAddress";
            public const string CITY = "frmCity";
            public const string STATE = "frmState";
            public const string ZIP = "frmPostalCode";
            public const string OWN_RENT = "frmResidenceType";
            public const string RESIDENCE_TYPE = "frmDwellingType";
            public const string CREDIT_RATING = "frmCreditRating";
            public const string INSURANCE_PROVIDER = "frmInsuranceCarrier";
            public const string INSURANCE_EXPIRATION_MONTH = "frmPolicyExpires_Month";
            public const string INSURANCE_EXPIRATION_YEAR = "frmPolicyExpires_Year";
            public const string INSURANCE_START_MONTH = "frmPolicyStart_Month";
            public const string INSURANCE_START_YEAR = "frmPolicyStart_Year";

            public const string VEHICLE_1_YEAR = "vehicle-year";
            public const string VEHICLE_1_MAKE = "vehicle-make";
            public const string VEHICLE_1_MODEL = "vehicle-model";

            public const string VEHICLE_2_YEAR = "vehicle2-year";
            public const string VEHICLE_2_MAKE = "vehicle2-make";
            public const string VEHICLE_2_MODEL = "vehicle2-model";

            public const string VEHICLE_3_YEAR = "vehicle3-year";
            public const string VEHICLE_3_MAKE = "vehicle3-make";
            public const string VEHICLE_3_MODEL = "vehicle3-model";

            public const string VEHICLE_4_YEAR = "vehicle4-year";
            public const string VEHICLE_4_MAKE = "vehicle4-make";
            public const string VEHICLE_4_MODEL = "vehicle4-model";

            public const string DOB_MONTH = "frmDOB_Month";
            public const string DOB_DAY = "frmDOB_Day";
            public const string DOB_YEAR = "frmDOB_Year";

            public const string GENDER = "frmGender";
            public const string MARITAL_STATUS = "frmMaritalStatus";
            public const string SPOUSE_FIRST_NAME = "frmSpouseFirstName";
            public const string SPOUSE_LAST_NAME = "frmSpouseLastName";
            public const string SPOUSE_DOB_MONTH = "frmDOB_Month";
            public const string SPOUSE_DOB_DAY = "frmDOB_Day";
            public const string SPOUSE_DOB_YEAR = "frmDOB_Year";
            public const string SPOUSE_GENDER = "frmSpouseGender";
            public const string TCPA = "frmTcpaConsumerConsented";
        } // Class of Constants to use with the LeadForm
        public class Customer
        {
            public class Vehicle
            {
                public string Year { get; set; }
                public string Make { get; set; }
                public string Model { get; set; }

            } // vehicle class

            public bool Objected { get; set; } = false;
            public string IMPORT_ID { get; set; }
            public string LEADGUID { get; set; }
            public string LeadID { get; set; }
            public string speech { get; set; }
            public string phone { get; set; }
            public string IProvider { get; set; }
            public int numVehicles { get; set; }
            public Vehicle Vehicle_1 { get; set; }
            public Vehicle Vehicle_2 { get; set; }
            public Vehicle Vehicle_3 { get; set; }
            public Vehicle Vehicle_4 { get; set; }
            public string maritalStatus { get; set; }
            public string firstName { get; set; }
            public bool isNameEnabled { get; set; }
            public string expMonth { get; set; }
            public string expYear { get; set; }
            public string lastName { get; set; }
            // public bool doHome = false;
            // public bool doLife = false;
            // public bool doHealth = false;           //secondary bool checks
            // public bool doMedicare = false;
            // public bool doRenters = false;
        } //CUSTOMER CLASS
        public static Customer CurrentCustomer = new Customer { numVehicles = 0, maritalStatus = "Single", speech = "", LeadID = "", lastName = "" };
        public static string[] dobInfo;
        public static WebRequest webRequest;
        public static WebResponse resp;
        public static StreamReader reader;
        private static double calltime = 0;
        public bool DoesNameClipExist()
        {

            return (CurrentCustomer.isNameEnabled);
        }
        public string[] getDOBInfo()
        {
            return (dobInfo);
        }
        public string getDialerStatus()
        {
            return Dialer_Status;
        }    
        public bool Talking()
        {
            return isTalking;
        }
        public bool Rebuttaling()
        {
            return currentlyRebuttaling;
        }
        public void Rebuttaling(bool val)
        {
            currentlyRebuttaling = val;
        }
        public void loadNames()
        {
            try
            {
                string thepath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MALENAMES.txt";
                using (StreamReader r = new StreamReader(thepath))
                {
                    // 3
                    // Use while != null pattern for loop
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        // 4
                        // Insert logic here.
                        // ...
                        // "line" is a line in the file. Add it to our List.
                        maleNames.Add(line);
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("MALENAMES.txt Not found", "Ya dun goofed");
            }
        }
        public void setAgentNum(string Agent_Num)
        {
            AgentNum = Agent_Num;
        }
        public static void DebugOutput(string Current_Function)
        {
            Console.WriteLine("------CURRENT FUNCTION----->  " + Current_Function);
            Console.WriteLine("Current Agent Variables:");
            Console.WriteLine("");
            Console.WriteLine("AutoFormQuestion:-------> " + AutoFormQuestion);
            Console.WriteLine("inCall:-----------------> " + inCall);
            Console.WriteLine("IsTalking:--------------> " + isTalking);
            Console.WriteLine("IsListening:------------> " + isListening);
            Console.WriteLine("HasAsked:---------------> " + hasAsked);          
            Console.WriteLine("CurrentlyRebuttaling----> " + currentlyRebuttaling);
            Console.WriteLine("");
            Console.WriteLine("Current Customer Variables");
            Console.WriteLine("");
            Console.WriteLine("Customer First Name:----> " + CurrentCustomer.firstName );
            Console.WriteLine("Customer Last Name:-----> " + CurrentCustomer.lastName);
            Console.WriteLine("Customer.Objected-------> " + CurrentCustomer.Objected);
            Console.WriteLine("Customer.Num Vehicles:--> " + CurrentCustomer.numVehicles);       
        }
        public bool Asked()
        {
            return (hasAsked);
        }
        public void Asked(bool val)
        {
            hasAsked = val;
        }
        // This will get the value of the field provided using Driver
        public static string getValueOfField(string field)
        {
            try
            {
                string val = driver.FindElementById(field).GetAttribute("value");
                return val;
            }
            catch (Exception e)
            {
                Console.WriteLine("error getting value of field---->" + e);
                return (null);
            }
        }
        //----------------------------------------------------------------------------
        //CHECK TO SEE IF THE CUSTOMER IS STILL ON THE PHONE BY SAYING HELLO 
        public static void CheckForContact(double time)
        {
            Console.WriteLine("was called successfully");
            if (!isTalking)
            {
                switch ((int)time)
                {
                    case 5:
                        Application.Current.Dispatcher.Invoke((() => Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello 1.mp3")));
                        SilenceTimer = 6;
                        break;
                    case 8:
                        Application.Current.Dispatcher.Invoke((() => Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello 2.mp3")));
                        SilenceTimer = 10;
                        break;
                    case 12:
                        Application.Current.Dispatcher.Invoke((() => Conversation.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\bad connection.mp3")));
                        Application.Current.Dispatcher.Invoke((() => HangUpandDispo("Not Available")));
                        break;
                }
            }
        }
        //----------------------------------------------------------------------------<<
        //  THIS CLEANS UP ANY INPUT BY MAKING IT UNIFORM (LOWERCASE NO PUNTUATION)
        public static string CleanUpResponse(string resp)
        {
            string response;
            response = resp.TrimEnd('.', '?', '!');
            response = response.Replace("'", "");
            response = response.Replace(",", "");
            response = response.ToLower();
            return response;
        }
        //---------------------------------------------
        //  THIS HANDLES CALL ADVANCEMENT
        public async void AdjustConvoPosition(string response)
        {
            CurrentCustomer.speech = response;
            DebugOutput("AdjustConvoPosition");

            if (!isTalking)
            {
                string DBCommand;
                Understood_Speech = CleanUpResponse(Understood_Speech);                       
                switch (AutoFormQuestion)
                {
                    case Agent.CALL_START:      //Beginning of call
                        break;
                    case Agent.INTRO:           //Introduction

                    case Agent.PROVIDER:        // Insurance Provider 
                        CurrentCustomer.IProvider = CheckIProvider(response);
                        if (getValueOfField(AutoForm.INSURANCE_PROVIDER) != "")
                        {
                            AutoFormQuestion = Agent.INS_EXP;
                            DBCommand = "INSERT INTO `INS_PROVIDER` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            hasAsked = false;
                            break;
                        }
                        else
                        {
                            hasAsked = true;
                        }
                        break;

                    case Agent.INS_EXP:
                        if (getValueOfField(AutoForm.INSURANCE_EXPIRATION_MONTH) != ""  && getValueOfField(AutoForm.INSURANCE_EXPIRATION_YEAR) != "")
                        {
                            DBCommand = "INSERT INTO `INS_EXP` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            AutoFormQuestion = Agent.INS_START;
                            hasAsked = false;
                        }
                        else { hasAsked = true; }
                        break;
                    case Agent.INS_START:
                        string Data = CheckProviderStart(response);
                        Console.WriteLine("CHECKING FOR START DATE: " + Data);
                        if (Data != "FALSE")
                        {
                            string[] theDates = Data.Split(' ');
                            EnterSelectElementData("frmPolicyStart_Month", theDates[0]);
                            EnterSelectElementData("frmPolicyStart_Year", theDates[1]);
                            Callpos = Agent.INBETWEEN;
                            //Console.WriteLine("\n BEYBLADE \n");                      
                        }
                        else
                        {
                            Console.WriteLine("COULDN'T DO IT.");
                        }
                        if (driver.FindElementById("frmPolicyStart_Month").GetAttribute("value") != "")
                        {
                            AutoFormQuestion = Agent.NUM_VEHICLES;
                            hasAsked = false;
                            DBCommand = "INSERT INTO `INS_START` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        }
                        else { hasAsked = true; }
                        break;

                    case Agent.NUM_VEHICLES:
                        hasAsked = true;
                        if (CurrentCustomer.numVehicles > 1)
                        {
                            AutoFormQuestion = Agent.YMM1;
                            hasAsked = false;
                        }
                        else if (CurrentCustomer.numVehicles == 1)
                        {
                            AutoFormQuestion = Agent.YMM_ONLY_ONE;
                            hasAsked = false;
                        }
                        else
                        {
                            CurrentCustomer.numVehicles = 1;
                            hasAsked = false;
                        }


                        break;
                    case Agent.YMM_ONLY_ONE:
                        if (driver.FindElementById("vehicle-make").Displayed)
                        {
                            AutoFormQuestion = Agent.DOB;
                            hasAsked = false;
                            DBCommand = "INSERT INTO `YMM_1` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        }
                        else
                        {
                            hasAsked = true;
                        }
                        break;
                    case Agent.YMM1:
                        if (driver.FindElementById("vehicle-make").Displayed)
                        {
                            AutoFormQuestion = Agent.YMM2;
                            hasAsked = false;
                            DBCommand = "INSERT INTO `YMM_1` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        }
                        else
                        {
                           hasAsked = true;
                        }
                        break;
                    case Agent.YMM2:
                        if (CurrentCustomer.numVehicles > 2)
                        {
                            if (driver.FindElementById("vehicle2-make").Displayed)
                            {
                                AutoFormQuestion = Agent.YMM3;
                                hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_2` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            }
                            else { hasAsked = true; }
                        }
                        else
                        {
                            if (driver.FindElementById("vehicle2-make").Displayed)
                            {
                                AutoFormQuestion = Agent.DOB;
                                hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_2` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            }
                            else { hasAsked = true; }
                            break;
                        }

                        break;
                    case Agent.YMM3:
                        if (CurrentCustomer.numVehicles > 3)
                        {
                            if (driver.FindElementById("vehicle3-make").Displayed)
                            {
                                AutoFormQuestion = Agent.YMM4;
                                hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_3` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            }
                            else { hasAsked = true; }
                        }
                        else
                        {
                            if (driver.FindElementById("vehicle3-make").Displayed)
                            {
                                AutoFormQuestion = Agent.DOB;
                                hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_3` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            }
                            else { hasAsked = true; }
                        }
                        break;
                    case Agent.YMM4:
                        if (driver.FindElementById("vehicle4-make").Displayed)
                        {
                            AutoFormQuestion = Agent.DOB;
                            hasAsked = false;


                            DBCommand = "INSERT INTO `YMM_4` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        }
                        else { hasAsked = true; }
                        break;

                    case Agent.DOB:
                        if (!AskingBDay)
                        {
                            string[] dobby = dobInfo;
                            if (dobby != null)
                            {
                                if (dobby[0] != "" && dobby[1] != "")
                                {
                                    if (response.ToLower().Contains("yes") || response.ToLower().Contains("yeah") || response.ToLower().Contains("right") || response.ToLower().Contains("correct") || response.ToLower().Contains("yup") || response.ToLower().Contains("yah"))
                                    {
                                        AutoFormQuestion = Agent.MARITAL_STATUS;

                                    }
                                    else if (response.ToLower().Contains("no") || response.ToLower().Contains("wrong") || response.ToLower().Contains("incorrect"))
                                    {
                                        bool x = await Conversation.RollTheClipAndWait(@"C:\Soundboard\Cheryl\DRIVER INFO\dob1.mp3");
                                        AskingBDay = true;
                                        dobby[0] = "";
                                        dobby[1] = "";

                                    }
                                }
                            }
                        }

                        else
                        {
                            AutoFormQuestion = Agent.MARITAL_STATUS;
                            hasAsked = false;
                        }
                        break;

                    case Agent.BDAYMONTH:
                        AutoFormQuestion = Agent.MARITAL_STATUS;
                        hasAsked = false;
                        break;

                    case Agent.MARITAL_STATUS:

                        if (CurrentCustomer.maritalStatus == "Married")
                        {
                            DBCommand = "INSERT INTO `MARITAL_STATUS` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            AutoFormQuestion = Agent.SPOUSE_NAME;
                            hasAsked = false;
                        }
                        else
                        {
                            DBCommand = "INSERT INTO `MARITAL_STATUS` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            AutoFormQuestion = Agent.OWN_OR_RENT;
                            hasAsked = false;
                        }
                        break;
                    case Agent.SPOUSE_NAME:
                        AutoFormQuestion = Agent.SPOUSE_DOB;
                        hasAsked = false;
                        EnterData("frmSpouseFirstName", response);
                        if (maleNames.Contains(response)) { EnterSelectElementData("frmGender", "Male"); } else { EnterData("frmGender", "Female"); }
                        break;
                    case Agent.SPOUSE_DOB:
                        string[] DayYear = response.Split(',');
                        Console.WriteLine(DayYear[0]);
                        if (DayYear.Length > 1)
                        {
                            DayYear[0] = returnNumeric(DayYear[0]);
                            DayYear[1] = returnNumeric(DayYear[1]);
                            Console.WriteLine("DAY: " + DayYear[0]);
                            EnterSelectElementData("frmSpouseDOB_Day", DayYear[0]);
                            Console.WriteLine("YEAR: " + DayYear[1]);
                            EnterSelectElementData("frmSpouseDOB_Year", DayYear[1]);
                        }
                        AutoFormQuestion = Agent.OWN_OR_RENT;
                        hasAsked = false;
                        break;
                    case Agent.OWN_OR_RENT: AutoFormQuestion = Agent.RES_TYPE; break;
                    case Agent.RES_TYPE: AutoFormQuestion = Agent.ADDRESS; break;
                    case Agent.ADDRESS:
                        Callpos = Agent.INBETWEEN;
                        AutoFormQuestion = Agent.EMAIL;
                        break;
                    case Agent.EMAIL:
                        Callpos = Agent.INBETWEEN;
                        AutoFormQuestion = Agent.CREDIT;
                        break;
                    case Agent.CREDIT: AutoFormQuestion = Agent.PHONE_TYPE; break;
                    case Agent.PHONE_TYPE:
                        CurrentCustomer.phone = driver.FindElementById("frmPhone1").GetAttribute("value");
                        AutoFormQuestion = Agent.LAST_NAME; break;
                    case Agent.LAST_NAME:
                        hasAsked = false;
                        AutoFormQuestion = Agent.TCPA;
                        break;
                    //case Agent.SECONDARIES:
                    //    if (response.Contains("no")) { AutoFormQuestion = Agent.TCPA; }
                    //    else { AutoFormQuestion = Agent.WHICHSECONDARIES; }
                    //    break;
                    //case Agent.WHICHSECONDARIES:
                    //    if (response.ToLower().Contains("home")) { getAgent().CheckBox("frmCrossSellHome"); getAgent().doHome = true; Console.WriteLine("HOME INSURANCE INTEREST DETECTED!"); }

                    //    else if (response.ToLower().Contains("rent")) { getAgent().CheckBox("frmCrossSellHome"); getAgent().doRenters = true; Console.WriteLine("RENTAL INSURANCE INTEREST DETECTED!"); }

                    //    if (response.ToLower().Contains("life")) { getAgent().CheckBox("frmCrossSellLife"); getAgent().doLife = true; Console.WriteLine("LIFE INSURANCE INTEREST DETECTED!"); }

                    //    if (response.ToLower().Contains("health")) { getAgent().CheckBox("frmCrossSellHealth"); getAgent().doHealth = true; Console.WriteLine("HEALTH INSURANCE INTEREST DETECTED!"); }

                    //    else if (response.ToLower().Contains("medicare")) { getAgent().CheckBox("frmCrossSellHealth"); getAgent().doMedicare = true; Console.WriteLine("MEDICARE INSURANCE INTEREST DETECTED!"); }
                    //    getAgent().Callpos = Agent.INBETWEEN;
                    //    //if (getAgent().doHome || getAgent().doRenters) { AutoFormQuestion = Agent.YEARBUILT; }
                    //    else { AutoFormQuestion = Agent.TCPA; break; }
                    //    break;
                    //case Agent.YEARBUILT:
                    //    string yearBuilt = getAgent().returnNumeric(response);
                    //    if (yearBuilt != "")
                    //    {
                    //        getAgent().selectData("frmYearBuilt", yearBuilt);
                    //        if (getAgent().doRenters) { getAgent().AutoFormQuestion = Agent.PPC; }
                    //        else { getAgent().AutoFormQuestion = Agent.SQFT; }


                    //    }
                    //    break;
                    //case Agent.SQFT:
                    //    string SQFEET = getAgent().returnNumeric(response);
                    //    if (SQFEET != "")
                    //    {
                    //        getAgent().EnterData("frmSqFt", SQFEET);
                    //        getAgent().AutoFormQuestion = Agent.TCPA;

                    //    }
                    //    break;
                    //case Agent.PPC:
                    //    string PPC = getAgent().GETPPC(response);
                    //    if (PPC != "")
                    //    {
                    //        getAgent().selectData("frmPersonalPropertyCoverage", PPC);
                    //        getAgent().AutoFormQuestion = Agent.TCPA;

                    //    }
                    //    break;
                    case "REPEAT":
                    case Agent.TCPA:
                        DBCommand = DBCommand = "INSERT INTO `LEADS` (`AGENT`,`NAME`, `PHONE`, `LEAD_ID`, `LEAD_GUID`, `IMPORT_ID`) VALUES ('" + AgentNum + "','" + CurrentCustomer.firstName + " " + CurrentCustomer.lastName + "','" + CurrentCustomer.phone + "','" + CurrentCustomer.LeadID + "','" + CurrentCustomer.LEADGUID + "','" + CurrentCustomer.IMPORT_ID + "')";
                        Agent.UpdateDBase(DBCommand);
                        if (response.ToLower().TrimEnd('.', '?', '!').Contains("yes") || response.Contains("ok") || response.ToLower().TrimEnd('.', '?', '!').Contains("fine") || response.ToLower().TrimEnd('.', '?', '!').Contains("okay") || response.ToLower().TrimEnd('.', '?', '!').Contains("sure") || response.Contains("yep") || response.Contains("yeah") || response.Contains("sounds good") || response.Contains("absolutely") || response.Contains("alright"))
                        {
                            Callpos = Agent.INBETWEEN;
                            AutoFormQuestion = "";
                            EnterSelectElementData("frmTcpaConsumerConsented", "Responded YES, said sure, I agree, that's okay, etc.");
                            string name =  CurrentCustomer.firstName + " " + driver.FindElementById("frmLastName").GetAttribute("value");
                            string phone = CurrentCustomer.phone;

                            driver.FindElementById("btnSubmit").Click();
                            SilenceTimer = 0;
                            SilenceTimer = 0;
                            AutoFormQuestion = "ENDCALL";
                            Console.WriteLine("called endcall successfully");
                            Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\ENDCALL.mp3");
                            App.screenshots++;
                            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/";
                        }
                        else if (response.ToLower().TrimEnd('.', '?', '!').Contains("no") || response.Contains("no thank you") || response.ToLower().TrimEnd('.', '?', '!').Contains("do not consent") || response.ToLower().TrimEnd('.', '?', '!').Contains("not okay") || response.ToLower().TrimEnd('.', '?', '!').Contains("nope") || response.Contains("don't think so") || response.Contains("negative") || response.Contains("sounds bad"))
                        {
                            EnterSelectElementData("frmTcpaConsumerConsented", "Responded NO, did not respond, hung up, etc.");
                            Callpos = Agent.INBETWEEN;
                            App.getWindow().cmbDispo.SelectedIndex = 7;
                            driver.FindElementById("btnSubmit").Click();
                            AutoFormQuestion = "";
                            //getAgent().low_blow_bro = true;
                            Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                            HangUpandDispo("LOW");
                        }

                        else
                        {
                            Callpos = Agent.INBETWEEN;
                            AutoFormQuestion = "REPEAT";
                        }
                        break;
                }
            }
        }
         public string CheckLead()
        {
            int i = 0;
            IReadOnlyCollection<OpenQA.Selenium.IWebElement> field = driver.FindElementsByClassName("error");
            foreach (OpenQA.Selenium.IWebElement item in field)
            {
                Console.WriteLine("MISSED" + i + ": " + item.GetAttribute("id"));
                if (item.GetAttribute("id") != "") { return item.GetAttribute("id"); }

           }
            return "";
        }
        //-----------------------------------------------------------------------
        // BELOW INPUTS DEFAULT ANSWER IN THE EVENT WE HAVEN'T RECEIVED AN ANSWER 
        public static void INPUTDEFAULT()
        {
            if (CurrentCustomer.speech != "")
            {
                Console.WriteLine("CUSTOMER SPEECH FOR DEFAULT: " + CurrentCustomer.speech);
                switch (AutoFormQuestion)
                {
                    case Agent.CALL_START:
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;
                    case Agent.INTRO:
                    case Agent.PROVIDER:

                        EnterSelectElementData("frmInsuranceCarrier", "Progressive Auto Pro");
                        AutoFormQuestion = Agent.INS_EXP;
                        Callpos = "INBETWEEN";
                        SilenceTimer = 0;
                        //timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;

                    case Agent.INS_EXP:
                        EnterSelectElementData("frmPolicyExpires_Month", MonthFromNumeral((DateTime.Now.Month + 1).ToString()));
                        EnterSelectElementData("frmPolicyExpires_Year", (DateTime.Now.Year).ToString());
                        Callpos = "INBETWEEN";
                        AutoFormQuestion = Agent.INS_START;
                        SilenceTimer = 0;
                       // timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;

                    case Agent.INS_START:
                        EnterSelectElementData("frmPolicyStart_Month", MonthFromNumeral((DateTime.Now.Month + 1).ToString()));
                        EnterSelectElementData("frmPolicyStart_Year", (DateTime.Now.Year - 1).ToString());
                        Callpos = "INBETWEEN";
                        AutoFormQuestion = Agent.NUM_VEHICLES;
                        SilenceTimer = 0;
                        //timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;

                    //   case Agent.NUM_VEHICLES:
                    //       if (Int32.Parse(NUM_VEHICLES) == 1)
                    //       {
                    //           Callpos = "INBETWEEN";
                    //           temp.AutoFormQuestion = Agent.YMM_ONLY_ONE;
                    //           SilenceTimer = 0;
                    //           Application.Current.Dispatcher.Invoke((() => temp.AskAutoFormQuestion()));
                    //           break;
                    //       }else
                    //       {
                    //           Callpos = "INBETWEEN";
                    //           temp.AutoFormQuestion = Agent.YMM1;
                    //           SilenceTimer = 0;
                    //           Application.Current.Dispatcher.Invoke((() => temp.AskAutoFormQuestion()));
                    //           break;
                    //       }
                    case NUM_VEHICLES:

                        Callpos = "INBETWEEN";
                        AutoFormQuestion = Agent.YMM_ONLY_ONE;
                        SilenceTimer = 0;
                        //timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;
                    case YMM_ONLY_ONE:
                        SilenceTimer = 0;
                        Callpos = Agent.YMM_ONLY_ONE;
                      AutoFormQuestion = Agent.YMM_ONLY_ONE;
                        SilenceTimer = 0;
                       // timeout = 0;
                        Application.Current.Dispatcher.Invoke((() =>
                        {
                            Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\can you repeat that.mp3");
                            Application.Current.Dispatcher.Invoke(() => setupReco(Agent.YMM_ONLY_ONE));

                        }));
                        break;
                    case Agent.YMM1:
                    case YMM2:
                    case YMM3:
                    case YMM4:
                        Callpos = "INBETWEEN";
                        AutoFormQuestion = Agent.DOB;
                        SilenceTimer = 0;
                        //timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;
                    case DOB:
                    case BDAYMONTH:
                        AutoFormQuestion = Agent.MARITAL_STATUS;
                        SilenceTimer = 0;
                       // timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;
                    case SPOUSEBDAYMONTH:
                        AutoFormQuestion = Agent.OWN_OR_RENT;
                        SilenceTimer = 0;
                       // timeout = 0;
                        Application.Current.Dispatcher.Invoke((() => AskAutoFormQuestion()));
                        break;


                }

            }

        }
        //--------------------------------------------------------------------------
        public static async Task<bool> doAgentStatusRequest()
        {
            try
            {
                while (LoggedIn || testing == true)
                {

                    Thread.Sleep(200);

                    StartWebRequest();
                    string stats = reader.ReadToEnd();
                    reader.Close();

                    string[] tempstr = stats.Split(',');

                    try
                    {
                        Dialer_Status = tempstr[0];
                        Agent_Name = tempstr[5];
                        try
                        {
                            if (stats.Contains("DEAD") || stats.Contains("DISPO") || stats.Contains("HANGUP"))
                            {
                                HangUpandDispo("hangup");
                                inCall = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            //App.getWindow().speechTxtBox.Text += Environment.NewLine + ex;
                            Console.WriteLine(ex);
                            Console.WriteLine(ex.StackTrace);
                            Console.WriteLine("Problem getting stats");
                        }

                        if (Dialer_Status == "READY")
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (App.getWindow().reco.is_recording)
                                {
                                    App.getWindow().reco.TurnOffMic();
                                }
                                
                            });
                            newCall = true;
                        }
                        else if ((Dialer_Status == "INCALL" || testing == true) && !stats.Contains("DEAD"))

                        {
                            if (isTalking == false) { SilenceTimer += .2;
                                //timeout += .2;
                                Console.WriteLine("Silence is " + SilenceTimer + " seconds"); }                            
                            if (SilenceTimer >= 2) { INPUTDEFAULT(); }
                            if (SilenceTimer >= 3) { CheckForContact(SilenceTimer); }
                            calltime += 0.2;
                            App.totalTimer += 0.2;

                            if (newCall)
                            {
                                newCall = false;
                                Application.Current.Dispatcher.Invoke(() => setupReco(Agent.CALL_START));

                                setupBot();

                            }
                        }
                        //Console.WriteLine("Dialer Status: " + Dialer_Status);
                        //Console.WriteLine("Agent Name: " + Agent_Name);
                        //Console.WriteLine("dead? " + dead);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        if (started)
                        {
                            MessageBox.Show("You're not logged in anymore");
                            driver.Quit();
                        }

                    }
                    setWindowColor();



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating stats report." + Environment.NewLine + ex);
            }
            return true;
        }
        protected static int portNum = 6000;
        public static void setupReco(string context = "")
        {


            var window = App.getWindow();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Process bill in Process.GetProcessesByName("Python"))
                {
                    bill.Kill();
                }
                window.reco = new Speech_Recognizer(portNum, context);
                window.reco.PartialSpeech += window.onGooglePartialSpeech;
                window.reco.FinalSpeech += window.onGoogleFinalSpeech;
                window.reco.MicChange += window.onMicChange;
                window.reco.TurnOnMic(Speech_Recognizer.Google);
            });
            if (portNum >= 6020)
            {
                portNum = 6000;
            }
            else
            {
                portNum += 1;
            }


        }
        //================================================================================
        private static void setWindowColor()
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
        //================================================================================
        public static  bool EnterData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            int unhideCount = 0;

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
                    if (unhideCount == 1)
                    {
                        Console.WriteLine("couldn't unhide, ending...");
                        retry = false;
                        return false;
                    }
                    unhideCount += 1;
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
        //================================================================================
        public static bool EnterSelectElementData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            int unhideCount = 0;

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
                    if (unhideCount == 1)
                    {
                        Console.WriteLine("couldn't unhide, ending...");
                        retry = false;
                        return false;
                    }
                    unhideCount += 1;
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
        //================================================================================
        public bool CheckBox(string elementId)
        {
            bool retry = true;
            int staleRefCount = 0;
            int unhideCount = 0;

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
                    if (unhideCount == 1)
                    {
                        Console.WriteLine("couldn't unhide, ending...");
                        retry = false;
                        return false;
                    }
                    unhideCount += 1;
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
        //================================================================================
        public async void async_selectData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            int unhideCount = 0;

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
                    if (unhideCount == 1)
                    {
                        Console.WriteLine("couldn't unhide, ending...");
                        retry = false;
                    }
                    unhideCount += 1;
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
                catch (Exception)
                {
                    //Console.WriteLine("Generic Exception");
                    //Console.WriteLine("Inner exception: " + ex.InnerException);
                    //Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
        }
        //================================================================================
        static void StartWebRequest()
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
            if (s.Contains("allied") || (s.Contains("ally") && !s.Contains("actually")))
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
        //================================================================================
        public static int getAge()
        {
            int Month; int Day; int Year;
            int.TryParse(driver.FindElementById("frmDOB_Month").GetAttribute("value"), out Month);
            int.TryParse(driver.FindElementById("frmDOB_Day").GetAttribute("value"), out Day);
            int.TryParse(driver.FindElementById("frmDOB_Year").GetAttribute("value"), out Year);
            Console.WriteLine(Month.ToString() + Day.ToString() + Year.ToString());
            DateTime now = DateTime.Now;
            DateTime birthDate = new DateTime(Year, Month, Day);
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) { age--; }
            Console.WriteLine(age);
            return age;
        }  
        //================================================================================
        public static string checkForSecondaries()
        {
            string secondaries = "";
            string HomeInsurance = "";
            string LifeInsurance = "";           //These will define what secondaries to offer
            string HealthInsurance = "";
            try
            {
                int age = getAge();
                if (driver.FindElementById("frmResidenceType").GetAttribute("value") == "Own") { HomeInsurance = "Home"; } else if (driver.FindElementById("frmResidenceType").GetAttribute("value") == "Rent") { HomeInsurance = "Rent"; } else { HomeInsurance = ""; }
                if (age <= 80 && age >= 25) { LifeInsurance = "Life"; }
                if (age >= 64) { HealthInsurance = "Medicare"; } else { HealthInsurance = "Health"; }
                secondaries = (HomeInsurance + " " + LifeInsurance + " " + HealthInsurance);
                return (secondaries);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return ("ERROR");
            }
        }
        //-----------------------------------------------------------------------------------------
        public static string getSecondaryClip()
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
                expMonth = MonthFromNumeral((DateTime.Now.Month).ToString());
                if (DateTime.Now.Month == 12)
                {
                    expyear = (DateTime.Now.Year + 1).ToString();
                }
                else { expyear = DateTime.Now.Year.ToString(); }
            }
            else if (s.Contains("in a month") || s.Contains("next month"))
            {
                expMonth = MonthFromNumeral((DateTime.Now.Month + 1).ToString());
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
        public static bool unhideElement(string elementId)
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
        public static string CheckProviderStart(string response)
        {
            string month = CurrentCustomer.expMonth;
            if (response.Contains("two") || response.Contains("2"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Nov " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Dec " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 2).ToString()); }
            }
            else if (response.Contains("three") || response.Contains("3"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Oct " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Nov " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 3).ToString()); }
            }
            else if (response.Contains("four") || response.Contains("4") || response == ("4"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Sep " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Oct " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 4).ToString()); }
            }
            else if (response.Contains("five") || response.Contains("5"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Aug " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Sep " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 5).ToString()); }
            }
            else if (response.Contains("six") || response.Contains("6") || response.Contains("sex"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Jul " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Aug " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 6).ToString()); }
            }
            else if (response.Contains("seven") || response.Contains("7"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Jun " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Jul " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 7).ToString()); }
            }

            else if (response.Contains("eight") || response.Contains("8"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("May " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Jun " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 8).ToString()); }
            }
            else if (response.Contains("nine") || response.Contains("9"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Apr " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("May " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 9).ToString()); }
            }
            else if (response.Contains("ten") || response.Contains("10"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Mar " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Apr " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 10).ToString()); }
            }
            else if (response.Contains("eleven") || response.Contains("11"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Feb " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Mar " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 11).ToString()); }
            }
            else if (response.Contains("twelve") || response.Contains("12"))
            {
                if (response.Contains("months"))
                {
                    if (DateTime.Now.Month == 1) { return ("Jan " + (DateTime.Now.Year - 1).ToString()); }
                    else if (DateTime.Now.Month == 2) { return ("Feb " + (DateTime.Now.Year - 1).ToString()); }
                }
                else { return (month + " " + (DateTime.Now.Year - 12).ToString()); }
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
            else if (response.Contains("twenty") || response.Contains("20"))
            {
                return (month + " " + (DateTime.Now.Year - 20).ToString());
            }


            return "FALSE";
        }
        //===================================================================================================
        public int getNumVehicles(string response)
        {
            if (response.Contains("1") || response.Contains("one") || response.Contains("won") || response.Contains("want"))
            { return 1; }
            else if (response.Contains("2") || response.Contains("two") || response.Contains("have to") || response.Contains("to vehicles") || response.Contains("to cars") || response.Contains("too") || response.Contains("take") || response.Contains("two") || response == " to " || response == " too " || response == "who")
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
            if (response.Contains("1981") || response.Contains("82")) { year = "1981"; }
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
            else if (response.Contains("2017") || response.Contains("17")) { year = "2017"; }
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
                        EnterSelectElementData(AutoForm.VEHICLE_1_YEAR, year);
                        EnterSelectElementData(AutoForm.VEHICLE_1_MAKE, year);
                        break;
                    case 2:
                        EnterSelectElementData(AutoForm.VEHICLE_2_YEAR, year);
                        EnterSelectElementData(AutoForm.VEHICLE_2_MAKE, year);
                        break;
                    case 3:
                        EnterSelectElementData(AutoForm.VEHICLE_3_YEAR, year);
                        EnterSelectElementData(AutoForm.VEHICLE_3_MAKE, year);
                        break;
                    case 4:
                        EnterSelectElementData(AutoForm.VEHICLE_4_YEAR, year);
                        EnterSelectElementData(AutoForm.VEHICLE_4_MAKE, year);
                        break;
                }
                Thread.Sleep(300);
                Console.WriteLine(Modelcontrol);

                models = driver.FindElementById(Modelcontrol);

                IReadOnlyCollection<OpenQA.Selenium.IWebElement> theModels = models.FindElements(OpenQA.Selenium.By.TagName("option"));
                foreach (OpenQA.Selenium.IWebElement option in theModels)
                {
                    Console.WriteLine("Searching...." + option.Text);
                    try
                    {
                        searcher = option.Text.Split(' ')[0];
                        if (response.Contains(searcher.ToLower()))
                        {
                            Console.WriteLine("FOUND MODEL!" + option.Text + " PUTTING IN " + vehicleNum);
                            model = option.Text;
                            switch (vehicleNum)
                            {
                                case 1:
                                    EnterSelectElementData(AutoForm.VEHICLE_1_MODEL, model);
                                    break;
                                case 2:
                                    EnterSelectElementData(AutoForm.VEHICLE_2_MODEL, model);
                                    break;
                                case 3:
                                    EnterSelectElementData(AutoForm.VEHICLE_3_MODEL, model);
                                    break;
                                case 4:
                                    EnterSelectElementData(AutoForm.VEHICLE_4_MODEL, model);
                                    break;
                            }

                            return (model);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error searching for models on page...error: " + ex + "---> retrying");
                        Thread.Sleep(250);
                        GETYMM(response, vehicleNum);

                    }
                }
            }
            return (year + " " + make + " " + model);
        }
        public string checkIfSecondaries(string phrase)
        {

            if (phrase.Contains("yes") || phrase.Contains("sure")) { return "YES"; }
            else { return "NONE"; }
        }
        public bool ParseDOB(string response, int spouse)
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
            switch (DayYear.Length)
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
            if (day != "" && year != "")
            {
                if (spouse == 0)
                {

                    EnterSelectElementData("frmDOB_Day", day);
                    EnterSelectElementData("frmDOB_Month", MonthFromNumeral(month));
                    EnterSelectElementData("frmDOB_Year", year);
                    return true;
                }
                else
                {
                    EnterSelectElementData("frmSpouseDOB_Day", day);
                    EnterSelectElementData("frmSpouseDOB_Month", MonthFromNumeral(month));
                    EnterSelectElementData("frmSpouseDOB_Year", year);
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
            for (int i = temp.Length - 1; i > 0; i--)
            {
                if (temp[i] == "at") { temp[i] = "@"; break; }
            }
            for (int j = 0; j < temp.Length; j++) { email += temp[j]; }
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
        public static string MonthFromNumeral(string monthNum)
        {
            switch (monthNum)
            {
                case "1": return "Jan";
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
        public static bool UpdateDBase(string command)
        {
            using (MySqlConnection myConnection = new MySqlConnection())
            {
                MySqlCommand Add = new MySqlCommand(command, myConnection);
                try
                {

                    myConnection.ConnectionString =
                            "Server=sql9.freemysqlhosting.net;" +
                            "Database=sql9136099;" +
                            "Uid=sql9136099;" +
                            "Pwd=HvsN6cVwbx;";
                    myConnection.Open();
                    Add.ExecuteNonQuery();
                    myConnection.Close();
                    Console.WriteLine("SUCCESSFULLY OPENED MSQL WITH STRING " + command);


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    myConnection.Close();
                    return false;
                }
                return true;
            }

        }
        //public static async Task<string> TrainReco(string speech)
        //{

        //    foreach (string check in trainer)
        //    {
        //        if (speech.Contains(check.Split('|')[0]))
        //        {
        //            Console.WriteLine("replacing " + speech + " with " + check.Split('|')[1]);
        //            return (check.Split('|')[1]);

        //        }
        //    }
        //    return speech;
        //}
        public async Task<bool> checkforData(string response)
        {
            CurrentCustomer.speech = CleanUpResponse(CurrentCustomer.speech);
            string Data;
            bool mrMeseeks = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentCustomer.speech = response;
                Console.WriteLine("Checking against AutoFormQuestion (" + AutoFormQuestion + ") with data {" + response + "}");
            });

            switch (AutoFormQuestion)
            {
                case Agent.CALL_START:
                    string check = "this is " + CurrentCustomer.firstName.ToLower();
                    if (response.Contains("yes") || response.Contains("speaking") || response.Contains("hello") || response == "hi"  || response.Contains("yeah") || response.Contains("this is him")  || response.Contains("this is her") || response.Contains("yup") || response.Contains("sure is") || response.Contains("you've got him") || response.Contains("can i help you"))
                    {
                        Console.WriteLine("THIS IS THE PERSON YOU WANT");
                        Thread.Sleep(300);
                        currentlyRebuttaling = false;
                        AutoFormQuestion = Agent.INTRO;
                        AskAutoFormQuestion();
                    }

                    else if (response.Contains("no it isn't") || response == "no" || response.Contains("no it is not") || (response.Contains("not") && response.Contains(CurrentCustomer.firstName.ToLower())) || response.Contains("this is not") || response.Contains("not " + CurrentCustomer.firstName) || response.Contains("he's not here") || response.Contains("hes not here"))
                    {
                        Console.WriteLine("THIS IS NOT THE PERSON YOU WANT");
                        Thread.Sleep(300);
                        string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Is This the Spouse.mp3";
                        bool x = await Conversation.RollTheClipAndWait(clip);
                        AutoFormQuestion = "SPOUSE?";
                    }
                    break;

                case "SPOUSE?":
                    if (response.Contains("yes") || response.Contains("speaking") || response.Contains("yup") || response.Contains("yeah") || response.Contains("yep") || response.Contains("it is"))
                    {
                        CurrentCustomer.Objected = false;
                        AutoFormQuestion = Agent.INTRO;
                        AskAutoFormQuestion();
                    }
                    else if (response.Contains("no"))
                    {
                        bool x = await Conversation.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                        HangUpandDispo("Not Available");
                    }
                    break;
                case Agent.INTRO: // fall through

                case Agent.PROVIDER:
                    Console.WriteLine(Agent.PROVIDER);
                    CurrentCustomer.IProvider = CheckIProvider(response);
                    Console.WriteLine("Checking for data for Insurance Provider with " + response);
                    if (CurrentCustomer.IProvider != "FALSE")

                    {
                        if (EnterSelectElementData("frmInsuranceCarrier", CurrentCustomer.IProvider))
                        {
                            Console.WriteLine("Val is: " + driver.FindElementById("frmInsuranceCarrier").GetAttribute("value"));
                            if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                            Console.WriteLine("put stuff in, current AutoFormQuestion is: " + AutoFormQuestion);
                            App.RESULTS.Add(Agent.PROVIDER, true);
                        }
                    }
                    else
                    {
                        mrMeseeks = false;
                        App.RESULTS.Add(Agent.PROVIDER, false);
                    }

                    if (driver.FindElementById("frmInsuranceCarrier").GetAttribute("value") != "")
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
                        if (EnterSelectElementData(AutoForm.INSURANCE_EXPIRATION_MONTH, theDates[0]) && EnterSelectElementData(AutoForm.INSURANCE_EXPIRATION_YEAR, theDates[1]))
                        {
                            CurrentCustomer.expMonth = theDates[0];
                            CurrentCustomer.expYear = theDates[1];
                            Callpos = Agent.INBETWEEN;
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("error entering data EXP");
                        break;
                    }

                  
                case Agent.NUM_VEHICLES:
                    int data = getNumVehicles(response);
                    if (data > 0)
                    {
                        Callpos = Agent.INBETWEEN;
                        CurrentCustomer.numVehicles = data;
                    }
                    break;
                case Agent.YMM_ONLY_ONE:

                case Agent.YMM1:
                    BackgroundWorker bg = new BackgroundWorker();
                    bg.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        Data = GETYMM(response, 1);
                    });
                    bg.RunWorkerCompleted += DoneZo;
                    bg.RunWorkerAsync();
                    break;
                case Agent.YMM2:
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                       Data = GETYMM(response, 2);
                    });
                    bw.RunWorkerAsync();
                    break;
                case Agent.YMM3:
                    BackgroundWorker bz = new BackgroundWorker();
                    bz.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        Data = GETYMM(response, 3);
                    });
                    bz.RunWorkerAsync();
                    break;
                case Agent.YMM4:
                    BackgroundWorker bx = new BackgroundWorker();
                    bx.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                    {
                        Data = GETYMM(response, 4);
                    });
                    bx.RunWorkerAsync();
                    break;
                case Agent.DOB:
                    break;
                case Agent.MARITAL_STATUS:
                    var maritalStatus = checkMaritalStatus(response);
                    if (maritalStatus.Length > 0)
                    {
                        EnterSelectElementData(AutoForm.MARITAL_STATUS, maritalStatus);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                    CurrentCustomer.maritalStatus = maritalStatus;
                    break;
                case Agent.SPOUSE_NAME:
                    if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.SPOUSE_DOB:
                    if (Callpos != Agent.FIXING) {Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.OWN_OR_RENT:
                    var ownership = checkOwnership(response);
                    if (ownership.Length > 0)
                    {
                        EnterSelectElementData(AutoForm.RESIDENCE_TYPE, ownership);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.RES_TYPE:
                    var resType = checkResType(response);
                    if (resType.Length > 0)
                    {
                        EnterSelectElementData("frmDwellingType", resType);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                    break;

                case Agent.CREDIT:
                    var credit = checkCredit(response);
                    if (credit.Length > 0)
                    {
                        EnterSelectElementData("frmCreditRating", credit);
                    }
                    else
                    {
                        mrMeseeks = false;
                    }
                    if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                    break;


                case Agent.PHONE_TYPE:
                    var phoneType = checkPhoneType(response);
                    if (phoneType.Length > 0)
                    {
                        EnterSelectElementData("frmPhoneType1", phoneType);

                    }

                    else
                    {
                        mrMeseeks = false;
                    }
                    if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; }
                    break;
                case Agent.LAST_NAME:
                    break;
                case Agent.SECONDARIES:
                    if (checkIfSecondaries(response) != "NONE") { if (Callpos != Agent.FIXING) { Callpos = Agent.INBETWEEN; } }
                    break;
                case Agent.YEARBUILT:
                    if (returnNumeric(response) != "") { }
                    break;
                case Agent.SQFT:
                    if (returnNumeric(response) != "") { }
                    break;
                case Agent.PPC:
                    if (returnNumeric(response) != "") { }
                    break;

            }

            return mrMeseeks;

        }
        public static void DoneZo(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("BACKGROUND WORKER DONE!");
            if (driver.FindElementById("vehicle-make").GetAttribute("value") == "- Select Make -")
            {
                Console.WriteLine("NO MAKE DETECTED");
                string clip = @"C:\Soundboard\Cheryl\VEHICLE INFO\Who Makes That Vehicle.mp3";
                Conversation.RollTheClip(clip);
            }
        }
        public bool ParseAddress(string response)
        {
            int i = 0;
            string[] addy = response.Split(' ');
            string AddressLine1 = "";
            for (i = 0; i < addy.Length - 1; i++)
            {
                AddressLine1 += addy[i];
                if (addy[i].ToLower() == "street" || addy[i].ToLower() == "st" || addy[i].ToLower() == "road" || addy[i].ToLower() == "avenue" || addy[i].ToLower() == "boulevard" || addy[i].ToLower() == "terrace" || addy[i].ToLower() == "circle" || addy[i].ToLower() == "lane" || addy[i].ToLower() == "court") { break; }
            }
            string zip = addy[addy.Length - 1];
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
                else
                {
                    Console.WriteLine("address is bad");
                    hasAsked = true;
                    Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\can you repeat that.mp3");
                    return false;

                }
            }

            catch (OpenQA.Selenium.NoSuchElementException)
            {
                Console.WriteLine("address is bad");
                hasAsked = true;
                Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\can you repeat that.mp3");
                return false;

            }
        }
        public static String returnNumeric(string response)
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
        public string badSpeechParser(string oldSpeech)
        {
            return "";
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static async Task<bool> checkForObjection(string response)
        {
            string resp = response.ToLower();
            string clip;

            Console.WriteLine("Checking against objections with------->" + response);

            if (currentlyRebuttaling == false)
            {

                if((resp.Contains(" no ") || resp == "no") && AutoFormQuestion == Agent.ADDRESS)
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Address Rebuttal.mp3";
                    Conversation.RollTheClip(clip);
                    CurrentCustomer.Objected = true;
                    currentlyRebuttaling = true;
                    return true;
                }
                else if((resp.Contains(" no ") || resp == "no") && AutoFormQuestion == Agent.DOB || (resp.Contains(" no ") || resp == "no") && AutoFormQuestion == Agent.SPOUSE_NAME)
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\This info.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }
 
                else if (resp.Contains("my spouse handles") || resp.Contains("my husband handles") || resp.Contains("my wife handles") || resp.Contains("my spouse takes care") || resp.Contains("my husband takes care") || resp.Contains("my wife takes care"))
                {
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\My spouse takes care of that.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("not here right now") || resp.Contains("leave a message") || resp.Contains("leave your name and phone number") || resp.Contains("after the beep") || resp.Contains("we will return your call") || resp.Contains("record your message") || resp.Contains("voicemail") || resp.Contains("mailbox") || resp.Contains("mail box") || resp.Contains("is full") || resp.Contains("press 2") || resp.Contains("satisfied with the message") || resp.Contains("the tone") || resp.Contains("for more options"))
                {
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;

                    HangUpandDispo("Not Available");

                }
                else if (resp.Contains("why do you need that") || resp.Contains("why do you need this information") || resp.Contains("you're asking a lot of AutoFormQuestions"))
                {
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Why do you need my info.mp3";
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    
                    Conversation.RollTheClip(clip);
                }
                else if (resp.Contains("what is this about") || resp.Contains("whats this about") || resp.Contains("why are you calling") || resp.Contains("what are you calling for") || resp.Contains("whats this 14") || resp.Contains("why you callin") || resp.Contains("whats this all about") || resp.Contains("purpose of your call") || resp.Contains("why are you calling") )
                {
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
                    Conversation.RollTheClip(clip);

                }
                else if (resp.Contains("real busy") || resp.Contains("i'm at work") || resp.Contains("going to work") || resp.Contains("call back") || resp.Contains("the middle of something") || resp.Contains("can't right now") || resp.Contains("can't talk") || resp.Contains("busy now") || resp.Contains("busy right now"))
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\This Will be Real quick.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;

                }
                else if (resp.Contains("not interested") || resp.Contains("no interest") || resp.Contains("no thank you") || resp.Contains("don't want it") || resp.Contains("fuck you"))
                {
                    NIReps += 1;
                    if (Callpos == SECONDARIES || AutoFormQuestion == SECONDARIES
                       || Callpos == WHICHSECONDARIES || AutoFormQuestion == WHICHSECONDARIES)
                    {
                        return true;
                    }
                    switch (NIReps)
                    {
                        case 1:
                            currentlyRebuttaling = true;
                            clip = @"C:\SoundBoard\Cheryl\REBUTTALS\nothing to be interested in.mp3";
                            Conversation.RollTheClip(clip);
                            currentlyRebuttaling = true;
                            currentlyRebuttaling = true;
                            return true;
                        case 2:
                            clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Rebuttal1.mp3";
                            Conversation.RollTheClip(clip);
                            currentlyRebuttaling = true;
                            currentlyRebuttaling = true;
                            return true;
                        case 3:
                            NIReps = 0;
                            HangUpandDispo("Not Interested");
                            break;

                    }
                }
                else if (resp.Contains("take me off your") || resp.Contains("take me off your list") || resp.Contains("take my name off your list") || resp.Contains("remove my phone number") || resp.Contains("put me on your") || resp.Contains("do not call list") || resp.Contains("no call list") || resp.Contains("don't call me again") || resp.Contains("don't ever call me again"))
                {
                    hasAsked = true;
                 
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\DNC.mp3";
                    Conversation.RollTheClip(clip);
                    App.Current.Dispatcher.Invoke(() => { App.getWindow().cmbDispo.SelectedIndex = 4; });
                    currentlyRebuttaling = true;
                    return false;
                }
                else if (resp.Contains("what did you say") || resp.Contains("what was that") || resp.Contains("could you repeat that") || resp.Contains("come again"))
                {
                    return true;
                }
                else if (resp.Contains("who are you") || resp.Contains("who is this") || resp.Contains("ask is calling") || resp.Contains("who is calling") || resp.Contains("whose calling") || resp.Contains("who's calling") || resp.Contains("ask you calling") || resp.Contains("who is calling") || resp.Contains("who's this"))
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\Soundboard\Cheryl\INTRO\CHERYLCALLING.mp3";
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    Conversation.RollTheClip(clip);

                    return true;
                }
                else if (resp.Contains("what's lcn") || resp.Contains("what is lcn") || resp.Contains("i'll see you then") || resp.Contains("whats lcn") || resp.Contains("what is lcn") || resp.Contains("else in") || resp.Contains("else n"))
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\What's LCN.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("already have") || resp.Contains("already got"))
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I already have insurance rebuttal.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("not giving you") || resp.Contains("none of your business") || resp.Contains("not giving that out") || resp.Contains("not getting that out") || resp.Contains("that's personal"))
                {
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Let me Just confirm a few things.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("whered you get") || resp.Contains("how did you get my number") || resp.Contains("get my info") || resp.Contains("where did you get") || resp.Contains("i never submitted") || resp.Contains("i never requested") ||  resp.Contains("i never asked") || resp.Contains("i didn't request"))
                {
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\where did you get my info.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }
                else if (resp.Contains("happy with") || resp.Contains("fine with my insurance") || resp.Contains("i'm good") || resp.Contains("all set") || resp.Contains("satisfied"))
                {
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\ThisIsJustAQuickProccess.mp3";
                    Conversation.RollTheClip(clip);
                    currentlyRebuttaling = true;
                    currentlyRebuttaling = true;
                    return true;
                }

                else if (resp.Contains("wrong number") || resp.Contains("doesn't live here") || resp.Contains("don't live here") || resp.Contains("wrong person") || resp.Contains("this is a business") || resp.Contains("the office of") || resp.Contains("office hours are"))
                {
                    
                    currentlyRebuttaling = true;
                    hasAsked = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
                    bool x = await Conversation.RollTheClipAndWait(clip);
                    x = await Conversation.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                    HangUpandDispo("Wrong Number");
                    return false;
                }
                else if (resp.Contains("don't have insurance") || resp.Contains("don't currently have insurance") || resp.Contains("don't currently have any insurance") || resp.Contains("don't have an auto insurance company") || resp.Contains("don't have the car") || resp.Contains("not insured") || resp.Contains("don't even have insurance") || resp.Contains("have no insurance") || resp.Contains("don't have a vehicle") || resp.Contains("don't own a vehicle") || resp.Contains("don't own a car") || resp.Contains("don't have a car") || resp.Contains("no car") || resp.Contains("no vehicle"))
                {
                   
                    currentlyRebuttaling = false;
                    hasAsked = true;
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
                    bool x = await Conversation.RollTheClipAndWait(clip);
                    x = await Conversation.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                    HangUpandDispo("No Car");
                    return false; 
                }

                else
                {
                    return false;
                }
            }
            return true;
        }
        //-----------------------------------------------------------------------------------------------------------
        public  bool Login(string AgentNumber)
        {
            AgentNum = AgentNumber;


            try
            {
                ChromeDriverService cds = ChromeDriverService.CreateDefaultService();
                cds.HideCommandPromptWindow = true;
                driver = new ChromeDriver(cds);
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
                foreach (IWebElement elem in driver.FindElementById("select-campaign").FindElements(OpenQA.Selenium.By.TagName("option")))
                {
                    if (elem.Text.Contains("5000") || elem.Text.Contains("BOT"))
                    {
                        elem.Click();
                    }
                }
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
        public void HangupAndDisposition(string dispo)
        {
            HangUpandDispo(dispo);
        }
        public void setAskingBDay(bool asking)
        {
            AskingBDay = asking;
        }
        public void resetSilenceTimer()
        {
            SilenceTimer = 0;
        }
        public void setTesting(bool val)
        {
            testing = val;
        }
        //------------------------------------------------------------------
        public static void HangUpandDispo(string dispo)
        {
            Console.WriteLine("got called");
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (App.getWindow().reco.is_recording)
                    {
                        App.getWindow().reco.TurnOffMic();

                    }
                });
                NIReps = 0;
                numrepeats = 0;
                AskingBDay = false;
                isListening = false;
                isTalking = false;
                SilenceTimer = 0;
                calltime = 0;
                inCall = false;
                calltime = 0;
                try
                {
                    Conversation.StopTheClip();
                }
                catch
                {

                }
                string hangupDisp = "http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_hangup&value=1";
                Console.WriteLine("API CALL TO YTEL: " + hangupDisp);
                WebRequest h = WebRequest.Create(hangupDisp);
                WebResponse r = h.GetResponse();
                Console.WriteLine("YTEL SAYS: " + r.GetResponseStream().ToString());
                AutoFormQuestion = CALL_START;
                Thread.Sleep(500);
                r.Close();
                switch (dispo)
                {
                    case "hangup":

                        if (!inCall)
                        {
                            if (endcall)
                            {
                                h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NI");
                                r = h.GetResponse();
                                break;
                            }
                            else
                            {
                                h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NotAvl");
                                r = h.GetResponse();
                                break;
                            }
                        }
                        else
                        {
                            h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NI");
                            r = h.GetResponse();
                        }
                        break;
                    case "Not Available":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NotAvl");
                        r = h.GetResponse();
                        break;
                    case "Not Interested":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NI");
                        r = h.GetResponse();
                        break;
                    case "No Insurance":
                    case "NO Ins Transfer Unsuccessful":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "NITU");
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
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "1Auto");
                        r = h.GetResponse();
                        break;
                    case "LOW":
                        h = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + AgentNum + "&function=external_status&value=" + "LOW");
                        r = h.GetResponse();
                        break;
                }
                r.Close();
            }
            catch (Exception ex)
            {
                App.getWindow().speechTxtBox.AppendText(ex.StackTrace);
                Thread.Sleep(250);
                HangUpandDispo(dispo);
            }
        }      
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void getDob()
        {
            try
            {
                string[] dob = new string[3]
                {
                new SelectElement(driver.FindElementById("frmDOB_Month")).SelectedOption.GetAttribute("value"),
                new SelectElement(driver.FindElementById("frmDOB_Day")).SelectedOption.GetAttribute("value"),
                new SelectElement(driver.FindElementById("frmDOB_Year")).SelectedOption.GetAttribute("value"),
                };

                dobInfo = dob;

                while (testing == true)
                {
                    if (isTalking == false) { SilenceTimer += .2; Console.WriteLine("Silence is " + SilenceTimer + " seconds"); }
                    //if (timeout >= 2 && !currentlyRebuttaling && !isTalking)
                    //{
                    //    Application.Current.Dispatcher.Invoke(() =>
                    //    {
                    //        Console.WriteLine("FINAL SPEECH FORCED: " + App.getWindow().reco.partial_speech);
                    //        timeout = 0;
                    //        if (App.getWindow().reco.partial_speech != "")
                    //        {
                    //            App.getWindow().reco.Final_Speech = App.getWindow().reco.partial_speech;
                    //        }
                    //    });
                    //}
                    if (SilenceTimer >= 3) { INPUTDEFAULT(); }
                    if (SilenceTimer >= 4) { CheckForContact(SilenceTimer); }
                    Thread.Sleep(200);
                }
            }
            catch
            {
                Thread.Sleep(250);
                getDob();
            }
        }
        public void killDriver()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // okey
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //BELOW FUNCTION SETS UP THE BOT WHEN A CALL POPS UP
        //---------------------------------------------------------------------------------------------------
        public static async void setupBot()
        {
            
            int x = 0;
            AutoFormQuestion = CALL_START;
            string firstName = "";
           
            try
            {
                while (driver.WindowHandles.Count < 2)
                {
                    Console.WriteLine("shoop");
                }
                inCall = true;
                currentlyRebuttaling = false;
                CurrentCustomer.Objected = false;

                Console.WriteLine("count of driver.windowhandles: " + driver.WindowHandles.Count);
                driver.SwitchTo().Window(driver.WindowHandles.Last());
                Console.WriteLine("driver title: " + driver.Title);
                firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
                CurrentCustomer.lastName = driver.FindElementByName("frmLastName").GetAttribute("value");
                CurrentCustomer.LeadID = driver.Url.Split('&')[1].Replace("lead_id=", "");
                CurrentCustomer.LEADGUID = driver.Url.Split('&')[2].Replace("lead_guid=", "");
                CurrentCustomer.IMPORT_ID = driver.Url.Split('&')[3].Replace("import_id=", "");
                Console.WriteLine("LEAD ID: " + driver.Url.Split('&')[1]);
                CurrentCustomer.firstName = firstName;
            }
            catch (Exception ex)
            {
                x += 1;
                if (x < 3)
                {
                    try
                    {
                        if (driver.PageSource.Contains("resource") || driver.PageSource.Contains("respectfully"))
                        {
                            HangUpandDispo("Not Available");
                            return;
                        }
                        else if (driver.PageSource.Contains("Service not available"))
                        {
                            HangUpandDispo("Not Available");
                            PauseUnPause("PAUSE");
                            return;
                        }
                        else
                        {
                            Thread.Sleep(50);
                            firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
                            CurrentCustomer.phone = driver.FindElementById("frmPhone1").GetAttribute("value");
                            CurrentCustomer.firstName = firstName;
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        HangUpandDispo("Not Available");
                        return;

                    }

                }
                else { HangUpandDispo("Not Available"); }
                Console.WriteLine(ex.StackTrace);
            }
            try
            {
                if (maleNames.Contains(firstName)) { EnterSelectElementData("frmGender", "Male"); } else { EnterData("frmGender", "Female"); }
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
                        CurrentCustomer.isNameEnabled = false;
                    }));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().setNameBtns(true);
                        CurrentCustomer.isNameEnabled = true;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
            }
            AskAutoFormQuestion();
            await Task.Run((Action)getDob);
            started = true;
        }
        //------------------------------------------------------TESTING SETUP FUNCTION
        public void setupTesting()
        {
            string firstName = "";

            firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
            CurrentCustomer.firstName = firstName;
            CurrentCustomer.phone = "123-456-7890";
            AgentNum = "1198";
            CurrentCustomer.LEADGUID = "!11111111";
            CurrentCustomer.LeadID = "1231342134";
            CurrentCustomer.IMPORT_ID = "1231341341";


            try
            {
                if (maleNames.Contains(firstName)) { EnterSelectElementData("frmGender", "Male"); } else { EnterData("frmGender", "Female"); }
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
            endcall = false;
            Callpos = CALL_START;
            AutoFormQuestion = CALL_START;
            calltime = 0;
            SilenceTimer = 0;
            AutoFormQuestion = CALL_START;
            CurrentCustomer.firstName = firstName;
            CurrentCustomer.isNameEnabled = true;
            numrepeats = 0;
            Application.Current.Dispatcher.Invoke(() => setupReco(CALL_START));
            Thread.Sleep(500);
            AskAutoFormQuestion();
            Task t = Task.Run((Action)getDob);
        }
        //---------------------------------------------------------------
        public void setPause(string PAction)
        {
            PauseUnPause(PAction);
        }
        //------------------------------------------------------------------
        public static void PauseUnPause(string pauseAction)
        {
            WebRequest Pause;
            WebResponse resp;

            switch (pauseAction)
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
        public static bool AskAutoFormQuestion()
        {
            Console.WriteLine("ASKING AutoFormQuestion");
            Console.WriteLine("ASKING AutoFormQuestion " + AutoFormQuestion);
            try
            {

                CurrentCustomer.speech = "";
                isTalking = true;
                SilenceTimer = 0;
                switch (AutoFormQuestion)
                {
                    case "SPOUSE?":
                        string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Is This the Spouse.mp3";
                        Conversation.RollTheClip(clip);
                        break;
                    case CALL_START:
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (App.findNameClips(CurrentCustomer.firstName)[0] != "no clip")
                            {
                                if (numrepeats > 0) { Conversation.RollTheClip(App.findNameClips(CurrentCustomer.firstName)[1]); }
                                else { Conversation.RollTheClip(App.findNameClips(CurrentCustomer.firstName)[0]); }
                            }
                            else
                            {

                                Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\HELLO.mp3");
                            }
                        });
                        break;
                    case INTRO:
                        if (!CurrentCustomer.Objected)
                        {   
                            Conversation.RollTheClip(@"C:\Soundboard\Cheryl\INTRO\Intro2.mp3"); }
                        else { Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\INTROREPEAT.mp3"); }

                        break;
                    case PROVIDER:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3");
                        break;
                    case INS_EXP:
                        if (numrepeats == 0) { Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3"); }
                        else if (numrepeats == 1) { Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Policy EXP 1.mp3"); }
                        else { Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Policy EXP 2.mp3"); }
                        break;
                    case INS_START:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3");
                        break;
                    case NUM_VEHICLES:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\How many vehicles do you have.mp3");
                        App.getWindow().reco.TurnOnMic(Speech_Recognizer.Google, Agent.NUM_VEHICLES);
                        break;
                    case YMM_ONLY_ONE:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\YMMYV.mp3");
                        break;
                    case YMM1:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\First Vehicle.mp3");
                        break;
                    case YMM2:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\2nd Vehicle.mp3");
                        break;
                    case YMM3:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\Third Vehicle.mp3");
                        break;
                    case YMM4:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\VEHICLE INFO\Fourth Vehicle.mp3");
                        break;
                    case DOB:
                        App.playDobClips();
                        break;
                    case BDAYMONTH:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REBUTTALS\Can You Just Verify the month.mp3");
                       
                        break;
                    case MARITAL_STATUS:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\Marital Status.mp3");
                        break;
                    case SPOUSE_NAME:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses First name.mp3");
                        break;
                    case SPOUSE_DOB:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses Date of Birth.mp3");

                        break;
                    case SPOUSEBDAYMONTH:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REBUTTALS\Can You Just Verify the month.mp3");

                        break;
                    case OWN_OR_RENT:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Do You Own Or Rent the Home.mp3");
                       
                        break;
                    case RES_TYPE:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\HOMETYPE.mp3");
                        Application.Current.Dispatcher.Invoke(() => setupReco(Agent.OWN_OR_RENT));
                        break;
                    case CREDIT:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Credit.mp3");
                        break;
                    case ADDRESS:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3");
                        break;
                    case EMAIL:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\EMAIL.mp3");
                        break;
                    case PHONE_TYPE:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\PHONETYPE.mp3");
                        break;
                    case LAST_NAME:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\PERSONAL INFO\Last Name.mp3");

                        break;
                    case SECONDARIES:
                        try
                        {
                            if (!Conversation.RollTheClip(getSecondaryClip())) { AutoFormQuestion = Agent.TCPA; goto TCPA; };
                            break;
                        }
                        catch { goto TCPA; }
                    case TCPA:
                        TCPA:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\TCPA.mp3");
                        Application.Current.Dispatcher.Invoke(() => setupReco(Agent.TCPA));
                        break;
                    case Agent.WHICHSECONDARIES:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Which secondaries.mp3");
                        break;
                    case YEARBUILT:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\YearBuilt.mp3");
                        break;
                    case SQFT:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Square footage.mp3");
                        break;
                    case PPC:
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\PPCoverage.mp3");
                        break;
                    case "REPEAT":
                        Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\REACTIONS\Can you repeat that.mp3");
                        break;
                }
                Callpos = INBETWEEN;
                hasAsked = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
