using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using Microsoft.ProjectOxford.SpeechRecognition;
using Microsoft.ProjectOxford;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

        /*
         * Returns the current agent object available to MainWindow.
         * :)
         */ 
        public static Agent getAgent()
        {
            Agent temp = null; 
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

       public static void reInitMicClient()
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\keys.txt";
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
            Console.WriteLine(e.SpeechErrorText);
       

        }
        public static void onMicrophoneStatusHandler(object sender, MicrophoneEventArgs e)
        {
        
            Agent temp = getAgent();
            Console.WriteLine("MIC IS RECORDING: " + e.Recording);
            temp.isListening = e.Recording;

        }


        public static void onPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {

            string response = e.PartialResult;
            Current.Dispatcher.Invoke((async () =>
            {
                bool x;
                getWindow().setSpeechBoxText("Partial: " + response);
                if (!(x = await Agent.checkForObjection(response)))
                {
                    Agent.checkforData(response);
                    
                    getAgent().hasAsked = false;
                }
                getAgent().custObjected = x;
                
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
                Current.Dispatcher.Invoke(async () => { REMIX(); });
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


                    Current.Dispatcher.Invoke(() =>
                    {

                        if (getAgent().custObjected == false)
                        {
                            if (waveOutIsStopped)
                            {
                                
                                    doBackgroundQuestionSwitchingStuff();
                                    if (!getAgent().hasAsked)
                                    {
                                        getAgent().AskQuestion();
                                        getAgent().hasAsked = true;
                                    }
                                
                                
                            }
                        }
                    });

                }
            }      
        }

        public static void doBackgroundQuestionSwitchingStuff()
        {
            Agent ag = getAgent();
            // call position advancement
            if (waveOutIsStopped)
            {
                switch (ag.Question)
                {
                    case Agent.STARTYMCSTARTFACE: ag.Question = Agent.INTRO; break;
                    case Agent.INTRO:
                        if (ag.driver.FindElementById("frmInsuranceCarrier").GetAttribute("value") != "") { ag.Question = Agent.INS_EXP; ag.hasAsked = false; } else { ag.hasAsked=true; } break; 
                    case Agent.PROVIDER:
                        if (ag.driver.FindElementById("frmInsuranceCarrier").GetAttribute("value") != "") { ag.Question = Agent.INS_EXP; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        break;
                    case Agent.INS_EXP:
                        if (ag.driver.FindElementById("frmPolicyExpires_Month").GetAttribute("value") != "") { ag.Question = Agent.INST_START; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        break;
                    case Agent.INST_START:
                        if (ag.driver.FindElementById("frmPolicyStart_Month").GetAttribute("value") != "") { ag.Question = Agent.NUM_VEHICLES; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        break;
                    case Agent.NUM_VEHICLES:
                        if (ag.cust.numVehicles > 1)
                        {
                            ag.Question = Agent.YMM1;
                        }
                        else if (ag.cust.numVehicles == 1)
                        {
                            ag.Question = Agent.YMM_ONLY_ONE;
                        }

                        break;
                    case Agent.YMM_ONLY_ONE:
                        if (ag.driver.FindElementById("vehicle-model").Displayed ) { ag.Question = Agent.DOB; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        break;
                    case Agent.YMM1:
                        if (ag.driver.FindElementById("vehicle-model").Displayed) { ag.Question = Agent.YMM2; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        break;
                    case Agent.YMM2:
                        if (ag.cust.numVehicles > 2)
                        {
                            if (ag.driver.FindElementById("vehicle2-model").Displayed)  { ag.Question = Agent.YMM3; ag.hasAsked = false; } else { ag.hasAsked = true; }
                          
                        }
                        else
                        {
                            if (ag.driver.FindElementById("vehicle2-model").Displayed) { ag.Question = Agent.DOB; ag.hasAsked = false; } else { ag.hasAsked = true; }
                            break;
                        }

                        break;
                    case Agent.YMM3:
                        if (ag.cust.numVehicles > 3)
                        {
                            if (ag.driver.FindElementById("vehicle3-model").Displayed){ ag.Question = Agent.YMM4; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        }
                        else
                        {
                            if (ag.driver.FindElementById("vehicle3-model").Displayed) { ag.Question = Agent.DOB; ag.hasAsked = false; } else { ag.hasAsked = true; }                          
                        }
                        break;
                    case Agent.YMM4:
                        if (ag.driver.FindElementById("vehicle4-model").Displayed) { ag.Question = Agent.DOB; ag.hasAsked = false; } else { ag.hasAsked = true; }
                        break;

                    case Agent.DOB: ag.Question = Agent.MARITAL_STATUS; break;

                    case Agent.MARITAL_STATUS:
                        if (ag.cust.maritalStatus == "Married")
                        {
                            ag.Question = Agent.SPOUSE_NAME;
                        }
                        else
                        {
                            ag.Question = Agent.OWN_OR_RENT;
                        }

                        break;
                    case Agent.SPOUSE_NAME: ag.Question = Agent.SPOUSE_DOB; break;
                    case Agent.SPOUSE_DOB: ag.Question = Agent.OWN_OR_RENT; break;
                    case Agent.OWN_OR_RENT: ag.Question = Agent.RES_TYPE; break;
                    case Agent.RES_TYPE: ag.Question = Agent.ADDRESS; break;
                    case Agent.ADDRESS: ag.Question = Agent.EMAIL; break;
                    case Agent.EMAIL: ag.Question = Agent.CREDIT; break;
                    case Agent.CREDIT: ag.Question = Agent.PHONE_TYPE; break;
                    case Agent.PHONE_TYPE: ag.Question = Agent.LAST_NAME; break;
                    case Agent.LAST_NAME: ag.Question = Agent.TCPA; break;

                }
            }
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
            longDictationClient.StartMicAndRecognition();
            Agent user = getAgent();
            //Console.WriteLine("PLAYBACK STOPPED");
            //Console.WriteLine(user.Callpos);
            //Console.WriteLine(user.Question);
            waveOutIsStopped = true;
            Console.WriteLine(waveOutIsStopped);
            if(user.custObjected == true)
            {
                user.custObjected = false;
                //Console.WriteLine(user.Question);
                user.AskQuestion();
                waveOut.Dispose();
                return;
            }
            if(user.Callpos == "INBETWEEN")
            {
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
                    case "MARITAL_STATUS":
                        user.Callpos = Agent.MARITAL_STATUS;
                        break;
                    case "SPOUSE_NAME":
                        user.Callpos = Agent.SPOUSE_NAME;
                        break;
                    case "SPOUSE_DOB":
                        user.Callpos = Agent.SPOUSE_DOB;
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
                    case "TCPA":
                        user.Callpos = Agent.TCPA;
                        break;
                }
            }
        }
        public static bool RollTheClip(string Clip)
        {
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
                return true;
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
            waveOut.Stop();
            waveOut.Dispose();
            waveOutIsStopped = false;
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

        private static int humanismIndex = 0;

        public static void playHumanism()
        {
            string[] humanismClips = new string[]
            {
                @"C:\Soundboard\Cheryl\REACTIONS\Excellent 2.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\Great 2.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\Wonderful.mp3",
            };
            
            string clip = humanismClips[humanismIndex];
            if (humanismIndex >= humanismClips.Length - 1)
            {
                humanismIndex = 0;
            }
            else
            {
                humanismIndex += 1;
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
                return;
            }
            if (dobby[2] != "")
            {
                RollTheClip(@"C:\Soundboard\Cheryl\Birthday\" + dobby[2] + ".mp3");
            }
        }
    }
}
