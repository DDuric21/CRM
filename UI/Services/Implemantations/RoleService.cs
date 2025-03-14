using Models.DTO;

namespace UI.Services
{
    public class RoleService : IRoleService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ICrmModalService _modalService;

        public RoleService(
            ICommunicationService communicationService,
            ICrmModalService modalService)
        {
            _communicationService = communicationService;
            _modalService = modalService;
        }

        public async Task<IEnumerable<UserRoleDTO>> GetApplicableRolesAsync()
        {
            var url = string.Format("https://localhost:7076/Roles");
            var request = _communicationService.CreateRequest(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsync<HashSet<UserRoleDTO>>(request);

                return response;
            }
            catch (Exception ex)
            {
                // logging
                _modalService.ShowErrorMessage(ex.Message);

                return null;
            }
        }
    }
}
