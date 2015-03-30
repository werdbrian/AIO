using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Wrapper
{
    public interface IStepable
    {

        /// <summary>
        ///     Determines if the current stepable is valid and able to proceed
        /// </summary>
        bool Valid { get; }

        /// <summary>
        ///     A "step" action 
        /// </summary>
        void Step();
    }
}
