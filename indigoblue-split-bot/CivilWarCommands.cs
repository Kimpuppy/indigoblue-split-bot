using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

namespace IndigoBlueSplitBot {
    public class CivilWarCommands : ModuleBase<SocketCommandContext> {
        [Command("내전")]
        public async Task SplitTeamCommand() {
            if (Context.Channel.Id != 970077142438281327) {
                return;
            }
            
            var guild = ((SocketGuildChannel)Context.Channel).Guild;
            
            var emote1 = guild.Emotes.First(e => e.Name == "indigo_blue");
            var emote2 = guild.Emotes.First(e => e.Name == "indigo_red");
            
            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = new Color(255, 255, 255);
            eb.Title = "__**인디고블루 내전 갈라치기 봇**__";
            eb.Description = "*인디고블루 산하 아싸격리소 내전 팀 갈라치기를 위한 봇입니다.*";
            eb.AddField("**사용법**", "아래 버튼을 누르면 역할이 부여됩니다.");
            
            var cb = new ComponentBuilder()
                .WithButton("1팀", "civilwar_team_first", ButtonStyle.Primary)
                .WithButton("2팀", "civilwar_team_second", ButtonStyle.Secondary)
                .WithButton("팀 해제", "civilwar_team_release", ButtonStyle.Danger);
            
            var msg = await Context.Channel.SendMessageAsync("", false, eb.Build(), components:cb.Build());
        }
    }
}