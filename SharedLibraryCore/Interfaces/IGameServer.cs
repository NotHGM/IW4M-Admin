using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using SharedLibraryCore.Database.Models;

namespace SharedLibraryCore.Interfaces
{
    public interface IGameServer
    {
        /// <summary>
        ///     kicks target on behalf of origin for given reason
        /// </summary>
        /// <param name="reason">reason client is being kicked</param>
        /// <param name="target">client to kick</param>
        /// <param name="origin">source of kick action</param>
        /// <param name="previousPenalty">previous penalty the kick is occuring for (if applicable)</param>
        /// <returns></returns>
        Task Kick(string reason, EFClient target, EFClient origin, EFPenalty previousPenalty = null);
        DateTime? MatchEndTime { get; }
        DateTime? MatchStartTime { get; }
        IReadOnlyList<EFClient> ConnectedClients { get; }
        Reference.Game GameCode { get; }
        bool IsLegacyGameIntegrationEnabled { get; }
        string Id { get; }
        string ListenAddress { get; }
        int ListenPort { get; }
        string ServerName { get; }
        string Gametype { get; }
        string GamePassword { get; }
        Map Map { get; }
        [Obsolete("Use Id")]
        long LegacyEndpoint { get; }
        long LegacyDatabaseId { get; }
    }
}
