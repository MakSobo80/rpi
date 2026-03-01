using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notatnik
{
    abstract public class Element
    {
        public abstract void Display(FrameworkElement pointer);
        public abstract string ParseToString();
    }
}
