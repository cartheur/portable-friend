using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Learning.ExplorationPolicy;
using Cartheur.Animals.CF.Learning.Maps;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Learning
{
    /// <summary>
    /// The learning type used.
    /// </summary>
    public enum LearningTypes
    {
        /// <summary>An off-policy, model-free reinforcement learning technique.</summary>
        QLearning,
        /// <summary>An on-policy, Markov decision process for reinforcement learning.</summary>
        SarsaAgent,
        /// <summary>Show the solution found by the learning types.</summary>
        SolutionPath
    }

    public class LearningThreads
    {
        #region Local variables
        // A cellular world map.
        private readonly CellularWorld _cellularWorld;
        private readonly LearningTypes _learningType;
        // A map and its dimensions.
        private int[,] _map;
        private int[,] _mapToDisplay;
        private int _mapWidth;
        private int _mapHeight;
        // Agent's start and stop positions.
        private int _agentStartX;
        private int _agentStartY;
        private int _agentStopX;
        private int _agentStopY;
        // Flag to stop the background job.
        private volatile bool _needToStop;
        // A worker thread.
        public static Thread LearningWorkerThread;
        // Learning settings.
        private const int LearningIterations = 100;
        private const double ExplorationRate = 0.5;
        private const double LearningRate = 0.5;
        private double _moveReward;
        private double _wallReward = -1;
        private double _goalReward = 1;
        private QLearning _qLearning;
        private SarsaAgent _sarsaAgent;
        private readonly Aeon _learningAeon;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningThreads" /> class.
        /// </summary>
        /// <param name="learningAeon">The learning aeon.</param>
        /// <param name="types">The types.</param>
        public LearningThreads(Aeon learningAeon, LearningTypes types)
        {
            _learningAeon = learningAeon;
            _learningType = types;
            _cellularWorld = new CellularWorld {Coloring = new[] {Color.White, Color.Green, Color.Black, Color.Red}};
            switch (types)
            {
                case LearningTypes.QLearning:
                    StartLearning();
                    break;
                case LearningTypes.SarsaAgent:
                    _agentStartX = 2;
                    _agentStartY = 3;
                    StartLearning();
                    break;
                case LearningTypes.SolutionPath:
                    StartLearning();
                    break;
            }
        }
        /// <summary>
        /// Starts the learning algorithm.
        /// </summary>
        /// <param name="types">The types.</param>
        public void StartLearning()
        {
            // Reset the algorithms.
            _qLearning = null;
            _sarsaAgent = null;

            // Initialize the map
            _map = new int[10, 10];

            if (_learningType == LearningTypes.QLearning)
            {
                // Create new QLearning algorithm instance.
                _qLearning = new QLearning(256, 4, new TabuSearchExploration(4, new EpsilonGreedyExploration(ExplorationRate)));
                LearningWorkerThread = new Thread(QLearningThread);
            }
            if (_learningType == LearningTypes.SarsaAgent)
            {
                // Create new SarsaAgent algorithm instance.
                _sarsaAgent = new SarsaAgent(256, 4, new TabuSearchExploration(4, new EpsilonGreedyExploration(ExplorationRate)));
                LearningWorkerThread = new Thread(SarsaAgentThread);
            }
            if (_learningType == LearningTypes.SolutionPath)
            {
                LearningWorkerThread = new Thread(ShowSolution);
            }
            // Run worker thread.
            _needToStop = false;
            LearningWorkerThread.Start();
        }
        /// <summary>
        /// Runs learning on the q-learning object using its agent thread. Off-policy.
        /// </summary>
        /// <returns>Learned q-learning object.</returns>
        public void QLearningThread()
        {
            try
            {
                int iteration = 0;
                TabuSearchExploration tabuPolicy = (TabuSearchExploration)_qLearning.ExplorationPolicy;
                EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;

                while ((!_needToStop) && (iteration < LearningIterations))
                {
                    // Set the exploration rate for this iteration.
                    explorationPolicy.ExplorationRate = ExplorationRate -
                                                        ((double)iteration / LearningIterations) * ExplorationRate;
                    // Set the learning rate for this iteration.
                    _qLearning.LearningRate = LearningRate - ((double)iteration / LearningIterations) * LearningRate;
                    // Clear tabu list.
                    tabuPolicy.ResetTabuList();
                    // Reset agent's coordinates to the starting position.
                    int agentCurrentX = _agentStartX;
                    int agentCurrentY = _agentStartY;
                    // Steps performed by agent to get to the goal.
                    int steps = 0;

                    while ((!_needToStop) && ((agentCurrentX != _agentStopX) || (agentCurrentY != _agentStopY)))
                    {
                        steps++;
                        // Get agent's current state.
                        int currentState = GetStateNumber(agentCurrentX, agentCurrentY);
                        // Get the action for this state.
                        int action = _qLearning.GetAction(currentState);
                        // Update the agent's current position and deliver its reward.
                        double reward = UpdateAgentPosition(ref agentCurrentX, ref agentCurrentY, action);
                        // Get the agent's next state.
                        int nextState = GetStateNumber(agentCurrentX, agentCurrentY);
                        // Do learning of the agent - update its Q-function.
                        _qLearning.UpdateState(currentState, action, reward, nextState);
                        // Set tabu action.
                        tabuPolicy.SetTabuAction((action + 2) % 4, 1);
                    }
                    iteration++;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.LearningThread);
            }
        }
        /// <summary>
        /// Runs learning on the sarsa-agent object using its agent thread. On policy.
        /// </summary>
        /// <returns>Learned sarsa agent object.</returns>
        public void SarsaAgentThread()
        {
            int iteration = 0;
            TabuSearchExploration tabuPolicy = (TabuSearchExploration)_sarsaAgent.ExplorationPolicy;
            EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;

            while ((!_needToStop) && (iteration < LearningIterations))
            {
                // Set the exploration rate for this iteration.
                explorationPolicy.ExplorationRate = ExplorationRate - ((double)iteration / LearningIterations) * ExplorationRate;
                // Set the learning rate for this iteration.
                _sarsaAgent.LearningRate = LearningRate - ((double)iteration / LearningIterations) * LearningRate;
                // Clear tabu list.
                tabuPolicy.ResetTabuList();
                // Reset the agent's coordinates to the starting position.
                int agentCurrentX = _agentStartX;
                int agentCurrentY = _agentStartY;
                // Steps performed by agent to get to the goal.
                int steps = 1;
                // The previous state and action.
                int previousState = GetStateNumber(agentCurrentX, agentCurrentY);
                int previousAction = _sarsaAgent.GetAction(previousState);
                // Update the agent's current position and de;over its reward.
                double reward = UpdateAgentPosition(ref agentCurrentX, ref agentCurrentY, previousAction);

                while ((!_needToStop) && ((agentCurrentX != _agentStopX) || (agentCurrentY != _agentStopY)))
                {
                    steps++;
                    // Set tabu action.
                    tabuPolicy.SetTabuAction((previousAction + 2) % 4, 1);
                    // Get the agent's next state.
                    int nextState = GetStateNumber(agentCurrentX, agentCurrentY);
                    // Get the agent's next action.
                    int nextAction = _sarsaAgent.GetAction(nextState);
                    // Do learning of the agent - update its Q-function.
                    _sarsaAgent.UpdateState(previousState, previousAction, reward, nextState, nextAction);
                    // Update the agent's new position and get his reward.
                    reward = UpdateAgentPosition(ref agentCurrentX, ref agentCurrentY, nextAction);

                    previousState = nextState;
                    previousAction = nextAction;
                }

                if (!_needToStop)
                {
                    // Update the Q-function if terminal state was reached.
                    _sarsaAgent.UpdateState(previousState, previousAction, reward);
                }
                iteration++; ;
            }
        }
        /// <summary>
        /// Shows the solution thread.
        /// </summary>
        public void ShowSolution()
        {
            // Set exploration rate to 0, so agent uses only what he learnt.
            TabuSearchExploration tabuPolicy;

            if (_qLearning != null)
                tabuPolicy = (TabuSearchExploration)_qLearning.ExplorationPolicy;
            else
                tabuPolicy = (TabuSearchExploration)_sarsaAgent.ExplorationPolicy;

            EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;
            explorationPolicy.ExplorationRate = 0;
            tabuPolicy.ResetTabuList();
            // The curent coordinates of the agent.
            int agentCurrentX = _agentStartX, agentCurrentY = _agentStartY;
            // Prepare the map to display.
            Array.Copy(_map, _mapToDisplay, _mapWidth * _mapHeight);
            _mapToDisplay[_agentStartY, _agentStartX] = 2;
            _mapToDisplay[_agentStopY, _agentStopX] = 3;

            while (!_needToStop)
            {
                // Dispay the map.
                _cellularWorld.Map = _mapToDisplay;
                // Sleep for a while.
                Thread.Sleep(200);
                // Check if we have reached the end point.
                if ((agentCurrentX == _agentStopX) && (agentCurrentY == _agentStopY))
                {
                    // Restore the map.
                    _mapToDisplay[_agentStartY, _agentStartX] = 2;
                    _mapToDisplay[_agentStopY, _agentStopX] = 3;
                    agentCurrentX = _agentStartX;
                    agentCurrentY = _agentStartY;
                    _cellularWorld.Map = _mapToDisplay;
                    Thread.Sleep(200);
                }
                // Remove agent from current position.
                _mapToDisplay[agentCurrentY, agentCurrentX] = 0;
                // Get the agent's current state.
                int currentState = GetStateNumber(agentCurrentX, agentCurrentY);
                // Get the action for this state.
                int action = (_qLearning != null) ? _qLearning.GetAction(currentState) : _sarsaAgent.GetAction(currentState);
                // Update the agent's current position and get its reward.
                double reward = UpdateAgentPosition(ref agentCurrentX, ref agentCurrentY, action);
                // Put agent to the new position.
                _mapToDisplay[agentCurrentY, agentCurrentX] = 2;
            }
        }
        /// <summary>
        /// Update agent position and return reward for the move.
        /// </summary>
        /// <param name="currentX">The current x.</param>
        /// <param name="currentY">The current y.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        protected double UpdateAgentPosition(ref int currentX, ref int currentY, int action)
        {
            // default reward is equal to moving reward
            double reward = _moveReward;
            // moving direction
            int dx = 0, dy = 0;

            switch (action)
            {
                case 0:         // go to north (up)
                    dy = -1;
                    break;
                case 1:         // go to east (right)
                    dx = 1;
                    break;
                case 2:         // go to south (down)
                    dy = 1;
                    break;
                case 3:         // go to west (left)
                    dx = -1;
                    break;
            }

            int newX = currentX + dx;
            int newY = currentY + dy;

            // check new agent's coordinates
            if (
                (_map[newY, newX] != 0) ||
                (newX < 0) || (newX >= _mapWidth) ||
                (newY < 0) || (newY >= _mapHeight)
                )
            {
                // we found a wall or got outside of the world
                reward = _wallReward;
            }
            else
            {
                currentX = newX;
                currentY = newY;

                // check if we found the goal
                if ((currentX == _agentStopX) && (currentY == _agentStopY))
                    reward = _goalReward;
            }

            return reward;
        }
        /// <summary>
        /// Get state number from agent's receptors in the specified position.
        /// </summary>
        /// <param name="x">The x-position.</param>
        /// <param name="y">The y-position.</param>
        /// <returns></returns>
        protected int GetStateNumber(int x, int y)
        {
            int c1 = (_map[y - 1, x - 1] != 0) ? 1 : 0;
            int c2 = (_map[y - 1, x] != 0) ? 1 : 0;
            int c3 = (_map[y - 1, x + 1] != 0) ? 1 : 0;
            int c4 = (_map[y, x + 1] != 0) ? 1 : 0;
            int c5 = (_map[y + 1, x + 1] != 0) ? 1 : 0;
            int c6 = (_map[y + 1, x] != 0) ? 1 : 0;
            int c7 = (_map[y + 1, x - 1] != 0) ? 1 : 0;
            int c8 = (_map[y, x - 1] != 0) ? 1 : 0;

            return c1 |
                (c2 << 1) |
                (c3 << 2) |
                (c4 << 3) |
                (c5 << 4) |
                (c6 << 5) |
                (c7 << 6) |
                (c8 << 7);
        }
        /// <summary>
        /// Imports the learning map into GPU memory.
        /// </summary>
        /// <param name="pathToMapFiles">The path to map files.</param>
        /// <param name="mapName">Name of the map.</param>
        public void ImportMap(string pathToMapFiles, string mapName)
        {
            StreamReader reader = null;

            try
            {
                // Open the file.
                reader = File.OpenText(pathToMapFiles + "/" + mapName);
                string str;
                // Line counter.
                int lines = 0;
                int j = 0;
                // Read the file.
                while ((str = reader.ReadLine()) != null)
                {
                    str = str.Trim();

                    // Skip comments and empty lines.
                    if ((str == string.Empty) || (str[0] == ';') || (str[0] == '\0'))
                        continue;

                    // Split the string.
                    string[] strs = str.Split(' ');
                    // Check the line.
                    if (lines == 0)
                    {
                        // Get the world size.
                        _mapWidth = int.Parse(strs[0]);
                        _mapHeight = int.Parse(strs[1]);
                        _map = new int[_mapHeight, _mapWidth];
                    }
                    else if (lines == 1)
                    {
                        // Get agents count.
                        if (int.Parse(strs[0]) != 1)
                        {
                            //log it
                            break;
                        }
                    }
                    else if (lines == 2)
                    {
                        // Agent position.
                        _agentStartX = int.Parse(strs[0]);
                        _agentStartY = int.Parse(strs[1]);
                        _agentStopX = int.Parse(strs[2]);
                        _agentStopY = int.Parse(strs[3]);
                        // Check position.
                        if (
                            (_agentStartX < 0) || (_agentStartX >= _mapWidth) ||
                            (_agentStartY < 0) || (_agentStartY >= _mapHeight) ||
                            (_agentStopX < 0) || (_agentStopX >= _mapWidth) ||
                            (_agentStopY < 0) || (_agentStopY >= _mapHeight)
                            )
                        {
                            //log it
                            break;
                        }
                    }
                    else if (lines > 2)
                    {
                        // Map lines.
                        if (j < _mapHeight)
                        {
                            for (int i = 0; i < _mapWidth; i++)
                            {
                                _map[j, i] = int.Parse(strs[i]);
                                if (_map[j, i] > 1)
                                    _map[j, i] = 1;
                            }
                            j++;
                        }
                    }
                    lines++;
                }

                // Set the worlds map.
                _mapToDisplay = (int[,])_map.Clone();
                _mapToDisplay[_agentStartY, _agentStartX] = 2;
                _mapToDisplay[_agentStopY, _agentStopX] = 3;
                _cellularWorld.Map = _mapToDisplay;

                // Show world size.
                string worldSize = string.Format("{0} x {1}", _mapWidth, _mapHeight);

            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.LearningThread);
            }
            finally
            {
                // Clean-up resources.
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }    
            }
        }
    }
}
