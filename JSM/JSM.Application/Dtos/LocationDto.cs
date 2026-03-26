namespace JSM.Application.Dtos
{
    public class LocationDto
    {
        public string Region { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public CoordinatesDto Coordinates { get; set; } = new();
        public TimezoneDto Timezone { get; set; } = new();
    }
}
