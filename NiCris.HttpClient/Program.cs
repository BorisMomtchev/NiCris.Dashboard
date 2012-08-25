using System;
using System.Net;
using Microsoft.Http;
using System.Xml.Linq;

namespace NiCris.HttpClient
{
    public class Program
    {
        static Microsoft.Http.HttpClient client = new Microsoft.Http.HttpClient();
        static string BizMsgServiceUrl = "http://localhost:3505/BizMsgService/";
        const string testID = "43";

        static void Main(string[] args)
        {
            client.Stages.Add(new MyHttpStage());

            Console.WriteLine("Press enter to issue an HTTP GET on {0} (ReadAsString)", BizMsgServiceUrl);
            Console.ReadLine();
            HttpGetOnServiceReadAsString();

            Console.WriteLine("\nPress enter to issue an HTTP GET on {0} (ReadAsXElement)", BizMsgServiceUrl);
            Console.ReadLine();
            HttpGetOnServiceReadAsString();

            // Create
            Console.WriteLine("\nPress enter to issue an HTTP POST (Create) on {0}", BizMsgServiceUrl);
            Console.ReadLine();
            HttpPostOnService();

            // Update
            Console.WriteLine("\nPress enter to issue an HTTP PUT (Update) on {0}", BizMsgServiceUrl + testID);
            Console.ReadLine();
            HttpPutOnService();

            // Delete
            Console.WriteLine("\nPress enter to issue an HTTP DELETE on {0}", BizMsgServiceUrl + testID);
            Console.ReadLine();
            HttpDeleteOnService();
        }


        // HTTP GET
        private static void HttpGetOnServiceReadAsString()
        {
            // client.DefaultHeaders.Authorization = new Credential("18f34d01-c345-4959-9ab1-7db6ac5433cd");
            // client.DefaultHeaders.Add("CustomAuthHeader", "18f34d01-c345-4959-9ab1-7db6ac5433cd");           // for a custom auth header...
            // client.TransportSettings.Credentials = new NetworkCredential("boris", "pass");                   // win || basic 

            using (HttpResponseMessage response = client.Get(BizMsgServiceUrl))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.OK); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }

                string data = response.Content.ReadAsString();
                Console.WriteLine("{0}", data);
            }
        }

        private static void HttpGetOnServiceReadAsXElement()
        {
            // client.TransportSettings.Credentials = new NetworkCredential("boris", "pass");
            using (HttpResponseMessage response = client.Get(BizMsgServiceUrl))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.OK); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }

                ProcessMessageAsXElement(response.Content.ReadAsXElement());
            }
        }

        private static void ProcessMessageAsXElement(XElement root)
        {
            Console.WriteLine();

            foreach (XElement child in root.Elements())
            {
                Console.WriteLine("Message Type: {0}", child.Name.LocalName);

                foreach (var item in child.Elements())
                    Console.WriteLine("{0}: {1}", item.Name.LocalName, item.Value);

                Console.WriteLine();
            }
        }


        // HTTP POST
        private static void HttpPostOnService()
        {
            // client.TransportSettings.Credentials = new NetworkCredential("boris", "pass");
            HttpContent httpContent = HttpContent.Create(postData, "application/xml");

            using (HttpResponseMessage response = client.Post(BizMsgServiceUrl, httpContent))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.Created); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }

                // string data = "Created - " + response.Content.ReadAsString();
                // Console.WriteLine("{0}", data);
            }
        }

        // HTTP PUT
        private static void HttpPutOnService()
        {
            HttpContent httpContent = HttpContent.Create(putData, "application/xml");

            using (HttpResponseMessage response = client.Put(BizMsgServiceUrl + testID, httpContent))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.Accepted); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }
            }
        }

        // HTTP DELETE
        private static void HttpDeleteOnService()
        {
            using (HttpResponseMessage response = client.Delete(BizMsgServiceUrl + testID))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.OK); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }

                Console.WriteLine("Deleted.");
            }
        }



        static string postData = @"<BizMsg xmlns='http://schemas.datacontract.org/2004/07/NiCris.BusinessObjects'>
                                    <Date>2001-01-01</Date> 
                                    <Name>XYZ</Name> 
                                    <User>Boris</User> 
                                   </BizMsg>";

        static string putData = @"<BizMsg xmlns='http://schemas.datacontract.org/2004/07/NiCris.BusinessObjects'>
                                    <Date>2002-02-02</Date> 
                                    <Name>XYZ-UPD</Name> 
                                    <User>Boris-UPD</User> 
                                  </BizMsg>";

    }

    public class MyHttpStage : HttpProcessingStage
    {
        public override void ProcessRequest(HttpRequestMessage request)
        {
            Console.WriteLine("ProcessRequest called: {0} {1}", request.Method, request.Uri);
        }

        public override void ProcessResponse(HttpResponseMessage response)
        {
            Console.WriteLine("ProcessResponse called: {0}", response.StatusCode);
        }
    }
}
