using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Interop;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace IndigoBlueSplitBot {
    public class IndigoBlueSplitBot {
        private DiscordSocketClient client;
        private CommandService commands;
        
        public async Task Main() {
            client = new DiscordSocketClient(new DiscordSocketConfig() {
                LogLevel = LogSeverity.Verbose
            });
            commands = new CommandService(new CommandServiceConfig() {
                LogLevel = LogSeverity.Verbose
            });
            
            client.Log += OnLogRecieved;
            commands.Log += OnLogRecieved;
            
            await client.LoginAsync(TokenType.Bot, "OTY5OTM3MTk0MDk5ODA2MjM4.Ym0qZw.mOTl9Lz8WUTFREap5b_6ObA9f5c");
            await client.StartAsync();
            
            client.MessageReceived += OnMessageReceived;
            client.ButtonExecuted += OnButtonExecuted;
            
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            
            await Task.Delay(-1);
        }
        
        private async Task OnMessageReceived(SocketMessage message) {
            var userMessage = message as SocketUserMessage;
            if (userMessage == null) {
                return;
            }
            
            int pos = 0;
            if (!(userMessage.HasCharPrefix('!', ref pos) || userMessage.HasMentionPrefix(client.CurrentUser, ref pos)) || userMessage.Author.IsBot) {
                return;
            }
            
            var context = new SocketCommandContext(client, userMessage);
            
            var result = await commands.ExecuteAsync(context, pos, null);
        }
        
        private async Task OnButtonExecuted(SocketMessageComponent component) {
            switch(component.Data.CustomId) {
                case "civilwar_team_first":
                    CivilWarRoleAdd(component, true);
                    await component.RespondAsync("**1팀** 역할이 부여되었습니다.", ephemeral: true);
                    break;
                case "civilwar_team_second":
                    CivilWarRoleAdd(component, false);
                    await component.RespondAsync("**2팀** 역할이 부여되었습니다.", ephemeral: true);
                    break;
                case "civilwar_team_release":
                    CivilWarRoleRemove(component);
                    await component.RespondAsync("팀 역할이 해제되었습니다.", ephemeral: true);
                    break;
            }
        }
        
        private Task OnLogRecieved(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task CivilWarRoleAdd(SocketMessageComponent component, bool isFirstTeam) {
            var guild = ((SocketGuildChannel)component.Message.Channel).Guild;
            var user = ((SocketGuildUser)component.User);
            
            var indigoBlueEmote = guild.Emotes.First(e => e.Name == "indigo_blue");
            var indigoRedEmote = guild.Emotes.First(e => e.Name == "indigo_red");
            
            var firstTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 1팀");
            var secondTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 2팀");
            
            if (user.Roles.Contains(isFirstTeam ? secondTeamRole : firstTeamRole)) {
                user.RemoveRoleAsync(isFirstTeam ? secondTeamRole : firstTeamRole);
            }
            
            await user.AddRoleAsync(isFirstTeam ? firstTeamRole : secondTeamRole);
        }
        
        private async Task CivilWarRoleRemove(SocketMessageComponent component) {
            var guild = ((SocketGuildChannel)component.Message.Channel).Guild;
            var user = ((SocketGuildUser)component.User);
            
            var indigoBlueEmote = guild.Emotes.First(e => e.Name == "indigo_blue");
            var indigoRedEmote = guild.Emotes.First(e => e.Name == "indigo_red");
            
            var firstTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 1팀");
            var secondTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 2팀");
            
            if (user.Roles.Contains(firstTeamRole)) {
                await user.RemoveRoleAsync(firstTeamRole);
            }
            
            if (user.Roles.Contains(secondTeamRole)) {
                await user.RemoveRoleAsync(secondTeamRole);
            }
        }
    }
}