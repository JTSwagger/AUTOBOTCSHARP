﻿using System;
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
        public Agent user;

        public MainWindow()
        {
            randy = new Random();
            InitializeComponent();
        }

        public void setNameText(string name)
        {
            btnTheirName.Content = name;
        }

        public int getTabControlIndex()
        {
            return tabControl.TabIndex;
        }
        public void setSpeechBoxText(string text)
        {
            speechTxtBox.Text = text;
        }
        public void appendSpeechBoxText(string text)
        {
            speechTxtBox.Text += "\n" + text;
        }
        private void txtAgentNum_GotFocus(object sender, RoutedEventArgs e)
        {
            txtAgentNum.Text = "";
        }

        // below is a bunch of button code.
        // Intro stuff button group
        private void btnHello_Click(object sender, RoutedEventArgs e)
        {
            // Keep methods like RollTheClip in App.xaml.cs, call them like this
            App.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello.mp3");
        }
        private void btnIntro_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\INTRO\Intro2.mp3";
            App.RollTheClip(clip);
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            user = new Agent();
            string AgentNum = txtAgentNum.Text;
            user.Login(AgentNum);
        }
        private void btnTheirName_Click(object sender, RoutedEventArgs e)
        {
            App.RollTheClip(App.findNameClips(btnTheirName.Content.ToString())[2]);
        }
        // Insurance info button group
        private void btnInsuranceProvider_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3";
            App.RollTheClip(clip);
        }
        private void btnPolicyExpiration_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3";
            App.RollTheClip(clip);
        }
        private void btnPolicyStart_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3";
            App.RollTheClip(clip);
        }
        // Vehicle info button group
        private void btnHowManyVehicles_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\How many vehicles do you have.mp3";
            App.RollTheClip(clip);
        }
        private void btnYmm1_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\First Vehicle.mp3";
            App.RollTheClip(clip);
        }
        private void btnYMM2_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\2nd Vehicle.mp3";
            App.RollTheClip(clip);
        }
        private void btnYMM3_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\Third Vehicle.mp3";
            App.RollTheClip(clip);
        }
        private void btnYMM4_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\Fourth Vehicle.mp3";
            App.RollTheClip(clip);
        }
        // Driver info button group
        private void btnDOB_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\DOB1.mp3";
            App.RollTheClip(clip);
        }
        private void btnMaritalStatus_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\Marital Status.mp3";
            App.RollTheClip(clip);
        }
        private void btnSpouseName_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses First name.mp3";
            App.RollTheClip(clip);
        }
        private void btnSpouseDOB_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses Date of Birth.mp3";
            App.RollTheClip(clip);
        }
        // Personal info button group
        private void cmboOwnRent_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Do You Own Or Rent the Home.mp3";
            App.RollTheClip(clip);
        }
        private void btnHomeType_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\HOMETYPE.mp3";
            App.RollTheClip(clip);
        }
        private void btnAddress_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\ADDRESS.mp3";
            App.RollTheClip(clip);
        }
        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\EMAIL.mp3";
            App.RollTheClip(clip);
        }
        private void btnCredit_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Credit.mp3";
            App.RollTheClip(clip);
        }
        private void btnPhoneType_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\PHONETYPE.mp3";
            App.RollTheClip(clip);
        }
        private void btnLastName_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Last Name.mp3";
            App.RollTheClip(clip);
        }
        private void btnTCPA_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\WRAPUP\TCPA.mp3";
            App.RollTheClip(clip);
        }
        // Reactions button group
        private void btnWhatIGot_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\VeryGood.mp3";
            App.RollTheClip(clip);
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            App.playOkClip();
        }
        private void butnThisIsCheryl_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\INTRO\CHERYLCALLING.mp3";
            App.RollTheClip(clip);
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Yes.mp3";
            App.RollTheClip(clip);
        }
        private void btnRepeatThat_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Can you repeat that.mp3";
            App.RollTheClip(clip);
        }
        private void btnLookingFor_Click(object sender, RoutedEventArgs e)
        {
            App.RollTheClip(App.findNameClips(btnTheirName.Content.ToString())[1]);
        }
        private void btnGreatQ_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\That's a great question.mp3";
            App.RollTheClip(clip);
        }
        private void btnThatsOk_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\That's okay.mp3";
            App.RollTheClip(clip);
        }
        private void btnLaugh_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\REACTIONS\Loud-laugh.mp3";
            App.RollTheClip(clip);
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\No.mp3";
            App.RollTheClip(clip);
        }
        private void btnSpellThat_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\TIE INS\Could You Please Spell That Out.mp3";
            App.RollTheClip(clip);
        }
        private void btnHi_Click(object sender, RoutedEventArgs e)
        {
            App.RollTheClip(App.findNameClips(btnTheirName.Content.ToString())[0]);
        }

        private void btnCompUnd_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I completely understand.mp3";
            App.RollTheClip(clip);
        }

        private void btnHelloQues_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Hello 4.mp3";
            App.RollTheClip(clip);
        }

        private void btnTY_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Thank you.mp3";
            App.RollTheClip(clip);
        }

        private void btnYW_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\You're-welcome.mp3";
            App.RollTheClip(clip);
        }

        private void btnSorry_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
            App.RollTheClip(clip);
        }

        private void btnSadSorry_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Sorry to Hear That 2.mp3";
            App.RollTheClip(clip);
        }

        private void btnWhat_sLCN_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\What's LCN.mp3";
            App.RollTheClip(clip);
        }

        private void btnWhatsThisRegarding_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
            App.RollTheClip(clip);
        }

        private void btnWheredYouGetInfo_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\where did you get my info.mp3";
            App.RollTheClip(clip);
        }

        private void btnDontYouHaveThis_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I'm required to have you verify it first.mp3";
            App.RollTheClip(clip);
        }

        private void btnWillIGetManyCalls_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Will I get many calls.mp3";
            App.RollTheClip(clip);
        }

        private void btnCantTheyEmail_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Can They Email.mp3";
            App.RollTheClip(clip);
        }

        private void btnCanYouQuote_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\You're not giving me a quote.mp3";
            App.RollTheClip(clip);
        }

        private void btnWhenWillTheyCall_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\When will they call.mp3";
            App.RollTheClip(clip);
        }

        private void btnWhyDoYouNeed_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Why do you need my info.mp3";
            App.RollTheClip(clip);
        }

        private void btnNotGivingThatOut_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Let me Just confirm a few things.mp3";
            App.RollTheClip(clip);
        }

        private void btHappyWithInsurance_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\HappyWithInsurance.mp3";
            App.RollTheClip(clip);
        }

        private void btnAlreadyHave_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I already have insurance rebuttal.mp3";
            App.RollTheClip(clip);
        }

        private void btnNotInterested_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\nothing to be interested in.mp3";
            App.RollTheClip(clip);
        }

        private void btnEmailRebuttal_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Email Rebuttal.mp3";
            App.RollTheClip(clip);
        }

        private void btnNoEmail_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\I would just need an email address that you have access to.mp3";
            App.RollTheClip(clip);
        }

        private void btnThisWillBeQuick_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\This Will be Real quick.mp3";
            App.RollTheClip(clip);
        }

        private void btnAlmsotDone_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\we're almost done.mp3";
            App.RollTheClip(clip);
        }

        private void btnSpouseHandles_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\My spouse takes care of that.mp3";
            App.RollTheClip(clip);
        }

        private void btnIsThisSpouse_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Is This the Spouse.mp3";
            App.RollTheClip(clip);
        }

        private void btnPOBox_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\P.O Box rebuttal.mp3";
            App.RollTheClip(clip);
        }

        private void btnAddressRebuttal_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Address Rebuttal.mp3";
            App.RollTheClip(clip);
        }

        private void btnMakeExamples_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PUSHONS\chevyfordgmc.mp3";
            App.RollTheClip(clip);
        }

        private void btnMonthExamples_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\January feb march april.mp3";
            App.RollTheClip(clip);
        }

        private void btnProviderExamples_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PUSHONS\allstategeicostatefarm.mp3";
            App.RollTheClip(clip);
        }

        private void btnVerifyMonth_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"c:\soundboard\cheryl\REBUTTALS\CAN YOU JUST VERIFY THE MONTH.mp3";
            App.RollTheClip(clip);
        }

        private void btnVerifyDay_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"c:\soundboard\cheryl\REBUTTALS\CAN YOU JUST VERIFY THE DAY.mp3";
            App.RollTheClip(clip);
        }

        private void btnVerifyYear_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"c:\soundboard\cheryl\REBUTTALS\CAN YOU JUST VERIFY THE YEAR.mp3";
            App.RollTheClip(clip);
        }

        private void btnVerifyMake_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\VEHICLE INFO\Who Makes That Vehicle.mp3";
            App.RollTheClip(clip);
        }

        private void btnVerifyModel_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\VEHICLE INFO\What is the model of the Car 1.mp3";
            App.RollTheClip(clip);
        }

        private void btnMoreSpecific_Click(object sender, RoutedEventArgs e)
        {
            // I'm not entirely sure we have this clip.
            MessageBox.Show("Not entirely sure if this clip exists yet. Sorry.");
        }
        private void btnKillLongDictation_Click(object sender, RoutedEventArgs e)
        {
            App.longDictationClient.EndMicAndRecognition();
            Console.WriteLine("LD ended");
        }

        private void btnKillShortPhrase_Click(object sender, RoutedEventArgs e)
        {
            App.shortPhraseClient.EndMicAndRecognition();
            App.shortPhraseClient.AudioStop();
            Console.WriteLine("SP ended");
        }

        private void btnReaction_Click(object sender, RoutedEventArgs e)
        {
            App.playHumanism();
        }

        // below is my hotkey handling code
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
        public static extern bool UnregisterHotKey(
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
            
            
            Application.Current.Shutdown();
            base.OnClosed(e);
        }
        private void RegisterHotkeys()
        {
            var helper = new WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, 0, 0, 0x70); // F1
            RegisterHotKey(helper.Handle, 1, 0, 0x71); // F2
            RegisterHotKey(helper.Handle, 2, 0, 0x1B); // Escape to pause
        }
        private void UnregisterHotkeys()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, 0);
            UnregisterHotKey(helper.Handle, 1);
            UnregisterHotKey(helper.Handle, 2);
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
                        case 2:
                            App.StopTheClip();
                            break;
                    }
                    break;
            }


            return IntPtr.Zero;
        }

        private void btnInitSpeechReco_Click(object sender, RoutedEventArgs e)
        {
            App.setupMicRecogClient();
        }

        private void btnStartSpeechRecoShort_Click(object sender, RoutedEventArgs e)
        {
            App.testSpeechReco(0);
        }

        private void btnStartSpeechRecoLong_Click(object sender, RoutedEventArgs e)
        {
            App.testSpeechReco(1);
        }

        private void frmReactions_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
    
}
