﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using Microsoft.ProjectOxford.SpeechRecognition;

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
        public static MicrophoneRecognitionClient shortPhraseClient;
        public static MicrophoneRecognitionClient longDictationClient;

        //private static string clipDir;
        
        public static void setupMicRecogClient()
        {
            string apiKey1 = "da75bfe0a6bc4d2bacda60b10b5cef7e";
            string apiKey2 = "c36c061f0b8748bd862aa5bbcceda683";
            shortPhraseClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.ShortPhrase, "en-US", apiKey1, apiKey2);
            longDictationClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", apiKey1, apiKey2);

            shortPhraseClient.OnPartialResponseReceived += onPartialResponseReceivedHandler;
            longDictationClient.OnPartialResponseReceived += onPartialResponseReceivedHandler;

            shortPhraseClient.OnResponseReceived += onResponseReceivedHandler;
            longDictationClient.OnResponseReceived += onResponseReceivedHandler;
        }


        public static MainWindow getWindow()
        {
            var mainwindow = App.Current.MainWindow as MainWindow;
            return mainwindow;
        }

        /*
         * Returns the current agent object available to MainWindow.
         * :)
         */ 
        public static Agent getAgent()
        {
            return getWindow().user;
        }

        public static void testSpeechReco(int mode)
        {
            Console.WriteLine("testing now");
            switch (mode)
            {
                case 0:
                    shortPhraseClient.StartMicAndRecognition();
                    Console.WriteLine("shortphrase started");
                    break;
                case 1:
                    longDictationClient.StartMicAndRecognition();
                    Console.WriteLine("longdictation started");
                    break;
            }
        }

        public static void onPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            string response = e.PartialResult;
            Application.Current.Dispatcher.Invoke((() =>
            {
                getWindow().setSpeechBoxText("Partial: " + response);
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
                (sender as MicrophoneRecognitionClient).AudioStop();
                
            }
        }

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
            waveOutIsStopped = true;
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
            string[] dobby = getAgent().getDob();
            string moday = dobby[0] + dobby[1];
            string modayPath = @"C:\Soundboard\Cheryl\Birthday\" + moday + ".mp3";
            bool isDone = await RollTheClipAndWait(modayPath);
            RollTheClip(@"C:\Soundboard\Cheryl\Birthday\" + dobby[2] + ".mp3");
        }
    }
}
