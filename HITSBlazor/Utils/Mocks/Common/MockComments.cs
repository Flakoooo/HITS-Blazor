using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockComments
    {
        public static string Comment1Id { get; } = Guid.NewGuid().ToString();
        public static string Comment2Id { get; } = Guid.NewGuid().ToString();

        public static List<Comment> GetMockComments()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId);

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

        public static Comment? GetCommentById(string id)
            => GetMockComments().FirstOrDefault(c => c.Id == id);

        public static List<Comment> GetCommentsByIdeaId(string ideaId)
            => [.. GetMockComments().Where(c => c.IdeaId == ideaId)];

        public static List<Comment> GetCommentsBySender(string senderId)
            => [.. GetMockComments().Where(c => c.Sender.Id == senderId)];

        public static List<Comment> GetCommentsCheckedByUser(string userIdentifier)
            => [.. GetMockComments().Where(c => c.CheckedBy.Contains(userIdentifier))];

        public static List<Comment> GetUncheckedComments()
            => [.. GetMockComments().Where(c => !c.CheckedBy.Any())];

        public static List<Comment> GetCheckedComments()
            => [.. GetMockComments().Where(c => c.CheckedBy.Any())];

        public static List<Comment> GetRecentComments(int count = 10)
            => [.. GetMockComments()
                  .OrderByDescending(c => c.CreatedAt)
                  .Take(count)];

        public static bool IsCommentCheckedByUser(string commentId, string userIdentifier)
        {
            var comment = GetCommentById(commentId);
            return comment?.CheckedBy.Contains(userIdentifier) ?? false;
        }

        public static void MarkCommentAsChecked(string commentId, string userIdentifier)
        {
            var comment = GetCommentById(commentId);
            if (comment != null && !comment.CheckedBy.Contains(userIdentifier))
            {
                comment.CheckedBy.Add(userIdentifier);
            }
        }

        public static void MarkCommentAsUnchecked(string commentId, string userIdentifier)
        {
            var comment = GetCommentById(commentId);
            comment?.CheckedBy.Remove(userIdentifier);
        }

        public static int GetCheckCount(string commentId)
            => GetCommentById(commentId)?.CheckedBy.Count ?? 0;
    }
}
