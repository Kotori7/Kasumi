using DSharpPlus.Entities;

namespace Kasumi;

public class Helpers
{
    /// <summary>
    /// Gets the username for a user, with or without a discriminator depending on if they have migrated their username
    /// </summary>
    /// <param name="user"><see cref="DiscordUser"/> to get the username of</param>
    /// <returns>Username as a string</returns>
    public static string GetUsername(DiscordUser user)
    {
        return user.Discriminator == "0" ? user.Username : $"{user.Username}#{user.Discriminator}";
    }
}