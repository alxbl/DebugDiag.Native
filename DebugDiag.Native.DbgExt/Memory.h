#pragma once
#include "stdafx.h"

/// Memory helpers on top of DbgEng.
class Memory
{
public:
    static const size_t PtrSize;
    static ULONG_PTR ReadPointer(ULONG_PTR address);
    static ULONG_PTR ReadQWord(ULONG_PTR address);
    static ULONG_PTR ReadDWord(ULONG_PTR address);
    static ULONG_PTR ReadWord(ULONG_PTR address);
    static ULONG_PTR ReadByte(ULONG_PTR address);

    /// Generic Read for n bytes.
    /// @param address the address at which to read memory in the dump.
    /// @param lpBuffer a pointer to the buffer in which to place the read memory.
    /// @param count the number of bytes to read from the memory
    /// @param lpcbBytesRead a pointer to an unsigned long where the number of bytes read will be stored.
    static void Read(ULONG_PTR address, PVOID lpBuffer, ULONG count, PULONG lpcbBytesRead);
};