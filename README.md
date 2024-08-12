## Cross-platform Sciter bindings for .NET

This library provides bindings of Sciter C/C++ headers to the C# language.

[Sciter](http://sciter.com/download/) is a multi-platform HTML engine. With this library you can create C#/.NET desktop applications using not just HTML, but all the features of Sciter: CSS3, SVG, scripting, AJAX, &lt;video&gt;, ... Sciter is free for commercial use. If you want to use SciterSharp in a commercial product you should acquire a commercial license.

The source is made portable to work in Windows, Linux/GTK+3/Mono and OSX/Mono.

License: **MIT**

## Comercial SUPPORT:

I don't really have time to mantain this code anymore.
But if you any specific help, reach me out **[for inquiries](mailto:r.fernandes.mendes@avanade.com)**

## NuGet

_These nugets are very outdated..._

```
Windows package:
PM> Install-Package SciterSharpWindows

Linux/GTK+3/Mono package:
PM> Install-Package SciterSharpGTK

OSX/Mono package:
PM> Install-Package SciterSharpOSX
```


## Documentation

[Introductory walk-through for writing apps with SciterSharp](http://www.codeproject.com/Articles/1057199/Sciter-HTML-Csharp-based-desktop-apps-walkthrough)

[Wednesday CODE](http://misoftware.com.br/Home/Tag/Wednesday%20CODE)

[Desktop cross-platform demo app + source-code](https://github.com/ramon-mendes/OctoDeskdexDesktop)

[Native C# API with SciterEventHandler and reflection](https://sciter.com/native-c-api-with-scitereventhandler-and-reflection/)

## Get help

[SciterSharp C# Forum](http://sciter.com/forums/forum/scitersharp-c/)

---

**Tools:**

[The Library](http://misoftware.com.br/Home/Post/TheLibrary) - free app with a collection of Sciter related helpful content (like Sciter's SDK documentation), tooling, and HTML samples

[Omni](http://misoftware.com.br/Omni) - desktop app with F12 tools (DOM inspecting and script debugging) for assisting the development of the HTML/CSS for Sciter pages

[OmniCode](http://misoftware.com.br/OmniCode) - Visual Studio extension that brings a full-featured TIScript and CSS editor for writing your HTML based code for Sciter technology

[OmniView](http://misoftware.com.br/Home/Post/OmniView) - Visual Studio extension to preview Sciter HTML content

[OmniLite](http://misoftware.com.br/Home/Post/OmniLite) - handy **signed** macOS desktop tool for previewing Sciter HTML

## Quick Start

### Sciter Bootstrap

Quick start your desktop app with our [Sciter Bootstrap](http://misoftware.com.br/Bootstrap) on-line tool.

All projects come with this library already configured and the necessary boilterplate code to create a Sciter window and load its initial HTML file.

The cross-platform template contains a solution with 3 projects, allowing you to build for **Windows** (Visual Studio), for **OSX** (Visual Studio MAC), and for **Linux** (MonoDevelop).

### From Source

Clone the repository and compile the project for your platform. In your project, add the resulting SciterSharpWindows.dll, SciterSharpGTK.dll or SciterSharpOSX.dll as a reference.

## Requirements

**Windows**

- &gt;= .NET 4.5

For running your desktop app, you need to **make sure that your program can find sciter.dll**, and make sure to use the correct version for your application, depending if it is 32bits or 64bits. So go and grab a copy from [Sciter SDK](http://sciter.com/sdk/sciter-sdk-3.zip), I don't redistribute it in any form. The best way is to put a copy of the DLL in the bin/Debug/ and bin/Release/ folders or simply add it to Windows PATH, as your prefer.

In Visual Studio, make sure to **enable native debugging** so you will see Sciter error messages in the Output window: ```Project Properties / Debug / Check 'Enable native code debugging'```
 
### Linux/Mono/GTK+3

- 64bit distro
- [Mono installation](http://www.mono-project.com/docs/getting-started/install/linux/)
- Install Sciter lib through the *install-libsciter.sh* script inside the download package

You need to add libsciter-gtk-64.so shared library to your path. The easiest way is to use the download the script from [here](https://raw.githubusercontent.com/midiway/SciterBootstrap-CSharp/TemplateMultiPlatform/install-libsciter.sh) and run it in a terminal with: ```sudo bash install-libsciter.sh```

The Sciter native library requires the following packages:

- GTK+3: ```sudo apt-get install libgtk-3-0```
- libcurl: probably already installed in your distro

### OSX/Mono

- [Mono installation](http://www.mono-project.com/docs/getting-started/install/mac/)

## WinForms and WPF

You can easily create a Sciter child window for you WinForms or WPF application. For samples, you can look at the two samples project included in this repo: Tests/TestWinForms and Tests/TestWPF.

**Tutorials:**

- WinForms: [WinForms + Sciter: embeddable HTML/CSS/TIScript control for modern UI development](https://www.codeproject.com/Articles/1162179/WinForms-plusSciter-embeddable-HTML-CSS-TIScript-c).
- WPF tutorial: [WPF + Sciter: embeddable HTML/CSS/TIScript control for modern UI development](https://www.codeproject.com/Articles/1166329/WPF-plus-Sciter-Embeddable-HTML-CSS-TIScript-Contr).


## Know Issues

Sciter have problems with video cards of old computers because the engine by default, is GPU accelerated. For me, in an old notebook I have, the execution hangs when I create the Sciter window. The solution is to switch the graphic engine to non-GPU mode, that is, CPU only. It's possible through the following Sciter API call:

```
SciterX.API.SciterSetOption(wnd._hwnd, SciterXDef.SCITER_RT_OPTIONS.SCITER_SET_GFX_LAYER,
new IntPtr(SciterXDef.GFX_LAYER.GFX_LAYER_WARP));
--or--
new IntPtr(SciterXDef.GFX_LAYER.GFX_LAYER_GDI));
```

## Classes API

The API is a complete OO wrap over Sciter C API making things more intuitive and organized.
There are 2 namespaces:

```SciterSharp``` - Public main instatiable classes.

```SciterSharp.Interop``` - Internal static classes, PInvoke definitions, Sciter ABI.

Here is a summary of the classes from SciterSharp namespace and their mapping over the official SDK C/C++ headers:

- **class SciterWindow**: API for creating a native window by PInvoke/SciterCreateWindow(), loading of HTML page via PInvoke/SciterLoadPage(), and message processing via callback for PInvoke/SciterCreateWindow()
- **abstract class SciterHost**: handling of host notifications generated by Sciter via PInvoke/SciterSetCallback(); attaching of event-handlers; registration of behaviors handlers (behavior factory); call function/eval script; and much more host related things
- **class SciterArchive**: open an archive from a binary array via PInvoke/SciterOpenArchive() and reading of file via PInvoke/SciterGetArchiveItem()
- **class SciterValue**: Sciter VALUE API for manipulating/interfacing with TIScript (JSON data)
- **class SciterElement**: Sciter DOM manipulation API (HELEMENT)
- **class SciterNode**: Sciter DOM manipulation API (HNODE)
- **abstract class SciterEventHandler**: Sciter event-handler native callback; extend this class for handling Sciter events
- **abstract class SciterDebugOutputHandler**: API for handling Sciter debug messages; note that by default Sciter output messages to stdout only if SW_ENABLE_DEBUG is set when SciterCreateWindow() is called, so this class usage might be required if you need custom handling/suppress of the output
- **class SciterGraphics**: Graphics native API - status: **INCOMPLETE**
- **class SciterImage**
- **class SciterPath**
- **class SciterText**
- **class SciterRequest**: Request native API

## Todo's (help appreciated) 

- add as C# documentation all the helpful API comments found in the official Sciter SDK C headers
