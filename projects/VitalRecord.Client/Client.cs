using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Net.Http;

namespace VR
{
    /// <summary>Abstract Client for handling BFDR and VRDR messages from the NVSS FHIR API</summary>
    public class Client
    {
        /// <summary>The API url</summary>
        ///TODO: should Url toggle based on message type? 
        /// TODO: does Url also need IG version now?
        public String Url { get; }
        /// <summary>Whether the client is running locally</summary>
        public bool LocalTesting { get; }
        /// <summary>The credentials to access the API server</summary>
        public Credentials Credentials { get; }
        /// <summary>The token to access the API server</summary>
        public string? Token { get; set; }
        /// <summary>Constructor</summary>
        private HttpClient client;
        /// <summary>Instantiate a new instance of the Client</summary>
        public Client(String url, bool local, Credentials credentials)
        {
            if (local)
            {
                // When testing locally we allow for self signed certificates
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                client = new HttpClient(handler);
            }
            else
            {
                client = new HttpClient();
            }
            this.Url = url;
            this.LocalTesting = local;
            this.Credentials = credentials;
        }
        /// <summary>GetMessageResponsesAsync makes a GET request to the NVSS FHIR API server for any new messages
        /// responses</summary>
        public async Task<HttpResponseMessage> GetMessageResponsesAsync(string? optionalPath = null)
        {
            // if testing against the NVSS FHIR API server, add the authentication token
            if (!this.LocalTesting)
            {
                if (String.IsNullOrEmpty(this.Token))
                {
                    HttpResponseMessage authResponse = await RefreshAccessTokenAsync();
                    // return authentication error
                    if (!authResponse.IsSuccessStatusCode)
                    {
                        return authResponse;
                    }
                }
            }

            string urlWithPath = CreateUrlWithPath(optionalPath);

            var response = await client.GetAsync(urlWithPath);

            // check if the token expired, refresh and try again
            if (!response.IsSuccessStatusCode)
            {
                HttpResponseMessage authRetry = await RefreshAccessTokenAsync();
                // return authentication error
                if (!authRetry.IsSuccessStatusCode)
                {
                    return authRetry;
                }
                response = await client.GetAsync(urlWithPath);
            }

            return response;
        }

        /// <summary>PostMessageAsync POSTS a single message to the NVSS FHIR API server for processing</summary>
        /// TODO: replace BaseMessage with CommonMessage !!!
        public async Task<HttpResponseMessage> PostMessageAsync(CommonMessage message, string? optionalPath = null)
        {

            var json = message.ToJSON();
            var data = new StringContent(json, Encoding.UTF8, "application/json");



            // if testing against the NVSS FHIR API server, add the authentication token
            if (!this.LocalTesting)
            {
                if (String.IsNullOrEmpty(this.Token))
                {
                    HttpResponseMessage authResponse = await RefreshAccessTokenAsync();
                    // return authentication error
                    if (!authResponse.IsSuccessStatusCode)
                    {
                        return authResponse;
                    }
                }
            }
            var response = client.PostAsync(CreateUrlWithPath(optionalPath), data).Result;

            // check if the token expired, refresh and try again
            if (!response.IsSuccessStatusCode)
            {
                HttpResponseMessage authRetry = await RefreshAccessTokenAsync();
                // return authentication error
                if (!authRetry.IsSuccessStatusCode)
                {
                    return authRetry;
                }
                response = await client.PostAsync(CreateUrlWithPath(optionalPath), data);
            }

            return response;
        }

        private string CreateUrlWithPath(string? optionalPath)
        {
            Uri baseUri = new Uri(this.Url);
            Uri fullUri = baseUri;
            if (optionalPath != null)
            {
                fullUri = new Uri(baseUri, optionalPath);
            }
            return fullUri.ToString();
        }

        /// <summary>Create the payload for submission to the NVSS FHIR API for bulk upload</summary>
        public static string CreateBulkUploadPayload(IEnumerable<CommonMessage> messages, string url, bool prettyPrint = false)
        {
            Bundle payload = new Bundle();
            payload.Id = Guid.NewGuid().ToString();
            payload.Type = Bundle.BundleType.Batch;
            payload.Timestamp = DateTime.Now;
            foreach (CommonMessage message in messages)
            {
                Bundle.EntryComponent entry = new Bundle.EntryComponent();
                entry.Resource = (Bundle)message;
                entry.Request = new Bundle.RequestComponent();
                entry.Request.Method = Bundle.HTTPVerb.POST;
                entry.Request.Url = url;
                payload.Entry.Add(entry);
            }
            return payload.ToJson(new FhirJsonSerializationSettings { Pretty = prettyPrint, AppendNewLine = prettyPrint });
        }

        /// <summary>PostMessageAsync POSTS a list of messages to the NVSS FHIR API server for processing using bulk upload</summary>
        public async Task<List<HttpResponseMessage>> PostMessagesAsync(IEnumerable<CommonMessage> messages, int batchSize, string? optionalPath = null)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
            while (messages.Count() > 0)
            {
                IEnumerable<CommonMessage> batch = messages.Take(batchSize);
                List<HttpResponseMessage> batchResponses = await PostMessagesAsync(batch, optionalPath);
                responses.AddRange(batchResponses);
                messages = messages.Skip(batchSize);
            }
            return responses;
        }

        // PostMessageAsync POSTS a list of messages to the NVSS FHIR API server for processing using bulk upload
        // This method is private because it sends batches of arbirary size, the public method requires a batch size to be set
        private async Task<List<HttpResponseMessage>> PostMessagesAsync(IEnumerable<CommonMessage> messages, string? optionalPath = null)
        {

            string urlWithPath = CreateUrlWithPath(optionalPath);

            string json = CreateBulkUploadPayload(messages, urlWithPath);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            // if testing against the NVSS FHIR API server, add the authentication token
            if (!this.LocalTesting)
            {
                if (String.IsNullOrEmpty(this.Token))
                {
                    HttpResponseMessage authResponse = await RefreshAccessTokenAsync();
                    // return authentication error
                    if (!authResponse.IsSuccessStatusCode)
                    {
                        return Enumerable.Repeat(authResponse, messages.Count()).ToList();
                    }
                }
            }
            var response = client.PostAsync(urlWithPath, data).Result;

            // check if the token expired, refresh and try again
            if (!response.IsSuccessStatusCode)
            {
                HttpResponseMessage authRetry = await RefreshAccessTokenAsync();
                // return authentication error
                if (!authRetry.IsSuccessStatusCode)
                {
                    return Enumerable.Repeat(authRetry, messages.Count()).ToList();
                }
                response = await client.PostAsync(urlWithPath, data);
            }

            // At this point we have an overall HTTP response and, if the request succeeded, one internal response for each
            // message in the bulk submission. To process this we first check for success and, if the request did succeed,
            // break out the internal responses, otherwise return the overall response as the response for each message
            if (response.IsSuccessStatusCode)
            {
                // We should have a batch-response Bundle
                string content = response.Content.ReadAsStringAsync().Result;
                Bundle bundle = CommonMessage.ParseGenericBundle(content, true);
                if (bundle?.Type == Bundle.BundleType.BatchResponse)
                {
                    List<HttpResponseMessage> httpResponses = new List<HttpResponseMessage>();
                    foreach (var entry in bundle.Entry)
                    {
                        HttpResponseMessage responseMessage = new HttpResponseMessage();
                        responseMessage.StatusCode = (System.Net.HttpStatusCode)int.Parse(entry.Response.Status);
                        httpResponses.Add(responseMessage);
                    }
                    return httpResponses;
                }
            }
            // If we're here we either have a failure response or a response that's not a batch response
            return Enumerable.Repeat(response, messages.Count()).ToList();
        }

        private async Task<HttpResponseMessage> RefreshAccessTokenAsync()
        {
            // clear the authentication headers
            client.DefaultRequestHeaders.Authorization = null;

            HttpResponseMessage response = await GetAuthorizeTokenAsync();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(content))
                {
                    JObject json = JObject.Parse(content);
                    this.Token = json["access_token"]?.ToString();
                    if (this.Token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
                    }
                }
                return response;
            }
            else
            {
                return response;
            }
        }

        /// <summary>Returns the token for the client's credientials</summary>
        public async Task<HttpResponseMessage> GetAuthorizeTokenAsync()
        {
            // add credentials to the request
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "password");
            parameters.Add("client_id", this.Credentials.ClientId);
            parameters.Add("client_secret", this.Credentials.ClientSecret);
            parameters.Add("username", this.Credentials.Username);
            parameters.Add("password", this.Credentials.Pass);

            var request = new HttpRequestMessage(HttpMethod.Post, this.Credentials.Url) { Content = new FormUrlEncodedContent(parameters) };
            var response = await client.SendAsync(request);

            return response;
        }        
    }
}
