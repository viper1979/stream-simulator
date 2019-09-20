using Microsoft.AspNetCore.Mvc.RazorPages;
using StreamSimulator.Core;

namespace StreamSimulator.WebApp.Pages
{
    public class CreateStreamModel : PageModel
    {
        private StreamManager _manager;

        public StreamSettings Settings;

        public CreateStreamModel(StreamManager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            Settings = new StreamSettings();
        }

        public void OnPost(StreamSettings settings)
        {
            if(settings != null)
            {
                var stream = _manager.Add(settings);
                stream.Start();
            }

            Redirect("/Index");
        }
    }
}