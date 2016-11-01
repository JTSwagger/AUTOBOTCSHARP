using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.ProjectOxford.SpeechRecognition;
using ApiAiSDK;
using System.ComponentModel;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ApiAi apiAi;

        public static AIConfiguration config = new AIConfiguration("6c83e9e057134cd792c045595aca9528 ", SupportedLanguage.English);

        public static int version = 66;
        public string session = "";
        public string url = "";
        public static int screenshots = 0;
        public static Dictionary<string, bool> RESULTS = new Dictionary<string, bool>();

        public static bool failedEntry = false;
        public Preferences prefs;
        private static Random randy = new Random();

        //public static MicrophoneRecognitionClient shortPhraseClient;


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
        }
           


       // }
       // public static void onConversationErrorHandler(object sender, SpeechErrorEventArgs e)
       // {
       //     Console.WriteLine("ERROR WITH SPEECH" + e.SpeechErrorText);

       // }
       // public static void onMicrophoneStatusHandler(object sender, MicrophoneEventArgs e)
       // {

       //     Agent temp = getAgent();
       //     Console.WriteLine("MIC IS RECORDING: " + e.Recording);

       //     Current.Dispatcher.Invoke((() => App.getWindow().lblreco.Content = "RECORDING STATUS: " + e + "  AutoFormQuestion--------->" + Agent.AutoFormQuestion));
       //     temp.isListening = e.Recording;

       //}


       // public static void onPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
       // {
            
       //     string response = e.PartialResult;
       //     string raw = response;
       //     SilenceTimer = 0;
       //     Current.Dispatcher.Invoke((async () =>
       //     {
       //         SilenceTimer = 0;
       //         Console.WriteLine(SilenceTimer);
       //         bool x;
       //         getWindow().setSpeechBoxText("Partial: " + response);
       //         if (!(getAgent().currentlyRebuttaling = await Agent.checkForObjection(response)))
       //         {
       //             x = await getAgent().checkforData(response);
       //             getAgent().hasAsked = false;
       //         }

       //     }));
       // }





        




        public async void doIntroduction()
        {
            Agent ag = getAgent();
            if (getAgent().DoesNameClipExist())
            {
                string clip = findNameClips(getAgent().getCustomer().firstName)[0];
                bool x = await Agent.Conversation.RollTheClipAndWait(clip);
            }
            else if (!getAgent().getCustomer().isNameEnabled)
            {
                string clip = @"C:\Soundboard\Cheryl\INTRO\Intro2.mp3";
                bool x = await Agent.Conversation.RollTheClipAndWait(clip);
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
            }
            else
            {
                nameClips[0] = "no clip";
            }
            if (System.IO.File.Exists(check2))
            {
                nameClips[1] = check2;
            }
            else
            {
                nameClips[1] = "no clip";
            }
            if (System.IO.File.Exists(check3))
            {
                nameClips[2] = check3;
            }
            else
            {
                nameClips[2] = "no clip";
            }
            return nameClips;
        }
        /*
         * RollTheClip is a method that's part of the application logic, not the Form logic. Therefore, it should be in the App class.
         */
        
        
        
        
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

            Agent.Conversation.RollTheClip(clip);
        }




        public static async void playDobClips()
        {
            string[] dobby = getAgent().getDOBInfo();
            if (dobby != null)
            {
                foreach (string clippy in dobby)
                {
                    Console.WriteLine(clippy);
                }

                if (dobby[0] != "" && dobby[1] != "")
                {
                    string moday = dobby[0] + dobby[1];
                    string modayPath = @"C:\Soundboard\Cheryl\Birthday\" + moday + ".mp3";
                    bool isDone = await Agent.Conversation.RollTheClipAndWait(modayPath);
                }
                else
                {
                    Agent.Conversation.RollTheClip(@"C:\Soundboard\Cheryl\DRIVER INFO\dob1.mp3");
                    getAgent().setAskingBDay(true);
                    return;
                }
                if (dobby[2] != "")
                {
                    Agent.Conversation.RollTheClip(@"C:\Soundboard\Cheryl\Birthday\" + dobby[2] + ".mp3");
                }
            }
            else
            {

                Agent.Conversation.RollTheClip(@"C:\Soundboard\Cheryl\DRIVER INFO\dob1.mp3");
                getAgent().setAskingBDay(true);
                return;

            }
        }
    }
}
