using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocketServer
{
    class Message
    {
        List<String> segments = new List<string>();
        String message = "";
        public Message(String message)
        {
            this.message = message;
        }

        public String getElement(String Segment, String field)
        {

            if (String.IsNullOrWhiteSpace(Segment) || String.IsNullOrWhiteSpace(field))
            {
                return "";
            }

            //Segment = this.getSegments(this.message, Segment);
            field = this.getField(this.getSegments(this.message, Segment), Convert.ToInt32(field));

            return field;
        }

        public String getElement(String Segment, int field)
        {

            if (String.IsNullOrWhiteSpace(Segment))
            {
                return "";
            }

            string temp = "";
            //Segment = this.getSegments(this.message, Segment);
            temp = this.getField(this.getSegments(this.message, Segment), field);

            return temp;
        }


        public String getElement(String Segment, int field, int sequence)
        {

            if (String.IsNullOrWhiteSpace(Segment))
            {
                return "";
            }

            //Segment = this.getSegments(this.message, Segment);
            String seqString = "";
            seqString = getSequence(this.getField(this.getSegments(this.message, Segment), field), sequence);
            return seqString;
        }


        public String getSequence(String Field, int SeqNum)
        {
            //StringBuilder t = new StringBuilder(J);
            String[] t = Field.Split('^');
            int c = 0;

            if (SeqNum < 1)
            {

                return Field;
            }

            foreach (String item in t)
            {
                c++;

                if (c == SeqNum)
                {
                    return item;
                }

            }

            return "";
        }

        public String getField(String segment, int FieldNum)
        {
            //StringBuilder t = new StringBuilder(J);
            String[] t = segment.Split('|');
            int c = 0;
            foreach (String item in t)
            {
                c++;

                if (c == FieldNum)
                {
                    return item;
                }

            }

            return "";
        }
        public String getSegments(String J, String segmentWanted)
        {

            StringBuilder t = new StringBuilder(J);

            hl7 hl7 = new hl7();
            J = hl7.StripMLLPContainer(t);

            int cnt = 0;
            foreach (char c in J)
            {
                if (c == (char)13) cnt++;
            }

            segmentWanted = segmentWanted.ToUpper().Trim() + "|";

            String[] Segments = J.Split((char)13);
            foreach (String item in Segments)
            {
                if (item.IndexOf(segmentWanted) > 0)
                {
                    return item;
                }
            }


            return "";
        }


    }
}
