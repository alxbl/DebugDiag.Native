#pragma once

#include "RBT.h"

#include <string>

class Set : public RBT
{
public:
    Set(ExtExtension* ext, ULONG_PTR address, std::string command = "");
    void Traverse() override;
};
