using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KompCheck_Krc_Jaroslav.Data
{
    public static class SaveData
    {
        /// <summary>
        /// Methode zum asynchronen Öffnen des Dialogfensters
        /// </summary>
        /// <returns>Der Pfad der spezifischen Datei</returns>
        public static Task<string> SaveFileAsync()
        {
            try
            {
                var taskCompletionSource = new TaskCompletionSource<string>();
                var thread = new Thread(() =>
                {
                    string result = SaveFile();
                    taskCompletionSource.SetResult(result); 
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                return taskCompletionSource.Task;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }

        /// <summary>
        /// Speicherdialoge öffnen
        /// </summary>
        /// <returns>Der Pfad der spezifischen Datei</returns>
        private static string SaveFile()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt"
                };

                return saveFileDialog.ShowDialog() == DialogResult.OK ? saveFileDialog.FileName : null;
            }          
            catch(Exception ex) { Console.WriteLine(ex.Message); }
            return null ;
        }
    }
}
