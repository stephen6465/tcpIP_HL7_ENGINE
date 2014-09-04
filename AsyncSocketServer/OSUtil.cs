using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocketServer
{ /// <summary>
    /// class OSUtil
    /// This class just does some string tricks for the sample app
    /// It is no big deal.
    /// </summary>
    
    class OSUtil
    {
        char[] seps;

        // Allowed commands for the console app
        public enum os_cmd
        {
            OS_EXIT,
            OS_STARTSERVER,
            OS_CONNECT,
            OS_SEND,
            OS_DISCONNECT,
            OS_HELP,
            OS_UNDEFINED
        }


        public OSUtil()
        {
            seps = new char[] { ' ' };
        }

        // Parse the parameters from a command string
        public List<string> ParseParams(string commandstring)
        {
            string[] parts = commandstring.Split(seps);

            var parameters = new List<string>();

            if (parts.Length > 1)
            {
                for (int i = 1; i < parts.Length; i++)
                {
                    parameters.Add(parts[i]);
                }
            }

            return parameters;
        }

        // Parse a command from a string
        public os_cmd ParseCommand(string commandstring)
        {
            string[] parts = commandstring.Split(seps);

            if (!string.IsNullOrEmpty(parts[0]))
            {
                string cmd = parts[0];

                switch (cmd.ToLower())
                {
                    case "exit":
                        return os_cmd.OS_EXIT;
                        break;
                    case "help":
                        return os_cmd.OS_HELP;
                        break;
                    default:
                        return os_cmd.OS_UNDEFINED;
                        break;
                }
            }

            return os_cmd.OS_UNDEFINED;
        }

    }
}
