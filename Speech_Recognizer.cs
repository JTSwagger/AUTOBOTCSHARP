﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.SpeechRecognition;
using System.Net.Sockets;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Timers;
using Google.Apis;
using Grpc.Core;
using Google.Apis.Oauth2;



#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace AutoBotCSharp
{
    public class Speech_Recognizer
    {
        public Speech_Recognizer(int port, string context = null)
        {
            shutdown = false;
            PORT = port;
            Context = context;
           
        }


        public void makeChannel(int host, int port)
        {         
           // Oauth2Service service = new Oauth2Service();
           // service.
        }


        public int timeout = 0;
        public int PORT;
        public string Context = "";
        public bool MicOn = false;
        private static string _partial = "";
        private static string _Final = "";
        public const string Microsoft = "MICROSOFT";
        public const string Google = "GOOGLE";
        public static string recognizer_opt;
        public static string APIKey1;
        public static string APIKey2;
        public static MicrophoneRecognitionClient Mic;
        public static Socket sock;
        public BackgroundWorker b;
        public static Process proc;
        public static Task task;
        public static Task timer;
        public static bool shutdown = false;
        public event System.EventHandler PartialSpeech;
        public event System.EventHandler FinalSpeech;
        public event System.EventHandler MicChange;

        protected virtual void OnPartialSpeech()
        {
            PartialSpeech?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnFinalSpeech()
        {
            FinalSpeech?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMicChange()
        {
            MicChange?.Invoke(this, EventArgs.Empty);
        }

        public bool is_recording
        {
            get { return MicOn; }
            set { MicOn = value; OnMicChange(); }

        }
        public string Final_Speech
        {
            get { return _Final; }
            set { _Final = value; OnFinalSpeech(); }

        }

        public string partial_speech
        {
            get { return _partial; }
            set { _partial = value; OnPartialSpeech(); }

        }

        public static string Final_Result = "";


        public async Task<bool> reco_google()
        {

            Console.WriteLine("starting reco on port---->" + PORT + Environment.NewLine + "Using Context------>" + Context);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\creds.json");
            foreach (Process proc in Process.GetProcessesByName("python"))
            {
                proc.Kill();
            }
            shutdown = false;
            Console.WriteLine("***STARTING GOOGLE SPEECH RECO***");
            ProcessStartInfo info = new ProcessStartInfo("python");
            info.UseShellExecute = true;
            info.Arguments = "transcribe_streaming.py " + PORT + " ";
            if (Context != null) { info.Arguments += Context; }
            proc = Process.Start(info);
            Thread.Sleep(300);
            sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.Connect("localhost", PORT);
            while (shutdown == false)
            {
                this.is_recording = true;
                byte[] buff = new byte[1024];
                int data = sock.Receive(buff);
                char[] message = new char[data];
                Decoder d = Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buff, 0, data, message, 0);
                string recv = new string(message);
                // if(recv == "::FINAL::" || recv.Contains("::FINAL::")) { Console.WriteLine("FINAL RESULT"); }
                string[] poop = recv.Split('"');

                if (poop.Length > 0)
                {

                    string Speech = poop[1].Trim();
                    Speech = Speech.Replace("\\", "");
                    partial_speech = Speech;
                    if (recv.Contains("confidence:") || recv.Contains("<<FINAL>>")) { Final_Speech = Speech; }


                }
            }
            return false;
        }



        public async Task<bool> counter()
        {
            while (shutdown == false)
            {
                while (timeout < 3000)
                {

                    Thread.Sleep(500);
                    timeout += 500;
                    Console.WriteLine("waiting for final speech " + timeout);
                }
                Console.WriteLine("It's been 3 seconds. Timeout to final speech");
                Final_Speech = partial_speech;
                timeout = 0;
            }
            return (false);

        }
        //-------------------------------------------------------------------------
        public bool TurnOnMic(string Recognizer, string cues = "")
        {
            switch (Recognizer)
            {
                case Microsoft:
                    try
                    {
                        Mic = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.LongDictation, "en-US", APIKey1, APIKey2);
                        return (true);
                    }
                    catch
                    {
                        return (false);
                    }
                case "Google":
                case Google:
                    task = Task.Run(reco_google);


                    return true;
                default:
                    return false;
            }
        }
        //------------------------------------------------------------------------------------
        public void TurnOffMic()
        {
            try
            {
                Console.WriteLine("***STOPPING GOOGLE SPEECH RECO***");
                byte[] toBytes = Encoding.ASCII.GetBytes("TURNOFF::");
                sock.Send(toBytes);
                sock.Close();
                shutdown = true;
                this.is_recording = false;
                foreach (Process proc in Process.GetProcessesByName("python"))
                {
                    proc.Kill();
                }
            }
            catch
            {
                Console.WriteLine("Could not shut off socket...socket is already shutoff");
            }

        }

        public void clearSpeech()
        {
            _partial = "";
            _Final = "";
        }
        //--------------------------------------------------------------------------------

    }
}
