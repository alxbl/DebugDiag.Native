# DebugDiag.Native

Extensions to make navigating types in a native crash dump less painful. The aim
of this project is to give a .NET like way of handling type exploration and dump
navigation and improve the experience of writing automated dump analysis for
native code.

The project is still in its early phases and as such is missing a lot of documentation.

## Getting Started

### Clone this repository

### Compile

### Configure DebugDiag

## Writing your first analysis

## Current limitations and improvements

There are a few things that have yet to be supported, and a few things that need 
to be improved. Below is a non-exhaustive list of those things:

* Add support for treating strings as primitives.
* Add support for treating bitfields using the `:` syntax as primitives.
* Add support for recursion on `dt` to improve performances (Investigate)
* Support type information for nested pointer types (i.e. `Ptr32 Ptr32 Void`)
* Add example analyses that use DebugDiag.Native and documentation on the framework.