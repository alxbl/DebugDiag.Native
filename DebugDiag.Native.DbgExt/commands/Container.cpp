#include "Container.h"

Container::Container(ExtExtension* ext, ULONG_PTR address, std::string command)
    : Command(ext)
    , _cmd(command)
    , _skip(0)
    , _max(0)
    , _addr(address)
{
}

void Container::Execute()
{
    Iterate();
}

void Container::HandleElement(ULONG_PTR element)
{
    if (GetCommand().length())
    {
        char buf[18 + 1]; // "0x" + [0-9a-f]{1,16} + '\0'
        sprintf_s(buf, "0x%X", element);
        Ext()->m_Control4->SetTextReplacement("e", buf); // aS e <value>
        Ext()->m_Control->Execute(DEBUG_OUTCTL_THIS_CLIENT, GetCommand().c_str(), DEBUG_EXECUTE_NOT_LOGGED);
        Ext()->m_Control4->SetTextReplacement("e", nullptr); // ad e
    }
    else Out("0x%X\r\n", element);
}