0014102D | CC                       | int3                                         |
0014102E | CC                       | int3                                         |
0014102F | CC                       | int3                                         |
00141030 | 83EC 08                  | sub esp,8                                    |
00141033 | 53                       | push ebx                                     |
00141034 | 0F1F40 00                | nop dword ptr ds:[eax],eax                   |
00141038 | 0F1F8400 00000000        | nop dword ptr ds:[eax+eax],eax               |
00141040 | C74424 08 00000000       | mov dword ptr ss:[esp+8],0                   | dummyY = 0
00141048 | C74424 04 00000000       | mov dword ptr ss:[esp+4],0                   | dummyX = 0
00141050 | A1 80431400              | mov eax,dword ptr ds:[144380]                | mov eax, [Y]
00141055 | 0FAEF0                   | mfence                                       |
00141058 | 8B1D 7C431400            | mov ebx,dword ptr ds:[14437C]                | mov ebx, [X]
0014105E | 894424 08                | mov dword ptr ss:[esp+8],eax                 | mov [dummyY], eax
00141062 | 895C24 04                | mov dword ptr ss:[esp+4],ebx                 | mov [dummyX], ebx
00141066 | 837C24 04 01             | cmp dword ptr ss:[esp+4],1                   | (dummyX == 1)
0014106B | 74 1F                    | je storestore occurence with movnti.14108C   | true -> return 0
0014106D | 837C24 08 02             | cmp dword ptr ss:[esp+8],2                   | (dummyY == 2)
00141072 | 75 CC                    | jne storestore occurence with movnti.141040  | false -> restart_loop
00141074 | 8B0D 5C301400            | mov ecx,dword ptr ds:[<&?cout@std@@3V?$basic | 0014305C:&"HfFi"
0014107A | 68 60131400              | push storestore occurence with movnti.141360 |
0014107F | E8 9C000000              | call storestore occurence with movnti.141120 |
00141084 | 8BC8                     | mov ecx,eax                                  |
00141086 | FF15 44301400            | call dword ptr ds:[<&??6?$basic_ostream@GU?$ |
0014108C | 33C0                     | xor eax,eax                                  |
0014108E | 5B                       | pop ebx                                      |
0014108F | 83C4 08                  | add esp,8                                    |
00141092 | C2 0400                  | ret 4                                        |
00141095 | CC                       | int3                                         |
00141096 | CC                       | int3                                         |
00141097 | CC                       | int3                                         |