using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamSimulator.Core;
using System.ComponentModel.DataAnnotations;

namespace StreamSimulator.WebApp.Pages
{
    public class StreamDetailModel : PageModel
    {
        public StreamManager _manager;

        public StreamDetailModel(StreamManager manager)
        {
            _manager = manager;
        }

        [Required]
        public StreamSettings StreamSettings { get; set; }

        public PageResult OnGet(int? port)
        {
            if(port.HasValue)
            {
                var streamSimulator = _manager.Get(port.Value);
                StreamSettings = streamSimulator?.Settings;
            }

            return Page();
        }

        public void OnPost(StreamSettings settings)
        {
            if(settings != null)
            {
                _manager.Remove(settings.ListeningPort);
            }
        }
    }
}