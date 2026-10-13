using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviewSamples.Modules.Variants;

public sealed class Book
{
    public string InventoryNumber { get; init; } = "";
    public string Title { get; init; } = "";
    public bool IsIssued { get; set; }
}

public sealed class Reader
{
    public int CardNumber { get; init; }
    public string Name { get; init; } = "";
}

public sealed class IssueRecord
{
    public int ReaderCardNumber { get; init; }
    public string BookInventoryNumber { get; init; } = "";
    public DateOnly DueDate { get; init; }
}

public sealed class Library
{
    private readonly List<Book> books = new();
    private readonly List<Reader> readers = new();
    private readonly List<IssueRecord> issueRecords = new();

    public string Issue(Reader? reader, Book? book, int days)
    {
        if (reader is null || book is null)
            return "Ошибка: читатель или книга не заданы.";

        if (days <= 0)
            return "Ошибка: срок выдачи должен быть больше нуля.";

        if (!readers.Contains(reader))
            return "Ошибка: читатель не найден.";

        if (!books.Contains(book))
            return "Ошибка: книга не найдена.";

        if (book.IsIssued)
            return "Ошибка: книга уже выдана.";

        book.IsIssued = true;
        issueRecords.Add(new IssueRecord
        {
            ReaderCardNumber = reader.CardNumber,
            BookInventoryNumber = book.InventoryNumber,
            DueDate = DateOnly.FromDateTime(DateTime.Today).AddDays(days)
        });

        return $"Книга «{book.Title}» выдана читателю {reader.Name}.";
    }

    public string Return(Reader? reader, Book? book)
    {
        if (reader is null || book is null)
            return "Ошибка: читатель или книга не заданы.";

        var record = issueRecords.FirstOrDefault(x =>
            x.ReaderCardNumber == reader.CardNumber &&
            x.BookInventoryNumber == book.InventoryNumber);

        if (record is null)
            return "Ошибка: эта книга не числится за данным читателем.";

        issueRecords.Remove(record);
        book.IsIssued = false;

        var overdueDays =
            DateOnly.FromDateTime(DateTime.Today).DayNumber - record.DueDate.DayNumber;

        if (overdueDays > 0)
            return $"Книга возвращена с просрочкой {overdueDays} дн.; штраф: {overdueDays * 10}.";

        return "Книга возвращена вовремя.";
    }
}
