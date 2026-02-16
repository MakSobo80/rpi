using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik.Elements
{
    public class Header(string textInHeader, int headerType) : Element
    {
        public string text = textInHeader;
        public int type = headerType;

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string ParseToString()
        {
            return $"\n{new string('#', type)} {text}\n";
        }
    }
}
