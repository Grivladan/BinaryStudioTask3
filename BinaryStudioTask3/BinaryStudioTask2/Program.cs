using System;
using System.Linq;
using System.Threading;

class Program
{

    static void Main(string[] args)
    {
        AdressBook book = new AdressBook();
        LoggerProviderFactory factory = LoggerProviderFactory.GetInstance();
        try
        {
            ILogger log = factory.GetLoggingProvider(LoggingProviders.Consol);
            book.UserAdded += log.Info;     //подписка на события
            book.UserRemoved += log.Info;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: "+e.Message);
        }
        User u1 = new User("Ivan", "Ivanov", new DateTime(1993, 1, 5), "Kyiv", "Sechenova,6", "0446785676","male","ivan93@ukr.net");
        User u2 = new User("Petro", "Petrenko", new DateTime(1995, 6, 7), "Lviv", "Franko,12", "0678945632", "male","petro95@gmail.com");
        User u3 = new User("Katerina", "Antonenko", new DateTime(1990, 5, 10), "Kyiv", "Khreshchatuk,24", "067985324", "female", "antonenko@gmail.com");
        book.AddUser(u1);
        book.AddUser(u2);
        book.AddUser(u3);
        book.AddUser(new User("Oksana","Smirnova",new DateTime(1997,5,31),"Cherkasy","Blagovisna,143","0978972356","female","OSmirnova@ukr.net"));
        book.AddUser(new User("Dmytro", "Burlak", new DateTime(1999, 12, 1), "Kyiv", "Glyshkova,10", "0993456789", "male", "Dmytro99@gmail.com"));
        book.AddUser(new User("Vasul", "Gerashchenko", new DateTime(1992, 1, 10), "Ternopil", "Shevchenka, 30", "0994567890", "male", "Vasya@mail.ru"));
        book.AddUser(new User("Taras", "Panichenko", new DateTime(1992, 1, 20), "Dnipropetrovsk", "Shevchenka, 30", null, "male", "Vasya@mail.ru"));
        Console.WriteLine();
        Console.WriteLine("Print adressbook");
        book.PrintAdressBook("F");
        Console.WriteLine("Demonstrate query execution\n");
        DemonstrateSelectionByGmail(book);
        DemonstrateUsersFromKyiv(book);
        DemonstrateSelectionFemale(book);
        DemonstrateSelectionUsersBornedInJanuary(book);
        DemonstrateSelectionDictionary(book);
        DemonstratePassingCondition(book, x => x.Gender == "male", 1, 4);
        DemonstrateCountingUsersThatHaveBirthdayToday(book, "Cherkasy");
        
        Notifier notifier = new Notifier();
        for (; ; )
        {
            Thread.Sleep(60000);
            DateTime date = DateTime.Now;
            if (date.Hour == 9 && (date.Minute >= 0 && date.Minute <= 1)) //рассылаем поздравления с 9 до 9.01
                notifier.SendEmails(book);
            Thread.Sleep(60000);
        }
    }

    public static void DemonstrateSelectionByGmail(AdressBook book)
    {
        Console.WriteLine("Select users that use domen gmail");
        var result = book.SelectByGmail();
        foreach (var item in result)
            Console.WriteLine(item.ToString("F"));
        Console.WriteLine();
    }

    public static void DemonstrateUsersFromKyiv(AdressBook book)
    {
        Console.WriteLine("Select users from Kyiv and elder than 18");
        var result = book.SelectUsersFromKyiv();
        foreach (var item in result)
            Console.WriteLine(item.ToString("F"));
        Console.WriteLine();
    }

    public static void DemonstrateSelectionFemale(AdressBook book)
    {
        Console.WriteLine("Select girls that was added less than 10 days ago");
        var result = book.SelectFemale();
        foreach (var item in result)
            Console.WriteLine(item.ToString("F"));
        Console.WriteLine();
    }

    public static void DemonstrateSelectionUsersBornedInJanuary(AdressBook book)
    {
        //отложенное выполнение запроса. Пользователь добавляется записи LiNQ запроса но попадает в ответ
        Console.WriteLine("Select users borned in January");
        var result = book.SelectUsersBornInJanuary();
        book.AddUser(new User("Karyna", "Karpenko", new DateTime(1995, 1, 19), "Rivne", "Franka, 30", "066567894", "female", "Kara@gmail.com"));
        foreach (var item in result)
            Console.WriteLine(item.ToString("F"));
        Console.WriteLine();
    }

    public static void DemonstrateSelectionDictionary(AdressBook book)
    {
        Console.WriteLine("Create dictionary and group by gender");
        var dt = book.DictionaryByGender();
        foreach (var item in dt)
            Console.WriteLine(item.Key + "\n " + string.Join("\n", from user in item.Value select user));
        Console.WriteLine();
    }

    public static void DemonstratePassingCondition(AdressBook book, Func<User, bool> condition, int first, int last)
    {
        Console.WriteLine("Pass condition and apply it");
        var res = book.ApplayExpression(x => x.Gender == "male", 1, 4);
        foreach (var item in res)
            Console.WriteLine(item.ToString("F"));
        Console.WriteLine();
    }

    public static void DemonstrateCountingUsersThatHaveBirthdayToday(AdressBook book,string city)
    {
        Console.WriteLine("Calculate users from Cherkasy that have birthday");
        Console.WriteLine(book.BirthdaysNumber("Cherkasy"));
        Console.WriteLine();
    }
}
