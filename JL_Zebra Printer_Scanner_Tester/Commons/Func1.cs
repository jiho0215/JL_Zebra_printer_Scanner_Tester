using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace JL_Zebra_Printer_Scanner_Tester
{
    class Func1
    {
 

        /// <summary>
        /// RadioButton DoubleClick
        /// </summary>
        [BrowsableAttribute(true)]
        public event EventHandler DoubleClick;

        /// <summary>
        /// Start Mousedown Move Form/Panel
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;


        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        /// End Mousedown Move Form/Panel

        ///Start Config DLLImport 
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string FileName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string FileName);


        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);


        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);
        ///End Config DLLImport 

        /// <summary>
        /// Get Section Names from Config
        /// </summary>
        /// <param name="iniPath"></param>
        /// <returns></returns>
        public static string[] GetSectionNames(string iniPath)
        {
            //    Sets the maxsize buffer to 500, if the more
            //    is required then doubles the size each time.
            for (int maxsize = 500; true; maxsize *= 2)
            {
                //    Obtains the information in bytes and stores
                //    them in the maxsize buffer (Bytes array)
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxsize, iniPath);

                // Check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size < maxsize - 2)
                {
                    // Converts the bytes value into an ASCII char. This is one long string.
                    string Selected = Encoding.ASCII.GetString(bytes, 0,
                                               size - (size > 0 ? 1 : 0));
                    // Splits the Long string into an array based on the "\0"
                    // or null (Newline) value and returns the value(s) in an array
                    return Selected.Split(new char[] { '\0' });
                }
            }
        }

        /// <summary>
        /// Get Key Names from Config
        /// </summary>
        /// <param name="iniPath"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string[] GetKeyNames(string iniPath, string section)
        {
            //    Sets the maxsize buffer to 500, if the more
            //    is required then doubles the size each time. 
            for (int maxsize = 500; true; maxsize *= 2)
            {
                //    Obtains the EntryKey information in bytes
                //    and stores them in the maxsize buffer (Bytes array).
                //    Note that the SectionHeader value has been passed.
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(section, 0, "", bytes, maxsize, iniPath);

                // Check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size < maxsize - 2)
                {
                    // Converts the bytes value into an ASCII char.
                    // This is one long string.
                    string entries = Encoding.ASCII.GetString(bytes, 0,
                                              size - (size > 0 ? 1 : 0));
                    // Splits the Long string into an array based on the "\0"
                    // or null (Newline) value and returns the value(s) in an array
                    return entries.Split(new char[] { '\0' });
                }
            }
        }

        /// <summary>
        /// Get Valuse of a Key from Config
        /// </summary>
        /// <param name="iniPath"></param>
        /// <param name="section"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string GetKeyValue(string iniPath, string section, string entry)
        {
            //    Sets the maxsize buffer to 250, if the more
            //    is required then doubles the size each time. 
            for (int maxsize = 250; true; maxsize *= 2)
            {
                //    Obtains the EntryValue information and uses the StringBuilder
                //    Function to and stores them in the maxsize buffers (result).
                //    Note that the SectionHeader and EntryKey values has been passed.
                StringBuilder result = new StringBuilder(maxsize);
                int size = GetPrivateProfileString(section, entry, "",
                                                   result, maxsize, iniPath);
                if (size < maxsize - 1)
                {
                    // Returns the value gathered from the EntryKey
                    return result.ToString();
                }
            }
        }
        
        /// <summary>
        /// SET INI Value
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetIniValue(string iniPath, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, iniPath);
        }

        /// <summary>
        /// Check DB Connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool checkConn(string connectionString)
        {
            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                try
                {
                    _con.Open();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Zebra Printer 
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        
    }
}
