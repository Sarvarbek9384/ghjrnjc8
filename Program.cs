using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Record
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

class RecordTable
{
    private List<Record> records;

    public RecordTable()
    {
        records = new List<Record>();
    }

    public void AddRecord(Record record)
    {
        records.Add(record);
    }

    public void DisplayTable()
    {
        Console.WriteLine("Таблица результатов:");
        Console.WriteLine("Имя\tСимволов в минуту\tСимволов в секунду");

        foreach (Record record in records)
        {
            Console.WriteLine($"{record.Name}\t{record.CharactersPerMinute}\t\t\t{record.CharactersPerSecond}");
        }
    }

    public void SaveToFile(string filePath)
    {
        using (StreamWriter file = new StreamWriter(filePath))
        {
            foreach (Record record in records)
            {
                file.WriteLine($"{record.Name},{record.CharactersPerMinute},{record.CharactersPerSecond}");
            }
        }
    }

    public void LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            records.Clear();

            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] fields = line.Split(',');
                    string name = fields[0];
                    int charsPerMinute = int.Parse(fields[1]);
                    int charsPerSecond = int.Parse(fields[2]);

                    Record record = new Record
                    {
                        Name = name,
                        CharactersPerMinute = charsPerMinute,
                        CharactersPerSecond = charsPerSecond
                    };

                    records.Add(record);
                }
            }
        }
    }
}

class TypingTest
{
    private const int TestDurationSeconds = 60;
    private const string RecordsFilePath = "records.txt";

    private string inputText;
    private Stopwatch stopwatch;
    private RecordTable recordTable;

    public TypingTest()
    {
        inputText = "Это текст, который пользователь должен набрать.";
        stopwatch = new Stopwatch();
        recordTable = new RecordTable();
        recordTable.LoadFromFile(RecordsFilePath);
    }

    public void RunTest()
    {
        Console.WriteLine("Пожалуйста, введите ваше имя:");
        string name = Console.ReadLine();

        Console.WriteLine("Наберите следующий текст:\n" + inputText);
        Console.ReadLine();

        stopwatch.Start();

        Thread timerThread = new Thread(new ThreadStart(StopwatchThread));
        timerThread.Start();

        string typedText = Console.ReadLine();
        stopwatch.Stop();
        timerThread.Join();

        int charactersTyped = typedText.Length;
        double elapsedTimeMinutes = stopwatch.Elapsed.TotalMinutes;
        double elapsedTimeSeconds = stopwatch.Elapsed.TotalSeconds;

        Record record = new Record
        {
            Name = name,
            CharactersPerMinute = (int)(charactersTyped / elapsedTimeMinutes),
            CharactersPerSecond = (int)(charactersTyped / elapsedTimeSeconds)
        };

        recordTable.AddRecord(record);
        recordTable.DisplayTable();
        recordTable.SaveToFile(RecordsFilePath);
    }

    private void StopwatchThread()
    {
        Thread.Sleep(TestDurationSeconds * 1000);
        stopwatch.Stop();
    }
}

class Program
{
    static void Main(string[] args)
    {
        TypingTest typingTest = new TypingTest();
        typingTest.RunTest();
    }
}