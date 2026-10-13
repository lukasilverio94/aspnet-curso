using System.Text.Json;
using FeedbackApp.Models;
using Microsoft.JSInterop;

namespace FeedbackApp.Services
{
    public class FeedbackState
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "feedbackItems";

        public FeedbackState(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // Serialize the list to JSON and save to localStorage.
        public async Task SaveFeedbackAsync(List<Feedback> feedbackList)
        {
            var json = JsonSerializer.Serialize(feedbackList);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }

        // Load JSON from localStorage and deserialize. Empty -> new list.
        public async Task<List<Feedback>> LoadFeedbackAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);

            if (string.IsNullOrEmpty(json))
            {
                return new List<Feedback>();
            }

            return JsonSerializer.Deserialize<List<Feedback>>(json) ?? new List<Feedback>();
        }
    }
}