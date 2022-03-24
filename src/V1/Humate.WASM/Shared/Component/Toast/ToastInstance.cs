using System;
using Microsoft.AspNetCore.Components;

namespace Humate.WASM.Shared.Component.Toast
{
    public class ToastSettings
    {
        public ToastSettings(
            string heading,
            RenderFragment message,
            string baseClass,
            string additionalClasses,
            string iconClass,
            bool showProgressBar,
            Action onClick)
        {
            Heading = heading;
            Message = message;
            BaseClass = baseClass;
            AdditionalClasses = additionalClasses;
            IconClass = iconClass;
            ShowProgressBar = showProgressBar;
            OnClick = onClick;
            if (onClick != null)
            {
                AdditionalClasses += " blazored-toast-action";
            }
        }

        public string Heading { get; set; }
        public RenderFragment Message { get; set; }
        public string BaseClass { get; set; }
        public string AdditionalClasses { get; set; }
        public string IconClass { get; set; }
        public bool ShowProgressBar { get; set; }
        public Action OnClick { get; set; }
    }

    public class ToastInstance
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public ToastSettings ToastSettings { get; set; }
    }

    #region Enums
    public enum ToastLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    public enum ToastPosition
    {
        TopLeft,
        TopRight,
        TopCenter,
        BottomLeft,
        BottomRight,
        BottomCenter
    } 
    #endregion
}
