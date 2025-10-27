using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik.Elements
{
    public class List : Element
    {
        public List<ListItem> content = [];

        public void AddListElement(List<Element> elementsToAdd)
        {
            content.Add(new ListItem(elementsToAdd));
        }

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string ParseToString()
        {
            string text = "";
            for (int i = 0; i < content.Count; i++)
            {
                text += $"- {content[i].ParseToString()}\n";
            }
            return $"{text}";
        }
    }
}
