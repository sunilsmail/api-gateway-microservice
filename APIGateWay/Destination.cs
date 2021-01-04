﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APIGateWay
{
    public class Destination
    {
        public string Path { get; set; }
        public bool RequiresAuthentication { get; set; }
        static HttpClient client = new HttpClient();
        public Destination(string uri, bool requiresAuthentication)
        {
            Path = uri;
            RequiresAuthentication = requiresAuthentication;
        }

        public Destination(string path)
            : this(path, false)
        {
        }

        private Destination()
        {
            Path = "/";
            RequiresAuthentication = false;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequest request)
        {
            string requestContent;
            using (Stream receiveStream = request.Body)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    requestContent = readStream.ReadToEnd();
                }
            }

            using (var newRequest = new HttpRequestMessage(new HttpMethod(request.Method), CreateDestinationUri(request)))
            {
                newRequest.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);
                //using (var response = await client.SendAsync(newRequest))
                //{
                //    return response;
                //}
                var response = await client.SendAsync(newRequest);
                return response;
            }
        }

        private string CreateDestinationUri(HttpRequest request)
        {
            string requestPath = request.Path.ToString();
            string queryString = request.QueryString.ToString();

            string endpoint = "";
            string[] endpointSplit = requestPath.Substring(1).Split('/');

            if (endpointSplit.Length > 1)
                endpoint = endpointSplit[1];


            return Path + endpoint + queryString;
        }

    }
}
