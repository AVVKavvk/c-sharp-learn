namespace BASICS_1
{
    class Program
    {
        

        class Person
        {
            public string Name {get; set;}
            public string Email {get; set;}
            public int Age {get; set;}
        };

        class GetSet
        {
            
            private string _name;

            public string Name
            {
                get { return _name;}
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new Exception("Name cannot be empty");
                    }
                    _name = value;
                }
            }
        };

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }

            int count = 0;

            while (true)
            {
                Console.WriteLine(count);
                count++;
                if (count >= 10)
                {
                    break;
                }
            }

            List<Person> persons = new List<Person>
            {
                new() {Name = "Vipin", Email="vipin@gmail.com", Age=22},
                new() {Name = "Avvk", Email="avvk@gmail.com", Age=22},
                new() {Name = "Kumawat", Email="kumawat@gmail.com", Age=22},
            };

            foreach (Person person in persons)
            {
                Console.WriteLine(person.Name);
                Console.WriteLine(person.Email);
                Console.WriteLine(person.Age);
                Console.WriteLine("\n");

            }



            GetSet getSetObj = new GetSet();
            getSetObj.Name = "";
            Console.WriteLine(getSetObj.Name);
            
            // getSetObj.Name = "Vipin";
            // Console.WriteLine(getSetObj.Name);

        }
    }
    
}