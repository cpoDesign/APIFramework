using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CPODesign.ApiFramework;
using CPODesign.ApiFramework.Enums;
using Newtonsoft.Json;

namespace ImplementationExample
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
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


            //Console.WriteLine($"Patch a new user {objToSend.Id}");
            //await postDataConfiguration.ExecuteApiCallAsync<User>(HttpMethod.Patch, JsonConvert.SerializeObject(objToSend));
            //Console.WriteLine($"Patch: have been: {postDataConfiguration.HttpResponse.StatusCode}");


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
