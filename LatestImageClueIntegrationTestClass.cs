using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageClueIntegrationTest
{
    [TestClass]
    public class LatestImageClueIntegrationTestClass
    {
        [TestMethod]
        public void LatestTestBasicFlowWithFourPeople()
        {
            IWebDriver driver = null;
            try
            {
                // Setup and load page
                ChromeOptions options = new ChromeOptions();
                //options.AddArguments("--no-sandbox"); 
                //options.AddArguments("--headless"); 
                //options.AddArguments("--disable-gpu");
                driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl("https://34.193.188.174/?env=uat");

                // Need to navigate through the certificate stuff first
                Assert.AreEqual("Privacy error", driver.Title);
                ClickButtonById(driver, "details-button");
                ClickButtonById(driver, "proceed-link");
                Thread.Sleep(1500);
                // Now back to normal
                Assert.AreEqual("Generate Teams and Clues!", driver.Title);

                // Enter the player names and submit (and wait)
                EnterPlayerNameAndPhoneIntoTable(driver, 4, "phones_good.txt");
                ClickButtonById(driver, "btn_players");
                Thread.Sleep(4000);

                // Check the hidden teams text is correct
                Assert.IsFalse(GetVisibilityById(driver, "outTeams"));
                // Just want to check the result is an integer
                int.Parse(GetTextById(driver, "outTeams"));

                // Check the displayed teams text is also correct
                Assert.IsTrue(GetVisibilityById(driver, "outTeamsUserFriendly"));
                Assert.AreEqual("Team1has2membersPaulEmilyTeam2has2membersChrisJoe",
                    RemoveWhitespaceAndNewlines(GetElementById(driver, "outTeamsUserFriendly").Text));

                // Submit the request to get the clues
                ClickButtonById(driver, "btn_Clues");
                Thread.Sleep(4000);

                // Check the hidden clues text is correct
                Assert.IsFalse(GetVisibilityById(driver, "outCluesHidden"));
                int.Parse(GetTextById(driver, "outCluesHidden"));

                // Check the clues have been generated and look sensible!
                string clues = GetElementById(driver, "outClues").Text;
                Assert.IsTrue(clues.Contains("Team 1: please draw a"));
                Assert.IsTrue(clues.Contains("Team 2: please draw a"));

                // Now test that the SMS send works successfully
                ClickButtonById(driver, "btn_sendSMS");
                Thread.Sleep(8000);
                string smsOutcome = GetTextById(driver, "outSendSMSStatus");
                Assert.AreEqual("Successfully sent SMS messages!", smsOutcome);
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
        public void LatestTestBasicFlowWithNinePeople()
        {
            IWebDriver driver = null;
            try
            {
                // Setup and load page
                ChromeOptions options = new ChromeOptions();
                //options.AddArguments("--no-sandbox");
                //options.AddArguments("--headless");
                //options.AddArguments("--disable-gpu");
                driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl("https://34.193.188.174/?env=uat");

                // Need to navigate through the certificate stuff first
                Assert.AreEqual("Privacy error", driver.Title);
                ClickButtonById(driver, "details-button");
                ClickButtonById(driver, "proceed-link");
                Thread.Sleep(1500);
                // Now back to normal
                Assert.AreEqual("Generate Teams and Clues!", driver.Title);

                // Enter the player names and submit (and wait)
                EnterPlayerNameAndPhoneIntoTable(driver, 9, "phones_bad.txt");
                ClickButtonById(driver, "btn_players");
                Thread.Sleep(2000);

                // Check the hidden teams text is correct
                Assert.IsFalse(GetVisibilityById(driver, "outTeams"));
                // Just want to check the result is an integer
                int.Parse(GetTextById(driver, "outTeams"));

                // Check the displayed teams text is also correct
                Assert.IsTrue(GetVisibilityById(driver, "outTeamsUserFriendly"));
                Assert.AreEqual("Team1has3membersPaulHicksyWinnieTeam2has2membersChrisBenTeam3has2membersEmilyAdamTeam4has2membersJoeJosh",
                    RemoveWhitespaceAndNewlines(GetElementById(driver, "outTeamsUserFriendly").Text));

                // Submit the request to get the clues
                ClickButtonById(driver, "btn_Clues");
                Thread.Sleep(2000);

                // Check the hidden clues text is correct
                Assert.IsFalse(GetVisibilityById(driver, "outCluesHidden"));
                // Just want to check the result is an integer
                int.Parse(GetTextById(driver, "outCluesHidden"));

                // Check the clues have been generated and look sensible!
                string clues = GetElementById(driver, "outClues").Text;
                Assert.IsTrue(clues.Contains("Team 1: please draw a"));
                Assert.IsTrue(clues.Contains("Team 2: please draw a"));
                Assert.IsTrue(clues.Contains("Team 3: please draw a"));
                Assert.IsTrue(clues.Contains("Team 4: please draw a"));

                // Now test that the SMS send fails as expected for bad phone numbers
                ClickButtonById(driver, "btn_sendSMS");
                Thread.Sleep(4000);
                string smsOutcome = GetTextById(driver, "outSendSMSStatus");
                Assert.AreEqual("The 'To' number +441111 is not a valid phone number.", smsOutcome);
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

        private void EnterPlayerNameAndPhoneIntoTable(IWebDriver driver, int numberOfPlayers, string phonesFile)
        {
            // Firstly, need to request extra fields as needed
            // the page contains 3 fields by default, so add as needed
            for (int playerIndex = 3; playerIndex < numberOfPlayers; playerIndex++)
            {
                ClickButtonById(driver, "btn_addRows");
            }
            List<IWebElement> playerNameFields = GetElementsByName(driver, "tbl_editable_name_fields");
            List<IWebElement> playerPhoneFields = GetElementsByName(driver, "tbl_editable_phone_fields");

            List<string> playerNames = GetPlayerNames(playerNameFields.Count);
            List<string> playerPhones = GetPlayerPhones(phonesFile, playerPhoneFields.Count);

            // for each player name field
            for (int fieldIndex = 0; fieldIndex < playerNameFields.Count; fieldIndex++)
            {
                IWebElement currentPlayerNameField = playerNameFields[fieldIndex];
                // delete the default word "player"
                for (int delCount = 0; delCount < 6; delCount++)
                {
                    currentPlayerNameField.SendKeys(Keys.Backspace);
                }
                currentPlayerNameField.SendKeys(playerNames[fieldIndex]);
            }

            // same for phones
            for (int fieldIndex = 0; fieldIndex < playerPhoneFields.Count; fieldIndex++)
            {
                IWebElement currentPlayerPhoneField = playerPhoneFields[fieldIndex];
                // delete the default entry "44"
                for (int delCount = 0; delCount < 2; delCount++)
                {
                    currentPlayerPhoneField.SendKeys(Keys.Backspace);
                }
                currentPlayerPhoneField.SendKeys(playerPhones[fieldIndex]);
            }
        }

        private List<string> GetPlayerNames(int count)
        {
            string[] names = File.ReadAllLines("names.txt");
            return names.ToList().Take(count).ToList();
        }

        private List<string> GetPlayerPhones(string phonesFile, int count)
        {
            string[] names = File.ReadAllLines(phonesFile);
            return names.ToList().Take(count).ToList();
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
