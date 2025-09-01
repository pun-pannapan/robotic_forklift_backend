using Moq;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Forklifts.Commands;
using Robotic.Forklift.Application.Interfaces;
using Robotic.Forklift.Backend.Tests;
using Robotic.Forklift.Domain.Entities;
using ForkliftEntity =  Robotic.Forklift.Domain.Entities.Forklift;

namespace Robotic.Forklift.MSTests;

[TestClass]
public class SendCommandHandlerTests
{
    [TestMethod]
    public async Task Persists_log_and_returns_actions()
    {
        using var db = TestSettings.NewDb(nameof(Persists_log_and_returns_actions));

        var forklift = new ForkliftEntity { Name = "F1", ModelNumber = "M1" };
        var user = new User { Username = "u1", PasswordHash = "x"};
        db.Forklifts.Add(forklift);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var parser = new Mock<ICommandParser>();
        parser.Setup(p => p.Parse("MOVE 5"))
              .Returns(new List<ParsedActionDto> { new("move", 5, "meters") });

        var handler = new SendCommandHandler(db, parser.Object);

        var req = new SendCommandRequest(forklift.Id, "MOVE 5", user.Id);
        var actions = await handler.Handle(new SendCommandCommand(req), default);

        Assert.AreEqual(1, actions.Count);
        Assert.AreEqual(1, db.ForkliftCommands.Count());
        Assert.AreEqual("MOVE 5", db.ForkliftCommands.Single().Command);
    }
}
