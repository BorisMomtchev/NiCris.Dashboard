using System.IO;

namespace NiCris.WCF.REST.Utility
{
    public interface IServiceParameterMapper
    {
        dynamic ParseQueryString(string queryStering);
        dynamic ParseRequestBody(Stream requestStream);
    }
}