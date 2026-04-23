using RestSharp;

namespace Krugozor.Infrastructure.Strapi
{
  public interface IStrapiService
  {
    Task<RestResponse> UpdateProposalEntity(Data contract, int id);
    Task<RestResponse> AddProposalEntity(Data contract);
    Task<DataDto> GetProposalEntity(int id);
    Task<DataDto> GetCompanyEntity(int id);
    Task<RestResponse> AddCompanyAsync(Data data);
    Task<RestResponse> UpdateCompanyEntity(Data contract, int id);
    Task<bool> RemoveOrAddDraftStatusForProposal(int strapiProposalId, bool status);
    Task<string> AddImageToMediaLibrary(string filePath);


  }
}