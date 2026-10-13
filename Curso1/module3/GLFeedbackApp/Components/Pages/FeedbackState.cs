using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace FeedbackApp
{
    public class FeedbackState
    {
        private readonly IJSRuntime jsRuntime;
    
        public FeedbackState(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task SaveFeedbackAsync(List<Feedback> feedbackList){
            var json = JsonSerializer.Serialize(feedbackList);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", "feedbackList", json);
        }

        public async Task<List<Feedback>> LoadFeedbackAsync(){
            var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "feedbackList");
            if (string.IsNullOrEmpty(json))
            {
                return new List<Feedback>();
            }
            var feedbackList = JsonSerializer.Deserialize<List<Feedback>>(json);
            return feedbackList ?? new List<Feedback>();
        }
    }


}