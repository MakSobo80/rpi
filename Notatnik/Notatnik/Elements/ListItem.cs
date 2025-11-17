using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik.Elements
{
    public class ListItem(List<Element> contentInListItem) : Element
    {
        public List<Element> content = contentInListItem;

        public override void Display()
        {
            throw new NotImplementedException();
        }
        public override string ParseToString()
        {
            string text = "";
            for (int i = 0; i < content.Count; i++)
            {
                if (content[i] is not TextBlock)
                    text += "\n";
                text += content[i].ParseToString();
            };
            return $"{text}";
        }
    }
}
