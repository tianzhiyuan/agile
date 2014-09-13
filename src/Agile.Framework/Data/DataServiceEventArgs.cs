﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agile.Common.Data;

namespace Agile.Framework.Data
{

    public class DataServiceEventArgs : EventArgs
    {
        public IModel[] Items { get; internal set; }
    }
}
