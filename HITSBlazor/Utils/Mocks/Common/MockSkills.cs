using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Utils.Mocks.Common
{
    public static class MockSkills
    {
        private static readonly List<Skill> _skills = CreateSkills();

        public static Guid JavaScriptId { get; } = Guid.NewGuid();
        public static Guid PythonId { get; } = Guid.NewGuid();
        public static Guid RustId { get; } = Guid.NewGuid();
        public static Guid SwiftId { get; } = Guid.NewGuid();
        public static Guid KotlinId { get; } = Guid.NewGuid();
        public static Guid GoId { get; } = Guid.NewGuid();
        public static Guid ReactId { get; } = Guid.NewGuid();
        public static Guid CppId { get; } = Guid.NewGuid();
        public static Guid VueId { get; } = Guid.NewGuid();
        public static Guid AngularId { get; } = Guid.NewGuid();
        public static Guid NodeId { get; } = Guid.NewGuid();
        public static Guid DockerId { get; } = Guid.NewGuid();
        public static Guid GitId { get; } = Guid.NewGuid();
        public static Guid GrafanaId { get; } = Guid.NewGuid();
        public static Guid ElasticsearchId { get; } = Guid.NewGuid();
        public static Guid MongoDBId { get; } = Guid.NewGuid();
        public static Guid PostgreSQLId { get; } = Guid.NewGuid();
        public static Guid MySQLId { get; } = Guid.NewGuid();
        public static Guid SQLite { get; } = Guid.NewGuid();
        public static Guid RedisId { get; } = Guid.NewGuid();
        public static Guid UnrealEngineId { get; } = Guid.NewGuid();
        public static Guid CSharpId { get; } = Guid.NewGuid();
        public static Guid KerasId { get; } = Guid.NewGuid();
        public static Guid ScikitLearnId { get; } = Guid.NewGuid();

        private static List<Skill> CreateSkills() => [
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

        public static Skill? GetSkillById(Guid id)
            => _skills.FirstOrDefault(s => s.Id == id);

        public static List<Skill> GetAllSkills() => _skills;
    }
}
