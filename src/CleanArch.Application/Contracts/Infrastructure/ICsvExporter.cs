using CleanArch.Application.Features.Events.Queries.GetEventsExport;

namespace CleanArch.Application.Contracts.Infrastructure;

public interface ICsvExporter
{
    byte[] ExportEventsToCsv(List<EventExportDto> eventExportDtos);
}