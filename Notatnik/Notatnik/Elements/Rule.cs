using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Notatnik.Elements
{
    internal class Rule : Element
    {
        public override void Display(FrameworkElement pointer)
        {
            Canvas.SetTop(pointer, Canvas.GetTop(pointer) + 10);
            Separator rule = new()
            {
                Height = 1,
                Width = 500,
                Background = new SolidColorBrush(Colors.Black)
            };
            Canvas parentCanvas = (Canvas)pointer.Parent;
            parentCanvas.Children.Add(rule);
            Canvas.SetLeft(rule, Canvas.GetLeft(pointer));
            Canvas.SetTop(rule, Canvas.GetTop(pointer));

            Canvas.SetTop(pointer, Canvas.GetTop(pointer) + 10);
        }

        public override string ParseToString()
        {
            return $"\n***\n";
        }
    }
}
