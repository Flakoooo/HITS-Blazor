namespace HITSBlazor.Services.Teams
{
    public record class TeamsFilter(
        string? SearchText = null,
        bool? Privacy = null,
        bool? Survey = null,
        bool? HasActiveProject = null,
        HashSet<Guid>? SearchSkillIds = null,
        string? OrderBy = null,
        bool? ByDescending = null
    );
}
