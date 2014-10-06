// A silly project used for dump fixtures in DebugDiag.Native.Test
#include <iostream>

struct PODType
{
    int Offset1;
    int Offset2;
    int Offset3;
};

class VirtualType
{
public:
    int POD;
    int Offset;
    int MoreOffset;
    VirtualType* Child; // A non POD offset to test nesting.
    PODType PODObject;

    VirtualType() : POD(0)
    {
    }

    virtual ~VirtualType()
    {
    }

    virtual void OverrideMe(int param)
    {
        std::cout << param << std::endl;
    }
};

class VirtualTypeDeriv : public VirtualType
{
public:
    VirtualTypeDeriv() : VirtualType()
    {

    }

    virtual void OverrideMe(int param) override
    {
        std::cout << param + 1 << std::endl;
    }
};

class HasAStaticField
{
public:
    static const int IAmSoStatic = 3;
    static int* HInstPtr;
};
int main(int argc, char** argv)
{
    auto deriv = new VirtualTypeDeriv();
    deriv->OverrideMe(12); // 13

    HasAStaticField* statico = new HasAStaticField();

    int* crash = nullptr;
    std::cout << *crash; // segfault
}