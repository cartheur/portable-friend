using System.Collections.Generic;
using Cartheur.Animals.CF.Core;

namespace Cartheur.Animals.CF.Personality
{
    /// <summary>
    /// Used to determine the gender of things.
    /// </summary>
    public enum AeonGender
    {
        Unknown = -1,
        Female = 0,
        Male = 1
    }

    public class MeaningFive
    {
        // NOTE: Any learning should go through the <learn> tag. Have removed AeonLearning.cs but kept the constructor logic here.
        public List<string> Thats { get; set; } 

        public MeaningFive(Result theMood)
        {
            // What is below is very interesting to create higher areas from the <learn> tag processing including file-writing and the inclusion of Boagaphish, in terms of the numeric pattern of mood represented in the weights embedded in the eight emotional "centers".
            Thats = new List<string>();
            // Create a position index of all that has been said in the session. Can we hook the mood alterations here too?
            for (int n = 0; n <= theMood.ThisUser.AeonReplies.Count; n++)
            {
                // What's next?
                Thats.Add(theMood.ThisUser.GetThat(n));
            }
            // Send to Boagaphish for processing. NO! It is not numeric!
            // X-|
            //
        }
    }
}
