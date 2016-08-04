using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;

namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Random randy = new Random();
        /*
         * RollTheClip is a method that's part of the application logic, not the Form logic. Therefore, it should be in the App class.
         */
        public static bool RollTheClip(string Clip)
        {
            Console.WriteLine("CLIP");
            try
            {
                Mp3FileReader Reader = new Mp3FileReader(Clip);
                var mp3 = new WaveOut();
                mp3.Init(Reader);
                mp3.Play();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
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
