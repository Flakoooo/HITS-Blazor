namespace HITSBlazor.Utils.Models
{
    public abstract class ViewModelBase
    {
        public abstract string GetDisplayInfo();

        public virtual bool MatchesSearch(string searchText)
            => GetDisplayInfo().Contains(searchText, StringComparison.OrdinalIgnoreCase);

        public virtual object GetId() => GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is not ViewModelBase other) return false;
            return GetId().Equals(other.GetId());
        }

        public override int GetHashCode() => GetId().GetHashCode();
    }
}
