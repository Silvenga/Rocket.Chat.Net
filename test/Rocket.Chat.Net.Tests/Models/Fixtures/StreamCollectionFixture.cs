namespace Rocket.Chat.Net.Tests.Models.Fixtures
{
    public class StreamCollectionFixture
    {
        public string Id { get; set; }

        public string Username { get; set; }

        private bool Equals(StreamCollectionFixture other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Username, other.Username);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((StreamCollectionFixture) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id?.GetHashCode() ?? 0) * 397) ^ (Username?.GetHashCode() ?? 0);
            }
        }
    }
}