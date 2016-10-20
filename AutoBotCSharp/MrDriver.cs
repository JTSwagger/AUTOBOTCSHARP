using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium;

namespace AutoBotCSharp
{
    class MrDriver
    {
        int agentNumber;
        ChromeDriverService cds;
        ChromeDriver driver;
        Agent_Google myAgent;

        public MrDriver(int agentNum, Agent_Google agent)
        {
            agentNumber = agentNum;
            cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;
            driver = new ChromeDriver(cds);
            myAgent = agent;
        }

        public void Login()
        {
            try
            {
                driver.Navigate().GoToUrl("https://loudcloud9.ytel.com");
                driver.SwitchTo().Frame("top");
                Thread.Sleep(500);
                driver.FindElementById("login-agent").Click();
                Thread.Sleep(250);
                driver.FindElementById("agent-login").SendKeys(agentNumber.ToString());
                Thread.Sleep(500);
                driver.FindElementById("agent-password").SendKeys("y" + agentNumber.ToString() + "IE");
                Thread.Sleep(500);
                driver.FindElementById("btn-get-campaign").Click();
                Thread.Sleep(750);
                driver.FindElementById("select-campaign").Click();
                foreach (IWebElement elem in driver.FindElementById("select-campaign").FindElements(By.TagName("option")))
                {
                    if (elem.Text.Contains("5000") || elem.Text.Contains("BOT"))
                    {
                        elem.Click();
                    }
                }
                Thread.Sleep(250);
                driver.FindElementById("btn-submit").Click();
                myAgent.loggedIn = true;
                Task task = Task.Run(()=>Agent_Google.doAgentStatusRequest());
            } 
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

    }
}
