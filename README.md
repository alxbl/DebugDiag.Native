# DebugDiag.Native

Extensions to make navigating types in a native crash dump less painful. The aim
of this project is to give a .NET like way of handling type exploration and dump
navigation and improve the experience of writing automated dump analysis for
native code. For the initial release, only code compiled down from `C++` is 
being considered.

The project is still in its early phases and as such is missing a lot of documentation.

## Getting Started

### Clone this repository

### Compile

### Configure DebugDiag

## Writing your first analysis

## Current limitations and improvements

There are a few things that have yet to be supported, and a few things that need 
to be improved. Below is a non-exhaustive list of those things:

* Using `NativeType.AtAddress()` with numeric types will always yield a value of `ulong.MaxValue`.
* Add support for treating bitfields using the `:` syntax as primitives.
* Add support for `const` in the type parser.
* Add support for recursion on `dt` to improve performances (Investigate)
* Add example analyses that use DebugDiag.Native and documentation on the framework.