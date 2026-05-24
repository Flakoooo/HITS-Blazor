using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockInvitationTeamToIdeas
    {
        public static Guid CarpArmatureId { get; } = Guid.NewGuid();
        public static Guid CarpEMetricsViewerId { get; } = Guid.NewGuid();

        private static readonly List<InvitationTeamToIdea> _invitationTeamToIdeas = CreateInvitationTeamToIdeas();

        private static List<InvitationTeamToIdea> CreateInvitationTeamToIdeas()
        {
            var armatureIdea = MockIdeaMarkets.GetIdeaMarketById(MockIdeaMarkets.ArmatureId)!;
            var eMetricsViewer = MockIdeaMarkets.GetIdeaMarketById(MockIdeaMarkets.EMetricsViewerId)!;

            var carpTeam = MockTeams.GetTeamById(MockTeams.CarpId)!;

            return
            [
                new InvitationTeamToIdea
                {
                    Id = CarpArmatureId,
                    IdeaId = armatureIdea.Id,
                    IdeaName = armatureIdea.Name,
                    Status = TeamRequestStatus.Annulled,
                    TeamId = carpTeam.Id,
                    InitiatorId = armatureIdea.Initiator.Id,
                    TeamName = carpTeam.Name,
                    TeamMembersCount = carpTeam.MembersCount,
                    Skills = armatureIdea.Stack
                },
                new InvitationTeamToIdea
                {
                    Id = CarpEMetricsViewerId,
                    IdeaId = eMetricsViewer.Id,
                    IdeaName = eMetricsViewer.Name,
                    Status = TeamRequestStatus.New,
                    TeamId = carpTeam.Id,
                    InitiatorId = eMetricsViewer.Initiator.Id,
                    TeamName = carpTeam.Name,
                    TeamMembersCount = carpTeam.MembersCount,
                    Skills = eMetricsViewer.Stack
                }
            ];
        }

        public static void AnnulledInvitationByTeamId(Guid teamId)
        {
            foreach (var invitation in _invitationTeamToIdeas.Where(i => i.TeamId == teamId))
                invitation.Status = TeamRequestStatus.Annulled;
        }

        public static ListDataResponse<InvitationTeamToIdea> GetInvitationsTeamToIdeas(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            Guid? ideaMarketId = null,
            string? searchText = null
        )
        {
            IQueryable<InvitationTeamToIdea> query;

            query = ((teamId, ideaMarketId) switch
            {
                (null, null) => _invitationTeamToIdeas,
                (null, _) => _invitationTeamToIdeas.Where(itti => itti.IdeaId == ideaMarketId),
                (_, null) => _invitationTeamToIdeas.Where(itti => itti.TeamId == teamId),
                (_, _) => _invitationTeamToIdeas.Where(itti => itti.TeamId == teamId && itti.IdeaId == ideaMarketId)
            }).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = (teamId, ideaMarketId) switch
                {
                    (null, _) => query.Where(itti => itti.IdeaName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)),
                    (_, null) => query.Where(itti => itti.TeamName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)),
                    (_, _) => query.Where(itti => itti.IdeaName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) 
                        || itti.IdeaName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                };
            }

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<InvitationTeamToIdea>(count, query.ToList());
        }

        public static List<InvitationTeamToIdea> GetTeamInvitationsForCurrentIdeaMarketsAndTeam(
            Guid teamId,
            HashSet<Guid> ideaMarketIds
        ) => _invitationTeamToIdeas.Where(rtti => rtti.TeamId == teamId && ideaMarketIds.Contains(rtti.IdeaId)).ToList();

        public static InvitationTeamToIdea? CreateNewInvitation(Guid teamId, Guid ideaMarketId)
        {
            var team = MockTeams.GetTeamById(teamId);
            var ideaMarket = MockIdeaMarkets.GetIdeaMarketById(ideaMarketId);

            if (team is null || ideaMarket is null) return null;

            var newInvitation = new InvitationTeamToIdea
            {
                Id = Guid.NewGuid(),
                IdeaId = ideaMarket.Id,
                IdeaName = ideaMarket.Name,
                Status = TeamRequestStatus.New,
                TeamId = team.Id,
                InitiatorId = ideaMarket.Initiator.Id,
                TeamName = team.Name,
                TeamMembersCount = team.MembersCount,
                Skills = ideaMarket.Stack
            };

            _invitationTeamToIdeas.Add(newInvitation);
            return newInvitation;
        }

        public static bool UpdateStatus(Guid invitationId, TeamRequestStatus newStatus)
        {
            var invitation = _invitationTeamToIdeas.FirstOrDefault(i => i.Id == invitationId);
            if (invitation is null) return false;

            if (newStatus is TeamRequestStatus.Accepted)
            {
                var acceptedTeam = MockTeams.GetTeamById(invitation.TeamId);
                var ideaMarket = MockIdeaMarkets.GetIdeaMarketById(invitation.IdeaId);

                if (acceptedTeam is null || ideaMarket is null) return false;

                acceptedTeam.IsAcceptedToIdea = true;

                ideaMarket.Team = acceptedTeam;
                ideaMarket.Status = IdeaMarketStatusType.RecruitmentIsClosed;

                AnnulledInvitationByTeamId(invitation.TeamId);
                MockRequestTeamToIdeas.AnnulledRequestByTeamId(invitation.TeamId);
            }

            invitation.Status = newStatus;

            return true;
        }
    }
}
