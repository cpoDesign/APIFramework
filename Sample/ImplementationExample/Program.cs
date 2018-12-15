using System;
using System.Collections.Generic;
using CPODesign.ApiFramework;
using CPODesign.ApiFramework.Enums;

namespace ImplementationExample
{
    class Program
    {
        static void Main(string[] args)
        {
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
            Console.ReadKey();
        }
    }

    public class UserListItem
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
