using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Tests
{
    public static class MockTestAnswers
    {
        private static readonly Random _random = new();

        private static readonly List<TestAnswer> _testAnswers = CreateTestAnswer();

        private static string BelbinTestName => MockTests.GetTestById(MockTests.BelbinId)!.TestName;
        private static string TemperTestName => MockTests.GetTestById(MockTests.TemperId)!.TestName;
        private static string MindTestName => MockTests.GetTestById(MockTests.MindId)!.TestName;

        private static List<TestAnswer> GenerateBelbinTestAnswers(User user)
        {
            var answers = new List<TestAnswer>();

            // Получаем все вопросы теста
            var questions = MockTestQuestions.GetTestQuestionsByTestName(BelbinTestName);

            for (int i = 1; i <= 7; ++i)
            {
                // Получаем вопросы модуля
                var module1Questions = questions.Where(q => q.QuestionModuleNumber == i).ToList();

                // можно ответить минимум 2 балла, на вопрос,
                // ответить можно максимум на 3 вопроса,
                // сумма баллов должна быть 10

                // получаем то, сколько вопросов было выбрано в модуле
                var answersInModuleCount = _random.Next(1, 4);

                // заранее задаем количество баллов
                var scores = new List<int>();
                if (answersInModuleCount == 1) scores.Add(10);
                else if (answersInModuleCount == 2)
                {
                    var firstAnswer = _random.Next(2, 9);
                    scores.AddRange(firstAnswer, 10 - firstAnswer);
                }
                else if (answersInModuleCount == 3)
                {
                    var firstAnswer = _random.Next(2, 7);
                    var secondAnswer = _random.Next(2, 7 - firstAnswer);
                    scores.AddRange(firstAnswer, secondAnswer, 10 - firstAnswer - secondAnswer);
                }

                // находим те самые вопросы, присваиваем сразу им баллы
                var answersInModule = new Dictionary<string, string>();
                for (int j = 0; j < answersInModuleCount; ++j)
                {
                    var currentQuestion = false;

                    while (currentQuestion)
                    {
                        var question = module1Questions[_random.Next(0, 8)];
                        if (!answersInModule.TryGetValue(question.QuestionName, out string? _))
                        {
                            answersInModule[question.QuestionName] = $"{scores[j]}";
                            currentQuestion = true;
                        }
                    }
                }

                // создаем ответы на вопросы, указывая там где нужно сгенерированный ответ
                foreach (var question in module1Questions)
                {
                    answers.Add(
                        new TestAnswer
                        {
                            Id = Guid.NewGuid().ToString(),
                            TestName = question.TestName,
                            User = user,
                            QuestionName = question.QuestionName,
                            QuestionModuleNumber = question.QuestionModuleNumber,
                            QuestionNumber = question.QuestionNumber,
                            Answer = answersInModule.GetValueOrDefault(question.QuestionName) ?? "0"
                        }
                    );
                }
            }

            return answers;
        }

        private static List<TestAnswer> CreateTestAnswer()
        {
            return GenerateBelbinTestAnswers(MockUsers.GetUserById(MockUsers.KirillId)!);
        }

        public static List<TestAnswer> GetTestAnswersByTestNameAndUserId(string testName, string userId) =>
            [.. _testAnswers.Where(ta => ta.TestName == testName && ta.User.Id == userId)];
    }
}
