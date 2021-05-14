using NetTopologySuite.Geometries;
using System.Threading.Tasks;

namespace Poputi.Logic
{
    public interface IGeocodingService
    {
        public Task<(string error, Point)> GetGeocode(string address);
    }
}
