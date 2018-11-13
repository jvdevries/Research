00C1102D | CC                       | int3                                            |
00C1102E | CC                       | int3                                            |
00C1102F | CC                       | int3                                            |
00C11030 | 83EC 08                  | sub esp,8                                       |
00C11033 | 53                       | push ebx                                        |
00C11034 | 0F1F40 00                | nop dword ptr ds:[eax],eax                      |
00C11038 | 0F1F8400 00000000        | nop dword ptr ds:[eax+eax],eax                  |
00C11040 | C74424 08 00000000       | mov dword ptr ss:[esp+8],0                      | dummyY = 0
00C11048 | C74424 04 00000000       | mov dword ptr ss:[esp+4],0                      | dummyX = 0
00C11050 | A1 8043C100              | mov eax,dword ptr ds:[C14380]                   | EAX = Y
00C11055 | 0FAEF0                   | mfence                                          |
00C11058 | 8B1D 7C43C100            | mov ebx,dword ptr ds:[C1437C]                   | EBX = X
00C1105E | 894424 08                | mov dword ptr ss:[esp+8],eax                    | dummyY = EAX
00C11062 | 895C24 04                | mov dword ptr ss:[esp+4],ebx                    | dummyX = EBX
00C11066 | 0FAEE8                   | lfence                                          |
00C11069 | 837C24 08 02             | cmp dword ptr ss:[esp+8],2                      | (dummyY == 2)?
00C1106E | 74 10                    | je storestore occurence with movnti.C11080      | Yes? Handle && dummyX != 1
00C11070 | 837C24 04 01             | cmp dword ptr ss:[esp+4],1                      | (dummyX == 1)?
00C11075 | 75 C9                    | jne storestore occurence with movnti.C11040     | No? Repeat loop
00C11077 | 33C0                     | xor eax,eax                                     | Yes? handle return
00C11079 | 5B                       | pop ebx                                         |
00C1107A | 83C4 08                  | add esp,8                                       |
00C1107D | C2 0400                  | ret 4                                           |
00C11080 | 837C24 04 01             | cmp dword ptr ss:[esp+4],1                      | (dummyX == 1)?
00C11085 | 74 F0                    | je storestore occurence with movnti.C11077      | Yes? handle return
00C11087 | 8B0D 5C30C100            | mov ecx,dword ptr ds:[<&?cout@std@@3V?$basic_os | No? Handle print & exit
00C1108D | 68 A013C100              | push storestore occurence with movnti.C113A0    |
00C11092 | E8 C9000000              | call storestore occurence with movnti.C11160    |
00C11097 | 8BC8                     | mov ecx,eax                                     |
00C11099 | FF15 4430C100            | call dword ptr ds:[<&??6?$basic_ostream@GU?$cha |
00C1109F | 6A 01                    | push 1                                          |
00C110A1 | FF15 BC30C100            | call dword ptr ds:[<&exit>]                     |
00C110A7 | CC                       | int3                                            |
00C110A8 | CC                       | int3                                            |
00C110A9 | CC                       | int3                                            |