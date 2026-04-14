#define _CRT_SECURE_NO_WARNINGS
#define _CRT_NON_CONFORMING_WCSTOK

#include "pch.h" 
#include <utility>
#include <limits.h>
#include <algorithm>
#include <string>
#include "HT.h"

using namespace std;		

namespace HT
{
	HTHandle::HTHandle() {}

	HTHandle::HTHandle(int Capacity, int SecSnapshotInterval, int MaxKeyLength, int MaxPayloadLength, const wchar_t FileName[512])
	{
		this->Capacity = Capacity;
		this->SecSnapshotInterval = SecSnapshotInterval;
		this->MaxKeyLength = MaxKeyLength;
		this->MaxPayloadLength = MaxPayloadLength;
		this->ElementCount = 0;
		memcpy(this->FileName, FileName, sizeof(this->FileName));
	}


	Element::Element()
	{
		this->key = NULL;
		this->keylength = NULL;
		this->payload = NULL;
		this->payloadlength = NULL;
	}

	Element::Element(const void* key, int keylength)
	{
		Element();
		this->keylength = keylength;
		this->key = (void*)key;
	}

	Element::Element(const void* key, int keylength, const void* payload, int  payloadlength)
	{
		this->key = (void*)key;
		this->keylength = keylength;
		this->payload = (void*)payload;
		this->payloadlength = payloadlength;
	}

	Element::Element(Element* oldelement, const void* newpayload, int newpayloadlength)
	{
		this->key = oldelement->key;
		this->keylength = oldelement->keylength;
		this->payload = (void*)newpayload;
		this->payloadlength = newpayloadlength;
	}

	HTHandle* Create
	(
		int	  Capacity,
		int   SecSnapshotInterval,
		int   MaxKeyLength,
		int   MaxPayloadLength,
		const wchar_t FileName[512]
	)
	{
		HTHandle* ht = NULL;

		HANDLE hFile = NULL;
		HANDLE hMap = NULL;
		HANDLE hMutex = NULL;
		LPVOID lpFileMap = NULL;
		HANDLE hSnaphot = NULL;
		DWORD SnapshotThread = NULL;

		hFile = CreateFile((LPCWSTR)FileName, GENERIC_READ | GENERIC_WRITE, NULL, NULL, CREATE_NEW, FILE_ATTRIBUTE_NORMAL, NULL);
		if (!hFile)
		{
			throw "create file error";
		}

		long size = sizeof(HTHandle) + (MaxKeyLength + MaxPayloadLength) * Capacity;

		hMap = CreateFileMapping(hFile, NULL, PAGE_READWRITE, 0, size, L"mapping");
		if (!hMap)
		{
			throw "create file mapping error";
		}

		lpFileMap = MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		if (!lpFileMap)
		{
			throw "create file mapping view error";
		}

		ZeroMemory(lpFileMap, size);

		hMutex = CreateMutex(NULL, FALSE, (LPCWSTR)FileName);
		if (!hMutex)
		{
			throw "create mutex error";
		}

		hSnaphot = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)SnapshotRoutine, ht, 0, &SnapshotThread);
		if (!hSnaphot)
		{
			return NULL;
		}

		ht = new(lpFileMap) HTHandle(Capacity, SecSnapshotInterval, MaxKeyLength, MaxPayloadLength, FileName);
		ht->File = hFile;
		ht->FileMapping = hMap;
		ht->Addr = lpFileMap;
		ht->Mutex = hMutex;
		ht->SnapshotThread;

		return ht;
	}

	HTHandle* OpenExist(const wchar_t FileName[512])
	{
		HANDLE fileHandle = NULL;
		HANDLE fileMappingHandle = NULL;
		HANDLE hm = NULL;
		LPVOID lp = NULL;
		HANDLE mutexHandle = NULL;

		fileHandle = CreateFile(FileName, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
			NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		if (fileHandle == NULL || fileHandle == INVALID_HANDLE_VALUE)
		{
			throw("Отсутствует файл");
		}

		HANDLE mutex = CreateMutex(NULL, FALSE, (LPCWSTR)FileName);

		hm = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, L"mapping");
		if (!hm)
		{
			throw("Невозможно открыть");
		}

		lp = MapViewOfFile(hm, FILE_MAP_ALL_ACCESS | FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 0);

		if (!lp)
		{
			throw("Map ):");
		}

		HTHandle* ht = (HTHandle*)lp;
		ht->FileMapping = hm;
		ht->Addr = lp;
		ht->Mutex = mutex;
		return ht;
	}

	HTHandle* Open(const wchar_t FileName[512])
	{
		HANDLE hFile = NULL;
		HANDLE hMap = NULL;
		HANDLE hMutex = NULL;
		LPVOID lpFileMap = NULL;
		HANDLE hSnapshot = NULL;
		DWORD SnapshotThreadId = NULL;

		hFile = CreateFile(
			(LPCWSTR)FileName,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
			NULL,
			OPEN_EXISTING,
			FILE_ATTRIBUTE_NORMAL,
			NULL
		);
		if (!hFile || hFile == INVALID_HANDLE_VALUE)
		{
			throw "create file error";
		}

		hMap = CreateFileMapping(
			hFile,
			NULL,
			PAGE_READWRITE,
			0,
			0,
			L"mapping"
		);
		if (hMap == NULL || hMap == INVALID_HANDLE_VALUE)
		{
			CloseHandle(hFile);
			throw "create file mapping error";
		}

		bool isCreator = (::GetLastError() != ERROR_ALREADY_EXISTS);

		lpFileMap = MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		if (!lpFileMap)
		{
			CloseHandle(hMap);
			CloseHandle(hFile);
			throw "map view error";
		}

		hMutex = CreateMutex(NULL, FALSE, (LPCWSTR)FileName);
		if (!hMutex || hMutex == INVALID_HANDLE_VALUE)
		{
			UnmapViewOfFile(lpFileMap);
			CloseHandle(hMap);
			CloseHandle(hFile);
			throw "create mutex error";
		}

		HTHandle* ht = (HTHandle*)lpFileMap;
		ht->File = hFile;
		ht->FileMapping = hMap;
		ht->Addr = lpFileMap;
		ht->Mutex = hMutex;

		if (isCreator)
		{
			hSnapshot = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)SnapshotRoutine, ht, 0, &SnapshotThreadId);
			if (!hSnapshot)
			{
				ReleaseMutex(hMutex);
				UnmapViewOfFile(lpFileMap);
				CloseHandle(hMutex);
				CloseHandle(hMap);
				CloseHandle(hFile);
				throw "snapshot thread create error";
			}

			ht->SnapshotThread = hSnapshot;
			std::wcout << L"[SERVER] Snapshot thread started" << std::endl;
		}
		else
		{
			ht->SnapshotThread = NULL;
			std::wcout << L"[CLIENT] Connected to existing storage" << std::endl;
		}

		return ht;
	}


	BOOL Snap
	(
		HTHandle* ht
	)
	{
		WaitForSingleObject(ht->Mutex, INFINITE);
		if (!FlushViewOfFile(ht->Addr, NULL))
		{
			ReleaseMutex(ht->Mutex);
			SetLastError(ht, "Cannot flush view to disk");
			return FALSE;
		}
		ht->Lastsnaptime = time(NULL);
		ReleaseMutex(ht->Mutex);
		return TRUE;
	}

	BOOL Close(HTHandle* ht)
	{
		BOOL ok = TRUE;

		if (!ht) return FALSE;

		try
		{
			Snap(ht);
		}
		catch (...)
		{
			std::cout << "Warning: Snap() failed during Close()" << std::endl;
		}

		if (ht->SnapshotThread != NULL)
		{
			WaitForSingleObject(ht->SnapshotThread, INFINITE);
			CloseHandle(ht->SnapshotThread);
			ht->SnapshotThread = NULL;
			std::cout << "[SERVER] Snapshot thread closed" << std::endl;
		}

		if (ht->Mutex)
		{
			if (!CloseHandle(ht->Mutex))
				ok = FALSE;
			ht->Mutex = NULL;
		}

		if (ht->Addr)
		{
			if (!UnmapViewOfFile(ht->Addr))
				ok = FALSE;
			ht->Addr = NULL;
		}

		if (ht->FileMapping)
		{
			if (!CloseHandle(ht->FileMapping))
				ok = FALSE;
			ht->FileMapping = NULL;
		}

		if (ht->File)
		{
			if (!CloseHandle(ht->File))
				ok = FALSE;
			ht->File = NULL;
		}

		return ok;
	}

	BOOL Insert
	(
		HTHandle* ht,
		Element* el
	)
	{
		WaitForSingleObject(ht->Mutex, INFINITE);
		if (ht->Capacity == ht->ElementCount)
		{
			SetLastError(ht, "Hash table is full");
			ReleaseMutex(ht->Mutex);
			return FALSE;
		}

		if (!CheckElementParm(ht, el))
		{
			ReleaseMutex(ht->Mutex);
			return FALSE;
		}

		bool isInserted = FALSE;

		for (int i = 0, j = HashFunction(el, ht->Capacity, 0);
			i != ht->Capacity && !isInserted;
			j = NextHash(j, ht->Capacity, ++i))
		{
			Element* elFromHT = GetElementFromHT(ht, j);
			if (elFromHT == NULL || IsDeleted(elFromHT))
			{
				SetElementToHT(ht, el, j);
				ht->ElementCount++;
				isInserted = true;
			}
			if (elFromHT != NULL && CheckEqualElementKeys(elFromHT, el)) {
				SetLastError(ht, "Key exists\n");
				ReleaseMutex(ht->Mutex);
				return FALSE;
			}
		}
		ReleaseMutex(ht->Mutex);
		return isInserted;
	}


	BOOL Delete
	(
		HTHandle* ht,
		Element* el
	)
	{
		if (!CheckElementParm(ht, el))
			return FALSE;
		WaitForSingleObject(ht->Mutex, INFINITE);
		int indexInHT = -1;
		bool deleted = false;
		if (ht->ElementCount != 0)
			for (int i = 0, j = HashFunction(el, ht->Capacity, 0);
				GetElementFromHT(ht, j) != NULL && i != ht->Capacity && !deleted;
				j = NextHash(j, ht->Capacity, ++i)) {
			Element* elFromHT = GetElementFromHT(ht, j);
			if (!IsDeleted(elFromHT)) {
				if (CheckEqualElementKeys(elFromHT, el)) {
					indexInHT = j;
					deleted = true;
				}
			}
		}
		if (indexInHT < 0) {
			SetLastError(ht, "Not found key\n");
			ReleaseMutex(ht->Mutex);
			return FALSE;
		}

		SetDeletedFlag(GetElementFromHT(ht, indexInHT));

		ht->ElementCount--;
		ReleaseMutex(ht->Mutex);
		return TRUE;
	}

	Element* Get
	(
		HTHandle* ht,
		Element* el
	)
	{
		if (!CheckElementParm(ht, el))
			return NULL;
		WaitForSingleObject(ht->Mutex, INFINITE);
		int indexInHT = -1;
		bool found = false;
		if (ht->ElementCount != 0)
			for (int i = 0, j = HashFunction(el, ht->Capacity, 0);
				GetElementFromHT(ht, j) != NULL && i != ht->Capacity && !found;
				j = NextHash(j, ht->Capacity, ++i))
		{
			Element* elFromHT = GetElementFromHT(ht, j);
			if (!IsDeleted(elFromHT))
			{
				if (CheckEqualElementKeys(elFromHT, el))
				{
					indexInHT = j; found = true;
				}
			}
		}
		if (indexInHT < 0) {
			SetLastError(ht, "Not found key\n");
			ReleaseMutex(ht->Mutex);
			return NULL;
		}
		ReleaseMutex(ht->Mutex);
		return GetElementFromHT(ht, indexInHT);
	}


	BOOL Update
	(
		HTHandle* ht,
		Element* oldelement,
		const void* newpayload,
		int             newpayloadlength
	)
	{
		if (!CheckElementParm(ht, oldelement) || !CheckElementParm(ht, newpayloadlength))
			return FALSE;
		WaitForSingleObject(ht->Mutex, INFINITE);
		int indexInHT = -1;
		bool updated = false;
		if (ht->ElementCount != 0)
			for (int i = 0, j = HashFunction(oldelement, ht->Capacity, 0);
				GetElementFromHT(ht, j) != NULL && i != ht->Capacity && !updated;
				j = NextHash(j, ht->Capacity, ++i)) {
			Element* elFromHT = GetElementFromHT(ht, j);
			if (!IsDeleted(elFromHT)) {
				if (CheckEqualElementKeys(elFromHT, oldelement)) {
					indexInHT = j;
					updated = true;
				}
			}
		}
		if (indexInHT < 0) {
			SetLastError(ht, "Not found key\n");
			ReleaseMutex(ht->Mutex);
			return FALSE;
		}

		UpdateElement(ht, GetElementFromHT(ht, indexInHT), (void*)newpayload, newpayloadlength);
		ReleaseMutex(ht->Mutex);
		return TRUE;
	}

	char* GetLastError
	(
		HTHandle* ht
	)
	{
		return ht->LastErrorMessage;
	}

	void Print
	(
		const Element* el
	)
	{
		std::cout << "Element:" << std::endl;
		std::cout << "{" << std::endl;
		std::cout << "\t\"key\": \"" << (char*)el->key << "\"," << std::endl;
		std::cout << "\t\"keyLength\": " << el->keylength << "," << std::endl;
		std::cout << "\t\"payload\": \"" << (char*)el->payload << "\"," << std::endl;
		std::cout << "\t\"payloadLength\": " << el->payloadlength << std::endl;
		std::cout << "}" << std::endl;
	}

	BOOL CheckElementParm(HTHandle* ht, Element* el)
	{
		if (el->keylength > ht->MaxKeyLength || el->payloadlength > ht->MaxPayloadLength)
		{
			SetLastError(ht, "Element's key is too long");
			return FALSE;
		}

		if (el->payloadlength > ht->MaxPayloadLength)
		{
			SetLastError(ht, "Element's payload is too long");
			return FALSE;
		}

		if (el->keylength == 0 || el->payloadlength == 0)
		{
			SetLastError(ht, "Element's field is zero");
			return FALSE;
		}

		return TRUE;
	}

	BOOL CheckElementParm(HTHandle* ht, int payloadLength)
	{
		if (payloadLength > ht->MaxPayloadLength)
		{
			SetLastError(ht, "el's key is too long");
			return FALSE;
		}
		return TRUE;
	}

	void SetLastError(HTHandle* ht, const char* message)
	{
		memcpy(ht->LastErrorMessage, (char*)message, sizeof(message) + 10);
	}

	int HashFunction(const Element* el, int size, int p)
	{
		char* arrKeyBytes = new char[el->keylength];
		memcpy(arrKeyBytes, el->key, el->keylength);
		int sumBytes = 0;
		for (int i = 0; i < el->keylength; i++) {
			sumBytes += arrKeyBytes[i];
		}
		double key2 = 5 * ((0.6180339887499 * sumBytes) - int((0.6180339887499 * sumBytes)));
		delete[] arrKeyBytes;
		return (p + sumBytes) % size;
	}

	int NextHash(int hash, int size, int p)
	{
		return (hash + 5 * p + 3 * p * p) % size;
	}

	BOOL CheckEqualElementKeys(Element* el1, Element* el2) {
		int result = !memcmp(el1->key, el2->key, el2->keylength);
		return result;
	}

	Element* GetElementFromHT(HTHandle* ht, int hash) {
		void* elementsAddr = ht + 1;
		int maxElementSize = ht->MaxKeyLength + ht->MaxPayloadLength + 2 * sizeof(int);
		void* elementAddr = (char*)elementsAddr + maxElementSize * hash;

		Element* el = new Element();
		el->key = elementAddr;
		el->keylength = *(int*)((char*)elementAddr + ht->MaxKeyLength);
		el->payload = ((char*)elementAddr + ht->MaxKeyLength + sizeof(int));
		el->payloadlength = *(int*)((char*)elementAddr + ht->MaxKeyLength + sizeof(int) + ht->MaxPayloadLength);
		if (el->keylength == 0) {
			delete el;
			return NULL;
		}
		return el;
	}

	BOOL SetElementToHT(HTHandle* ht, Element* el, int n) {
		void* elementsAddr = ht + 1;
		int maxElementSize = ht->MaxKeyLength + ht->MaxPayloadLength + 2 * sizeof(int);
		void* elementAddr = (char*)elementsAddr + maxElementSize * n;

		memcpy(elementAddr, el->key, el->keylength);
		memcpy(((char*)elementAddr + ht->MaxKeyLength), &el->keylength, sizeof(int));
		memcpy(((char*)elementAddr + ht->MaxKeyLength + sizeof(int)), el->payload, el->payloadlength);
		memcpy(((char*)elementAddr + ht->MaxKeyLength + sizeof(int) + +ht->MaxPayloadLength), &el->payloadlength, sizeof(int));
		return TRUE;
	}

	DWORD WINAPI SnapshotRoutine(HTHandle* ht) {
		while (true)
		{
			if (ht) {
				if (time(NULL) >= ht->Lastsnaptime + ht->SecSnapshotInterval)
				{
					WaitForSingleObject(ht->Mutex, INFINITE);
					if (!FlushViewOfFile(ht->Addr, NULL)) {
						SetLastError(ht, "Snapshot error");
						return FALSE;
					}
					ht->Lastsnaptime = time(NULL);
					cout << "----SNAPSHOT----" << endl;

					ReleaseMutex(ht->Mutex);
				}
			}
			else
				break;
		}
		return TRUE;
	}

	BOOL IsDeleted(Element* el) {
		if (*(int*)el->key == -1) {
			return TRUE;
		}
		return FALSE;
	}

	void UpdateElement(HTHandle* ht, Element* el, const void* newpayload, int newpayloadlength) {
		ZeroMemory(el->payload, ht->MaxPayloadLength + sizeof(int));
		memcpy(el->payload, newpayload, newpayloadlength);
		memcpy((char*)el->payload + ht->MaxPayloadLength, &newpayloadlength, sizeof(int));
	}

	void SetDeletedFlag(Element* el) {
		memcpy(el->key, &DELETED, sizeof(DELETED));
	}
}