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
    /// This is the class that sends and recieves the data in a way to manipulate (other words the bread n butter)
    /// class OSUserToken : IDisposable
    /// This class represents the instantiated read socket on the server side.
    /// It is instantiated when a server listener socket accepts a connection.
    /// </summary>
    sealed class OSUserToken : IDisposable
    {
        // This is a ref copy of the socket that owns this token
        private Socket ownersocket;

        // this stringbuilder is used to accumulate data off of the readsocket
        private StringBuilder stringbuilder;

        // This stores the total bytes accumulated so far in the stringbuilder
        private Int32 totalbytecount;

        // We are holding an exception string in here, but not doing anything with it right now.
        public String LastError;

        // The read socket that creates this object sends a copy of its "parent" accept socket in as a reference
        // We also take in a max buffer size for the data to be read off of the read socket
        public OSUserToken(Socket readSocket, Int32 bufferSize)
        {
            ownersocket = readSocket;
            stringbuilder = new StringBuilder(bufferSize);
            
        }

        // This allows us to refer to the socket that created this token's read socket
        public Socket OwnerSocket
        {
            get
            {
                return ownersocket;
            }
        }


        // Do something with the received data, then reset the token for use by another connection.
        // This is called when all of the data have been received for a read socket.
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

            // All the data has been read from the 
            // client. Display it on the console.
            //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
            //    content.Length, content);
            // Echo the ACk message here async or in the caller
            //Send(handler, content);



            //TODO: Load up a send buffer to send an ack back to the calling client
           
            //

            // Clear StringBuffer, so it can receive more data from the client.


        }


        // This method gets the data out of the read socket and adds it to the accumulator string builder
        public bool ReadSocketData(SocketAsyncEventArgs readSocket)
        {
            Int32 bytecount = readSocket.BytesTransferred;

            stringbuilder.Append(Encoding.ASCII.GetString(readSocket.Buffer, readSocket.Offset, bytecount));
            totalbytecount += bytecount;

            //// put my custom Hl7 code here...
            string content;
            string content2 = new String((char)hl7.MLLP_FIRST_END_CHARACTER, 1);
            content2 = content2 + new String((char)hl7.MLLP_LAST_END_CHARACTER, 1);

            content = stringbuilder.ToString();
            if (content.IndexOf(content2) > -1)
            {

                this.ProcessData(readSocket);

            }

            return true;

        }

        // This is a standard IDisposable method
        // In this case, disposing of this token closes the accept socket
        public void Dispose()
        {
            try
            {
                ownersocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                //Nothing to do here, connection is closed already
            }
            finally
            {
                ownersocket.Close();
            }
        }
    }

}
