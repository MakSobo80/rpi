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
            Canvas parentCanvas = (Canvas)pointer.Parent;

            int wrappingLimit = 500;
            double startX = Canvas.GetLeft(pointer);
            double startY = Canvas.GetTop(pointer);

            TextBlock measureBlock = new TextBlock
            {
                FontSize = 20
            };

            switch (style)
            {
                case TextStyle.Bold:
                    measureBlock.FontWeight = FontWeights.Bold;
                    break;
                case TextStyle.Italic:
                    measureBlock.FontStyle = FontStyles.Italic;
                    break;
                case TextStyle.ItalicBold:
                    measureBlock.FontStyle = FontStyles.Italic;
                    measureBlock.FontWeight = FontWeights.Bold;
                    break;
            }

            List<string> splitString = SplitByWidthFast(text, measureBlock, wrappingLimit - startX, wrappingLimit);

            double currentX = startX;
            double currentY = startY;

            for (int i = 0; i < splitString.Count; i++)
            {
                string lineText = splitString[i];

                TextBlock textBlock = new TextBlock
                {
                    Text = lineText.Replace(" ", "\u00A0"),
                    FontSize = 20
                };

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
               
                var ft = new FormattedText(
                    textBlock.Text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                    textBlock.FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(textBlock).PixelsPerDip
                );

                double lineWidth = ft.Width;
                double lineHeight = ft.Height;

                parentCanvas.Children.Add(textBlock);
                Canvas.SetLeft(textBlock, currentX);
                Canvas.SetTop(textBlock, currentY);

                if (i == splitString.Count - 1)
                {
                    Canvas.SetLeft(pointer, currentX + lineWidth);
                    Canvas.SetTop(pointer, currentY);
                }
                else 
                {
                    currentY += lineHeight;
                    Canvas.SetTop(pointer, currentY);
                    currentX = 0;
                    Canvas.SetLeft(pointer, currentX);
                }
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
