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
using System.Windows;

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
        public int getFormAge()
        {

            int Month; int Day; int Year;
            int.TryParse(driver.FindElementById("frmDOB_Month").GetAttribute("value"), out Month);
            int.TryParse(driver.FindElementById("frmDOB_Day").GetAttribute("value"), out Day);
            int.TryParse(driver.FindElementById("frmDOB_Year").GetAttribute("value"), out Year);
            Console.WriteLine(Month.ToString() + Day.ToString() + Year.ToString());
            DateTime now = DateTime.Now;
            DateTime birthDate = new DateTime(Year, Month, Day);
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) { age--; }
            Console.WriteLine(age);
            return age;
        }
        public async void Setup()
        {
            myAgent.customer = new Customer();
            myAgent.callPos = AgentStrings.STARTYMCSTARTFACE;
            while (driver.WindowHandles.Count < 2)
            {
                Console.WriteLine("Shoop Da Whoop!");
            }
            try
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
                //myAgent.customer.firstName = driver.FindElementByName("frmFirstName").GetAttribute("value");
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't switch windows, reason: {0}", e);
            }
            checkPageSource(driver.PageSource);
            string[] clips = App.findNameClips(myAgent.customer.firstName);
            Application.Current.Dispatcher.Invoke((() =>
            {
                App.getWindow().setNameText(myAgent.customer.firstName);
            }));
            if (App.findNameClips(myAgent.customer.firstName)[0] == "no clip")
            {
                Application.Current.Dispatcher.Invoke((() =>
                {
                    App.getWindow().setNameBtns(false);
                    myAgent.customer.isNameEnabled = false;
                }));
            }
            else
            {
                Application.Current.Dispatcher.Invoke((() =>
                {
                    App.getWindow().setNameBtns(true);
                    myAgent.customer.isNameEnabled = true;
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
            }
        }
        public void checkPageSource(string pageSource)
        {
            pageSource = pageSource.ToLower();
            if (pageSource.Contains("cannot be found"))
            {
                myAgent.hangupDispositionCall("Not Available");
            }
            else if (pageSource.Contains("respectfully end"))
            {
                myAgent.hangupDispositionCall("Not Interested");
            }
            return;
        }
    }
}
