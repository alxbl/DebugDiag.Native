#pragma once
#include "../stdafx.h"

#include <string>
#include "Container.h"

// Command: RBT
// Description: Red-Black Tree traversal generic code.
class RBT : public Container
{
public:
    RBT(ExtExtension* ext, ULONG_PTR address, std::string command) 
        : Container(ext, address, command), _count(0) {}

    virtual ~RBT() {}

    void Iterate() override;
    virtual void Traverse() = 0;

protected:
    void TraverseTree(ULONG_PTR node);
    void TraverseTree(ULONG_PTR node, ULONG_PTR root);

private:
    ULONG64 _count;
};