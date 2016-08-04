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
        /*
         * RollTheClip is a method that's part of the application logic, not the Form logic. Therefore, it should be in the App class.
         */
        public static bool RollTheClip(string Clip)
        {
            try
            {
                Mp3FileReader Reader = new Mp3FileReader(Clip);
                var mp3 = new NAudio.Wave.WaveOut();
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
    }
}
