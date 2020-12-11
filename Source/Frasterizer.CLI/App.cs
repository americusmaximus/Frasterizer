#region License
/*
MIT License

Copyright (c) 2020 Americus Maximus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using Frasterizer.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Frasterizer.CLI
{
    public static class App
    {
        static readonly Dictionary<string, string> Help = new Dictionary<string, string>()
        {
            { "config",             "A path to a JSON file with the rasterization configuration.\n                    The config file can be used to set all values, or to set\n                    base configuration while overriding some of the parameters\n                    by providing additional command line parameters."},
            { "backcolor",          "A background color of the output image. \n                    The color can be specified as a name, ARGB integer,\n                    or a HEX value.\n                    Example: \"red\", \"-65536\", \"#00ff0000\".\n                    Default value is \"Black\"."},
            { "color",              "A font color. Color can be specified as a name,\n                    ARGB integer, or a HEX value.\n                    Example: \"red\", \"-65536\", \"#00ff0000\".\n                    Default value is \"White\"."},
            { "dpi",                "A non-negative value of \"Dots per Inch\", it is used for\n                    rasterization of a font with a size specified in points.\n                    Default value is \"96\"."},
            { "fonts",              "A pipe-delimited (\"|\") list of font files to be used for\n                    rasterization. At least one file is required."},
            { "isemptyallowed",     "A boolean flag indicating whether empty and non-printable\n                    characters are allowed in the output. Examples of such\n                    characters include spaces, tabs, and others.\n                    Default value is \"TRUE\"."},
            { "ismonospace",        "A boolean flag indicating whether the characters on the\n                    output image have to take equal amount of space\n                    regardless of actual character size.\n                    Default value is \"FALSE\"." },
            { "ispoweroftwo",       "A boolean flag indicating whether the output image\n                    dimensions have to be a power of two values.\n                    Default value is \"TRUE\"." },
            { "outline",            "A configuration for the character outline, it consists of a\n                    color and a non-negative thickness value in pixels.\n                    The color and the thickness must be specified either as\n                    coma-separated values, or be specified as separate input\n                    parameters.\n                    Example: \"#00FFFFFF,1\" sets white outline of 1 (one) pixel.\n                    Please see help on specifying colors for more options.\n                    Default values are \"Maroon\" for the color and\n                    thickness of \"0\" (zero) pixels, which disables the outline."},
            { "output",             "A pipe-delimited (\"|\") pair of an output image file name\n                    and description file name that are ought to be created with\n                    the results of the rasterization. If only one file\n                    name specified it will be treated as an image file name,\n                    and the description file will be created as with the same\n                    file name with extra .json as the extension."},
            { "output.desc",        "A file name where description of a rasterization will be\n                    saved in a json format. This parameter is optional, if\n                    omitted, the description file will be saved with the same\n                    file name as image with extra .json extension."},
            { "output.image",       "A file name where description of a rasterization will be\n                    saved. Supported image types are BMP, EMF, EXIF, GIF, ICON,\n                    JPEG, PNG, TIFF, and WMF."},
            { "outline.color",      "An outline color. Color can be specified as a name,\n                    ARGB integer, or a HEX value.\n                    Example: \"red\", \"-65536\", \"#00ff0000\".\n                    Default value is \"Maroon\"."},
            { "outline.size",       "A non-negative number that sets the outline size in pixels.\n                    Default value is \"0\" (zero), which disables the outline."},
            { "margin",             "A margin value for character rasterization in pixels.\n                    Margins are non-negative values, where the format is a\n                    single number applied to left, right, top, and bottom\n                    margins. Alternatively 4 (four) coma-separated values can\n                    be provided to be applied as left, top, right, and bottom.\n                    Example: \"0\" and \"0,1,0,1\".\n                    Default value is \"0\"."},
            { "padding",            "A padding value for character rasterization in pixels.\n                    Paddings are non-negative values, where the format is a\n                    single number applied to left, right, top, and bottom\n                    paddings. Alternatively 4 (four) coma-separated values can\n                    be provided to be applied as left, top, right, and bottom.\n                    Example: \"0\" and \"0,1,0,1\".\n                    Default value is \"0\"."},
            { "scale",              "A non-negative values for vertical and horizontal scaling\n                    of rasterized characters. Values are either a single\n                    non-negative integer, or 2 (two) coma-separated\n                    non-negative percentages for height and width scaling.\n                    Example: \"100\" and \"100,100\".\n                    Default value is \"100\"."},
            { "size",               "A non-negative floating point value of a font size.\n                    Please note that size type is set by [sizetype] parameter.\n                    Default value is \"24\"."},
            { "sizetype",           "A size type of used for font rasterization. Possible values\n                    are \"pixel\" or \"point\", or alternatively \"px\" and \"pt\".\n                    Default value is \"pixel\", or \"px\"."},
            { "styletype",          "A string with requested styles. The value can be any\n                    combination of \"r\", \"b\", \"i\", \"u\", and \"s\".\n                    Where \"r\" is \"Regular\", \"b\" is \"Bold\", \"i\" is \"Italic\",\n                    \"u\" is \"Underline\", and \"s\" is \"Strikeout\".\n                    Regular font style has the lowest priority, and being\n                    omitted if any other modifier provided.\n                    Default value is \"r\"."},
            { "text",               "A path to a file with the characters to rasterize, or a\n                    string with the characters to do the same.\n                    At least one character is required." }
        };

        public static string GetHelpString(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter)) { return "No help available for <blank> parameter."; }

            if (parameter == "c") { parameter = "config"; }
            if (parameter == "t") { parameter = "text"; }

            if (Help.TryGetValue(parameter, out var result))
            {
                return result;
            }

            return string.Format("No help available for <{0}> parameter.", parameter);
        }

        public static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(string.Format("Frasterizer [Version {0}]", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            Console.WriteLine("Copyright © 2020 Americus Maximus.");
            Console.WriteLine();

            if (args == default || args.Length == 0)
            {
                Console.WriteLine("Usage: Frasterizer.CLI [options|parameters].");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine(" c=[path]|config=[path]           Rasterize font with specified configuration.");
                Console.WriteLine(" h|help                           Display help.");
                Console.WriteLine(" h=[parameter]|help=[parameter]   Display help for a specified parameter.");
                Console.WriteLine(" v|version                        Display version.");
                Console.WriteLine(" [parameters]                     Rasterize a font with specified parameters.");

                return 0;
            }

            var parameters = args.ToArray();

            // Version takes priority over anything else
            if (parameters.Any(a => a.ToLowerInvariant() == "v") || parameters.Any(a => a.ToLowerInvariant() == "version"))
            {
                Console.WriteLine(string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
				
                return 0;
            }

            // Help is a second highest priority.
            if (parameters.Any(a => a.ToLowerInvariant() == "h") || parameters.Any(a => a.ToLowerInvariant() == "help"))
            {
                Console.WriteLine("Help:");

                foreach (var x in Help.OrderBy(h => h.Key))
                {
                    Console.Write(x.Key.PadRight(20));
                    Console.WriteLine(x.Value);
                    Console.WriteLine();
                }

                return 0;
            }

            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("h=")) || parameters.Any(a => a.ToLowerInvariant().StartsWith("help=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("h=") || a.ToLowerInvariant().StartsWith("help=")).OrderBy(a => a).ToArray();

                Console.WriteLine("Help:");

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("h=")) { ars[x] = ars[x].Substring(2, ars[x].Length - 2); }
                    if (ars[x].StartsWith("help=")) { ars[x] = ars[x].Substring(5, ars[x].Length - 5); }

                    Console.Write(ars[x].PadRight(20));
                    Console.WriteLine(GetHelpString(ars[x]));
                    Console.WriteLine();
                }

                return 0;
            }

            // Load configuration file, if applicable
            var config = default(RasterizerSettings);

            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("c=")) || parameters.Any(a => a.ToLowerInvariant().StartsWith("config=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("c=") || a.ToLowerInvariant().StartsWith("config=")).OrderBy(a => a).ToArray();

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("c=")) { ars[x] = ars[x].Substring(2, ars[x].Length - 2); }
                    if (ars[x].StartsWith("config=")) { ars[x] = ars[x].Substring(7, ars[x].Length - 7); }
                }

                if (ars.Length != 1)
                {
                    var files = string.Join("\n", ars);

                    Console.WriteLine(string.Format("There can be only one configuration file specified. Found <{0}> files:\n{1}", ars.Length, files));

                    return -1;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(ars[0]))
                    {
                        Console.WriteLine("Configuration file path cannot be empty.");

                        return -1;
                    }

                    var normalizedPath = NormalizeFileName(ars[0]);

                    if (!File.Exists(normalizedPath))
                    {
                        Console.WriteLine(string.Format("Configuration file <0> not found.", normalizedPath));

                        return -1;
                    }

                    var json = File.ReadAllText(normalizedPath, Encoding.UTF8);
                    config = JsonConvert.DeserializeObject<RasterizerSettings>(json);

                    if (config == default)
                    {
                        Console.WriteLine(string.Format("Unable to deserialize the content of the configuration file <{0}>. Unknown reason.", normalizedPath));

                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to deserialize the content of the configuration file. Please see the error below.");
                    Console.WriteLine(ex.ToString());

                    return -1;
                }
            }

            // Create default configuration, if needed
            if (config == default)
            {
                config = new RasterizerSettings();

                config.RendererSettings.StyleType = FontStyleType.Regular;
                config.RendererSettings.Size = 24;
                config.RendererSettings.SizeType = FontSizeType.Pixel;

                config.RendererSettings.BackColor = Color.Black;
                config.RendererSettings.Color = Color.White;

                config.RendererSettings.DPI = 96;
                config.RendererSettings.IsEmptyAllowed = true;

                config.RendererSettings.Outline.Color = Color.Maroon;
                config.RendererSettings.Outline.Size = 0;

                config.RendererSettings.Scale.Height = 1;
                config.RendererSettings.Scale.Width = 1;

                config.ComposerSettings.IsPowerOfTwo = true;
            }

            // Process parameters
            foreach (var p in parameters)
            {
                if (!p.Contains("="))
                {
                    Console.WriteLine(string.Format("Unable to parse <{0}> parameter. Skipping it.", p));
                    continue;
                }

                var key = p.Substring(0, p.IndexOf("=")).ToLowerInvariant();

                if (key == "c" || key == "config" || key == "t" || key == "text" || key.StartsWith("output")) { continue; }

                var pms = parameters.Where(ar => ar.ToLowerInvariant().StartsWith(key + "=")).ToArray();

                if (pms.Length != 1)
                {
                    Console.WriteLine(string.Format("There can be only one parameter <{0}>", key));

                    return -1;
                }

                var value = p.Substring(key.Length + 1, p.Length - key.Length - 1);

                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine(string.Format("Empty value for <{0}> is not allowed.", key));

                    return -1;
                }

                if (key == "backcolor")
                {
                    if (TryParseColor(value, out var backColor))
                    {
                        config.RendererSettings.BackColor = backColor;
                        config.ComposerSettings.Color = backColor;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a color.", value));

                        return -1;
                    }
                }
                else if (key == "color")
                {
                    if (TryParseColor(value, out var color))
                    {
                        config.RendererSettings.Color = color;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a color.", value));

                        return -1;
                    }
                }
                else if (key == "dpi")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue <= 0)
                        {
                            Console.WriteLine(string.Format("DPI value must be a non-negative integer, current value is <{0}>.", value));

                            return -1;
                        }

                        config.RendererSettings.DPI = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));

                        return -1;
                    }
                }
                else if (key == "fonts")
                {
                    var files = value
                        .Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(f => !string.IsNullOrWhiteSpace(f))
                        .Select(f => new string[] { f, NormalizeFileName(f) })
                        .Where(f => File.Exists(f[1])).ToArray();

                    if (files.Length == 0)
                    {
                        Console.WriteLine(string.Format("At least one existing font file has to be specified. Current value is <{0}>.", value));

                        return -1;
                    }

                    config.RendererSettings.Fonts = files.Select(f => f[1]).ToArray();
                }
                else if (key == "isemptyallowed")
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        config.RendererSettings.IsEmptyAllowed = boolValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a boolean.", value));

                        return -1;
                    }
                }
                else if (key == "ismonospace")
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        config.ComposerSettings.IsMonospace = boolValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a boolean.", value));

                        return -1;
                    }
                }
                else if (key == "ispoweroftwo")
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        config.ComposerSettings.IsPowerOfTwo = boolValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a boolean.", value));

                        return -1;
                    }
                }
                else if (key == "outline")
                {
                    var parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2)
                    {
                        Console.WriteLine(string.Format("Outline format is \"color,size\", current value is <{0}>.", value));

                        return -1;
                    }

                    if (TryParseColor(parts[0], out var color))
                    {
                        config.RendererSettings.Outline.Color = color;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a color.", parts[0]));

                        return -1;
                    }

                    if (int.TryParse(parts[1], out var intValue))
                    {
                        if (intValue <= 0)
                        {
                            Console.WriteLine(string.Format("Size value must be a non-negative integer, current value is <{0}>.", parts[1]));

                            return -1;
                        }

                        config.RendererSettings.Outline.Size = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));

                        return -1;
                    }
                }
                else if (key == "outline.color")
                {
                    if (TryParseColor(value, out var color))
                    {
                        config.RendererSettings.Outline.Color = color;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a color.", value));

                        return -1;
                    }
                }
                else if (key == "outline.size")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue <= 0)
                        {
                            Console.WriteLine(string.Format("Size value must be a non-negative integer, current value is <{0}>.", value));

                            return -1;
                        }

                        config.RendererSettings.Outline.Size = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));

                        return -1;
                    }
                }
                else if (key == "margin")
                {
                    if (TryParseMargin(value, out var margin))
                    {
                        config.ComposerSettings.Margin = margin;
                    }
                    else
                    {
                        Console.Write(string.Format("Unable to parse value <{0}> as margin. The value has to be either single non-negative integer, or 4 (four) coma-separated non-negative integers", value));

                        return -1;
                    }
                }
                else if (key == "padding")
                {
                    if (TryParseMargin(value, out var padding))
                    {
                        config.RendererSettings.Padding = padding;
                    }
                    else
                    {
                        Console.Write(string.Format("Unable to parse value <{0}> as padding. The value has to be either single non-negative integer, or 4 (four) coma-separated non-negative integers", value));

                        return -1;
                    }
                }
                else if (key == "scale")
                {
                    var parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 1 && parts.Length != 2)
                    {
                        Console.Write(string.Format("Unable to parse value <{0}> as scale. The value has to be either single non-negative integer, or 2 (two) coma-separated non-negative integers", value));

                        return -1;
                    }

                    // Single value
                    if (parts.Length == 1)
                    {
                        if (float.TryParse(parts[0], out var floatValue))
                        {
                            if (floatValue <= 0)
                            {
                                Console.WriteLine(string.Format("Scale value must be a non-negative integer, current value is <{0}>.", value));

                                return -1;
                            }

                            config.RendererSettings.Scale.Height = floatValue / 100;
                            config.RendererSettings.Scale.Width = floatValue / 100;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));

                            return -1;
                        }
                    }

                    // Two values
                    var floatValues = new float[2];
                    for (var x = 0; x < parts.Length; x++)
                    {
                        if (float.TryParse(parts[0], out var floatValue))
                        {
                            if (floatValue <= 0)
                            {
                                Console.WriteLine(string.Format("Scale value must be a non-negative integer, current value is <{0}>.", value));

                                return -1;
                            }

                            floatValues[x] = floatValue;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));

                            return -1;
                        }
                    }

                    config.RendererSettings.Scale.Height = floatValues[0] / 100;
                    config.RendererSettings.Scale.Width = floatValues[1] / 100;
                }
                else if (key == "size")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue <= 0)
                        {
                            Console.WriteLine(string.Format("Size value must be a non-negative integer, current value is <{0}>.", value));

                            return -1;
                        }

                        config.RendererSettings.Size = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));

                        return -1;
                    }
                }
                else if (key == "sizetype")
                {
                    var val = value.ToLowerInvariant();

                    if (val == "pixel" || val == "px")
                    {
                        config.RendererSettings.SizeType = FontSizeType.Pixel;
                    }
                    else if (val == "point" || val == "pt")
                    {
                        config.RendererSettings.SizeType = FontSizeType.Point;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a size type. Allowed values are \"px\", \"pixel\", \"pt\", \"point\".", value));

                        return -1;
                    }
                }
                else if (key == "styletype")
                {
                    var dict = new Dictionary<char, FontStyleType>()
                    {
                        {'r', FontStyleType.Regular },
                        {'b', FontStyleType.Bold },
                        {'i', FontStyleType.Italic },
                        {'u', FontStyleType.Underline },
                        {'s', FontStyleType.Strikeout }
                    };

                    var val = value.ToLowerInvariant();

                    var styleType = FontStyleType.Regular;

                    foreach (var c in val)
                    {
                        if (!dict.TryGetValue(c, out var dictStyle))
                        {
                            Console.WriteLine(string.Format("Unable to parse <{0}> as a valid style type. Acceptable values are: \"r\", \"b\", \"i\", \"u\", and \"s\".", c));

                            return -1;
                        }

                        styleType |= dictStyle;
                    }

                    if (!val.Contains("r"))
                    {
                        styleType ^= FontStyleType.Regular;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Unknown parameter <{0}>. Skipping it.", key));
                }
            }

            // Check if fonts are present
            if (!config.RendererSettings.Fonts.Any())
            {
                Console.WriteLine("The fonts parameter is required. Please specify at least one font file.");

                return -1;
            }

            // Load unique characters
            var characters = new char[0];

            var textParams = parameters.Where(a => a.ToLowerInvariant().StartsWith("t=") || a.ToLowerInvariant().StartsWith("text=")).ToArray();

            if (textParams.Length == 0)
            {
                Console.WriteLine("The text parameter is required. Please specify a filename to a text file or a string of characters.");

                return -1;
            }

            if (textParams.Length != 1)
            {
                Console.WriteLine("There can be only one text parameter.");

                return -1;
            }

            var textParamsValue = textParams[0];

            if (textParamsValue.ToLowerInvariant().StartsWith("t=")) { textParamsValue = textParamsValue.Substring(2, textParamsValue.Length - 2); }
            if (textParamsValue.ToLowerInvariant().StartsWith("text=")) { textParamsValue = textParamsValue.Substring(5, textParamsValue.Length - 5); }

            try
            {
                var fileName = NormalizeFileName(textParamsValue);

                if (File.Exists(fileName))
                {
                    characters = File.ReadAllText(fileName, Encoding.UTF8).Distinct().OrderBy(c => c).ToArray();

                    if (characters.Length == 0)
                    {
                        Console.WriteLine(string.Format("The file <{0}> is empty. Please provide a non-empty file.", textParamsValue));

                        return -1;
                    }
                }
                else
                {
                    characters = textParamsValue.Distinct().OrderBy(c => c).ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Unable to load file <{0}>.", textParamsValue));
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Format("Treating input value <{0}> as a string.", textParamsValue));

                characters = textParamsValue.Distinct().OrderBy(c => c).ToArray();
            }

            if (characters.Length == 0)
            {
                Console.WriteLine("The text parameter is a required parameter. At least one non-space character is required.");

                return -1;
            }

            // Outputs
            var imageFileName = string.Empty;
            var imageDescritionFileName = string.Empty;

            var outputParams = parameters.Where(a => a.ToLowerInvariant().StartsWith("output")).ToArray();

            if (outputParams.Length == 0)
            {
                Console.WriteLine("The output parameter is required. Please specify a filename where the result image will be saved.");

                return -1;
            }

            foreach (var outputParam in outputParams)
            {
                var key = outputParam.Substring(0, outputParam.IndexOf("=")).ToLowerInvariant();
                var value = outputParam.Substring(outputParam.IndexOf("=") + 1, outputParam.Length - outputParam.IndexOf("=") - 1);

                if (key == "output")
                {
                    var files = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                    if (files.Length == 0)
                    {
                        Console.WriteLine(string.Format("At least one output file has to be specified. Current value is <{0}>.", value));

                        return -1;
                    }

                    if (files.Length == 1)
                    {
                        imageFileName = NormalizeFileName(files[0]);
                    }
                    else
                    {
                        imageFileName = NormalizeFileName(files[0]);
                        imageDescritionFileName = NormalizeFileName(files[1]);
                    }
                }
                else if (key == "output.desc")
                {
                    imageDescritionFileName = NormalizeFileName(value);
                }
                else if (key == "output.image")
                {
                    imageFileName = NormalizeFileName(value);
                }
                else
                {
                    Console.WriteLine(string.Format("Unknown parameter <{0}>. Skipping it.", key));
                }
            }

            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                Console.WriteLine("The output image file name is a required value. Unable to proceed without it. Please specify a file name of the resulting image.");

                return -1;
            }

            var imageFileNameDirectory = Path.GetDirectoryName(imageFileName);
            if (!Directory.Exists(imageFileNameDirectory))
            {
                Console.WriteLine(string.Format("The outut directory <{0}> does not exists.\nPlease create it before attempting to save a file into it.", imageFileNameDirectory));
                return -1;
            }

            if (string.IsNullOrWhiteSpace(imageDescritionFileName))
            {
                imageDescritionFileName = imageFileName + ".json";
            }

            // Rasterizing the font with secified parameters
            try
            {
                // Rasterize
                var result = Rasterizer.Rasterize(config, characters);

                // Save Image
                var extension = Path.GetExtension(imageFileName).ToLowerInvariant().Replace(".", string.Empty).Replace("ico", "icon").Replace("jpg", "jpeg").Replace("tif", "tiff");

                var imageFormatProperty = typeof(ImageFormat).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                                                                .FirstOrDefault(p => p.Name.ToLowerInvariant() == extension);

                if (imageFormatProperty == default)
                {
                    Console.WriteLine(string.Format("Image extension <{0}> isn't supported. Saving image as a BMP.", extension));
                }

                var imageFormat = imageFormatProperty == default ? ImageFormat.Bmp : (ImageFormat)imageFormatProperty.GetValue(default, default);

                try
                {
                    if (result == default || result.Image == default)
                    {
                        Console.WriteLine("Provided combination of paramerters produced an empty result. There's nothing to save.");

                        return -1;
                    }

                    result.Image.Save(imageFileName, imageFormat);
                    Console.WriteLine(string.Format("Image saved as <{0}>.", imageFileName));
                }
                catch (Exception iex)
                {
                    Console.WriteLine(string.Format("Unable to save image as <{0}>.", imageFileName));
                    Console.WriteLine(iex.ToString());

                    return -1;
                }

                // Save description
                try
                {
                    File.WriteAllText(imageDescritionFileName, JsonConvert.SerializeObject(result.Items.Select(i => new { Item = i.Item, Bounds = i.Bounds }).ToArray()), Encoding.UTF8);
                    Console.WriteLine(string.Format("Image description saved as <{0}>.", imageDescritionFileName));
                }
                catch (Exception iex)
                {
                    Console.WriteLine(string.Format("Unable to save image description as <{0}>.", imageDescritionFileName));
                    Console.WriteLine(iex.ToString());

                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to rasterize. Please see details below.");
                Console.WriteLine("Configuration:");
                Console.WriteLine(JsonConvert.SerializeObject(config));
                Console.WriteLine("Characters:");
                Console.WriteLine(string.Join(string.Empty, characters));
                Console.WriteLine("--------------------");
                Console.WriteLine(ex.ToString());

                return -1;
            }

            return 0;
        }

        public static string NormalizeFileName(string fileName)
        {
            return string.IsNullOrWhiteSpace(Path.GetDirectoryName(fileName)) ? Path.Combine(Environment.CurrentDirectory, fileName) : fileName;
        }

        public static bool TryParseColor(string value, out Color color)
        {
            if (value.StartsWith("#"))
            {
                // Example: #00ff0000
                if (int.TryParse(value.Substring(1, value.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var intHexValue))
                {
                    color = Color.FromArgb(intHexValue);

                    return true;
                }
            }

            // Example: -65536
            if (int.TryParse(value, out var intValue))
            {
                color = Color.FromArgb(intValue);

                return true;
            }

            // Example: Red
            var property = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(p => p.Name.ToLowerInvariant() == value);

            color = property == default ? Color.Transparent : (Color)property.GetValue(default, default);

            return property != default;
        }

        public static bool TryParseMargin(string value, out Spacing spacing)
        {
            var parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 1 && parts.Length != 4)
            {
                spacing = default;
                return false;
            }

            // Single value
            if (parts.Length == 1)
            {
                if (int.TryParse(parts[0], out var intValue))
                {
                    if (intValue < 0)
                    {
                        Console.WriteLine(string.Format("Margin and padding values must be non-negative integer, current value is <{0}>.", value));

                        spacing = default;
                        return false;
                    }

                    spacing = new Spacing()
                    {
                        Left = intValue,
                        Top = intValue,
                        Right = intValue,
                        Bottom = intValue
                    };

                    return true;
                }

                spacing = default;
                return false;
            }

            // Four values
            var intValues = new int[4];

            for (var x = 0; x < parts.Length; x++)
            {
                if (int.TryParse(parts[x], out var intValue))
                {
                    if (intValue < 0)
                    {
                        Console.WriteLine(string.Format("Margin and padding values must be non-negative integer, current value is <{0}>.", value));

                        spacing = default;
                        return false;
                    }

                    intValues[x] = intValue;
                }
                else
                {
                    Console.WriteLine(string.Format("Unable to parse value <{0}> as a integer.", value));
                    spacing = default;
                    return false;
                }
            }


            spacing = new Spacing()
            {
                Left = intValues[0],
                Top = intValues[1],
                Right = intValues[2],
                Bottom = intValues[3]
            };

            return true;
        }
    }
}
