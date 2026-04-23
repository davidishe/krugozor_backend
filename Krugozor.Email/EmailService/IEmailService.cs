using Krugozor.Email.Models;
using RestSharp;

namespace Krugozor.Email.EmailService
{
  public interface IEmailService
  {
    Task<RestResponse> SendEmailMessage(MailRequest body);

  }
}