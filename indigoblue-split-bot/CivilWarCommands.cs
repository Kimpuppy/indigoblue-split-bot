using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

namespace IndigoBlueSplitBot {
    public class CivilWarCommands : ModuleBase<SocketCommandContext> {
        const ulong noticeChannel = 970358574310109234;
        const ulong civilWarNoticeChannel = 970077142438281327;
        const ulong civilWarNormalChannel = 970406358677610586;
        
        [Command("내전공지")]
        public async Task JoinCommand() {
            if (Context.Channel.Id != noticeChannel) {
                return;
            }
            
            var guild = ((SocketGuildChannel)Context.Channel).Guild;
            
            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = new Color(255, 255, 255);
            eb.Title = "**인디고블루 내전 갈라치기 봇**";
            eb.Description = "인디고블루 산하 아싸격리단 내전을 위한 봇입니다.";
            eb.AddField("사용법", "아래 버튼을 누르면 **내전 참가자** 역할이 부여됩니다.");
            
            var cb = new ComponentBuilder()
                .WithButton("역할 부여", "civilwar_add", ButtonStyle.Primary)
                .WithButton("역할 해제", "civilwar_remove", ButtonStyle.Secondary);
            
            await Context.Channel.SendMessageAsync("", false, eb.Build(), components:cb.Build());
        }
        
        [Command("내전팀역할부여기")]
        public async Task SplitTeamWithButtonCommand() {
            if (Context.Channel.Id != civilWarNoticeChannel) {
                return;
            }
            
            var guild = ((SocketGuildChannel)Context.Channel).Guild;
            
            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = new Color(255, 255, 255);
            eb.Title = "**인디고블루 내전 팀 역할 부여기**";
            eb.Description = "인디고블루 산하 아싸격리단 내전 팀 역할 부여를 위한 기능입니다.";
            eb.AddField("사용법", "아래 버튼을 누르면 **팀** 역할이 부여됩니다.", true);
            
            var cb = new ComponentBuilder()
                .WithButton("1팀", "civilwar_team_first", ButtonStyle.Primary)
                .WithButton("2팀", "civilwar_team_second", ButtonStyle.Danger)
                .WithButton("팀 해제", "civilwar_team_remove", ButtonStyle.Secondary);
            
            await Context.Channel.SendMessageAsync("", false, eb.Build(), components:cb.Build());
        }
        
        [Command("내전팀분배")]
        public async Task SplitTeamCommand(params string[] names) {
            if (Context.Channel.Id != civilWarNormalChannel) {
                return;
            }
            
            if (names.Length < 10) {
                await Context.Channel.SendMessageAsync("등록된 멤버가 **10명 미만**입니다.\n멤버를 **10명** 등록 후 다시 시도 해 주시길 바랍니다.");
                return;
            }
            else if (names.Length > 10) {
                await Context.Channel.SendMessageAsync("등록된 멤버가 **10명 이상**입니다.\n멤버를 **10명** 등록 후 다시 시도 해 주시길 바랍니다.");
                return;
            }
            
            var guild = Context.Guild;
            
            var civilWarRole = guild.Roles.FirstOrDefault(x => x.Name == "내전 참가자");
            
            Random random = new Random();
            for (int i = 0; i < names.Length; i++) {
                var tempValue = names[i];
                int randomCount = random.Next(0, names.Length);
                names[i] = names[randomCount];
                names[randomCount] = tempValue;
            }

            var firstTeam = new List<string>();
            var secondTeam = new List<string>();
            for (int i = 0; i < names.Length; i++) {
                if (i < 5) {
                    firstTeam.Add(names[i]);
                }
                else {
                    secondTeam.Add(names[i]);
                }
            }
            
            string firstTeamText = "";
            for (int i = 0; i < firstTeam.Count; i++) {
                firstTeamText += firstTeam[i] + "\n";
            }
            
            string secondTeamText = "";
            for (int i = 0; i < secondTeam.Count; i++) {
                secondTeamText += secondTeam[i] + "\n";
            }
            
            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = new Color(255, 255, 255);
            eb.Title = "**인디고블루 내전 갈라치기 봇**";
            eb.Description = $"<@&{civilWarRole.Id}> 팀 분배가 완료되었습니다.\n팀 역할 부여기를 통해 팀 역할을 부여 해 주시면 됩니다.";
            eb.AddField(firstTeamText, "1팀", true);
            eb.AddField(secondTeamText, "2팀", true);
            
            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }
        
        [Command("내전팀분배사용법")]
        public async Task SplitTeamHelpCommand() {
            if (Context.Channel.Id != civilWarNormalChannel) {
                return;
            }
            
            var guild = ((SocketGuildChannel)Context.Channel).Guild;
            
            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = new Color(255, 255, 255);
            eb.Title = "**인디고블루 내전 팀 분배 사용법**";
            eb.Description = "!내전팀분배 **닉네임(띄어쓰기로 구분)**";
            
            await Context.Channel.SendMessageAsync("", false, eb.Build());
        }
    }
}