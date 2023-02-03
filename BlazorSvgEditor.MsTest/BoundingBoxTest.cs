using System.Diagnostics;
using BlazorSvgEditor.SvgEditor;
using BlazorSvgEditor.SvgEditor.Helper;

namespace BlazorSvgEditor.MsTest;

[TestClass]
public class BoundingBoxTest
{
    [TestInitialize]
    public void Setuo()
    {
        Console.WriteLine("Setup test enviorment...");
    }
    
    
    [TestMethod("Get Available Moving Values With (double) Coords")]
    public void GetAvailableMovingValuesWithCoordsDoubleTest()
    {
        var coord = new Coord<double>(30.15, 40.37);
        var outerBox = new BoundingBox(40.5, 60.6);
        
        var res = BoundingBox.GetAvailableMovingValues(outerBox, coord);
        
        Assert.IsTrue(res.Left.IsEqual(30.15));
        Assert.IsTrue(res.Top.IsEqual(40.37));
        Assert.IsTrue(res.Right.IsEqual(10.35));
        Assert.IsTrue(res.Bottom.IsEqual(20.23));
    }
    
    [TestMethod("Get Available Moving Values With (int) Coords")]
    public void GetAvailableMovingValuesWithCoordsIntTest()
    {
        var coord = new Coord<int>(30, 40);
        var outerBox = new BoundingBox(40.5, 60.6);
        
        var res = BoundingBox.GetAvailableMovingValues(outerBox, coord);
        
        Assert.IsTrue(res.Left.IsEqual(30));
        Assert.IsTrue(res.Top.IsEqual(40));
        Assert.IsTrue(res.Right.IsEqual(10.5));
        Assert.IsTrue(res.Bottom.IsEqual(20.6));
    }

    [TestMethod("Get Available Moving Coord With (double) Coords")]
    public void GetAvailableMoovingCoordTest()
    {
        Stopwatch sw = Stopwatch.StartNew();

        var actualCoord = new Coord<double>(30, 40);
        var moovingCoord = new Coord<double>(10.7, -20);
        var outerBox = new BoundingBox(40.5, 60.6);
        var availableMovingValues = BoundingBox.GetAvailableMovingValues(outerBox, actualCoord);
        var res = BoundingBox.GetAvailableMovingCoord(availableMovingValues, moovingCoord);
        
        Assert.IsTrue(res.X.IsEqual(10.5));
        Assert.IsTrue(res.Y.IsEqual(-20));
        
        var res2 = BoundingBox.GetAvailableMovingCoord(outerBox, actualCoord, moovingCoord);
        
        Assert.IsTrue(res2.X.IsEqual(10.5));
        Assert.IsTrue(res2.Y.IsEqual(-20));

        var res3 = BoundingBox.GetAvailableMovingCoord(outerBox, actualCoord, new Coord<double>(10.4, -40));

        Assert.IsTrue(res3.X.IsEqual(10.4));
        Assert.IsTrue(res3.Y.IsEqual(-40));
        
        sw.Stop();
        Console.WriteLine("Elapsed={0}ms ({1}µs)",sw.Elapsed.TotalMilliseconds, sw.Elapsed.TotalMicroseconds );
    }


    [TestMethod("Get Available Result Coord")]
    public void GetAvailableResultCoord()
    {
        Stopwatch sw = Stopwatch.StartNew();

        var outerBox = new BoundingBox(700, 394); //Normales Bild

        var res1 = BoundingBox.GetAvailableResultCoord(outerBox, new Coord<double>(32, 34));
        Assert.IsTrue(res1.X.IsEqual(32));
        Assert.IsTrue(res1.Y.IsEqual(34));
        
        var res2 = BoundingBox.GetAvailableResultCoord(outerBox, new Coord<double>(-32, -34));
        Assert.IsTrue(res2.X.IsEqual(0));
        Assert.IsTrue(res2.Y.IsEqual(0));
        
        var res3 = BoundingBox.GetAvailableResultCoord(outerBox, new Coord<double>(-32, 34));
        Assert.IsTrue(res3.X.IsEqual(0));
        Assert.IsTrue(res3.Y.IsEqual(34));
        
        var res4 = BoundingBox.GetAvailableResultCoord(outerBox, new Coord<double>(32, -34));
        Assert.IsTrue(res4.X.IsEqual(32));
        Assert.IsTrue(res4.Y.IsEqual(0));
        
        var res5 = BoundingBox.GetAvailableResultCoord(outerBox, new Coord<double>(-32, 0));
        Assert.IsTrue(res5.X.IsEqual(0));
        Assert.IsTrue(res5.Y.IsEqual(0));
        
        var res6 = BoundingBox.GetAvailableResultCoord(outerBox, new Coord<double>(54.334, -34.45));
        Assert.IsTrue(res6.X.IsEqual(54.334));
        Assert.IsTrue(res6.Y.IsEqual(0));

        sw.Stop();
        Console.WriteLine("Elapsed={0}ms ({1}µs)",sw.Elapsed.TotalMilliseconds, sw.Elapsed.TotalMicroseconds );

    }
    
}