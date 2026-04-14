#include <windows.h>
#include <stdio.h>

int main()
{
    SYSTEM_INFO si;
    GetSystemInfo(&si);

    DWORD pageSize = si.dwPageSize;
    printf("Page size = %lu bytes\n", pageSize);

    SIZE_T totalPages = 256;
    SIZE_T reserveSize = totalPages * pageSize;

    printf("STEP 1: Reserving 256 pages...\n");

    void* region = VirtualAlloc(
        NULL,
        reserveSize,
        MEM_RESERVE,
        PAGE_NOACCESS
    );

    if (region == NULL) {
        printf("Error reserving memory: %lu\n", GetLastError());
        return 1;
    }

    printf("Reserved region = %p\n", region);
    printf("Reserved size = %zu bytes (%zu pages)\n", reserveSize, totalPages);
    getchar(); // STEP 1 pause

    printf("STEP 2: Commit 128 pages (2nd half)...\n");

    void* commitAddr = (char*)region + 128 * pageSize;
    SIZE_T commitSize = 128 * pageSize;

    void* committed = VirtualAlloc(
        commitAddr,
        commitSize,
        MEM_COMMIT,
        PAGE_READWRITE
    );

    if (committed == NULL) {
        printf("Error committing memory: %lu\n", GetLastError());
        VirtualFree(region, 0, MEM_RELEASE);
        return 1;
    }

    printf("Committed 2nd half = %p\n", commitAddr);
    printf("Committed size = %zu bytes (%zu pages)\n", commitSize, commitSize / pageSize);
    getchar(); // STEP 2 pause

    printf("STEP 3: Filling committed memory...\n");
    int* arr = (int*)commitAddr;
    size_t intCount = commitSize / sizeof(int);

    for (size_t i = 0; i < intCount; i++)
        arr[i] = (int)i;

    printf("Filled %zu integers\n", intCount);
    getchar(); // STEP 3 pause

    printf("STEP 4: Make pages READONLY...\n");

    DWORD oldProt;
    BOOL protOk = VirtualProtect(
        commitAddr,
        commitSize,
        PAGE_READONLY,
        &oldProt
    );

    if (!protOk) {
        printf("Error changing protection: %lu\n", GetLastError());
    }
    else {
        printf("Protection changed to PAGE_READONLY (old protection: 0x%lx)\n", oldProt);
    }
    getchar(); // STEP 4 pause

    printf("STEP 5: Decommit (free physical memory)...\n");

    BOOL decommitOk = VirtualFree(
        commitAddr,
        commitSize,
        MEM_DECOMMIT
    );

    if (!decommitOk) {
        printf("Error decommitting memory: %lu\n", GetLastError());
    }
    else {
        printf("Decommitted 2nd half\n");
    }
    getchar(); // STEP 5 pause

    printf("STEP 6: Release virtual memory...\n");

    BOOL releaseOk = VirtualFree(
        region,
        0,
        MEM_RELEASE
    );

    if (!releaseOk) {
        printf("Error releasing memory: %lu\n", GetLastError());
    }
    else {
        printf("Released region\n");
    }
    getchar(); // STEP 6 pause

    return 0;
}