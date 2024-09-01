using MediatR;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderCommand: IRequest<DeleteOrderResult>
{
    public int Id { get; set; }   
}

public record DeleteOrderResult(bool IsSuccess);