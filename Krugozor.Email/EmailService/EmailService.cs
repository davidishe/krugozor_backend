using Microsoft.Extensions.Logging;
using Krugozor.Email.Models;
using RestSharp;

namespace Krugozor.Email.EmailService
{

  public class EmailService : IEmailService
  {

    private readonly ILogger<EmailService> _logger;


    public EmailService(
      ILogger<EmailService> logger
    )
    {
      _logger = logger;
    }


    public async Task<RestResponse> SendEmailMessage(MailRequest body)
    {

      var client = new RestClient("http://localhost:6047/api/mail/feedback/");
      var restRequest = new RestRequest();
      restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
      restRequest.Method = Method.Post;
      Console.WriteLine("Sending email..");
      var result = await client.ExecuteAsync(restRequest);
      return result;
    }
  }

}