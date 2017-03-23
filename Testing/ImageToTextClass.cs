using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using asprise_ocr_api;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Debug.WriteLine("---------------------------------------------");
            //checkOCR();
            checkOCRfromWatson();
            System.Diagnostics.Debug.WriteLine("---------------------------------------------");
        }


        static void checkOCR()
        {
            AspriseOCR.SetUp();
            AspriseOCR ocr = new AspriseOCR();
            ocr.StartEngine("eng", AspriseOCR.SPEED_FASTEST);

            // string uri = "http://pad1.whstatic.com/images/thumb/f/f5/Write-a-Business-Process-Document-Step-7-Version-2.jpg/aid1537933-900px-Write-a-Business-Process-Document-Step-7-Version-2.jpg";

            // string str = @"../../images/a.png";
            string s = ocr.Recognize(@"../../images/a.png", -1, -1, -1, -1, -1, AspriseOCR.RECOGNIZE_TYPE_TEXT, AspriseOCR.OUTPUT_FORMAT_PLAINTEXT);


            System.Diagnostics.Debug.WriteLine("OCR Result: " + s);
            // process more images here ...

            ocr.StopEngine();
        }

        static void checkOCRfromWatson()
        {
            string str = "https://gateway-a.watsonplatform.net/visual-recognition/api/v3/recognize_text/classify?api_key=18f228f986752f68190ed4ac1e997edd9bcb392c&url=http://www.antigrain.com/research/font_rasterization/adobe_text_rendering.png&version=2016-05-19";
            WebRequest request = WebRequest.Create(str);

            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;

            // Get the response.  
            WebResponse response = request.GetResponse();

            // Display the status.  
            System.Diagnostics.Debug.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            char c = '"';
            string s_begin = c + "text" + c + ": ";
            string s_end = c + "words" + c + ": [";

            int i_begin = responseFromServer.IndexOf(s_begin) + 8;
            int i_end = responseFromServer.IndexOf(s_end) - 15;

            int length = i_end - i_begin + 1;


            string s = responseFromServer.Substring(i_begin, length);
            // Display the content.  
            System.Diagnostics.Debug.WriteLine(s);
            // Clean up the streams and the response.  
            reader.Close();

            response.Close();
        }
    }


}


