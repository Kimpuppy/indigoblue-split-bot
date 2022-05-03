using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace IndigoBlueSplitBot {
    class Program {
        private static IndigoBlueSplitBot bot;
        
        private static void Main(string[] args) {
            Initialize();
            
            bot.Main().GetAwaiter().GetResult();
        }

        private static void Initialize() {
            bot = new IndigoBlueSplitBot();
        }
    }
}