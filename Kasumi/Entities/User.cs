using System;
using DSharpPlus.Entities;

namespace Kasumi.Entities
{
    // Kasumi's User object. Mainly used for permission tracking and updating the database.
    public class User
    {
        ulong Id;
        string Name;
        string Discriminator;
        string FullName;
        string AvatarUrl;
        DateTime CreationDate;
        public User(DiscordUser user)
        {
            Id = user.Id;
            Name = user.Username;
            Discriminator = user.Discriminator;
            FullName = Name + "#" + Discriminator;
            AvatarUrl = user.AvatarUrl;
            CreationDate = user.CreationTimestamp.UtcDateTime;
        }
    }
}
