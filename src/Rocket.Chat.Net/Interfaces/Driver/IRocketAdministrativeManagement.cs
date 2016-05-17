namespace Rocket.Chat.Net.Interfaces.Driver
{
    using System.Threading.Tasks;

    using Rocket.Chat.Net.Models.MethodResults;

    public interface IRocketAdministrativeManagement
    {
        /// <summary>
        /// Gets statistics about the Rocket.Chat instance. Requires the `view-statistics` permission.
        /// </summary>
        /// <param name="refresh">Should permissions be flushed first</param>
        /// <returns></returns>
        Task<MethodResult<StatisticsResult>> GetStatisticsAsync(bool refresh = false);
    }
}