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
    public class MrDriver
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
        public bool enterData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            int unhideCount = 0;

            while (retry)
            {
                try
                {
                    driver.FindElementById(elementId).SendKeys(data);
                    return true;
                }
                catch (ElementNotVisibleException)
                {
                    unhideElement(elementId);
                    Console.WriteLine("Element has been unhidden, retrying...");
                    if (unhideCount == 1)
                    {
                        Console.WriteLine("couldn't unhide, ending...");
                        retry = false;
                        return false;
                    }
                    unhideCount += 1;
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine(elementId + " does not exist on the current form. Try a different ID?");
                    retry = false;
                }
                catch (StaleElementReferenceException)
                {
                    if (staleRefCount == 2)
                    {
                        Console.WriteLine("Two stale references, ending");
                        retry = false;
                    }
                    Thread.Sleep(1000);
                    staleRefCount += 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Generic Exception");
                    Console.WriteLine("Inner exception: " + ex.InnerException);
                    Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
            return false;
        }
        public bool selectData(string elementId, string data)
        {
            bool retry = true;
            int staleRefCount = 0;
            int unhideCount = 0;

            while (retry)
            {
                try
                {
                    var select = new SelectElement(driver.FindElementById(elementId));
                    select.SelectByText(data);
                    return true;
                }
                catch (ElementNotVisibleException)
                {
                    unhideElement(elementId);
                    Console.WriteLine("Element has been unhidden, retrying...");
                    if (unhideCount == 1)
                    {
                        Console.WriteLine("couldn't unhide, ending...");
                        retry = false;
                        return false;
                    }
                    unhideCount += 1;
                }
                catch (NoSuchElementException ex)
                {
                    string message = ex.Message;
                    if (message.Contains(data))
                    {
                        Console.WriteLine(data + " is not a valid option for " + elementId + "; try a different option.");
                    }
                    else
                    {
                        Console.WriteLine(elementId + " does not exist on the current form. Try a different ID?");
                    }
                    retry = false;
                }
                catch (StaleElementReferenceException)
                {
                    if (staleRefCount == 2)
                    {
                        Console.WriteLine("Two stale references, ending");
                        retry = false;
                    }
                    Thread.Sleep(1000);
                    staleRefCount += 1;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Generic Exception");
                    //Console.WriteLine("Inner exception: " + ex.InnerException);
                    //Console.WriteLine("Message: " + ex.Message);
                    retry = false;
                }
            }
            return false;
        }
        public bool unhideElement(string elementId)
        {
            try
            {
                driver.ExecuteScript("$('" + elementId + "').removeClass('hide')");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
