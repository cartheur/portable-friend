using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Normalize
{
    /// <summary>
    /// Normalizes the input text into upper case.
    /// </summary>
    public class MakeCaseInsensitive : TextTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MakeCaseInsensitive"/> class.
        /// </summary>
        /// <param name="aeon">The aeon is this transformer a part of</param>
        /// <param name="inputString">The input string to be transformed</param>
        public MakeCaseInsensitive(Aeon aeon, string inputString) : base(aeon, inputString)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MakeCaseInsensitive"/> class.
        /// </summary>
        /// <param name="aeon">The aeon this transformer is a part of</param>
        public MakeCaseInsensitive(Aeon aeon) : base(aeon) 
        { }
        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        protected override string ProcessChange()
        {
            return InputString.ToUpper();
        }
        /// <summary>
        /// An ease-of-use static method that re-produces the instance transformation methods.
        /// </summary>
        /// <param name="input">The string to transform</param>
        /// <returns>The resulting string</returns>
        public static string TransformInput(string input)
        {
            return input.ToUpper();
        }
    }
}
