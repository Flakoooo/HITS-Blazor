using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeams
    {        
        private static readonly string _lorem = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Eius aperiam delectus possimus, voluptates quo accusamus? Consequatur, quasi rem temporibus blanditiis delectus aliquid officia aut, totam incidunt reiciendis eaque laborum fugiat!";

        public static Guid CardId { get; } = Guid.NewGuid();
        public static Guid CactusId { get; } = Guid.NewGuid();
        public static Guid CarpId { get; } = Guid.NewGuid();

        private static readonly List<Team> _teams = CreateTeams();

        private static List<TeamMember> CreateTeamMembers(Guid teamId, params User[] users)
        {
            var members = new List<TeamMember>();

            foreach (var user in users)
            {
                members.Add(new TeamMember
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Skills = MockUsersSkills.GetUserSkillsById(user.Id)
                });
            }

            return members;
        }

        private static List<Team> CreateTeams()
        {
            var javaScript = MockSkills.GetSkillById(MockSkills.JavaScriptId)!;

            var wantedSkills = new List<Skill>()
            {
                javaScript,
                MockSkills.GetSkillById(MockSkills.DockerId)!,
                MockSkills.GetSkillById(MockSkills.PostgreSQLId)!
            };

            var cardMembers = CreateTeamMembers(
                CardId, MockUsers.GetUserById(MockUsers.KirillId)!, MockUsers.GetUserById(MockUsers.DenisId)!
            );
            var kirill = cardMembers.FirstOrDefault(m => m.UserId == MockUsers.KirillId)!;

            var cactusMembers = CreateTeamMembers(
                CactusId, MockUsers.GetUserById(MockUsers.TimurId)!, MockUsers.GetUserById(MockUsers.AdminId)!
            );
            var timur = cactusMembers.FirstOrDefault(m => m.UserId == MockUsers.TimurId)!;

            var carpMembers = CreateTeamMembers(
                CarpId, MockUsers.GetUserById(MockUsers.LubovId)!, MockUsers.GetUserById(MockUsers.DmitryId)!, MockUsers.GetUserById(MockUsers.AntonId)!
            );
            var lubov = carpMembers.FirstOrDefault(m => m.UserId == MockUsers.LubovId)!;

            return
            [
                new Team
                {
                    Id = CardId,
                    Name = "Визитка",
                    Closed = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                    Description = _lorem,
                    MembersCount = 2,
                    Owner = kirill,
                    Leader = kirill,
                    Members = cardMembers,
                    Skills = [.. cardMembers.SelectMany(m => m.Skills).Distinct()],
                    Tags = new TeamTags { StudyGroups = ["ИИП-22-1", "ИСТНб-21"], StudyCourses = [Course.first, Course.second] },
                    WantedSkills = [.. wantedSkills],
                    IsRefused = false,
                    HasActiveProject = true,
                    IsAcceptedToIdea = true,
                    StatusQuest = false
                },
                new Team
                {
                    Id = CactusId,
                    Name = "Кактус",
                    Closed = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                    Description = _lorem,
                    MembersCount = 2,
                    Owner = timur,
                    Leader = timur,
                    Members = cactusMembers,
                    Skills = [.. cactusMembers.SelectMany(m => m.Skills).Distinct()],
                    Tags = new TeamTags { StudyGroups = ["ИИПб-23-1", "АСОИУ-22-1"], StudyCourses = [Course.second] },
                    WantedSkills = [..wantedSkills],
                    StatusQuest = false,
                    HasActiveProject = true,
                    IsAcceptedToIdea = true,
                    IsRefused = false
                },
                new Team
                {
                    Id = CarpId,
                    Name = "Карасики",
                    Closed = false,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc),
                    Description = _lorem,
                    MembersCount = 3,
                    Owner = lubov,
                    Leader = lubov,
                    Members = carpMembers,
                    Skills = [.. carpMembers.SelectMany(m => m.Skills).Distinct()],
                    Tags = new TeamTags { StudyGroups = ["ИСТНб-21-2", "АСОИУ-20-1"], StudyCourses = [Course.third, Course.fourth] },
                    WantedSkills = [..wantedSkills],
                    IsRefused = false,
                    HasActiveProject = false,
                    IsAcceptedToIdea = false,
                    StatusQuest = false
                }
            ];
        }

        public static Team? GetTeamById(Guid id) 
            => _teams.FirstOrDefault(t => t.Id == id);

        public static ListDataResponse<Team> GetAllTeamsByQueryParams(
            int page,
            int pageSize = 20,
            Guid? userId = null,
            string? searchText = null,
            bool? privacy = null,
            bool? hasActiveProject = null,
            HashSet<Guid>? searchSkillIds = null,
            string? orderBy = null,
            bool? byDescending = null
        )
        {
            var query = _teams.AsQueryable();

            if (userId.HasValue)
                query = query.Where(t => t.Owner.Id == userId || (t.Leader != null && t.Leader.Id == userId));

            if (privacy.HasValue)
                query = query.Where(t => t.Closed == privacy);

            if (hasActiveProject.HasValue)
                query = query.Where(t => t.HasActiveProject == hasActiveProject);

            if (searchSkillIds?.Count > 0)
                query = query.Where(t => t.Skills.Any(s => searchSkillIds.Contains(s.Id)));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(t => t.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(orderBy) && byDescending.HasValue)
            {
                query = (orderBy, byDescending.Value) switch
                {
                    (nameof(Team.Closed), true) => query.OrderByDescending(t => t.Closed),
                    (nameof(Team.Closed), false) => query.OrderBy(t => t.Closed),
                    (nameof(Team.Name), true) => query.OrderByDescending(t => t.Name),
                    (nameof(Team.Name), false) => query.OrderBy(t => t.Name),
                    (nameof(Team.HasActiveProject), true) => query.OrderByDescending(t => t.HasActiveProject),
                    (nameof(Team.HasActiveProject), false) => query.OrderBy(t => t.HasActiveProject),
                    (nameof(Team.MembersCount), true) => query.OrderByDescending(t => t.MembersCount),
                    (nameof(Team.MembersCount), false) => query.OrderBy(t => t.MembersCount),
                    (nameof(Team.CreatedAt), true) => query.OrderByDescending(t => t.CreatedAt),
                    (nameof(Team.CreatedAt), false) => query.OrderBy(t => t.CreatedAt),
                    _ => query
                };
            }

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<Team>(count, query.ToList());
        }

        public static List<Team> GetTeamsByOwnerIdOrLeaderId(Guid userId) 
            => _teams.Where(t => t.Owner.UserId == userId || t.Leader?.UserId == userId).ToList();

        public static ListDataResponse<TeamMember> GetTeamMembers(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            string? searchText = null
        )
        {
            IQueryable<TeamMember> query;
            if (teamId.HasValue)
            {
                var team = _teams.FirstOrDefault(t => t.Id == teamId.Value);
                if (team is null) return new ListDataResponse<TeamMember>(0, []);

                query = team.Members.AsQueryable();
            }
            else
            {
                query = _teams.SelectMany(t => t.Members).AsQueryable();
            }

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(tm => tm.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                    || tm.Email.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<TeamMember>(count, query.ToList());
        }

        public static bool CreateTeam(CreateTeamRequest request)
        {
            var owner = MockUsers.GetUserById(request.OwnerId);
            if (owner is null) return false;

            var newTeam = new Team
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Name = request.Name,
                Description = request.Description,
                Closed = request.IsClosed,
                WantedSkills = request.WantedSkills.Select(id => MockSkills.GetSkillById(id)!).ToList(),
            };

            var teamOwner = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = newTeam.Id,
                UserId = owner.Id,
                Email = owner.Email,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Skills = MockUsersSkills.GetUserSkillsById(owner.Id)
            };

            newTeam.Owner = teamOwner;
            newTeam.Leader = teamOwner;
            newTeam.Members.Add(teamOwner);

            MockTeamInvitations.CreateNewInvitations(newTeam.Id, request.InvitedMembers);

            newTeam.Skills = newTeam.Members.SelectMany(m => m.Skills).DistinctBy(m => m.Id).ToList();

            _teams.Add(newTeam);

            return true;
        }

        public static bool UpdateTeam(UpdateTeamRequest request)
        {
            var teamForUpdate = _teams.FirstOrDefault(t => t.Id == request.Id);
            if (teamForUpdate is null) return false;

            teamForUpdate.Name = request.Name;
            teamForUpdate.Description = request.Description;
            teamForUpdate.Closed = request.IsClosed;
            teamForUpdate.WantedSkills = request.WantedSkills.Select(id => MockSkills.GetSkillById(id)!).ToList();

            if (request.NewOwnerId.HasValue)
            {
                var owner = MockUsers.GetUserById(request.NewOwnerId.Value);
                if (owner is not null)
                {
                    teamForUpdate.Owner = new TeamMember
                    {
                        Id = Guid.NewGuid(),
                        TeamId = request.Id,
                        UserId = owner.Id,
                        Email = owner.Email,
                        FirstName = owner.FirstName,
                        LastName = owner.LastName,
                        Skills = MockUsersSkills.GetUserSkillsById(owner.Id)
                    };
                }
            }

            if (request.NewLeaderId.HasValue)
            {
                var leader = MockUsers.GetUserById(request.NewLeaderId.Value);
                if (leader is not null)
                {
                    teamForUpdate.Owner = new TeamMember
                    {
                        Id = Guid.NewGuid(),
                        TeamId = request.Id,
                        UserId = leader.Id,
                        Email = leader.Email,
                        FirstName = leader.FirstName,
                        LastName = leader.LastName,
                        Skills = MockUsersSkills.GetUserSkillsById(leader.Id)
                    };
                }
            }

            teamForUpdate.Members.RemoveAll(tm => request.KickedMembers.ToHashSet().Contains(tm.UserId));

            MockTeamInvitations.CreateNewInvitations(teamForUpdate.Id, request.InvitedMembers);

            return true;
        }

        public static bool UpdateTeamLeader(Guid teamId, Guid? leaderId = null)
        {
            var team = _teams.FirstOrDefault(t => t.Id == teamId);
            if (team is null) return false;

            if (leaderId.HasValue)
            {
                var user = MockUsers.GetUserById(leaderId.Value);
                if (user is null) return false;

                var existMember = team.Members.FirstOrDefault(m => m.UserId == leaderId);
                if (existMember is null)
                {
                    team.Leader = new TeamMember
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        TeamId = team.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Skills = MockUsersSkills.GetUserSkillsById(user.Id)
                    };
                }
                else
                {
                    team.Leader = existMember;
                }
            }
            else
            {
                team.Leader = team.Owner;
            }

            return true;
        }

        public static bool KickMember(TeamMember member)
        {
            var team = _teams.FirstOrDefault(t => t.Id == member.TeamId);
            if (team is null) return false;

            return team.Members.Remove(member);
        }

        public static bool DeleteTeam(Team team) => _teams.Remove(team);
    }
}
