using System;
using System.IO;

namespace Lesson_9_Text_And_Files
{
    public class Contact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime Birth { get; set; }

        public Contact(string name, string phone, DateTime birth)
        {
            Name = name;
            Phone = phone;
            Birth = birth;
        }

        public int Age
        {
            get
            {
                return DateTime.Now.Year - Birth.Year;
            }
        }

        public override string ToString()
        {
            return $"Name: {Name}, Phone: {Phone}, Age: {Age}";
        }
    }

    public class ContactDatabase
    {
        public string databaseFile;
        public Contact[] contacts;

        public ContactDatabase(string file)
        {
            databaseFile = file;
            LoadContacts();
        }

        public void LoadContacts()
        {
            try
            {
                string[] records = File.ReadAllLines(databaseFile);
                contacts = ConvertStringsToContacts(records);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found. Creating a new database.");
                contacts = new Contact[0];
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                contacts = new Contact[0];
            }
        }

        public Contact[] ConvertStringsToContacts(string[] records)
        {
            var contactList = new Contact[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                try
                {
                    string[] fields = records[i].Split(',');
                    if (fields.Length != 3)
                    {
                        Console.WriteLine($"Line #{i + 1}: '{records[i]}' cannot be parsed");
                        continue;
                    }
                    string name = fields[0];
                    string phone = fields[1];
                    DateTime birth = DateTime.Parse(fields[2]);
                    contactList[i] = new Contact(name, phone, birth);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Line #{i + 1}: Error: {e.Message}");
                }
            }
            return contactList.Where(contact => contact != null).ToArray();
        }

        public void SaveContacts()
        {
            try
            {
                string[] lines = contacts.Select(contact => $"{contact.Name},{contact.Phone},{contact.Birth}").ToArray();
                File.WriteAllLines(databaseFile, lines);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Contacts are not saved : {e.Message}");
            }
        }

        public void AddContact(Contact contact)
        {
            var updatedContacts = contacts.ToList();
            updatedContacts.Add(contact);
            contacts = updatedContacts.ToArray();
        }

        public void EditContact(int index, Contact contact)
        {
            if (index >= 0 && index < contacts.Length)
            {
                contacts[index] = contact;
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
        }

        public Contact[] SearchContacts(string searchQuery)
        {
            return contacts.Where(contact =>
                contact.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                contact.Phone.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToArray();
        }

        public Contact[] GetAllContacts()
        {
            return contacts;
        }
    }

    internal class Program
    {
        static string database = "db.txt";
        static ContactDatabase contactDatabase;

        static void Main(string[] args)
        {
            try
            {
                contactDatabase = new ContactDatabase(database);

                while (true)
                {
                    UserInteraction();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} {e.StackTrace}");
            }
        }

        static void UserInteraction()
        {
            Console.WriteLine("1. Write all contacts");
            Console.WriteLine("2. Add new contact");
            Console.WriteLine("3. Edit contact");
            Console.WriteLine("4. Search by name");
            Console.WriteLine("6. Save");

            int input = int.Parse(Console.ReadLine());
            switch (input)
            {
                case 1:
                    WriteAllContactsToConsole();
                    break;
                case 2:
                    AddNewContact();
                    break;
                case 3:
                    EditContact();
                    break;
                case 4:
                    SearchContact();
                    break;
                case 6:
                    SaveContactsToFile();
                    break;
                default:
                    Console.WriteLine("No such operation.");
                    break;
            }
        }

        static void AddNewContact()
        {
            Console.WriteLine("Enter new name ");
            string name = Console.ReadLine();
            Console.WriteLine("Enter new phone ");
            string phone = Console.ReadLine();
            DateTime date = DateTime.Now;
            try
            {
                Console.WriteLine("Enter date of birth: ");
                date = DateTime.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Sorry, wrong format. Date of birth set to default value.");
            }

            Contact newContact = new Contact(name, phone, date);
            contactDatabase.AddContact(newContact);
        }

        static void EditContact()
        {
            Console.Write("Enter index of the contact to edit: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            if (index >= 0 && index < contactDatabase.GetAllContacts().Length)
            {
                Contact contact = contactDatabase.GetAllContacts()[index];
                Console.WriteLine($"Editing contact: {contact}");
                Console.Write("Enter new name: ");
                string name = Console.ReadLine();
                Console.Write("Enter new phone: ");
                string phone = Console.ReadLine();
                Console.Write("Enter new date of birth: ");
                DateTime date = DateTime.Parse(Console.ReadLine());

                Contact editedContact = new Contact(name, phone, date);
                contactDatabase.EditContact(index, editedContact);
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
        }

        static void SearchContact()
        {
            Console.Write("Enter search query: ");
            string searchQuery = Console.ReadLine().Trim();

            Contact[] searchResults = contactDatabase.SearchContacts(searchQuery);

            if (searchResults.Length == 0)
            {
                Console.WriteLine("No contacts found.");
            }
            else
            {
                Console.WriteLine("Search results:");
                for (int i = 0; i < searchResults.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {searchResults[i]}");
                }
            }
        }

        static void WriteAllContactsToConsole()
        {
            Console.WriteLine("All contacts:");
            Contact[] allContacts = contactDatabase.GetAllContacts();
            for (int i = 0; i < allContacts.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {allContacts[i]}");
            }
        }

        static void SaveContactsToFile()
        {
            contactDatabase.SaveContacts();
            Console.WriteLine("Contacts saved to file.");
        }

    }
}


