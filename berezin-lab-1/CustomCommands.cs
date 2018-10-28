using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace berezin_lab_1
{
    class CustomCommands
    {
        // Создание команды AddCustomProgrammer
        private static RoutedUICommand _DetectFaces;

        static CustomCommands()
        {
            // Инициализация команды
            //InputGestureCollection inputs = new InputGestureCollection();
            //inputs.Add(new KeyGesture(Key.R, ModifierKeys.Control, "Ctrl + R"));
            //requery = new RoutedUICommand("Requery", "Requery", typeof(CustomCommands), inputs);
            _DetectFaces = new RoutedUICommand("DetectFaces", "DetectFaces", typeof(CustomCommands));
        }

        public static RoutedUICommand DetectFaces
        {
            get { return _DetectFaces; }
        }
    }
}
