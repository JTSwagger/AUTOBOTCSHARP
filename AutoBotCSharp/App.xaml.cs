using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using Microsoft.ProjectOxford.SpeechRecognition;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            Agent temp;
            try
            {
               temp = getWindow().user;
            } catch (NullReferenceException)
            {
                temp = new Agent();
            }
            return temp;
        }

        /*
         * Testing Stuff No Touchy
         */

        public static void testSpeechReco(int mode)
        {
            Console.WriteLine("testing now");
            switch (mode)
            {
                case 0:
                    //shortPhraseClient.StartMicAndRecognition();
                    Console.WriteLine("shortphrase started");
                    break;
                case 1:
                    longDictationClient.StartMicAndRecognition();
                    Console.WriteLine("longdictation started");
                    break;
            }
        }
        public static void startReco()
        {
            longDictationClient.StartMicAndRecognition();
        }
        public static void onMicrophoneStatusHandler(object sender, MicrophoneEventArgs e)
        {
            if(!e.Recording)
            {
                ((MicrophoneRecognitionClient)sender).StartMicAndRecognition();
            }

        }

        public static void onPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            string response = e.PartialResult;
            Application.Current.Dispatcher.Invoke((async () =>
            {
                bool x;
                getWindow().setSpeechBoxText("Partial: " + response);
                if (!(x = await Agent.checkForObjection(response)))
                { Agent.checkforData(response); }
            }));
            
        }

        public static void onResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            foreach (RecognizedPhrase result in e.PhraseResponse.Results)
            {
                Application.Current.Dispatcher.Invoke((() =>
                {
                    getWindow().appendSpeechBoxText("Full: " + result.DisplayText);                    
                }));  
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
            Agent user = App.getAgent();
            Console.WriteLine("PLAYBACK STOPPED");
            Console.WriteLine(user.Callpos);
            Console.WriteLine(user.Question);
            waveOutIsStopped = true;
            if(user.Callpos == "INBETWEEN")
            {
                // hidden below is a massive switch statement...
                switch (user.Question)
                {
                    case "INS_PROVIDER": user.Callpos = Agent.INS_PROVIDER; break;
                    case "INS_EXP": user.Callpos = Agent.INS_EXP; break;
                    case "YMM1": user.Callpos = Agent.YMM1; break;
                    case "YMM2": user.Callpos = Agent.YMM2; break;
                    case "YMM3": user.Callpos = Agent.YMM3; break;
                    case "YMM4": user.Callpos = Agent.YMM4; break;
                    case "DOB": user.Callpos = Agent.DOB; break;
                    case "MARITAL_STATUS": user.Callpos = Agent.MARITAL_STATUS; break;
                    case "SPOUSE_NAME": user.Callpos = Agent.SPOUSE_NAME; break;
                    case "SPOUSE_DOB": user.Callpos = Agent.SPOUSE_DOB; break;
                    case "OWN OR RENT": user.Callpos = Agent.OWN_OR_RENT; break;
                    case "RESIDENCE TYPE": user.Callpos = Agent.RES_TYPE; break;
                    case "CREDIT": user.Callpos = Agent.CREDIT; break;
                    case "ADDRESS": user.Callpos = Agent.ADDRESS; break;
                    case "EMAIL": user.Callpos = Agent.EMAIL; break;
                    case "PHONE TYPE": user.Callpos = Agent.PHONE_TYPE; break;
                    case "LAST NAME": user.Callpos = Agent.LAST_NAME; break;
                    case "TCPA": user.Callpos = Agent.TCPA; break;
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
