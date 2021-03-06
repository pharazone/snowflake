﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Scraping
{
    public class Seed : ISeed
    {
        public string Source { get; }
        public Guid Guid { get; }
        public Guid Parent { get; }

        public SeedContent Content { get; }

        public Seed(SeedContent content, ISeed parent, string source)
        {
            this.Content = content;
            this.Guid = Guid.NewGuid();
            this.Parent = parent.Guid;
            this.Source = source;
        }

        internal Seed(SeedContent content, Guid guid, Guid parent, string source)
        {
            this.Content = content;
            this.Guid = guid;
            this.Parent = parent;
            this.Source = source;
        }
    }
}
