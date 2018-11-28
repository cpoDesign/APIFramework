# APIFramework

## Quick implementation guide
``` c#
string basicAuthorisationString = "Basic VGVBaRRlD3Q6bkQ2NGxXVjZraDAw";

var searcher = new ApiWrapper()
    .SetBaseUrl("http://testingendpoint/api/")
    .SetBasicAuthentication(basicAuthorisationString)
    .SetApiVersion("v1.0")
    .WithDefaultContentType(ContentTypes.ApplicationJson)
    .SetEndpointUrl("/SendMessage")
    .AddCustomHeaders(new List<CustomHeader> {
        new CustomHeader(name: "Content-type", value: ContentTypes.ApplicationJson)
    });

string jsonBody = "{\"key\":\"value\"}";
ResponseCallObject results = searcher.ExecuteApiCall<ResponseCallObject>(jsonBody);

if (searcher.IsRequestSuccessfull)
{
    Console.WriteLine($"do your work");
}
else
{
    var responseInfo = searcher.GetResponseInformation();
}
			```