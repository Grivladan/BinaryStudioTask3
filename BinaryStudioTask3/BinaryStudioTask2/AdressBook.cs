using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

public class User
{
    public User(string FirstName, string LastName, DateTime BirthDate, string City,
        string Adress, string PhoneNumber, string Gender, string Email)
    {
        this.LastName = LastName;
        this.FirstName = FirstName;
        this.BirthDate = BirthDate;
        this.City = City;
        this.Adress = Adress;
        this.PhoneNumber = PhoneNumber;
        this.Gender = Gender;
        this.Email = Email;
    }

    public string LastName { get; set; }
    public string FirstName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime TimeAdded { get; set; }
    public string City { get; set; }
    public string Adress { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public override string ToString()
    {
        return ToString("S");
    }

    public string ToString(string fmt)
    {
        if (String.IsNullOrEmpty(fmt))
            fmt = "S";
        switch (fmt.ToUpperInvariant())
        {
            case "S":
                return string.Format("[{0}, {1}, {2}, {3}]", LastName, FirstName, PhoneNumber, Adress);
            case "F":
                string str = "";
                PropertyInfo[] infos = this.GetType().GetProperties();
                foreach (PropertyInfo pi in infos)
                {
                    str += pi.Name + " " + (pi.GetValue(this, null)!=null?pi.GetValue(this, null).ToString():"null") + "\n";
                }
                return str;
            default:
                String msg = String.Format("'{0}' is an invalid format string", fmt);
                throw new ArgumentException(msg);
        }
    }
}

public class AdressBook: IEnumerable<User>
{
    private List<User> users;
    public AdressBook()
    {
        users = new List<User>();
    }

    public IEnumerator<User> GetEnumerator(){
        return users.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private bool Contain(User user)
    {
        foreach (var item in users)
            if (item.FirstName == user.FirstName && item.LastName == user.LastName && item.Adress == user.Adress)
                return true;
        return false;
    }

    private User Find(User user)
    {
        foreach (var item in users)
            if (item.FirstName == user.FirstName && item.LastName == user.LastName && item.Adress == user.Adress)
                return item;
        return null;
    }

    public void AddUser(User user)
    {
        try
        {
            user.TimeAdded = DateTime.Now;
            if (Contain(user))
                throw new Exception("Adress book already contains this user");
            users.Add(user);
            UserAdded("User " + user.ToString() + " was added");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public void RemoveUser(User user)
    {
        try
        {
            if (!Contain(user))
                throw new Exception("Adress book doesn't contain this user");
            users.Remove(Find(user));
            UserRemoved("User " + user.ToString() + " was removed");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public void PrintAdressBook(string str)
    {
        foreach (var i in users)
            Console.WriteLine(i.ToString(str));
    }

    public delegate void SomeActionWithUser(string message);

    public event SomeActionWithUser UserAdded;
    public event SomeActionWithUser UserRemoved;

}

public static class AdressBookExtensions{
    public static IEnumerable<User> SelectByGmail(this AdressBook adressBook)
    {
        var result = adressBook.Where(x=>x.Email.Contains("gmail.com"));
        return result;
    }

    public static IEnumerable<User> SelectUsersFromKyiv(this AdressBook adressBook)
    {
        foreach (var item in adressBook)
            if (item.City == "Kyiv" && (DateTime.Today.Year - item.BirthDate.Year > 18))
                yield return item;
    }

    public static IEnumerable<User> SelectFemale(this AdressBook adressBook)
    {
        var result = from user in adressBook
                     where user.Gender == "female" && user.TimeAdded>DateTime.Now.AddDays(-10)
                     select user;
        return result;
    }

    public static IEnumerable<User> SelectUsersBornInJanuary(this AdressBook adressBook)
    {
        var result=adressBook.Select(x=>x).Where(x=>x.BirthDate.Month==1 && x.Adress!=null && x.PhoneNumber!=null )
                   .OrderByDescending(x=>x.LastName);
        return result;
    }

    public static Dictionary<string,List<User>> DictionaryByGender(this AdressBook adressBook)
    {
        var result = adressBook.ToLookup(x=>x.Gender).ToDictionary(x=>x.Key,x=>x.ToList());
        return result;
    }

    public static IEnumerable<User> ApplayExpression(this AdressBook adressBook, Func<User, bool> condition, int first, int last)
    {
        var result = adressBook.Skip(first).Take(last-first+1).Where(condition);
        return result;
    }

    public static int BirthdaysNumber(this AdressBook adressBook, string city)
    {
        int result = (from user in adressBook
                      where user.City == city && user.BirthDate.Month == DateTime.Now.Month && user.BirthDate.Day == DateTime.Now.Day
                      select user).Count();
        return result;
    }

    public static IEnumerable<User> BirthdayUser(this AdressBook adressBook)
    {
        var result = from user in adressBook
                      where user.BirthDate.Month == DateTime.Now.Month && user.BirthDate.Day == DateTime.Now.Day
                      select user;
        return result;
    }
}

