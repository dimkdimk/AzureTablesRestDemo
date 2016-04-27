using System;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace AzureTablesRestDemo
{

    public static class AzureTableHelper
    {
        /// <summary>
        /// Requests JSON Data from Azure Table  using Shared Key Lite Authentication
        /// </summary>
        /// <param name="storageAccount"> Azure Storage Account</param>
        /// <param name="accessKey">access key from the management portal</param>
        /// <param name="resourcePath">name of an existing Azure Table in this storage account</param>
        /// <param name="jsonData">[out] JSON object representing the new entity </param>
        /// <returns>Web Response code</returns>
        public static int RequestResource(string storageAccount, string accessKey, string resourcePath, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath;

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            request.Accept = "application/json;odata=nometadata";
            request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2015-04-05");
            request.Headers.Add("Accept-Charset", "UTF-8");
            request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
            request.Headers.Add("DataServiceVersion", "1.0;NetFx");


            // Signature string for  Shared Key Lite Authentication must be in the form
            // StringToSign = Date + "\n" + CanonicalizedResource
            // Date 
            string stringToSign = request.Headers["x-ms-date"] + "\n";

            // Canonicalized Resource in the format  /{0}/{1} where 0 is name of the account and 1 is resources URI path
            // remove the query string
            int query = resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }
            stringToSign += "/" + storageAccount + "/" + resourcePath;

            // Hash-based Message Authentication Code (HMAC) using SHA256 hash
            System.Security.Cryptography.HMACSHA256 hasher = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(accessKey));

            // Authorization header
            string strAuthorization = "SharedKeyLite " + storageAccount + ":" + System.Convert.ToBase64String(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringToSign)));

            // Add the Authorization header to the request
            request.Headers.Add("Authorization", strAuthorization);


            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }


        /// <summary>
        /// Inserts Azure Table JSON entity using Shared Key Lite Authentication
        /// </summary>
        /// <param name="storageAccount"> Azure Storage Account</param>
        /// <param name="accessKey">access key from the management portal</param>
        /// <param name="tableName">name of an existing Azure Table in this storage account</param>
        /// <param name="jsonData">JSON object representing the new entity </param>
        /// <returns>Web Response code</returns>
        public static int InsertEntity(string storageAccount, string accessKey, string tableName, string jsonData)
        {
            string host = string.Format(@"https://{0}.table.core.windows.net/", storageAccount);
            string resource = string.Format(@"{0}", tableName);
            string uri = host + resource;

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = jsonData.Length;
            request.Accept = "application/json;odata=nometadata";
            request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2015-04-05");
            request.Headers.Add("Accept-Charset", "UTF-8");
            request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
            request.Headers.Add("DataServiceVersion", "1.0;NetFx");


            // Signature string for  Shared Key Lite Authentication must be in the form
            // StringToSign = Date + "\n" + CanonicalizedResource
            // Date 
            string stringToSign = request.Headers["x-ms-date"] + "\n";

            // Canonicalized Resource in the format  /{0}/{1} where 0 is name of the account and 1 is resources URI path
            stringToSign += "/" + storageAccount + "/" + resource;

            // Hash-based Message Authentication Code (HMAC) using SHA256 hash
            System.Security.Cryptography.HMACSHA256 hasher = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(accessKey));

            // Authorization header
            string strAuthorization = "SharedKeyLite " + storageAccount + ":" + System.Convert.ToBase64String(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringToSign)));

            // Add the Authorization header to the request
            request.Headers.Add("Authorization", strAuthorization);

            // Write Entity's JSON data into the request body
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    string res = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }



        private static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
