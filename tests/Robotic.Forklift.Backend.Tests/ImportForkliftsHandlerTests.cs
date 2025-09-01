using Robotic.Forklift.Application.Forklifts.Commands;
using Robotic.Forklift.Backend.Tests;

namespace Robotic.Forklift.MSTests;

[TestClass]
public class ImportForkliftsHandlerTests
{
    [TestMethod]
    public async Task Imports_from_csv_multiple_rows()
    {
        using var db = TestSettings.NewDb(nameof(Imports_from_csv_multiple_rows));
        var handler = new ImportForkliftsHandler(db);

        var csv =
            "Name,ModelNumber,ManufacturingDate\n" +
            "Titan Lifter,XA123,5/12/2015\n" +
            "Zephyr Mover,YB456,7/23/2018\n";

        var res = await handler.Handle(new ImportForkliftsCommand("forklifts.csv", csv), default);

        Assert.AreEqual(2, res.Inserted);
        Assert.AreEqual(0, res.Skipped);
        Assert.AreEqual(0, res.Errors.Count);
    }

    [TestMethod]
    public async Task Imports_from_json_single_ok()
    {
        using var db = TestSettings.NewDb(nameof(Imports_from_json_single_ok));
        var handler = new ImportForkliftsHandler(db);

        var json = "[{\"Name\":\"B1\",\"ModelNumber\":\"Z9\",\"ManufacturingDate\":\"2023-12-31\"}]";
        var res = await handler.Handle(new ImportForkliftsCommand("data.json", json), default);

        Assert.AreEqual(1, res.Inserted);
        Assert.AreEqual(0, res.Skipped);
        Assert.AreEqual(0, res.Errors.Count);
    }
}