using FluentValidation;

namespace Robotic.Forklift.Application.Validations
{
    public class ImportRow
    {
        public string Name { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public DateTime ManufacturingDate { get; set; }
    }

    public class ImportRowValidator : AbstractValidator<ImportRow>
    {
        public ImportRowValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ModelNumber).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ManufacturingDate).LessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}
