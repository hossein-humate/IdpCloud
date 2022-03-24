using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace IdpCloud.REST.Infrastructure.Helper
{
    public class ActionHidingConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            // Remove any Action that I don't want to display in Swagger When Child Controller inherit from parent Controller
            if (action.Controller.ControllerName != "IdentityBase")
            {
                if (action.ActionName == "IsValidToken")
                {
                    action.ApiExplorer.IsVisible = false;
                }
            }
        }
    }
}
