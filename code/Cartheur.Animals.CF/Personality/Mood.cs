using System;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Personality
{
    // Emotions which can be expressed by the program.
    public enum Emotions { Happy, Confident, Energized, Helped, Insecure, Sad, Hurt, Tired }
    // Subsets of these emotions.
    public enum HappyFeelings { Hopeful, Supported, Charmed, Grateful, Optimistic, Content, Loving }
    public enum ConfidentFeelings { Strong, Certain, Assured, Successful, Valuable, Beautiful, Relaxed }
    public enum EnergizedFeelings { Determined, Inspired, Creative, Healthy, Vibrant, Alert, Motivated }
    public enum HelpedFeelings { Cherished, Befriended, Appreciated, Understood, Empowered, Accepted, Loved }
    public enum InsecureFeelings { Weak, Hopeless, Doubtful, Scared, Anxious, Stressed, Nervous }
    public enum SadFeelings { Depressed, Lonely, Angry, Frustrated, Upset, Disappointed, Hateful }
    public enum HurtFeelings { Forgotten, Ignored, Offended, Rejected, Hated, Mistreated, Injured }
    public enum TiredFeelings { Indifferent, Bored, Sick, Weary, Powerless, Listless, Drained }
    /// <summary>
    /// The class which manifests the mood of the program.
    /// </summary>
    public class Mood
    {
        // For the next iteration of this implementation.
        //protected static bool AffectionDetect { get; set; }
        // The RNG
        protected static int WheelSpun { get; set; }
        protected static int NextWheelPosition { get; set; }
        protected const int Limit = 20;
        // Emotional variety: the state and the emotional weights.
        public static bool InLove { get; set; }
        public static int Dislike { get; set; }
        public static int Insults { get; set; }
        public static int Normal { get; set; }
        public static int Love { get; set; }
        public static int Compliment { get; set; }
        /// <summary>
        /// Gets or sets the current mood.
        /// </summary>
        public static string CurrentMood { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Mood"/> class. Allows the program to express emotional variety to the user.
        /// </summary>
        /// <param name="seedValue">The seed value which controls random emotional expressions. Value should be between 0 and 20.</param>
        /// <remarks>This section of code contains what is called the "mood engine". The mood engine is designed, via the <see cref="StaticRandom"/> class, that low seed values perpetuate the happy moods, mid-range sadness (insecurity), and upper-range the more anti-social moods.</remarks>
        public Mood(int seedValue)
        {
            if (seedValue > Limit) return;
            WheelSpun = StaticRandom.Next(0, Limit);
            NextWheelPosition = new Random(seedValue).Next(Limit);
            // Emotional variety: a simple implementation.
            if (NextWheelPosition.IsBetween(0, 5) & WheelSpun.IsBetween(11, Limit))
            {
                CurrentMood = Emotions.Confident.ToString();
                Love = Love + 1;
                Normal = Normal + 1;
                Compliment = Compliment + 1;
            }
            if (NextWheelPosition.IsBetween(6, 15) & WheelSpun.IsBetween(0, 10))
            {
                CurrentMood = Emotions.Energized.ToString();
                Normal = Normal + 1;
                Love = Love + 1;
            }
            if (NextWheelPosition.IsBetween(0, 5) & WheelSpun.IsBetween(0, 10))
            {
                CurrentMood = Emotions.Happy.ToString();
                Love = Love + 2;
                Compliment = Compliment + 2;
            }
            if (NextWheelPosition.IsBetween(6, 15) & WheelSpun.IsBetween(11, Limit))
            {
                CurrentMood = Emotions.Helped.ToString();
                Normal = Normal + 1;
                Compliment = Compliment + 1;
            }
            if (NextWheelPosition.IsBetween(16, Limit) & WheelSpun.IsBetween(11, Limit))
            {
                CurrentMood = Emotions.Hurt.ToString();
                Love = Love - 1;
                Normal = Normal - 1;
                Compliment = Compliment - 1;
                Dislike = Dislike + 2;
                Insults = Insults + 2;
            }
            if (NextWheelPosition.IsBetween(16, Limit) & WheelSpun.IsBetween(0, 5))
            {
                CurrentMood = Emotions.Insecure.ToString();
                Love = Love - 1;
                Normal = Normal + 1;
                Dislike = Dislike + 1;
            }
            if (NextWheelPosition.IsBetween(16, Limit) & WheelSpun.IsBetween(0, 10))
            {
                CurrentMood = Emotions.Sad.ToString();
                Love = Love - 1;
                Normal = Normal - 1;
                Dislike = Dislike + 1;
                Insults = Insults + 1;
            }
            if (NextWheelPosition.IsBetween(0, Limit) & WheelSpun.IsBetween(16, Limit))
            {
                CurrentMood = Emotions.Tired.ToString();
                Normal = Normal - 1;
                Compliment = Compliment + 1;
            }
            CheckIfInLove();
        }
        /// <summary>
        /// Checks if aeon is in love.
        /// </summary>
        public bool CheckIfInLove()
        {
            if (Love > 10)
                return InLove = true;
            return InLove = false;
        }
        /// <summary>
        /// Gets the current mood.
        /// </summary>
        public string GetCurrentMood()
        {
            return CurrentMood;
        }
        /// <summary>
        /// Changes the mood of aeon.
        /// </summary>
        /// <param name="mood">The mood resulting from detecting emotional triggers from a that.</param>
        /// <remarks>What happens at each mood to those parts of the program when it is changed (for some reason)?</remarks>
        public void ChangeMood(Emotions mood)
        {
            switch (mood)
            {
                case Emotions.Confident:
                    CurrentMood = Emotions.Confident.ToString();
                    Love = Love + 1;
                    Normal = Normal + 1;
                    Compliment = Compliment + 1;
                    break;
                case Emotions.Energized:
                    CurrentMood = Emotions.Energized.ToString();
                    Normal = Normal + 1;
                    Love = Love + 1;
                    break;
                case Emotions.Hurt:
                    CurrentMood = Emotions.Hurt.ToString();
                    Love = Love - 1;
                    Normal = Normal - 1;
                    Compliment = Compliment - 1;
                    Dislike = Dislike + 2;
                    Insults = Insults + 2;
                    break;
                case Emotions.Happy:
                    CurrentMood = Emotions.Happy.ToString();
                    Love = Love + 2;
                    Compliment = Compliment + 2;
                    break;
                case Emotions.Helped:
                    CurrentMood = Emotions.Helped.ToString();
                    Normal = Normal + 1;
                    Compliment = Compliment + 1;
                    break;
                case Emotions.Insecure:
                    CurrentMood = Emotions.Insecure.ToString();
                    Love = Love - 1;
                    Normal = Normal + 1;
                    Dislike = Dislike + 1;
                    break;
                case Emotions.Sad:
                    CurrentMood = Emotions.Sad.ToString();
                    Love = Love - 1;
                    Normal = Normal - 1;
                    Dislike = Dislike + 1;
                    Insults = Insults + 1;
                    break;
                case Emotions.Tired:
                    CurrentMood = Emotions.Tired.ToString();
                    Normal = Normal - 1;
                    Compliment = Compliment + 1;
                    break;
            }
        }
    }
}
