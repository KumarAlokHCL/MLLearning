using StudentSearchApp.Data;
using StudentSearchApp.Services;

// Initialize data
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     ML.NET Student Search Engine - NLP Based Search      ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

Console.WriteLine("Generating 100 sample students...");
var students = SampleDataGenerator.GenerateSampleStudents(100);
Console.WriteLine($"✓ Generated {students.Count} students");
Console.WriteLine();

Console.WriteLine("Initializing ML.NET search engine...");
var searchEngine = new StudentSearchEngine();
searchEngine.Initialize(students);
Console.WriteLine("✓ Search engine initialized with text featurization pipeline");
Console.WriteLine();

// Display help menu
DisplayHelpMenu();

// Interactive search loop
while (true)
{
    Console.Write("\n> Enter search query (or 'help'/'exit'): ");
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    input = input.Trim().ToLower();

    if (input == "exit" || input == "quit")
    {
        Console.WriteLine("\nThank you for using Student Search Engine. Goodbye!");
        break;
    }

    if (input == "help")
    {
        DisplayHelpMenu();
        continue;
    }

    if (input == "show all")
    {
        DisplayAllStudents(students);
        continue;
    }

    if (input == "passed")
    {
        DisplayPassedStudents(searchEngine);
        continue;
    }

    if (input == "failed")
    {
        DisplayFailedStudents(searchEngine);
        continue;
    }

    if (input == "science")
    {
        DisplayScienceStudents(searchEngine);
        continue;
    }

    if (input == "passed science")
    {
        DisplayFilteredStudents(searchEngine, query: "", passed: true, science: true, title: "PASSED STUDENTS WITH SCIENCE SUBJECTS");
        continue;
    }

    if (input == "failed science")
    {
        DisplayFilteredStudents(searchEngine, query: "", passed: false, science: true, title: "FAILED STUDENTS WITH SCIENCE SUBJECTS");
        continue;
    }

    // Handle custom search queries with optional filters
    HandleSearchQuery(searchEngine, input);
}

void DisplayHelpMenu()
{
    Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                        HELP MENU                         ║");
    Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
    Console.WriteLine("║ COMMAND              │ DESCRIPTION                       ║");
    Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
    Console.WriteLine("║ passed               │ Show all students with grade A/B  ║");
    Console.WriteLine("║ failed               │ Show all students with grade C/D/F║");
    Console.WriteLine("║ science              │ Show students with Science subjs  ║");
    Console.WriteLine("║ passed science       │ Passed + Science                  ║");
    Console.WriteLine("║ failed science       │ Failed + Science                  ║");
    Console.WriteLine("║ show all             │ Display all 100 students          ║");
    Console.WriteLine("║ [search query]       │ Semantic search (NLP-based)      ║");
    Console.WriteLine("║ help                 │ Display this help menu            ║");
    Console.WriteLine("║ exit                 │ Exit the application              ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
    Console.WriteLine("\nEXAMPLE QUERIES:");
    Console.WriteLine("  • \"student named john\" - Find students with name like John");
    Console.WriteLine("  • \"students in delhi\" - Find students in Delhi");
    Console.WriteLine("  • \"physics students\" - Find students studying Physics");
    Console.WriteLine("  • \"students with good grades\" - Find high-performing students");
    Console.WriteLine("  • \"delhi public school\" - Find students from Delhi Public School");
}

void DisplayAllStudents(List<StudentSearchApp.Models.Student> studentList)
{
    Console.WriteLine($"\n╔════ ALL {studentList.Count} STUDENTS ════╗\n");
    for (int i = 0; i < studentList.Count; i++)
    {
        Console.WriteLine($"{i + 1:D3}. {studentList[i]}");
        if ((i + 1) % 10 == 0)
        {
            Console.WriteLine();
        }
    }
}

void DisplayPassedStudents(StudentSearchEngine engine)
{
    Console.WriteLine("\n╔════ STUDENTS WHO PASSED (Grade A or B) ════╗\n");
    var passed = engine.SearchPassed(50);

    if (passed.Count == 0)
    {
        Console.WriteLine("No students found with passing grades.");
        return;
    }

    for (int i = 0; i < passed.Count; i++)
    {
        Console.WriteLine($"{i + 1:D3}. {passed[i]}");
    }

    Console.WriteLine($"\nTotal: {passed.Count} students passed");
}

void DisplayFailedStudents(StudentSearchEngine engine)
{
    Console.WriteLine("\n╔════ STUDENTS WHO FAILED (Grade C, D, or F) ════╗\n");
    var failed = engine.SearchFailed(50);

    if (failed.Count == 0)
    {
        Console.WriteLine("No students found with failing grades.");
        return;
    }

    for (int i = 0; i < failed.Count; i++)
    {
        Console.WriteLine($"{i + 1:D3}. {failed[i]}");
    }

    Console.WriteLine($"\nTotal: {failed.Count} students failed");
}

void DisplayScienceStudents(StudentSearchEngine engine)
{
    Console.WriteLine("\n╔════ STUDENTS WITH SCIENCE SUBJECTS ════╗\n");
    var science = engine.SearchScience(50);

    if (science.Count == 0)
    {
        Console.WriteLine("No students found with science subjects.");
        return;
    }

    for (int i = 0; i < science.Count; i++)
    {
        Console.WriteLine($"{i + 1:D3}. {science[i]}");
    }

    Console.WriteLine($"\nTotal: {science.Count} students with science subjects");
}

void DisplayFilteredStudents(StudentSearchEngine engine, string query, bool? passed = null, bool? science = null, string title = "FILTERED RESULTS")
{
    Console.WriteLine($"\n╔════ {title} ════╗\n");
    var filtered = engine.SearchWithFilters(query, passed: passed, science: science, maxResults: 50);

    if (filtered.Count == 0)
    {
        Console.WriteLine("No students found matching the filters.");
        return;
    }

    for (int i = 0; i < filtered.Count; i++)
    {
        Console.WriteLine($"{i + 1:D3}. {filtered[i]}");
    }

    Console.WriteLine($"\nTotal: {filtered.Count} students matched");
}

void PerformSemanticSearch(StudentSearchEngine engine, string query)
{
    Console.WriteLine($"\n╔════ SEMANTIC SEARCH: \"{query}\" ════╗\n");

    var results = engine.SemanticSearch(query, topResults: 15);

    if (results.Count == 0)
    {
        Console.WriteLine("No students found matching your query.");
        return;
    }

    Console.WriteLine($"Found {results.Count} results (sorted by relevance):\n");

    for (int i = 0; i < results.Count; i++)
    {
        var (student, similarity) = results[i];
        int relevanceScore = (int)(similarity * 100);
        string relevanceBar = new string('█', Math.Max(1, relevanceScore / 5)) + new string('░', 20 - Math.Max(1, relevanceScore / 5));

        Console.WriteLine($"{i + 1:D2}. [{relevanceBar}] {relevanceScore}% - {student}");
    }
}

void HandleSearchQuery(StudentSearchEngine engine, string input)
{
    // Check for combined filters in the query
    if (input.Contains("passed") && input.Contains("science"))
    {
        // Remove filter keywords and use remaining text as query
        string query = input.Replace("passed", "").Replace("science", "").Trim();
        Console.WriteLine($"\n╔════ SEMANTIC SEARCH WITH FILTERS: \"{query}\" (Passed + Science) ════╗\n");
        var results = engine.SearchWithFilters(query, passed: true, science: true, maxResults: 15);
        DisplaySearchResults(results);
        return;
    }

    if (input.Contains("failed") && input.Contains("science"))
    {
        // Remove filter keywords and use remaining text as query
        string query = input.Replace("failed", "").Replace("science", "").Trim();
        Console.WriteLine($"\n╔════ SEMANTIC SEARCH WITH FILTERS: \"{query}\" (Failed + Science) ════╗\n");
        var results = engine.SearchWithFilters(query, passed: false, science: true, maxResults: 15);
        DisplaySearchResults(results);
        return;
    }

    if (input.Contains("passed"))
    {
        // Remove filter keyword and use remaining text as query
        string query = input.Replace("passed", "").Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            query = "";
        }
        Console.WriteLine($"\n╔════ SEMANTIC SEARCH WITH FILTER: \"{query}\" (Passed Only) ════╗\n");
        var results = engine.SearchWithFilters(query, passed: true, maxResults: 15);
        DisplaySearchResults(results);
        return;
    }

    if (input.Contains("failed"))
    {
        // Remove filter keyword and use remaining text as query
        string query = input.Replace("failed", "").Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            query = "";
        }
        Console.WriteLine($"\n╔════ SEMANTIC SEARCH WITH FILTER: \"{query}\" (Failed Only) ════╗\n");
        var results = engine.SearchWithFilters(query, passed: false, maxResults: 15);
        DisplaySearchResults(results);
        return;
    }

    // Default: semantic search without filters
    PerformSemanticSearch(engine, input);
}

void DisplaySearchResults(List<StudentSearchApp.Models.Student> results)
{
    if (results.Count == 0)
    {
        Console.WriteLine("No students found matching your criteria.");
        return;
    }

    Console.WriteLine($"Found {results.Count} results:\n");

    for (int i = 0; i < results.Count; i++)
    {
        Console.WriteLine($"{i + 1:D2}. {results[i]}");
    }
}