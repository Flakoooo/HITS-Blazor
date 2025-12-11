using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockCompanies
    {
        private static readonly List<Company> _companies = CreateCompanies();

        public static string HITSId { get; } = Guid.NewGuid().ToString();
        public static string GazpromId { get; } = Guid.NewGuid().ToString();
        public static string RosneftId { get; } = Guid.NewGuid().ToString();

        private static List<Company> CreateCompanies()
        {
            var ivan = MockUsers.GetUserById(MockUsers.IvanId)!;
            var manager = MockUsers.GetUserById(MockUsers.IvanId)!;

            return
            [
                new Company { Id = HITSId,      Name = "ВШЦТ",      Owner = ivan,       Users = [manager]   },
                new Company { Id = GazpromId,   Name = "Газпром",   Owner = ivan,       Users = [ivan]      },
                new Company { Id = RosneftId,   Name = "Роснефть",  Owner = manager,    Users = [ivan]      }
            ];
        }

        public static Company? GetCompanyById(string id)
            => _companies.FirstOrDefault(c => c.Id == id);
    }
}
