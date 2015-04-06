#include "../stdafx.h"

#include <regex>

#include "Set.h"
#include "../Memory.h"

// -- Offsets --------------------------------------------------------
// Set
#define Set_Root(x) (x+(Memory::PtrSize*1))
#define Set_Size(x) (x+(Memory::PtrSize*2))
// -------------------------------------------------------------------

Set::Set(ExtExtension* ext, ULONG_PTR address, std::string command)
    : RBT(ext, address, command)
{
}

void Set::Traverse() // override
{
    ULONG_PTR size = Memory::ReadPointer(Set_Size(GetAddress()));
    Out("Size=%d\r\n", size);

    if (size == 0)
    {
        return;
    }
    else if (GetSkip() >= size)
    {
        Out("Skipped all elements.\r\n");
        return;
    }

    ULONG_PTR root = Memory::ReadPointer(Set_Root(GetAddress()));

    TraverseTree(root);
}