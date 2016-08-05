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
        public static MicrophoneRecognitionClient shortPhraseClient;
        public static MicrophoneRecognitionClient longDictationClient;

        //private static string clipDir;

        //private static MainWindow mainwindow = (MainWindow)Current.MainWindow;

        public static void setupMicRecogClient()
        {
            string apiKey1 = "da75bfe0a6bc4d2bacda60b10b5cef7e";
            string apiKey2 = "c36c061f0b8748bd862aa5bbcceda683";
            shortPhraseClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.ShortPhrase, "en-US", apiKey1, apiKey2);
            longDictationClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", apiKey1, apiKey2);
        }

        public static void testSpeechReco(int mode)
        {
            switch (mode)
            {
                case 0:
                    shortPhraseClient.StartMicAndRecognition();
                    break;
                case 1:
                    longDictationClient.StartMicAndRecognition();
                    break;
            }
        }

        public static void onPartialResponseRecieved(object sender, PartialSpeechResponseEventArgs e)
        {
            string response = e.PartialResult;
            //mainwindow.setSpeechBoxText(response);
        }

        public static void void_name(string name)
        {

        }
        

        /*
         * RollTheClip is a method that's part of the application logic, not the Form logic. Therefore, it should be in the App class.
         */
        public static bool RollTheClip(string Clip)
        {
            Console.WriteLine("CLIP");
            try
            {
                StopTheClip();

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
        public static void StopTheClip()
        {
            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }
        }
        public static void playOkClip()
        {
            string[] okClips = new string[] {
                @"C:\Soundboard\Cheryl\REACTIONS\OK.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\OK2.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\okGreat.mp3",
            };
            int index = randy.Next(okClips.Length);
            string clip = okClips[index];
            RollTheClip(clip);
        }
        public static void playHumanism()
        {
            string[] humanismClips = new string[]
            {
                @"C:\Soundboard\Cheryl\REACTIONS\Excellent2.mp3",
                @"C:\Soundboard\Cheryl\REACTIONS\Great 2.mp3",
            };
            int index = randy.Next(humanismClips.Length);
            string clip = humanismClips[index];
            RollTheClip(clip);
        }



    }
}
