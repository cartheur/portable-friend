using System;

namespace Cartheur.Animals.CF.Learning.ExplorationPolicy
{
    /// <summary>
    /// Tabu search exploration policy.
    /// </summary>
    /// <remarks>The class implements simple tabu search exploration policy, allowing to set certain actions as tabu for a specified amount of iterations. The actual exploration and choosing from non-tabu actions is done by <see cref="BasePolicy">base exploration policy</see>.</remarks>
    public class TabuSearchExploration : IExplorationPolicy
    {
        private readonly int _actionsCount;
        private readonly int[] _tabuActions;
        private IExplorationPolicy _baseExplorationPolicy;
        /// <summary>
        /// Base exploration policy.
        /// </summary>
        /// <remarks>Base exploration policy is the policy, which is used to choose from non-tabu actions.</remarks>
        public IExplorationPolicy BasePolicy
        {
            get { return _baseExplorationPolicy; }
            set { _baseExplorationPolicy = value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TabuSearchExploration"/> class.
        /// </summary>
        /// <param name="actionsCount">Total actions count.</param>
        /// <param name="baseExplorationPolicy">Base exploration policy.</param>
        public TabuSearchExploration(int actionsCount, IExplorationPolicy baseExplorationPolicy)
        {
            _actionsCount = actionsCount;
            _baseExplorationPolicy = baseExplorationPolicy;
            // Create a tabu list.
            _tabuActions = new int[actionsCount];
        }
        /// <summary>
        /// Choose an action.
        /// </summary>
        /// <param name="actionEstimates">Action estimates.</param>
        /// <returns>Returns selected action.</returns>
        /// <remarks>The method chooses an action depending on the provided estimates. The estimates can be any sort of estimate, which values usefulness of the action (expected summary reward, discounted reward, etc). The action is choosed from non-tabu actions only.</remarks>
        public int ChooseAction(double[] actionEstimates)
        {
            // Get the amount of non-tabu actions.
            int nonTabuActions = _actionsCount;
            for (int i = 0; i < _actionsCount; i++)
            {
                if (_tabuActions[i] != 0)
                {
                    nonTabuActions--;
                }
            }
            // Estimate the allowedableactions.
            double[] allowableActionEstimates = new double[nonTabuActions];
            int[] allowableActionMap = new int[nonTabuActions];

            for (int i = 0, j = 0; i < _actionsCount; i++)
            {
                if (_tabuActions[i] == 0)
                {
                    // Allowable actions.
                    allowableActionEstimates[j] = actionEstimates[i];
                    allowableActionMap[j] = i;
                    j++;
                }
                else
                {
                    // Decrease tabu time of tabu action.
                    _tabuActions[i]--;
                }
            }

            return allowableActionMap[_baseExplorationPolicy.ChooseAction(allowableActionEstimates)];
        }
        /// <summary>
        /// Reset tabu list.
        /// </summary>
        /// <remarks>Clears tabu list making all actions allowed.</remarks>
        public void ResetTabuList()
        {
            Array.Clear(_tabuActions, 0, _actionsCount);
        }
        /// <summary>
        /// Set tabu action.
        /// </summary>
        /// <param name="action">Action to set tabu for.</param>
        /// <param name="tabuTime">Tabu time in iterations.</param>
        public void SetTabuAction(int action, int tabuTime)
        {
            _tabuActions[action] = tabuTime;
        }
    }
}
