using System;

/// 
/// This helper class supports the Minimum Low Layer Protocol

/// 
public abstract class MLLPHelper
{
    #region Private constants
    public const int MLLP_START_CHARACTER = 11; // HEX 0B
    public const int MLLP_FIRST_END_CHARACTER = 28; // HEX 1C
    public const int MLLP_LAST_END_CHARACTER = 13; // HEX 0D
    public String MllPendString = new String((char)MLLP_FIRST_END_CHARACTER, 1);
    public String MllPendString2 = new String((char)MLLP_LAST_END_CHARACTER, 1);

    public string MllPendString  { get; set; }
    public string MllPendString2 { get; set; }
    
    
    #endregion

    #region Public static methods
    /// 
    /// Validate MLLP message
    /// 
    ///

    ///Stringbuilder containing the message
    public static void StripMLLPContainer(StringBuilder sb)
    {
        // Strip the message of the MLLP container characters
        sb.Remove(0, 1);
        sb.Remove(sb.Length - 2, 2);
    }

    /// 
    /// Validate the MLLP message containing the HL7 message
    /// 
    ///Message
    /// true if valid

    public static bool ValidateMLLPMessage(StringBuilder sb)
    {
        bool result = false;

        if (sb.Length > 3)
        {
            if (((int)sb[0] == MLLP_START_CHARACTER))
            {
                if (((int)sb[sb.Length - 2] == MLLP_FIRST_END_CHARACTER) && ((int)sb[sb.Length - 1] == MLLP_LAST_END_CHARACTER))
                    result = true;
            }
        }

        return result;
    }

    /// 
    /// Create a MLLP message
    /// 
    ///Original message
    /// MLLP message

    public static string CreateMLLPMessage(string p)
    {
        StringBuilder sb = new StringBuilder(p);
        sb.Insert(0, (char)MLLP_START_CHARACTER);
        sb.Append((char)MLLP_FIRST_END_CHARACTER);
        sb.Append((char)MLLP_LAST_END_CHARACTER);

        return sb.ToString();
    }

    /// 
    /// Test if the character equals the start message character
    /// 
    ///Character to test
    /// True if matches

    public static bool IsStartCharacter(char start)
    {
        return (start == MLLP_START_CHARACTER);
    }

    public static bool IsEndCharacters(char second, char last)
    {
        if (((int)second == MLLP_FIRST_END_CHARACTER))
        {
            if (((int)last == MLLP_LAST_END_CHARACTER))
                result = true;
                return result;
        }
    }
    
    
    #endregion
}




