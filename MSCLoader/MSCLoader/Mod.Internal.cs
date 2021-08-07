using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSCLoader
{
    public partial class Mod
    {
        /// <summary>
        /// true if mod is disabled
        /// </summary>
        public bool isDisabled { get; internal set; }
        internal bool hasUpdate;
        internal int modErrors;
        internal string compiledVersion;
        internal string fileName;
        internal ModsManifest metadata;
        internal ModsManifest RemMetadata;

    }
}
