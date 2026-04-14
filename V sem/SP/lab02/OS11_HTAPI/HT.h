#pragma once
#include <Windows.h>
#include <iomanip>
#include <iostream>

#define _CRT_SECURE_NO_WARNINGS

#ifdef OS11HTAPI_EXPORTS
#define OS11HTAPI __declspec(dllexport)
#else
#define OS11HTAPI __declspec(dllimport)
#endif

namespace HT   
{

	extern "C" OS11HTAPI struct HTHandle   
	{
		HTHandle();
		HTHandle(int Capacity, int SecSnapshotInterval, int MaxKeyLength, int MaxPayloadLength, const wchar_t FileName[512]);
		int     Capacity;               
		int     SecSnapshotInterval;    
		int     MaxKeyLength;           
		int     MaxPayloadLength;       
		char    FileName[512];          
		HANDLE  File;                   
		HANDLE  FileMapping;            
		LPVOID  Addr;                   
		char    LastErrorMessage[512];  
		time_t  Lastsnaptime;           
		HANDLE  Mutex;
		int ElementCount;
		HANDLE SnapshotThread; 
	};

	extern "C" OS11HTAPI struct Element   
	{
		OS11HTAPI Element();
		OS11HTAPI Element(const void* key, int keylength);                                             
		OS11HTAPI Element(const void* key, int keylength, const void* payload, int  payloadlength);    
		OS11HTAPI Element(Element* oldelement, const void* newpayload, int  newpayloadlength);         
		void* key;               										   
		int             keylength;           
		void* payload;       
		int             payloadlength;       
	};
	extern "C" OS11HTAPI HTHandle * OpenExist                  
	(
		const wchar_t    FileName[512]         
	); 	

	extern "C" OS11HTAPI HTHandle * Create          
	(
		int	  Capacity,					  
		int   SecSnapshotInterval,		   
		int   MaxKeyLength,                
		int   MaxPayloadLength,            
		const wchar_t FileName[512]        
	); 	

	extern "C" OS11HTAPI HTHandle * Open       
	(
		const wchar_t FileName[512]
		
	); 	
	extern "C" OS11HTAPI HTHandle * OpenExist(const wchar_t FileName[512]);

	extern "C" OS11HTAPI BOOL Snap        
	(
		HTHandle * ht          
	);


	extern "C" OS11HTAPI BOOL Close        
	(
		HTHandle * ht           
	);


	extern "C" OS11HTAPI BOOL Insert    
	(
		HTHandle * ht,           
		Element * el              
	);	


	extern "C" OS11HTAPI BOOL Delete     
	(
		HTHandle * ht,         
		Element * el            
	);	

	extern "C" OS11HTAPI Element * Get    
	(
		HTHandle * ht,           
		Element * el             
	); 


	extern "C" OS11HTAPI BOOL Update    
	(
		HTHandle * ht,           
		Element * oldelement,          
		const void* newpayload,          
		int             newpayloadlength    
	); 

	char* GetLastError  
	(
		HTHandle* ht                     
	);

	const int DELETED = -1;

	void SetDeletedFlag(Element* el);

	void SetLastError(HTHandle* ht, const char* message); 

	BOOL CheckElementParm(HTHandle* ht, Element* el);

	BOOL CheckElementParm(HTHandle* ht, int payloadLength);

	BOOL CheckEqualElementKeys(Element* el1, Element* el2);

	int HashFunction(const Element* el, int size, int p);

	int NextHash(int hash, int size, int p);

	Element* GetElementFromHT(HTHandle* ht, int hash);

	BOOL SetElementToHT(HTHandle* ht, Element* el, int n);

	DWORD WINAPI SnapshotRoutine(HTHandle* ht);

	BOOL IsDeleted(Element* el);

	void UpdateElement(HTHandle* ht, Element* el, const void* newpayload, int newpayloadlength);

	extern "C" OS11HTAPI void Print                              
	(
		const Element * el             
	);
};
