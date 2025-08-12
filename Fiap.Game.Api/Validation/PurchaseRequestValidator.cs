using Fiap.Game.Api.Contracts.Request;
using FluentValidation;

namespace Fiap.Game.Api.Validation
{

    public class PurchaseRequestValidator : AbstractValidator<PurchaseRequest>
    {
        public PurchaseRequestValidator()
        {
            RuleFor(x => x.GameId).NotEmpty();
        }
    }
}
