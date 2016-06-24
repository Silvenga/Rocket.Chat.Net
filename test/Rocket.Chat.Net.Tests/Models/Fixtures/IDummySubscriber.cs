namespace Rocket.Chat.Net.Tests.Models.Fixtures
{
    using Rocket.Chat.Net.Collections;

    public interface IDummySubscriber
    {
        void React(object sender, StreamCollectionEventArgs streamCollectionEventArgs);
    }
}