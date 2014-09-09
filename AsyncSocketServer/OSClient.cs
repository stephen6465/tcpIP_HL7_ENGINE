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
    /// class OSClient : OSCore
    /// This is a client class that I added into this project
    /// </summary>
    class OSClient : OSCore
    {

        // This method is used to send a message to the server
        public bool Send(string cmdstring)
        {
            hl7 hl7 = new hl7();

            cmdstring = hl7.CreateMLLPMessage(cmdstring);


            exceptionthrown = false;

            //var parameters = os_util.ParseParams(cmdstring);
            if (cmdstring.Length > 0 )
            {
                try
                {
                    // We need a connection to the server to send a message
                    if (connectionsocket.Connected)
                    {
                        byte[] byData = System.Text.Encoding.ASCII.GetBytes(cmdstring);
                        connectionsocket.Send(byData);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    lasterror = ex.ToString();
                    return false;
                }
            }
            else
            {
                lasterror = "No message provided for Send.";
                return false;
            }
        }

        public void myFileWatcher_ChangeDetecter(object sender,
        System.IO.FileSystemEventArgs e)
        {
            // do something here.... as in read the file in and check it and then send it to the server 
            //if we are connected. if not connect and then send off
           using(StreamReader newMsg = new StreamReader(e.FullPath))
            {
                String msg = newMsg.ReadToEnd();
                
               this.Send(msg);
             }
            
        }


        // This method disconnects us from the server
        public void DisConnect()
        {
            try
            {
                connectionsocket.Close();
            }
            catch
            {
                //nothing to do since connection is already closed
            }
        }


        // This method connects us to the server.
        // Winsock is very optimistic about connecting to the server.
        // It will not tell you, for instance, if the server actually accepted the connection.  It assumes that it did.
        public bool Connect(string iporname, int port)
        {
            exceptionthrown = false;

            if (CreateSocket( iporname,  port))
            {
                try
                {
                   
                        var connectendpoint = CreateIPEndPoint(iporname, port);
                        connectionsocket.Connect(connectionendpoint);
                        return true;
                   
                }
                catch (Exception ex)
                {
                    exceptionthrown = true;
                    lasterror = ex.ToString();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
