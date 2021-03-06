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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace AutoBotCSharp
{
    public class Speech_Recognizer
    {
        public Speech_Recognizer()
        {



        }

        private static string _partial = "";
        private static string _Final = "";
        public const string Microsoft = "MICROSOFT";
        public const string Google = "GOOGLE";
        public static string recognizer_opt;
        public static string APIKey1;
        public static string APIKey2;
        public static MicrophoneRecognitionClient Mic;
        public static Socket sock;
        
        public static Process proc;
        public static Task task;
        public static bool shutdown = false;
        public event System.EventHandler PartialSpeech;
        public event System.EventHandler FinalSpeech;

        protected virtual void OnPartialSpeech()
        {
            if (PartialSpeech != null) PartialSpeech(this, EventArgs.Empty);
        }
        protected virtual void OnFinalSpeech()
        {
            if (FinalSpeech != null) FinalSpeech(this, EventArgs.Empty);
        }

        public string Final_Speech
        {
            get { return _Final; }
            set { _Final = value; this.OnFinalSpeech(); }

        }

        public string partial_speech
        {
            get { return _partial; }
            set { _partial = value; this.OnPartialSpeech(); }

        }

        public static string Final_Result = "";

        public async Task<bool> reco_google()
        {         
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe");                               
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            proc = Process.Start(info);
            proc.StandardInput.WriteLine("python transcribe_streaming.py");
            proc.StandardInput.Flush();
            proc.StandardInput.Close();
            sock = new Socket(System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
            sock.Connect("192.168.1.218", 6969);
            while (shutdown == false)
            {
                
                byte[] buff = new byte[1024];
                int data = sock.Receive(buff);
                char[] message = new char[data];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buff, 0, data, message, 0);
                System.String recv = new System.String(message);
                string[] poop = recv.Split('"');
                if (poop.Length > 0)
                {
                    string Speech = poop[1].Trim();
                     this.partial_speech = Speech;
                    if (recv.Contains("confidence:")) { this.Final_Speech = Speech; }

                }
            }
            proc.Kill();
            proc.WaitForExit();
            return false;
        }
//-------------------------------------------------------------------------
        public bool TurnOnMic(string Recognizer, string cues="")
        {
            switch(Recognizer)
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
            shutdown = true;
            proc.Kill();
            proc.WaitForExit();

        }
//--------------------------------------------------------------------------------
    
    }
}
