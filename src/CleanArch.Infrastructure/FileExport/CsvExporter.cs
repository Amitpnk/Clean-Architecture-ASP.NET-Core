using System.Globalization;
using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CsvHelper;

namespace CleanArch.Infrastructure.FileExport;

public class CsvExporter : ICsvExporter
{
    private static readonly CsvWriterOptions Options = new() { FormatProvider = CultureInfo.InvariantCulture };

    static CsvExporter()
    {
        // CsvHelper 34+ writes records through a CsvRecordWriter instead of reflection. Registering
        // a hand-written one here keeps CSV concerns in Infrastructure (no [CsvRecord] on the Application DTO).
        CsvRecordWriter.Register(columnPrefix => new EventExportRecordWriter(columnPrefix));
    }

    public byte[] ExportEventsToCsv(List<EventExportDto> eventExportDtos)
    {
        using var memoryStream = new MemoryStream();

        // Dispose the writer (flushing it) before reading the buffer; keep the stream open for ToArray.
        using (var csvWriter = CsvWriter.Create(memoryStream, Options, leaveOpen: true))
        {
            csvWriter.WriteRecords(eventExportDtos);
        }

        return memoryStream.ToArray();
    }

    private sealed class EventExportRecordWriter(string columnPrefix) : CsvRecordWriter<EventExportDto>(columnPrefix)
    {
        public override int FieldCount => 3;

        public override void WriteHeader(CsvWriter writer)
        {
            WriteColumnName(writer, nameof(EventExportDto.EventId));
            WriteColumnName(writer, nameof(EventExportDto.Name));
            WriteColumnName(writer, nameof(EventExportDto.Date));
        }

        public override void Write(CsvWriter writer, EventExportDto record)
        {
            writer.WriteField(record.EventId);
            writer.WriteField(record.Name);
            writer.WriteField(record.Date);
        }
    }
}
