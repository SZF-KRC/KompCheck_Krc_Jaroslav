using KompCheck_Krc_Jaroslav.ToDo;
using System;
using System.Threading.Tasks;

namespace KompCheck_Krc_Jaroslav.Menu
{
    public static class MainMenu
    {
        /// <summary>
        /// Zeigt das Hauptmenü an
        /// </summary>
        /// <returns>Wir geben den Faden lose zurück</returns>
        public static async Task OpenMenu()
        {
            while (true)
            {
                Console.Write("*** Welcome in Reader ***\n" +
                    "[1] Add Books\n" +
                    "[2] Print result\n" +
                    "[3] Save result\n" +
                    "[4] Exit\n" +
                    "Enter index of your choice: ");

                switch(Console.ReadLine())
                {
                    case "1": await Manager.EnterBooks();break;
                    case "2": Manager.PrintResult(); break;
                    case "3": Manager.Save(); break;
                    case "4": return;
                    default: Console.WriteLine("Wrong Input, plesae try again add number between 1-4");break;
                }
            }
        }
    }
}
