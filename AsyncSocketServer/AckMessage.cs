using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocketServer
{
    class AckMessage
    {
        public String ack;

        public AckMessage(String message)
        {
            this.ack = this.makeAck(message);
        }

        public String makeAck(String message)
        {
            String tempString = message.Substring(1, message.IndexOf((char)13));
            String firstPartAck = @"MSH|^~\&|MagView||||20140510233808||ACK|";
            String secondPartAck = "|T|2.x||||||||";
            String thirdPartAck = "MSA|AA||||||";
            StringBuilder t = new StringBuilder(firstPartAck);
            String messageId = this.getMessageId(message, 9);
            String tempString2 = "";

            t.Append(messageId);
            t.Append(secondPartAck);
            t.Append((char)13);
            t.Append(thirdPartAck);
            hl7 hl7 = new hl7();
            tempString2 = hl7.CreateMLLPMessage(t.ToString());

            this.ack = tempString2;

            return tempString2;
        }


        // Finish making the getmessageID function
        public String getMessageId(String message, int fieldNum)
        {
            Message t = new Message(message);
            String tempString = t.getElement("MSH", fieldNum);

            return tempString;
        }

    }
}
