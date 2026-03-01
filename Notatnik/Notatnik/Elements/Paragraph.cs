using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Notatnik.Elements
{
    internal class Paragraph(List<Element> contentInParagraph) : Element
    {
        public List<Element> content = contentInParagraph;

        public override void Display(FrameworkElement pointer)
        {
            foreach (Element element in content)
            {
                element.Display(pointer);
            }

            Canvas.SetTop(pointer, Canvas.GetTop(pointer) + 32);
            Canvas.SetLeft(pointer, 0);
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
