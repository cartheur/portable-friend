using System;
using Cartheur.Animals.CF.Gui;

namespace Cartheur.Animals.CF.App
{
    static class Program
    {
        /// <summary>
        /// The call to the managed memory entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            StartUp.Animals();
        }
    }
}