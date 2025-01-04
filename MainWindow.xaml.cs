using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Media;
using System.Data.SQLite;

namespace TypingSkillApp
{

    public partial class MainWindow : Window
    {
        private readonly DatabaseHelper DH = new();
        private bool displayData = false;
        private int totalMillisecondsLeft = 20000;
        private string setText = "";
        private bool timerOn = false;
        private int stringIndex = 0;
        private string userString = "";
        //        private bool timerDone = false;
        private readonly DispatcherTimer timer = new();
        private int totalErrors = 0;
        public MainWindow()
        {
            InitializeComponent();
            RandomizeText();
            DH.CreateDatabase();
            UserInputTextBox.IsEnabled = true;
        }
        private readonly string[] randomTexts =
        {
            "apple banana cherry dog elephant frog garden house island jump kite lemon monkey night ocean pineapple queen rabbit sun tiger umbrella vase whale xylophone yellow zebra",
        };

        //private int typedWords = 0; // Track the number of words typed

        private void RandomizeText()
        {
            List<string> words = new List<string>
        {
            "ability", "absence", "academy", "accident", "account", "achieve", "acquire", "address", "advance",
            "advice", "against", "airport", "amazing", "ancient", "anxiety", "article", "artist", "balance",
            "balloon", "barbecue", "battery", "benefit", "biology", "blanket", "bravery", "broccoli", "buffalo",
            "cabinet", "cactus", "calendar", "capital", "captain", "careful", "carpet", "castle", "ceiling",
            "central", "century", "chamber", "charity", "chicken", "chimney", "chocolate", "citizen", "classic",
            "climate", "cluster", "coconut", "college", "comfort", "company", "concert", "conduct", "connect",
            "contact", "control", "courage", "crystal", "culture", "curtain", "custom", "danger", "declare",
            "defend", "dentist", "diamond", "digital", "dignity", "dolphin", "driver", "dynamic", "earlier",
            "economy", "educate", "elastic", "elegant", "element", "elevator", "embrace", "emotion", "endless",
            "energy", "engine", "English", "enhance", "evening", "example", "express", "fashion", "feature",
            "fiction", "fitness", "flatter", "flowers", "forever", "formula", "fortune", "freedom", "gallery",
            "garbage", "general", "geology", "giant", "ginger", "glasses", "grammar", "gratitude", "harvest",
            "hazard", "healthy", "heaven", "helpful", "history", "holiday", "horizon", "hospital", "housing",
            "husband", "iceberg", "imagine", "improve", "include", "inherit", "inspire", "instant", "instead",
            "journey", "justice", "kitchen", "kingdom", "laundry", "leather", "library", "license", "limited",
            "loyalty", "machine", "manager", "massage", "measure", "melody", "message", "mineral", "miracle",
            "modesty", "monitor", "morning", "mystery", "natural", "nearest", "network", "neutral", "nourish",
            "ocean", "officer", "opening", "operate", "organic", "package", "palace", "partner", "passion",
            "payment", "penalty", "pension", "perfect", "physics", "plastic", "pleasant", "popular", "pottery",
            "poverty", "predict", "premium", "prepare", "private", "produce", "program", "project", "promise",
            "protect", "purpose", "quality", "quarter", "reality", "recover", "regular", "relaxes", "release",
            "request", "respect", "romance", "science", "scratch", "secure", "serious", "session", "shelter",
            "silicon", "silence", "speaker", "stadium", "storage", "student", "subject", "support", "surface",
            "sweater", "teacher", "texture", "theater", "thunder", "timber", "tourism", "traffic", "transit",
            "trigger", "trophy", "trouble", "tsunami", "tuition", "tunnels", "uniform", "unknown", "unusual",
            "utility", "village", "victory", "virtual", "volcano", "waiting", "walking", "warrior", "welcome",
            "western", "whiskey", "willing", "witness", "worship", "writing", "yogurt", "younger", "zipper",
            "zombie"
        };
            RandomTextBlock.Document.Blocks.Clear();
            Random random = new();
            List<string> shuffledWords = words.OrderBy(x => random.Next()).ToList();
            List<string> first100Words = shuffledWords.Take(60).ToList();
            string shuffledWordsString = string.Join(" ", first100Words);
            setText = shuffledWordsString;
            Paragraph pg = new();
            foreach (char c in setText)
            {
                Run run = new(c.ToString());  // Create a Run for each letter
                pg.Inlines.Add(run);  // Add the Run to the paragraph's Inlines
            }
            RandomTextBlock.Document.Blocks.Add(pg);

        }
        // This method will handle the Enter key press

        private void UserInputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (RandomTextBlock.Document.Blocks.ElementAt(0) != null)
            {
                if (timerOn == false)
                {
                    timerOn = true;
                    Timer();
                    timer.Start();
                }
                /*    else if (timerDone == true)
                    {
                        WPMCounter(setText, userString);
                        TypingDone();
                    }*/
                Paragraph? pge = RandomTextBlock.Document.Blocks.ElementAt(0) as Paragraph;
#pragma warning disable CS8602 // Dereference of a possibly null reference. for this code I am checking before hand to ensure that there is no null values, however these warnings still occur
                Run? r = pge.Inlines.ElementAt(stringIndex) as Run;
                // Default user input from the key event
                string userInput = e.Key.ToString();
                // Helper function to map Key values to actual symbols
                static string MapKeyToSymbol(Key key, bool shiftPressed)
                {
                    return key switch
                    {
                        Key.OemComma => shiftPressed ? "<" : ",",
                        Key.OemPeriod => shiftPressed ? ">" : ".",
                        Key.OemMinus => shiftPressed ? "_" : "-",
                        Key.OemPlus => shiftPressed ? "+" : "=",
                        Key.OemQuestion => shiftPressed ? "?" : "/",
                        Key.Oem1 => shiftPressed ? ":" : ";",
                        Key.OemQuotes => shiftPressed ? "\"" : "'",
                        Key.OemOpenBrackets => shiftPressed ? "{" : "[",
                        Key.OemCloseBrackets => shiftPressed ? "}" : "]",
                        Key.OemBackslash => shiftPressed ? "|" : "\\",
                        _ => "",
                    };
                }
                // Check if the key corresponds to a letter or number
                if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9))
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                    {
                        userInput = userInput.ToUpper();
                    }
                    else
                    {
                        userInput = userInput.ToLower();
                    }
                }
                else if (e.Key == Key.Space)
                {
                    userInput = " "; // Handle space key as a string with a space character
                }
                else if (e.Key == Key.Back)
                {
                    r.Background = new SolidColorBrush(Colors.Transparent);
                    if (stringIndex > 0)
                    {
                        stringIndex--;
                    }
                    if (userString.Length > 0)
                    {
                        userString = userString[..^1];
                    }
                    else
                    {
                        userString = "";
                    }
                    r = pge.Inlines.ElementAt(stringIndex) as Run;
                    r.Background = new SolidColorBrush(Colors.LightBlue);
                }
                else
                {
                    // Handle symbols
                    bool shiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
                    string symbol = MapKeyToSymbol(e.Key, shiftPressed);
                    if (!string.IsNullOrEmpty(symbol))
                    {
                        userInput = symbol;
                    }
                }

                // Check user input against the target text and update background color
                if (userInput.Length == 1 && r != null && stringIndex < setText.Length - 1) // Only proceed if a single key is pressed
                {
                    if (userInput == r.Text)
                    {
                        r.Background = new SolidColorBrush(Colors.Green);
                        stringIndex++;
                    }
                    else
                    {
                        r.Background = new SolidColorBrush(Colors.Red);
                        totalErrors++;
                        stringIndex++;
                    }
                    userString += userInput;
                }

                // Highlight the next character
                if (stringIndex < pge.Inlines.Count)
                {
                    r = pge.Inlines.ElementAt(stringIndex) as Run;
                    r.Background = new SolidColorBrush(Colors.LightBlue);
                }
            }
        }
        private void Timer()
        {
            timer.Interval = TimeSpan.FromMilliseconds(10); // Tick every 10ms
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            totalMillisecondsLeft -= 10;
            double seconds = totalMillisecondsLeft / 1000.0;
            TimerTextBlock.Text = $"Time Left: {seconds:F2}s";
            if (totalMillisecondsLeft <= 0)
            {
                timer.Stop();
                TimerTextBlock.Text = "Time's up!";
                //timerDone = true;
                WPMCounter(setText, userString);
                TypingDone();
            }
        }

        private void TypingDone()
        {
            //disable typing box, display words per minute
            //enable a restart button
            UserInputTextBox.IsEnabled = false;
            RestartButton.Visibility = Visibility.Visible;
            DisplayDataButton.Visibility = Visibility.Visible;
        }



        private void WPMCounter(string setString, string writtenString)
        {
            //MessageBox.Show($"{writtenString.Length} {totalErrors}");
            string[] setWords = setString.Split(' ');
            string[] writtenWords = setString.Split(' ');
            int correctWords = 0;
            double acc = (double)(writtenString.Length - totalErrors) / writtenString.Length;
            acc *= 100;
            for (int i = 0; i < writtenWords.Length; i++)
            {
                if (writtenWords[i] == setWords[i])
                {
                    correctWords++;
                }
            }
            
            WpmTextBlock.Visibility = Visibility.Visible;
            WpmTextBlock.Text = $"Words Per Minute: {correctWords}    Errors: {totalErrors}      Accuracy: {acc:F2}%";
            DH.InsertResult(correctWords, totalErrors, acc);
        }


        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset variables
            totalMillisecondsLeft = 20000; // or any other initial value
            totalErrors = 0;
            stringIndex = 0;
            userString = "";
            //         timerDone = false;
            timerOn = false;

            // Hide WPM, accuracy, and error text
            WpmTextBlock.Visibility = Visibility.Collapsed;
            TimerTextBlock.Text = $"Time Left: {totalMillisecondsLeft / 1000.0:F2}s";

            // Hide the restart button and show the input box
            RestartButton.Visibility = Visibility.Collapsed;
            UserInputTextBox.Visibility = Visibility.Visible;

            UserInputTextBox.Text = string.Empty;
            UserInputTextBox.IsEnabled = true;

            // Randomize the text again for the new session
            RandomizeText();
        }

        public class DatabaseHelper
        {
            private string connectionString = "Data Source=typing_results.db;Version=3;";

            public void CreateDatabase()
            {
                using SQLiteConnection connection = new SQLiteConnection(connectionString);
                connection.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS TypingResults (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    WPM INTEGER,
                    Errors INTEGER,
                    Accuracy REAL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

                using SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }

            public void InsertResult(int wpm, int errors, double accuracy)
            {
                using SQLiteConnection connection = new SQLiteConnection(connectionString);
                connection.Open();

                string insertQuery = "INSERT INTO TypingResults (WPM, Errors, Accuracy) VALUES (@WPM, @Errors, @Accuracy)";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@WPM", wpm);
                    command.Parameters.AddWithValue("@Errors", errors);
                    command.Parameters.AddWithValue("@Accuracy", accuracy);

                    command.ExecuteNonQuery();
                }
            }
            public class TypingResult
            {
                public int ID { get; set; }
                public int WPM { get; set; }
                public int Errors { get; set; }
                public double Accuracy { get; set; }
                public DateTime Timestamp { get; set; }
            }
            public List<TypingResult> GetResults()
            {
                List<TypingResult> results = new List<TypingResult>();

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM TypingResults";

                    using SQLiteCommand command = new(selectQuery, connection);
                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        TypingResult result = new TypingResult
                        {
                            ID = reader.GetInt32(0),
                            WPM = reader.GetInt32(1),
                            Errors = reader.GetInt32(2),
                            Accuracy = reader.GetDouble(3),
                            Timestamp = reader.GetDateTime(4)
                        };

                        results.Add(result);
                    }
                }

                return results;
            }
        }
        private void DisplayDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayData == true)
            {
                displayData = false;
                DatabaseContentListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                var results = DH.GetResults();

                // Clear the ListBox before displaying new data
                DatabaseContentListBox.Items.Clear();

                // Display the results in the ListBox
                foreach (var result in results)
                {
                    string resultText = $"ID: {result.ID}, WPM: {result.WPM}, Errors: {result.Errors}, Accuracy: {result.Accuracy}, Timestamp: {result.Timestamp}";
                    DatabaseContentListBox.Items.Add(resultText);
                }
                displayData = true;
                DatabaseContentListBox.Visibility = Visibility.Visible;
            }
        }
    }
}