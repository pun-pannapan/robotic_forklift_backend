using FluentValidation;
using Robotic.Forklift.Application.Dtos;

namespace Robotic.Forklift.Application.Validations
{
    public class SendCommandValidator : AbstractValidator<SendCommandRequest>
    {
        public SendCommandValidator()
        {
            RuleFor(x => x.ForkliftId).GreaterThan(0);
            RuleFor(x => x.Command).NotEmpty().MaximumLength(200);
            RuleFor(x => x.IssuedByUserId).GreaterThan(0);
        }
    }
}
