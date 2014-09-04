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
    /// class OSCore
    /// This is a base class that is used by both clients and servers.
    /// It contains the plumbing to set up a socket connection.
    /// </summary>
    class OSCore
    {
        // This is just some utilities that we use all over
        protected OSUtil os_util;

        // these are the defaults if the user does not provide any parameters
        protected const string DEFAULT_SERVER = "localhost";
        protected const int DEFAULT_PORT = 3915;

        //  We default to a 1024 Byte buffer size
        protected const int DEFAULT_BUFFER_SIZE = 1500;

        // This is the connection socket and endpoint information
        protected Socket connectionsocket;
        protected IPEndPoint connectionendpoint;

        // This is some error handling stuff that is not well implemented
        protected string lasterror;
        protected bool exceptionthrown;

        // This is the current buffer size for receive and send
        protected int buffersize;


        // We only instantiate the utility class here.
        // We could probably make it static and avoid this.
        public OSCore()
        {
            os_util = new OSUtil();
        }

        // An IPEndPoint contains all of the information about a server or client
        // machine that a socket needs.  Here we create one from information
        // that we send in as parameters
        public IPEndPoint CreateIPEndPoint(string servername, int portnumber)
        {
            try
            {
                // We get the IP address and stuff from DNS (Domain Name Services)
                // I think you can also pass in an IP address, but I would not because
                // that would not be extensible to IPV6 later
                IPHostEntry hostInfo = Dns.GetHostEntry(servername);
                IPAddress serverAddr = hostInfo.AddressList[0];
                return new IPEndPoint(serverAddr, portnumber);
            }
            catch (Exception ex)
            {
                exceptionthrown = true;
                lasterror = ex.ToString();
                return null;
            }
        }


        // This method peels apart the command string to create either a client or server socket,
        // which is not great because it means the method has to know the semantics of the command
        // that is passed to it.  So this needs to be fixed.
        protected bool CreateSocket(string iporname, int port)
        {
            exceptionthrown = false;

            if (iporname.Length > 0 && (port > 1024 && port < 65500))
            {
                connectionendpoint = CreateIPEndPoint(iporname, port);
            }
            else
            {
                //We need to log here that we went to defualts... 
                connectionendpoint = CreateIPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);
            }
            // If we get here, we try to create the socket using the IPEndpoint information.
            // We are defaulting here to TCP Stream sockets, but you could change this with more parameters.
            if (!exceptionthrown)
            {
                try
                {
                    connectionsocket = new Socket(connectionendpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }
                catch (Exception ex)
                {
                    exceptionthrown = true;
                    lasterror = ex.ToString();
                    return false;
                }
            }
            return true;
        }

        // This method is a lame way for external classes to get the last error message that was posted
        // from this class.  It is a poor man's exception handler.  Don't do this in production code.
        // Use proper exception handling.
        public string GetLastError()
        {
            return lasterror;
        }
    }
}
