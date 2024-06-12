using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KompCheck_Krc_Jaroslav.Data
{
    public static class UploadData
    {
        /// <summary>
        /// Methode zum asynchronen Öffnen des Dialogfensters
        /// </summary>
        /// <param name="prompt">Titelbezeichnung</param>
        /// <returns>Der Pfad der spezifischen Datei</returns>
        public static Task<string> OpenFileAsync(string prompt)
        {
            try
            {
                TaskCompletionSource<string> taskSource = new TaskCompletionSource<string>();
                Thread thread = new Thread(() =>
                {
                    string result = OpenFile(prompt);
                    taskSource.SetResult(result); 
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                
                return taskSource.Task;
            }
            catch (ThreadAbortException) { }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }

        /// <summary>
        /// Öffnen des Unterbuch-Dialogfensters zur Eingabe eines bestimmten Buches
        /// </summary>
        /// <param name="prompt">Titelbezeichnung</param>
        /// <returns>Der Pfad der spezifischen Datei</returns>
        private static string OpenFile(string prompt)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt",
                    Title = prompt
                };
                return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : null;
            }
            catch(FileNotFoundException ex) { Console.WriteLine(ex.Message); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }           
            return null; 
        }
    }
}
