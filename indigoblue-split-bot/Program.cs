using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace IndigoBlueSplitBot {
    class Program {
        private static IndigoBlueSplitBot bot;
        
        private static void Main(string[] args) {
            bot = new IndigoBlueSplitBot();

            bot.Main().GetAwaiter().GetResult();
        }
    }
}