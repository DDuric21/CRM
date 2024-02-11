using Backend_API.Data.Model;

namespace Backend_API.Data.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Finds and returnes the user for given email
        /// </summary>
        /// <param name="userEmail">Email of user</param>
        /// <returns>Returnes the found user</returns>
        User GetUserByEmail (string userEmail);
    }
}
