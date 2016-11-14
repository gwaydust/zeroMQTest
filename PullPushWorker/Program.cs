namespace PullPushWorker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using CommandLine;
    using ZeroMQ;
    using Newtonsoft.Json;	
    using System.Threading;
    using System.Xml;

    class Program
    {    	
        static void Main(string[] args)
        {        	
            try
            {            	
                var options = new Options();                
                var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
                if (!parser.ParseArguments(args, options))
                    Environment.Exit(1);
                
                Logger.logFile(ConfigurationManager.AppSettings["LogDir"] + "\\" + options.workerId + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                using(var ctx = ZmqContext.Create())
                {
                	using (ZmqSocket receiver = ctx.CreateSocket(SocketType.PULL))
                    {
                        receiver.Connect(options.pullEndPoint);     

                        while (true)
                        {
                            var rcvdMsg = receiver.Receive(Encoding.UTF8);
                            Console.WriteLine("Got msg: " + rcvdMsg);
                            Dictionary<string, string> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(rcvdMsg);                            
                            Console.WriteLine("msg type: " + jsonDict["MessageType"]);
                            string filePath = ".\\";                                                        
                                                        
                            if (jsonDict["MessageType"].ToUpper() == "BIRTHDAY") {
								jsonDict["MessageText"] = jsonDict["MessageText"].ToUpper();
                            	string jsonStr = JsonConvert.SerializeObject(jsonDict, Newtonsoft.Json.Formatting.Indented);
                            	filePath = ConfigurationManager.AppSettings["BirthDir"] + "\\" + jsonDict["MessageType"] + "_" + jsonDict["MessageId"] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
                            	File.WriteAllText(filePath, jsonStr);
                            } else if (jsonDict["MessageType"].ToUpper() == "CONGRATS") {
                            	XmlDocument xDoc = JsonConvert.DeserializeXmlNode(rcvdMsg, "BabyBirth");
                            	var bytes = Encoding.UTF8.GetBytes(xDoc["BabyBirth"]["Name"].InnerText);
								var base64 = Convert.ToBase64String(bytes);								
								xDoc["BabyBirth"]["Name"].InnerText = base64;
								var bbd = DateTime.ParseExact(xDoc["BabyBirth"]["BabyBirthDate"].InnerText, "yyyyMMdd",System.Globalization.CultureInfo.InvariantCulture);
								xDoc["BabyBirth"]["BabyBirthDate"].InnerText = bbd.ToString("dd MMM yyyy");
		                        filePath = ConfigurationManager.AppSettings["BabyDir"] + "\\" + jsonDict["MessageType"] + "_" + jsonDict["MessageId"] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
		                        xDoc.Save(filePath);
                            } else {
                            	Console.WriteLine("Unsupported message type: " + jsonDict["MessageType"]);
                            	Logger.Error("Unsupported message type: " + jsonDict["MessageType"],"Program");
                            }
                                                                                    
                            Logger.Info("Message ID: " + jsonDict["MessageId"] + ", Message Type: " + jsonDict["MessageType"] + 
                                        ", Message Path:" +  filePath, "Program");
                            Console.WriteLine("Message ID: " + jsonDict["MessageId"] + ", Message Type: " + jsonDict["MessageType"] + " processed");
							Thread.Sleep(options.delay);                            
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
