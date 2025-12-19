using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockInvitationTeamToIdeas
    {
        private static readonly List<InvitationTeamToIdea> _invitationTeamToIdeas = CreateInvitationTeamToIdeas();

        public static Guid CarpArmatureId { get; } = Guid.NewGuid();
        public static Guid CarpEMetricsViewerId { get; } = Guid.NewGuid();

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
                    Status = InvitationTeamToIdeaStatus.ANNULLED,
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
                    Status = InvitationTeamToIdeaStatus.NEW,
                    TeamId = carpTeam.Id,
                    InitiatorId = eMetricsViewer.Initiator.Id,
                    TeamName = carpTeam.Name,
                    TeamMembersCount = carpTeam.MembersCount,
                    Skills = eMetricsViewer.Stack
                }
            ];
        }
    }
}
