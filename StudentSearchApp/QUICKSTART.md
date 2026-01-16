# ML.NET Student Search Engine - Quick Start Guide

## ?? Quick Start

### Prerequisites
- .NET 10 SDK installed
- Visual Studio 2022 or VS Code

### Build and Run

```bash
# Navigate to project directory
cd StudentSearchApp

# Build the project
dotnet build

# Run the application
dotnet run
```

## ?? Project Configuration

**Framework**: .NET 10.0  
**Language**: C# 14  
**ML Library**: Microsoft.ML 3.0.1 (Latest stable for .NET 10)  
**Output Type**: Console Application  

### Dependencies (in StudentSearchApp.csproj)
```xml
<PackageReference Include="Microsoft.ML" Version="3.0.1" />
```

## ?? Core Components

### 1. Student Model (Models/Student.cs)
Represents a student with:
- Id, Name, Address, SchoolName, Subject, Grade

Methods:
- `IsPassed()` - Returns true if grade is A or B
- `HasScience()` - Returns true if subject contains science keywords

### 2. Sample Data Generator (Data/SampleDataGenerator.cs)
- Generates 100 unique student records
- Uses fixed random seed (42) for reproducibility
- Provides realistic names, addresses, schools, and subjects

### 3. Search Engine (Services/StudentSearchEngine.cs)
Main NLP-based search engine with:

**Key Methods:**
```csharp
// Initialize with student data
Initialize(List<Student> students)

// Semantic search using NLP
SemanticSearch(string query, int topResults = 10)

// Filter methods
SearchPassed(int maxResults = 20)      // Grade A or B
SearchScience(int maxResults = 20)     // Science subjects

// Combined filtering
SearchWithFilters(string query, bool? passed, bool? science)
```

**Internal Process:**
1. Builds ML.NET text featurization pipeline
2. Converts each student to 50-dimensional feature vector
3. On search: vectorizes query and calculates cosine similarity
4. Returns ranked results by similarity score

### 4. Console Application (Program.cs)
Interactive UI with:
- Help menu with command descriptions
- Semantic search functionality
- Filtering for passed students
- Filtering for science subjects
- Display all students option
- Visual relevance bars for search results

## ?? How the NLP Search Works

### Step 1: Text Featurization
```csharp
var pipeline = _mlContext.Transforms.Text.FeaturizeText(
    outputColumnName: "Features",
    inputColumnName: nameof(TextData.Text))
```

This uses ML.NET's built-in TF-IDF vectorizer to convert text to numerical features.

### Step 2: Student Data Vectorization
Each student record is converted to searchable text:
```
"John Smith 123 Main Street, Delhi Delhi Public School Physics gradeB"
```
Then featurized to a 50-dimensional vector.

### Step 3: Similarity Calculation
Cosine similarity formula:
```
similarity = (V1 · V2) / (||V1|| × ||V2||)
```
Where V1 = query vector, V2 = student vector

### Step 4: Ranking
Results sorted by similarity score (highest first) and displayed with:
- Student information
- Relevance percentage (0-100%)
- Visual bar chart

## ?? Example Usage Scenarios

### Scenario 1: Find Passed Students
```
Input:  passed
Output: List of students with Grade A or B
```

### Scenario 2: Find Science Students
```
Input:  science
Output: List of students studying Physics, Chemistry, or Biology
```

### Scenario 3: Semantic Search
```
Input:  physics students in delhi
Process:
1. Convert query to vector
2. Calculate similarity with all 100 student vectors
3. Rank by similarity
4. Display top 15 results with relevance scores
```

### Scenario 4: Natural Language Query
```
Input:  students with good grades
Output: Top 15 students with highest semantic similarity
Note: The exact phrase doesn't need to match; semantic meaning does!
```

## ?? Search Result Interpretation

Each search result shows:

```
01. [????????????????????] 87% - 001. Aarav Kumar, 123 Main St, Delhi...
```

- **01.** - Result rank
- **[????????]** - Visual relevance bar (20 segments)
- **87%** - Relevance percentage (higher = better match)
- **Student info** - Full student details

## ?? Advanced Features

### Custom Search Filters
```csharp
// Search for physics students who passed
var results = searchEngine.SearchWithFilters(
    query: "physics", 
    passed: true,      // Only students with Grade A or B
    science: true      // Only science subjects
);
```

### Adjusting Result Count
```csharp
// Get top 20 results instead of 10
var results = searchEngine.SemanticSearch(query, topResults: 20);
```

### Adding New Subjects
Edit `SampleDataGenerator.cs`:
```csharp
private static readonly string[] Subjects = new[]
{
    // ... existing subjects ...
    "Data Science",      // Add new subjects here
    "Artificial Intelligence"
};
```

## ?? Performance Tips

1. **First Run**: Slower due to pipeline initialization (~2-3 seconds)
2. **Subsequent Searches**: Fast (<100ms) due to cached featurization
3. **Memory**: ~50MB for 100 students with 50-dimensional vectors
4. **Scaling**: Can handle 1000s of students, but accuracy may vary

## ?? Troubleshooting

### Application Won't Build
```
Error: "error NU1101: Unable to find package 'Microsoft.ML'"
Solution: 
1. Check internet connection
2. Run: dotnet restore
3. Ensure NuGet feed is accessible
```

### Slow Performance
```
Possible Causes:
1. First run includes pipeline initialization
2. Large number of students (>1000)
Solution:
1. Run warm-up search first
2. Reduce feature vector dimensions (change VectorType(50))
```

### No Search Results
```
Possible Causes:
1. Query too specific and no exact matches
2. Empty student database
Solution:
1. Try broader search terms
2. Verify Initialize() was called with student data
```

## ?? Key Concepts

### TF-IDF (Term Frequency-Inverse Document Frequency)
- Measures importance of words in documents
- Frequent words get lower weights
- Unique words get higher weights
- Used for text vectorization

### Cosine Similarity
- Measures angle between two vectors
- Range: 0 (opposite) to 1 (identical)
- Commonly used for text similarity

### Feature Vector
- Numerical representation of text
- Each dimension represents a term
- Can be used for mathematical operations

## ?? Learning Resources

### Understand ML.NET
- [Microsoft ML.NET Documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [Text Featurization in ML.NET](https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/train-model-textual-ml-net)

### Understand NLP Concepts
- [TF-IDF Explanation](https://en.wikipedia.org/wiki/Tf%E2%80%93idf)
- [Cosine Similarity](https://en.wikipedia.org/wiki/Cosine_similarity)

## ?? Project Structure

```
StudentSearchApp/
??? Models/
?   ??? Student.cs                    # Student entity class
??? Data/
?   ??? SampleDataGenerator.cs        # Creates sample data
??? Services/
?   ??? StudentSearchEngine.cs        # ML.NET search logic
??? Program.cs                        # Console UI
??? StudentSearchApp.csproj           # Project configuration
??? README.md                         # Full documentation
??? QUICKSTART.md                     # This file
??? bin/
    ??? Debug/
        ??? net10.0/
            ??? StudentSearchApp.dll  # Compiled executable
```

## ?? Next Steps

1. **Run the application**: `dotnet run`
2. **Try search queries**: "physics", "delhi", "students with good grades"
3. **Explore the code**: Check StudentSearchEngine.cs for NLP logic
4. **Extend functionality**: Add database, more filters, or UI improvements

## ?? Notes

- All sample data is generated with a fixed random seed (42) for reproducibility
- Search results are deterministic (same query = same results)
- The application is designed for educational purposes
- For production use, consider database backing and caching

---

**Version**: 1.0  
**Language**: C# 14  
**Framework**: .NET 10  
**ML Library**: ML.NET 5 (3.0.1)
