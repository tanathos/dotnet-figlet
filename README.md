# dotnet-figlet

.NET global tool to convert in a figlet representation the given text.

# Install
```bash
dotnet tool install -g dotnet-figlet
```

# Usage
```bash
figlet Lorem Ipsum Dolor
```

will produce

![](https://raw.githubusercontent.com/tanathos/dotnet-figlet/master/preview.png)

For special characters like '&' or escape sequences for options (eg. '-f', '-p', etc...) you'll need to wrap your input in double quotes, like:
```bash
figlet "Foo & Bar"
```

## Options
- -f, --font [font name]: Uses a specific figlet font to render the text. There are 148 figlet fonts embedded in the application, for a complete list check [here](http://www.figlet.org/fontdb.cgi)
- -c, --color [color name]: Renders the text in the given color. __KNOW ISSUE__: the color will change also previous rendered figlet outputs, any help on that is very welcome :)
- -p, --preview: Shows a preview for all the embedded fonts
- -o, --output [filename]: Instead of writing on the console, the output will be redirected to the given file

# Thanks to

I've just packed together the awesome work of [@tomakita](https://github.com/tomakita) ([Colorful.Console](https://github.com/tomakita/Colorful.Console)) and the guys at [figlet.org](http://www.figlet.org/).