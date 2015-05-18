#include "stdafx.h"

#include "commands/Map.h"
#include "commands/Set.h"
#include "commands/List.h"

class EXT_CLASS : public ExtExtension
{
    public:
        EXT_COMMAND_METHOD(map);
        EXT_COMMAND_METHOD(set);
        EXT_COMMAND_METHOD(list);
};

EXT_DECLARE_GLOBALS();

// mk:@MSITStore:C:\Program%20Files%20(x86)\Windows%20Kits\8.0\Debuggers\x64\debugger.chm::/debugger/parsing_extension_arguments.htm
// !map <address> [/skip M] [/max N] [/v] [/c:"My Command"]
EXT_COMMAND(map,
    "Enumerate an std::map and optionally run a command on each element.",
    "{;e;address;The address of the map.}{skip;ed,o,d=0;skip;Skips the specified number of elements}{max;ed,o,d=0;;Maximum number of entries to analyze}{v;b,o;verbose;Enable verbosity}{;x,o;command;The command to run on each object in the map.}")
{
    ULONG_PTR address = (ULONG_PTR)GetUnnamedArgU64(0);

    RBT* cmd = nullptr;
    if (GetNumUnnamedArgs() == 3)
        cmd = new Map(this, address, std::string(GetUnnamedArgStr(2)));
    else
        cmd = new Map(this, address);

    // Configure the command
    cmd->SetSkip(GetArgU64("skip"));
    cmd->SetMax(GetArgU64("max"));
    cmd->SetVerbose(HasArg("v"));
    cmd->Execute();

    delete cmd;
}

// !set <address> [/skip M] [/max N] [/v] [/c:"My Command"]
EXT_COMMAND(set,
    "Enumerate an std::set and optionally run a command on each element.",
    "{;e;address;The address of the set.}{skip;ed,o,d=0;skip;Skips the specified number of elements}{max;ed,o,d=0;;Maximum number of entries to analyze}{v;b,o;verbose;Enable verbosity}{;x,o;command;The command to run on each object in the set.}")
{
    ULONG_PTR address = (ULONG_PTR)GetUnnamedArgU64(0);

    RBT* cmd = nullptr;
    if (GetNumUnnamedArgs() == 3)
        cmd = new Set(this, address, std::string(GetUnnamedArgStr(2)));
    else
        cmd = new Set(this, address);

    // Configure the command
    cmd->SetSkip(GetArgU64("skip"));
    cmd->SetMax(GetArgU64("max"));
    cmd->SetVerbose(HasArg("v"));
    cmd->Execute();

    delete cmd;
}

EXT_COMMAND(list,
    "Enumerates an std::list and optionally run a command on each element.",
    "{;e;address;The address of the list.}{skip;ed,o,d=0;skip;Skips the specified number of elements}{max;ed,o,d=0;;Maximum number of entries to analyze}{v;b,o;verbose;Enable verbosity}{;x,o;command;The command to run on each object in the list.}")
{
    ULONG_PTR address = (ULONG_PTR)GetUnnamedArgU64(0);

    List* cmd = nullptr;
    if (GetNumUnnamedArgs() == 3)
        cmd = new List(this, address, std::string(GetUnnamedArgStr(2)));
    else
        cmd = new List(this, address);

    cmd->SetSkip(GetArgU64("skip"));
    cmd->SetMax(GetArgU64("max"));
    cmd->SetVerbose(HasArg("v"));
    cmd->Execute();
}