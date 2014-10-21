// A silly project used for dump fixtures in DebugDiag.Native.Test
#include <iostream>
#include <vector>
#include <map>

struct PODType
{
    PODType() : PODType(0,0,0) 
    {
    }

    PODType(int o1, int o2, int o3) : Offset1(o1), Offset2(o2), Offset3(o3)
    {
    }

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
    VirtualType subType;
};
int main(int argc, char** argv)
{
    auto deriv = new VirtualTypeDeriv();
    deriv->OverrideMe(12); // 13

    HasAStaticField* statico = new HasAStaticField();

    // Test enumerable with POD.
    std::vector<PODType> objVector;
    objVector.push_back(PODType(1,1,1));
    objVector.push_back(PODType(2, 2, 2));
    objVector.push_back(PODType(3, 3, 3));

    /// Test enumerable with pointers.
    std::vector<PODType*> ptrVector;
    ptrVector.push_back(new PODType(1, 1, 1));
    ptrVector.push_back(new PODType(2, 2, 2));
    ptrVector.push_back(new PODType(3, 3, 3));

    // Test map enumerable with primitives
    std::map<int, int> objMap;
    objMap[1] = 2;
    objMap[2] = 4;
    objMap[3] = 6;
    objMap[4] = 8;

    int* crash = nullptr;
    std::cout << *crash; // segfault
}