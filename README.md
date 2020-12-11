# Frasterizer
Frasterizer is a C# font to image rasterizer library with a wide range of options for user to use. Frasterizer is a library, as well as CLI and GUI interfaces for it.

Frasterizer is a library that runs on .Net Framework 4.0, 4.5, 4.7, 4.8, .Net Core 3.1, as well as .Net 5. Please see Build and Usage sections below for details.

## Example
Example of rasterized characters of 96 pixel bold Aharoni font with blue outline.

![Example of rasterization with Frasterizer](https://github.com/americusmaximus/Frasterizer/blob/main/Docs/Aharoni_96_Pixel_Bold.png)

## Build
### Windows
#### Visual Studio
Open one of the solutions and build it. Please see `<TargetFrameworks>` node in the `.csproj` files to add or remove target frameworks for the build.
#### CLI
To build the solution please use following command:

> dotnet build Frasterizer.CLI.sln --configuration Release

To build the solution for only one of the target frameworks please use the following command that shows an example of building for .Net 5.

> dotnet build Frasterizer.CLI.sln --framework net50 -- configuration Release

To publish the code you always have to specify the target framework since `dotnet` doesn't support publishing multi-target projects.

> dotnet publish Frasterizer.CLI.sln --framework net50 --configuration Release

**Note**: `dotnet` is unable to build the UI for any of the target frameworks.

### Linux
#### CLI
Please see the CLI section of building the code under Windows.
#### Dependencies
.Net on Linux depends on `libgdiplus` for font rasterization.

In case you see errors mentioning the following:

> The type initializer for 'Gdip' threw an exception.

or

> Unable to load DLL 'libgdiplus': The specified module could not be found.

you have to install libgdiplus library on your computer, which you can do by executing the following command:

> sudo apt install libgdiplus
 
## Use
### Windows
#### CLI
Frasterizer CLI on Windows 7

![Frasterizer CLI on Windows 7](https://github.com/americusmaximus/Frasterizer/blob/main/Docs/Frasterizer.CLI.Win.7.png)


Below is the output of running a help command
>Frasterizer.CLI.exe h

##### backcolor
A background color of the output image. The color can be specified as a name, ARGB integer, or a HEX value.
Example: "red", "-65536", "#00ff0000".
Default value is "Black".

##### color
A font color. Color can be specified as a name, ARGB integer, or a HEX value.
Example: "red", "-65536", "#00ff0000".
Default value is "White".

##### config
A path to a JSON file with the rasterization configuration. The config file can be used to set all values, or to set base configuration while overriding some of the parameters by providing additional command line parameters.

##### dpi
A non-negative value of "Dots per Inch", it is used for rasterization of a font with a size specified in points.
Default value is "96".

##### fonts
A pipe-delimited ("|") list of font files to be used for rasterization. At least one file is required.

##### isemptyallowed
A boolean flag indicating whether empty and non-printable characters are allowed in the output. Examples of such characters include spaces, tabs, and others.
Default value is "TRUE".

##### ismonospace
A boolean flag indicating whether the characters on the output image have to take equal amount of space regardless of actual character size.
Default value is "FALSE".

##### ispoweroftwo
A boolean flag indicating whether the output image dimensions have to be a power of two values.
Default value is "TRUE".

##### margin
A margin value for character rasterization in pixels. Margins are non-negative values, where the format is a single number applied to left, right, top, and bottom margins. Alternatively 4 (four) coma-separated values can be provided to be applied as left, top, right, and bottom.
Example: "0" and "0,1,0,1".
Default value is "0".

##### outline
A configuration for the character outline, it consists of a color and a non-negative thickness value in pixels. The color and the thickness must be specified either as coma-separated values, or be specified as separate input parameters.
Example: "#00FFFFFF,1" sets white outline of 1 (one) pixel. Please see help on specifying colors for more options.
Default values are "Maroon" for the color and thickness of "0" (zero) pixels, which disables the outline.

##### outline.color
An outline color. Color can be specified as a name, ARGB integer, or a HEX value.
Example: "red", "-65536", "#00ff0000".
Default value is "Maroon".

##### outline.size
A non-negative number that sets the outline size in pixels.
Default value is "0" (zero), which disables the outline.

##### output
A pipe-delimited ("|") pair of an output image file name and description file name that are ought to be created with the results of the rasterization. If only one file name specified it will be treated as an image file name, and the description file will be created as with the same file name with extra .json as the extension.

##### output.desc
A file name where description of a rasterization will be saved in a json format. This parameter is optional, if omitted, the description file will be saved with the same file name as image with extra .json extension.

##### output.image
A file name where description of a rasterization will be saved. Supported image types are BMP, EMF, EXIF, GIF, ICON, JPEG, PNG, TIFF, and WMF.

##### padding
A padding value for character rasterization in pixels. Paddings are non-negative values, where the format is a single number applied to left, right, top, and bottom paddings. Alternatively 4 (four) coma-separated values can be provided to be applied as left, top, right, and bottom.
Example: "0" and "0,1,0,1".
Default value is "0".

##### scale
A non-negative values for vertical and horizontal scaling of rasterized characters. Values are either a single non-negative integer, or 2 (two) coma-separated non-negative percentages for height and width scaling.
Example: "100" and "100,100".
Default value is "100".

##### size
A non-negative floating point value of a font size. Please note that size type is set by [sizetype] parameter.
Default value is "24".

##### sizetype
A size type of used for font rasterization. Possible values are "pixel" or "point", or alternatively "px" and "pt".
Default value is "pixel", or "px".

##### styletype
A string with requested styles. The value can be any combination of "r", "b", "i", "u", and "s". Where "r" is "Regular", "b" is "Bold", "i" is "Italic", "u" is "Underline", and "s" is "Strikeout". Regular font style has the lowest priority, and being omitted if any other modifier provided.
Default value is "r".

##### text
A path to a file with the characters to rasterize, or a string with the characters to do the same. At least one character is required.

**Note**: configuration file can be generated by the UI, and then manually modified in the UI or in any text editor.

#### Example
The following command line produces an image shown below.
>Frasterizer.CLI.exe text="qwertyuiop1234567890QWERTYUIOP" fonts="C:\Windows\Fonts\consola.ttf" isemptyallowed=true padding=1 margin=2 ispoweroftwo=true backcolor=transparent color=white output.image=example.png sizetype=pt outline=blue,1 size=36

![Complex example of rasterization with Frasterizer](https://github.com/americusmaximus/Frasterizer/blob/main/Docs/Consolas_36_Point_Regular.png)


#### UI
Frasterizer UI runs on Windows exclusively. It allows for easy and dynamic preview of the font rasterization, as well as testing user entered free form text.

Frasterizer on Windows 7

![Frasterizer UI on Windows 7](https://github.com/americusmaximus/Frasterizer/blob/main/Docs/Frasterizer.UI.Win.7.png)

Frasterizer on Windows 10

![Frasterizer UI on Windows 10](https://github.com/americusmaximus/Frasterizer/blob/main/Docs/Frasterizer.UI.Win.10.png)
### Linux
#### CLI
Frasterizer CLI on xUbuntu 20.04

![Frasterizer CLI on xUbuntu 20.04](https://github.com/americusmaximus/Frasterizer/blob/main/Docs/Frasterizer.CLI.xUbuntu.20.04.png)

Please see detailed description and example of the calls in Windows CLI section. Please note the differences in calling the CLI.

On Linux you have to call dotnet and provide path to the Frasterizer.CLI.dll as a first parameter, the Frasterizer parameters must follow afterward, please see example below:

>dotnet Frasterizer.CLI.dll [parameters]