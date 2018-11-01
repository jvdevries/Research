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
	__asm mov [Y], ebx

	return 0;
}
DWORD WINAPI Thread2(LPVOID lpParam)
{
	while (true)
	{
		int dummyY = 0;
		int dummyX = 0;
		
	    __asm mov eax, [Y];
    	__asm mfence // Ensure that Y is read before X
	    __asm mov ebx, [X];
    	__asm mov [dummyY], eax
    	__asm mov [dummyX], ebx
    	__asm nop
    	__asm nop
    	__asm nop
    	
    	if (dummyX == 1)
    		return 0;
    	if (dummyY == 2)
    	{
			cout << "Found occurence" << endl;
			exit(1);
    	}
	}
	
	return 0;
}

int main(int argc, char* argv[]) 
{
	int dummy = stoi("1000");
	int maxRuns = 1000;
	cout << "Doing " << maxRuns << " runs" <<endl;
	for (int i = 0; i < maxRuns; i++)
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