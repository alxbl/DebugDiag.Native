# DebugDiag.Native

Extensions to make navigating types in a native crash dump less painful. The aim
of this project is to give a .NET like way of handling type exploration and dump
navigation and improve the experience of writing automated dump analysis for
native code. For the initial release, only code compiled down from `C++` is 
being considered.

The project is still in its early phases and as such is missing a lot of documentation.

## Getting Started

### Pre-requisites

	* Microsoft Debug Diagnostics must be installed. It can be [downloaded from MSDN][1].
	* .NET 4.5
	* Visual Studio 2012 or more recent.
	* A C# compiler.

[1]: http://www.microsoft.com/en-us/download/details.aspx?id=42933

### Acquire and Build

The first thing you'll want to do is clone the repository. Anywhere will do. Once
you have the code, you will probably want to edit  `DebugDiag.Native.csproj` and
find the section that looks like:

    <!--======================================================-->
    <!-- This is the path to your DebugDiag installation.
         By default, that is: C:\Program Files\DebugDiag
         If you installed the x64 version. Do not change this value.-->
    <PropertyGroup>
      <DebugDiagLocation>C:\Program Files\DebugDiag</DebugDiagLocation>
    </PropertyGroup>
    <!--======================================================-->

Make sure to change this to point to the location where you installed DebugDiag.
This will ensure that all references for the projects resolve.

Do the same for `DebugDiag.Native.Test.csproj`.

Now everything should build fine.

**Note:** You may need to disable `DebugDiag.Native.Test.App` if you have an older
version of Visual Studio.

### Configure DebugDiag

**Analysis Rules**

The general idea is that DebugDiag needs to know where to find the DLLs that will
contain your Analysis Rules. There are two ways to achieve that:

    1.  Make DebugDiag aware of your rules by adding `C:\path\to\rules\`.
    This is usually your project's `bin\Debug` folder. **Recommended**

    2. Copy all of your analysis rules and DebugDiag.Native.dll to the DebugDiag
    Analysis Rule folder (`C:\Program Files\DebugDiag\AnalysisRules\`)

**Extra Symbols**

By default, DebugDiag will follow `_NT_SYMBOL_PATH` to find symbols. You can add
additional search paths in the option menu.

## References

    * [Tutorial][2]
    * API Documentation (Working on it)

[2]: /doc/intro.md

## Current limitations and improvements

There are a few things that have yet to be supported, and a few things that need 
to be improved. Below is a non-exhaustive list of those things:

* Using `NativeType.AtAddress()` with numeric types will always yield a value of `ulong.MaxValue`.
* Casting `Base -> Derived` in multiple inheritance scenarios might return trash.
* Add support for treating bitfields using the `:` syntax as primitives.
* Add support for `const` in the type parser.
* Add support for recursion on `dt` to improve performances (Investigate)
* Add example analyses that use DebugDiag.Native and documentation on the framework.