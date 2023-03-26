using Cartheur.Animals.CF.Core;

namespace Cartheur.Animals.CF.Utilities
{
    /// <summary>
    /// Encapsulates all the required methods and attributes for any text transformation. An input string is provided and various methods and attributes can be used to grab a transformed string. The protected ProcessChange() method is abstract and should be overridden to contain the code for transforming the input text into the output text.
    /// </summary>
    abstract public class TextTransformer
    {
        /// <summary>
        /// Instance of the input string.
        /// </summary>
        string _inputString;
        /// <summary>
        /// The aeon that this transformation is connected with.
        /// </summary>
        public Aeon ThisAeon;
        /// <summary>
        /// The input string to be transformed.
        /// </summary>
        public string InputString
        {
            get { return _inputString; }
            set { _inputString = value; }
        }
        /// <summary>
        /// The transformed string.
        /// </summary>
        public string OutputString
        {
            get { return Transform(); }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextTransformer"/> class.
        /// </summary>
        /// <param name="aeon">The aeon is this transformer a part of.</param>
        /// <param name="inputString">The input string to be transformed.</param>
        protected TextTransformer(Aeon aeon, string inputString)
        {
            ThisAeon = aeon;
            _inputString = inputString;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextTransformer"/> class.
        /// </summary>
        /// <param name="aeon">The aeon this transformer is a part of.</param>
        protected TextTransformer(Aeon aeon)
        {
            ThisAeon = aeon;
            _inputString = string.Empty;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextTransformer"/> class. Used as part of late binding mechanism.
        /// </summary>
        protected TextTransformer()
        {
            ThisAeon = null;
            _inputString = string.Empty;
        }
        /// <summary>
        /// Do a transformation on the supplied input string.
        /// </summary>
        /// <param name="input">The string to be transformed.</param>
        /// <returns>The resulting output.</returns>
        public string Transform(string input)
        {
            _inputString = input;
            return Transform();
        }
        /// <summary>
        /// Do a transformation on the string found in the InputString attribute.
        /// </summary>
        /// <returns>The resulting transformed string.</returns>
        public string Transform()
        {
            return _inputString.Length > 0 ? ProcessChange() : string.Empty;
        }
        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        /// <returns>The resulting processed text.</returns>
        protected abstract string ProcessChange();
    }
}
