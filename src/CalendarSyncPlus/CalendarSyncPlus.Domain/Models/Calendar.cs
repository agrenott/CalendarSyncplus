﻿using System.Waf.Foundation;

namespace CalendarSyncPlus.Domain.Models
{
    public class Calendar : Model
    {
        private string _id;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
    }
}