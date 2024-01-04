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

        [HttpPost]
        [Route("/Users")]
        public int InsertUser(User user)
        {
            try
            {
                _repository.Users.Insert(user);

                return _repository.Users.SaveAsync().Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }

        [HttpDelete]
        [Route("/Users/{id}")]
        public string DeleteUser(long id)
        {
            var isDeleted = string.Empty;

            try
            {
                isDeleted = _repository.Users.DeleteByIdAsync(id).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return isDeleted;
        }

        [HttpPut]
        [Route("/Users")]
        public int UpdateUser(User user)
        {
            try
            {
                return _repository.Users.UpdateAsync(user).Result;
            }
            catch (Exception ex)
            {
                //add loging
            }

            return 0;
        }
    }
}
