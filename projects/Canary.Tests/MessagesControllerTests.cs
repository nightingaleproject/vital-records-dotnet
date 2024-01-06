using Xunit;
using canary.Controllers;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using VRDR;

namespace canary.tests
{
  public class MessagesControllerTests
  {
    private MessagesController _messagesController;
    public MessagesControllerTests()
    {
      _messagesController = new MessagesController();
    }

    [Fact]
    public async void NewMessagePostCreatesNewCodingResponseMessageObject()
    {
      var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(FixturePath("fixtures/json/CauseOfDeathCodingMessage.json"))));
      var httpContext = new DefaultHttpContext()
      {
        Request = { Body = stream, ContentLength = stream.Length }
      };
      _messagesController.ControllerContext.HttpContext = httpContext;
      var response = await _messagesController.NewMessagePost();
      var message = response.message;
      var issues = response.issues;
      Assert.Equal(new List<Dictionary<string, string>>(), issues);
      Assert.IsType<canary.Models.Message>(message);
      Assert.IsAssignableFrom<CauseOfDeathCodingMessage>(message.GetMessage());
    }

    [Fact]
    public async void BadNewMessagePostReturnsIssues()
    {
      var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(FixturePath("fixtures/json/EmptyMessage.json"))));
      var httpContext = new DefaultHttpContext()
      {
        Request = { Body = stream, ContentLength = stream.Length }
      };
      _messagesController.ControllerContext.HttpContext = httpContext;
      var response = await _messagesController.NewMessagePost();
      var issues = response.issues[0];
      Assert.Equal(2, issues.Count);
      Assert.Contains(new KeyValuePair<string, string>("severity", "error"), issues);
      Assert.Contains(new KeyValuePair<string, string>("message", "Failed to find a Bundle Entry containing a Resource of type MessageHeader. Error occurred at VRDR.BaseMessage in function Parse."), issues);
    }

    private string FixturePath(string filePath)
    {
      if (Path.IsPathRooted(filePath))
      {
        return filePath;
      }
      else
      {
        return Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);
      }
    }
  }
}
