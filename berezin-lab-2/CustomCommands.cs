using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace berezin_lab_2
{
    class CustomCommands
    {
        static CustomCommands()
        {
            // Инициализация команды
            //InputGestureCollection inputs = new InputGestureCollection();
            //inputs.Add(new KeyGesture(Key.R, ModifierKeys.Control, "Ctrl + R"));
            //requery = new RoutedUICommand("Requery", "Requery", typeof(CustomCommands), inputs);
            DetectFaces = new RoutedUICommand("DetectFaces", "DetectFaces", typeof(CustomCommands));
            ClearDataBase = new RoutedUICommand("ClearDataBase", "ClearDataBase", typeof(CustomCommands)); ;
            Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(CustomCommands));
        }

        public static RoutedUICommand DetectFaces { get; private set; }
        public static RoutedUICommand Cancel { get; private set; }
        public static RoutedUICommand DetectNewOrError { get; private set; }
        public static RoutedUICommand ClearDataBase { get; private set; }
    }
}
