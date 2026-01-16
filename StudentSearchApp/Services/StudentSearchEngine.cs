using Microsoft.ML;
using Microsoft.ML.Data;
using StudentSearchApp.Models;

namespace StudentSearchApp.Services
{
    /// <summary>
    /// Input model for ML.NET text featurization
    /// </summary>
    public class TextData
    {
        [LoadColumn(0)]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Output model for featurized text
    /// </summary>
    public class TextFeatures
    {
        public float[] Features { get; set; } = Array.Empty<float>();
    }

    /// <summary>
    /// ML.NET-based student search engine using NLP
    /// </summary>
    public class StudentSearchEngine
    {
        private readonly MLContext _mlContext;
        private ITransformer? _featurizationModel;
        private List<Student> _studentDatabase = new();
        private List<TextFeatures> _studentFeatures = new();

        public StudentSearchEngine()
        {
            _mlContext = new MLContext();
        }

        /// <summary>
        /// Initialize the search engine with student data
        /// </summary>
        public void Initialize(List<Student> students)
        {
            _studentDatabase = students;
            BuildTextFeaturizationPipeline();
            FeaturizeStudentData();
        }

        /// <summary>
        /// Build the ML.NET text featurization pipeline
        /// </summary>
        private void BuildTextFeaturizationPipeline()
        {
            // Create pipeline that:
            // 1. Converts student data to text
            // 2. Featurizes text using TfidfVectorizer
            var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(TextData.Text));

            // Fit pipeline with actual student data to learn vocabulary and TF-IDF statistics
            var trainingData = _mlContext.Data.LoadFromEnumerable(
                _studentDatabase.Select(s => new TextData { Text = CreateStudentSearchText(s) }).ToList());
            _featurizationModel = pipeline.Fit(trainingData);
        }

        /// <summary>
        /// Featurize all students' text data
        /// </summary>
        private void FeaturizeStudentData()
        {
            _studentFeatures.Clear();

            foreach (var student in _studentDatabase)
            {
                string studentText = CreateStudentSearchText(student);
                var textData = new TextData { Text = studentText };
                var dataView = _mlContext.Data.LoadFromEnumerable(new[] { textData });
                var transformed = _featurizationModel!.Transform(dataView);
                var features = _mlContext.Data.CreateEnumerable<TextFeatures>(transformed, false).First();
                _studentFeatures.Add(features);
            }
        }

        /// <summary>
        /// Create searchable text from student data
        /// </summary>
        private string CreateStudentSearchText(Student student)
        {
            return $"{student.Name} {student.Address} {student.SchoolName} {student.Subject} grade{student.Grade}".ToLower();
        }

        /// <summary>
        /// Calculate cosine similarity between two feature vectors
        /// </summary>
        private float CalculateSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
                return 0f;

            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = (float)Math.Sqrt(magnitude1);
            magnitude2 = (float)Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0f;

            return dotProduct / (magnitude1 * magnitude2);
        }

        /// <summary>
        /// Semantic search using NLP and featurized text
        /// </summary>
        public List<(Student Student, float Similarity)> SemanticSearch(string query, int topResults = 10)
        {
            // Featurize the query
            var queryData = new TextData { Text = query.ToLower() };
            var dataView = _mlContext.Data.LoadFromEnumerable(new[] { queryData });
            var transformed = _featurizationModel!.Transform(dataView);
            var queryFeatures = _mlContext.Data.CreateEnumerable<TextFeatures>(transformed, false).First();

            // Calculate similarity scores
            var results = new List<(Student, float)>();
            for (int i = 0; i < _studentDatabase.Count; i++)
            {
                float similarity = CalculateSimilarity(queryFeatures.Features, _studentFeatures[i].Features);
                results.Add((_studentDatabase[i], similarity));
            }

            // Sort by similarity and return top results
            return results
                .OrderByDescending(x => x.Item2)
                .Take(topResults)
                .ToList();
        }

        /// <summary>
        /// Search with grade filter (pass = grade A or B)
        /// </summary>
        public List<Student> SearchPassed(int maxResults = 20)
        {
            return _studentDatabase
                .Where(s => s.IsPassed())
                .Take(maxResults)
                .ToList();
        }

        /// <summary>
        /// Search with failed grade filter (failed = grade C, D, or F)
        /// </summary>
        public List<Student> SearchFailed(int maxResults = 20)
        {
            return _studentDatabase
                .Where(s => !s.IsPassed())
                .Take(maxResults)
                .ToList();
        }

        /// <summary>
        /// Search with subject filter (science subjects)
        /// </summary>
        public List<Student> SearchScience(int maxResults = 20)
        {
            return _studentDatabase
                .Where(s => s.HasScience())
                .Take(maxResults)
                .ToList();
        }

        /// <summary>
        /// Combined semantic search with filters for passed/failed students
        /// </summary>
        public List<Student> SearchWithFilters(string query, bool? passed = null, bool? science = null, int maxResults = 20)
        {
            // Start with semantic search
            var semanticResults = SemanticSearch(query, maxResults * 3);
            var results = semanticResults.Select(x => x.Student).ToList();

            // Apply grade filter (passed: true = A or B, passed: false = C, D, or F)
            if (passed.HasValue)
            {
                if (passed.Value)
                {
                    results = results.Where(s => s.IsPassed()).ToList();
                }
                else
                {
                    results = results.Where(s => !s.IsPassed()).ToList();
                }
            }

            // Apply science filter
            if (science.HasValue && science.Value)
            {
                results = results.Where(s => s.HasScience()).ToList();
            }

            return results.Take(maxResults).ToList();
        }

        /// <summary>
        /// Get total student count
        /// </summary>
        public int GetStudentCount() => _studentDatabase.Count;
    }
}