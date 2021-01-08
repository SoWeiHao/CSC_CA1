using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Clarifai.Channels;
using Clarifai.Api;
using Grpc.Core;
using Google.Protobuf;

namespace Task_7.Controllers
{
    static class Keys
    {
        public const string ENVIRONMENT_URL = "";
        public const string USERNAME = "";
        public const string API_KEY = "";
        public const string CLIENT_ID = "";
    }

    public class ImageRecognitionController : ApiController
    {
        [HttpPost]
        [Route("api/imageRecognition/{fileName}/{fileExtension}")]
        public string ImageRecognition(string fileName, string fileExtension)
        {
            var response = PostModelOutputsWithFileBytes(fileName, fileExtension);
            return response;
        }

        public string PostModelOutputsWithFileBytes(string fileName, string fileExtension)
        {
            ByteString bytes;

            string path = @"D:\"+fileName+"."+fileExtension;
            using (FileStream stream = File.OpenRead(path))
            {
                bytes = ByteString.FromStream(stream);
            }

            var client = new V2.V2Client(ClarifaiChannel.Grpc());

            var metadata = new Metadata
            {
                { "Authorization", "Key"}
            };

            var response = client.PostModelOutputs(
                new PostModelOutputsRequest()
                {
                    ModelId = "",
                    Inputs =
                    {
                        new List<Input>()
                        {
                            new Input()
                            {
                                Data = new Data()
                                {
                                    Image = new Image()
                                    {
                                        Base64 = bytes
                                    }
                                }
                            }
                        }
                    }
                },
                metadata
            );
            if (response.Status.Code != Clarifai.Api.Status.StatusCode.Success)
                throw new Exception("Request failed, response: " + response);

            Console.WriteLine("Predicted concepts:");
            string output = "";
            foreach (var concept in response.Outputs[0].Data.Concepts)
            {
                output += concept.Name+"\n, ";
                Console.WriteLine($"{concept.Name,15} {concept.Value:0.00}");
            }
            return output;
        }

        [HttpPost]
        [Route("api/receiptRecognition/{fileName}/{fileExtension}")]
        public string ReceiptRecognition(string fileName, string fileExtension)
        {
            var filePath = @"D:\" + fileName + "." + fileExtension;
            var response = ProcessDocumentBase64(filePath);
            return response;
        }
        public string ProcessDocumentBase64(string filePath)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + Keys.ENVIRONMENT_URL + "/api/v7/partner/documents/");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "apikey " + Keys.USERNAME + ":" + Keys.API_KEY);
                httpWebRequest.Headers.Add("Client-id", Keys.CLIENT_ID);
                Byte[] bytes = File.ReadAllBytes(filePath);
                String fileData = Convert.ToBase64String(bytes);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"file_name\":\"download.jpg\"," +
                                   "\"file_data\":\"" + fileData + "\"}";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var jsonResponse = streamReader.ReadToEnd();
                    Console.WriteLine(String.Format("Response: {0}", jsonResponse));
                    return jsonResponse;
                }
            }
            catch
            {
                var error = "Error, please try again";
                return error;
            }
        }
    }
}
