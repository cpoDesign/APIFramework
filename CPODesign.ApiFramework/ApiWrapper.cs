using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CPODesign.ApiFramework.DataConverters;
using CPODesign.ApiFramework.Encryption;
using CPODesign.ApiFramework.Enums;

namespace CPODesign.ApiFramework
{
    public class ApiWrapper
    {
        public Uri BaseUrl { get; private set; }
        public string AutorisationHeaderString { get; private set; }
        public string ApiVersion { get; private set; }
        public ApiVersionLocationEnum ApiVersionLocation { get; set; }
        public bool IsRequestSuccessfull { get; private set; }
        public HttpResponseMessage HttpResponse { get; private set; } = null;
        public string DefaultRequestType { get; private set; }
        public string DefaultResponseType { get; private set; }
        public IList<CustomHeader> HttpCustomHeaders { get; private set; }
        public DataConverter DataConverter { get; private set; }
        public IUserAuthenticationEncryption UserAuthenticationEncryption { get; private set; }

        private string EndPointUrl;

        public bool IsApiEndpoint { get; private set; }

        private bool useAPIVersioning = false;

        public ApiWrapper()
        {
            this.ApiVersionLocation = ApiVersionLocationEnum.None;
            this.UserAuthenticationEncryption = new Base64EncryptionUserAuthenticationWrapper();
        }

        public ApiWrapper AddCustomHeaders(IList<CustomHeader> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("Please provide headers", nameof(headers));
            }

            this.HttpCustomHeaders = headers;
            return this;
        }
        public ApiWrapper AddCustomHeader(CustomHeader customHeader)
        {
            if (string.IsNullOrWhiteSpace(customHeader.Name))
            {
                throw new ArgumentException("Custom header has to have a value", nameof(customHeader.Name));
            }

            this.HttpCustomHeaders.Add(customHeader);

            return this;
        }

        public ApiWrapper InstallDataConverter(DataConverter dataConverter)
        {
            this.DataConverter = dataConverter ?? throw new ArgumentNullException(nameof(dataConverter));
            return this;
        }

        public HttpResponseMessage GetResponseInformation()
        {
            return this.HttpResponse;
        }

        /// <summary>
        /// Withes the default type of the media.
        /// </summary>
        /// <param name="defaultMediaType">Default type of the media.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Unsupported default media type - defaultMediaType</exception>
        public ApiWrapper WithDefaultContentType(string defaultMediaType = "application/json")
        {
            if (string.IsNullOrWhiteSpace(defaultMediaType))
            {
                throw new ArgumentException("Please provide default media type", nameof(defaultMediaType));
            }

            this.DefaultRequestType = defaultMediaType;
            this.DefaultResponseType = defaultMediaType;
            return this;
        }

        public ApiWrapper WithResponseType(string defaultMediaType = "application/json")
        {
            if (string.IsNullOrWhiteSpace(defaultMediaType))
            {
                throw new ArgumentException("Unsupported default media type", nameof(defaultMediaType));
            }

            this.DefaultResponseType = defaultMediaType;
            return this;
        }

        /// <summary>
        /// Sets the base URL.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <returns>
        /// Instance of it self with defined BaseUrl
        /// </returns>
        /// <exception cref="System.ArgumentException">please provide valid base url - baseUrl</exception>
        public ApiWrapper SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("please provide valid base url", nameof(baseUrl));
            }

            this.BaseUrl = new Uri(baseUrl);
            return this;
        }

        /// <summary>
        /// Sets the basic authentication.
        /// </summary>
        /// <param name="authenticationString">The authentication string.</param>
        /// <returns>
        /// Instance of it self with defined Authentication string
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Please provide a authentication string - authenticationString
        /// or
        /// BaseAuthentication expect space between two strings - authenticationString
        /// </exception>
        public ApiWrapper SetBasicAuthentication(string authenticationString)
        {
            if (string.IsNullOrWhiteSpace(authenticationString))
            {
                throw new ArgumentException("Please provide a authentication string", nameof(authenticationString));
            }

            if (!authenticationString.Contains(" "))
            {
                throw new ArgumentException("BaseAuthentication expect space between two strings", nameof(authenticationString));
            }

            this.AutorisationHeaderString = authenticationString;

            return this;
        }

        /// <summary>
        /// Sets basic authentication using username and password where password.
        /// </summary>
        /// <param name="userName">Username provided for basic authentication</param>
        /// <param name="password">Password can be provided as empty string but cannot be null</param>
        /// <returns>Instance of an object</returns>
        /// <exception cref="System.ArgumentException">
        /// Please provide a authentication string - authenticationString
        /// or
        /// BaseAuthentication expect space between two strings - authenticationString
        /// </exception>
        public ApiWrapper SetBasicAuthentication(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Please provide user name", nameof(userName));
            }

            if (password == null)
            {
                throw new ArgumentException("Please provide password", nameof(password));
            }

            string encrypted = this.UserAuthenticationEncryption.EncryptUserNameAndPassword(userName, password);

            this.AutorisationHeaderString = $"Basic {encrypted}";

            return this;
        }

        public ApiWrapper OverrideUserAuthenticationEncryption(IUserAuthenticationEncryption userAuthenticationEncryption)
        {
            this.UserAuthenticationEncryption = userAuthenticationEncryption ?? throw new ArgumentNullException(nameof(userAuthenticationEncryption));

            return this;
        }

        /// <summary>
        /// Sets the API version for Header (Accept-Version)
        /// Default version is set to v1.0
        /// </summary>
        /// <param name="apiVersionString">The API version string.</param>
        /// <param name="versionLocation">The version location. Default value is URL</param>
        /// <returns>
        /// Instance of it self with defined API Version
        /// </returns>
        /// <exception cref="System.ArgumentException">Please provide a valid version - apiVersionString</exception>
        public ApiWrapper SetApiVersion(string apiVersionString, ApiVersionLocationEnum versionLocation = ApiVersionLocationEnum.Url)
        {
            if (string.IsNullOrWhiteSpace(apiVersionString))
            {
                throw new ArgumentException("Please provide a valid version", nameof(apiVersionString));
            }

            this.ApiVersion = apiVersionString;
            this.ApiVersionLocation = versionLocation;
            this.useAPIVersioning = true;

            return this;
        }

        /// <summary>
        /// Sets the endpoint URL.
        /// </summary>
        /// <param name="sitesEndPoint">The sites end point.</param>
        /// <returns>
        /// Instance of it self with defined endpoint
        /// </returns>
        /// <exception cref="System.ArgumentException">Please enter valid endpoint - search</exception>
        /// <exception cref="System.NullReferenceException">BaseUrl has not been provided</exception>
        /// <exception cref="UriFormatException">Sites endpoint is not valid to create a valid url</exception>
        public ApiWrapper SetEndpointUrl(string sitesEndPoint)
        {
            if (string.IsNullOrWhiteSpace(sitesEndPoint))
            {
                throw new ArgumentException("Please enter valid endpoint", nameof(sitesEndPoint));
            }

            if (this.BaseUrl == null)
            {
                throw new NullReferenceException("BaseUrl has not been provided");
            }

            this.EndPointUrl = sitesEndPoint;
            return this;
        }

        /// <summary>
        /// Sets API endpoint.
        /// </summary>
        /// <param name="apiEndpoint">The sites end point.</param>
        /// <param name="useAPIVersioning">Override switch to use api versioning in call</param>
        /// <returns>
        /// Instance of it self with defined Web API Version
        /// </returns>
        /// <exception cref="System.ArgumentException">Please enter valid endpoint - search</exception>
        /// <exception cref="System.NullReferenceException">BaseUrl has not been provided</exception>
        /// <exception cref="UriFormatException">Sites endpoint is not valid to create a valid url</exception>
        public ApiWrapper SetWebApiEndpointUrl(string apiEndpoint)
        {
            if (string.IsNullOrWhiteSpace(apiEndpoint))
            {
                throw new ArgumentException("Please enter valid endpoint", nameof(apiEndpoint));
            }

            if (this.BaseUrl == null)
            {
                throw new NullReferenceException("BaseUrl has not been provided");
            }
            if (apiEndpoint.StartsWith("/"))
            {
                apiEndpoint = apiEndpoint.Substring(1, apiEndpoint.Length - 1);
            }

            this.IsApiEndpoint = true;
            this.useAPIVersioning = true;
            this.EndPointUrl = apiEndpoint;
            return this;
        }

        /// <summary>
        /// Searches the restaurant.
        /// </summary>
        /// <typeparam name="T">Object to conver the result into.</typeparam>
        /// <param name="httpMethod">Method type to use for the request.</param>
        /// <param name="content">Content to sent to the endpoint.</param>
        /// <returns>parsed object</returns>
        public async System.Threading.Tasks.Task<T> ExecuteApiCallAsync<T>(HttpMethod httpMethod, string content = null)
        {
            HttpClient http = new HttpClient();

            PopulateCustomHeaders(http);
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, CalculateUrl());
            HttpResponseMessage response = null;

            var requestUri = CalculateUrl();

            switch (httpMethod.Method)
            {
                case "POST":
                    response = http.PostAsync(requestUri, CreateContentForRequest(content)).Result;
                    break;
                case "PUT":
                    response = http.PutAsync(requestUri, CreateContentForRequest(content)).Result;
                    break;
                case "DELETE":
                    response = http.DeleteAsync(requestUri).Result;
                    break;
                default:
                    response = http.GetAsync(requestUri).Result;
                    break;
            }

            this.IsRequestSuccessfull = response.IsSuccessStatusCode;
            this.HttpResponse = response;
            if (response.IsSuccessStatusCode)
            {
                if (this.DataConverter != null)
                {
                    return this.DataConverter.Convert<T>(response.Content.ReadAsStringAsync().Result);
                }
            }

            return (T)Activator.CreateInstance(typeof(T));
        }

        private HttpContent CreateContentForRequest(string content)
        {
            return new StringContent(content, UnicodeEncoding.UTF8, "application/json");
        }

        public Uri CalculateUrl()
        {
            string computedEndPoint = CleanUrl(EndPointUrl);

            if (this.useAPIVersioning && this.ApiVersionLocation == ApiVersionLocationEnum.Url)
            {
                computedEndPoint = $"{this.ApiVersion}/{computedEndPoint}";
            }

            if (this.IsApiEndpoint)
            {
                computedEndPoint = $"api/{computedEndPoint}";
            }

            return new Uri(this.BaseUrl, CleanUrl(computedEndPoint));
        }

        private string CleanUrl(string urlString)
        {
            return urlString.Replace("//", "/");
        }

        private void PopulateCustomHeaders(HttpClient http)
        {
            if (!string.IsNullOrWhiteSpace(this.AutorisationHeaderString))
            {
                var authorisationHeader = AutorisationHeaderString.Split(" ");
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorisationHeader[0], authorisationHeader[1]);
            }

            if (this.HttpCustomHeaders != null)
            {
                foreach (var customHeader in this.HttpCustomHeaders)
                {
                    http.DefaultRequestHeaders.TryAddWithoutValidation(customHeader.Name, customHeader.Value);
                }
            }
        }
    }
}