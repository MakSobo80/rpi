using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik
{
    abstract public class Element
    {
        public abstract void Display();
        public abstract string ParseToString();
    }
}
