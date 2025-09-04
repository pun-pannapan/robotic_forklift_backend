using FluentValidation;
using Robotic.Forklift.Application.Forklifts.Commands;

namespace Robotic.Forklift.Application.Validations
{
    public class SendCommandCommandValidator : AbstractValidator<SendCommandCommand>
    {
        public SendCommandCommandValidator()
        {
            RuleFor(x => x.Request.ForkliftId).GreaterThan(0);
            RuleFor(x => x.Request.Command).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.IssuedByUserId).GreaterThan(0);
        }
    }
}
