# Blazor Svg Editor 

Blazor Svg Editor is a simple SVG editor for **Blazor** that allows annotations (in the form of SVG elements) to be placed on images. You are able to scale and translate the image (of course with the annotations). 
Actually it supports circles, rectangles and polygons - but because of the abstract interface they can be easily extended. The shapes are movable and resizable. It is also checked that the shapes are not placed outside the image.

### Demo
A demo application is hosted by GitHub-Pages. You can visit it [here](https://florian03-1.github.io/BlazorSvgEditor/).

## Documentation

### Avaiable Properties
- **CssClass/CssStyle:** Own classes and Styles for the component
- **MinScale/MaxScale**
- **ImageManipulations**: A class with properties for *brightness*, *contrast*, *saturation* and *hue* - they get applied on the loaded image
- **ImageSize**: Set image height and width
- **ImageSource:** There are  two ways:
	- **Directly**: (via Propertie ImageSource)
	- **ImageSourceLoadingFunc:** A async func which loads the image source string (e.g. when you get a base64 string from an api)
- **OnShapeChanged:** Event when Shapes get changed (with information about the ChangeType and the affected shape
- **bind-SelectedShapeId:** The id from the selected shape (can also be set from outside)

### Sample Code for Implementation
Here is a sample code for the implementation. 

    <SvgEditor @ref="svgEditor" CssClass="class" MinScale="0.8" ImageSize="(1000,750)" ImageManipulations="ImageManipulations" OnShapeChanged="EditorShapeChanged" ImageSourceLoadingFunc="GetImageSource" @bind-SelectedShapeId="SelectedShapeId">  

The used ImageLoadFunc look as follows:

    private async Task<string> GetImageSource()  
    {  
        await Task.Delay(1000);  //An async api call is also possible...
        return "https://url-to-image.de"; 
	}

For the other methods please take a look at the example project in the [GitHub](https://github.com/florian03-1/BlazorSvgEditor) Repository.

## Questions, Ideas, Feedback?

You can visit the [GitHub Repository](https://github.com/florian03-1/BlazorSvgEditor) here. Please report bugs or request new features by opening an issue. You can also make an Pull-Request when you implement a new feature.
