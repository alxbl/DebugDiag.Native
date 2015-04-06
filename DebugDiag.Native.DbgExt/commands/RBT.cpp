#include "RBT.h"
#include "../Memory.h"

#define Tree_Left(x) x
#define Tree_Parent(x) (x+(Memory::PtrSize*1))
#define Tree_Right(x) (x+(Memory::PtrSize*2))
#define Tree_Value(x) (x+(Memory::PtrSize==4 ? 0xc : 0x1c)) // std::tree_node have different layouts on x64 and x86...


void RBT::Execute() // override
{
    Traverse();
}
/// Traverses a std::RBT rooted at the specified node.
/// @param node is the current node to inspect
void RBT::TraverseTree(ULONG_PTR node)
{
    ULONG_PTR head = Memory::ReadPointer(Tree_Parent(node));
    if (IsVerbose()) Out("Skip=%p, Max=%p\r\n", GetSkip(), GetMax());
    if (IsVerbose()) Out("v:Head=0x%X\r\n", head);
    TraverseTree(head, node);
}

/// Traverses a std::RBT
/// @param node is the current node to inspect
/// @param root is the top level node (null) used to indicate leaf nodes.
void RBT::TraverseTree(ULONG_PTR node, ULONG_PTR root)
{
    ULONG_PTR sub_l = Memory::ReadPointer(Tree_Left(node));
    ULONG_PTR sub_r = Memory::ReadPointer(Tree_Right(node));
    ULONG_PTR value = Tree_Value(node); // Ptr to the value

    if (_max && _count > _max) return;
    // ------------------
    // In-order traversal
    // ------------------

    // Recurse on left
    if (sub_l != root) TraverseTree(sub_l, root);

    // Current node
    _count++;

    if (_max && _count > _max) return;

    if (!_skip || _count > _skip)
    {
        if (IsVerbose()) Out("v:CurrentNode(Address=0x%X, Left=0x%X, Value=0x%X, Right=0x%X)\r\n", node, sub_l, value, sub_r);
        if (_cmd.length())
        {
            char buf[18 + 1]; // "0x" + [0-9a-f]{1,16} + '\0'
            sprintf_s(buf, "0x%X", value);
            Ext()->m_Control4->SetTextReplacement("e", buf); // aS e <value>
            Ext()->m_Control->Execute(DEBUG_OUTCTL_THIS_CLIENT, _cmd.c_str(), DEBUG_EXECUTE_NOT_LOGGED);
            Ext()->m_Control4->SetTextReplacement("e", NULL); // ad e
        }
        else Out("0x%X\r\n", value);
    }
    else if (IsVerbose()) Out("v:Skipping node at 0x%X\r\n", node);

    // Recurse on right
    if (sub_r != root) TraverseTree(sub_r, root);
}
