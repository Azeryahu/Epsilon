using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using System.IO;
using Discord;
using Microsoft.Extensions.DependencyInjection;

namespace Epsilon.Modules
{
    class HelpCommands : InteractiveBase<SocketCommandContext>
    {
        public static CommandService _commandService;
        public static DatabaseContext _db;
        bool commandResult = false;
        private string VanillaMinecraftFileLocation = "E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\";
        private string ModdedMinecraftFileLocation = string.Empty;
        [Command("Help")]
        public async Task help([Remainder] string commandName = null)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var askingUser = GetUser(user);
            var channel = await Context.User.CreateDMChannelAsync();
            if (commandName == null)
            {
                List<CommandInfo> commands = _commandService.Commands.ToList();
                EmbedBuilder embedBuilder = new EmbedBuilder();
                foreach (CommandInfo command in commands)
                {
                    string embedFieldText = command.Summary ?? "No description available\n";
                    embedBuilder.AddField(command.Name, embedFieldText);
                }
                await ReplyAsync("Here is a list of commands and their description:  ", false, embedBuilder.Build());
                commandResult = true;
            }
            await SendLogMessage(user, "Help", commandResult);
        }
        [Command("Info")]
        public async Task info([Remainder] SocketGuildUser userId = null)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (userId == null)
            {
                var asking = GetUser(user);
                await ReplyAsync("Hello, " + user.Username + ".  Here is some of the information I have on you:\n" +
                    "You have a standing of " + asking.PersonalStanding + " with The Raiders of the Lost Sector.\n" +
                    "You have " + asking.NumberOfWarnings + " number of warnings against you.\n" +
                    "You have tried " + asking.NumberOfAttempts + " without a successful command.\n");
                commandResult = true;
            }
            else
            {

            }
            ResetAttempts(user);
            await SendLogMessage(user, "Info", commandResult);
        }
        [Command("Time")]
        public async Task time(string time = null)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (time != null)
            {
                try
                {

                    await ReplyAsync("The time entered is:  UTC.");
                    commandResult = true;
                }
                catch (Exception e)
                {
                    await ReplyAsync("Unable to complete command \"Time\" properly." + e.Message);
                }
            }
            else
            {
                await ReplyAsync("The current time on deck is:  " + DateTimeOffset.UtcNow.TimeOfDay + " UTC");
                commandResult = true;
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Time", commandResult);
        }
        [Command("Flight Time")]
        [Alias("FT")]
        public async Task flightTime(float distance, string sRate, string distanceMeasurement = null)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            float time = 0;
            float.TryParse(sRate, out float rate);
            if (rate > 30000)
            {
                await ReplyAsync("The maximum rate you can fly is 30,000 km/h.  I have set your speed to this.");
                rate = 30000;
            }
            if (sRate.Equals("max", StringComparison.OrdinalIgnoreCase))
            {
                rate = 30000;
                if (distance > 0 && (distanceMeasurement == null || distanceMeasurement.Equals("SU", StringComparison.OrdinalIgnoreCase)))
                {
                    distance = distance * 200;
                    time = distance / rate; ;
                    if (time < 1)
                    {
                        double minutes = Math.Floor(time * 60);
                        double seconds = Math.Round((time - (minutes / 60)) * 3600);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    else
                    {
                        double hours = Math.Floor(time);
                        double minutes = Math.Floor((time - hours) * 60);
                        double seconds = Math.Round(((time - hours) * 60 - Math.Floor(minutes)) * 60);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        while (minutes >= 60)
                        {
                            hours++;
                            minutes = minutes - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + hours + " hours, " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    commandResult = true;
                }
                else if (distance > 0 && distanceMeasurement.Equals("KM", StringComparison.OrdinalIgnoreCase))
                {
                    time = distance / rate;
                    if (time < 1)
                    {
                        double minutes = Math.Floor(time * 60);
                        double seconds = Math.Round((time - (minutes / 60)) * 3600);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    else
                    {
                        double hours = Math.Floor(time);
                        double minutes = Math.Floor((time - hours) * 60);
                        double seconds = Math.Round(((time - hours) * 60 - Math.Floor(minutes)) * 60);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        while (minutes >= 60)
                        {
                            hours++;
                            minutes = minutes - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + hours + " hours, " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    commandResult = true;
                }
                else if (distance > 0)
                {
                    await ReplyAsync("The distance format was incorrect.");
                }
                else
                {
                    await ReplyAsync("The distance entered must be a positive number.");
                }
            }
            else if (rate > 0)
            {
                if (distance > 0 && distanceMeasurement == null)
                {
                    distance = distance * 200;
                    time = distance / rate;
                    if (time < 1)
                    {
                        double minutes = Math.Floor(time * 60);
                        double seconds = Math.Round((time - (minutes / 60)) * 3600);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    else
                    {
                        double hours = Math.Floor(time);
                        double minutes = Math.Floor((time - hours) * 60);
                        double seconds = Math.Round(((time - hours) * 60 - Math.Floor(minutes)) * 60);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        while (minutes >= 60)
                        {
                            hours++;
                            minutes = minutes - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + hours + " hours, " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    commandResult = true;
                }
                else if (distance > 0 && distanceMeasurement.Equals("KM", StringComparison.OrdinalIgnoreCase))
                {
                    time = distance / rate;
                    if (time < 1)
                    {
                        double minutes = Math.Floor(time * 60);
                        double seconds = Math.Round((time - (minutes / 60)) * 3600);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    else
                    {
                        double hours = Math.Floor(time);
                        double minutes = Math.Floor((time - hours) * 60);
                        double seconds = Math.Round(((time - hours) * 60 - Math.Floor(minutes)) * 60);
                        while (seconds >= 60)
                        {
                            minutes++;
                            seconds = seconds - 60;
                        }
                        while (minutes >= 60)
                        {
                            hours++;
                            minutes = minutes - 60;
                        }
                        await ReplyAsync("You will arrive in aproximately " + hours + " hours, " + minutes + " minutes, and " + seconds + " seconds.  Fly safe!");
                    }
                    commandResult = true;
                }
                else if (distance > 0)
                {
                    await ReplyAsync("The distance format was incorrect.");
                }
                else
                {
                    await ReplyAsync("The distance entered must be a positive number.");
                }
            }
            else
            {
                await ReplyAsync("The speed rate you entered was not valid.  Please try again.");
            }
            await SendLogMessage(user, "Flight Time", commandResult);
        }
        [Command("Flight Speed")]
        [Alias("FS")]
        public async Task flightSpeed(float distance, string sTime, string distanceMeasurement = null)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            double rate = 0;
            float.TryParse(sTime, out float time);
            if (distance > 0 && distanceMeasurement == null)
            {
                rate = Math.Ceiling((distance * 200) / time);
                if (rate > 30000)
                {
                    await ReplyAsync("You will not be able to make that distance at the current max rate of flight in game.");
                }
                else if (rate < 1)
                {
                    await ReplyAsync("You cannot go slow enough and would be better if you waited a bit longer before departing your current location.");
                }
                else
                {
                    await ReplyAsync("You will need to travel at an average rate of:  " + rate + " kph.");
                }
            }
            else if (distance > 0 && distanceMeasurement.Equals("km", StringComparison.OrdinalIgnoreCase))
            {
                rate = distance / time;
                if (rate > 30000)
                {
                    await ReplyAsync("You will not be able to make that distance at the current max rate of flight in game.");
                }
                else if (rate < 1)
                {
                    await ReplyAsync("You cannot go slow enough and would be better if you waited a bit longer before departing your current location.");
                }
                else
                {
                    await ReplyAsync("You will need to travel at an average rate of:  " + rate + " kph.");
                }
            }
            commandResult = true;
            await SendLogMessage(user, "Flight Speed", commandResult);
        }
        [Command("Warp")]
        public async Task warp(float distance, float mass, string massValue = null)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            if (distance <= 500 && distance > 0)
            {
                if (massValue == null)
                {
                    var result = distance * mass * 0.000306659;
                    result = Math.Ceiling(result);
                    await ReplyAsync("You will need " + (int)result + " warp cells to traverse that distance.");
                }
                else if (massValue.Equals("kg", StringComparison.OrdinalIgnoreCase))
                {
                    mass = mass / 1000;
                    var result = distance * mass * 0.000306659;
                    result = Math.Ceiling(result);
                    await ReplyAsync("You will need " + (int)result + " warp cells to traverse that distance.");
                }
            }
            else
            {
                await ReplyAsync("You've entered an invalid distance.  Please try again.");
            }
            commandResult = true;
            await SendLogMessage(user, "DU Status", commandResult);
        }
        [Command("Break Distance", RunMode = RunMode.Async)]
        [Alias("BD")]
        public async Task breakDistance(int velocity, string mass, string breakingForce)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            double breakingDistance = 0;
            float sMass = 0;
            int bForce = 0;
            int mCount = mass.Count() - 1;
            int bCount = breakingForce.Count() - 1;
            string massValueString = string.Empty;
            string forceValueString = string.Empty;
            float velocity2 = velocity * 1000 / 3600;
            if (mass[mCount].Equals('t') || mass[mCount].Equals('T'))
            {
                mCount = mass.Count() - 1;
                int x = 0;
                while (x < mCount)
                {
                    massValueString += mass[x];
                    x++;
                }
                sMass = float.Parse(massValueString);
                sMass = sMass * 1000;
            }
            else if (mass[mCount].Equals('g') || mass[mCount].Equals('G'))
            {
                mCount = mass.Count() - 2;
                int x = 0;
                while (x < mCount)
                {
                    massValueString += mass[x];
                    x++;
                }
                sMass = float.Parse(massValueString);
            }
            else
            {
                try
                {
                    sMass = float.Parse(massValueString);
                }
                catch (Exception)
                {
                    await ReplyAsync("Your mass was not entered correctly.  You will need to try again.");
                }
            }
            if (breakingForce[bCount - 1].Equals('k') || breakingForce[bCount - 1].Equals('K'))
            {
                bCount = breakingForce.Count() - 2;
                int x = 0;
                while (x < bCount)
                {
                    forceValueString += breakingForce[x];
                    x++;
                }
                bForce = int.Parse(forceValueString);
                bForce = bForce * 1000;
            }
            else if (breakingForce[bCount].Equals('N'))
            {
                bCount = breakingForce.Count() - 1;
                int x = 0;
                while (x < bCount)
                {
                    forceValueString += breakingForce[x];
                    x++;
                }
                bForce = int.Parse(forceValueString);
            }
            else
            {
                bForce = int.Parse(forceValueString);
            }
            float maxVelocity2 = 30000 * 1000 / 3600;
            breakingDistance = sMass * Math.Pow(maxVelocity2, 2) / bForce * (1 - Math.Sqrt(1 - Math.Pow(velocity2, 2) / Math.Pow(maxVelocity2, 2)));
            await ReplyAsync("Would you like to round for safety?");
            var response = await NextMessageAsync(true, true, Epsilon.WaitTime);
            if (response != null && response.Content.Equals("yes", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("y",StringComparison.OrdinalIgnoreCase))
            {
                if (breakingDistance > 200000)
                {
                    breakingDistance = Math.Ceiling(breakingDistance / 200000);
                    await ReplyAsync("You will need to start slowing down at a distance of " + breakingDistance + "su from your destination.");
                }
                else if (breakingDistance > 1000)
                {
                    breakingDistance = Math.Ceiling(breakingDistance / 1000);
                    await ReplyAsync("You will need to start slowing down at a distance of " + breakingDistance + "km from your destination.");
                }
            }
            else
            {
                if (breakingDistance > 200000)
                {
                    breakingDistance = breakingDistance / 200000;
                    await ReplyAsync("You will need to start slowing down at a distance of " + breakingDistance + "su from your destination.");
                }
                else if (breakingDistance > 1000)
                {
                    breakingDistance = breakingDistance / 1000;
                    await ReplyAsync("You will need to start slowing down at a distance of " + breakingDistance + "km from your destination.");
                }
            }
            commandResult = true;
            await SendLogMessage(user, "Break Distance", commandResult);
        }
        //Minecraft Commands
        [Command("Minecraft Locations")]
        public async Task MinecraftLocation()
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            var locationsList = _db.MCLocations.ToList();
            string locationsOutputList = string.Empty;
            if (_db.MCLocations.Any(x => x.Looter.Equals(0)))
            {
                foreach (var location in locationsList)
                {
                    if (location.Looter.Equals(0))
                    {
                        locationsOutputList += location.ThingFound + ", x=" + location.XCoordinate + ", z=" + 
                            location.ZCoordinate + ", y=" + location.YCoordinate + "\n";
                    }
                }
                var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotSpamChannelID);
                await channel.SendMessageAsync(locationsOutputList);
            }
            commandResult = true;
            await SendLogMessage(user, "Minecraft Locations", commandResult);
        }
        [Command("Recipe", RunMode = RunMode.Async)]
        public async Task MinecraftRecipe([Remainder] string recipeName)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;

            var blocks = GoogleSheets.MinecraftBlocks();
            if (blocks.Any(x => x.Equals(recipeName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Block Found");
            }
            if (!recipeName.Equals(null))
            {
                var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotSpamChannelID);
                if (recipeName.Equals("Planks", StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendMessageAsync("There are multiple recipes for \"Planks\", \n\nAcacia\nBirch\nDark Oak\nJungle\nOak\nSpruce\n\nwhat logs are you starting with?");
                    var response = await NextMessageAsync(true, true, Epsilon.WaitTime);
                    if (!response.Equals(null))
                    {
                        if (response.Content.Equals("Acacia", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("Acacia Logs", StringComparison.OrdinalIgnoreCase)
                            || response.Content.Equals("Acacia Log", StringComparison.OrdinalIgnoreCase))
                        {
                            await channel.SendFileAsync(VanillaMinecraftFileLocation + "AcaciaPlanks1.png",
                                "This is the recipe for \"Acacia Planks\".");
                            await channel.SendFileAsync(VanillaMinecraftFileLocation + "AcaciaPlanks2.png",
                                "This is the alternate recipe for \"Acacia Planks\".");
                        }
                        if (response.Content.Equals("Birch", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("Birch Logs", StringComparison.OrdinalIgnoreCase)
                            || response.Content.Equals("Birch Log", StringComparison.OrdinalIgnoreCase))
                        {
                            await channel.SendFileAsync(VanillaMinecraftFileLocation + "BirchPlanks1.png",
                                "This is the recipe for \"Birch Planks\".");
                            await channel.SendFileAsync(VanillaMinecraftFileLocation + "BirchPlanks2.png",
                                "This is the alternate recipe for \"Birch Planks\".");
                        }
                        else if (response.Content.Equals("Dark Oak", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("Dark Oak Logs", StringComparison.OrdinalIgnoreCase)
                            || response.Content.Equals("Dark Oak Log", StringComparison.OrdinalIgnoreCase))
                        {
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\DarkOakPlanks1.png",
                                "This is the recipe for \"Dark Oak Planks\".");
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\DarkOakPlanks2.png",
                                "This is the alternate recipe for \"Dark Oak Planks\".");
                        }
                        else if (response.Content.Equals("Jungle", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("Jungle Logs", StringComparison.OrdinalIgnoreCase)
                            || response.Content.Equals("Jungle Log", StringComparison.OrdinalIgnoreCase))
                        {
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\JunglePlanks1.png",
                                "This is the recipe for \"Jungle Planks\".");
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\JunglePlanks2.png",
                                "This is the alternate recipe for \"Jungle Planks\".");
                        }
                        else if (response.Content.Equals("Oak", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("Oak Logs", StringComparison.OrdinalIgnoreCase)
                            || response.Content.Equals("Oak Log", StringComparison.OrdinalIgnoreCase))
                        {
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\OakPlanks1.png",
                                "This is the recipe for \"Oak Planks\".");
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\OakPlanks2.png",
                                "This is the alternate recipe for \"Oak Planks\".");
                        }
                        else if (response.Content.Equals("Spruce", StringComparison.OrdinalIgnoreCase) || response.Content.Equals("Spruce Logs", StringComparison.OrdinalIgnoreCase)
                            || response.Content.Equals("Spruce Log", StringComparison.OrdinalIgnoreCase))
                        {
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\SprucePlanks1.png",
                                "This is the recipe for \"Spruce Planks\".");
                            await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\SprucePlanks2.png",
                                "This is the alternate recipe for \"Spruce Planks\".");
                        }
                    }
                }
                else if (recipeName.Equals("Acacia Planks", StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\AcaciaPlanks1.png",
                        "This is the recipe for \"Acacia Planks\".");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\AcaciaPlanks2.png",
                        "This is the alternate recipe for \"Acacia Planks\".");
                }
                else if (recipeName.Equals("Birch Planks", StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\BirchPlanks1.png",
                        "This is the recipe for \"Birch Planks\".");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\BirchPlanks2.png",
                        "This is the alternate recipe for \"Birch Planks\".");
                }
                else if (recipeName.Equals("Dark Oak Planks", StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\DarkOakPlanks1.png",
                        "This is the recipe for \"Dark Oak Planks\".");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\DarkOakPlanks2.png",
                        "This is the alternate recipe for \"Dark Oak Planks\".");
                }
                else if (recipeName.Equals("Jungle Planks", StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\JunglePlanks1.png",
                        "This is the recipe for \"Jungle Planks\".");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\JunglePlanks2.png",
                        "This is the alternate recipe for \"Jungle Planks\".");
                }
                else if (recipeName.Equals("Oak Planks",StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\OakPlanks1.png",
                        "This is the recipe for \"Oak Planks\".");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\OakPlanks2.png",
                        "This is the alternate recipe for \"Oak Planks\".");
                }
                else if (recipeName.Equals("Spruce Planks",StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\SprucePlanks1.png",
                        "This is the recipe for \"Spruce Planks\".");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftVanillaRecipes\\SprucePlanks2.png",
                        "This is the alternate recipe for \"Spruce Planks\".");
                }
                else if (recipeName.Equals("Cheeseburger", StringComparison.OrdinalIgnoreCase))
                {
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftSimpleFarmingRecipes\\MCSFCheeseBurger1.png",
                        "This is a modded recipe and the primary recipe for a cheeseburger is:");
                    await channel.SendFileAsync("E:\\Users\\Ryan\\source\\repos\\Epsilon\\Epsilon\\MinecraftRecipes\\MinecraftSimpleFarmingRecipes\\MCSFCheeseBurger2.png",
                        "The the alternate recipe is:");
                }
            commandResult = true;
            }
            else
            {
                await ReplyAsync("You did not provide a recipe name. A recipe name is needed in order to look up the coorisponding recipe.");
            }
            await SendLogMessage(user, "Minecraft Recipe", commandResult);
        }
        //Methods
        private async Task SendLogMessage(SocketGuildUser userID, string commandName, bool commandResult)
        {
            var resultString = string.Empty;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotLogChannelID);
            if (commandResult)
            {
                resultString = "successfully";
                if (channel != null)
                {
                    await channel.SendMessageAsync("```" + userID.ToString() + " " + resultString + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
                }
            }
            else if (!commandResult)
            {
                resultString = "unsuccessfully";
                if (channel != null)
                {
                    await channel.SendMessageAsync("```" + userID.ToString() + " " + resultString + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
                }
            }
        }
        private void ResetAttempts(SocketGuildUser user)
        {
            var guest = GetUser(user);
            guest.NumberOfAttempts = 0;
            SaveUser(guest);
        }
        private void SaveUser(User guest)
        {
            var db = new DatabaseContext();
            try
            {
                db.Users.Update(guest);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("User Failed to retrieve. " + e.Message);
            }
        }
        private User GetUser(SocketGuildUser user)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Users.Single(x => x.DiscordUserID == user.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private MCLocation GetLocations(int id)
        {
            try
            {
                return _db.MCLocations.Single(x => x.ID.Equals(id) && x.Looter.Equals(0));
            }
            catch (Exception e)
            {
                Console.WriteLine("The database failed to return a location from the database. " + e.Message);
                return null;
            }
        }
    }
}
