using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/Contrib/users")]
    [ApiController]
    public class ContribUsersController : ControllerBase
    {
        private IContribUserRepository _repository;

        public ContribUsersController(IContribUserRepository repository)
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
        public IActionResult Insert([FromBody] User user)
        {
            try
            {
                _repository.Insert(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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
