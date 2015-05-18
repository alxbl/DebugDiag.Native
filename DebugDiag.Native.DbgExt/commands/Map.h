#pragma once
#include "../stdafx.h"

#include "RBT.h"

class Map : public RBT
{
public:
    Map(ExtExtension* ext, ULONG_PTR address, std::string command = "");
    void Traverse() override;
};
