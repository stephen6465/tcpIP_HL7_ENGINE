using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;




namespace AsyncSocketServer
{
    /// <summary>
    /// This is a console app to test the client and server.
    /// It does minimal error handling.
    /// To see the valid commands, start the app and type "help" at the command prompt
    /// </summary>
    class Program
    {
        // We use util, and one server, and one client in this app
        static OSUtil os_util;
        static OSServer os_server;
        // static OSClient os_client;


        static void Main(string[] args)
        {
            //application state trackers
            bool shutdown = false;
            bool serverstarted = false;

            os_util = new OSUtil();

            //Intialize your settings and settings file
            Settings intSettings = new Settings();

            String path = Directory.GetCurrentDirectory();
            String setFile = @"\settings.xml";
            String ComPath = path + setFile;

            //Check for config file and if it doesn't exist then create it

            if (File.Exists(ComPath))
            {
                Settings intSet = intSettings.Deserialize(ComPath);
                intSettings = intSet;
            }
            else
            {
                //Might want to end here or do something with no config
                intSettings.Serialize(ComPath, intSettings);
                // to shut down just call os_server.Stop();
            }
            if (intSettings.Type.ToString().ToUpper().Trim() == "SERVER")
            {
                os_server = new OSServer(intSettings);

                bool started = os_server.Start(intSettings);
                if (!started)
                {
                    Console.WriteLine("Failed to Start Server.");
                    Console.WriteLine(os_server.GetLastError());
                }
                else
                {
                    Console.WriteLine(string.Format("Server started successfully.\nRunning on Port:{0} and IP:{1}", intSettings.LocalPort, intSettings.LocalIPAddress));
                    serverstarted = true;
                }
            }

            if (intSettings.Type.ToString().ToUpper().Trim() == "CLIENT")
            {
               OSClient os_client = new OSClient();

               bool connected = os_client.Connect(intSettings.RemoteIPAddress, Convert.ToInt32(intSettings.RemotePort));
               if (!connected)
               {
                    Console.WriteLine("Failed to Start Client.");
                    Console.WriteLine(os_client.GetLastError());
                }
                else
                {
                    Console.WriteLine(string.Format("Client started successfully.\nRunning on Port:{0} and IP:{1}", intSettings.RemoteIPAddress, intSettings.RemotePort));
                    connected = true;

                    // Watch only for changes to *.txt  or hl7 files this means multiple watchers.
                    String[] filters = { "*.txt", "*.hl7" };
                    List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
                   //     MyWatcher.Path = intSettings.OutFolderPath;
                
                    foreach (string f in filters)
                    {
                        FileSystemWatcher w = new FileSystemWatcher();
                        w.Filter = f;
                        w.Path = intSettings.OutFolderPath;
                        w.IncludeSubdirectories = false;
                        // Enable the component to begin watching for changes.
                        w.EnableRaisingEvents = true;
                        
                        w.Changed += new System.IO.FileSystemEventHandler(os_client.myFileWatcher_ChangeDetecter);
                        w.Created += new System.IO.FileSystemEventHandler(os_client.myFileWatcher_ChangeDetecter);
                        watchers.Add(w);
                    }
                 }
            }
            
            
            while (!shutdown)
            {
                string userinput = Console.ReadLine();

                if (!string.IsNullOrEmpty(userinput))
                {
                    switch (os_util.ParseCommand(userinput))
                    {
                        case OSUtil.os_cmd.OS_EXIT:
                            {
                                if (serverstarted)
                                {
                                    os_server.Stop();
                                }
                                shutdown = true;
                                break;
                            }
                        case OSUtil.os_cmd.OS_HELP:
                            {
                                Console.WriteLine("Available Commands:");
                                Console.WriteLine("exit = Stop the server and quit the program");
                                break;
                            }

                    }

                }
            }


        }
    }
}
