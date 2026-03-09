using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Notatnik.Elements.Text;

namespace Notatnik.Elements
{
    public class Header(string textInHeader, int headerType) : Element
    {
        public string text = textInHeader;
        public int type = headerType;

        public override void Display(FrameworkElement pointer)
        {
            TextBlock textBlock = new()
            {
                Text = text,
                FontWeight = FontWeights.Bold
            };
            Canvas parentCanvas = (Canvas)pointer.Parent;
            parentCanvas.Children.Add(textBlock);
            Canvas.SetLeft(textBlock, Canvas.GetLeft(pointer));
            Canvas.SetTop(textBlock, Canvas.GetTop(pointer));

            switch (type)
            {
                case 1:
                    textBlock.FontSize = 40;
                    break;
                case 2:
                    textBlock.FontSize = 36;
                    break;
                case 3:
                    textBlock.FontSize = 32;
                    break;
                case 4:
                    textBlock.FontSize = 28;
                    break;
                case 5:
                    textBlock.FontSize = 24;
                    break;
                case 6:
                    textBlock.FontSize = 20;
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
            double endY = ft.Height; // opcjonalnie dół TextBlocka

            Canvas.SetLeft(pointer, 0);
            Canvas.SetTop(pointer, Canvas.GetTop(pointer) + endY);
        }

        public override string ParseToString()
        {
            return $"\n{new string('#', type)} {text}\n";
        }
    }
}
