using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Users
{
    public static class MockProfiles
    {
        private static readonly List<Profile> _profiles = CreateProfiles();

        private static List<Profile> CreateProfiles()
        {
            var profiles = new List<Profile>();

            foreach (var user in MockUsers.GetAllUsers())
            {
                var profile = CreateProfile(user.Id);
                if (profile != null) profiles.Add(profile);
            }

            return profiles;
        }

        public static Profile? CreateProfile(Guid userId)
        {
            var user = MockUsers.GetUserById(userId);
            if (user == null) return null;

            var userTag = MockUserTelegrams.GetTelegramInfoByUserId(user.Id);

            return new Profile
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = [.. user.Roles],
                CreatedAt = user.CreatedAt,
                Telephone = user.Telephone,
                StudyGroup = user.StudyGroup,

                Skills = [.. MockUsersSkills.GetUserSkillsById(user.Id)],
                Ideas = [.. MockIdeas.GetIdeasByInitiatorId(user.Id)],
                Teams = [.. MockTeamExperiences.GetTeamExperiencesByUserId(user.Id)],
                UserTag = userTag?.UserTag,
                IsUserTagVisible = userTag?.IsVisible
            };
        }
    }
}
