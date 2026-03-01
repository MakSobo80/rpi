using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Notatnik.Elements
{
    public class Text(string text) : Element
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

        public override void Display(FrameworkElement pointer)
        {
            TextBlock textBlock = new()
            {
                Text = text,
                FontSize = 20
            };
            Canvas parentCanvas = (Canvas)pointer.Parent;
            parentCanvas.Children.Add(textBlock);
            Canvas.SetLeft(textBlock, Canvas.GetLeft(pointer));
            Canvas.SetTop(textBlock, Canvas.GetTop(pointer));

            switch (style)
            {
                case TextStyle.Bold:
                    textBlock.FontWeight = FontWeights.Bold;
                    break;
                case TextStyle.Italic:
                    textBlock.FontStyle = FontStyles.Italic;
                    break;
                case TextStyle.ItalicBold:
                    textBlock.FontStyle = FontStyles.Italic;
                    textBlock.FontWeight = FontWeights.Bold;
                    break;
            }

            // mierzymy szerokość ostatniej linii
            var ft = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(textBlock).PixelsPerDip
            );

            // pozycja końca tekstu względem TextBlock
            double endX = ft.Width;
            double endY = textBlock.ActualHeight; // opcjonalnie dół TextBlocka

            Canvas.SetLeft(pointer, Canvas.GetLeft(textBlock) + endX);
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
