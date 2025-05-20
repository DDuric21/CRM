using Models.DTO;

namespace UI.Services
{
    public class RoleService : IRoleService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;

        public RoleService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<IEnumerable<UserRoleDTO>> GetApplicableRolesAsync()
        {
            var url = "Roles";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<HashSet<UserRoleDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return Array.Empty<UserRoleDTO>();
            }
        }
    }
}
