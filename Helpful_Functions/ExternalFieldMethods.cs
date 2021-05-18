using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using YoAppWebProxy.Logs;
using YoAppWebProxy.Models;

namespace YoAppWebProxy.Helpful_Functions
{
    public class ExternalFieldMethods
    {
        YoAppResponse yoAppResponse = new YoAppResponse();
        public YoAppResponse ReadAllFileLines(string chosenFile, string fuelCardNumber, string serviceProvider)
        {            
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (FileStream fileStream = new FileStream(chosenFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string line = "";
                    bool first = true;

                    if (String.IsNullOrEmpty((line = sr.ReadLine())))
                    {
                        yoAppResponse.ResponseCode = "00055"; // We no longer have fuel card numbers
                        yoAppResponse.Narrative = "00055";

                        Log.RequestsAndResponses("FCards-Response", serviceProvider, yoAppResponse);

                        return yoAppResponse;
                    }
                    else
                    {                        
                        while (first || (line = sr.ReadLine()) != null)
                        {
                            if (first)
                            {
                                fuelCardNumber = line;
                                first = false;
                            }
                            else
                            {
                                stringBuilder.AppendLine(line);
                            }
                        }
                    }                    
                }

                File.Delete(chosenFile);
                var file = "fuel_cards";
                string testFilePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/" + file + ".txt");

                using (FileStream fileStream = new FileStream(testFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    var lineData = stringBuilder.ToString();
                    sw.WriteLine(lineData);
                    yoAppResponse.Narrative = fuelCardNumber;
                    yoAppResponse.ResponseCode = "00000";                    
                    
                    Log.RequestsAndResponses("FCards-Response", serviceProvider, yoAppResponse);

                    return yoAppResponse;
                }
            }
            catch (Exception e)
            {
                var provider = "CBZ-FuelCards";
                var cardNumber = "";
                var fName = "fuel_cards";
                string filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/" + fName + ".txt");

                Console.WriteLine(e.Message);

                ReadAllFileLines(filePath, cardNumber, provider);
            }
             
            return yoAppResponse;
        }

        public YoAppResponse ReadAllTestFileLines(string chosenFile, string fuelCardNumber, string serviceProvider)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (FileStream fileStream = new FileStream(chosenFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string line = "";
                    bool first = true;

                    if (String.IsNullOrEmpty((line = sr.ReadLine())))
                    {
                        yoAppResponse.ResponseCode = "00055"; // We no longer have fuel card numbers
                        yoAppResponse.Narrative = "00055";

                        Log.RequestsAndResponses("FCards-Response", serviceProvider, yoAppResponse);

                        return yoAppResponse;
                    }
                    else
                    {
                        while (first || (line = sr.ReadLine()) != null)
                        {
                            if (first)
                            {
                                fuelCardNumber = line;
                                first = false;
                            }
                            else
                            {
                                stringBuilder.AppendLine(line);
                            }
                        }
                    }
                }

                File.Delete(chosenFile);
                var testFileName = "test_fuel_cards";
                string testFilePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/" + testFileName + ".txt");

                using (FileStream fileStream = new FileStream(testFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    var lineData = stringBuilder.ToString();
                    sw.WriteLine(lineData);
                    yoAppResponse.Narrative = fuelCardNumber;
                    yoAppResponse.ResponseCode = "00000";

                    Log.RequestsAndResponses("FCards-Response", serviceProvider, yoAppResponse);

                    return yoAppResponse;
                }
            }
            catch (Exception e)
            {
                var sProvider = "CBZ-FuelCards";
                var testFuelCardNumber = "";
                var testFileName = "test_fuel_cards";
                string testFilePath = HttpContext.Current.Server.MapPath("~/App_Data/" + serviceProvider + "/" + testFileName + ".txt");

                Console.WriteLine(e.Message);

                ReadAllFileLines(testFilePath, testFuelCardNumber, sProvider);
            }

            return yoAppResponse;
        }
    }
}