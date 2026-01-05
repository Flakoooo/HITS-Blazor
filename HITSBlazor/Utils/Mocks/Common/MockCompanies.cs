using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockCompanies
    {
        public static Guid HITSId { get; } = Guid.NewGuid();
        public static Guid GazpromId { get; } = Guid.NewGuid();
        public static Guid RosneftId { get; } = Guid.NewGuid();

        private static readonly List<Company> _companies = CreateCompanies();

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

        public static List<Company> GetAllCompanies() => [.. _companies];

        public static Company? GetCompanyById(Guid id)
            => _companies.FirstOrDefault(c => c.Id == id);
    }
}
