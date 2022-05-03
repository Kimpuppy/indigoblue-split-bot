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
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
            });
            commands = new CommandService(new CommandServiceConfig() {
                LogLevel = LogSeverity.Verbose,
            });
            
            client.Log += OnLogRecieved;
            commands.Log += OnLogRecieved;
            
            await client.LoginAsync(TokenType.Bot, "token");
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
                case "civilwar_add":
                    await CivilWarRoleAdd(component);
                    break;
                case "civilwar_remove":
                    await CivilWarRoleRemove(component);
                    break;
                case "civilwar_team_first":
                    await CivilWarTeamRoleAdd(component, true);
                    break;
                case "civilwar_team_second":
                    await CivilWarTeamRoleAdd(component, false);
                    break;
                case "civilwar_team_remove":
                    await CivilWarTeamRoleRemove(component);
                    break;
            }
        }
        
        private Task OnLogRecieved(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        
        private async Task CivilWarRoleAdd(SocketMessageComponent component) {
            var guild = ((SocketGuildChannel)component.Message.Channel).Guild;
            var user = ((SocketGuildUser)component.User);
            
            var civilWarRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 참가자");
            
            await user.AddRoleAsync(civilWarRole);
            
            await component.RespondAsync("**내전 참가자** 역할이 부여되었습니다.", ephemeral: true);
        }
        
        private async Task CivilWarRoleRemove(SocketMessageComponent component) {
            var guild = ((SocketGuildChannel)component.Channel).Guild;
            var user = ((SocketGuildUser)component.User);
            
            var civilWarRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 참가자");
            
            if (user.Roles.Contains(civilWarRole) == false) {
                await component.RespondAsync("내전에 참여하고 있지 않습니다.", ephemeral: true);
                return;
            }
            
            await user.RemoveRoleAsync(civilWarRole);
            
            await component.RespondAsync("**내전 참가자** 역할이 해제되었습니다.", ephemeral: true);
        }

        private async Task CivilWarTeamRoleAdd(SocketMessageComponent component, bool isFirstTeam) {
            var guild = ((SocketGuildChannel)component.Channel).Guild;
            var user = ((SocketGuildUser)component.User);
            
            var civilWarRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 참가자");
            var firstTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 1팀");
            var secondTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 2팀");
            
            if (user.Roles.Contains(civilWarRole) == false) {
                await component.RespondAsync("내전 참가자가 아닙니다.\n공지를 확인 해 주세요.", ephemeral: true);
                return;
            }
            
            if (user.Roles.Contains(isFirstTeam ? secondTeamRole : firstTeamRole)) {
                await user.RemoveRoleAsync(isFirstTeam ? secondTeamRole : firstTeamRole);
            }
            
            
            await user.AddRoleAsync(isFirstTeam ? firstTeamRole : secondTeamRole);
            
            await component.RespondAsync("**" + (isFirstTeam ? "1팀" : "2팀") + "** 역할이 부여되었습니다.", ephemeral: true);
        }
        
        private async Task CivilWarTeamRoleRemove(SocketMessageComponent component) {
            var guild = ((SocketGuildChannel)component.Message.Channel).Guild;
            var user = ((SocketGuildUser)component.User);
            
            var civilWarRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 참가자");
            var firstTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 1팀");
            var secondTeamRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 2팀");
            
            if (user.Roles.Contains(civilWarRole) == false) {
                component.RespondAsync("내전 참가자가 아닙니다.\n공지를 확인 해 주세요.", ephemeral: true);
                return;
            }
            
            if (user.Roles.Contains(firstTeamRole)) {
                await user.RemoveRoleAsync(firstTeamRole);
            }
            else if (user.Roles.Contains(secondTeamRole)) {
                await user.RemoveRoleAsync(secondTeamRole);
            }
            else {
                component.RespondAsync("팀에 소속되어있지 않습니다.", ephemeral: true);
                return;
            }
            
            await component.RespondAsync("팀 역할이 해제되었습니다.", ephemeral: true);
        }
    }
}