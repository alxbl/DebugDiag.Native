// A silly project used for dump fixtures in DebugDiag.Native.Test
#include <iostream>
#include <vector>
#include <map>

// ------------- Define data structures used for test fixtures. -------------
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


struct Fixture
{
    VirtualTypeDeriv deriv;
    HasAStaticField staticField;
    std::vector<PODType> objVector;
    std::vector<PODType*> ptrVector;
    std::map<int, int> primitiveMap;
};

// ------------- Initialize the fixture and crash. -------------

void Crash()
{
    int* crash = nullptr;
    *crash = 42;
}

int main(int argc, char** argv)
{
    Fixture f;

    // Test enumerable with POD.
    std::vector<PODType> objVector;
    f.objVector.push_back(PODType(1,1,1));
    f.objVector.push_back(PODType(2, 2, 2));
    f.objVector.push_back(PODType(3, 3, 3));

    /// Test enumerable with pointers.
    std::vector<PODType*> ptrVector;
    f.ptrVector.push_back(new PODType(1, 1, 1));
    f.ptrVector.push_back(new PODType(2, 2, 2));
    f.ptrVector.push_back(new PODType(3, 3, 3));

    // Test map enumerable with primitives
    std::map<int, int> objMap;
    f.primitiveMap[1] = 2;
    f.primitiveMap[2] = 4;
    f.primitiveMap[3] = 6;
    f.primitiveMap[4] = 8;

    Crash();
}
