using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _repository;

        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _repository.GetUsers();

            return Ok(users);
        }


        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {

            var user = _repository.GetById(userId);

            if (user == null) 
                NotFound();


            return Ok(user);
        }

        [HttpPost]
        public IActionResult Insert([FromBody]User user)
        {
            _repository.Insert(user);


            return Ok(user);

        }

        [HttpPut]
        public IActionResult Put([FromBody] User user)
        {
            _repository.Update(user);
            return Ok(user);
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(int userId)
        {
            var user = _repository.GetById(userId);

            if (user == null)
                NotFound();

            _repository.Delete(userId);
            return Ok("user deleted.");
        }


    }
}
