using System;
using Microsoft.AspNetCore.Components;

namespace Humate.WASM.Shared.Component.Toast
{
    public class ToastService : IToastService
    {
        public event Action<ToastLevel, RenderFragment, string, Action> OnShow;

        public void ShowToast(ToastLevel level, string message, string heading = "", Action onClick = null) =>
            ShowToast(level, builder => builder.AddContent(0, message), heading, onClick);

        public void ShowToast(ToastLevel level, RenderFragment message, string heading = "", Action onClick = null) =>
               OnShow?.Invoke(level, message, heading, onClick);
    }

    public interface IToastService
    {
        event Action<ToastLevel, RenderFragment, string, Action> OnShow;

        void ShowToast(ToastLevel level, string message, string heading = "", Action onClick = null);

        void ShowToast(ToastLevel level, RenderFragment message, string heading = "", Action onClick = null);
    }
}
