using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Resources.Translations;
using UI.Enums;
using UI.Pages.Modals;

namespace UI.Services
{
    public class CrmModalService : ICrmModalService
    {
        private readonly IModalService _modalService;
        private string ErrorTitle = Translation.Error;
        private string WarningTitle = Translation.Warning;
        private string InfoTitle = Translation.InfoTitle;
        private const int DefaultToastDelay = 5000;

        public CrmModalService(IModalService modalService)
        {
            _modalService = modalService;
        }

        /// <summary>
        /// Show a non-blocking “success” toast in the bottom-left that auto-closes.
        /// </summary>
        public void ShowSuccessToast(string message, string title = "", int delay = DefaultToastDelay)
        {
            if (string.IsNullOrEmpty(title))
            {
                title = Translation.Success;
            }

            var parameters = new ModalParameters
            {
                { nameof(ToastModal.Title), title },
                { nameof(ToastModal.Message), message },
                { nameof(ToastModal.Delay), delay }
            };
    
            var options = new ModalOptions
            {
                HideHeader = true,
                UseCustomLayout = true,
                Class = "p-0 border-0"
            };
    
            _modalService.Show<ToastModal>(title, parameters, options);
        }

        public void ShowCustomerFriendlyMessage()
        {
            // Show correlation ID to customer would be great
            ShowErrorMessage(Translation.CustomerFriendlyMessage);
        }

        public void ShowErrorMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("body", message);

            _modalService.Show<ModalBase>(ErrorTitle, parameters);
        }

        public async Task<ModalResult> ShowErrorMessageAsync(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("body", message);

            return await _modalService.Show<ModalBase>(ErrorTitle, parameters).Result;
        }

        public void ShowWarningMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("body", message);

            _modalService.Show<ModalBase>(WarningTitle, parameters);
        }

        public async Task<ModalResult> ShowDialog(string message, ModalDialogType type)
        {
            var parameters = new ModalParameters();
            parameters.Add("body", message);
            parameters.Add(typeof(ModalDialogType).Name, type);

            var modalResult = await _modalService.Show<ModalDialog>(WarningTitle, parameters).Result;

            return modalResult;
        }

        public void ShowInfoMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("body", message);

            _modalService.Show<ModalBase>(InfoTitle, parameters);
        }

        public async Task<ModalResult> Show<TComponent>() where TComponent : IComponent
        {
            return await _modalService.Show<TComponent>().Result;
        }

        public async Task<ModalResult> Show<TComponent>(ModalParameters parameters) where TComponent : IComponent
        {
            return await _modalService.Show<TComponent>(parameters).Result;
        }

        public async Task<ModalResult> Show<TComponent>(ModalParameters parameters, ModalOptions options) where TComponent : IComponent
        {
            return await _modalService.Show<TComponent>(parameters, options).Result;
        }
    }
}
