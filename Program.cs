using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.VisualBasic.FileIO;

namespace MortgageOneTimeMigration
{

    class Program
    {
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            /*
            soapEnvelopeDocument.LoadXml(
            @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
               xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">
        <SOAP-ENV:Body>
            <DP_GetEncryptionKey xmlns=""http://tempuri.org/"" 
                SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">                
            </DP_GetEncryptionKey>
        </SOAP-ENV:Body>
    </SOAP-ENV:Envelope>");
            */
            soapEnvelopeDocument.LoadXml(
          @"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
              <soap:Body>
                <DP_GetEncryptionKey xmlns=""http://tempuri.org/"">
                  <EncryptionKey>string</EncryptionKey>
                  <error>string</error>
                </DP_GetEncryptionKey>
              </soap:Body>
            </soap:Envelope>");
            return soapEnvelopeDocument;
        }

        private static XmlDocument CreateSoapEnvelope_EncryptText(string text, string key)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();

            string strEnvelope = @"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body><EncryptText xmlns=""http://tempuri.org/"">";
            strEnvelope = strEnvelope + "<plainText>" + text + "</plainText>";
            strEnvelope = strEnvelope + "<passPhrase>" + key + "</passPhrase>";
            strEnvelope = strEnvelope + "</EncryptText></soap:Body></soap:Envelope>";

            soapEnvelopeDocument.LoadXml(strEnvelope);
            return soapEnvelopeDocument;
        }

        
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        /*
        public static HttpWebRequest CreateSOAPWebRequest()
        {
            var directory = Directory.GetCurrentDirectory();

            var path = directory + @"\WS_URL.txt";

            var URL="";

            using (var reader = new StreamReader(path))
            {                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    URL = line;                  
                }
            }

           // var URL_GetKey = URL + "DP_GetEncryptionKey";


            //Making Web Request    
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(URL);
            //SOAPAction    
            Req.Headers.Add(@"SOAPAction:http://tempuri.org/Addition");
            //Content_type    
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            //HTTP method    
            Req.Method = "POST";
            //return HttpWebRequest    
            return Req;
        }
        */
        /*
        public static string InvokeServiceGetKey()
        {
            string output="";
            //Calling CreateSOAPWebRequest method    
            HttpWebRequest request = CreateSOAPWebRequest();

            XmlDocument SOAPReqBody = new XmlDocument();
            //SOAP Body Request    

            
            var someString = String.Join(
                Environment.NewLine,
                "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:tem=\"http://tempuri.org/\"",
                "< soapenv:Header />< soapenv:Body >< tem:DP_GetEncryptionKey >< !--Optional:-->< tem:EncryptionKey >?</ tem : EncryptionKey >< !--Optional:-->",
                "< tem:error >?</ tem : error ></ tem:DP_GetEncryptionKey ></ soapenv:Body ></ soapenv:Envelope > "
                );
            
            var someString = @"< tem:DP_GetEncryptionKey > < !--Optional:-->< tem:EncryptionKey >?</ tem : EncryptionKey >   < !--Optional:-->    < tem:error >?</ tem : error >     </ tem:DP_GetEncryptionKey >";

            SOAPReqBody.LoadXml(someString);                

             using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            //Geting response from request    
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    var ServiceResult = rd.ReadToEnd();
                    output = output + ServiceResult;
                    //writting stream result on console    
                    //Console.WriteLine(ServiceResult);
                    Console.ReadLine();
                }
            }
            return output;
        }
        */

        static void Main(string[] args)
        {
            try
            {                
                var directory = Directory.GetCurrentDirectory();

                var path = directory + @"\WS_URL.txt";
                var DBConfigpath = directory + @"\DB_Config.txt";

                var URL = "";
                var DBServer = "";
                var DBName = "";
                var DBUserName = "";
                var DBUserPassword = "";

                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        URL = line;
                    }
                }

                using (var reader = new StreamReader(DBConfigpath))
                {
                    //while (!reader.EndOfStream)
                    //{
                    var line = reader.ReadLine();
                    DBServer = line;
                    line = reader.ReadLine();
                    DBName = line;
                    line = reader.ReadLine();
                    DBUserName = line;
                    line = reader.ReadLine();
                    DBUserPassword = line;
                    //}
                }

                var _url = URL;              
                var _action = "http://tempuri.org/DP_GetEncryptionKey";

                XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
                HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                string soapResult;
                string key = "";

                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(soapResult);

                        XmlNodeList elemList = doc.GetElementsByTagName("EncryptionKey");
                        key= elemList[0].InnerXml.ToString();

                    }                    
                }
                
                var csvpath = directory + @"\cramg.csv";

                

                //var path = @"C:\Person.csv"; // Habeeb, "Dubai Media City, Dubai"
                /*
                using (TextFieldParser csvParser = new TextFieldParser(csvpath))
                {
                    csvParser.CommentTokens = new string[] { "#" };
                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;

                    // Skip the row with the column names
                    csvParser.ReadLine();

                    while (!csvParser.EndOfData)
                    {
                        // Read current line fields, pointer moves to the next line.
                        string[] fields = csvParser.ReadFields();
                        string Name = fields[0];
                        string Address = fields[1];
                    }
                }
                */

                using (TextFieldParser csvParser = new TextFieldParser(csvpath))
                //using (var reader = new StreamReader(csvpath))
                {
                    //List<string> listID = new List<string>();
                    //List<string> listName = new List<string>();
                    //List<string> listPassword = new List<string>();

                    csvParser.CommentTokens = new string[] { "#" };
                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;

                    // Skip the row with the column names
                    csvParser.ReadLine();

                    int index = 1;

                    //while (!reader.EndOfStream)
                    while (!csvParser.EndOfData)
                    {
                        //var line = reader.ReadLine();
                        //var values = line.Split(',');

                        //string id = values[0];
                       // string name = values[1];
                        //string password = values[2];
                        //listID.Add(values[0]);
                        //listName.Add(values[1]);
                        //listPassword.Add(values[2]);

                        string[] fields = csvParser.ReadFields();
                        string id = fields[0];
                        string name = fields[1];
                        string password = fields[2];

                        _action = "http://tempuri.org/EncryptText";
                        XmlDocument soapEnvelopeXml2 = CreateSoapEnvelope_EncryptText(password, key);
                        HttpWebRequest webRequest2 = CreateWebRequest(_url, _action);
                        InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml2, webRequest2);

                        // begin async call to web request.
                        IAsyncResult asyncResult2 = webRequest2.BeginGetResponse(null, null);

                        asyncResult2.AsyncWaitHandle.WaitOne();

                        string EncryptedPassword = "";

                        using (WebResponse webResponse2 = webRequest2.EndGetResponse(asyncResult2))
                        {
                            using (StreamReader rd = new StreamReader(webResponse2.GetResponseStream()))
                            {
                                soapResult = rd.ReadToEnd();

                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(soapResult);

                                XmlNodeList elemList = doc.GetElementsByTagName("EncryptTextResult");
                                EncryptedPassword = elemList[0].InnerXml.ToString();

                                string connstr = @"Data Source=" + DBServer + ";Initial Catalog=" + DBName + ";Persist Security Info=True;User ID=" + DBUserName + ";Password=" + DBUserPassword;
                                
                                SqlConnection conn = null;
                                SqlDataAdapter sqlDA = null;
                                conn = new SqlConnection(connstr);

                                sqlDA = new SqlDataAdapter();
                                sqlDA.SelectCommand = new SqlCommand("insert into [SQLSolicitor] values ('"+ id + "','"+ EncryptedPassword + "','Active','"+name+ "',getdate(),getdate(),'system','system',NULL,'Yes',getdate(),getdate(),NULL)", conn);

                                DataSet ds = new DataSet("ds");
                                sqlDA.Fill(ds);

                                Console.WriteLine(index +":" + id + "added\n");

                                index = index + 1;

                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;

            }
            
        }
    }
}
