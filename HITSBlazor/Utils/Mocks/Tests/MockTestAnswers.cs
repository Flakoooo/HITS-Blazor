using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Tests
{
    public static class MockTestAnswers
    {
        private static readonly Random _random = new();

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
                var moduleQuestions = questions.Where(q => q.QuestionModuleNumber == i).ToList();

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
                    var secondAnswer = _random.Next(2, 10 - firstAnswer);
                    scores.AddRange(firstAnswer, secondAnswer, 10 - firstAnswer - secondAnswer);
                }

                var answersInModule = new Dictionary<string, string>();

                // cоздаем список индексов всех вопросов модуля, мешаем и берем нужное количество
                var questionIndices = Enumerable.Range(0, moduleQuestions.Count)
                    .OrderBy(x => _random.Next())
                    .Take(answersInModuleCount)
                    .ToList();

                // присваиваем баллы
                for (int j = 0; j < answersInModuleCount; ++j)
                {
                    var question = moduleQuestions[questionIndices[j]];
                    answersInModule[question.QuestionName] = $"{scores[j]}";
                }

                // создаем ответы на вопросы, указывая там где нужно сгенерированный ответ
                foreach (var question in moduleQuestions)
                {
                    answers.Add(
                        new TestAnswer
                        {
                            Id = Guid.NewGuid(),
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

        private static List<TestAnswer> GenerateTemperTestAnswers(User user)
        {
            var answers = new List<TestAnswer>();

            var questions = MockTestQuestions.GetTestQuestionsByTestName(TemperTestName);

            foreach (var question in questions)
            {
                var answer = _random.Next(0, 2);

                answers.Add(
                    new TestAnswer
                    {
                        Id = Guid.NewGuid(),
                        TestName = question.TestName,
                        User = user,
                        QuestionName = question.QuestionName,
                        QuestionModuleNumber = question.QuestionModuleNumber,
                        QuestionNumber = question.QuestionNumber,
                        Answer = answer == 0 ? "-" : "+"
                    }
                );
            }

            return answers;
        }

        private static List<TestAnswer> GenerateMindTestAnswers(User user)
        {
            var answers = new List<TestAnswer>();
            var questions = MockTestQuestions.GetTestQuestionsByTestName(MindTestName);

            var modules = questions.GroupBy(q => q.QuestionModuleNumber);

            foreach (var module in modules)
            {
                var moduleQuestions = module.OrderBy(q => q.QuestionNumber).ToList();

                var scores = Enumerable.Range(1, 5).OrderBy(x => _random.Next()).ToList();

                for (int i = 0; i < moduleQuestions.Count; i++)
                {
                    answers.Add(new TestAnswer
                    {
                        Id = Guid.NewGuid(),
                        TestName = MindTestName,
                        User = user,
                        QuestionName = moduleQuestions[i].QuestionName,
                        QuestionModuleNumber = module.Key,
                        QuestionNumber = moduleQuestions[i].QuestionNumber,
                        Answer = scores[i].ToString()
                    });
                }
            }

            return answers;
        }

        private static readonly List<TestAnswer> _testAnswers = CreateTestAnswer();

        private static List<TestAnswer> CreateTestAnswer()
        {
            return
            [
                ..GenerateBelbinTestAnswers(MockUsers.GetUserById(MockUsers.KirillId)!),
                ..GenerateTemperTestAnswers(MockUsers.GetUserById(MockUsers.KirillId)!),
                ..GenerateMindTestAnswers(MockUsers.GetUserById(MockUsers.KirillId)!)
            ];
        }

        public static List<TestAnswer> GetTestAnswersByTestNameAndUserId(string testName, Guid userId) =>
            [.. _testAnswers.Where(ta => ta.TestName == testName && ta.User.Id == userId)];
    }
}
