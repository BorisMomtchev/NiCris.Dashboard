using System;
using System.Net;
using System.Xml.Linq;
using Microsoft.Http;

namespace MessageTester
{
    class Programo_old
    {
        // ServiceAuthKey demo
        //static string ServiceAuthKeyUri = "http://localhost/Xata/ServiceAuthKey.svc/GetData";
        //static string JsonServiceAuthKeyUri = ServiceAuthKeyUri + "?json";

        // ServiceBasicAuth demo
        //static string ServiceBasicAuthUri = "http://localhost/Xata/ServiceBasicAuth.svc/User";

        // AVL & HOS services - Secured
        static string AVLServiceUri = "http://localhost/XataSecure/Avl/";  // Via an Auth Token/Key
        static string HOSServiceUri = "http://localhost/XataSecure/Hos/";  // Custom Basic Auth

        const string testID = "55555555-787b-4b9d-91d8-51bed90f8e90";

        // Init our Http Client...
        static HttpClient client = new HttpClient();

        static void MainEx(string[] args)
        {
            client.Stages.Add(new MyHttpStage());

            Console.WriteLine("Press enter when ready to issue an HTTP GET on {0}", AVLServiceUri);
            Console.ReadLine();
            HttpGetOnServiceAVLBasicAuth();

            Console.WriteLine("\nPress enter when ready to issue an HTTP GET on {0} \n(Demonstrates the ReadAsXElement method)", HOSServiceUri);
            Console.ReadLine();
            HttpGetOnServiceHOSBasicAuth();

            // Create
            Console.WriteLine("\nPress enter when ready to insert a new AVL record (HTTP POST) on {0}", AVLServiceUri);
            Console.ReadLine();
            HttpPostOnServiceAVLBasicAuth();

            // Update
            Console.WriteLine("\nPress enter when ready to update an the new AVL record (HTTP PUT) on {0}", AVLServiceUri + testID);
            Console.ReadLine();
            HttpPutOnServiceAVLBasicAuth();

            // Delete
            Console.WriteLine("\nPress enter when ready to delete the updated AVL record (HTTP DELETE) on {0}", AVLServiceUri + testID);
            Console.ReadLine();
            HttpDeleteOnServiceAVLAuthKey();
        }

        private static void HttpGetOnServiceAVLBasicAuth()
        {
            //client.DefaultHeaders.Authorization = new Credential("18f34d01-c345-4959-9ab1-7db6ac5433cd");
            //client.DefaultHeaders.Add("XataAuthHeader", "18f34d01-c345-4959-9ab1-7db6ac5433cd");      // for a custom auth header...
            client.TransportSettings.Credentials = new NetworkCredential("boris", "xata");

            using (HttpResponseMessage response = client.Get(AVLServiceUri))
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

        private static void HttpGetOnServiceHOSBasicAuth()
        {
            client.TransportSettings.Credentials = new NetworkCredential("boris", "xata");

            using (HttpResponseMessage response = client.Get(HOSServiceUri))
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

        private static void HttpPostOnServiceAVLBasicAuth()
        {
            //client.TransportSettings.Credentials = null;  // - to reset
            client.TransportSettings.Credentials = new NetworkCredential("boris", "xata");
            //client.DefaultHeaders.Authorization = new Credential("18f34d01-c345-4959-9ab1-7db6ac5433cd");

            HttpContent httpContent = HttpContent.Create(postData, "application/xml");

            using (HttpResponseMessage response = client.Post(AVLServiceUri, httpContent))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.Created); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }

                string data = "Created - " + response.Content.ReadAsString();
                Console.WriteLine("{0}", data);
            }
        }

        private static void HttpPutOnServiceAVLBasicAuth()
        {
            //client.DefaultHeaders.Authorization = new Credential("18f34d01-c345-4959-9ab1-7db6ac5433cd");
            client.TransportSettings.Credentials = new NetworkCredential("boris", "xata");
            HttpContent httpContent = HttpContent.Create(putData, "application/xml");

            using (HttpResponseMessage response = client.Put(AVLServiceUri + testID, httpContent))
            {
                // Throws an exception if not OK
                try { response.EnsureStatusIs(HttpStatusCode.Accepted); }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.Message);
                    response.Dispose();
                    return;
                }

                string data = "Updated - " + response.Content.ReadAsString();
                Console.WriteLine("{0}", data);
            }
        }

        private static void HttpDeleteOnServiceAVLAuthKey()
        {
            //client.DefaultHeaders.Authorization = new Credential("18f34d01-c345-4959-9ab1-7db6ac5433cd");
            client.TransportSettings.Credentials = new NetworkCredential("boris", "xata");

            using (HttpResponseMessage response = client.Delete(AVLServiceUri + testID))
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

        // PLEASE MAKE SURE THAT postData & putData MATCH THE LAST SCHEMA FIRST...
        static string postData =
            @"<AVL xmlns='http://schemas.datacontract.org/2004/07/Xata.Mobile.BusinessObjects' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>
                <ArrivalDeparture i:nil='true' /> 
                <City i:nil='true' /> 
                <CompanyID>CompanyID:HttpClient_CREATED</CompanyID> 
                <DateTime>2010-11-04T15:26:53.98</DateTime> 
                <Direction i:nil='true' /> 
                <Distance i:nil='true' /> 
                <GroupID>GroupID:0</GroupID> 
                <ID>55555555-787b-4b9d-91d8-51bed90f8e90</ID> 
                <Latitude>45.106349000000000</Latitude> 
                <Longtitude>-93.249335000000000</Longtitude> 
                <MTAvailability i:nil='true' /> 
                <MTSateFlag i:nil='true' /> 
                <MotionStatus i:nil='true' /> 
                <Odometer i:nil='true' /> 
                <PositionAccuracy i:nil='true' /> 
                <Proximity i:nil='true' /> 
                <Speed i:nil='true' /> 
                <State i:nil='true' /> 
                <VehicleID>VehicleID:0</VehicleID> 
                <VehicleIgnition i:nil='true' /> 
            </AVL>";

        static string putData =
            @"<AVL xmlns='http://schemas.datacontract.org/2004/07/Xata.Mobile.BusinessObjects' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>
                <ArrivalDeparture i:nil='true' /> 
                <City i:nil='true' /> 
                <CompanyID>CompanyID:HttpClient_UPDATED</CompanyID> 
                <DateTime>2010-11-04T15:26:53.98</DateTime> 
                <Direction i:nil='true' /> 
                <Distance i:nil='true' /> 
                <GroupID>GroupID:0</GroupID> 
                <ID>55555555-787b-4b9d-91d8-51bed90f8e90</ID> 
                <Latitude>45.106349000000000</Latitude> 
                <Longtitude>-93.249335000000000</Longtitude> 
                <MTAvailability i:nil='true' /> 
                <MTSateFlag i:nil='true' /> 
                <MotionStatus i:nil='true' /> 
                <Odometer i:nil='true' /> 
                <PositionAccuracy i:nil='true' /> 
                <Proximity i:nil='true' /> 
                <Speed i:nil='true' /> 
                <State i:nil='true' /> 
                <VehicleID>VehicleID:0</VehicleID> 
                <VehicleIgnition i:nil='true' /> 
            </AVL>";
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
