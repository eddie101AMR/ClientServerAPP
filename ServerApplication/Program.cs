using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Win32;

namespace ServerApplication
{
    class Program
    {
        //Initializes the Listener
        private static TcpListener myList;
        private static Socket s;
        private static byte[] b = new byte[300];
        private static int k;
        private static string port;
        private static string portalPath;

        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                XDocument doc = XDocument.Load("ClientVirtualiztion.xml");
                port = doc.XPathSelectElement("//ClientVirtualiztionConfigData/Port").Value;
                myList = new TcpListener(Convert.ToInt32(port));
                OpenConnection();
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

        private static void RecieveMessage()
        {
            string PACSURLPluginPath = "";
            string strCmdText = "";
            while (true)
            {

                strCmdText = "";
                k = s.Receive(b);
                Console.WriteLine("Recieved...");
                for (int i = 0; i < k; i++)
                {
                    strCmdText = strCmdText + Convert.ToChar(b[i]);
                    Console.Write(Convert.ToChar(b[i]));
                }

                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("The string was recieved by the server."));
                Console.WriteLine("\nSent Acknowledgement");

                if (strCmdText != "")
                {
                    PACSURLPluginPath = "\"" + portalPath + @"\PACSURLPlugin.exe" + "\"";
                    //System.Diagnostics.Process.Start("\"C:\\Program Files (x86)\\Philips IntelliSpace Portal\\System\\PACSURLPlugin.exe\"",strCmdText); 
                    System.Diagnostics.Process.Start(PACSURLPluginPath, strCmdText);
                }


                OpenConnection();
            }
        }

        private static void OpenConnection()
        {
            /* Start Listeneting at the specified port */
            myList.Start();

            Console.WriteLine("The server is running at port " + port + " ...");
            Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");

            s = myList.AcceptSocket();
            Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

            RecieveMessage();
        }

        private static void UpdatePortalPath()
        {
            RegistryKey root = Registry.CurrentUser;
            RegistryKey systemTypeKey = null;
            string registryPath = @"Software\Philips Healthcare\CT\PortalClient";
            systemTypeKey = root.OpenSubKey(registryPath);
            if (systemTypeKey == null)
            {
                root = Registry.Users;
                registryPath = @".Default\" + registryPath;
                systemTypeKey = root.OpenSubKey(registryPath);
            }
            portalPath = (string)systemTypeKey.GetValue("pmsRoot", null) + "system";

        }
    }
}

