﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

using KenticoCloud.ContentManagement.Modules.HttpClient;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace KenticoCloud.ContentManagement.Tests.Mocks
{
    public class FileSystemHttpClientMock : IContentManagementHttpClient
    {
        private bool _saveToFileSystem;

        private IContentManagementHttpClient _nativeClient = new ContentManagementHttpClient();

        public FileSystemHttpClientMock(bool saveToFileSystem)
        {
            _saveToFileSystem = saveToFileSystem;
        }


        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            var serializedRequest = JsonConvert.SerializeObject(message);
            var serializedRequestContent = await SerializeContent(message.Content);

            var folderPath = GetMockFileFolder(message, serializedRequest + serializedRequestContent);

            Directory.CreateDirectory(folderPath);

            if (_saveToFileSystem)
            {
                var response = await _nativeClient.SendAsync(message);

                File.WriteAllText(Path.Combine(folderPath, "request.json"), serializedRequest);
                File.WriteAllText(Path.Combine(folderPath, "request_content.json"), serializedRequestContent);

                var serializedResponse = JsonConvert.SerializeObject(response);
                var serializedResponseContent = await SerializeContent(response.Content);

                File.WriteAllText(Path.Combine(folderPath, "response.json"), serializedResponse);
                File.WriteAllText(Path.Combine(folderPath, "response_content.json"), serializedResponseContent);

                return response;
            }
            else
            {
                // TODO - Handle deserialization better, it now hangs on 
                var serializedResponse = File.ReadAllText(Path.Combine(folderPath, "response.json"));
                var serializedResponseContent = File.ReadAllText(Path.Combine(folderPath, "response_content.json"));

                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new IgnoreHttpContentContractResolver()
                };
                var response = JsonConvert.DeserializeObject<HttpResponseMessage>(serializedResponse, serializerSettings);
                response.Content = new StringContent(serializedResponseContent);

                return response;
            }
        }

        private async Task<string> SerializeContent(HttpContent content)
        {
            if (content == null)
            {
                return null;
            }

            return await content.ReadAsStringAsync();
        }

        private string GetMockFileFolder(HttpRequestMessage message, string serializedRequest)
        {
            var rootPath = Path.Combine(AppContext.BaseDirectory, "Data\\Requests");
            var endpointPath = Path.Combine(rootPath, message.RequestUri.PathAndQuery.Replace("/", "\\").Replace("?", "\\").TrimStart('\\'));

            var md5 = MD5.Create();
            var messageHash = md5.ComputeHash(Encoding.UTF8.GetBytes(serializedRequest));
            var stringMessageHash = Convert.ToBase64String(messageHash).Replace("/", "-");

            var uniqueRequestPath = Path.Combine(endpointPath, $"{message.Method}_{stringMessageHash}");

            return uniqueRequestPath;
        }
    }
}
