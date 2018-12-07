using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using CPODesign.ApiFramework.DataConverters;
using CPODesign.ApiFramework.Encryption;

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

        public ApiWrapper()
        {
            this.ApiVersion = "v1.0";
            this.ApiVersionLocation = ApiVersionLocationEnum.Url;
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
            if (dataConverter == null)
            {
                throw new ArgumentNullException(nameof(dataConverter));
            }

            this.DataConverter = dataConverter;
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
            return this;
        }

        /// <summary>
        /// Sets the endpoint URL.
        /// </summary>
        /// <param name="sitesEndPoint">The sites end point.</param>
        /// <returns></returns>
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
        /// Searches the restaurant.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postCode">The post code.</param>
        /// <param name="cuisine">The cuisine.</param>
        /// <param name="restaurantName">Name of the restaurant.</param>
        /// <returns>parsed object</returns>
        public T ExecuteApiCall<T>(string postCode, string cuisine = "", string restaurantName = "")
        {
            HttpClient http = new HttpClient();

            if (!string.IsNullOrWhiteSpace(this.AutorisationHeaderString))
            {
                var authorisationHeader = AutorisationHeaderString.Split(" ");
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorisationHeader[0], authorisationHeader[1]);
            }

            PopulateCustomHeaders(http);

            var result = http.GetAsync(CalculateUrl()).Result;
            this.IsRequestSuccessfull = result.IsSuccessStatusCode;
            this.HttpResponse = result;
            if (result.IsSuccessStatusCode)
            {
                if (this.DataConverter != null)
                {
                    return this.DataConverter.Convert<T>(result.Content.ReadAsStringAsync().Result);
                }
            }

            return (T)Activator.CreateInstance(typeof(T));
        }

        private Uri CalculateUrl()
        {
            if (this.ApiVersionLocation == ApiVersionLocationEnum.Url)
            {
                return new Uri(this.BaseUrl, $"{ApiVersion}/{EndPointUrl}");
            }

            return new Uri(this.BaseUrl, $"{EndPointUrl}");
        }

        private void PopulateCustomHeaders(HttpClient http)
        {
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