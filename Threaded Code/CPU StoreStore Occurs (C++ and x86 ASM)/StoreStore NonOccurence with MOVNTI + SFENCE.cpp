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
	__asm movnti [X], eax
	__asm sfence
	__asm mov [Y], ebx

	return 0;
}
DWORD WINAPI Thread2(LPVOID lpParam)
{
    while (true)
    {
    	// For making clear the read isn't the 
    	// problem: excessive usage of mfence.
    	__asm mfence
    	int dY = Y;
    	__asm mfence
		int dX = X;
    	__asm mfence
    	
		if (dX == 0 && dY == 2)
    	{
			cout << "Found occurence" << endl;
			exit(1);
    	}
    	if (dX == 1 && dY == 2)
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