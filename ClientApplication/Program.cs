using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Win32;


namespace ClientApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //RegistryKey root = Registry.CurrentUser;
            //RegistryKey systemTypeKey = null;
            //string registryPath = @"Software\Philips Healthcare\CT\PortalClient";
            //systemTypeKey = root.OpenSubKey(registryPath);
            //if (systemTypeKey == null)
            //{
            //    root = Registry.Users;
            //    registryPath = @".Default\" + registryPath;
            //    systemTypeKey = root.OpenSubKey(registryPath);
            //}
            //string portalPath = (string)systemTypeKey.GetValue("pmsRoot", null) + "system";
            //portalPath = portalPath + @"\PACSURLPlugin.exe";
            //System.Diagnostics.Process.Start(portalPath, "");

            string argument = "";
            if (args == null)
            {
                Console.WriteLine("args is null"); // Check for null array
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("args is null", EventLogEntryType.Information, 101, 1);
                }
            }
            else
            {
                for (int i = 0; i < args.Length; i++) // Loop through array
                {

                    argument = argument +" "+ args[i];

                }

                // for test - eddie
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("argument: " + argument, EventLogEntryType.Information, 101, 1);
                }
            }

            try
                {
                    //XDocument doc = XDocument.Load("ClientVirtualiztion.xml");
                    XDocument doc = XDocument.Load(@"C:\Program Files (x86)\Philips Intellispace Portal\Config\System\ClientVirtualiztion.xml");
                    string ip = doc.XPathSelectElement("//ClientVirtualiztionConfigData/IP").Value;
                    
                    TcpClient tcpclnt = new TcpClient();
                    Console.WriteLine("Connecting.....");
                    tcpclnt.Connect(ip, 8001); // use the ipaddress as in the server program
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Connecting to: " + ip + " sending CMD command : \n" + argument,
                            EventLogEntryType.Information, 101, 1);
                    }


                    Console.WriteLine("Connected");
                    Stream stm = tcpclnt.GetStream();

                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Connected to: " + ip, EventLogEntryType.Information, 101, 1);
                    }


                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(argument);
                    Console.WriteLine("Transmitting.....");

                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[300];
                    int k = stm.Read(bb, 0, 300);

                    for (int i = 0; i < k; i++)
                        Console.Write(Convert.ToChar(bb[i]));


                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("CMD cmmand transmitted", EventLogEntryType.Information, 101, 1);
                    }

                    tcpclnt.Close();

                }

                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);

                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Error..... " + e.StackTrace, EventLogEntryType.Error, 101, 1);
                    }

                }
            }
    }
}

