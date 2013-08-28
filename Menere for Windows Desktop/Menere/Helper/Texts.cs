using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.Helper
{
    public class Texts
    {
        public static string remove_html(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
        }

        public static string get_command_line_text(string text, int max_length = -1)
        {
            string command_line = Helper.Texts.remove_html(text);
            command_line = command_line.Replace(System.Environment.NewLine, "%0A");

            if (max_length > 0 && command_line.Length > max_length)
            {
                command_line = command_line.Substring(0, max_length - 3) + "...";
            }
            return command_line;
        }

        
    }
}
