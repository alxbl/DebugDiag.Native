namespace DebugDiag.Native.Test.Fixtures
{
    /// <summary>
    /// Fixture data for the 32 bit unit tests taken from a 32 bit compile dump of DebugDiag.Native.Test.App.
    /// </summary>
    public static class X86
    {
        #region VirtualTypeDeriv
        public const string VirtualTypeDerivDt = @"   +0x000 __VFN_table : Ptr32 
   +0x004 POD              : Int4B
   +0x008 Offset           : Int4B
   +0x00c MoreOffset       : Int4B
   +0x010 Child            : Ptr32 VirtualType
   +0x014 PODObject        : PODType";
        public const string VirtualTypeDerivInst = @"   +0x000 __VFN_table : 0x0114cc84 
   +0x004 POD              : 0n8
   +0x008 Offset           : 0n0
   +0x00c MoreOffset       : 0n0
   +0x010 Child            : (null) 
   +0x014 PODObject        : PODType";
        public const string VtableAddr = "0x49beb8";
        public const ulong VtableAddrULong = 0x0049beb8;

        public const string VtableLnPoi = @"(0114cc84)   DebugDiag_Native_Test_App!VirtualTypeDeriv::`vftable'   |  (0114cc90)   DebugDiag_Native_Test_App!`string'";
        #endregion

        #region MultiVtable

        public const string MultiVtableAddr = @"0x88ccff22";
        public const string MultiVtableDt = @"   +0x000 __VFN_table          : Ptr32
   +0x004 Offset1          : Int4B
   +0x008 __VFN_table          : Ptr32";
        public const string MultiVtableDtInst = @"   +0x000 __VFN_table      : 0x13141516
   +0x004 Offset1          : Int4B
   +0x008 __VFN_table          : 0x2cff32bb";
        #endregion
        #region PODType
        public const string PodTypeAddr = "0x49becc";
        public const ulong PodTypeAddrULong = 0x0049becc;
        public const string DtPodType = @"   +0x000 Offset1          : Int4B
   +0x004 Offset2          : Int4B
   +0x008 Offset3          : Int4B";
        public const string DtPodTypeInst = @"   +0x000 Offset1          : 0n42
   +0x004 Offset2          : 0n0
   +0x008 Offset3          : 0n0";

        #endregion
        
        #region PEB
        public const string PebAddr = "0x7efde000"; // dt nt!_PEB 7efde000
        public const string DtPeb = @"ntdll!_PEB
   +0x000 InheritedAddressSpace : UChar
   +0x001 ReadImageFileExecOptions : UChar
   +0x002 BeingDebugged    : UChar
   +0x003 BitField         : UChar
   +0x003 ImageUsesLargePages : Pos 0, 1 Bit
   +0x003 IsProtectedProcess : Pos 1, 1 Bit
   +0x003 IsLegacyProcess  : Pos 2, 1 Bit
   +0x003 IsImageDynamicallyRelocated : Pos 3, 1 Bit
   +0x003 SkipPatchingUser32Forwarders : Pos 4, 1 Bit
   +0x003 SpareBits        : Pos 5, 3 Bits
   +0x004 Mutant           : Ptr32 Void
   +0x008 ImageBaseAddress : Ptr32 Void
   +0x00c Ldr              : Ptr32 _PEB_LDR_DATA
   +0x010 ProcessParameters : Ptr32 _RTL_USER_PROCESS_PARAMETERS
   +0x014 SubSystemData    : Ptr32 Void
   +0x018 ProcessHeap      : Ptr32 Void
   +0x01c FastPebLock      : Ptr32 _RTL_CRITICAL_SECTION
   +0x020 AtlThunkSListPtr : Ptr32 Void
   +0x024 IFEOKey          : Ptr32 Void
   +0x028 CrossProcessFlags : Uint4B
   +0x028 ProcessInJob     : Pos 0, 1 Bit
   +0x028 ProcessInitializing : Pos 1, 1 Bit
   +0x028 ProcessUsingVEH  : Pos 2, 1 Bit
   +0x028 ProcessUsingVCH  : Pos 3, 1 Bit
   +0x028 ProcessUsingFTH  : Pos 4, 1 Bit
   +0x028 ReservedBits0    : Pos 5, 27 Bits
   +0x02c KernelCallbackTable : Ptr32 Void
   +0x02c UserSharedInfoPtr : Ptr32 Void
   +0x030 SystemReserved   : [1] Uint4B
   +0x034 AtlThunkSListPtr32 : Uint4B
   +0x038 ApiSetMap        : Ptr32 Void
   +0x03c TlsExpansionCounter : Uint4B
   +0x040 TlsBitmap        : Ptr32 Void
   +0x044 TlsBitmapBits    : [2] Uint4B
   +0x04c ReadOnlySharedMemoryBase : Ptr32 Void
   +0x050 HotpatchInformation : Ptr32 Void
   +0x054 ReadOnlyStaticServerData : Ptr32 Ptr32 Void
   +0x058 AnsiCodePageData : Ptr32 Void
   +0x05c OemCodePageData  : Ptr32 Void
   +0x060 UnicodeCaseTableData : Ptr32 Void
   +0x064 NumberOfProcessors : Uint4B
   +0x068 NtGlobalFlag     : Uint4B
   +0x070 CriticalSectionTimeout : _LARGE_INTEGER
   +0x078 HeapSegmentReserve : Uint4B
   +0x07c HeapSegmentCommit : Uint4B
   +0x080 HeapDeCommitTotalFreeThreshold : Uint4B
   +0x084 HeapDeCommitFreeBlockThreshold : Uint4B
   +0x088 NumberOfHeaps    : Uint4B
   +0x08c MaximumNumberOfHeaps : Uint4B
   +0x090 ProcessHeaps     : Ptr32 Ptr32 Void
   +0x094 GdiSharedHandleTable : Ptr32 Void
   +0x098 ProcessStarterHelper : Ptr32 Void
   +0x09c GdiDCAttributeList : Uint4B
   +0x0a0 LoaderLock       : Ptr32 _RTL_CRITICAL_SECTION
   +0x0a4 OSMajorVersion   : Uint4B
   +0x0a8 OSMinorVersion   : Uint4B
   +0x0ac OSBuildNumber    : Uint2B
   +0x0ae OSCSDVersion     : Uint2B
   +0x0b0 OSPlatformId     : Uint4B
   +0x0b4 ImageSubsystem   : Uint4B
   +0x0b8 ImageSubsystemMajorVersion : Uint4B
   +0x0bc ImageSubsystemMinorVersion : Uint4B
   +0x0c0 ActiveProcessAffinityMask : Uint4B
   +0x0c4 GdiHandleBuffer  : [34] Uint4B
   +0x14c PostProcessInitRoutine : Ptr32     void 
   +0x150 TlsExpansionBitmap : Ptr32 Void
   +0x154 TlsExpansionBitmapBits : [32] Uint4B
   +0x1d4 SessionId        : Uint4B
   +0x1d8 AppCompatFlags   : _ULARGE_INTEGER
   +0x1e0 AppCompatFlagsUser : _ULARGE_INTEGER
   +0x1e8 pShimData        : Ptr32 Void
   +0x1ec AppCompatInfo    : Ptr32 Void
   +0x1f0 CSDVersion       : _UNICODE_STRING
   +0x1f8 ActivationContextData : Ptr32 _ACTIVATION_CONTEXT_DATA
   +0x1fc ProcessAssemblyStorageMap : Ptr32 _ASSEMBLY_STORAGE_MAP
   +0x200 SystemDefaultActivationContextData : Ptr32 _ACTIVATION_CONTEXT_DATA
   +0x204 SystemAssemblyStorageMap : Ptr32 _ASSEMBLY_STORAGE_MAP
   +0x208 MinimumStackCommit : Uint4B
   +0x20c FlsCallback      : Ptr32 _FLS_CALLBACK_INFO
   +0x210 FlsListHead      : _LIST_ENTRY
   +0x218 FlsBitmap        : Ptr32 Void
   +0x21c FlsBitmapBits    : [4] Uint4B
   +0x22c FlsHighIndex     : Uint4B
   +0x230 WerRegistrationData : Ptr32 Void
   +0x234 WerShipAssertPtr : Ptr32 Void
   +0x238 pContextData     : Ptr32 Void
   +0x23c pImageHeaderHash : Ptr32 Void
   +0x240 TracingFlags     : Uint4B
   +0x240 HeapTracingEnabled : Pos 0, 1 Bit
   +0x240 CritSecTracingEnabled : Pos 1, 1 Bit
   +0x240 SpareTracingBits : Pos 2, 30 Bits";
        public const string DtPebInst =
            @"ntdll!_PEB
   +0x000 InheritedAddressSpace : 0 ''
   +0x001 ReadImageFileExecOptions : 0 ''
   +0x002 BeingDebugged    : 0x1 ''
   +0x003 BitField         : 0x8 ''
   +0x003 ImageUsesLargePages : 0y0
   +0x003 IsProtectedProcess : 0y0
   +0x003 IsLegacyProcess  : 0y0
   +0x003 IsImageDynamicallyRelocated : 0y1
   +0x003 SkipPatchingUser32Forwarders : 0y0
   +0x003 SpareBits        : 0y000
   +0x004 Mutant           : 0xffffffff Void
   +0x008 ImageBaseAddress : 0x01130000 Void
   +0x00c Ldr              : 0x77a60200 _PEB_LDR_DATA
   +0x010 ProcessParameters : 0x00493090 _RTL_USER_PROCESS_PARAMETERS
   +0x014 SubSystemData    : (null) 
   +0x018 ProcessHeap      : 0x00490000 Void
   +0x01c FastPebLock      : 0x77a62100 _RTL_CRITICAL_SECTION
   +0x020 AtlThunkSListPtr : (null) 
   +0x024 IFEOKey          : (null) 
   +0x028 CrossProcessFlags : 0
   +0x028 ProcessInJob     : 0y0
   +0x028 ProcessInitializing : 0y0
   +0x028 ProcessUsingVEH  : 0y0
   +0x028 ProcessUsingVCH  : 0y0
   +0x028 ProcessUsingFTH  : 0y0
   +0x028 ReservedBits0    : 0y000000000000000000000000000 (0)
   +0x02c KernelCallbackTable : (null) 
   +0x02c UserSharedInfoPtr : (null) 
   +0x030 SystemReserved   : [1] 0
   +0x034 AtlThunkSListPtr32 : 0
   +0x038 ApiSetMap        : 0x00040000 Void
   +0x03c TlsExpansionCounter : 0
   +0x040 TlsBitmap        : 0x77a64250 Void
   +0x044 TlsBitmapBits    : [2] 1
   +0x04c ReadOnlySharedMemoryBase : 0x7efe0000 Void
   +0x050 HotpatchInformation : (null) 
   +0x054 ReadOnlyStaticServerData : 0x7efe0a90  -> (null) 
   +0x058 AnsiCodePageData : 0x7efb0000 Void
   +0x05c OemCodePageData  : 0x7efc0228 Void
   +0x060 UnicodeCaseTableData : 0x7efd0650 Void
   +0x064 NumberOfProcessors : 8
   +0x068 NtGlobalFlag     : 0x70
   +0x070 CriticalSectionTimeout : _LARGE_INTEGER 0xffffe86d`079b8000
   +0x078 HeapSegmentReserve : 0x100000
   +0x07c HeapSegmentCommit : 0x2000
   +0x080 HeapDeCommitTotalFreeThreshold : 0x10000
   +0x084 HeapDeCommitFreeBlockThreshold : 0x1000
   +0x088 NumberOfHeaps    : 1
   +0x08c MaximumNumberOfHeaps : 0x10
   +0x090 ProcessHeaps     : 0x77a64760  -> 0x00490000 Void
   +0x094 GdiSharedHandleTable : (null) 
   +0x098 ProcessStarterHelper : (null) 
   +0x09c GdiDCAttributeList : 0
   +0x0a0 LoaderLock       : 0x77a620c0 _RTL_CRITICAL_SECTION
   +0x0a4 OSMajorVersion   : 6
   +0x0a8 OSMinorVersion   : 1
   +0x0ac OSBuildNumber    : 0x1db1
   +0x0ae OSCSDVersion     : 0x100
   +0x0b0 OSPlatformId     : 2
   +0x0b4 ImageSubsystem   : 3
   +0x0b8 ImageSubsystemMajorVersion : 6
   +0x0bc ImageSubsystemMinorVersion : 0
   +0x0c0 ActiveProcessAffinityMask : 0xff
   +0x0c4 GdiHandleBuffer  : [34] 0
   +0x14c PostProcessInitRoutine : (null) 
   +0x150 TlsExpansionBitmap : 0x77a64248 Void
   +0x154 TlsExpansionBitmapBits : [32] 1
   +0x1d4 SessionId        : 1
   +0x1d8 AppCompatFlags   : _ULARGE_INTEGER 0x0
   +0x1e0 AppCompatFlagsUser : _ULARGE_INTEGER 0x0
   +0x1e8 pShimData        : (null) 
   +0x1ec AppCompatInfo    : (null) 
   +0x1f0 CSDVersion       : _UNICODE_STRING ""Service Pack 1""
   +0x1f8 ActivationContextData : 0x00060000 _ACTIVATION_CONTEXT_DATA
   +0x1fc ProcessAssemblyStorageMap : (null) 
   +0x200 SystemDefaultActivationContextData : 0x00050000 _ACTIVATION_CONTEXT_DATA
   +0x204 SystemAssemblyStorageMap : (null) 
   +0x208 MinimumStackCommit : 0
   +0x20c FlsCallback      : 0x00498308 _FLS_CALLBACK_INFO
   +0x210 FlsListHead      : _LIST_ENTRY [ 0x4980e8 - 0x4980e8 ]
   +0x218 FlsBitmap        : 0x77a64240 Void
   +0x21c FlsBitmapBits    : [4] 3
   +0x22c FlsHighIndex     : 1
   +0x230 WerRegistrationData : (null) 
   +0x234 WerShipAssertPtr : (null) 
   +0x238 pContextData     : 0x00070000 Void
   +0x23c pImageHeaderHash : (null) 
   +0x240 TracingFlags     : 0
   +0x240 HeapTracingEnabled : 0y0
   +0x240 CritSecTracingEnabled : 0y0
   +0x240 SpareTracingBits : 0y000000000000000000000000000000 (0)";
        #endregion

        #region Invalid Type
        public const string InvalidType = "nt!InvalidDoNotExist";
        public const string InvalidTypeDt = "Symbol nt!InvalidDoNotExist not found.";
        #endregion

        #region Static Field
        public const string StaticDtAddr = "0x29cc00";
        public const string StaticDt = @"DebugDiag_Native_Test_App!HasAStaticField
   =00f1cb74 IAmSoStatic      : Int4B
   =00f00000 HInstPtr         : Ptr32 Int4B
   +0x000 subType          : VirtualType";
        public const string StaticDtInst = @"DebugDiag_Native_Test_App!HasAStaticField
   =00f1cb74 IAmSoStatic      : 0n3
   =00f00000 HInstPtr         : 0x00905a4d  -> ??
   +0x000 subType          : VirtualType";
        public const string StaticDtDrillSubType = @"   +0x000 __VFN_table : 0x00f1cc84 
   +0x004 POD              : 0n0
   +0x008 Offset           : 0n0
   +0x00c MoreOffset       : 0n0
   +0x010 Child            : (null) 
   +0x014 PODObject        : PODType";
        public const string StaticDtDrillPod = @"
   +0x000 Offset1          : 0n42
   +0x004 Offset2          : 0n0
   +0x008 Offset3          : 0n0";

        #endregion
    }
}

