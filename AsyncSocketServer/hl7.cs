using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocketServer
{
    class hl7 : MLLPHelper
    {

        public const int MLLP_START_CHARACTER = 11; // HEX 0B
        public const int MLLP_FIRST_END_CHARACTER = 28; // HEX 1C
        public const int MLLP_LAST_END_CHARACTER = 13; // HEX 0D

        public String MllPendString = new String((char)MLLP_FIRST_END_CHARACTER, 1);
        public String MllPendString2 = new String((char)MLLP_LAST_END_CHARACTER, 1);


    }
}
