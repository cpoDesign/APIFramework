using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        List<User> UserList = new List<User>
        {
            new User{ Id = 1, Name = "Joe", Surname = "Doe"},
            new User{ Id = 2, Name = "Jimmy", Surname = "McDonnald"}
        };

        // GET: api/User
        [HttpGet]
        public IEnumerable<UserListItem> Get()
        {
            return UserList.Select(x => new UserListItem { Id = x.Id, UserFullName = $"{x.Name}-{x.Surname}" });
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public User Get(int id)
        {
            return UserList.FirstOrDefault(x => x.Id.Equals(id));
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserList.Add(user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            //throw new NotImplementedException();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            
        }
    }


    public class UserListItem
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
    }

}
