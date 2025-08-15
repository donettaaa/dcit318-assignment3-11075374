using System;
using System.Collections.Generic;
using System.IO;

// a) Student
public class Student
{
    public int Id;
    public string FullName;
    public int Score;

    public Student(int id, string fullName, int score) => (Id, FullName, Score) = (id, fullName, score);

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score <= 79) return "B";
        if (Score >= 60 && Score <= 69) return "C";
        if (Score >= 50 && Score <= 59) return "D";
        return "F";
    }
}

// b, c) Custom exceptions
public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg) : base(msg) { } }
public class MissingFieldException : Exception { public MissingFieldException(string msg) : base(msg) { } }

// d) Processor
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var list = new List<Student>();

        using var reader = new StreamReader(inputFilePath);
        string? line;
        int lineNo = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNo++;
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length != 3)
                throw new MissingFieldException($"Line {lineNo}: expected 3 fields, got {parts.Length}");

            if (!int.TryParse(parts[0], out var id))
                throw new InvalidScoreFormatException($"Line {lineNo}: invalid ID '{parts[0]}'");

            var fullName = parts[1];

            if (!int.TryParse(parts[2], out var score))
                throw new InvalidScoreFormatException($"Line {lineNo}: invalid Score '{parts[2]}'");

            list.Add(new Student(id, fullName, score));
        }
        return list;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        foreach (var s in students)
        {
            writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        try
        {
            var processor = new StudentResultProcessor();
            var inputPath = "input.txt";
            var outputPath = "report.txt";

            var students = processor.ReadStudentsFromFile(inputPath);
            processor.WriteReportToFile(students, outputPath);

            Console.WriteLine($"Report written to {outputPath}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File missing: {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Invalid format: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing field: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex}");
        }
    }
}

