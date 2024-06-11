using KompCheck_Krc_Jaroslav.Menu;
using System;
using System.Threading.Tasks;

namespace KompCheck_Krc_Jaroslav
{
    public class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            await MainMenu.OpenMenu();
        }
    }
}
