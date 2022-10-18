using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BlobStorage.Domain.Enums;
using BlobStorage.Domain.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text;

namespace BlobStorage.Domain.Helpers
{

    public static class Global_Helper
    {
        public static string GetAzureBlobStorageConnStrFromAppSetings(AppEnvironment myEnv)
        {
            
            string filename = "";

            switch (myEnv)
            {
                case AppEnvironment.Production:
                    filename = "appsettings.Production.json";
                    break;
                case AppEnvironment.Staging:
                    filename = "appsettings.Staging.json";
                    break;
                case AppEnvironment.Dev:
                    filename = "appsettings.Development.json";
                    break;
                default:
                    filename = "appsettings.Production.json";
                    break;
            }

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(filename);
            var configuration = builder.Build();
            string result = configuration["AzureBlobStorage:ConnectionString"];
            return result;
        }

        public static string GetAzureBlobStorageConnStrFromAzureKeyVault(AppEnvironment myEnv)
        {
            string result = "";
            try
            {


                string kvUri = "";

               var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
             

                KeyVaultSecret secret = client.GetSecret("AzureBlobStorageConString");
                result = secret.Value;


            }
            catch (Exception ex)
            {

                Global_Helper.SendEmailAsync("GetAzureBlobStorageConnStrFromAzureKeyVault" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }

            return result;
        }

        public static string GetAzureBlobStorageAPIAuthPasswordFromAzureKeyVault(AppEnvironment myEnv)
        {
            string result = "";
            try
            {

                string kvUri = "";
               
                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
         

                KeyVaultSecret secret = client.GetSecret("BlobStorageAPIAuthPassword");
                result = secret.Value;

            }
            catch (Exception ex)
            {

                Global_Helper.SendEmailAsync("GetAzureBlobStorageAPIAuthPasswordFromAzureKeyVault" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace.ToString() + "\r\n" + ex.InnerException.Message.ToString());
            }

            return result;
        }

        public static Dictionary<string, string> CreateTags(string container, string filename)
        {
            Dictionary<string, string> tags_result = null;
            switch (container)
            {
                case "contracts":
                    Dictionary<string, string> tags = new Dictionary<string, string>
                    {
                        { "Type", filename.GetPolicyTypeBySymbol() + " Policy" },
                        { "Priority", "1" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags;
                    break;
                case "receipts":
                    Dictionary<string, string> tags_r = new Dictionary<string, string>
                    {
                        { "Type", filename.GetPolicyTypeBySymbol() + " Receipt" },
                        { "Priority", "1" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_r;
                    break;
                case "covernotes":
                    Dictionary<string, string> tags_c = new Dictionary<string, string>
                    {
                        { "Type", "Cover Note" },
                        { "Priority", "1" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_c;
                    break;
                case "claims":
                    Dictionary<string, string> tags_cl = new Dictionary<string, string>
                    {
                        { "Type", "Claims File" },
                        { "Priority", "1" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_cl;
                    break;
                case "sales":
                    Dictionary<string, string> tags_sl = new Dictionary<string, string>
                    {
                        { "Type", "Sales File" },
                        { "Priority", "1" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_sl;
                    break;
                case "archiveinli":
                    Dictionary<string, string> tags_ai = new Dictionary<string, string>
                    {
                        { "Type", "kuznos Archive DB Files" },
                        { "Priority", "1" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_ai;
                    break;
                case "archivegnomon":
                    Dictionary<string, string> tags_ag = new Dictionary<string, string>
                    {
                        { "Type", "GNOMON Files" },
                        { "Priority", "2" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_ag;
                    break;
                case "filestemp":
                    Dictionary<string, string> tags_ft = new Dictionary<string, string>
                    {
                        { "Type", "Files Temp" },
                        { "Priority", "2" },
                        { "Sealed", "true" }
                    };
                    tags_result = tags_ft;
                    break;
                default:
                    Dictionary<string, string> tags_def = new Dictionary<string, string>
                    {
                        { "Type", "Not Set" },
                        { "Priority", "3" },
                        { "Sealed", "false" }
                    };
                    tags_result = tags_def;
                    break;
            }

            return tags_result;
        }
        public static Dictionary<string, string> CreateMetadata(string container, string filename)
        {
            Dictionary<string, string> tags_result = null;
            switch (container)
            {
                case "contracts":
                    Dictionary<string, string> tags = new Dictionary<string, string>
                    {
                        { "Category", "Critical" }
                    };
                    tags_result = tags;
                    break;
                case "filestemp":
                    Dictionary<string, string> tmp = new Dictionary<string, string>
                    {
                        { "Category", "Default" }
                    };
                    tags_result = tmp;
                    break;

                case "receipts":
                    Dictionary<string, string> tags_r = new Dictionary<string, string>
                    {
                        { "Category", "Critical" }
                    };
                    tags_result = tags_r;
                    break;
                case "covernotes":
                    Dictionary<string, string> tags_c = new Dictionary<string, string>
                    {
                        { "Category", "Critical" }
                    };
                    tags_result = tags_c;
                    break;
                case "claims":
                    Dictionary<string, string> tags_cl = new Dictionary<string, string>
                    {
                        { "Category", "Critical" }
                    };
                    tags_result = tags_cl;
                    break;
                case "sales":
                    Dictionary<string, string> tags_sl = new Dictionary<string, string>
                    {
                        { "Category", "Critical" }
                    };
                    tags_result = tags_sl;
                    break;
                case "archiveinli":
                    Dictionary<string, string> tags_ai = new Dictionary<string, string>
                    {
                        { "Category", "Critical" }
                    };
                    tags_result = tags_ai;
                    break;
                case "archivegnomon":
                    Dictionary<string, string> tags_ag = new Dictionary<string, string>
                    {
                        { "Category", "Default" }
                    };
                    tags_result = tags_ag;
                    break;
                default:
                    Dictionary<string, string> tags_def = new Dictionary<string, string>
                    {
                        { "Category", "Default" }
                    };
                    tags_result = tags_def;
                    break;
            }

            return tags_result;
        }

        public static bool SendEmailAsync(string errorMessage)
        {
            var result = false;
            AzSendRequest sendRequest = new AzSendRequest();

            string url = "http://mail.inli.gr/api/v1/AzEmailOps/SendEmailAsync";

            try
            {
                sendRequest.sender = "vkuznos@kuznos.gr";
                sendRequest.distributionGroup = "dev@kuznos.gr";
                sendRequest.recipients = new List<string>()
                                                {
                                                "dev@kuznos.gr",
                                                };
                sendRequest.subject = "Azure Blob Storage library Error";
                sendRequest.body = errorMessage;
                sendRequest.saveToSentItems = false;
              
                Uri api_uri = new Uri(url);
                WebRequest webRequest = WebRequest.Create(api_uri);
                string sb = JsonConvert.SerializeObject(sendRequest);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.Headers["ApiKey"] = "AzEmailKey";
                byte[] bt = Encoding.UTF8.GetBytes(sb);
                Stream st = webRequest.GetRequestStream();
                st.Write(bt, 0, bt.Length);
                st.Close();
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    AzResult azRes = new AzResult();
                    Stream stream1 = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream1);
                    string resp = sr.ReadToEnd();
                    azRes = (AzResult)JsonConvert.DeserializeObject(resp, typeof(AzResult));
                    if (azRes.status == Status.Succeed)
                    {
                        result = true;
                    };
                }
            }
            catch (WebException ex)
            {
                result = false;
                HttpWebResponse oo = (HttpWebResponse)ex.Response;
                using (StreamReader rd = new StreamReader(oo.GetResponseStream()))
                {
                    string errorString = rd.ReadToEnd();
                    //SendMail_Error(Env, ex.Message + Constants.vbCrLf + "Request:" + url + Constants.vbCrLf + "ERROR:" + errorString);
                }
            }
            catch (Exception)
            {
                //SendMail_Error(Env, ex1.Message + Constants.vbCrLf + "Request:" + url);
            }

            return result;
        }

    }
    public enum Status
    {
        Failed = 0,
        Succeed = 1
    }
    public class AzSendRequest
    {
        [Required]
        public string subject { get; set; }
        [Required]
        public string body { get; set; }
        [Required]
        public string sender { get; set; }
        [Required]
        public string distributionGroup { get; set; }
        [Required]
        public List<string> recipients { get; set; }
        public List<AzFileAttachment> attachments { get; set; }
        [Required]
        public bool saveToSentItems { get; set; }

    }
    public class AzFileAttachment
    {
        public string Base64ContentBytes { get; set; }
        [Required]
        public string Filename { get; set; }
        [Required]
        public byte[] ContentBytes { get; set; }
    }
    public class AzResult
    {
        public Status status { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get { return DateTime.Now; } }
    }
}
