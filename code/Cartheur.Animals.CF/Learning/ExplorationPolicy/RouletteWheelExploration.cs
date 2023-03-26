using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Learning.ExplorationPolicy
{
    /// <summary>
    /// Roulette wheel exploration policy.
    /// </summary>
    /// <remarks><para>The class implements roulette whell exploration policy. Acording to the policy,
    /// action <b>a</b> at state <b>s</b> is selected with the next probability:</para>
    /// <code lang="none">
    ///                   Q( s, a )
    /// p( s, a ) = ------------------
    ///              SUM( Q( s, b ) )
    ///               b
    /// </code>
    /// <para>where <b>Q(s, a)</b> is action's <b>a</b> estimation (usefulness) at state <b>s</b>.</para><para><note>The exploration policy may be applied only in cases, when action estimates (usefulness) are represented with positive value greater then 0.</note></para></remarks>
    public class RouletteWheelExploration : IExplorationPolicy
    {
        /// <summary>
        /// Choose an action.
        /// </summary>
        /// <param name="actionEstimates">Action estimates.</param>
        /// <returns>Returns selected action.</returns>
        /// <remarks>The method chooses an action depending on the provided estimates. The estimates can be any sort of estimate, which values usefulness of the action (expected summary reward, discounted reward, etc).</remarks>
        public int ChooseAction( double[] actionEstimates )
        {
            int actionsCount = actionEstimates.Length;
            double actionsSum = 0, estimateSum = 0;

            for ( int i = 0; i < actionsCount; i++ )
            {
                estimateSum += actionEstimates[i];
            }

            // Get random number which determines the action to choose.
            double actionRandomNumber = StaticRandom.NextDouble( );

            for ( int i = 0; i < actionsCount; i++ )
            {
                actionsSum += actionEstimates[i] / estimateSum;
                if ( actionRandomNumber <= actionsSum )
                    return i;
            }

            return actionsCount - 1;
        }
    }
}
