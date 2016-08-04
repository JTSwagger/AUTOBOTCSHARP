using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;


namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private bool RollTheClip(string Clip)
        {
            try
            {
                Mp3FileReader Reader = new Mp3FileReader(Clip);
                var mp3 = new NAudio.Wave.WaveOut();
                mp3.Init(Reader);
                mp3.Play();
                return true;
            }
            catch
            {
                return false;
            }
          
        }


        private void btnHello_Click(object sender, RoutedEventArgs e)
        {
            RollTheClip("C:\\SoundBoard\\Cheryl\\INTRO\\hello.mp3");
        }

        private void butnThisIsCheryl_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLaugh_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnIntro_Copy4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
