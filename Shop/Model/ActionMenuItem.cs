﻿namespace Shop.Model
{
    class ActionMenuItem: MenuItem
    {
        public string Command { get; set; }

        public ActionMenuItem(string text, string command):base(text)
        {
            Command = command;
        }
    }
}
