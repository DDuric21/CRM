using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using UI.Enums;

namespace UI.Services
{
    public interface ICrmModalService
    {
        void ShowErrorMessage(string message);
        void ShowWarningMessage(string message);
        void ShowInfoMessage(string message);
        Task<ModalResult> ShowDialog(string message, ModalDialogType type);
        Task<ModalResult> Show<TComponent>() where TComponent : IComponent;
        Task<ModalResult> Show<TComponent>(ModalParameters parameters) where TComponent : IComponent;
    }
}
