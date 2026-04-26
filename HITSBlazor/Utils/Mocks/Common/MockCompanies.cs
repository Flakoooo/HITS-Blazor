using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
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
            var manager = MockUsers.GetUserById(MockUsers.ManagerId)!;

            return
            [
                new Company { Id = HITSId,      Name = "ВШЦТ",      Owner = ivan,       Members = [manager]   },
                new Company { Id = GazpromId,   Name = "Газпром",   Owner = ivan,       Members = [ivan]      },
                new Company { Id = RosneftId,   Name = "Роснефть",  Owner = manager,    Members = [ivan]      }
            ];
        }

        public static ListDataResponse<Company> GetAllCompaniesByQueryParams(
            int page, 
            int pageSize = 20,
            string? searchText = null
        )
        {
            var query = _companies.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(c => c.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<Company> { Count = count, List = query.ToList() };
        }

        public static Company? GetCompanyById(Guid id)
            => _companies.FirstOrDefault(c => c.Id == id);

        public static Company? GetCompanyByName(string name)
            => _companies.FirstOrDefault(c => c.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));

        public static Company? CreateCompany(string name, Guid ownerId, List<Guid> membersIds)
        {
            var owner = MockUsers.GetUserById(ownerId);
            if (owner is null) return null;

            var members = new List<User>();
            foreach (var memberId in membersIds)
            {
                var member = MockUsers.GetUserById(memberId);
                if (member is not null) members.Add(member);
            }

            var company = new Company
            {
                Name = name,
                Owner = owner,
                Members = members
            };

            _companies.Add(company);

            return company;
        }

        public static Company? UpdateCompany(Guid companyId, string name, Guid ownerId, List<Guid> membersIds)
        {
            var company = GetCompanyById(companyId);
            if (company is null) return null;

            var owner = MockUsers.GetUserById(ownerId);
            if (owner is null) return null;

            var members = new List<User>();
            foreach (var memberId in membersIds)
            {
                var member = MockUsers.GetUserById(memberId);
                if (member is not null) members.Add(member);
            }

            company.Name = name;
            company.Owner = owner;
            company.Members = members;

            return company;
        }

        public static bool DeleteCompany(Company company) => _companies.Remove(company);
    }
}
