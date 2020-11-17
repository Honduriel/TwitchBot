using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    interface ICommand
    {
        public string Trigger { get; set; }
        public List<string> Options { get; set; }
        public bool IsPublic { get; set; }
    }
}
