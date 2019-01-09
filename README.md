# APIFramework

## Badges

|Badge type| Current status|
| --- | --- |
|**Build status**| [![Build status](https://cpodesign.visualstudio.com/PB/_apis/build/status/cpoDesign.APIFramework)](https://cpodesign.visualstudio.com/PB/_build/latest?definitionId=32) |
|**Open Cover**| [![Coverage Status](https://coveralls.io/repos/github/cpoDesign/APIFramework/badge.svg?branch=master)](https://coveralls.io/github/cpoDesign/APIFramework?branch=master)|
|**NuGet**| [![nuget](https://img.shields.io/nuget/v/cpoDesign.APIFramework.svg)](https://www.nuget.org/packages/cpoDesign.APIFramework/)|
|**DepShield Badge**|[![DepShield Badge](https://depshield.sonatype.org/badges/cpoDesign/APIFramework/depshield.svg)](https://depshield.github.io)


## Quick implementation guide

1. Install nuget package **CPODesign.APIFramework**
1. Copy the code below
1. Update and use in your project

```c#
 Console.WriteLine("Press any key to start executing sample against demo app project");
            Console.ReadKey();

            var usersManager = new ApiWrapper()
                .SetBaseUrl("https://localhost:44364/")
                .InstallDataConverter(new CPODesign.ApiFramework.DataConverters.DataConverter())
                .WithDefaultContentType(ContentTypes.ApplicationJson)
                .AddCustomHeaders(new List<CustomHeader> {
                    new CustomHeader(name: "Content-type", value: ContentTypes.ApplicationJson),
                    new CustomHeader(name: "Accept", value: ContentTypes.ApplicationJson)
                });

            var userList = usersManager.SetWebApiEndpointUrl("/user");

            Console.WriteLine("Getting user list from " + userList.CalculateUrl().ToString());
            var userListResult = await userList.ExecuteApiCallAsync<List<UserListItem>>(HttpMethod.Get);
            Console.WriteLine($"Received {userListResult.Count} records");

            foreach (var user in userListResult)
            {
                Console.WriteLine($"Getting information for {user.Id}");

                var userDetailConfiguration = usersManager.SetWebApiEndpointUrl($"/user/{user.Id}");

                var userDetail = await userDetailConfiguration.ExecuteApiCallAsync<User>(HttpMethod.Get);

                Console.WriteLine($"UserID: {userDetail.Id} - Name: {userDetail.Name} - Surname: {userDetail.Surname}");
            };

            /* Sample for posting to the api */

            var postDataConfiguration = userList.SetWebApiEndpointUrl("/User");
            var objToSend = new User() { Id = 3, Name = "Luke", Surname = "Skywalker" };

            Console.WriteLine($"Posting a new user {objToSend.Id}");
            await postDataConfiguration.ExecuteApiCallAsync<User>(HttpMethod.Post, JsonConvert.SerializeObject(objToSend));
            Console.WriteLine($"POST: have been: {postDataConfiguration.HttpResponse.StatusCode}");

            var deleteDataConfiguration = userList.SetWebApiEndpointUrl("/User/123");
            Console.WriteLine($"DELETE a new user {objToSend.Id}");
            await deleteDataConfiguration.ExecuteApiCallAsync<User>(HttpMethod.Delete);
            Console.WriteLine($"DELETE: have been: {postDataConfiguration.HttpResponse.StatusCode}");


            var putDataConfiguration = userList.SetWebApiEndpointUrl("/User/123");
            Console.WriteLine($"PUT a new user {objToSend.Id}");
            await putDataConfiguration.ExecuteApiCallAsync<User>(HttpMethod.Put, JsonConvert.SerializeObject(objToSend));
            Console.WriteLine($"PUT: have been: {postDataConfiguration.HttpResponse.StatusCode}");


            Console.WriteLine("Completed");
```

We have added a Sample folder with dummy application and console implementation for you to have a look at and get you to working solution fast.
