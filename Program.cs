using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TwitchBot
{
    class Program
    {
        // Bot settings
        private static string _botName = "balasiel";
        private static string _broadcasterName = "balasiel";
        private static string _twitchOAuth = "oauth:9kl1keq3pks4jl1fhro6rsuaipz9cf"; // get chat bot's oauth from www.twitchapps.com/tmi/

        private static SurveyCommand survey = new SurveyCommand();
        private static int surveyEndOffset = 2;

        static void Main(string[] args)
        {
            //Survey survey = new Survey();
            // Initialize and connect to Twitch chat
            IrcClient irc = new IrcClient("irc.chat.twitch.tv", 6667,
                    _botName, _twitchOAuth, _broadcasterName);

            // Ping to the server to make sure this bot stays connected to the chat
            // Server will respond back to this bot with a PONG (without quotes):
            // Example: ":tmi.twitch.tv PONG tmi.twitch.tv :irc.twitch.tv"
            PingSender ping = new PingSender(irc);
            ping.Start();

            // Listen to the chat until program exits
            while (true)
            {
                if (survey.IsActive && survey.End < DateTime.Now)
                {
                    irc.SendPublicChatMessage(survey.ReportResults());
                }

                // Read any message from the chat room
                string message = irc.ReadMessage();
                Console.WriteLine(message); // Print raw irc messages

                if (message.Contains("PRIVMSG"))
                {
                    // Messages from the users will look something like this (without quotes):
                    // Format: ":[user]![user]@[user].tmi.twitch.tv PRIVMSG #[channel] :[message]"

                    // Modify message to only retrieve user and message
                    int intIndexParseSign = message.IndexOf('!');
                    string userName = message.Substring(1, intIndexParseSign - 1); // parse username from specific section (without quotes)
                                                                                   // Format: ":[user]!"
                                                                                   // Get user's message
                    intIndexParseSign = message.IndexOf(" :");
                    message = message.Substring(intIndexParseSign + 2).Trim();

                    Console.WriteLine(message); // Print parsed irc message (debugging only)



                    // Broadcaster commands
                    if (userName.Equals(_broadcasterName))
                    {
                        if (message.Equals("!exitbot"))
                        {
                            irc.SendPublicChatMessage("Bye! Have a beautiful time!");
                            Environment.Exit(0); // Stop the program
                        }
                        if (message.Contains("!umfrage ") || message.Contains("!Umfrage "))
                        {
                            if (message.Contains("--s"))
                            {
                                irc.SendPublicChatMessage(survey.ReportResults());
                            }
                            else
                            {
                                surveyEndOffset = 2;
                                if (message.Contains("--t "))
                                {
                                    surveyEndOffset = Int32.Parse(message.Substring(message.LastIndexOf(' ') + 1));
                                }

                                survey = new SurveyCommand()
                                {
                                    Title = message.Substring(message.IndexOf(' ') + 1),
                                    AnsweredMaybe = 0,
                                    AnsweredNo = 0,
                                    AnsweredYes = 0,
                                    Start = DateTime.Now,
                                    End = DateTime.Now.AddMinutes(surveyEndOffset),
                                    IsActive = true,
                                    Participants = new List<string>()
                                };

                                irc.SendPublicChatMessage(survey.GetSurveyStartedMessage());
                            }
                        }

                    }
                    else
                    {
                        // General commands anyone can use
                        if (survey.IsActive)
                        {
                            if (!survey.Participants.Contains(userName))
                            {
                                if (message.ToLower().Equals("+"))
                                {
                                    survey.AnsweredYes += 1;
                                    survey.Participants.Add(userName);
                                }
                                if (message.ToLower().Equals("-"))
                                {
                                    survey.AnsweredNo += 1;
                                    survey.Participants.Add(userName);
                                }
                                if (message.ToLower().Equals("+-") || message.ToLower().Equals("+/-"))
                                {
                                    survey.AnsweredMaybe += 1;
                                    survey.Participants.Add(userName);
                                }
                            }
                            else
                            {
                                irc.SendPublicChatMessage($"/w {userName} Du kannst nur ein mal pro Umfrage abstimmen. Die aktuelle Umfrage läuft noch bis {survey.End.ToString("hh:mm:ss")}");
                            }

                        }
                        if (message.Equals("!hello"))
                        {
                            irc.SendPublicChatMessage("Hello World!");
                        }

                    }
                }
            }
        }
    }
}