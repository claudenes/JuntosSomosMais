using JSM.Application.Dtos;
using JSM.Application.Interfaces;
using System.Text.RegularExpressions;

namespace JSM.Application.Services
{
    public class UserTransformerService : IUserTransformerService
    {
        // Bounding boxes para classificação
        private static readonly List<BoundingBox> SpecialBoxes = new()
    {
        new(-2.196998, -46.361899, -15.411580, -34.276938),
        new(-19.766959, -52.997614, -23.966413, -44.428305)
    };

        private static readonly BoundingBox NormalBox = new(-26.155681, -54.777426, -34.016466, -46.603598);

        public UserDto TransformCSV(dynamic rawUser)
        {
            var user = new UserDto();

            // Classificação
            double lat = double.Parse(rawUser.location__coordinates__latitude.ToString());
            double lon = double.Parse(rawUser.location__coordinates__longitude.ToString());
            user.Type = ClassifyLocation(lat, lon);

            // Gênero
            user.Gender = rawUser.gender == "male" ? "M" : "F";

            // Nome
            user.Name = new NameDto
            {
                First = rawUser.name__first,
                Last = rawUser.name__last,
                Title = rawUser.name__title,
            };

            // Localização com região
            user.Location = new LocationDto
            {
                Region = GetRegionFromState(rawUser.location__state.ToString()),
                Street = rawUser.location__street.ToString(),
                City = rawUser.location__city,
                State = rawUser.location__state,
                Postcode = rawUser.location__postcode.ToString(),
                Coordinates = new CoordinatesDto
                {
                    Latitude = rawUser.location__coordinates__latitude.ToString(),
                    Longitude = rawUser.location__coordinates__longitude.ToString()
                },
                Timezone = new TimezoneDto
                {
                    Offset = rawUser.location__timezone__offset,
                    Description = rawUser.location__timezone__description
                }
            };

            // Telefones formatados
            user.TelephoneNumbers = new List<string> { FormatPhoneToE164(rawUser.phone.ToString()) };
            user.MobileNumbers = new List<string> { FormatPhoneToE164(rawUser.cell.ToString()) };

            // Email
            user.Email = rawUser.email;

            // Datas
            user.Birthday = DateTime.Parse(rawUser.dob__date.ToString());
            user.Registered = DateTime.Parse(rawUser.registered__date.ToString());

            // Picture
            user.Picture = new PictureDto
            {
                Large = rawUser.picture__large,
                Medium = rawUser.picture__medium,
                Thumbnail = rawUser.picture__thumbnail
            };

            // Nacionalidade fixa
            user.Nationality = "BR";

            return user;
        }
        public UserDto TransformJSON(dynamic rawUser)
        {
            var user = new UserDto();

            // Classificação
            double lat = double.Parse(rawUser.Location.Coordinates.Latitude.ToString());
            double lon = double.Parse(rawUser.Location.Coordinates.Longitude.ToString());
            user.Type = ClassifyLocation(lat, lon);

            // Gênero
            user.Gender = rawUser.Gender == "male" ? "M" : "F";

            // Nome
            user.Name = new NameDto
            {
                Title = rawUser.Name.Title,
                First = rawUser.Name.First,
                Last = rawUser.Name.Last
            };

            // Localização com região
            user.Location = new LocationDto
            {
                Region = GetRegionFromState(rawUser.Location.State.ToString()),
                Street = rawUser.Location.Street.ToString(),
                City = rawUser.Location.City,
                State = rawUser.Location.State,
                Postcode = rawUser.Location.Postcode.ToString(),
                Coordinates = new CoordinatesDto
                {
                    Latitude = rawUser.Location.Coordinates.Latitude.ToString(),
                    Longitude = rawUser.Location.Coordinates.Longitude.ToString()
                },
                Timezone = new TimezoneDto
                {
                    Offset = rawUser.Location.Timezone.Offset,
                    Description = rawUser.Location.Timezone.Description
                }
            };

            // Telefones formatados
            user.TelephoneNumbers = new List<string> { FormatPhoneToE164(rawUser.Phone.ToString()) };
            user.MobileNumbers = new List<string> { FormatPhoneToE164(rawUser.Cell.ToString()) };

            // Email
            user.Email = rawUser.Email;

            // Datas
            user.Birthday = DateTime.Parse(rawUser.Dob.Date.ToString());
            user.Registered = DateTime.Parse(rawUser.Registered.Date.ToString());

            // Picture
            user.Picture = new PictureDto
            {
                Large = rawUser.Picture.Large,
                Medium = rawUser.Picture.Medium,
                Thumbnail = rawUser.Picture.Thumbnail
            };

            // Nacionalidade fixa
            user.Nationality = "BR";

            return user;
        }
        private string ClassifyLocation(double lat, double lon)
        {
            // Verifica se está em alguma região ESPECIAL
            foreach (var box in SpecialBoxes)
            {
                if (lat >= box.MinLat && lat <= box.MaxLat &&
                    lon >= box.MinLon && lon <= box.MaxLon)
                    return "especial";
            }

            // Verifica se está em região NORMAL
            if (lat >= NormalBox.MinLat && lat <= NormalBox.MaxLat &&
                lon >= NormalBox.MinLon && lon <= NormalBox.MaxLon)
                return "normal";

            return "trabalhoso";
        }

        private string GetRegionFromState(string state)
        {
            var north = new[] { "acre", "amapá", "amazonas", "pará", "rondônia", "roraima", "tocantins" };
            var northeast = new[] { "alagoas", "bahia", "ceará", "maranhão", "paraíba", "pernambuco", "piauí", "rio grande do norte", "sergipe" };
            var centralWest = new[] { "distrito federal", "goiás", "mato grosso", "mato grosso do sul" };
            var southeast = new[] { "espírito santo", "minas gerais", "rio de janeiro", "são paulo" };
            var south = new[] { "paraná", "rio grande do sul", "santa catarina" };

            state = state.ToLowerInvariant();

            if (north.Contains(state)) return "norte";
            if (northeast.Contains(state)) return "nordeste";
            if (centralWest.Contains(state)) return "centro-oeste";
            if (southeast.Contains(state)) return "sudeste";
            if (south.Contains(state)) return "sul";

            return "desconhecida";
        }

        private string FormatPhoneToE164(string phone)
        {
            // Remove caracteres não numéricos
            var numbers = Regex.Replace(phone, @"[^\d]", "");

            // Formato brasileiro: +55 + DDD + número
            if (numbers.Length >= 10 && numbers.Length <= 11)
            {
                // Remove leading zero if present
                numbers = numbers.TrimStart('0');

                // Garante que tem 10 ou 11 dígitos (com ou sem 9 no celular)
                if (numbers.Length == 10 || numbers.Length == 11)
                {
                    return $"+55{numbers}";
                }
            }

            // Fallback: retorna original se não conseguir formatar
            return phone;
        }

        private record BoundingBox(double MinLon, double MinLat, double MaxLon, double MaxLat);
    }
}
