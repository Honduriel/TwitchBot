using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TwitchBot
{
    class SurveyCommand : ICommand
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public int AnsweredYes { get; set; }
        public int AnsweredMaybe { get; set; }
        public int AnsweredNo { get; set; }
        public bool IsActive { get; set; }
        public List<string> Participants { get; set; }
        public string Trigger { get; set; }
        public List<string> Options { get; set; }
        public bool IsPublic { get; set; }

        public SurveyCommand()
        {
            Trigger = "!umfrage";
            Options = new List<string>()
            {
                "--t",
                "--s"
            };
            IsPublic = false;
        }
        public string GetSurveyStartedMessage()
        {
            StringBuilder msg = new StringBuilder();
            msg.Append($"Es wurde die Umfrage \"{Title}\" gestartet! ");
            msg.Append("Tippe \"+\", \"-\" oder \"+-\" in den Chat, um teilzunehmen. ");
            msg.Append($"Die Umfrage läuft bis {End.ToString("HH:mm")} Uhr.");


            return msg.ToString();
        }
        public string ReportResults()
        {
            IsActive = false;

            float sum = AnsweredYes + AnsweredNo + AnsweredMaybe;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            string yesPercent = (AnsweredYes == 0) ? "0 %" : (100.0 / sum * AnsweredYes / 100).ToString("p", culture);
            string maybePercent = (AnsweredMaybe == 0) ? "0 %" : (100.0 / sum * AnsweredMaybe / 100).ToString("p", culture);
            string noPercent = (AnsweredNo == 0) ? "0 %" : (100.0 / sum * AnsweredNo / 100).ToString("p", culture);
            StringBuilder results = new StringBuilder();
            results.Append($"Die Umfrage \"{Title}\" wurde beendet. Hier sind die Ergebnisse der {Participants.Count} Teilnehmer: ");
            results.Append($"+: {AnsweredYes} ({yesPercent}) ");
            results.Append($"+-: {AnsweredMaybe} ({maybePercent}) ");
            results.Append($"-: {AnsweredNo} ({noPercent})");

            return results.ToString();
        }
    }
}
