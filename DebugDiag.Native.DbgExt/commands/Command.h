#pragma once
#include "../stdafx.h"

class Command 
{
public:
    Command(ExtExtension* ext) :_verbose(false), _ext(ext) {}
    virtual ~Command() {};

    virtual void Execute() = 0;

    void SetVerbose(bool verbose) { _verbose = verbose; }
    bool IsVerbose() const { return _verbose; }

protected:
    void Err(PCSTR format, ...);
    void Out(PCSTR format, ...);

    ExtExtension* Ext() const { return _ext; }
private:
    bool _verbose;
    ExtExtension* _ext;
};

// Inline functions...
inline void Command::Err(PCSTR format, ...)
{
        va_list Args;

        va_start(Args, format);
        _ext->m_Control->OutputVaList(DEBUG_OUTPUT_ERROR, format, Args);
        va_end(Args);
}

inline void Command::Out(PCSTR format, ...)
{
    va_list Args;

    va_start(Args, format);
    _ext->m_Control->OutputVaList(_ext->m_OutMask, format, Args);
    va_end(Args);
}