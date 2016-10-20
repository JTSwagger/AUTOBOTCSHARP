using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoBotCSharp
{
    class MrSocketGetter
    {
        public static Socket sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
        public async static Task GetSpeechData()
        {
            while (true)
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
                    Console.WriteLine("FULL: " + recv);
                    Console.WriteLine("SPEECH: " + Speech);
                    try
                    {
                        await Application.Current.Dispatcher.Invoke(async () => {
                            App.getWindow().setSpeechBoxText("Partial: " + Speech);
                            if (!await Agent.checkForObjection(Speech))
                            {
                                if (await Agent.checkforData(Speech))
                                {
                                    App.getAgent().hasAsked = true;
                                    App.doBackgroundQuestionSwitchingStuff(Speech);
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
        public static void killEverything()
        {
            sock.Send(Encoding.ASCII.GetBytes("I am He-Man, destroyer of Skeletor"));
        }
    }
}
