using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokeePdfLib
{
    internal interface IPdfDestination
    {
        /// <summary>
        /// Method that returns the PDF codes to write the destination
        /// </summary>
        /// <returns>String that contains PDF codes</returns>
        string getDestinationValue();
    }
}
