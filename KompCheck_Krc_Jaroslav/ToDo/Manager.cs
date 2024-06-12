using KompCheck_Krc_Jaroslav.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KompCheck_Krc_Jaroslav.ToDo
{
    public static class Manager
    {
        private static int _linesBook1;
        private static int _linesBook2;

        private static int _wordsBook1;
        private static int _wordsBook2;

        private static string book1Path;
        private static string book2Path;

        private static Dictionary<string, int> _top20Book1 = new Dictionary<string, int>();
        private static Dictionary<string, int> _top20Book2 = new Dictionary<string, int>();

        private static Dictionary<string, int> _allWordsBook1 = new Dictionary<string, int>();
        private static Dictionary<string, int> _allWordsBook2 = new Dictionary<string, int>();

        private static readonly object _lock = new object();

        private static int uniqueWordsBook1;
        private static int uniqueWordsBook2;
        private static int allContainsWords;
        private static double percentContains;


        /// <summary>
        /// Zwei Bücher gleichzeitig asynchron laden.
        /// </summary>
        /// <returns>Wir geben den Faden lose zurück</returns>
        public static async Task EnterBooks()
        {
            Console.Clear();
            try
            {
                MessageBox.Show("Enter first book please...");
                book1Path = await UploadData.OpenFileAsync("Enter first book");
                MessageBox.Show("Enter second book please...");
                book2Path = await UploadData.OpenFileAsync("Enter second book");

                if (book1Path != null && book2Path != null)
                {
                    var book1Task = ProcessBooks(book1Path,1);
                    var book2Task = ProcessBooks(book2Path,2);

                    var result = await Task.WhenAll(book1Task, book2Task);
                    
                    _linesBook1 = result[0].lines;
                    _wordsBook1 = result[0].words;
                    _top20Book1 = result[0].top20;
                    _allWordsBook1 = result[0].allWords;
                    Console.WriteLine("First book loaded successfully.");

                    _linesBook2 = result[1].lines;
                    _wordsBook2 = result[1].words;
                    _top20Book2 = result[1].top20;
                    _allWordsBook2 = result[1].allWords;
                    Console.WriteLine("Second book loaded successfully.");

                    CountPercentMatches();
                }
                else
                {
                    if (book1Path == null){ Console.WriteLine("--- Failed to load the first book. ---");}

                    if (book2Path == null){ Console.WriteLine("--- Failed to load the second book. ---");}
                }
            }
            catch(FileNotFoundException ex) { Console.WriteLine(ex.Message); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
           
        }

        /// <summary>
        /// Öffnen Sie ein bestimmtes Buch und berechnen Sie die Wortzeile und Prozentsätze
        /// </summary>
        /// <param name="bookPath">Pfad eines Buches</param>
        /// <param name="bookNumber">Buchnummerierung</param>
        /// <returns>Gibt die Gesamtzahl der Zeilen, Wörter, ein Wörterbuch mit den 20 am häufigsten verwendeten Wörtern und ein Wörterbuch mit allen Wörtern zurück</returns>
        private static async Task<(int lines, int words, Dictionary<string,int> top20, Dictionary<string,int> allWords)> ProcessBooks(string bookPath, int bookNumber)
        {
            try
            {
                Console.Clear();
                int lines = 0;
                int words = 0;
                int cursorTop;
                int percent = 0;

                lock (_lock)
                {
                    cursorTop = Console.CursorTop;
                    Console.SetCursorPosition(0, cursorTop + bookNumber - 1);
                    Console.WriteLine($"{percent}%");
                }

                Dictionary<string, int> wordCounts = new Dictionary<string, int>();
                int allLines = File.ReadAllLines(bookPath).Count();
                using (StreamReader reader = new StreamReader(bookPath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        lines++;
                        string pattern = @"\b\w+\b";
                        MatchCollection matches = Regex.Matches(line, pattern);
                        words += matches.Count;

                        foreach (Match match in matches)
                        {
                            string word = match.Value.ToLower();
                            if (wordCounts.ContainsKey(word)) { wordCounts[word]++; }
                            else { wordCounts[word] = 1; }
                        }

                        lock (_lock)
                        {
                            percent = (int)Math.Round((double)(100 * lines) / allLines);
                            Console.SetCursorPosition(0, cursorTop + bookNumber - 1);
                            Console.Write("{0}%", percent);
                        }
                    }
                }
                Console.Clear();
                return (lines, words, wordCounts
                                      .OrderByDescending(top20 => top20.Value)
                                      .Take(20)
                                      .ToDictionary(top20 => top20.Key, top20 => top20.Value), wordCounts);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return (0,0,null,null);
        }

        /// <summary>
        /// Buchdaten in der Konsole anzeigen
        /// </summary>
        public static void PrintResult() 
        {
            Console.Clear();
            if (string.IsNullOrEmpty(book1Path) || string.IsNullOrEmpty(book2Path))
            {
                Console.WriteLine("Books have not been entered");
                return;
            }
            Console.WriteLine($"Book 1:\nPath: {book1Path}\nLines: {_linesBook1}\nWords: {_wordsBook1}\nTop20 words:");       
            foreach ( var x in _top20Book1.OrderByDescending(x => x.Value)) {  Console.WriteLine($"{x.Key} : {x.Value}"); }

            Console.WriteLine($"\nBook 2:\nPath: {book2Path}\nLines: {_linesBook2}\nWords: {_wordsBook2}\nTop20 words:");
            foreach (var x in _top20Book2.OrderByDescending(x => x.Value)) { Console.WriteLine($"{x.Key} : {x.Value}"); }

            Console.WriteLine($"\n\nAll common words: {allContainsWords}\nUnique words of the first book: {uniqueWordsBook1}\nUnique words of the second book: {uniqueWordsBook2}\n\t{percentContains}% of the words match\n\n");
        }

        /// <summary>
        /// Speichern eines Textdokuments mit allen Daten
        /// </summary>
        public static async void Save()
        {
            try
            {
                Console.Clear();
                if (string.IsNullOrEmpty(book1Path) || string.IsNullOrEmpty(book2Path))
                {
                    Console.WriteLine("Books have not been entered.");
                    return;
                }

                string savePath = await SaveData.SaveFileAsync();
                if (savePath != null)
                {
                    using (StreamWriter writer = new StreamWriter(savePath))
                    {
                        await writer.WriteLineAsync($"Book 1:\nPath: {book1Path}\nLines: {_linesBook1}\nWords: {_wordsBook1}\nTop20 words:");
                        foreach (var x in _top20Book1.OrderByDescending(x => x.Value)) {await writer.WriteLineAsync($"{x.Key} : {x.Value}"); }
         
                        await writer.WriteLineAsync($"\nBook 2:\nPath: {book2Path}\nLines: {_linesBook2}\nWords: {_wordsBook2}\nTop20 words:");
                        foreach (var x in _top20Book2.OrderByDescending(x => x.Value)) {await writer.WriteLineAsync($"{x.Key} : {x.Value}"); }

                        await writer.WriteLineAsync($"\n\nAll common words: {allContainsWords}\nUnique words of the first book: {uniqueWordsBook1}\nUnique words of the second book: {uniqueWordsBook2}\n\t{percentContains}% of the words match\n\n");
                    }

                    Console.WriteLine("Data saved successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to save data.");
                }
            }
            catch(IOException ex) { Console.WriteLine(ex.Message); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }  
        }

        /// <summary>
        /// Erkennung einzigartiger Wörter, aller gängigen Wörter und anschließende Anzeige auf der Konsole
        /// </summary>
        private static void CountPercentMatches()
        {
            uniqueWordsBook1 = _allWordsBook1.Keys.Count;
            uniqueWordsBook2 = _allWordsBook2.Keys.Count;
            allContainsWords = _allWordsBook1.Where(x => _allWordsBook2.ContainsKey(x.Key)).Count();
            percentContains = 0;

            if (uniqueWordsBook1 > uniqueWordsBook2)
            {
                percentContains = Math.Round((double)(allContainsWords * 100) / uniqueWordsBook1);
            }
            else
            {
                percentContains = Math.Round((double)(allContainsWords * 100) / uniqueWordsBook2);
            }          
        }
    }
}
