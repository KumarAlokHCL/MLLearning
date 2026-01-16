namespace StudentSearchApp.Models
{
    /// <summary>
    /// Represents a student record with academic information
    /// </summary>
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public char Grade { get; set; } // A, B, C, D, F

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Address: {Address}, School: {SchoolName}, Subject: {Subject}, Grade: {Grade}";
        }

        public bool IsPassed()
        {
            // Pass if grade is better than C (A or B)
            return Grade == 'A' || Grade == 'B';
        }

        public bool HasScience()
        {
            // Check if subject contains science-related keywords
            string subjectLower = Subject.ToLower();
            return subjectLower.Contains("physics") ||
                   subjectLower.Contains("chemistry") ||
                   subjectLower.Contains("biology") ||
                   subjectLower.Contains("science");
        }
    }
}
