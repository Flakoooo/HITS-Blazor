using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockComments
    {
        private static readonly List<Comment> _comments = CreateComments();

        public static Guid Comment1Id { get; } = Guid.NewGuid();
        public static Guid Comment2Id { get; } = Guid.NewGuid();

        private static List<Comment> CreateComments()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;

            return
            [
                new Comment
                {
                    Id = Comment1Id,
                    IdeaId = MockIdeas.RefactorId,
                    CreatedAt = new DateTime(2023, 10, 20, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Sender = kirill,
                    Text = "Комментарий 1",
                    CheckedBy = []
                },
                new Comment
                {
                    Id = Comment2Id,
                    IdeaId = MockIdeas.RefactorId,
                    CreatedAt = new DateTime(2023, 10, 21, 11, 2, 17, DateTimeKind.Utc).ToString(Settings.DateFormat),
                    Sender = kirill,
                    Text = "Комментарий 2",
                    CheckedBy = [kirill.Id, MockUsers.IvanId]
                }
            ];
        }
    }
}
