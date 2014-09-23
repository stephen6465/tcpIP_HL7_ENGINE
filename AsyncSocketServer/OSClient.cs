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

        public List<Message> MessagesOut = new List<Message>();
        // put a config file object here maybe... This way we can reconnect if not connected... put on a timer to check. and reconnect. put  a heart beat here somewhere and ack processor. 
        Settings intSet = new Settings();
         SocketAsyncEventArgs item = new SocketAsyncEventArgs();
                
        public OSClient(Settings intSet)
        {
            intSet = intSet;
            item.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            item.SetBuffer(new Byte[Convert.ToInt32(intSet.BufferSize)], 0, Convert.ToInt32(intSet.BufferSize));
          
        }

        // This method is used to send a message to the server
        public bool Send(string cmdstring)
        {
            hl7 hl7 = new hl7();

            cmdstring = hl7.CreateMLLPMessage(cmdstring);


            exceptionthrown = false;

            //var parameters = os_util.ParseParams(cmdstring);
            if (cmdstring.Length > 0)
            {
                try
                {
                    // We need a connection to the server to send a message
                    if (connectionsocket.Connected)
                    {
                        byte[] byData = System.Text.Encoding.ASCII.GetBytes(cmdstring);
                        connectionsocket.Send(byData);
                        // connectionsocket.Receive()
                        return true;
                    }
                    else
                    {
                        try
                        {
                            
                            this.Connect(intSet.RemoteIPAddress, Convert.ToInt32(intSet.RemotePort));
                        
                            if (connectionsocket.Connected)
                            {
                                byte[] byData = System.Text.Encoding.ASCII.GetBytes(cmdstring);
                                connectionsocket.Send(byData);
                                return true;
                            }
                            return false;
                        }
                        catch (Exception ex)
                        {
                            lasterror = ex.ToString();
                            return false;
                        }

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

        private void ProcessReceive(SocketAsyncEventArgs readSocket)
        {
            // if BytesTransferred is 0, then the remote end closed the connection
            if (readSocket.BytesTransferred > 0)
            {
                //SocketError.Success indicates that the last operation on the underlying socket succeeded
                if (readSocket.SocketError == SocketError.Success)
                {
                    OSUserToken token = readSocket.UserToken as OSUserToken;
                    if (token.ReadClientSocketData(readSocket))
                    {
                        Socket readsocket = token.OwnerSocket;

                        // If the read socket is empty, we can do something with the data that we accumulated
                        // from all of the previous read requests on this socket
                        if (readsocket.Available == 0)
                        {
                            token.ProcessClientData(readSocket);
                        }

                        // Start another receive request and immediately check to see if the receive is already complete
                        // Otherwise OnIOCompleted will get called when the receive is complete
                        // We are basically calling this same method recursively until there is no more data
                        // on the read socket
                        bool IOPending = readsocket.ReceiveAsync(readSocket);
                        if (!IOPending)
                        {
                            ProcessReceive(readSocket);
                        }
                    }
                    else
                    {
                        Console.WriteLine(token.LastError);
                    //    CloseReadSocket(readSocket);
                    }

                }
                else
                {
                    ProcessError(readSocket);
                }
            }
            else
            {
                //CloseReadSocket(readSocket);
            }
        }


        public void ProcessData(SocketAsyncEventArgs args)
        {
            // Get the last message received from the client, which has been stored in the stringbuilder.
            String received = stringbuilder.ToString();

            //TODO Use message received to perform a specific operation.


            string content2 = new String((char)hl7.MLLP_FIRST_END_CHARACTER, 1);
            content2 = content2 + new String((char)hl7.MLLP_LAST_END_CHARACTER, 1);



            // get the message up to the eof characters
            // and remove the message from the string builder
            if (received.IndexOf(content2) > -1)
            {
                if (received.IndexOf(content2) == 0)
                {

                    totalbytecount = 0;
                    stringbuilder.Length = 0;
                    Console.WriteLine("HERE CLEARING THINGS OUT");
                }
                else
                {

                    //int temp = received.IndexOf(content2);
                    int temp2 = received.IndexOf(content2);
                    totalbytecount = totalbytecount - received.Length;
                    received = received.Substring(1, temp2);  //Might need + 2 here to get the full message

                    stringbuilder.Remove(0, temp2);
                    totalbytecount = 0;
                    stringbuilder.Length = 0;
                    Console.WriteLine("Received: \"{0}\". The server has read {1} bytes. {2}", received, received.Length, temp2);

                    Message m = new Message(received);

                    AckMessage ack = new AckMessage(received);

                    Console.WriteLine(ack.ack);

                    Byte[] sendBuffer = Encoding.ASCII.GetBytes(ack.ack);
                    args.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                    this.OwnerSocket.Send(args.Buffer);

                }

            }

        private
            
            void ProcessError(SocketAsyncEventArgs readSocket)
        {
            Console.WriteLine(readSocket.SocketError.ToString());
           // CloseReadSocket(readSocket);
        }

        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            // Determine which type of operation just completed and call the associated handler.
            // We are only processing receives right now on this server.
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
              //  case SocketAsyncOperation.Send:
                    //this.ProcessSend(e);
                    //break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a Receive ");
            }
        }

        public void myFileWatcher_ChangeDetecter(object sender,
        System.IO.FileSystemEventArgs e)
        {
            // do something here.... as in read the file in and check it and then send it to the server 
            //if we are connected. if not connect and then send off
            using (StreamReader newMsg = new StreamReader(e.FullPath))
            {
                String msg = newMsg.ReadToEnd();

                if (this.Send(msg))
                {
                    //test = msg;
                    Message m = new Message(msg);

                    // an accumulator of messages leaving. 
                    this.MessagesOut.Add(m);

                }; // put lots of error handling in this skimp function

            }
            // put error code here.
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

            if (CreateSocket(iporname, port))
            {
                try
                {
                    var connectendpoint = CreateIPEndPoint(iporname, port);
                    connectionsocket.Connect(connectionendpoint);

                    item.UserToken = new OSUserToken(this.connectionsocket, Convert.ToInt32(intSet.BufferSize));
                    
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
