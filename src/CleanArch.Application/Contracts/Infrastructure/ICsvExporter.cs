using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using System.Collections.Generic;

namespace CleanArch.Application.Contracts.Infrastructure
{
    public interface ICsvExporter
    {
        byte[] ExportEventsToCsv(List<EventExportDto> eventExportDtos);
    }
}
