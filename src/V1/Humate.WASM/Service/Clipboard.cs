using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Humate.WASM.Service
{
    public class Clipboard
    {
        private readonly IJSRuntime _jsRuntime;

        public Clipboard(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> ReadTextAsync()
        {
            return _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
        }

        public ValueTask WriteTextAsync(string text)
        {
            return _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
