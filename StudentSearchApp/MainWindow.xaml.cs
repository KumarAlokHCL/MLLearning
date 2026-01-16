using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using StudentSearchApp.Models;
using StudentSearchApp.Services;

namespace StudentSearchApp
{
    public partial class MainWindow : Window
    {
        private StudentSearchEngine? _searchEngine;
        private List<Student> _students = new();

        public MainWindow()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            _searchEngine = App.SearchEngine;
            _students = App.Students;

            if (_searchEngine == null || _students.Count == 0)
            {
                MessageBox.Show("Failed to initialize search engine!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void ShowAllStudents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResultsDataGrid.ItemsSource = _students;
                UpdateResultsInfo($"Showing all {_students.Count} students");
                StatusTextBlock.Text = $"Loaded {_students.Count} students";
            }
            catch (Exception ex)
            {
                ShowError($"Error loading students: {ex.Message}");
            }
        }

        private void ShowPassed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var passed = _searchEngine?.SearchPassed(50) ?? new();
                ResultsDataGrid.ItemsSource = passed;
                UpdateResultsInfo($"Found {passed.Count} students with passing grades (A or B)");
                StatusTextBlock.Text = $"Passed: {passed.Count} students";
            }
            catch (Exception ex)
            {
                ShowError($"Error in passed search: {ex.Message}");
            }
        }

        private void ShowFailed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var failed = _searchEngine?.SearchFailed(50) ?? new();
                ResultsDataGrid.ItemsSource = failed;
                UpdateResultsInfo($"Found {failed.Count} students with failing grades (C, D, or F)");
                StatusTextBlock.Text = $"Failed: {failed.Count} students";
            }
            catch (Exception ex)
            {
                ShowError($"Error in failed search: {ex.Message}");
            }
        }

        private void ShowScience_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var science = _searchEngine?.SearchScience(50) ?? new();
                ResultsDataGrid.ItemsSource = science;
                UpdateResultsInfo($"Found {science.Count} students with science subjects");
                StatusTextBlock.Text = $"Science: {science.Count} students";
            }
            catch (Exception ex)
            {
                ShowError($"Error in science search: {ex.Message}");
            }
        }

        private void ShowPassedScience_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = _searchEngine?.SearchWithFilters("", passed: true, science: true, maxResults: 50) ?? new();
                ResultsDataGrid.ItemsSource = results;
                UpdateResultsInfo($"Found {results.Count} passed students with science subjects");
                StatusTextBlock.Text = $"Passed + Science: {results.Count} students";
            }
            catch (Exception ex)
            {
                ShowError($"Error in passed+science search: {ex.Message}");
            }
        }

        private void ShowFailedScience_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = _searchEngine?.SearchWithFilters("", passed: false, science: true, maxResults: 50) ?? new();
                ResultsDataGrid.ItemsSource = results;
                UpdateResultsInfo($"Found {results.Count} failed students with science subjects");
                StatusTextBlock.Text = $"Failed + Science: {results.Count} students";
            }
            catch (Exception ex)
            {
                ShowError($"Error in failed+science search: {ex.Message}");
            }
        }

        private void SemanticSearch_Click(object sender, RoutedEventArgs e)
        {
            PerformSemanticSearch();
        }

        private void SearchQueryTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                PerformSemanticSearch();
            }
        }

        private void PerformSemanticSearch()
        {
            try
            {
                string query = SearchQueryTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(query))
                {
                    ShowWarning("Please enter a search query");
                    return;
                }

                var results = _searchEngine?.SemanticSearch(query, topResults: 20) ?? new();

                if (results.Count == 0)
                {
                    ResultsDataGrid.ItemsSource = null;
                    UpdateResultsInfo("No results found for your query");
                    StatusTextBlock.Text = "No results found";
                    return;
                }

                // Convert to StudentWithSimilarity for display
                var displayResults = results.Select(r => new StudentWithSimilarity
                {
                    Id = r.Student.Id,
                    Name = r.Student.Name,
                    SchoolName = r.Student.SchoolName,
                    Subject = r.Student.Subject,
                    Address = r.Student.Address,
                    Grade = r.Student.Grade,
                    Similarity = r.Similarity
                }).ToList();

                // Add Relevance column for semantic search
                if (ResultsDataGrid.Columns.Count == 6)
                {
                    ResultsDataGrid.Columns.Add(
                        new DataGridTextColumn
                        {
                            Header = "Relevance",
                            Binding = new System.Windows.Data.Binding("SimilarityPercent"),
                            Width = 80
                        }
                    );
                }

                ResultsDataGrid.ItemsSource = displayResults;
                UpdateResultsInfo($"Found {results.Count} results for '{query}' (sorted by relevance)");
                StatusTextBlock.Text = $"Semantic search: {results.Count} results found";

                SearchQueryTextBox.Clear();
            }
            catch (Exception ex)
            {
                ShowError($"Error performing semantic search: {ex.Message}");
            }
        }

        private void SearchWithFilters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = FilteredSearchTextBox.Text.Trim();
                bool? passed = PassedCheckBox.IsChecked;
                bool? science = ScienceCheckBox.IsChecked;

                var results = _searchEngine?.SearchWithFilters(query, passed: passed, science: science, maxResults: 50) ?? new();

                if (results.Count == 0)
                {
                    ResultsDataGrid.ItemsSource = null;
                    UpdateResultsInfo("No results found matching your filters");
                    StatusTextBlock.Text = "No results found";
                    return;
                }

                ResultsDataGrid.ItemsSource = results;

                // Build filter description
                var filterDesc = new List<string>();
                if (!string.IsNullOrWhiteSpace(query))
                    filterDesc.Add($"Query: '{query}'");
                if (passed == true)
                    filterDesc.Add("Passed students only");
                if (science == true)
                    filterDesc.Add("Science subjects only");

                string description = filterDesc.Count > 0
                    ? string.Join(", ", filterDesc)
                    : "All filters";

                UpdateResultsInfo($"Found {results.Count} results matching: {description}");
                UpdateFilterInfo(description);
                StatusTextBlock.Text = $"Filtered search: {results.Count} results";
            }
            catch (Exception ex)
            {
                ShowError($"Error performing filtered search: {ex.Message}");
            }
        }

        private void UpdateResultsInfo(string message)
        {
            ResultsInfoTextBlock.Text = message;
        }

        private void UpdateFilterInfo(string filters)
        {
            FilterInfoPanel.Visibility = Visibility.Visible;
            FilterInfoText.Text = $"? Applied filters: {filters}";
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusTextBlock.Text = "Error occurred";
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            StatusTextBlock.Text = "Warning";
        }
    }

    /// <summary>
    /// Helper class to display similarity score alongside student data
    /// </summary>
    public class StudentWithSimilarity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public char Grade { get; set; }
        public float Similarity { get; set; }

        public string SimilarityPercent => $"{(int)(Similarity * 100)}%";
    }
}




