﻿using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace InteractivityAddon.Selection
{
    /// <summary>
    /// Represents the default selection which uses messages. This class is immutable!
    /// </summary>
    /// <typeparam name="T">The type of values to select from.</typeparam>
    public sealed class MessageSelection<T> : Selection<T, SocketMessage>
    {
        #region Fields
        /// <summary>
        /// The possibilites to select from.
        /// </summary>
        public ImmutableList<string> Possibilities { get; }

        /// <summary>
        /// Gets the cancel display name if cancel is enabled in the selection.
        /// </summary>
        public string CancelDisplayName { get; }
        #endregion

        #region Constructor
        internal MessageSelection(ImmutableList<T> values, ImmutableList<SocketUser> users, 
            Embed selectionEmbed, Embed cancelledEmbed, Embed timeoutedEmbed, DeletionOption deletion,
            ImmutableList<string> possabilies, string cancelDisplayName)
            : base(values, users, selectionEmbed, cancelledEmbed, timeoutedEmbed, deletion)
        {
            Possibilities = possabilies;
            CancelDisplayName = cancelDisplayName;
        }
        #endregion

        #region Methods
        public override Task<Optional<InteractivityResult<T>>> ParseAsync(SocketMessage value, DateTime startTime)
        {
            int index = Possibilities.FindIndex(x => x == value.Content) / 4;

            return Task.FromResult(Optional.Create(
                index >= Values.Count
                ? new InteractivityResult<T>(default, DateTime.UtcNow - startTime, false, true)
                : new InteractivityResult<T>(Values[index], DateTime.UtcNow - value.Timestamp, false, false)
                ));
        }

        public override Task<bool> RunChecksAsync(BaseSocketClient client, IUserMessage message, SocketMessage value) 
            => Task.FromResult(Possibilities.Contains(value.Content));
        #endregion
    }
}
