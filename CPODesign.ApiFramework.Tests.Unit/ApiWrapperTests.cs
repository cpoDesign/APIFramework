using CPODesign.ApiFramework.Tests.Unit.TestingResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CPODesign.ApiFramework.Tests.Unit
{
    [TestClass]
    public class ApiWrapperTests
    {
        [TestMethod]
        public void JustEatRestaurantSearcher_CreatesClass()
        {
            new ApiWrapper();
        }

        [TestMethod]
        public void CanSetBaseUrl_SetUpUrl_ShouldSucceed()
        {
            const string testingUrl = "https://www.test.com/";

            var apiWrapper = new ApiWrapper().SetBaseUrl(testingUrl);

            Assert.AreEqual(testingUrl, apiWrapper.BaseUrl.ToString());
        }

        [TestMethod]
        public void CanSetBaseUrl_SetUpUrl_ShouldThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetBaseUrl(string.Empty));
        }

        [TestMethod]
        public void SetAuthorisation_SetAuthentication_ShouldSucceed()
        {
            const string testingAuthenication = "Basic VGVjaFRlc3Q6bkQ2NGxXVnZreDVw";

            var apiWrapper = new ApiWrapper().SetBasicAuthentication(testingAuthenication);

            Assert.AreEqual(testingAuthenication, apiWrapper.AutorisationHeaderString.ToString());
        }

        [TestMethod]
        public void SetAuthentication_PassEmptyString_ShouldFailWtihArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetBasicAuthentication(string.Empty));
        }

        [TestMethod]
        public void SetAuthentication_PassSingleString_ShouldFailWtihArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetBasicAuthentication("testing"));
        }

        [TestMethod]
        public void SetAuthentication_UserNameAndPwdOverride_WithPassingNullToUserName_ShouldThrowAnException()
        {

            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetBasicAuthentication(null, null));
        }

        [TestMethod]
        public void SetAuthentication_UserNameAndPwdOverride_WithPassingNullToPwd_ShouldThrowAnException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetBasicAuthentication("userName", null));
        }

        [TestMethod]
        public void SetAuthentication_UserNameAndPassword_SouldSuccessfullySetAuthenticationValue()
        {
            const string ecryptedValue = "Basic dXNlcjpwYXNzd29yZA==";
            var apiWrappper = new ApiWrapper().SetBasicAuthentication("user", "password");
            Assert.AreEqual(ecryptedValue, apiWrappper.AutorisationHeaderString);
        }

        [TestMethod]
        public void ApiWrapper_ShouldSetupDefaultVersionAs1Dot0_ShouldSucced()
        {
            const string expectedVersion = "v1.0";
            var apiWrappper = new ApiWrapper().SetApiVersion("v1.0");
            Assert.AreEqual(expectedVersion, apiWrappper.ApiVersion);
        }

        [TestMethod]
        public void SetApiVersion_PassEmptyString_ShouldThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetApiVersion(string.Empty));
        }

        [TestMethod]
        public void SetApiVersion_ShouldSetupVersionAs1Dot1_ShouldSucced()
        {
            const string expectedVersion = "v1.1";
            var apiWrappper = new ApiWrapper().SetApiVersion(expectedVersion);
            Assert.AreEqual(expectedVersion, apiWrappper.ApiVersion);
        }

        [TestMethod]
        public void AddCustomHeaders_ShouldPopulateInternalProperty()
        {
            var apiWrappper = new ApiWrapper().AddCustomHeaders(new List<CustomHeader>(){
                new CustomHeader("test","Value")
            });

            Assert.AreEqual(1, apiWrappper.HttpCustomHeaders.Count);
        }

        [TestMethod]
        public void SetEndpointUrl_PassEmptyString_ShouldThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().SetEndpointUrl(string.Empty));
        }

        [TestMethod]
        public void SetEndPointUrl_SetEndpointUrlWithoutBaseUrl_ShouldThrowNullReferenceException()
        {
            Assert.ThrowsException<NullReferenceException>(() => new ApiWrapper().SetEndpointUrl("restaurant"));
        }

        [TestMethod]
        public void WithDefaultMediaType_WithoutContentType_ShouldThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().WithDefaultContentType(string.Empty));
        }

        [TestMethod]
        public void WithDefaultMediaType_PassSupportedType_ShouldSetDefaultRequestMediaType()
        {
            var api = new ApiWrapper().WithDefaultContentType();
            Assert.AreEqual("application/json", api.DefaultRequestType);
        }

        [TestMethod]
        public void WithDefaultMediaType_PassSupportedType_ShouldSetDefaultResponseType()
        {
            var api = new ApiWrapper().WithDefaultContentType();
            Assert.AreEqual("application/json", api.DefaultResponseType);
        }

        [TestMethod]
        public void SetResponseType_PassDefaultTypeAndSetDifferentResponse_ShouldSucceed()
        {
            var api = new ApiWrapper().WithDefaultContentType("application/json")
                .WithResponseType("application/xml");
            Assert.AreEqual("application/xml", api.DefaultResponseType);
        }

        [TestMethod]
        public void AddCustomHeaders_PassNullParamenter_ShouldReturnArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApiWrapper().AddCustomHeaders(null));
        }

        [TestMethod]
        public void AddCustomHeader_PassEmptyNameParamenter_ShouldReturnArgumentNullException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ApiWrapper().AddCustomHeader(new CustomHeader(string.Empty, string.Empty)));
        }

        [TestMethod]
        public void OverrideUserAuthenticationEncryption_PassNull_ShouldReturnNullReferenceException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApiWrapper().OverrideUserAuthenticationEncryption(null));
        }

        [TestMethod]
        public void OverrideUserAuthenticationEncryption_PassOverride_ShouldApplyChangesIntoHeader()
        {
            const string ecryptedValue = "Basic user-password";
            var apiWrappper = new ApiWrapper()
                .OverrideUserAuthenticationEncryption(new TestEncryption())
                .SetBasicAuthentication("user", "password");
            Assert.AreEqual(ecryptedValue, apiWrappper.AutorisationHeaderString);

        }
    }
}
