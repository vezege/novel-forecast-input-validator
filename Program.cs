using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => { return true; };

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/check-input", ([FromBody] UserInput input) =>
{
    return input.ClearedReview;
})
.WithName("CheckNovelForecastInput")
.WithOpenApi();

app.Run();

public class UserInput
{
    /// <summary>
    /// Original input
    /// </summary>
    public string Review { get; set; }

    /// <summary>
    /// Processed input
    /// </summary>
    public string ClearedReview
    {
        get
        {
            if (Review.Contains("http"))
            {
                return string.Empty;
            }

            string clearedReview;

            clearedReview = Review.Replace("\"\"", "\"");
            clearedReview = clearedReview.Replace("\"\"\"", "\"");
            clearedReview = clearedReview.Replace(" (hide spoiler)]", string.Empty);
            clearedReview = clearedReview.Replace("(view spoiler)[", string.Empty);
            clearedReview = clearedReview.Replace("** spoiler alert **", string.Empty);
            clearedReview = clearedReview.Replace("[image error]", string.Empty);

            return string.Join("", clearedReview.AsQueryable().Where(c => !char.IsPunctuation(c))).ToLower();
        }
    }
}
