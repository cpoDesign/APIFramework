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
            var userListResult = userList.ExecuteApiCall<List<UserListItem>>();
            Console.WriteLine($"Received {userListResult.Count} records");

            foreach (var user in userListResult)
            {
                Console.WriteLine($"Getting information for {user.Id}");

                var userDetailConfiguration = usersManager.SetWebApiEndpointUrl($"/user/{user.Id}");

                var userDetail = userDetailConfiguration.ExecuteApiCall<User>();

                Console.WriteLine($"UserID: {userDetail.Id} - Name: {userDetail.Name} - Surname: {userDetail.Surname}");
            };

            Console.WriteLine("Completed");
```

We have added a Sample folder with dummy application and console implementation for you to have a look at and get you to working solution fast.