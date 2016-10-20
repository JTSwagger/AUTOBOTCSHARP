using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Media;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace AutoBotCSharp
{
    public class Agent_Google
    {
        // instance variables
        // about as variable as my arrival time to work in the mornings
        // :)
        public Customer customer;
        public bool loggedIn;
        public bool inCall;
        public string agentNumber;
        public string agentName;
        public string dialerStatus = "";
        public string callPos;
        public string[] stats;
        public bool isNewCall = false;
        private MrDriver mrDriver;

        public Agent_Google(string socketIp)
        {
            MrSocketGetter.sock.Connect(socketIp, 7979);
            customer = new Customer { numVehicles = 0, maritalStatus = "Single", speech = "", LeadID = "", lastName = "" };
            loggedIn = false;
            inCall = false;
            stats = new string[10];
            agentName = "LCNMoo";
            mrDriver = new MrDriver(int.Parse(agentNumber), this);
        }
        public static async Task doAgentStatusRequest()
        {
            Agent_Google agent = App.getGoogleAgent();
            try
            {
                while (agent.loggedIn)
                {
                    Thread.Sleep(200);
                    agent.startWebRequest();
                    try
                    {
                        agent.dialerStatus = agent.stats[0];
                        agent.agentName = agent.stats[5];

                        if (agent.stats.Contains<string>("DEAD"))
                        {
                            // hang up and disposition the call
                            agent.inCall = false;
                        }
                        if (agent.dialerStatus == "READY")
                        {
                            agent.isNewCall = true;
                        }
                        else if (agent.dialerStatus == "INCALL")
                        {
                            if (agent.isNewCall)
                            {
                                agent.inCall = true;
                                agent.callPos = AgentStrings.STARTYMCSTARTFACE;
                                agent.isNewCall = false;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Agent is not logged in");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("caught exception: {0}", e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("caught exception: {0}", e);
            }
        }
        private void startWebRequest()
        {
            WebRequest webRequest = WebRequest.Create("http://loudcloud9.ytel.com/x5/api/non_agent.php?source=test&user=101&pass=API101IEpost&function=agent_status&agent_user=" + agentNumber + "&stage=csv&header=NO");
            WebResponse response = webRequest.GetResponse();
            stats = (new StreamReader(response.GetResponseStream())).ReadToEnd().Split(',');
        }
        private void setGlobals()
        {
            switch (dialerStatus)
            {
                case "READY":
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().Background = Brushes.LightGoldenrodYellow;
                    }));
                    break;
                case "PAUSED":
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().Background = Brushes.IndianRed;
                    }));
                    break;
                case "INCALL":
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        App.getWindow().Background = Brushes.ForestGreen;
                    }));
                    break;
            }
        }
        public void hangupDispositionCall(string disposition)
        {
            inCall = false;
            try
            {
                App.StopTheClip();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception when trying to stop currently playing clip, {0}", e);
            }
            string hangupApiCall = "http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_hangup&value=1";
            WebResponse response = (WebRequest.Create(hangupApiCall)).GetResponse();
            Thread.Sleep(300);
            response.Close();
            switch (disposition)
            {
                case "hangup":
                    if (/* !endCallBooleanHere*/ "42" != "the answer to life the universe and everything")
                    {

                    }
                    break;
                case "Not Available":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "NotAvl")).GetResponse();
                    break;
                case "Not Interested":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "NI")).GetResponse();
                    break;
                case "No Insurance":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "NITU")).GetResponse();
                    break;
                case "Do Not Call":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "DNC")).GetResponse();
                    break;
                case "Wrong Number":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "Wrong")).GetResponse();
                    break;
                case "No Car":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "NoCar")).GetResponse();
                    break;
                case "No English":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "NoEng")).GetResponse();
                    break;
                case "Auto Lead":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "1Auto")).GetResponse();
                    break;
                case "LOW":
                    response = (WebRequest.Create("http://loudcloud9.ytel.com/x5/api/agent.php?source=test&user=101&pass=API101IEpost&agent_user=" + agentNumber + "&function=external_status&value=" + "LOW")).GetResponse();
                    break;
            }
            response.Close();
        }
    }
    public class Customer
    {
        public string IMPORT_ID { get; set; }
        public string LEADGUID { get; set; }
        public string LeadID { get; set; }
        public string speech { get; set; }
        public string phone { get; set; }
        public int numVehicles { get; set; }
        public string maritalStatus { get; set; }
        public string firstName { get; set; }
        public bool isNameEnabled { get; set; }
        public string expMonth { get; set; }
        public string expYear { get; set; }
        public string lastName { get; set; }
    }
}
