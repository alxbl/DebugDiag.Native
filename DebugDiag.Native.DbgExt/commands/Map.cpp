#include <regex>

#include "Map.h"
#include "../Memory.h"

// -- Offsets --------------------------------------------------------
// Map
#define Map_Root(x) (x+(Memory::PtrSize*1))
#define Map_Size(x) (x+(Memory::PtrSize*2))
// Set
#define Set_Root(x) (x+(Memory::PtrSize*1))
#define Set_Size(x) (x+(Memory::PtrSize*2))
// -------------------------------------------------------------------

Map::Map(ExtExtension* ext, ULONG_PTR address, std::string command) 
    : RBT(ext, address, command)
{
}

void Map::Traverse() // override
{
    ULONG_PTR size = Memory::ReadPointer(Map_Size(GetAddress()));
    Out("Size=%d\r\n", size);

    if (size == 0)
    {
        return;
    }
    
    if (GetSkip() >= (ULONG64)size)
    {
        Out("Skipped all elements.\r\n");
        return;
    }

    ULONG_PTR root = Memory::ReadPointer(Map_Root(GetAddress()));
    
    TraverseTree(root);
}