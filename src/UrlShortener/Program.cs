using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using UrlShortener.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonDynamoDB>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

const string TableName = "UrlShortenerTable";

app.MapPost("/api/shorten", async (ShortenUrlRequest request, IAmazonDynamoDB dynamoDB) =>
{
    if (string.IsNullOrWhiteSpace(request.Url) || !Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("Invalid URL provided.");
    }

    string shortCode = Guid.NewGuid().ToString().Substring(0, 8);

    PutItemRequest requestItem = new PutItemRequest
    {
        TableName = TableName,
        Item = new Dictionary<string, AttributeValue>
        {
            { "ShortCode", new AttributeValue { S = shortCode } },
            { "OriginalUrl", new AttributeValue { S = request.Url } }
        }
    };

    await dynamoDB.PutItemAsync(requestItem);

    UrlMapping mapping = new UrlMapping(shortCode, request.Url);

    return Results.Ok(mapping);
});

app.MapGet("/api/{shortCode}", async (string shortCode, IAmazonDynamoDB dynamoDB) =>
{
    GetItemRequest getItemRequest = new GetItemRequest
    {
        TableName = TableName,
        Key = new Dictionary<string, AttributeValue>
        {
            { "ShortCode", new AttributeValue { S = shortCode } }
        }
    };

    GetItemResponse getItemResponse = await dynamoDB.GetItemAsync(getItemRequest);

    if (!getItemResponse.IsItemSet)
    {
        return Results.NotFound("Short URL not found.");
    }

    string originalUrl = getItemResponse.Item["OriginalUrl"].S;
    return Results.Redirect(originalUrl);
});

app.Run();
