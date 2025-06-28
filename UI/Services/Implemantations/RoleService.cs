using Models.DTO;
using Models.Requests;
using Models.Responses;
using UI.Helpers;

namespace UI.Services
{
    public class RoleService : IRoleService
    {
        private readonly ICommunicationService _communicationService;
        private readonly ILoggingService _loggingService;
        private const string ApiUrl = "Roles";

        public RoleService(
            ICommunicationService communicationService,
            ILoggingService loggingService)
        {
            _communicationService = communicationService;
            _loggingService = loggingService;
        }

        public async Task<IEnumerable<UserRoleDTO>> GetApplicableRolesAsync()
        {
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, ApiUrl);

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

        public async Task<ActionResult<object>> CreateRoleAsync(RoleDTO roleDTO)
        {
            var url = $"{ApiUrl}/Create";
            var body = new CreateNewRoleRQ { RoleDTO = roleDTO };
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Post, url, body);
            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                return new ActionResult<object>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<object>(ex.Message);
            }
        }

        public async Task<ActionResult<GetRolePermissionsRS>> GetRolePermissionsAsync(string roleName)
        {
            var url = $"{ApiUrl}/Permissions/{roleName}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<GetRolePermissionsRS>(request);

                return new ActionResult<GetRolePermissionsRS>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<GetRolePermissionsRS>(ex.Message);
            }
        }

        public async Task<ActionResult<object>> UpdateRolePermissionsAsync(string roleName, Dictionary<string, bool> permissions)
        {
            var url = $"{ApiUrl}/Permissions";
            var body = new RolePermissions(roleName, permissions);
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Put, url, body);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);
                return new ActionResult<object>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<object>(ex.Message);
            }
        }

        public async Task<ActionResult<object>> DeleteRoleAsync(string roleName)
        {
            var url = $"{ApiUrl}/Permissions/{roleName}";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Delete, url);

            try
            {
                var response = await _communicationService.SendRequestAsyncNew<ResponseBase>(request);

                return new ActionResult<object>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<object>(ex.Message);
            }
        }

        public async Task<ActionResult<GetAllPermissionsRS>> GetAllPermissionsAsync()
        {
            var url = $"{ApiUrl}/Permissions";
            var request = await _communicationService.CreateRequestAsync(HttpMethod.Get, url);
            try
            {
                var response = await _communicationService.SendRequestAsyncNew<GetAllPermissionsRS>(request);
                return new ActionResult<GetAllPermissionsRS>(response);
            }
            catch (Exception ex)
            {
                _loggingService.SendErrorLogToServerAsync(ex);
                return new ActionResult<GetAllPermissionsRS>(ex.Message);
            }
        }
    }
}
