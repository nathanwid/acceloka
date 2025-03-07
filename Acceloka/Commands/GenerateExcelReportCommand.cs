using MediatR;

namespace Acceloka.Commands
{
    public record GenerateExcelReportCommand(int BookedTicketId) : IRequest<byte[]>;
}
