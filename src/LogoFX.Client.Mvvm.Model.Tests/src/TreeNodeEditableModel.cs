using System;
using System.Collections.Generic;
using System.Net;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class TreeNodeEditableModel : EditableModel
    {
        public string Value { get; set; }

        public TreeNodeEditableModel Parent { get; set; }

        public TreeNodeEditableModel Next { get; set; }
    }
}