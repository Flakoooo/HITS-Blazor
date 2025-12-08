using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockSkills
    {
        public static string JavaScriptId { get; } = Guid.NewGuid().ToString();
        public static string PythonId { get; } = Guid.NewGuid().ToString();
        public static string RustId { get; } = Guid.NewGuid().ToString();
        public static string SwiftId { get; } = Guid.NewGuid().ToString();
        public static string KotlinId { get; } = Guid.NewGuid().ToString();
        public static string GoId { get; } = Guid.NewGuid().ToString();
        public static string ReactId { get; } = Guid.NewGuid().ToString();
        public static string CppId { get; } = Guid.NewGuid().ToString();
        public static string VueId { get; } = Guid.NewGuid().ToString();
        public static string AngularId { get; } = Guid.NewGuid().ToString();
        public static string NodeId { get; } = Guid.NewGuid().ToString();
        public static string DockerId { get; } = Guid.NewGuid().ToString();
        public static string GitId { get; } = Guid.NewGuid().ToString();
        public static string GrafanaId { get; } = Guid.NewGuid().ToString();
        public static string ElasticsearchId { get; } = Guid.NewGuid().ToString();
        public static string MongoDBId { get; } = Guid.NewGuid().ToString();
        public static string PostgreSQLId { get; } = Guid.NewGuid().ToString();
        public static string MySQLId { get; } = Guid.NewGuid().ToString();
        public static string SQLite { get; } = Guid.NewGuid().ToString();
        public static string RedisId { get; } = Guid.NewGuid().ToString();
        public static string UnrealEngineId { get; } = Guid.NewGuid().ToString();
        public static string CSharpId { get; } = Guid.NewGuid().ToString();
        public static string KerasId { get; } = Guid.NewGuid().ToString();
        public static string ScikitLearnId { get; } = Guid.NewGuid().ToString();

        public static List<Skill> GetMockSkills() => [
            new Skill { Id = JavaScriptId,      Name = "JavaScript",    Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = PythonId,          Name = "Python",        Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = RustId,            Name = "Rust",          Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = SwiftId,           Name = "Swift",         Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = KotlinId,          Name = "Kotlin",        Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = GoId,              Name = "Go",            Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = ReactId,           Name = "React JS",      Type = SkillType.FRAMEWORK,     Confirmed = true },
            new Skill { Id = CppId,             Name = "C++",           Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = VueId,             Name = "VueJS",         Type = SkillType.FRAMEWORK,     Confirmed = true },
            new Skill { Id = AngularId,         Name = "Angular",       Type = SkillType.FRAMEWORK,     Confirmed = true },
            new Skill { Id = NodeId,            Name = "NodeJS",        Type = SkillType.FRAMEWORK,     Confirmed = true },
            new Skill { Id = DockerId,          Name = "Docker",        Type = SkillType.DEVOPS,        Confirmed = true },
            new Skill { Id = GitId,             Name = "Git",           Type = SkillType.DEVOPS,        Confirmed = true },
            new Skill { Id = GrafanaId,         Name = "Grafana",       Type = SkillType.DEVOPS,        Confirmed = true },
            new Skill { Id = ElasticsearchId,   Name = "Elasticsearch", Type = SkillType.DEVOPS,        Confirmed = true },
            new Skill { Id = MongoDBId,         Name = "MongoDB",       Type = SkillType.DATABASE,      Confirmed = true },
            new Skill { Id = PostgreSQLId,      Name = "PostgreSQL",    Type = SkillType.DATABASE,      Confirmed = true },
            new Skill { Id = MySQLId,           Name = "MySQL",         Type = SkillType.DATABASE,      Confirmed = true },
            new Skill { Id = SQLite,            Name = "SQLite",        Type = SkillType.DATABASE,      Confirmed = true },
            new Skill { Id = RedisId,           Name = "Redis",         Type = SkillType.DATABASE,      Confirmed = true },
            new Skill { Id = UnrealEngineId,    Name = "Unreal Engine", Type = SkillType.FRAMEWORK,     Confirmed = true },
            new Skill { Id = CSharpId,          Name = "C#",            Type = SkillType.LANGUAGE,      Confirmed = true },
            new Skill { Id = KerasId,           Name = "Keras",         Type = SkillType.FRAMEWORK,     Confirmed = true },
            new Skill { Id = ScikitLearnId,     Name = "Scikit Learn",  Type = SkillType.FRAMEWORK,     Confirmed = true }
        ];

        public static Skill? GetSkillById(string id)
            => GetMockSkills().FirstOrDefault(s => s.Id == id);

        public static List<Skill> GetSkillsByType(SkillType type)
            => [.. GetMockSkills().Where(s => s.Type == type)];

        public static List<Skill> GetConfirmedSkills()
            => [.. GetMockSkills().Where(s => s.Confirmed)];

        public static Skill? GetSkillByName(string name)
            => GetMockSkills().FirstOrDefault(s => s.Name == name);

        public static List<Skill> GetLanguageSkills()
            => GetSkillsByType(SkillType.LANGUAGE);

        public static List<Skill> GetFrameworkSkills()
            => GetSkillsByType(SkillType.FRAMEWORK);

        public static List<Skill> GetDatabaseSkills()
            => GetSkillsByType(SkillType.DATABASE);

        public static List<Skill> GetDevOpsSkills()
            => GetSkillsByType(SkillType.DEVOPS);
    }
}
