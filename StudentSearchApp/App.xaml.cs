using System.Windows;
using StudentSearchApp.Data;
using StudentSearchApp.Services;

namespace StudentSearchApp
{
    public partial class App : Application
    {
        public static StudentSearchEngine? SearchEngine { get; private set; }
        public static List<Models.Student> Students { get; private set; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize data and search engine
            InitializeSearchEngine();
        }

        private static void InitializeSearchEngine()
        {
            // Generate sample students
            Students = SampleDataGenerator.GenerateSampleStudents(100);

            // Initialize search engine
            SearchEngine = new StudentSearchEngine();
            SearchEngine.Initialize(Students);
        }
    }
}
