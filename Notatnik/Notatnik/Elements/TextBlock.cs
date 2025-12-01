using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notatnik.Elements
{
    public class TextBlock(string text) : Element
    {
        public enum TextStyle
        {
            None,
            Bold,
            Italic,
            ItalicBold
        }

        //Text block can contain only text
        public string text = text;
        public TextStyle style = TextStyle.None;

        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override string ParseToString()
        {
            return style switch
            {
                TextStyle.None => text,
                TextStyle.Bold => $"**{text}**",
                TextStyle.Italic => $"*{text}*",
                TextStyle.ItalicBold => $"***{text}***",
                _ => text
            };
        }
    }
}
