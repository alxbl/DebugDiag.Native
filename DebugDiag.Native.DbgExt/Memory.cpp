#include "Memory.h"

// Define the proper constants for offsets
#ifdef NDBGEXT64
const size_t Memory::PtrSize = 8;
#else
const size_t Memory::PtrSize = 4;
#endif

void Memory::Read(ULONG_PTR address, PVOID lpBuffer, ULONG count, PULONG lpcbBytesRead)
{
    if (!ReadMemory(address, lpBuffer, count, lpcbBytesRead))
        dprintf("Error: Failed to read memory location 0x%X", address);
}

ULONG_PTR Memory::ReadByte(ULONG_PTR address)
{
    UCHAR byte = 0;
    ULONG size;
    Read(address, &byte, 1, &size);
    return byte;
}

ULONG_PTR Memory::ReadWord(ULONG_PTR address)
{
    UINT16 word = 0;
    ULONG size;
    Read(address, &word, 2, &size);
    return word;
}

ULONG_PTR Memory::ReadDWord(ULONG_PTR address)
{
    UINT32 dword = 0;
    ULONG size;
    Read(address, &dword, 4, &size);
    return (ULONG_PTR)dword;
}

ULONG_PTR Memory::ReadQWord(ULONG_PTR address)
{
    ULONG64 qword = 0;
    ULONG size;
    Read(address, &qword, 8, &size);
    return (ULONG_PTR)qword;
}

ULONG_PTR Memory::ReadPointer(ULONG_PTR address)
{
    return (PtrSize == sizeof(ULONG_PTR)) ?
        ReadQWord(address) // Ptr64
        : ReadDWord(address); // Ptr32
}