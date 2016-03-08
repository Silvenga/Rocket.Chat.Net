namespace Rocket.Chat.Net.Models.Results
{
    public abstract class BaseResult
    {
        public ErrorData ErrorData { get; set; }

        public bool HasError => ErrorData != null;
    }
}