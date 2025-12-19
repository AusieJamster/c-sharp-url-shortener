using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using DotNetEnv;

namespace UrlShortener.Infra;

public class UrlShortenerStack : Stack
{
  internal UrlShortenerStack(Amazon.CDK.Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
  {
    Table urlTable = new Table(this, "UrlShortenerTable", new TableProps
    {
      TableName = "UrlShortenerTable",
      PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute { Name = "ShortCode", Type = AttributeType.STRING },
      BillingMode = BillingMode.PAY_PER_REQUEST,
      RemovalPolicy = RemovalPolicy.DESTROY
    });

    new CfnOutput(this, "TableNameOutput", new CfnOutputProps
    {
      ExportName = "UrlShortenerTableName",
      Value = urlTable.TableName
    });
  }
}

public class Program
{
  public static void Main(string[] _)
  {
    Env.Load();

    App app = new App();
    new UrlShortenerStack(app, "UrlShortenerStack", new StackProps
    {
      Env = new Amazon.CDK.Environment
      {
        Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
        Region = System.Environment.GetEnvironmentVariable("AWS_REGION") ?? "ap-southeast-2"
      }
    });
    app.Synth();
  }
}