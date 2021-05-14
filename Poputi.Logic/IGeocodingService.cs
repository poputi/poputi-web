using NetTopologySuite.Geometries;

namespace Poputi.Logic
{
    public interface IGeocodingService
    {
        public (string error, Point) GetGeocode(string address);
    }
}
