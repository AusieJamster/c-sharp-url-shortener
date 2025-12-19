using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using UrlShortener.Models;

namespace UrlShortener.Repositories;

public class DynamoDbUrlRepository : IUrlRepository
{
  private readonly IAmazonDynamoDB _dynamoDb;
  private const string _tableName = "UrlShortenerTable";

  public DynamoDbUrlRepository(IAmazonDynamoDB dynamoDb)
  {
    _dynamoDb = dynamoDb;
  }

  public async Task SaveUrlMappingAsync(UrlMapping mapping)
  {
    PutItemRequest requestItem = new PutItemRequest
    {
      TableName = _tableName,
      Item = new Dictionary<string, AttributeValue>
            {
                { "ShortCode", new AttributeValue { S = mapping.ShortCode } },
                { "OriginalUrl", new AttributeValue { S = mapping.OriginalUrl } }
            }
    };

    await _dynamoDb.PutItemAsync(requestItem);
  }

  public async Task<string?> GetOriginalUrlAsync(string shortCode)
  {
    GetItemRequest requestItem = new GetItemRequest
    {
      TableName = _tableName,
      Key = new Dictionary<string, AttributeValue>
            {
                { "ShortCode", new AttributeValue { S = shortCode } }
            }
    };

    GetItemResponse response = await _dynamoDb.GetItemAsync(requestItem);

    if (!response.IsItemSet)
    {
      return null;
    }

    return response.Item["OriginalUrl"].S;
  }
}
