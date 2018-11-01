#include <atomic>
#include <iomanip>
#include <iostream>
#include <sstream>
#include <thread> 
#include <windows.h>
using namespace std;

int X, Y, r1, r2, r3, r4 = 0;
atomic<bool> ready(false);

DWORD WINAPI Thread1(LPVOID lpParam)
{
	volatile void* location = &X;

	while (!ready.load());	// Wait for Thread 2 to start, or
							// we will end before it starts.

	__asm mov eax, 1
	__asm movnti [X], eax
	r1 = X;
	r2 = Y;

	return 0;
}
DWORD WINAPI Thread2(LPVOID lpParam)
{
	ready.store(true);

	__asm mov eax, 1
	__asm movnti [Y], eax
	r3 = Y;
	r4 = X;

	return 0;
}

string GetRunTime(int runs, clock_t start)
{
	auto end = clock();
	stringstream stream;

	if (end - start < 60000.0)
		(stream << fixed << setprecision(1) << (double(end - start) / CLOCKS_PER_SEC) << " seconds");
	else
		(stream << fixed << setprecision(2) << (double(end - start) / (60 * CLOCKS_PER_SEC)) << " minutes");

	return stream.str();
}

void OccurenceFoundExit(int runs, clock_t start)
{
	cout << "Found occurence after " << runs << " runs, which took " << GetRunTime(runs, start) << endl;
	exit(1);
}

void OccurenceNotFoundExit(int runs, clock_t start)
{
	cout << "No occurence found after " << runs << " runs, which took " << GetRunTime(runs, start) << endl;
	exit(1);
}

void PrintRunTime(int runs, clock_t start)
{
	cout << "Done " << runs << " runs, which took " << GetRunTime(runs, start) << endl;
}

int main(int argc, char* argv[]) 
{
	if (argc != 3)
	{
		cout << "Call with arguments maxRuns & printPer" << endl;
		exit(0);
	}

	int maxRuns = stoi(argv[1]);
	int printPer = stoi(argv[2]);
	
	auto start = clock();
	for (int run = 1; run <= maxRuns; run++)
	{
		X = Y = r1 = r2 = r3 = r4 = 0;
		ready = false;
		__asm mfence	// Ensures other Threads
                        // see that start is 0.


		HANDLE Array_Of_Thread_Handles[2];
		Array_Of_Thread_Handles[0] = CreateThread(NULL, 0, Thread1, NULL, 0, NULL);
		Array_Of_Thread_Handles[1] = CreateThread(NULL, 0, Thread2, NULL, 0, NULL);
		WaitForMultipleObjects(2, Array_Of_Thread_Handles, TRUE, INFINITE);
		CloseHandle(Array_Of_Thread_Handles[0]);
		CloseHandle(Array_Of_Thread_Handles[1]);

		if (r2 == 0 && r4 == 0)
			OccurenceFoundExit(run, start);

		if (run % printPer == 0)
			PrintRunTime(run, start);
	}
	OccurenceNotFoundExit(maxRuns, start);
	
	return 0;
}