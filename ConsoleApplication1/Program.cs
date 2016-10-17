using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Rtc.Internal.Collaboration.Presence;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.InteropServices;

using Microsoft.Lync.Model;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
        lyncPresence test = new lyncPresence();

            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            // Show
           // ShowWindow(handle, SW_SHOW);
            int count=10;
            test.initCîent();
            do {
                test.setClient(ContactAvailability.Away);
                System.Threading.Thread.Sleep(500);

                test.setClient(ContactAvailability.Busy);
                System.Threading.Thread.Sleep(500);

                test.setClient(ContactAvailability.DoNotDisturb);
                System.Threading.Thread.Sleep(500);

                test.setClient(ContactAvailability.TemporarilyAway);
                System.Threading.Thread.Sleep(500);

                test.setClient(ContactAvailability.Free);
                System.Threading.Thread.Sleep(500);


                count -= 1;
            }while(count > 0);

        }


    }
    
    class lyncPresence
    {

        LyncClient lyncClient;

        public void initCîent()
        {
            try
            {
                lyncClient = LyncClient.GetClient();
            }

            catch (ClientNotFoundException clientNotFoundException)
            {
                Console.WriteLine(clientNotFoundException);
                return;
            }
            catch (NotStartedByUserException notStartedByUserException)
            {
                Console.Out.WriteLine(notStartedByUserException);
                return;
            }
            catch (LyncClientException lyncClientException)
            {
                Console.Out.WriteLine(lyncClientException);
                return;
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                    return;
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }

        public void setClient(ContactAvailability pres)
        {
            //$pres = new PresenceStateExtensions();

            Dictionary<PublishableContactInformationType, object> newInformation =
                new Dictionary<PublishableContactInformationType, object>();
            newInformation.Add(PublishableContactInformationType.Availability, pres);

            //Publish the new availability value
            try
            {
                lyncClient.Self.BeginPublishContactInformation(newInformation, PublishContactInformationCallback, null);
            }
            catch (LyncClientException lyncClientException)
            {
                Console.WriteLine(lyncClientException);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

        }
        private void PublishContactInformationCallback(IAsyncResult result)
        {
            lyncClient.Self.EndPublishContactInformation(result);
        }
        /// <summary>
        /// Identify if a particular SystemException is one of the exceptions which may be thrown
        /// by the Lync Model API.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsLyncException(SystemException ex)
        {
            return
                ex is NotImplementedException ||
                ex is ArgumentException ||
                ex is NullReferenceException ||
                ex is NotSupportedException ||
                ex is ArgumentOutOfRangeException ||
                ex is IndexOutOfRangeException ||
                ex is InvalidOperationException ||
                ex is TypeLoadException ||
                ex is TypeInitializationException ||
                ex is InvalidComObjectException ||
                ex is InvalidCastException;
        }
    }
    class TechnetSample
    {
        private void technetSample()
        {
            string URLAuth = "https://technet.rapaport.com/HTTP/Authenticate.aspx";
            WebClient webClient = new WebClient();

            NameValueCollection formData = new NameValueCollection();
            formData["Username"] = "myUser";
            formData["Password"] = "myPassword";

            byte[] responseBytes = webClient.UploadValues(URLAuth, "POST", formData);
            string resultAuthTicket = Encoding.UTF8.GetString(responseBytes);
            webClient.Dispose();

            string URL = "http://technet.rapaport.com/HTTP/Upload/Upload.aspx?Method=file";
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(URL);

            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            string FilePath = "C:\\test.csv";
            formData.Clear();
            formData["ticket"] = resultAuthTicket;
            formData["ReplaceAll"] = "false";

            Stream postDataStream = GetPostStream(FilePath, formData, boundary);

            webRequest.ContentLength = postDataStream.Length;
            Stream reqStream = webRequest.GetRequestStream();

            postDataStream.Position = 0;

            byte[] buffer = new byte[1024];
            int bytesRead = 0;

            while ((bytesRead = postDataStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                reqStream.Write(buffer, 0, bytesRead);
            }

            postDataStream.Close();
            reqStream.Close();

            StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string Result = sr.ReadToEnd();
        }

        private static Stream GetPostStream(string filePath, NameValueCollection formData, string boundary)
        {
            Stream postDataStream = new System.IO.MemoryStream();

            //adding form data
            string formDataHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
            "Content-Disposition: form-data; name=\"{0}\";" + Environment.NewLine + Environment.NewLine + "{1}";

            foreach (string key in formData.Keys)
            {
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(formDataHeaderTemplate,
                key, formData[key]));
                postDataStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            //adding file data
            FileInfo fileInfo = new FileInfo(filePath);

            string fileHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
            Environment.NewLine + "Content-Type: application/vnd.ms-excel" + Environment.NewLine + Environment.NewLine;

            byte[] fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate,
            "UploadCSVFile", fileInfo.FullName));

            postDataStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

            FileStream fileStream = fileInfo.OpenRead();

            byte[] buffer = new byte[1024];

            int bytesRead = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                postDataStream.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();

            byte[] endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes("--" + boundary + "--");
            postDataStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

            return postDataStream;
        }
    }
    public class WebRequestPostExample
    {
        public static 
            void  tessttt()
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("http://www.contoso.com/PostAccepter.aspx ");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            
            // Create POST data and convert it to a byte array.
            string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
        }
    }
}

