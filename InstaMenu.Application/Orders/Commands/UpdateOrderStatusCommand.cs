using InstaMenu.Application.Interfaces;
using InstaMenu.Domain.Enums;
using MediatR;

namespace InstaMenu.Application.Orders.Commands
{
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }

    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IInstaMenuDbContext _context;

        public UpdateOrderStatusCommandHandler(IInstaMenuDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);

            if (order == null)
                return false;

            order.Status = request.NewStatus;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }


}
