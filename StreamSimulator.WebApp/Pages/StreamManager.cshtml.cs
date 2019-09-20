using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamSimulator.Core;

namespace StreamSimulator.WebApp.Pages
{
    public class StreamManagerModel : PageModel
    {
        private StreamManager _manager;

        public StreamingServer[] _currentStreams;

        public StreamManagerModel(StreamManager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            _currentStreams = _manager.GetSimulators();
        }
    }
}