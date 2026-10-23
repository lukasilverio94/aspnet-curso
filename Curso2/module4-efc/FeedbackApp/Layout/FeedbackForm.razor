@page "/feedback"
@using FeedbackApp.Models
@using FeedbackApp.Services
@inject FeedbackService FeedbackService

<EditForm Model="@feedback" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div>
        <label for="name">Name:</label>
        <InputText id="name" @bind-Value="feedback.Name" class="form-control" />
    </div>

    <div>
        <label for="email">Email:</label>
        <InputText id="email" @bind-Value="feedback.Email" class="form-control" />
    </div>

    <div>
        <label for="comment">Comment:</label>
        <InputTextArea id="comment" @bind-Value="feedback.Comment" class="form-control" />
    </div>

    <button type="submit" class="btn btn-primary">Submit</button>
    <a href="/feedback-list" class="btn btn-link">View Submitted Feedback</a>
</EditForm>

@if (isSubmitted)
{
    <p>Thank you for your feedback!</p>
}

@code {
    private Feedback feedback = new();
    private bool isSubmitted = false;

    private void HandleValidSubmit()
    {
        FeedbackService.AddFeedback(feedback);
        feedback = new Feedback();
        isSubmitted = true;
    }
}