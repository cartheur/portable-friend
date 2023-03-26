using System;
using Cartheur.Animals.CF.Learning.ExplorationPolicy;

namespace Cartheur.Animals.CF.Learning
{
    /// <summary>
    /// An implementation of the Q-Learning algorithm, an off-policy temporal difference control.
    /// </summary>
    public class QLearning
    {
        private readonly int _possibleStates;
        private readonly int _possibleActions;
        private readonly double[][] _qvalues;
        private IExplorationPolicy _explorationPolicy;
        private double _discountFactor = 0.95;
        private double _learningRate = 0.25;
        /// <summary>
        /// Amount of possible states.
        /// </summary>
        public int StatesCount
        {
            get { return _possibleStates; }
        }
        /// <summary>
        /// Amount of possible actions.
        /// </summary>
        public int ActionsCount
        {
            get { return _possibleActions; }
        }
        /// <summary>
        /// Exploration policy used to select actions.
        /// </summary>
        public IExplorationPolicy ExplorationPolicy
        {
            get { return _explorationPolicy; }
            set { _explorationPolicy = value; }
        }
        /// <summary>
        /// The learning rate, whose value lies between [0, 1].
        /// </summary>
        /// <remarks>The value determines the amount of updates Q-function receives during learning. The greater the value, the more updates the function receives. The lower the value, the fewer updates it receives.</remarks>
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = Math.Max(0.0, Math.Min(1.0, value)); }
        }
        /// <summary>
        /// The discount factor, whose value lies between [0, 1].
        /// </summary>
        /// <remarks>Discount factor for the expected summary reward. The value serves as multiplier for the expected reward. So if the value is set to 1, then the expected summary reward is not discounted. If the value is getting smaller, then smaller amount of the expected reward is used for actions' estimates update.</remarks>
        public double DiscountFactor
        {
            get { return _discountFactor; }
            set { _discountFactor = Math.Max(0.0, Math.Min(1.0, value)); }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="QLearning"/> class. Action estimates are randomized in the case of this constructor is used.
        /// </summary>
        /// <param name="possibleStates">Amount of possible states.</param>
        /// <param name="possibleActions">Amount of possible actions.</param>
        /// <param name="explorationPolicy">Exploration policy.</param>
        public QLearning(int possibleStates, int possibleActions, IExplorationPolicy explorationPolicy) :
            this(possibleStates, possibleActions, explorationPolicy, true)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="QLearning"/> class.
        /// </summary>
        /// <param name="possibleStates">Amount of possible states.</param>
        /// <param name="possibleActions">Amount of possible actions.</param>
        /// <param name="explorationPolicy">Exploration policy.</param>
        /// <param name="randomize">Randomize action estimates or not.</param>
        /// <remarks>The <b>randomize</b> parameter specifies if initial action estimates should be randomized with small values or not. Randomization of action values may be useful, when greedy exploration policies are used. In this case randomization ensures that actions of the same type are not chosen always.</remarks>
        public QLearning(int possibleStates, int possibleActions, IExplorationPolicy explorationPolicy, bool randomize)
        {
            _possibleStates = possibleStates;
            _possibleActions = possibleActions;
            _explorationPolicy = explorationPolicy;
            // Create a Q-array.
            _qvalues = new double[possibleStates][];
            for (int i = 0; i < possibleStates; i++)
            {
                _qvalues[i] = new double[possibleActions];
            }
            // Do randomization.
            if (randomize)
            {
                Random rand = new Random();

                for (int i = 0; i < possibleStates; i++)
                {
                    for (int j = 0; j < possibleActions; j++)
                    {
                        _qvalues[i][j] = rand.NextDouble() / 10;
                    }
                }
            }
        }
        /// <summary>
        /// Get next action from the specified state.
        /// </summary>
        /// <param name="state">Current state to get an action for.</param>
        /// <returns>Returns the action for the state.</returns>
        /// <remarks>The method returns an action according to current <see cref="ExplorationPolicy">exploration policy</see>.</remarks>
        public int GetAction(int state)
        {
            return _explorationPolicy.ChooseAction(_qvalues[state]);
        }
        /// <summary>
        /// Update Q-function's value for the previous state-action pair.
        /// </summary>
        /// <param name="previousState">Previous state.</param>
        /// <param name="action">Action, which leads from previous to the next state.</param>
        /// <param name="reward">Reward value, received by taking specified action from previous state.</param>
        /// <param name="nextState">Next state.</param>
        public void UpdateState(int previousState, int action, double reward, int nextState)
        {
            // The next state's action estimations.
            double[] nextActionEstimations = _qvalues[nextState];
            // Find the maximum expected summary reward from the next state.
            double maxNextExpectedReward = nextActionEstimations[0];

            for (int i = 1; i < _possibleActions; i++)
            {
                if (nextActionEstimations[i] > maxNextExpectedReward)
                    maxNextExpectedReward = nextActionEstimations[i];
            }
            // The previous state's action estimations.
            double[] previousActionEstimations = _qvalues[previousState];
            // Update the expected summary reward of the previous state.
            previousActionEstimations[action] *= (1.0 - _learningRate);
            previousActionEstimations[action] += (_learningRate * (reward + _discountFactor * maxNextExpectedReward));
        }
    }
}
