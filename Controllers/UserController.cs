using Backend_API.Data.Model;
using Backend_API.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend_API.Controllers
{
    public class UserController : Controller
    {
        private readonly ICrmRepository _repository;

        public UserController(ICrmRepository crmRepository)
        {
            _repository = crmRepository;
        }

        [HttpGet]
        [Route("/Users")]
        public IEnumerable<User> GetAll()
        {
            var users = _repository.Users.GetAllAsync();

            return users.Result;
        }

        [HttpGet]
        [Route("/Users/{id}")]
        public User GetById(long id)
        {
            var user = _repository.Users.GetByIdAsync(id);

            if (user == null)
            {
                return new User();
            }

            return user.Result;
        }
    }
}
