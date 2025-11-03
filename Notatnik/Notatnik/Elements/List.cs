using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik.Elements
{
    public class List(bool isOrdered) : Element
    {
        public List<ListItem> content = [];
        public bool isOrdered = isOrdered;

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
            if (!isOrdered)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    text += $"- {content[i].ParseToString()}\n";
                }
            }
            else
            {
                for (int i = 0; i < content.Count; i++)
                {
                    text += $"{i+1}. {content[i].ParseToString()}\n";
                }
            }
            return $"{text}";
        }
    }
}
