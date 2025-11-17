using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notatnik.Elements
{
    public class MarkdownList(bool isOrdered, int nestingDepth = 0) : Element
    {
        public List<ListItem> content = [];
        public bool isOrdered = isOrdered;
        public int nestingDepth = nestingDepth;

        public void AddListElement(List<Element> elementsToAdd)
        {
            content.Add(new ListItem(elementsToAdd));
            foreach(Element element in elementsToAdd)
            {
                if (element is MarkdownList markdownList)
                    markdownList.IncreaseNestingDepthOfElements();
            }

        }

        public void IncreaseNestingDepthOfElements()
        {
            nestingDepth++;
            foreach (var item in content)
            {
                foreach (var element in item.content)
                {
                    if (element is MarkdownList mdList)
                    {
                        mdList.nestingDepth = nestingDepth;
                        mdList.IncreaseNestingDepthOfElements();
                    }
                }
            }
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
                    text += $"{string.Join("", Enumerable.Repeat("    ", nestingDepth))}- {content[i].ParseToString()}";
                    if (i != content.Count - 1)
                        text += "\n";
                }
            }
            else
            {
                for (int i = 0; i < content.Count; i++)
                {
                    text += $"{string.Join("", Enumerable.Repeat("    ",nestingDepth))}{i+1}. {content[i].ParseToString()}";
                    if (i != content.Count - 1)
                    {
                        text += "\n";
                    }
                }
            }
            return $"{text}";
        }
    }
}
