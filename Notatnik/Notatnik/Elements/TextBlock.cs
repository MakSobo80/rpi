using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik.Elements
{
    public class TextBlock(string text) : Element
    {
        public string text = text;

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string ParseToString()
        {
            return text+"\n";
        }
    }
}
