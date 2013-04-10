using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;

namespace verifyemailsolution
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Button1_Click(object sender, EventArgs e)
        {
            mxLabel.Text = "";  //clear the mx records from previous verification
            string emailAddress = TextBox1.Text;
            string[] emailAddressArray = emailAddress.Split('@'); // split email address to get domain
            string domainName = emailAddressArray[(emailAddressArray.Length - 1)];
            domainLabel.Text = domainName;
            string[] mxRecords = Mx.GetMXRecords(domainName);
            foreach (string mailRecord in mxRecords)  // print out all MX records
            {
                mxLabel.Text += mailRecord + "<br>";
            }

            TcpClient tClient = new TcpClient(mxRecords[0], 25);  //Use the first MX record, and smtp port 25
            string CRLF = "\r\n";
            byte[] dataBuffer;
            string ResponseString;
            NetworkStream netStream = tClient.GetStream();
            StreamReader reader = new StreamReader(netStream);
            ResponseString = reader.ReadLine();
            /* Perform HELO to SMTP Server and get Response */
            dataBuffer = BytesFromString("HELO NicholasDecker" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            ResponseString = reader.ReadLine();
            dataBuffer = BytesFromString("MAIL FROM:<6studiozero@gmail.com>" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            ResponseString = reader.ReadLine();
            /* Read Response of the RCPT TO Message to know from google if it exist or not */
            dataBuffer = BytesFromString("RCPT TO:<" + TextBox1.Text.Trim() + ">" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            /* this is the response that can take a moment, and may need to be paused?
             * 
             >> ResponseString = reader.ReadLine(); <<
             * 
             * I don't know how to properly pause
             * 
             */
            
            ResponseString = reader.ReadLine();  
            errorLabel.Text = ResponseString;

            if (GetResponseCode(ResponseString) == 550)  // 550 is a bounce.  We only want to affect these hard bounces -- everything else can pass
            {
                //labelWaitCount.Text = waitCount.ToString();
                Label1.Text = TextBox1.Text.Trim() + "Mail Address Does not Exist !<br/><br/>";
                Label1.Text = "<B><font color='red'>Original Error from Smtp Server : " + ResponseString + "</font></b>";
            }
            else
            {
                //labelWaitCount.Text = waitCount.ToString();
                Label1.Text = TextBox1.Text.Trim() + " is a good email address";
            }
            /* QUITE CONNECTION */
            dataBuffer = BytesFromString("QUITE" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            tClient.Close();
        }
        private byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        private int GetResponseCode(string ResponseString)
        {
            return int.Parse(ResponseString.Substring(0, 3));
        }
    }
    public class Mx
    {
        public Mx()
        {
        }
        [DllImport("dnsapi", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern int DnsQuery([MarshalAs(UnmanagedType.VBByRefStr)]ref string pszName, QueryTypes wType, QueryOptions options, int aipServers, ref IntPtr ppQueryResults, int pReserved);

        [DllImport("dnsapi", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void DnsRecordListFree(IntPtr pRecordList, int FreeType);

        public static string[] GetMXRecords(string domain)
        {

            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            MXRecord recMx;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new NotSupportedException();
            }
            ArrayList list1 = new ArrayList();
            int num1 = Mx.DnsQuery(ref domain, QueryTypes.DNS_TYPE_MX, QueryOptions.DNS_QUERY_BYPASS_CACHE, 0, ref ptr1, 0);
            try
            {
                if (num1 != 0)
                {
                    throw new Win32Exception(num1);  // this is where the handling of an exception needs to go (where domain is invalid pops)
                }

                for (ptr2 = ptr1; !ptr2.Equals(IntPtr.Zero); ptr2 = recMx.pNext)  // these occasionally throw something also.
                {
                    recMx = (MXRecord)Marshal.PtrToStructure(ptr2, typeof(MXRecord));
                    if (recMx.wType == 15)
                    {
                        string text1 = Marshal.PtrToStringAuto(recMx.pNameExchange);
                        list1.Add(text1);
                    }
                }
            }
            catch(Win32Exception e)
            {
                e.Message.ToString();
            }


            Mx.DnsRecordListFree(ptr2, 0);
            return (string[])list1.ToArray(typeof(string));
        }

        private enum QueryOptions
        {
            DNS_QUERY_ACCEPT_TRUNCATED_RESPONSE = 1,
            DNS_QUERY_BYPASS_CACHE = 8,
            DNS_QUERY_DONT_RESET_TTL_VALUES = 0x100000,
            DNS_QUERY_NO_HOSTS_FILE = 0x40,
            DNS_QUERY_NO_LOCAL_NAME = 0x20,
            DNS_QUERY_NO_NETBT = 0x80,
            DNS_QUERY_NO_RECURSION = 4,
            DNS_QUERY_NO_WIRE_QUERY = 0x10,
            DNS_QUERY_RESERVED = -16777216,
            DNS_QUERY_RETURN_MESSAGE = 0x200,
            DNS_QUERY_STANDARD = 0,
            DNS_QUERY_TREAT_AS_FQDN = 0x1000,
            DNS_QUERY_USE_TCP_ONLY = 2,
            DNS_QUERY_WIRE_ONLY = 0x100
        }

        private enum QueryTypes
        {
            DNS_TYPE_MX = 15
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MXRecord
        {
            public IntPtr pNext;
            public string pName;
            public short wType;
            public short wDataLength;
            public int flags;
            public int dwTtl;
            public int dwReserved;
            public IntPtr pNameExchange;
            public short wPreference;
            public short Pad;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}