using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notatnik.Elements
{
    public class ListItem(List<Element> contentInListItem) : Element
    {
        public List<Element> content = contentInListItem;

        public override void Display(FrameworkElement pointer)
        {
            throw new NotImplementedException();
        }
        public override string ParseToString()
        {
            string text = "";
            for (int i = 0; i < content.Count; i++)
            {
                text += content[i].ParseToString();
            }
            ;
            return $"{text}";
        }
    }
}
