using JSM.Application.Services;
using System.Dynamic;

namespace JSM.Domain.Test
{
    public class UserTransformerServiceTests
    {
        private static dynamic CreateCsvLikeRaw()
        {
            dynamic raw = new ExpandoObject();
            var dict = (IDictionary<string, object>)raw;

            dict["location__coordinates__latitude"] = "-40.0";
            dict["location__coordinates__longitude"] = "-20.0";
            dict["gender"] = "male";
            dict["name__first"] = "John";
            dict["name__last"] = "Doe";
            dict["name__title"] = "Mr";
            dict["location__state"] = "São Paulo";
            dict["location__street"] = "Rua A, 123";
            dict["location__city"] = "São Paulo";
            dict["location__postcode"] = "01000-000";
            dict["location__timezone__offset"] = "-03:00";
            dict["location__timezone__description"] = "Brasília";
            dict["phone"] = "011 1234-5678";
            dict["cell"] = "01234-567-890";
            dict["email"] = "john.doe@example.com";
            dict["dob__date"] = "1980-01-01T00:00:00Z";
            dict["registered__date"] = "2020-06-15T12:30:00Z";
            dict["picture__large"] = "large.jpg";
            dict["picture__medium"] = "medium.jpg";
            dict["picture__thumbnail"] = "thumb.jpg";

            return raw;
        }

        private static dynamic CreateJsonLikeRaw()
        {
            dynamic raw = new ExpandoObject();
            var rawDict = (IDictionary<string, object>)raw;

            dynamic location = new ExpandoObject();
            var locDict = (IDictionary<string, object>)location;

            dynamic coordinates = new ExpandoObject();
            var coordDict = (IDictionary<string, object>)coordinates;
            coordDict["Latitude"] = "-40.0";
            coordDict["Longitude"] = "-20.0";

            dynamic timezone = new ExpandoObject();
            var tzDict = (IDictionary<string, object>)timezone;
            tzDict["Offset"] = "-03:00";
            tzDict["Description"] = "Brasília";

            locDict["Coordinates"] = coordinates;
            locDict["Timezone"] = timezone;
            locDict["State"] = "São Paulo";
            locDict["Street"] = "Rua B, 456";
            locDict["City"] = "Campinas";
            locDict["Postcode"] = "13000-000";

            rawDict["Location"] = location;
            rawDict["Gender"] = "female";

            dynamic name = new ExpandoObject();
            var nameDict = (IDictionary<string, object>)name;
            nameDict["Title"] = "Ms";
            nameDict["First"] = "Maria";
            nameDict["Last"] = "Silva";
            rawDict["Name"] = name;

            rawDict["Phone"] = "(11) 9999-8888";
            rawDict["Cell"] = "019 91234-5678";
            rawDict["Email"] = "maria.silva@example.com";

            dynamic dob = new ExpandoObject();
            ((IDictionary<string, object>)dob)["Date"] = "1990-05-20T00:00:00Z";
            rawDict["Dob"] = dob;

            dynamic registered = new ExpandoObject();
            ((IDictionary<string, object>)registered)["Date"] = "2021-07-01T08:00:00Z";
            rawDict["Registered"] = registered;

            dynamic picture = new ExpandoObject();
            var picDict = (IDictionary<string, object>)picture;
            picDict["Large"] = "p_large.jpg";
            picDict["Medium"] = "p_medium.jpg";
            picDict["Thumbnail"] = "p_thumb.jpg";
            rawDict["Picture"] = picture;

            return raw;
        }

        [Fact]
        public void TransformCSV_MapsFields_FormatsPhonesAndSetsDefaults()
        {
            // Arrange
            var transformer = new UserTransformerService();
            dynamic raw = CreateCsvLikeRaw();

            // Act
            var result = transformer.TransformCSV(raw);

            // Assert - basic mappings
            Assert.Equal("M", result.Gender);
            Assert.Equal("John", result.Name.First);
            Assert.Equal("Doe", result.Name.Last);
            Assert.Equal("Mr", result.Name.Title);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.Equal("BR", result.Nationality);

            // Region mapping (São Paulo -> sudeste)
            Assert.Equal("sudeste", result.Location.Region);

            // Coordinates preserved as strings
            Assert.Equal("-40.0", result.Location.Coordinates.Latitude);
            Assert.Equal("-20.0", result.Location.Coordinates.Longitude);

            // Pictures
            Assert.Equal("large.jpg", result.Picture.Large);
            Assert.Equal("medium.jpg", result.Picture.Medium);
            Assert.Equal("thumb.jpg", result.Picture.Thumbnail);

            // Phones formatted to E.164 (+55...). Based on FormatPhoneToE164 logic:
            // phone "011 1234-5678" -> digits "01112345678" -> TrimStart('0') => "1112345678" -> +55...
            Assert.Single(result.TelephoneNumbers);
            Assert.Equal("+551112345678", result.TelephoneNumbers[0]);

            // cell "01234-567-890" -> digits "01234567890" -> TrimStart('0') => "1234567890" -> +55...
            Assert.Single(result.MobileNumbers);
            Assert.Equal("+551234567890", result.MobileNumbers[0]);

            // Dates parsed
            Assert.Equal(DateTime.Parse("1980-01-01T00:00:00Z"), result.Birthday);
            Assert.Equal(DateTime.Parse("2020-06-15T12:30:00Z"), result.Registered);

            // Classification: given the bounding-box logic in the implementation, both "special" and "normal"
            // branches are unreachable with the provided boxes (min/max lon ordering). The method therefore
            // falls back to "trabalhoso".
            Assert.Equal("trabalhoso", result.Type);
        }

        [Fact]
        public void TransformJSON_MapsNestedFields_FormatsPhonesAndParsesDates()
        {
            // Arrange
            var transformer = new UserTransformerService();
            dynamic raw = CreateJsonLikeRaw();

            // Act
            var result = transformer.TransformJSON(raw);

            // Assert - basic mappings
            Assert.Equal("F", result.Gender);
            Assert.Equal("Maria", result.Name.First);
            Assert.Equal("Silva", result.Name.Last);
            Assert.Equal("Ms", result.Name.Title);
            Assert.Equal("maria.silva@example.com", result.Email);
            Assert.Equal("BR", result.Nationality);

            // Region mapping (São Paulo -> sudeste)
            Assert.Equal("sudeste", result.Location.Region);

            // Coordinates preserved as strings
            Assert.Equal("-40.0", result.Location.Coordinates.Latitude);
            Assert.Equal("-20.0", result.Location.Coordinates.Longitude);

            // Pictures
            Assert.Equal("p_large.jpg", result.Picture.Large);
            Assert.Equal("p_medium.jpg", result.Picture.Medium);
            Assert.Equal("p_thumb.jpg", result.Picture.Thumbnail);

            // Phones formatted to E.164
            // Phone "(11) 9999-8888" -> digits "1199998888" -> +55...
            Assert.Single(result.TelephoneNumbers);
            Assert.Equal("+551199998888", result.TelephoneNumbers[0]);

            // Cell "019 91234-5678" -> digits "019912345678" -> TrimStart('0') => "19912345678" (11 digits) -> +55...
            Assert.Single(result.MobileNumbers);
            Assert.Equal("+5519912345678", result.MobileNumbers[0]);

            // Dates parsed via nested Date properties
            Assert.Equal(DateTime.Parse("1990-05-20T00:00:00Z"), result.Birthday);
            Assert.Equal(DateTime.Parse("2021-07-01T08:00:00Z"), result.Registered);

            // Type expectation same as CSV test
            Assert.Equal("trabalhoso", result.Type);
        }
    }
}