#pragma once
#include "../stdafx.h"

#include "Container.h"

class List : public Container
{
public:
    ~List() {};
    explicit List(ExtExtension* ext, ULONG_PTR address, std::string command = "")
        : Container(ext, address, command), _count(0) {}

    void Iterate() override;
private:
    void Traverse(ULONG_PTR node, ULONG64 size);
    ULONG64 _count;
};