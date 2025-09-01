using FluentValidation;
using MediatR;
using Robotic.Forklift.Infrastructure;
using Robotic.Forklift.Application.Validations;
using Robotic.Forklift.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddForkliftInfrastructure(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<SendCommandValidator>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.EnsureCreatedAndSeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DevelopmentPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();