using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Common.DX
{
    /// <summary>
    ///     Abstract DXItem 
    /// </summary>
    public abstract class DXItem
    {
        /// <summary>
        ///     Determines if the DXItem is valid
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        ///     Determines if the DXItem is Enabled 
        /// </summary>
        public abstract bool IsEnabled { get; set; }

        public abstract Vector2 Position { get; set; }

        public abstract void Add();
        public abstract void Remove();

    }
}
