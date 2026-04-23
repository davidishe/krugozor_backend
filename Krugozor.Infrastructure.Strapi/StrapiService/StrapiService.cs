using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Krugozor.Infrastructure.Strapi
{
  public class StrapiService : IStrapiService
  {

    private readonly IConfiguration _config;

    public StrapiService(IConfiguration config)
    {
      _config = config;
    }

    private void WriteDebugLog(string runId, string hypothesisId, string location, string message, object data)
    {
      try
      {
        var payload = new
        {
          sessionId = "ea380a",
          runId,
          hypothesisId,
          location,
          message,
          data,
          timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        File.AppendAllText("/Users/akobiyada/Documents/SpecialProjects/01_Focus_for_developing/58_DAVINCI/.cursor/debug-ea380a.log", JsonConvert.SerializeObject(payload) + Environment.NewLine);
      }
      catch
      {
        // ignore debug logging failures
      }
    }


    /// <summary>
    /// geting company from strapi via id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<DataDto> GetCompanyEntity(int id)
    {
      var client = new RestClient($"https://strapi.krugozor.space/api/companies/{id}");
      var restRequest = new RestRequest();
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);
      restRequest.Method = Method.Get;
      var response = await client.ExecuteAsync(restRequest);
      if (response.Content is not null)
      {
        var result = JsonConvert.DeserializeObject<StrapiProposalContractDto>(response.Content);
        if (result?.Data is not null) return result.Data;
        #region agent log
        WriteDebugLog("pre-fix", "H1", "StrapiService.cs:GetCompanyEntity", "deserialized response has null Data", new { id, response.StatusCode, response.ContentLength });
        #endregion
        return new DataDto();
      }

      else
      {
        #region agent log
        WriteDebugLog("pre-fix", "H1", "StrapiService.cs:GetCompanyEntity", "response content is null", new { id, response.StatusCode, response.ErrorMessage });
        #endregion
        return new DataDto();
      }
    }



    public async Task<bool> RemoveOrAddDraftStatusForProposal(int strapiProposalId, bool status)
    {
      var data = await GetProposalEntity(strapiProposalId);
      var attributes = data.Attributes;
      if (attributes is null) return false;

      var strapiDto = new Data()
      {
        Name = attributes.Name,
        Description = attributes.Description,
        IsUrgent = attributes.IsUrgent,
        Price = attributes.Price,
        Images = attributes.Images,
        Cities = attributes.Cities,
        IsPublished = status
      };
      await UpdateProposalEntity(strapiDto, data.Id);
      return true;

    }




    /// <summary>
    /// getting proposal objects from strapi via id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<DataDto> GetProposalEntity(int id)
    {
      var client = new RestClient($"https://strapi.krugozor.space/api/proposals/{id}");
      var restRequest = new RestRequest();
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);
      restRequest.Method = Method.Get;
      var response = await client.ExecuteAsync(restRequest);
      if (response.Content is not null)
      {
        var result = JsonConvert.DeserializeObject<StrapiProposalContractDto>(response.Content);
        if (result?.Data is not null) return result.Data;
        #region agent log
        WriteDebugLog("pre-fix", "H2", "StrapiService.cs:GetProposalEntity", "deserialized response has null Data", new { id, response.StatusCode, response.ContentLength });
        #endregion
        return new DataDto();
      }

      else
      {
        #region agent log
        WriteDebugLog("pre-fix", "H2", "StrapiService.cs:GetProposalEntity", "response content is null", new { id, response.StatusCode, response.ErrorMessage });
        #endregion
        return new DataDto();
      }
    }


    /// <summary>
    /// Update already existing entity
    /// </summary>
    /// <param name="data"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<RestResponse> UpdateProposalEntity(Data data, int id)
    {
      var client = new RestClient($"https://strapi.krugozor.space/api/proposals/{id}");
      var restRequest = new RestRequest();
      var serilaizeJson = JsonConvert.SerializeObject(data, Formatting.None,
        new JsonSerializerSettings
        {
          NullValueHandling = NullValueHandling.Ignore
        });

      var correctData = JsonConvert.DeserializeObject<dynamic>(serilaizeJson);
      var strapiProposalContract = new StrapiProposalContract()
      {
        Data = correctData
      };

      var json = JsonConvert.SerializeObject(strapiProposalContract);
      restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);

      restRequest.Method = Method.Put;
      var response = await client.ExecuteAsync(restRequest);
      return response;
    }


    /// <summary>
    /// add new proposal entity
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<RestResponse> AddProposalEntity(Data data)
    {
      var client = new RestClient("https://strapi.krugozor.space/api/proposals");
      var restRequest = new RestRequest();
      var serilaizeJson = JsonConvert.SerializeObject(data, Formatting.None,
        new JsonSerializerSettings
        {
          NullValueHandling = NullValueHandling.Ignore
        });

      var correctData = JsonConvert.DeserializeObject<dynamic>(serilaizeJson);
      var strapiProposalContract = new StrapiProposalContract()
      {
        Data = correctData
      };

      var json = JsonConvert.SerializeObject(strapiProposalContract);
      restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);

      restRequest.Method = Method.Post;
      var response = await client.ExecuteAsync(restRequest);
      return response;
    }



    /// <summary>
    /// adding company entity in strapi
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<RestResponse> AddCompanyAsync(Data data)
    {
      var client = new RestClient("https://strapi.krugozor.space/api/companies");
      var restRequest = new RestRequest();
      var serilaizeJson = JsonConvert.SerializeObject(data, Formatting.None,
        new JsonSerializerSettings
        {
          NullValueHandling = NullValueHandling.Ignore
        });

      var correctData = JsonConvert.DeserializeObject<dynamic>(serilaizeJson);
      var strapiProposalContract = new StrapiProposalContract()
      {
        Data = correctData
      };

      var json = JsonConvert.SerializeObject(strapiProposalContract);
      restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);

      restRequest.Method = Method.Post;
      var response = await client.ExecuteAsync(restRequest);
      return response;
    }


    public async Task<string> AddImageToMediaLibrary(string filePath)
    {

      var client = new RestClient("https://strapi.krugozor.space/api/upload");
      var restRequest = new RestRequest();
      restRequest.Method = Method.Post;
      restRequest.AlwaysMultipartFormData = true;
      restRequest.AddFile("files", filePath);
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);
      var response = await client.ExecuteAsync(restRequest);

      Console.WriteLine(filePath);
      Console.WriteLine(response.StatusCode);
      // var json = JsonConvert.DeserializeObject(response.Content);
      // Console.WriteLine(json);
      Console.WriteLine("==========================================================================");
      Console.WriteLine("==========================================================================");
      if (response.Content is not null) return response.Content;
      #region agent log
      WriteDebugLog("pre-fix", "H3", "StrapiService.cs:AddImageToMediaLibrary", "upload response content is null", new { filePath, response.StatusCode, response.ErrorMessage });
      #endregion
      return string.Empty;
    }



    /// <summary>
    /// update exisiting company in strapi
    /// </summary>
    /// <param name="data"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<RestResponse> UpdateCompanyEntity(Data data, int id)
    {
      var client = new RestClient($"https://strapi.krugozor.space/api/companies/{id}");
      var restRequest = new RestRequest();
      var serilaizeJson = JsonConvert.SerializeObject(data, Formatting.None,
        new JsonSerializerSettings
        {
          NullValueHandling = NullValueHandling.Ignore
        });

      var correctData = JsonConvert.DeserializeObject<dynamic>(serilaizeJson);
      var strapiProposalContract = new StrapiProposalContract()
      {
        Data = correctData
      };

      var json = JsonConvert.SerializeObject(strapiProposalContract);
      restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
      restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);

      restRequest.Method = Method.Put;
      var response = await client.ExecuteAsync(restRequest);
      return response;
    }




  }
}