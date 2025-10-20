using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Notatnik.Elements
{
    internal class Rule : Element
    {
        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string ParseToString()
        {
            return $"\n***\n";
        }
    }
}
