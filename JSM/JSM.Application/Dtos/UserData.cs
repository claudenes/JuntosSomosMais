using CsvHelper.Configuration.Attributes;
using System.Text.Json.Serialization;

namespace JSM.Application.Dtos
{
    public class UserData
    {
        [Name("gender")]
        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [Name("name__title")]
        public string NameTitle { get; set; }

        [Name("name__first")]
        public string NameFirst { get; set; }

        [Name("name__last")]
        public string NameLast { get; set; }

        [Name("location__street")]
        public string LocationStreet { get; set; }

        [Name("location__city")]
        public string LocationCity { get; set; }

        [Name("location__state")]
        public string LocationState { get; set; }

        [Name("location__postcode")]
        public string LocationPostcode { get; set; }

        [Name("location__coordinates__latitude")]
        public string LocationCoordinatesLatitude { get; set; }

        [Name("location__coordinates__longitude")]
        public string LocationCoordinatesLongitude { get; set; }

        [Name("location__timezone__offset")]
        public string LocationTimezoneOffset { get; set; }

        [Name("location__timezone__description")]
        public string LocationTimezoneDescription { get; set; }

        [Name("email")]
        public string Email { get; set; }

        [Name("dob__date")]
        public string DobDate { get; set; }

        [Name("dob__age")]
        public int DobAge { get; set; }

        [Name("registered__date")]
        public string RegisteredDate { get; set; }

        [Name("registered__age")]
        public int RegisteredAge { get; set; }

        [Name("phone")]
        public string Phone { get; set; }

        [Name("cell")]
        public string Cell { get; set; }

        [Name("picture__large")]
        public string PictureLarge { get; set; }

        [Name("picture__medium")]
        public string PictureMedium { get; set; }

        [Name("picture__thumbnail")]
        public string PictureThumbnail { get; set; }
    }
}