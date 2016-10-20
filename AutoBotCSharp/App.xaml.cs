using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave; 
using Microsoft.ProjectOxford.SpeechRecognition;
using Microsoft.ProjectOxford;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;
using MySql.Data.MySqlClient;
using ApiAiSDK;
using ApiAiSDK.Model;
using System.Net.Sockets;

namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public  static ApiAi apiAi;
       
        public static AIConfiguration config = new AIConfiguration("6c83e9e057134cd792c045595aca9528 ", SupportedLanguage.English);

        public static int version = 66;
        public string session = "";
        public string url = "";
        public static int screenshots = 0;
        public static Dictionary<string, bool> RESULTS = new Dictionary<string, bool>();

        public static bool failedEntry = false;
        public Preferences prefs;
        private static Random randy = new Random();
        private static WaveOut waveOut = new WaveOut();
        public  static bool waveOutIsStopped = true;
        //public static MicrophoneRecognitionClient shortPhraseClient;
        public static MicrophoneRecognitionClient longDictationClient;

        public static double totalTimer = 0.0;

        public static MainWindow getWindow()
        {
            var mainwindow = Current.MainWindow as MainWindow;
            return mainwindow;
        }
        
        public static Agent getAgent()
        {
            Agent temp = new Agent(); 
            try
            {
                Current.Dispatcher.Invoke(() =>
                {
                    temp = getWindow().user;
                });
            }
            catch (Exception)
            {
                Console.WriteLine("gracefully not handling this error very well");
            }
            return temp;
        }
        public static Agent_Google getGoogleAgent()
        {
            Agent_Google agent = null;
            try
            {
                Current.Dispatcher.Invoke(() =>
                {
                    agent = getWindow().googleUser;
                });
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return agent;
        }
        void App_Startup(object sender, StartupEventArgs e)
        {
            string agent = "";
            // Application is running
            // Process command line args
            bool startMinimized = false;

            if (e.Args.Length > 1)
            {
                int ver = int.Parse(System.Text.RegularExpressions.Regex.Match(e.Args[0], @"\d+").Value);
                version = ver;
                agent = System.Text.RegularExpressions.Regex.Match(e.Args[1], @"\d+").Value;
            }  
         
            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            if (startMinimized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
          
            mainWindow.Show();
            MainWindow.Title = "AutoBotC# ver. " + version.ToString();
            mainWindow.speechTxtBox.Text += "APP_STARTUP CALLED";
            mainWindow.txtAgentNum.Text = agent;
            if (agent != "") { App.getAgent().Login(agent); }

        }

        public static void reInitMicClient()
        {
            string path =  System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\keys.txt";
            string[] keys = System.IO.File.ReadAllLines(path);
            string apiKey1 = keys[0];
            string apiKey2 = keys[1];
            longDictationClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", apiKey1, apiKey2);
            longDictationClient.OnPartialResponseReceived += onPartialResponseReceivedHandler;
            longDictationClient.OnResponseReceived += onResponseReceivedHandler;
            longDictationClient.OnMicrophoneStatus += onMicrophoneStatusHandler;      
            longDictationClient.OnConversationError += onConversationErrorHandler;
            
            Console.WriteLine("Change the reco, don't let the reco change you."); 
            longDictationClient.StartMicAndRecognition();
        }

        public static void startReco()
        {
            longDictationClient.StartMicAndRecognition();
        }
        public static void onConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            Console.WriteLine("ERROR WITH SPEECH" + e.SpeechErrorText);
       
        }
        public static void onMicrophoneStatusHandler(object sender, MicrophoneEventArgs e)
        {
        
            Agent temp = getAgent();
            Console.WriteLine("MIC IS RECORDING: " + e.Recording);
            Current.Dispatcher.Invoke((()=> App.getWindow().lblreco.Content = "RECORDING STATUS: " + e));
            temp.isListening = e.Recording;

        }


        public static void onPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {

            string response = e.PartialResult;
            string raw = response;
            App.getAgent().SilenceTimer = 0;
            Current.Dispatcher.Invoke((async () =>
            {
                App.getAgent().SilenceTimer = 0;
                Console.WriteLine(App.getAgent().SilenceTimer);
                bool x;
                getWindow().setSpeechBoxText("Partial: " + response);              
                if (!(getAgent().custObjected = await Agent.checkForObjection(response)))
                {
                    Agent.checkforData(response); 
                    
                    getAgent().hasAsked = false;
                }
               // getAgent().custObjected = x;               
            }));          
        }

       public static void REMIX()
        {
            Console.WriteLine("\n EETSA ME, MARIO! \n");
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\keys.txt";

            string[] keys = System.IO.File.ReadAllLines(path);
            string apiKey1 = keys[0];
            string apiKey2 = keys[1];
            App.longDictationClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", apiKey1, apiKey2);
            App.longDictationClient.OnPartialResponseReceived += App.onPartialResponseReceivedHandler;
            App.longDictationClient.OnResponseReceived += App.onResponseReceivedHandler;
            longDictationClient.OnMicrophoneStatus += onMicrophoneStatusHandler;
            if (App.getAgent().inCall)
            {
                App.longDictationClient.StartMicAndRecognition();
            }
        }


        public static void onResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {

            //Console.WriteLine(e.PhraseResponse.RecognitionStatus);
            if (e.PhraseResponse.RecognitionStatus == ((RecognitionStatus)611) || e.PhraseResponse.RecognitionStatus.ToString() == "611")
            {
                Console.WriteLine("REACHED 2 MIN.");
                Current.Dispatcher.Invoke((() =>
                {
                    REMIX();
                   
                }));
            }
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            {
                if (App.getAgent().inCall)
                {
                    Console.WriteLine("DICTATION END SILENCE");
                    longDictationClient.EndMicAndRecognition();
                    longDictationClient.StartMicAndRecognition();
                }
            }
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.InitialSilenceTimeout)
            {
                if (App.getAgent().inCall)
                {
                    Console.WriteLine("INITIAL SILENCE");
                    longDictationClient.EndMicAndRecognition();
                    longDictationClient.StartMicAndRecognition();
                }

            }

            foreach (RecognizedPhrase result in e.PhraseResponse.Results)
            {
                if (result.DisplayText != "")
                {
                    Console.WriteLine(waveOutIsStopped);
                    Current.Dispatcher.Invoke((() =>
                    {
                        getWindow().appendSpeechBoxText("Full: " + result.DisplayText);
                        if (result.DisplayText.ToLower().Contains("incoming")) { System.Threading.Thread.Sleep(500); };
                    }));


                    Current.Dispatcher.Invoke(async () =>
                    {

                        if (getAgent().custObjected == false)
                        {
                            if (waveOutIsStopped)
                            {
                                {                              
                                         doBackgroundQuestionSwitchingStuff(e.PhraseResponse.Results[0].DisplayText);
                                        if (!getAgent().hasAsked)
                                        {                                          
                                            getAgent().hasAsked = true;
                                             bool ba = await PlayHumanism();
                                        }
                                  
                                   

                                }
                            }
                        };
                    });

                }
            }      
        }

        public static async Task<bool>  PlayHumanism()
        {
            bool b;
            string clip;
            switch (getAgent().Question)
            {
     
                case Agent.INS_EXP:
                   // if(!App.getAgent().currentlyRebuttaling)
                  //  App.getAgent().currentlyRebuttaling = true
                    clip = @"C:\SoundBoard\Cheryl\REACTIONS\OTAGC.mp3";
                    b = await RollTheClipAndWait(clip);
                    break;
                case Agent.RES_TYPE:
                    clip = @"C:\SoundBoard\Cheryl\REBUTTALS\we're almost done.mp3";
                    b = await RollTheClipAndWait(clip);
                    break;

            }
                
            getAgent().AskQuestion();
            return true;
        }



        public static async void doBackgroundQuestionSwitchingStuff(string response)
        {
            Console.WriteLine("DOING BACKGROUND SWITCHY THINGS WITH "+ response);
            string raw = response;
            string DBCommand;
            bool DBSuccess;
            response = response.TrimEnd('.', '?', '!');
            response = response.Replace("'","");
            response = response.Replace(",", "");
            response = response.ToLower();
            Agent ag = getAgent();
            // call position advancement
            if (waveOutIsStopped)
            {
                Console.WriteLine("SPEECH FINALIZED: " + response + Environment.NewLine + "ON QUESTION " + ag.Question);
                switch (ag.Question)
                {
                    case Agent.STARTYMCSTARTFACE:


                        string check = "this is " + getAgent().cust.firstName.ToLower();
                        if (getAgent().cust.isNameEnabled)
                        {

                            if (response.Contains("yes") || response.Contains("speaking") || response.Contains(check) || response.Contains("yeah") || (response.Contains("hi") && !(response.Contains("this"))) || response.Contains("yup") || response.Contains("sure is") || response.Contains("you've got him"))
                            {
                                Console.WriteLine("THIS IS THE PERSON YOU WANT");
                                Thread.Sleep(300);
                                App.getAgent().custObjected = false;
                                getAgent().Question = Agent.INTRO;
                            }
                            else if (response.Contains("hello"))
                            {
                                Console.WriteLine("WE DON'T KNOW IF THIS IS THE PERSON YOU WANT");

                                getAgent().hasAsked = true;
                                    App.RollTheClip(App.findNameClips(App.getWindow().btnTheirName.Content.ToString())[1]);                           
                            }
                            else if (response.Contains("no it isnt") ||response.Contains("no, it is not") || response.Contains("he's not here"))
                            {
                                Console.WriteLine("THIS IS NOT THE PERSON YOU WANT"); 
                                                  
                                getAgent().Question = "SPOUSE?";
                                ag.hasAsked = false;
                            }
                            else
                            {
                                getAgent().hasAsked = true;
                            }
                        }
                        else
                        {
                            getAgent().Question = Agent.INTRO;
                        }
                        break;
                    case "SPOUSE?":
                        if (response.Contains("yes") || response.Contains("speaking") || response.Contains("yup") || response.Contains("yeah") || response.Contains("yep") || response.Contains("it is"))
                        {
                            App.getAgent().custObjected = false;
                            getAgent().Question = Agent.INTRO;
                            App.getAgent().AskQuestion();
                        }
                        else if (response.Contains("no"))
                        {


                            App.RollTheClipAndWait(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                            getAgent().HangUpandDispo("Not Available");

                        }
                        break;
                    case Agent.INTRO:

                    case Agent.PROVIDER:
                        string Provider = App.getAgent().CheckIProvider(response);
                       
                        if (ag.driver.FindElementById("frmInsuranceCarrier").GetAttribute("value") != "")
                        {
                            ag.Question = Agent.INS_EXP;
                            DBCommand = "INSERT INTO `INS_PROVIDER` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            ag.hasAsked = false;
                            break;
           
                        } else
                        { ag.hasAsked = true; }
                        break;
                    case Agent.INS_EXP:
                        if (ag.driver.FindElementById("frmPolicyExpires_Month").GetAttribute("value") != "")
                        {

                            DBCommand = "INSERT INTO `INS_EXP` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            ag.Question = Agent.INST_START;
                            ag.hasAsked = false;

                        } else { ag.hasAsked = true; }
                        break;
                    case Agent.INST_START:
                        if (ag.driver.FindElementById("frmPolicyStart_Month").GetAttribute("value") != "")
                        {
                            ag.Question = Agent.NUM_VEHICLES;
                            ag.hasAsked = false;                       
                            DBCommand = "INSERT INTO `INS_START` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        } else { ag.hasAsked = true; }
                        break;
                    case Agent.NUM_VEHICLES:
                        ag.hasAsked = true;
                        if (ag.cust.numVehicles > 1)
                        {
                            ag.Question = Agent.YMM1;
                            ag.hasAsked = false;
                            longDictationClient.EndMicAndRecognition();
                        }
                        else if (ag.cust.numVehicles == 1)
                        {
                            ag.Question = Agent.YMM_ONLY_ONE;
                            ag.hasAsked = false;
                            longDictationClient.EndMicAndRecognition();
                        }
                        else
                        {
                            ag.cust.numVehicles = 1;
                            ag.hasAsked = true;
                        }

                        break;
                    case Agent.YMM_ONLY_ONE:
                        if (ag.driver.FindElementById("vehicle-make").Displayed)
                        {
                            ag.Question = Agent.DOB;
                            ag.hasAsked = false;       
                            DBCommand = "INSERT INTO `YMM_1` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        }
                        else
                        {
                            App.getAgent().hasAsked = true;
                        }   
                        break;
                    case Agent.YMM1:
                        if (ag.driver.FindElementById("vehicle-make").Displayed)
                        {
                            ag.Question = Agent.YMM2;
                            ag.hasAsked = false;                           
                            DBCommand = "INSERT INTO `YMM_1` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        }
                        else
                        {
                            App.getAgent().hasAsked = true;
                        }

                        break;
                    case Agent.YMM2:
                        if (ag.cust.numVehicles > 2)
                        {
                            if (ag.driver.FindElementById("vehicle2-make").Displayed)  {
                                ag.Question = Agent.YMM3;
                                ag.hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_2` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);

                            } else { ag.hasAsked = true; }
                          
                        }
                        else
                        {
                            if (ag.driver.FindElementById("vehicle2-make").Displayed) {
                                ag.Question = Agent.DOB;
                                ag.hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_2` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            } else { ag.hasAsked = true; }
                            break;
                        }

                        break;
                    case Agent.YMM3:
                        if (ag.cust.numVehicles > 3)
                        {
                            if (ag.driver.FindElementById("vehicle3-make").Displayed)
                            {
                                ag.Question = Agent.YMM4;
                                ag.hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_3` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            } else { ag.hasAsked = true; }
                        }
                        else
                        {
                            if (ag.driver.FindElementById("vehicle3-make").Displayed)
                            {
                                ag.Question = Agent.DOB;
                                ag.hasAsked = false;
                                DBCommand = "INSERT INTO `YMM_3` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                                Agent.UpdateDBase(DBCommand);
                            } else { ag.hasAsked = true; }                          
                        }
                        break;
                    case Agent.YMM4:
                        if (ag.driver.FindElementById("vehicle4-make").Displayed)
                        {
                            ag.Question = Agent.DOB;
                            ag.hasAsked = false;

                            
                            DBCommand = "INSERT INTO `YMM_4` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                        } else { ag.hasAsked = true; }
                        break;

                    case Agent.DOB:

                        string[] dobby = App.getAgent().dobInfo;
                        if (dobby[0] != "" && dobby[1] != "")
                        {
                            
                            if (response.ToLower().Contains("yes") || response.ToLower().Contains("yeah") || response.ToLower().Contains("right") || response.ToLower().Contains("correct") || response.ToLower().Contains("yup") || response.ToLower().Contains("yah"))
                            {
     
                               ag.Question = Agent.MARITAL_STATUS;
                              
                            }
                            else if(response.ToLower().Contains("no") || response.ToLower().Contains("wrong") || response.ToLower().Contains("incorrect") )
                            {
                                RollTheClip(@"C:\Soundboard\Cheryl\DRIVER INFO\dob1.mp3");
                                getAgent().AskingBDay = true;
                                dobby[0] = "";
                                dobby[1] = "";
                            }
                           
                        }
                        else
                        {
                            if (!App.getAgent().CheckForMonth(response))
                            {
                                ag.BDAYHOLDER = App.getAgent().returnNumeric(response);
                                ag.hasAsked = true;
                            }

                            else
                            {
                                string[] DayYear = response.Split(' ');
                                Console.WriteLine(DayYear[0]);
                                DayYear[0] = App.getAgent().returnNumeric(DayYear[0]);
                                DayYear[1] = App.getAgent().returnNumeric(DayYear[1]);
                                Console.WriteLine("DAY: " + DayYear[0]);
                                getAgent().selectData("frmDOB_Day", DayYear[0]);
                                Console.WriteLine("YEAR: " + DayYear[1]);
                                getAgent().selectData("frmDOB_Year", DayYear[1]);
                                ag.Question = Agent.MARITAL_STATUS;
                            }
                        }
                        break;

                    case Agent.BDAYMONTH:
                        ag.Question = Agent.MARITAL_STATUS;
                        ag.hasAsked = false;
                        break;

                    case Agent.MARITAL_STATUS:
                        
                        if (ag.cust.maritalStatus == "Married")
                        {

                            DBCommand = "INSERT INTO `MARITAL_STATUS` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            ag.Question = Agent.SPOUSE_NAME;
                            ag.hasAsked = false;
                        }
                        else
                        {
                           
                            DBCommand = "INSERT INTO `MARITAL_STATUS` (`SPEECH`,`PASS/FAIL`) VALUES('" + response + "',1)";
                            Agent.UpdateDBase(DBCommand);
                            ag.Question = Agent.OWN_OR_RENT;
                            ag.hasAsked = false;
                        }

                        break;
                    case Agent.SPOUSE_NAME:
                        ag.Question = Agent.SPOUSE_DOB; 
                        getAgent().EnterData("frmSpouseFirstName", response);
                        if (getAgent().maleNames.Contains(response)) { getAgent().selectData("frmGender", "Male"); } else { getAgent().EnterData("frmGender", "Female"); }
                        break;
                    case Agent.SPOUSE_DOB: 
                        if (!App.getAgent().CheckForMonth(response))
                        {
                            ag.BDAYHOLDER = App.getAgent().returnNumeric(response);
                            ag.Question = Agent.SPOUSEBDAYMONTH;

                        }

                        else
                        {
                            string[] DayYear = response.Split(',');
                            Console.WriteLine(DayYear[0]);
                            DayYear[0] = App.getAgent().returnNumeric(DayYear[0]);
                            DayYear[1] = App.getAgent().returnNumeric(DayYear[1]);
                            Console.WriteLine("DAY: " + DayYear[0]);
                            getAgent().selectData("frmSpouseDOB_Day", DayYear[0]);
                            Console.WriteLine("YEAR: " + DayYear[1]);
                            getAgent().selectData("frmSpouseDOB_Year", DayYear[1]);
                        }
                        break;
                    case Agent.SPOUSEBDAYMONTH:
                        ag.Question = Agent.OWN_OR_RENT; 
                        break;
                    case Agent.OWN_OR_RENT: ag.Question = Agent.RES_TYPE; break;
                    case Agent.RES_TYPE: ag.Question = Agent.ADDRESS; break;
                    case Agent.ADDRESS:
                        ag.Callpos = Agent.INBETWEEN;
                        ag.Question = Agent.EMAIL;
                        break;      
                    case Agent.EMAIL:
                        ag.Callpos = Agent.INBETWEEN;
                        ag.Question = Agent.CREDIT;
                        break;                                              
                    case Agent.CREDIT: ag.Question = Agent.PHONE_TYPE; break;
                    case Agent.PHONE_TYPE:
                        App.getAgent().cust.phone = App.getAgent().driver.FindElementById("frmPhone1").GetAttribute("value");
                        ag.Question = Agent.LAST_NAME; break;
                    case Agent.LAST_NAME:
                        ag.hasAsked = false;
                        ag.Question = Agent.SECONDARIES;
                        
                        break;
                    case Agent.SECONDARIES:
                        if (response.Contains("no")) { ag.Question = Agent.TCPA; }
                        else { ag.Question = Agent.WHICHSECONDARIES;  }
                        break;
                    case Agent.WHICHSECONDARIES:
                        if (response.ToLower().Contains("home")) { getAgent().CheckBox("frmCrossSellHome"); getAgent().doHome = true; Console.WriteLine("HOME INSURANCE INTEREST DETECTED!"); }

                        else if (response.ToLower().Contains("rent")) { getAgent().CheckBox("frmCrossSellHome"); getAgent().doRenters = true; Console.WriteLine("RENTAL INSURANCE INTEREST DETECTED!"); }

                        if (response.ToLower().Contains("life")) { getAgent().CheckBox("frmCrossSellLife"); getAgent().doLife = true; Console.WriteLine("LIFE INSURANCE INTEREST DETECTED!"); }

                        if (response.ToLower().Contains("health")) { getAgent().CheckBox("frmCrossSellHealth"); getAgent().doHealth = true; Console.WriteLine("HEALTH INSURANCE INTEREST DETECTED!"); }

                        else if (response.ToLower().Contains("medicare")) { getAgent().CheckBox("frmCrossSellHealth"); getAgent().doMedicare = true; Console.WriteLine("MEDICARE INSURANCE INTEREST DETECTED!"); }
                        getAgent().Callpos = Agent.INBETWEEN;
                        if (getAgent().doHome || getAgent().doRenters) { ag.Question = Agent.YEARBUILT;}
                        else { ag.Question = Agent.TCPA;break; }
                        break;
                    case Agent.YEARBUILT:
                        string yearBuilt = getAgent().returnNumeric(response);
                        if(yearBuilt != "")
                        {
                            getAgent().selectData("frmYearBuilt", yearBuilt);
                            if (getAgent().doRenters) { getAgent().Question = Agent.PPC; }
                            else { getAgent().Question = Agent.SQFT; }


                        }
                        break;
                    case Agent.SQFT:
                        string SQFEET = getAgent().returnNumeric(response);
                        if (SQFEET != "")
                        {
                            getAgent().EnterData("frmSqFt", SQFEET);
                            getAgent().Question = Agent.TCPA; 

                        }
                        break;
                    case Agent.PPC:
                        string PPC = getAgent().GETPPC(response);
                        if (PPC != "")
                        {                          
                            getAgent().selectData("frmPersonalPropertyCoverage", PPC);
                            getAgent().Question = Agent.TCPA;

                        }
                        break;
                    case "REPEAT":
                    case Agent.TCPA:

                        if (response.ToLower().TrimEnd('.','?','!').Contains("yes") || response.Contains("ok") || response.ToLower().TrimEnd('.', '?', '!').Contains("fine") || response.ToLower().TrimEnd('.', '?', '!').Contains("okay") || response.ToLower().TrimEnd('.', '?', '!').Contains("sure") || response.Contains("yep") || response.Contains("yeah") || response.Contains("sounds good") || response.Contains("absolutely") || response.Contains("alright"))
                        {
                            App.getAgent().endcall = true;
                            getAgent().Callpos = Agent.INBETWEEN;
                            App.getAgent().Question = "";
                            App.getAgent().selectData("frmTcpaConsumerConsented", "Responded YES, said sure, I agree, that's okay, etc.");
                            string name = App.getAgent().cust.firstName + " " + App.getAgent().driver.FindElementById("frmLastName").GetAttribute("value");
                            string phone = App.getAgent().cust.phone;
                            DBCommand = "INSERT INTO `LEADS` (`AGENT`, `NAME`, `PHONE`, `LEAD_ID`, `LEAD_GUID`, `IMPORT_ID`) VALUES ('" + App.getAgent().AgentNum + "','" + name + "','" + phone + "','" + App.getAgent().cust.LeadID + "','" + App.getAgent().cust.LEADGUID + "','" + App.getAgent().cust.IMPORT_ID + "')";
                            Agent.UpdateDBase(DBCommand);
                            App.getAgent().driver.FindElementById("btnSubmit").Click();
                            App.getAgent().SilenceTimer = 0;
                            App.getAgent().SilenceTimer = 0;
                            getAgent().Question = "ENDCALL";
                            Console.WriteLine("called endcall successfully");
                            App.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\ENDCALL.mp3");
                            App.screenshots ++;
                            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/";
                            try
                            {
                                OpenQA.Selenium.Screenshot Lead = App.getAgent().driver.GetScreenshot();
                                Lead.SaveAsFile(path + "Lead" + App.screenshots + ".jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }                                                                                                      
                        }
                        else if(response.ToLower().TrimEnd('.', '?', '!').Contains("no") || response.Contains("no thank you") || response.ToLower().TrimEnd('.', '?', '!').Contains("do not consent") || response.ToLower().TrimEnd('.', '?', '!').Contains("not okay") || response.ToLower().TrimEnd('.', '?', '!').Contains("nope") || response.Contains("don't think so") || response.Contains("negative") || response.Contains("sounds bad"))
                        {
                            getAgent().selectData("frmTcpaConsumerConsented", "Responded NO, did not respond, hung up, etc.");
                            getAgent().Callpos = Agent.INBETWEEN;
                            getWindow().cmbDispo.SelectedIndex = 7;
                            getAgent().driver.FindElementById("btnSubmit").Click();
                            getAgent().Question = "";
                            getAgent().low_blow_bro = true;
                            RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
                            getAgent().HangUpandDispo("LOW");
                        }

                        else
                        {
                            getAgent().Callpos = Agent.INBETWEEN;
                            getAgent().Question = "REPEAT";
                        }
                        break;
                }
            }
             if (App.getAgent().Callpos == Agent.FIXING) {getAgent().FixLead(); }
        }
        public async void doIntroduction()
        {
            Agent ag = getAgent();
            if (ag.cust.isNameEnabled)
            {
                string clip = findNameClips(ag.cust.firstName)[0];
                bool x = await RollTheClipAndWait(clip);
            }
            else if (!ag.cust.isNameEnabled)
            {
                string clip = @"C:\Soundboard\Cheryl\INTRO\Intro2.mp3";
                bool x = await RollTheClipAndWait(clip);
            }
        }

        /*
         *  End testing stuff. You can touch stuff again.
         *  Creep.
         */
        public static string[] findNameClips(string name)
        {
            string namesDir = @"C:\SoundBoard\Cheryl\NAMES";
            string[] nameClips = new string[3];
            string check1 = namesDir + @"\" + name + " 1.mp3";
            string check2 = namesDir + @"\" + name + " 2.mp3";
            string check3 = namesDir + @"\" + name + " 3.mp3";

            if (System.IO.File.Exists(check1))
            {
                nameClips[0] = check1;
            } else
            {
                nameClips[0] = "no clip";
            }
            if (System.IO.File.Exists(check2))
            {
                nameClips[1] = check2;
            } else
            {
                nameClips[1] = "no clip";
            }
            if (System.IO.File.Exists(check3))
            {
                nameClips[2] = check3;
            } else
            {
                nameClips[2] = "no clip";
            }
            return nameClips;
        }
        /*
         * RollTheClip is a method that's part of the application logic, not the Form logic. Therefore, it should be in the App class.
         */
        public static void onPlaybackStopped(object sender, StoppedEventArgs e)
        {
            Console.WriteLine(getAgent().Callpos);
            Console.WriteLine(getAgent().Question);
            if (getAgent().endcall == true) { getAgent().endcall = false; getAgent().HangUpandDispo("Auto Lead"); }
            if (getAgent().low_blow_bro) { getAgent().low_blow_bro = false;  getAgent().HangUpandDispo("LOW");  }
            if (getAgent().inCall || getAgent().testing){ longDictationClient.StartMicAndRecognition();}
            Agent user = getAgent();
            Console.WriteLine("CHERYL JUST REBUTTALED " + getAgent().currentlyRebuttaling);   
            user.isTalking = false;
            Console.WriteLine("PLAYBACK STOPPED. CHERYL TALKING: " + user.isTalking );
            //Console.WriteLine(user.Callpos);
            //Console.WriteLine(user.Question);
            waveOutIsStopped = true;
            if (getAgent().Question == "START") { longDictationClient.StartMicAndRecognition(); }
            if (getAgent().currentlyRebuttaling == true)
            {
                user.custObjected = false;
                user.currentlyRebuttaling = false;
                Console.WriteLine(user.Question);
                if (user.Question == "INTRO") { user.Question = Agent.PROVIDER; }
                user.AskQuestion();
                return;
            }
            if(user.Callpos == "INBETWEEN")
            {
                longDictationClient.StartMicAndRecognition();
                // hidden below is a massive switch statement...
                switch (user.Question)
                {
                    case Agent.STARTYMCSTARTFACE:
                        user.Callpos = Agent.INTRO; 
                        break;
                    case Agent.INTRO:
                        user.Callpos = Agent.INTRO;
                        break;
                    case "INS_PROVIDER":
                        user.Callpos = Agent.PROVIDER;
                        break;
                    case "INS_EXP":
                        user.Callpos = Agent.INS_EXP;
                        break;
                    case "INS_START":
                        user.Callpos = Agent.INST_START;
                        break;
                    case "NUM_VEHICLES":
                        user.Callpos = Agent.NUM_VEHICLES;
                        break;
                    case "YMM1":
                        user.Callpos = Agent.YMM1;
                        break;
                    case "YMM2":
                        user.Callpos = Agent.YMM2;
                        break;
                    case "YMM3":
                        user.Callpos = Agent.YMM3;
                        break;
                    case "YMM4":
                        user.Callpos = Agent.YMM4;
                        break;
                    case "DOB":
                        user.Callpos = Agent.DOB;
                        break;
                    case Agent.BDAYMONTH:
                        user.Callpos = Agent.BDAYMONTH;
                        break;
                    case "MARITAL_STATUS":
                        user.Callpos = Agent.MARITAL_STATUS;
                        break;
                    case "SPOUSE_NAME":
                        user.Callpos = Agent.SPOUSE_NAME;
                        break;
                    case "SPOUSE_DOB":
                        user.Callpos = Agent.SPOUSE_DOB;
                        break;
                    case Agent.SPOUSEBDAYMONTH:
                        user.Callpos = Agent.SPOUSEBDAYMONTH;
                        break;
                    case "OWN OR RENT":
                        user.Callpos = Agent.OWN_OR_RENT;
                        break;
                    case "RESIDENCE TYPE":
                        user.Callpos = Agent.RES_TYPE;
                        break;
                    case "CREDIT":
                        user.Callpos = Agent.CREDIT;
                        break;
                    case "ADDRESS":
                        user.Callpos = Agent.ADDRESS;
                        break;
                    case "EMAIL":
                        user.Callpos = Agent.EMAIL;
                        break;
                    case "PHONE TYPE":
                        user.Callpos = Agent.PHONE_TYPE;
                        break;
                    case "LAST NAME":
                        user.Callpos = Agent.LAST_NAME;
                        break;
                    case "SECONDARIES":
                        user.Callpos = Agent.SECONDARIES;
                        break;
                    case Agent.WHICHSECONDARIES:
                        if (getAgent().doHome || getAgent().doRenters) { user.Callpos = Agent.YEARBUILT; }
                        else { user.Callpos = Agent.TCPA; break; }
                        break;
                    case "TCPA":
                        user.Callpos = Agent.TCPA;
                        break;
                    case "REPEAT":
                        user.Callpos = "REPEAT";
                        break;
                }
            }
            else if(getAgent().Callpos == Agent.FIXING)
            {
                Current.Dispatcher.Invoke(() => { getAgent().FixLead(); });

            }
        }
        public static bool RollTheClip(string Clip)
        {
            //if (Clip == "no clip")
            //{
            //    return false;
            //}

                Console.WriteLine("CLIP: " + Clip + " CHERYL IS TALKING: " + getAgent().isTalking);
                try
                {
                    App.waveOutIsStopped = false;
                    getAgent().isTalking = true;
                    getAgent().SilenceTimer = 0;
                    StopTheClip();
                    waveOut = new WaveOut();
                    waveOut.PlaybackStopped += onPlaybackStopped;
                    Mp3FileReader Reader = new Mp3FileReader(Clip);
                    waveOut.Init(Reader);
                    waveOut.Play();
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

        

        public static async Task<bool> RollTheClipAndWait(string Clip)
        {
            App.getAgent().SilenceTimer = 0;
            //if (Clip == "no clip")
            //{
            //    return false;
            //}
            Console.WriteLine("CLIP");
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
                waveOut.Stop();
                waveOut.Dispose();
                waveOutIsStopped = true;
            }
            catch
            {
                Console.WriteLine("couldn't stop...probably not a clip playing.");
            }
        }
        private static int okClipIndex = 0;
        public static void playOkClip()
        {
            string[] okClips = new string[] {
                @"C:\Soundboard\Cheryl\REACTIONS\OK.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\OK2.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\okGreat.mp3",
            };

            string clip = okClips[okClipIndex];
            if (okClipIndex >= okClips.Length - 1)
            {
                okClipIndex = 0;
            }
            else
            {
                okClipIndex += 1;
            }

            RollTheClip(clip);
        }



       
        public static async void playDobClips()
        {
            string[] dobby = getAgent().dobInfo;
            foreach (string clippy in dobby)
            {
                Console.WriteLine(clippy);
            }
            if (dobby[0] != "" && dobby[1] != "")
            {
                string moday = dobby[0] + dobby[1];
                string modayPath = @"C:\Soundboard\Cheryl\Birthday\" + moday + ".mp3";
                bool isDone = await RollTheClipAndWait(modayPath);
            }
            else
            {
                RollTheClip(@"C:\Soundboard\Cheryl\DRIVER INFO\dob1.mp3");
                getAgent().AskingBDay = true;

                return;
            }
            if (dobby[2] != "")
            {
                RollTheClip(@"C:\Soundboard\Cheryl\Birthday\" + dobby[2] + ".mp3");
            }
        }
    }
}
