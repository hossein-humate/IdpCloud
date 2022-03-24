namespace Humate.Sdk.Model.BaseInfo
{
    public class City
    {
        public int CityId { get; set; }

        public string Name { get; set; }

        public Country Country { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}
