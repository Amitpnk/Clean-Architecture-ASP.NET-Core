using MapsterMapper;
using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Events.Queries.GetEventsExport;

public class GetEventsExportQueryHandler(
    IMapper mapper,
    IGenericRepositoryAsync<Event> eventRepository,
    ICsvExporter csvExporter)
    : IQueryHandler<GetEventsExportQuery, EventExportFileVm>
{
    public async Task<EventExportFileVm> Handle(GetEventsExportQuery request, CancellationToken cancellationToken)
    {
        var allEvents = mapper.Map<List<EventExportDto>>((await eventRepository.ListAllAsync()).OrderBy(x => x.Date));

        var fileData = csvExporter.ExportEventsToCsv(allEvents);

        var eventExportFileDto = new EventExportFileVm() { ContentType = "text/csv", Data = fileData, EventExportFileName = $"{Guid.NewGuid()}.csv" };

        return eventExportFileDto;
    }
}