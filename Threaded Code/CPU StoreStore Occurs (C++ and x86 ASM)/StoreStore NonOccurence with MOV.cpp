#include <atomic>
#include <iomanip>
#include <iostream>
#include <sstream>
#include <thread> 
#include <windows.h>
using namespace std;

int X, Y = 0;


DWORD WINAPI Thread1(LPVOID lpParam)
{
	__asm mov eax, 1
	__asm mov ebx, 2
	__asm mfence
	__asm mov [X], eax
	__asm mov [Y], ebx

	return 0;
}
DWORD WINAPI Thread2(LPVOID lpParam)
{
	while (true)
	{
		int dummyY = 0;
		int dummyX = 0;
		
    	// For making clear the read isn't the 
    	// problem: excessive usage of mfence.
    	__asm mfence
	    __asm mov eax, [Y];
    	__asm mfence
	    __asm mov ebx, [X];
    	__asm mfence
    	__asm mov [dummyY], eax
    	__asm mfence
    	__asm mov [dummyX], ebx
    	__asm mfence
    	
		if (dummyX == 0 && dummyY == 2)
    	{
			cout << "Found occurence" << endl;
			exit(1);
    	}
    	if (dummyX == 1 && dummyY == 2)
    		return 0;
	}
	
	return 0;
}

int main(int argc, char* argv[]) 
{
	for (int i = 0; i < 10000; i++)
	{
		X = Y = 0;
		__asm mfence // Ensure that X and Y are seen as 0 by all.

		HANDLE Array_Of_Thread_Handles[2];
		Array_Of_Thread_Handles[0] = 
			CreateThread(NULL, 0, Thread1, NULL, 0, NULL);
		Array_Of_Thread_Handles[1] = 
			CreateThread(NULL, 0, Thread2, NULL, 0, NULL);
		WaitForMultipleObjects(2, Array_Of_Thread_Handles, TRUE, INFINITE);
		CloseHandle(Array_Of_Thread_Handles[0]);
		CloseHandle(Array_Of_Thread_Handles[1]);
	}
	
	return 0;
}