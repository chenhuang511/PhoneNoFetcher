using System;
using System.Net;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PhoneNoFetcher
{
    public class Program
    {
        static string urlPrefix;
        static string input;
        static string output;

        static void Main(string[] args)
        {
            Init();

            Console.WriteLine($"Program started, input={input} , output={output}");
            Console.WriteLine($"Fetch data from url={urlPrefix}");

            var lines = File.ReadLines(input);
            List<string> responses = new List<string>();
            int itemNo = 0;
            foreach (var line in lines)
            {
                if(!string.IsNullOrWhiteSpace(line))
                {
                    itemNo++;
                    Console.WriteLine($"Process item number {itemNo}");
                    string url = urlPrefix + line;
                    string response = GetPhone(url);
                    PhoneData obj = JsonConvert.DeserializeObject<PhoneData>(response);
                    string result = "";
                    if (obj.phone == null || obj.uid == null)
                    {
                        result = $"{line} {response}";
                    }
                    else
                    {
                        result = $"{obj.uid} {obj.phone}";
                    }
                    responses.Add(result);
                }
            }
            Console.WriteLine($"Write all data to file, total={itemNo} items");
            File.WriteAllLines(output, responses.ToArray());
            Console.WriteLine($"Done, output={output}");
            Console.ReadLine();
        }

        static void Init()
        {
            input = ConfigurationManager.AppSettings["Input"];
            output = ConfigurationManager.AppSettings["Output"];
            urlPrefix = ConfigurationManager.AppSettings["UrlPrefix"];
        }

        static string GetPhone(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseData = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseData = sr.ReadToEnd();
            }
            return responseData;
        }
    }

    class PhoneData
    {
        public string uid { set; get; }
        public string phone { set; get; }
    }
}
