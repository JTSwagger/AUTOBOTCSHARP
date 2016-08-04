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
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random randy;

        
        public MainWindow()
        {
            randy = new Random();
            InitializeComponent();
        }

        private void btnHello_Click(object sender, RoutedEventArgs e)
        {
            // Keep methods like RollTheClip in App.xaml.cs, call them like this
            App.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello.mp3");
        }

        private void butnThisIsCheryl_Click(object sender, RoutedEventArgs e)
        {

        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            App.playOkClip();
        }

        private void btnIntro_Click(object sender, RoutedEventArgs e)
        {

        }


        /*
         * Here be dragons.  
         */ 
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            int fsModifiers,
            int vk);
        // RegisterHotKey takes a window handle (normally this.Handle), an ID (whatever we want), a modifer key -- fsModifiers --  (alt, ctrl, etc) and an int-ified letter -- vk -- (ie. (int)'A', (int)Keys.F1s)
        // We should start ids at 0.
        /* modifier key values: 
         * ALT: 0x1
         * CTRL: 0x2
         * Shift: 0x4
         * Windows Key: 0x8
         */
        /* ID numbers and their assignments (add to this for reference)
         * 0 : 'OK' and related
         * 1 : Humanism button
         */
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotkey(
            IntPtr hWnd,
            int id);
        private HwndSource _source;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotkeys();
        }
        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotkeys();
            base.OnClosed(e);
        }
        private void RegisterHotkeys()
        {
            var helper = new WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, 0, 0, 0x70); // F1
            RegisterHotKey(helper.Handle, 1, 0, 0x71); // F2
        }
        private void UnregisterHotkeys()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotkey(helper.Handle, 0);
            UnregisterHotkey(helper.Handle, 1);
        }
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case 0:
                            App.playOkClip();
                            break;
                        case 1:
                            App.playHumanism();
                            break;
                    }
                    break;
            }


            return IntPtr.Zero;
        }
    }
}
