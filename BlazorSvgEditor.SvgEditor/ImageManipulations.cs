namespace BlazorSvgEditor.SvgEditor;

public class ImageManipulations
{
    public ImageManipulations()
    {
        //https://www.w3schools.com/cssref/css3_pr_filter.php
        
        //Values in percent, 100% is no change
        Brightness = 100;
        Contrast = 100;
        Saturation = 100;
        
        //Values in degrees
        Hue = 0; //until 360
    }
    
    public int Brightness { get; set; }
    public int Contrast { get; set; }
    public int Saturation { get; set; }
    public int Hue { get; set; }
}