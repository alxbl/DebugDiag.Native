#pragma once
#include "../stdafx.h"

#include <string>

#include "Command.h"

class Container : public Command
{
public:
    explicit Container(ExtExtension* ext, ULONG_PTR address, std::string command = "");
    ~Container() override {}

    /// Iterates the container.
    virtual void Iterate() = 0;
    void Execute() override;

    // Skipping collection
    void SetSkip(ULONG64 skip) { _skip = skip; }
    void SetMax(ULONG64 max) { _max = max; }
    void SetCommand(std::string cmd) { _cmd = cmd; }

    ULONG64 GetSkip() const { return _skip; }
    ULONG64 GetMax() const { return _max; }
    std::string GetCommand() const { return _cmd; }
    ULONG_PTR GetAddress() const { return _addr; }

protected:
    void HandleElement(ULONG_PTR element);

private:
    std::string _cmd;
    ULONG64 _skip, _max;
    ULONG_PTR _addr;
};