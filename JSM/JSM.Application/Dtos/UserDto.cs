using System.Xml.Linq;

namespace JSM.Application.Dtos
{
    public class UserDto
    {
        public string Type { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public NameDto Name { get; set; } = new();
        public LocationDto Location { get; set; } = new();
        public string Email { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public DateTime Registered { get; set; }
        public List<string> TelephoneNumbers { get; set; } = new();
        public List<string> MobileNumbers { get; set; } = new();
        public PictureDto Picture { get; set; } = new();
        public string Nationality { get; set; } = "BR";
    }
}
