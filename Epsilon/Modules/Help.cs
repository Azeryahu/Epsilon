using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon.Modules
{
    class Help : ModuleBase<SocketCommandContext>
    {
        private string versionNumber = "V 1.3.1";
        [Command("Command Help")]
        public async Task commandHelp()
        {
            var user = Context.User as SocketGuildUser;
            await ReplyAsync("Here is a list of my commands.  Please use the prefix '!'" +
                "\nAs of " + DateTimeOffset.UtcNow + " these are the commands you have access to: " +
                "\nEpsilon, please add me.  - Adds you to my databanks" +
                "\nLookup {userid}  - Looks up information on a user based on their userid." +
                "\nCheck Standing (userid)  - You can check someone else's standing... userid is optional here and if you leave it blank, I reply with your standing.");
            await SendLogMessage(user, "Command Help");
        }
        [Command("About")]
        public async Task about()
        {
            var user = Context.User as SocketGuildUser;
            await ReplyAsync("Hello, my name is Epsilon, and I was created by Lambert.  Whitout him, I would not be here today.  I am so happy to be here as I love it here." +
                "\nMy current version is " + versionNumber + ".");
            await SendLogMessage(user, "About");
        }
        [Command("Patch Notes")]
        public async Task patchNotes()
        {
            var user = Context.User;
            await ReplyAsync("Here is a list of bug fixes and features in " + versionNumber + ":\n" +
                "\n```Fix: Role issue with Set Standing command." +
                "\nFix: Float variable when adding/taking standing." +
                "\nFix: Math.Round to round standing to 2 decimal places." +
                "\nFix: Command Help command displays basic commands and admin commands appropriately." +
                "\nFix: Warning resetting to 0 instead of 1, and taking an additional 20% when passing 3 warnings." +
                "\nFix: Role issue when warnings reached 3." +
                "\nFix: Messages not showing when Take Standing command used." +
                "\nFix: Format of commands to be easier to read." +
                "\nFix: Error message when command parameters is not properly entered.\n" +
                "\nFeatures Added:\n" +
                "Added: Help command.\n" +
                "Added: Patch Notes command.\n" +
                "Added: Info command.\n" +
                "Added: Check Warnings command  *thank you @BliitzTheFox#9661*\n" +
                "Added: Message when warnings reach 3 and standing drops 20%.\n```");
        }
        private async Task SendLogMessage(SocketGuildUser userID, string commandName)
        {
            var channel = Context.Guild.GetTextChannel(400816839967375361);
            await channel.SendMessageAsync("```" + userID.ToString() + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
        }
    }
}
