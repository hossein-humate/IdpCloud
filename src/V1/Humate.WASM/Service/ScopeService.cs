using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Humate.WASM.Service
{
    public class ScopeService
    {
        public event Action CurrentProjectChanged;
        public event Func<Task> CurrentProjectChangedAsync;

        public void OnCurrentProjectChanged()
        {
            CurrentProjectChanged?.Invoke();
        }

        public void ClearStack()
        {
            CurrentProjectChanged = null;
            CurrentProjectChangedAsync = null;
        }

        public async Task OnCurrentProjectChangedAsync()
        {
            var handler = CurrentProjectChangedAsync;

            if (handler == null)
            {
                return;
            }
            var invocationList = handler.GetInvocationList();
            var handlerTasks = new Task[invocationList.Length];

            for (var i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<Task>)invocationList[i])();
            }

            await Task.WhenAll(handlerTasks);
        }
    }
}
