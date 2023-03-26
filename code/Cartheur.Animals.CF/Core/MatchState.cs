namespace Cartheur.Animals.CF.Core
{
    /// <summary>
    /// Denotes what part of the input path a node represents. 
    /// </summary>
    /// <remarks>
    /// Used when pushing values represented by wildcards onto collections for the star, thatstar, and topicstar values.
    /// </remarks>
    public enum MatchState
    {
        UserInput,
        That,
        Topic,
        Emotion
    }
}
