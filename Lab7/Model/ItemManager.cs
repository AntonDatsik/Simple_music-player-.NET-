using Microsoft.Win32;
using System;

namespace Lab7.Model
{
    internal class ItemManager
    {
        public static Item AddItem()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                String fileName = openFileDialog.FileName;
                return new Item(fileName);
            }
            else return null;
        }
    }
}
