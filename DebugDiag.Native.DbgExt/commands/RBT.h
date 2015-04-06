#pragma once
#include "../stdafx.h"

#include <string>
#include "Command.h"

// Command: RBT
// Description: Red-Black Tree traversal generic code.
class RBT : public Command
{
public:
    RBT(ExtExtension* ext, ULONG_PTR address, std::string command) 
    : Command(ext), _addr(address), _cmd(command) {}

    virtual ~RBT() {}

    void Execute() override;
    virtual void Traverse() = 0;

    void SetSkip(ULONG64 skip) { _skip = skip; }
    void SetMax(ULONG64 max) { _max = max; }
    void SetCommand(std::string cmd) { _cmd = cmd; }

    ULONG64 GetSkip() const { return _skip; }
    ULONG64 GetMax() const { return _max; }

    ULONG_PTR GetAddress() const { return _addr; }

protected:
    void TraverseTree(ULONG_PTR node);
    void TraverseTree(ULONG_PTR node, ULONG_PTR root);

private:
    ULONG64 _skip, _max, _count;
    ULONG_PTR _addr;
    std::string _cmd;
};