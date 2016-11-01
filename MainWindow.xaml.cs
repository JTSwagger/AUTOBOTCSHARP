using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using Microsoft.ProjectOxford.SpeechRecognition;
using System.IO;
using System.ComponentModel;

namespace AutoBotCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {  
        public ChromeDriver testDriver;
        private Random randy;
        public Agent user;
        public string version;
  
        public Speech_Recognizer reco;

        public void FormClose(object sender, EventArgs e)
        {
            foreach (Process proc in Process.GetProcessesByName("chromedriver"))
            {
                proc.Kill();
            }
            foreach (Process proc in Process.GetProcessesByName("python"))
            {
                proc.Kill();
            }
            Application.Current.Shutdown();

        }
        public MainWindow()
        {
            this.Closing += FormClose;
            reco = new Speech_Recognizer(6000, Speech_Recognizer.Google);
            Agent user = new AutoBotCSharp.Agent();
            string apiKey1 = "";
            string apiKey2 = "";
            App.getWindow().Closed += closeall;
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\keys.txt";
                string[] keys = File.ReadAllLines(path);
                apiKey1 = keys[0];
                apiKey2 = keys[1];
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("keys.txt not found", "ya dun goofed");
            }
            

            randy = new Random();
            InitializeComponent();
       

            string procName = Process.GetCurrentProcess().ProcessName;
            foreach (Process proc in Process.GetProcessesByName("chromedriver"))
            {
                proc.Kill();
            }
            foreach (Process proc in Process.GetProcessesByName("AutoBotCSharp"))
            {
                if (proc.Id == Process.GetCurrentProcess().Id)
                {
                    Console.WriteLine("cannot self terminate");
                }
                else
                {
                    proc.Kill();
                }
            }
           
            
            
            frmMain.Title = "AutoBotC# Ver: " + user.version;
        }

           

        public void setNameText(string name)
        {
            btnTheirName.Content = name;
        }

        public int getTabControlTopIndex()
        {
            return tabControlTop.TabIndex;
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
            Agent.Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello.mp3");

        }

        private void btnIntro_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello.mp3");

            App.getAgent().setAutoFormQuestion(Agent.INTRO);
            string clip = @"C:\Soundboard\Cheryl\INTRO\Intro2.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            string AgentNum = txtAgentNum.Text;
            if (user == null)
            {
                user = new Agent();
            }
            user.Login(AgentNum);

        }
        private void btnTheirName_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(App.findNameClips(btnTheirName.Content.ToString())[2]);
        }
        // Insurance info button group
        private void btnInsuranceProvider_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello.mp3");
            user.setAutoFormQuestion(Agent.PROVIDER);
            user.setCallPos(Agent.INBETWEEN);
            string clip = @"C:\SoundBoard\Cheryl\INSURANCE INFO\Ins provider 1.mp3";
            Agent.Conversation.RollTheClip(clip);

        }
        private void btnPolicyExpiration_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\INTRO\hello.mp3");
            user.setAutoFormQuestion(Agent.INS_EXP);
            user.setCallPos(Agent.INBETWEEN);
            string clip = @"C:\SoundBoard\Cheryl\INSURANCE INFO\EXPIRATION.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnPolicyStart_Click(object sender, RoutedEventArgs e)
        {

            string clip = @"C:\SoundBoard\Cheryl\INSURANCE INFO\Years with 1.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        // Vehicle info button group
        private void btnHowManyVehicles_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\How many vehicles do you have.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnYmm1_Click(object sender, RoutedEventArgs e)
        {

            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\First Vehicle.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnYMM2_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion(Agent.YMM2);
            user.setCallPos(Agent.INBETWEEN);
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\2nd Vehicle.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnYMM3_Click(object sender, RoutedEventArgs e)
        {

            user.setAutoFormQuestion("YMM3");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\Third Vehicle.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnYMM4_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("YMM4");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\Fourth Vehicle.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        // Driver info button group
        private void btnDOB_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("DOB");
            //user.Callpos = "INBETWEEN";
            App.playDobClips();

        }
        private void btnMaritalStatus_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("MARITAL_STATUS");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\Marital Status.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnSpouseName_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("SPOUSE_NAME");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses First name.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnSpouseDOB_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("SPOUSE_DOB");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\DRIVER INFO\Spouses Date of Birth.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        // Personal info button group
        private void cmboOwnRent_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("OWN_RENT");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Do You Own Or Rent the Home.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnHomeType_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("HOME_TYPE");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\HOMETYPE.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnAddress_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("ADDRESS");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Could you please verify your address.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("EMAIL");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\EMAIL.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnCredit_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("CREDIT");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Credit.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnPhoneType_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("PHONE_TYPE");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\PHONETYPE.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnLastName_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("LAST_NAME");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\Last Name.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnTCPA_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("TCPA");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\WRAPUP\TCPA.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        // Reactions button group
        private void btnWhatIGot_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\VeryGood.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            App.playOkClip();
        }
        private void butnThisIsCheryl_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\INTRO\CHERYLCALLING.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Yes.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnRepeatThat_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Can you repeat that.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnLookingFor_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(App.findNameClips(btnTheirName.Content.ToString())[1]);
        }
        private void btnGreatQ_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\That's a great AutoFormQuestion.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnThatsOk_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\That's okay.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnLaugh_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\REACTIONS\Loud-laugh.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\No.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnSpellThat_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\TIE INS\Could You Please Spell That Out.mp3";
            Agent.Conversation.RollTheClip(clip);
        }
        private void btnHi_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(App.findNameClips(btnTheirName.Content.ToString())[0]);
        }

        private void btnCompUnd_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I completely understand.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnHelloQues_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Hello 4.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnTY_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\Thank you.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnYW_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REACTIONS\You're-welcome.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnSorry_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\sorry.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnSadSorry_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Sorry to Hear That 2.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnWhat_sLCN_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\What's LCN.mp3";

            Agent.Conversation.RollTheClip(clip);
        }

        private void btnWhatsThisRegarding_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\INTRO\THISISTOGIVENEWQUOTE.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnWheredYouGetInfo_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\where did you get my info.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnDontYouHaveThis_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I'm required to have you verify it first.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnWillIGetManyCalls_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Will I get many calls.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnCantTheyEmail_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Can They Email.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnCanYouQuote_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\You're not giving me a quote.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnWhenWillTheyCall_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\When will they call.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnWhyDoYouNeed_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Why do you need my info.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnNotGivingThatOut_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Let me Just confirm a few things.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btHappyWithInsurance_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\HappyWithInsurance.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnAlreadyHave_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\I already have insurance rebuttal.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnNotInterested_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\nothing to be interested in.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnEmailRebuttal_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Email Rebuttal.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnNoEmail_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PERSONAL INFO\I would just need an email address that you have access to.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnThisWillBeQuick_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\This Will be Real quick.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnAlmsotDone_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\we're almost done.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnSpouseHandles_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\My spouse takes care of that.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnIsThisSpouse_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Is This the Spouse.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnPOBox_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\P.O Box rebuttal.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnAddressRebuttal_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\Address Rebuttal.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnMakeExamples_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PUSHONS\chevyfordgmc.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnMonthExamples_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\REBUTTALS\January feb march april.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnProviderExamples_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\PUSHONS\allstategeicostatefarm.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnVerifyMonth_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"c:\soundboard\cheryl\REBUTTALS\CAN YOU JUST VERIFY THE MONTH.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnVerifyDay_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"c:\soundboard\cheryl\REBUTTALS\CAN YOU JUST VERIFY THE DAY.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnVerifyYear_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"c:\soundboard\cheryl\REBUTTALS\CAN YOU JUST VERIFY THE YEAR.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnVerifyMake_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\VEHICLE INFO\Who Makes That Vehicle.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnVerifyModel_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\Soundboard\Cheryl\VEHICLE INFO\What is the model of the Car 1.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnMoreSpecific_Click(object sender, RoutedEventArgs e)
        {
            // I'm not entirely sure we have this clip.
            MessageBox.Show("Not entirely sure if this clip exists yet. Sorry.");
        }
        private void btnKillLongDictation_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("LD ended");
        }
        private void btnReaction_Click(object sender, RoutedEventArgs e)
        {

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

            RegisterHotkeys();
        }
        protected override void OnClosed(EventArgs e)
        {

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

   

        private void frmReactions_Navigated(object sender, NavigationEventArgs e)
        {

        }
        private void btnDispo_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Hanging up and dispoing as: " + cmbDispo.Text);
            user.HangupAndDisposition(cmbDispo.Text);
        }
        private void cmbDispo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            switch (user.getDialerStatus())
            {
                case "PAUSED":
                    user.setPause("UNPAUSE");
                    break;
                case "READY":
                    user.setPause("PAUSE");
                    break;
            }
        }

        private void btnYMMOnly1_Click(object sender, RoutedEventArgs e)
        {
            user.setAutoFormQuestion("YMM ONLY ONE");
            user.setCallPos("INBETWEEN");
            string clip = @"C:\SoundBoard\Cheryl\VEHICLE INFO\YMMYV.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        public void setNameBtns(bool torf)
        {
            if (torf)
            {
                btnTheirName.IsEnabled = true;
                btnLookingFor.IsEnabled = true;
                btnHi.IsEnabled = true;
            }
            else if (!torf)
            {
                btnTheirName.IsEnabled = false;
                btnLookingFor.IsEnabled = false;
                btnHi.IsEnabled = false;
            }
        }

        private void btnGreatDay_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\Have a great day.mp3");
        }

        private void speechTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void btnOpenTestPage_Click(object sender, RoutedEventArgs e)
        {
            user = new Agent();
            user.setAgentNum("1198");
            try
            {
                user.openTestPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MALENAMES.txt";
        }
        private void btnBestGuess_Click(object sender, RoutedEventArgs e)
        {
            string clip = @"C:\SoundBoard\Cheryl\TIE INS\Great What's Your Best Guess.mp3";
            Agent.Conversation.RollTheClip(clip);
        }

        private void btnTurnOnAutoStuff_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnInitSpeechReco_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnLeadCheck_click(object sender, RoutedEventArgs e)
        {


        }

        private void tabControlBottom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void frmReactions_Navigated_1(object sender, NavigationEventArgs e)
        {

        }
        protected void closeall(Object sender, EventArgs args)
        {

            Environment.Exit(Environment.ExitCode);
        }
        private void doTestingStuff_Click(object sender, RoutedEventArgs e)
        {
            user.setTesting(true);
            user.setupTesting();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Agent.Conversation.RollTheClip(@"C:\SoundBoard\Cheryl\WRAPUP\bad connection.mp3");

        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {       
            reco.TurnOnMic(Speech_Recognizer.Google);
        }

        public void onMicChange(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Speech_Recognizer r = (Speech_Recognizer)sender;
                bool status = r.is_recording;
                lblreco.Content = "RECORDING: " + status;
            });
        }

        //FINAL SPEECH RESULTS GENERATED AS AN EVENT TRIGGERED BY THE SPEECH_RECOGNITION CLASS
        public void onGoogleFinalSpeech(object sender, EventArgs e)
        {
            string FResult = reco.Final_Speech.ToLower().Trim();   //Sets local FinalResult

            if (user.Talking()
                    && !user.Rebuttaling())
            {
                if (FResult != "")
                {
                    Dispatcher.Invoke((() =>
                     {
                         App.getWindow().appendSpeechBoxText("Full: " + FResult);
                         if (FResult.Contains("incoming")) { System.Threading.Thread.Sleep(500); };
                     }));
                    Dispatcher.Invoke(async () =>
                    {
                        if (user.Rebuttaling())
                        {

                            Console.WriteLine("Did not object....");
                            if (user.Talking())
                            {
                                Console.WriteLine("Cheryl isn't talking...");
                                user.AdjustConvoPosition(FResult);
                                if (!user.Asked())
                                {
                                    user.Asked(true);

                                    bool ba = await Agent.Conversation.PlayHumanism();
                                }
                            }
                        }
                    });
                }
            }

            }

        //PARTIAL SPEECH RESULTS GENERATED AS AN EVENT TRIGGERED BY THE SPEECH_RECOGNITION CLASS
        public void onGooglePartialSpeech(object sender, EventArgs e)
        {          
            string response = reco.partial_speech.ToLower().Trim();
            string raw = response;
            if (response != "")
            {
                Console.WriteLine("Partial Results Generated--->" + response);
                //  timeout = 0;
                user.resetSilenceTimer();
                    Dispatcher.Invoke((async () =>
                    {
                        App.getWindow().setSpeechBoxText("Partial: " + response);
                        if (!user.Talking())
                        {
                            if (!((await Agent.checkForObjection(response))))
                            {

                                user.checkforData(response);
                                user.Asked(false);
                            }
                            else
                            {
                                user.Rebuttaling(true);
                            }
                        }

                    }));               
            }
        }   

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            reco.TurnOffMic();
        }
    }
    
}
