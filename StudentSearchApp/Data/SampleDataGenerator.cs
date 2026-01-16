using StudentSearchApp.Models;

namespace StudentSearchApp.Data
{
    /// <summary>
    /// Generates sample student data for demonstration
    /// </summary>
    public static class SampleDataGenerator
    {
        private static readonly string[] FirstNames = new[]
        {
            "Aarav", "Vivaan", "Aditya", "Arjun", "Rohan", "Ravi", "Nikhil", "Karan",
            "Rahul", "Amit", "Vikram", "Sanjay", "Deepak", "Prakash", "Suresh", "Rajesh",
            "Akshay", "Rohit", "Manish", "Naveen", "Priya", "Anjali", "Sneha", "Pooja",
            "Neha", "Divya", "Swati", "Shreya", "Kavya", "Ananya", "Diya", "Nisha",
            "Sakshi", "Riya", "Aisha", "Sophia", "Emma", "Olivia", "Ava", "Isabella",
            "Mia", "Charlotte", "Amelia", "Harper", "Evelyn", "Abigail", "Ella", "Madison",
            "John", "Michael", "David", "Robert", "James", "Richard", "Joseph", "Thomas",
            "Charles", "Daniel", "Matthew", "Mark", "Donald", "George", "Kenneth", "Steven",
            "Paul", "Andrew", "Joshua", "Edward", "Kevin", "Ronald", "George", "Anthony",
            "Frank", "Ryan", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry",
            "Justin", "Scott", "Brandon", "Benjamin", "Samuel", "Raymond", "Gregory", "Alexander",
            "Patrick", "Jack", "Dennis", "Jerry", "Tyler", "Aaron", "Jose", "Adam",
            "Henry", "Douglas", "Zachary", "Peter", "Kyle", "Walter", "Harold", "Keith",
            "Christian", "Terry", "Sean", "Gerald", "Austin", "Carl"
        };

        private static readonly string[] LastNames = new[]
        {
            "Singh", "Kumar", "Patel", "Sharma", "Gupta", "Verma", "Rao", "Nair",
            "Menon", "Iyer", "Bhat", "Reddy", "Chopra", "Bansal", "Joshi", "Mishra",
            "Pandey", "Sinha", "Trivedi", "Bhatt", "Smith", "Johnson", "Williams", "Brown",
            "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez",
            "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez",
            "Lewis", "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott",
            "Torres", "Peterson", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins",
            "Reeves", "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez",
            "Ortiz", "Morgan", "Peterson", "Cooper", "Peterson", "Gray", "Rice", "Crawford",
            "Henry", "Boyd", "Mason", "Moreno", "Kennedy", "Warren", "Dixon", "Ramos",
            "Reyes", "Burns", "Gordon", "Shaw", "Holmes", "Rice", "Robertson", "Hunt"
        };

        private static readonly string[] Schools = new[]
        {
            "Delhi Public School", "Doon School", "Mayo College", "The Cathedral School",
            "Rajkumar College", "Modern School", "Delhi Public School Vasant Kunj",
            "Sanskriti School", "Springdales School", "Salwan Public School",
            "Bluebells International School", "Amity International School",
            "Invictus School", "North Point School", "La Martinière for Girls",
            "Bishop Cotton School", "St. Stephen's School", "Sherwood College",
            "Welham Schools", "Tara Academy", "Heritage School", "Pathways School",
            "Presidium School", "Apple Core Academy", "Ryan International School",
            "Candor International School", "Chitrakoot International School",
            "Delhi Public School Sector 45", "Indian Public School", "Vidyarambha School"
        };

        private static readonly string[] Subjects = new[]
        {
            "Mathematics", "Physics", "Chemistry", "Biology", "English",
            "Hindi", "History", "Geography", "Science", "Computer Science",
            "Economics", "Political Science", "Sociology", "Psychology",
            "Art", "Physical Education", "Music", "Sanskrit", "French",
            "German", "Accountancy", "Business Studies", "Information Technology"
        };

        private static readonly string[] Cities = new[]
        {
            "Delhi", "Mumbai", "Bangalore", "Pune", "Chennai", "Kolkata",
            "Hyderabad", "Ahmedabad", "Jaipur", "Lucknow", "Indore", "Surat",
            "Chandigarh", "Vadodara", "Gurgaon", "Noida", "Faridabad", "Bhopal",
            "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia",
            "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville"
        };

        private static readonly string[] StreetNames = new[]
        {
            "Main Street", "Oak Avenue", "Maple Drive", "Pine Road", "Elm Street",
            "Cedar Lane", "Birch Court", "Willow Way", "Ash Boulevard", "Spruce Circle",
            "Market Street", "Park Avenue", "Garden Road", "Lake Drive", "Hill Street",
            "River Road", "Valley Lane", "Mountain View", "Forest Path", "Spring Lane"
        };

        public static List<Student> GenerateSampleStudents(int count = 100)
        {
            var random = new Random(42); // Fixed seed for reproducibility
            var students = new List<Student>();

            for (int i = 1; i <= count; i++)
            {
                string firstName = FirstNames[random.Next(FirstNames.Length)];
                string lastName = LastNames[random.Next(LastNames.Length)];
                string city = Cities[random.Next(Cities.Length)];
                string streetName = StreetNames[random.Next(StreetNames.Length)];
                int houseNumber = random.Next(1, 999);
                string school = Schools[random.Next(Schools.Length)];
                string subject = Subjects[random.Next(Subjects.Length)];
                
                // Grade distribution: 30% A, 40% B, 20% C, 10% D/F
                char grade = random.Next(100) switch
                {
                    < 30 => 'A',
                    < 70 => 'B',
                    < 90 => 'C',
                    < 95 => 'D',
                    _ => 'F'
                };

                students.Add(new Student
                {
                    Id = i,
                    Name = $"{firstName} {lastName}",
                    Address = $"{houseNumber} {streetName}, {city}",
                    SchoolName = school,
                    Subject = subject,
                    Grade = grade
                });
            }

            return students;
        }
    }
}
