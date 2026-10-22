# SciterSharp — Sciter.JS bindings for .NET

[![NuGet](https://img.shields.io/nuget/v/SciterSharpJS.svg)](https://www.nuget.org/packages/SciterSharpJS)
[![Publish NuGet](https://github.com/ramon-mendes/SciterSharp/actions/workflows/nuget.yml/badge.svg)](https://github.com/ramon-mendes/SciterSharp/actions/workflows/nuget.yml)
[![License: GPL-3.0](https://img.shields.io/badge/license-GPL--3.0-blue.svg)](https://github.com/ramon-mendes/SciterSharp/blob/master/LICENSE.md)

C# bindings for the [Sciter](https://sciter.com/) C/C++ API

Sciter is a small embeddable HTML/CSS/JavaScript engine, an alternative to Electron and Tauri for desktop apps. Unlike Electron, it does not bundle Chromium, and unlike Tauri, it does not depend on the system WebView: your app ships with a single native library, much smaller than a browser runtime.

## Quick start

A minimal app has 3 parts: a window, a host, and an HTML page.

**Program.cs**

```csharp
using SciterSharp;
using SciterSharp.Interop;

class Program
{
    [STAThread]
    static void Main()
    {
        // Required for drag and drop support on Windows
        if (OperatingSystem.IsWindows())
            PInvokeWindows.OleInitialize(IntPtr.Zero);

        var wnd = new SciterMixinWindow();
        wnd.CreateTopLevelWindow(new PInvokeUtils.RECT(800, 600));
        wnd.CenterTopLevelWindow();

        var host = new AppHost();
        host.SetupWindow(wnd);
        host.AttachEvh(new AppEventHandler());

        wnd.LoadPage("file://" + Path.GetFullPath("res/index.html"));
        wnd.Show();
        wnd.Title = "My App";

        PInvokeUtils.RunMsgLoop();
    }
}

class AppHost : SciterHost
{
}

class AppEventHandler : SciterEventHandler
{
    // Called from JavaScript with Window.this.xcall("Host_HelloWorld", ...)
    public bool Host_HelloWorld(SciterElement el, SciterValue[] args, out SciterValue result)
    {
        result = new SciterValue("Hello from C#");
        return true;
    }
}
```

**res/index.html**

```html
<html>
<head>
  <script>
    document.on("click", "button", () => {
      const msg = Window.this.xcall("Host_HelloWorld");
      document.$("#out").innerText = msg;
    });
  </script>
</head>
<body>
  <button>Call C#</button>
  <p id="out"></p>
</body>
</html>
```

For a complete sample, including how to pack the `res/` folder into the executable with `SciterArchive`, see [Tests/TestCoreApp](https://github.com/ramon-mendes/SciterSharp/tree/master/Tests/TestCoreApp).

## Support

I do not have much time to maintain this project. For commercial support or specific help, [contact me by email](mailto:rfmendesbrazil@gmail.com).


## Install

### NuGet

```bash
dotnet add package SciterSharpJS
```

The [`SciterSharpJS`](https://www.nuget.org/packages/SciterSharpJS) package has 2 targets:

- `net8.0` — Windows and Linux. The library picks the platform code at runtime.
- `net8.0-windows` — the same, plus the WinForms `SciterControl` and the `System.Drawing` helpers.

The package includes the Sciter native libraries under `runtimes/`:

| RID | Native library |
|---|---|
| `win-x64` | `sciter.dll` |
| `win-x86` | `sciter.dll` |
| `linux-x64` | `libsciter.so` |
| `linux-arm64` | `libsciter.so` |

`SciterSharpJS` replaces the old `SciterSharpWindows`, `SciterSharpGTK` and `SciterSharpOSX` packages, which are no longer updated.

### From source

```bash
git clone https://github.com/ramon-mendes/SciterSharp.git
```

Open `SciterSharpJS.sln` in Visual Studio 2022 (or later) and build. Then add a project reference to `SciterSharp/SciterSharp.csproj`.

## API overview

The library has 2 namespaces:

- `SciterSharp` — the public classes you use in your app.
- `SciterSharp.Interop` — P/Invoke definitions, the Sciter ABI, and low-level helpers.

Main classes and their mapping to the Sciter SDK:

| Class | Purpose |
|---|---|
| `SciterWindow` / `SciterMixinWindow` | Create a top-level, child or popup window (`SciterCreateWindow`), load HTML (`SciterLoadPage`), process window messages |
| `SciterHost` | Handle host notifications (`SciterSetCallback`), attach event handlers, register behaviors, call functions and evaluate script |
| `SciterEventHandler` | Base class for native event handlers. Public methods can be called from JavaScript with `Window.this.xcall()` |
| `SciterValue` | Sciter `VALUE` API. Convert data between C# and JavaScript (`SciterValue.FromObject`, JSON) |
| `SciterElement` | DOM element API (`HELEMENT`) |
| `SciterArchive` | Read files from a packed resource archive (`SciterOpenArchive`, `SciterGetArchiveItem`) |
| `SciterDebugOutputHandler` | Receive Sciter debug and console output |
| `SciterRequest` | Request API |
| `SciterGraphics`, `SciterImage`, `SciterPath`, `SciterText` | Graphics API (incomplete) |
| `SciterSharp.WinForms.SciterControl` | WinForms control that hosts a Sciter child window (`net8.0-windows` only) |

## License

SciterSharp is licensed under the [GNU General Public License v3.0](https://github.com/ramon-mendes/SciterSharp/blob/master/LICENSE.md).

Sciter itself is a separate product with its own license. Check the [Sciter license terms](https://sciter.com/prices/) before you ship your app.
