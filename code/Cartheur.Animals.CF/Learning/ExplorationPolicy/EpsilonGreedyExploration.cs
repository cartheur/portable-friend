using System;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Learning.ExplorationPolicy
{
    /// <summary>
    /// Epsilon greedy exploration policy.
    /// </summary>
    /// <remarks><para>The class implements epsilon greedy exploration policy. Acording to the policy, the best action is chosen with probability <b>1-epsilon</b>. Otherwise,
    /// with probability <b>epsilon</b>, any other action, except the best one, is chosen randomly.</para>
    /// <para>According to the policy, the epsilon value is known also as exploration rate.</para>
    /// </remarks>
    public class EpsilonGreedyExploration : IExplorationPolicy
    {
        private double _explorationRate;
        /// <summary>
        /// Exploration rate value, [0, 1].
        /// </summary>
        /// <remarks><para>The value determines the amount of exploration driven by the policy. If the value is high, then the policy drives more to exploration - choosing random
        /// action, which excludes the best one. If the value is low, then the policy is more greedy - choosing the beat so far action.
        /// </para></remarks>
        public double ExplorationRate
        {
            get { return _explorationRate; }
            set { _explorationRate = Math.Max(0.0, Math.Min(1.0, value)); }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EpsilonGreedyExploration"/> class.
        /// </summary>
        /// <param name="explorationRate">Epsilon value (exploration rate).</param>
        public EpsilonGreedyExploration(double explorationRate)
        {
            ExplorationRate = explorationRate;
        }
        /// <summary>
        /// Choose an action.
        /// </summary>
        /// <param name="actionEstimates">Action estimates.</param>
        /// <returns>Returns selected action.</returns>
        /// <remarks>The method chooses an action depending on the provided estimates. The estimates can be any sort of estimate, which values usefulness of the action (expected summary reward, discounted reward, etc).</remarks>
        public int ChooseAction(double[] actionEstimates)
        {
            int actionsCount = actionEstimates.Length;
            // Find the best action (greedy).
            double maxReward = actionEstimates[0];
            int greedyAction = 0;

            for (int i = 1; i < actionsCount; i++)
            {
                if (actionEstimates[i] > maxReward)
                {
                    maxReward = actionEstimates[i];
                    greedyAction = i;
                }
            }
            // Try to do exploration
            if (StaticRandom.NextDouble() < _explorationRate)
            {
                int randomAction = StaticRandom.Next(actionsCount - 1);

                if (randomAction >= greedyAction)
                    randomAction++;

                return randomAction;
            }

            return greedyAction;
        }
    }
}
