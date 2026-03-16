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


        List<string> SplitByWidthFast(string text, TextBlock textBlock, double firstWidth, double maxWidth)
        {
            var result = new List<string>();
            int start = 0;
            bool first = true;

            while (start < text.Length)
            {
                double limit = first ? firstWidth : maxWidth;

                int low = 1;
                int high = text.Length - start;
                int best = 1;

                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    string part = text.Substring(start, mid);

                    var ft = new FormattedText(
                        part,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                        textBlock.FontSize,
                        Brushes.Black,
                        VisualTreeHelper.GetDpi(textBlock).PixelsPerDip
                    );

                    if (ft.Width <= limit)
                    {
                        best = mid;
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }

                result.Add(text.Substring(start, best));
                start += best;
                first = false;
            }

            return result;
        }

        public override void Display(FrameworkElement pointer)
        {
            TextBlock textBlock = new()
            {
                FontSize = 20
            };
            Canvas parentCanvas = (Canvas)pointer.Parent;
            parentCanvas.Children.Add(textBlock);

            int wrappingLimit = 500;
            List<string> splitString = SplitByWidthFast(text, textBlock, wrappingLimit - Canvas.GetLeft(pointer), wrappingLimit);

            int currentLine = 0;
            foreach (string text in splitString)
            {

                currentLine += 1;
                textBlock = new()
                {
                    Text = text,
                    FontSize = 20
                };
                parentCanvas = (Canvas)pointer.Parent;
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
                double endY = textBlock.ActualHeight + Canvas.GetTop(pointer); // opcjonalnie dół TextBlocka

                if (endX > 0.95 * wrappingLimit || (text == splitString[0] && endX > 0.95 * (wrappingLimit - Canvas.GetLeft(pointer))))
                    Canvas.SetTop(pointer, endY + 20);

                if (text == splitString[splitString.Count - 1])
                    Canvas.SetLeft(pointer, Canvas.GetLeft(textBlock) + endX);
                else
                    Canvas.SetLeft(pointer, 0);

                Canvas.SetTop(textBlock, endY);
            }
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
