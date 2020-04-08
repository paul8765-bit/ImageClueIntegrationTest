using System;

namespace ImageClueIntegrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ImageClueIntegrationTestClass testClass = new ImageClueIntegrationTestClass();
                Console.WriteLine("\n----------About to run 2 integration tests-------------\n");
                Console.WriteLine("\n----------First Test: Basic Flow with 4 people -------------\n");
                testClass.TestBasicFlowWithFourPeople();
                Console.WriteLine("\n----------Second Test: Basic Flow with 9 people -------------\n");
                testClass.TestBasicFlowWithNinePeople();
                Console.WriteLine("\n----------All tests successfully completed -------------\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("\n----------Test failed -------------\n");
                Console.WriteLine("\n----------" + e.Message + " -------------\n");
                Console.WriteLine("\n----------" + e.StackTrace + " -------------\n");
                throw e;
            }
        }
    }
}
