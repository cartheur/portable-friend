using System;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Learning.ExplorationPolicy
{
    /// <summary>
    /// Boltzmann distribution exploration policy.
    /// </summary>
    /// <remarks><para>The class implements exploration policy base on Boltzmann distribution. Acording to the policy, action <b>a</b> at state <b>s</b> is selected with the next probability:</para>
    /// <code lang="none">
    ///                   exp( Q( s, a ) / t )
    /// p( s, a ) = -----------------------------
    ///              SUM( exp( Q( s, b ) / t ) )
    ///               b
    /// </code>
    /// <para>where <b>Q(s, a)</b> is action's <b>a</b> estimation (usefulness) at state <b>s</b> and
    /// <b>t</b> is <see cref="TemperatureParameter"/>.</para>
    /// </remarks> 
    public class BoltzmannExploration : IExplorationPolicy
    {
        private double _temperatureParameter;
        private const double Epsilon = 1E-3;
        /// <summary>
        /// Termperature parameter of Boltzmann distribution, > 0.
        /// </summary>
        /// <remarks><para>The property sets the balance between exploration and greedy actions. If temperature is low, then the policy tends to be more greedy.</para></remarks>
        public double TemperatureParameter
        {
            get { return _temperatureParameter; }
            set { _temperatureParameter = Math.Max(0, value); }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BoltzmannExploration"/> class.
        /// </summary>
        /// <param name="temperatureParameter">Termperature parameter of Boltzmann distribution.</param>
        public BoltzmannExploration(double temperatureParameter)
        {
            TemperatureParameter = temperatureParameter;
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
            double[] actionProbabilities = new double[actionsCount];
            double actionSum = 0, probabilitiesSum = 0;

            for (int i = 0; i < actionsCount; i++)
            {
                double actionProbability = Math.Exp(actionEstimates[i] / _temperatureParameter);

                actionProbabilities[i] = actionProbability;
                probabilitiesSum += actionProbability;
            }

            if ((double.IsInfinity(probabilitiesSum)) || (Math.Abs(probabilitiesSum) < Epsilon))
            {
                // Do a greedy selection in the case of infinity or zero.
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
                return greedyAction;
            }
            // Get a random number which determines which action to choose.
            double actionRandomNumber = StaticRandom.NextDouble();

            for (int i = 0; i < actionsCount; i++)
            {
                actionSum += actionProbabilities[i] / probabilitiesSum;
                if (actionRandomNumber <= actionSum)
                    return i;
            }

            return actionsCount - 1;
        }
    }
}
