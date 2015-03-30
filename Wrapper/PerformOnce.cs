using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Wrapper
{
    /// <summary>
    ///     http://stackoverflow.com/a/5597840
    /// </summary>
    public class PerformOnce
    {
        private class Context
        {
            internal bool Performed; 
        }

        /// <summary>
        ///     Performs the desired Action once per runtime
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Action A(Action action)
        {
            var context = new Context();

            Action ret = () =>
            {
                if (!context.Performed)
                {
                    action();
                    context.Performed = true;
                }
            };

            return ret; 
        }

        /// <summary>
        ///     Performs the desired Func once, and returns the desired type 
        /// </summary>
        /// <example> 
        ///     var perform = PerformOnce.F<List<ChampionSpell>>(in); 
        ///     var first = perform(); // real result
        ///     var second = perform(); // default of type
        /// </example>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<T> F<T>(Func<T> action)
        {
            var context = new Context();

            Func<T> ret = () =>
            {
                if (!context.Performed)
                {
                    context.Performed = true;
                    return action(); 
                }

                return default(T); 
            };

            return ret; 
        }
    }
}
