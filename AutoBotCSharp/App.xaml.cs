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
        public Preferences prefs;
        private static Random randy = new Random();
        private static WaveOut waveOut = new WaveOut();
        private static bool waveOutIsStopped = true;
        //public static MicrophoneRecognitionClient shortPhraseClient;
        public static MicrophoneRecognitionClient longDictationClient;
       
       

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
            
            string apiKey1 = "ce43e8a4d7a844b1be7950b260d6b8bd";
            string apiKey2 = "0d2797650c8648d18474399744512f17";
            longDictationClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", apiKey1, apiKey2);
            longDictationClient.OnPartialResponseReceived += onPartialResponseReceivedHandler;
            longDictationClient.OnResponseReceived += onResponseReceivedHandler;
            Console.WriteLine("Change the reco, don't let the reco change you."); 
        }

        public static void startReco()
        {
            longDictationClient.StartMicAndRecognition();
        }
        public static void onMicrophoneStatusHandler(object sender, MicrophoneEventArgs e)
        {
            GC.KeepAlive(longDictationClient);
            Agent temp = getAgent();
            //Console.WriteLine(e.Recording);
            temp.isListening = e.Recording;
            if(!e.Recording)
            {
                longDictationClient.StartMicAndRecognition();
            }
        }

        public static void onPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            GC.KeepAlive(longDictationClient);
            //Console.WriteLine(getAgent().Question);
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

        public static void onResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            GC.KeepAlive(longDictationClient);
            //Console.WriteLine(e.PhraseResponse.RecognitionStatus);
            if (e.PhraseResponse.RecognitionStatus == ((RecognitionStatus)611) || e.PhraseResponse.RecognitionStatus.ToString() == "611")
            {
                Console.WriteLine("\n EETSA ME, MARIO! \n");
                longDictationClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", "ce43e8a4d7a844b1be7950b260d6b8bd", "0d2797650c8648d18474399744512f17");
                longDictationClient.StartMicAndRecognition();
            }
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            {
                longDictationClient.EndMicAndRecognition();
                longDictationClient.StartMicAndRecognition();
            }
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.InitialSilenceTimeout)
            {
                longDictationClient.EndMicAndRecognition();
                longDictationClient.StartMicAndRecognition();
            }
            foreach (RecognizedPhrase result in e.PhraseResponse.Results)
            {
                Current.Dispatcher.Invoke((() =>
                {
                    getWindow().appendSpeechBoxText("Full: " + result.DisplayText);
                }));
            }

            Current.Dispatcher.Invoke(() =>
            { 
                
                if(getAgent().custObjected == false)
                {
                    doBackgroundQuestionSwitchingStuff();
                    if (!getAgent().hasAsked)
                    {
                        getAgent().AskQuestion();
                        getAgent().hasAsked = true;
                    }
                    
                }
            });
            longDictationClient.StartMicAndRecognition();
        }

        public static void doBackgroundQuestionSwitchingStuff()
        {
            GC.KeepAlive(longDictationClient);
            Agent ag = getAgent();
            // call position advancement
            switch (ag.Question)
            {
                case Agent.STARTYMCSTARTFACE: ag.Question = Agent.INTRO; break;
                case Agent.INTRO: ag.Question = Agent.INS_EXP; break;
                case Agent.PROVIDER: ag.Question = Agent.INS_EXP; break;
                case Agent.INS_EXP: ag.Question = Agent.INST_START; break;
                case Agent.INST_START: ag.Question = Agent.NUM_VEHICLES; break;
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
                case Agent.YMM_ONLY_ONE: ag.Question = Agent.DOB; break;
                case Agent.YMM1: ag.Question = Agent.YMM2; break;
                case Agent.YMM2:
                    if (ag.cust.numVehicles > 2)
                    {
                        ag.Question = Agent.YMM3;
                    }
                    else { ag.Question = Agent.DOB; }
                   
                    break;
                case Agent.YMM3:
                    if (ag.cust.numVehicles > 3)
                    {
                        ag.Question = Agent.YMM4;
                    }
                    else { ag.Question = Agent.DOB;}
                    break;
                case Agent.YMM4: ag.Question = Agent.DOB; break;
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
            Agent user = getAgent();
            //Console.WriteLine("PLAYBACK STOPPED");
            //Console.WriteLine(user.Callpos);
            //Console.WriteLine(user.Question);
            waveOutIsStopped = true;
            if(user.custObjected == true)
            {
                user.custObjected = false;
                //Console.WriteLine(user.Question);
                user.AskQuestion();
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
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.InnerException);
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
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public static void StopTheClip()
        {
            waveOut.Stop();
            waveOut.Dispose();
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
