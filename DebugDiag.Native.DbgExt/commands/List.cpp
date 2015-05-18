#include "List.h"
#include "../Memory.h"
// -- Offsets --------------------------------------------------------
#define List_Root(x) (x+(Memory::PtrSize))
#define List_Size(x) (x+(Memory::PtrSize*2))

#define Node_Next(x) (x)
#define Node_Prev(x) (x+(Memory::PtrSize))
#define Node_Value(x) (x+(Memory::PtrSize*2))
// -------------------------------------------------------------------
void List::Iterate()
{
    /*
0:000> dt 0044fba4+0x78 std::list<PODType,std::allocator<PODType> >
DebugDiag_Native_Test_App!std::list<PODType,std::allocator<PODType> >
+0x000 _Myproxy         : 0x0018e680 std::_Container_proxy
+0x004 _Myhead          : 0x0018e630 std::_List_node<PODType,void *>
+0x008 _Mysize          : 3

0:000> dt 0 std::_List_node<PODType,void *>
DebugDiag_Native_Test_App64!std::_List_node<PODType,void *>
+0x000 _Next            : Ptr64 std::_List_node<PODType,void *>
+0x008 _Prev            : Ptr64 std::_List_node<PODType,void *>
+0x010 _Myval           : PODType

    */
    ULONG_PTR size = Memory::ReadPointer(List_Size(GetAddress()));
    Out("Size=%d\r\n", size);
    
    if (size == 0) return;
    if (GetSkip() > (ULONG64)size)
    {
        Out("Skipped all elements.\r\n");
        return;
    }

    ULONG_PTR head = Memory::ReadPointer(List_Root(GetAddress()));

    if (IsVerbose()) Out("Skip=%p, Max=%p\r\n", GetSkip(), GetMax());
    if (IsVerbose()) Out("v:Head=0x%X\r\n", head);
    Traverse(head, size);
}

void List::Traverse(ULONG_PTR head, ULONG64 size)
{
    _count = 0; // Reset count;
    
    auto next = Memory::ReadPointer(Node_Next(head));
    while (_count < size)
    {
        if (GetMax() > 0 && _count >= GetSkip()+GetMax()) break;

        auto cur = next;
        next = Memory::ReadPointer(Node_Next(next));
        if (_count++ < GetSkip())
        {
            if (IsVerbose()) Out("v:Skipping node at 0x%X\r\n", cur);
            continue;
        }

        auto value = Node_Value(cur);
        if (IsVerbose()) Out("v:CurrentNode(Address=0x%X, Value=0x%X)\r\n", next, value);
        HandleElement(value);
    }
}