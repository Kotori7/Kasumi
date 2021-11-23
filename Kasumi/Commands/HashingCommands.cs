using System;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;

namespace Kasumi.Commands
{
    public class HashingCommands : BaseCommandModule
    {
        private MD5 md5hash = MD5.Create();
        private SHA1 sha1hash = SHA1.Create();
        private SHA256 sha256hash = SHA256.Create();
        private SHA384 sha384hash = SHA384.Create();
        private SHA512 sha512hash = SHA512.Create();
        
        [Command("md5")]
        [Description("Hashes a provided string with MD5. Insecure!")]
        public async Task MD5Hash(CommandContext ctx, params string[] args)
        {
            string input = String.Join(" ", args);
            
            byte[] d = md5hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder s = new StringBuilder();
            
            for(int i = 0; i < d.Length; i++)
                s.Append(d[i].ToString("x2"));

            await ctx.RespondAsync(s.ToString());
        }
        
        [Command("sha1")]
        [Description("Hashes a provided string with SHA-1. Insecure!")]
        public async Task SHA1Hash(CommandContext ctx, params string[] args)
        {
            string input = String.Join(" ", args);
            
            byte[] d = sha1hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder s = new StringBuilder();
            
            for (int i = 0; i < d.Length; i++)
                s.Append(d[i].ToString("x2"));
            
            await ctx.RespondAsync(s.ToString());
        }
        
        [Command("sha256")]
        [Description("Hashes a provided string with SHA-256.")]
        public async Task SHA256Hash(CommandContext ctx, params string[] args)
        {
            string input = String.Join(" ", args);
            
            byte[] d = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder s = new StringBuilder();
            
            for (int i = 0; i < d.Length; i++)
                s.Append(d[i].ToString("x2"));
            
            await ctx.RespondAsync(s.ToString());
        }
        
        [Command("sha384")]
        [Description("Hashes a provided string with SHA-384.")]
        public async Task SHA384Hash(CommandContext ctx, params string[] args)
        {
            string input = String.Join(" ", args);
            
            byte[] d = sha384hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder s = new StringBuilder();
            
            for (int i = 0; i < d.Length; i++)
                s.Append(d[i].ToString("x2"));
            
            await ctx.RespondAsync(s.ToString());
        }
        
        [Command("sha512")]
        [Description("Hashes a provided string with SHA-512.")]
        public async Task SHA512Hash(CommandContext ctx, params string[] args)
        {
            string input = String.Join(" ", args);
            
            byte[] d = sha512hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder s = new StringBuilder();
            
            for (int i = 0; i < d.Length; i++)
                s.Append(d[i].ToString("x2"));
            
            await ctx.RespondAsync(s.ToString());
        }

    }
}
