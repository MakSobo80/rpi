using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik.Elements
{
    internal class Paragraph(List<Element> contentInParagraph) : Element
    {
        public List<Element> content = contentInParagraph;

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string ParseToString()
        {
            string text = "";
            for (int i = 0; i < content.Count; i++)
            {
                text += content[i].ParseToString();
            };
            return $"\n{text}\n";
        }
    }
}
