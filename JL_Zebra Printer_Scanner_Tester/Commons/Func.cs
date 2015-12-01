using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace JL_Zebra_Printer_Scanner_Tester
{
    class Func
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public static string iniPath =  Application.StartupPath + "\\Config.ini";


        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        [DllImport("kernel32.dll")]

        private static extern long WritePrivateProfileString(string section, string key, string val, string FileName);

        [DllImport("kernel32.dll")]

        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string FileName);


        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);


        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);






        public static string[] GetSectionNames()
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


        public static string[] GetEntryNames(string section)
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

        public static string GetEntryValue(string section, string entry)
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

        public static void ApplicationExit()
        {
            DialogResult dr = MessageBox.Show("Do you want to exit the program?", "Exit", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes) Application.Exit();
            return;
        }


        //public static String GetIniValue(String Section, String Key)
        //{
        //    if (!File.Exists(iniPath)) throw new FileNotFoundException();
        //    StringBuilder temp = new StringBuilder(255);
        //    int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
        //    return temp.ToString();
        //}

        //public static int GetIniCount(String Section)
        //{
        //    if (!File.Exists(iniPath)) throw new FileNotFoundException();
        //    StringBuilder temp = new StringBuilder(255);
        //    int i = GetPrivateProfileString(Section, "count", "", temp, 255, iniPath);
        //    return i;
        //}

        //public static String GetIniValue(String Section, String Key, string path)
        //{
        //    if (!File.Exists(iniPath)) throw new FileNotFoundException();
        //    StringBuilder temp = new StringBuilder(255);
        //    int i = GetPrivateProfileString(Section, Key, "", temp, 255, path);
        //    return temp.ToString();
        //}

        /// <summary>
        /// SET INI Value
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public static void SetIniValue(String Section, String Key, String Value)
        {
            WritePrivateProfileString(Section, Key, Value, iniPath);
        }

        /// <summary>
        /// Check DB Conn
        /// </summary>
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
                }
                return false;
            }
        }

        public static void Minimize(object sender, NotifyIcon noti, string notiTitle, string notiText)
        {
            Control ctrl = (Control)sender;
            ctrl.Hide();
            noti.Visible = true;
            noti.ShowBalloonTip(2000, notiTitle,notiText,ToolTipIcon.Info);
        }

        public static void AddText(object from, object to)
        {
            Control _from = (Control)from;
            Control _to = (Control)to;

            _to.Text += "\n" + _from.Text;
            if(_from.Text!=null)_from.Text = "";
        }
        public static void AddText(string from, object to)
        {
            Control _to = (Control)to;
            _to.Text += "\n" + from;
        }

        public static void AddPrintInput(object from, object to, int ft)
        {
            if(to.GetType().Name == "GroupBox")
            {
                GroupBox _to = (GroupBox)to;
                Control _from = (Control)from;
                //_to.SelectionFont = new Font("tahoma", ft);
                _to.Text += "\n" + _from.Text;
                _from.Text = "";
            }
            else if(to.GetType().Name == "ListView")
            {
                ListView _to = (ListView)to;
                Control _from = (Control)from;
                _to.Text += "\n" + _from.Text;
                _from.Text = "";
            }
            else
            {
                Control _to = (Control)to;
                //RichTextBox _to = (RichTextBox)to;
                Control _from = (Control)from;
                //_to.SelectionFont = new Font("tahoma", ft);
                _to.Text += "\n" + _from.Text;
                _from.Text = "";
            }

        }

        public static void ReplaceText(object sender, string msg)
        {
            Control ctrl = (Control)sender;
            ctrl.Text = msg;
        }
    }
}
