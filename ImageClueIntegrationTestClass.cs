using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ImageClueIntegrationTest
{
    [TestClass]
    public class ImageClueIntegrationTestClass
    {
        [TestMethod]
        public void TestBasicFlowWithFourPeople()
        {
            IWebDriver driver = null;
            try
            {
                // Setup and load page
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("--no-sandbox"); 
                options.AddArguments("--headless"); 
                options.AddArguments("--disable-gpu");
                driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl("http://52.6.180.102/");
                Assert.AreEqual("Generate Teams and Clues!", driver.Title);

                // Enter the player names and submit (annd wait)
                SendKeysById(driver, "txt_inPlayers", "Paul\nChris\nEmily\nJoe");
                ClickButtonById(driver, "btn_players");
                Thread.Sleep(2000);

                // Check the hidden teams text is correct
                Assert.IsFalse(GetVisibilityById(driver, "outTeams"));
                Assert.AreEqual("[[\"Paul\",\"Emily\"],[\"Chris\",\"Joe\"]]", GetTextById(driver, "outTeams"));

                // Check the displayed teams text is also correct
                Assert.IsTrue(GetVisibilityById(driver, "outTeamsUserFriendly"));
                Assert.AreEqual("Team1has2membersPaulEmilyTeam2has2membersChrisJoe",
                    RemoveWhitespaceAndNewlines(GetElementById(driver, "outTeamsUserFriendly").Text));

                // Submit the request to get the clues
                ClickButtonById(driver, "btn_Clues");
                Thread.Sleep(2000);

                // Check the clues have been generated and look sensible!
                string clues = GetElementById(driver, "outClues").Text;
                Assert.IsTrue(clues.Contains("Team 1: please draw a"));
                Assert.IsTrue(clues.Contains("Team 2: please draw a"));
            }
            catch (DriverServiceNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Suggest ensuring Chrome Driver is added to PATH, and VS is restarted.");
                throw e;
            }
            catch (NotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Suggest debugging the contents of the page/test");
                driver.Quit();
                throw e;
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
        }

        [TestMethod]
        public void TestBasicFlowWithNinePeople()
        {
            IWebDriver driver = null;
            try
            {
                // Setup and load page
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("--no-sandbox");
                options.AddArguments("--headless");
                options.AddArguments("--disable-gpu");
                driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl("http://52.6.180.102/");
                Assert.AreEqual("Generate Teams and Clues!", driver.Title);

                // Enter the player names and submit (annd wait)
                SendKeysById(driver, "txt_inPlayers", "Paul\nChris\nEmily\nJoe\nHicksy\nBen\nAdam\nJosh\nWinnie");
                ClickButtonById(driver, "btn_players");
                Thread.Sleep(2000);

                // Check the hidden teams text is correct
                Assert.IsFalse(GetVisibilityById(driver, "outTeams"));
                Assert.AreEqual("[[\"Paul\",\"Hicksy\",\"Winnie\"],[\"Chris\",\"Ben\"],[\"Emily\",\"Adam\"],[\"Joe\",\"Josh\"]]", 
                    GetTextById(driver, "outTeams"));

                // Check the displayed teams text is also correct
                Assert.IsTrue(GetVisibilityById(driver, "outTeamsUserFriendly"));
                Assert.AreEqual("Team1has3membersPaulHicksyWinnieTeam2has2membersChrisBenTeam3has2membersEmilyAdamTeam4has2membersJoeJosh",
                    RemoveWhitespaceAndNewlines(GetElementById(driver, "outTeamsUserFriendly").Text));

                // Submit the request to get the clues
                ClickButtonById(driver, "btn_Clues");
                Thread.Sleep(2000);

                // Check the clues have been generated and look sensible!
                string clues = GetElementById(driver, "outClues").Text;
                Assert.IsTrue(clues.Contains("Team 1: please draw a"));
                Assert.IsTrue(clues.Contains("Team 2: please draw a"));
                Assert.IsTrue(clues.Contains("Team 3: please draw a"));
                Assert.IsTrue(clues.Contains("Team 4: please draw a"));
            }
            catch (DriverServiceNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Suggest ensuring Chrome Driver is added to PATH, and VS is restarted.");
                throw e;
            }
            catch (NotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Suggest debugging the contents of the page/test");
                driver.Quit();
                throw e;
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                }
            }
        }

        private void SendKeysById(IWebDriver driver, string elementId, string text)
        {
            GetElementById(driver, elementId).SendKeys(text);
        }

        private void ClickButtonById(IWebDriver driver, string elementId)
        {
            GetElementById(driver, elementId).Click();
        }

        private string GetTextById(IWebDriver driver, string elementId)
        {
            return GetElementById(driver, elementId).GetAttribute("innerText");
        }

        private bool GetVisibilityById(IWebDriver driver, string elementId)
        {
            return GetElementById(driver, elementId).Displayed;
        }

        private IWebElement GetElementById(IWebDriver driver, string elementId)
        {
            return driver.FindElement(By.Id(elementId));
        }

        private IWebElement GetElementByUniqueName(IWebDriver driver, string elementName)
        {
            return driver.FindElement(By.Name(elementName));
        }

        private List<IWebElement> GetElementsByName(IWebDriver driver, string elementName)
        {
            return driver.FindElements(By.Name(elementName)).ToList();
        }

        private string RemoveWhitespaceAndNewlines(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c) && ! Char.IsControl(c))
                .ToArray());
        }
    }
}
