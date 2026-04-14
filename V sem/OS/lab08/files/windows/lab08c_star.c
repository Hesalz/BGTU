#define _WIN32_WINNT 0x0600
#include <windows.h>
#include <stdio.h>

// Измененные параметры для задания со звездочкой
#define INITIAL_HEAP_SIZE (1 * 1024 * 1024)   // 1 MiB
#define MAX_HEAP_SIZE     (8 * 1024 * 1024)   // 8 MiB
#define BLOCK_SIZE        (1 * 1024 * 1024)   // 1 MiB (изменено!)
#define NUM_BLOCKS        5                   // 5 блоков (изменено!)

void HeapInfo(HANDLE heap) {
    printf("\n=== HEAP INFORMATION ===\n");
    
    PROCESS_HEAP_ENTRY entry = {0};
    SIZE_T totalBusy = 0;
    SIZE_T totalFree = 0;
    int busyCount = 0;
    int freeCount = 0;
    int regionCount = 0;
    
    if (HeapLock(heap)) {
        while (HeapWalk(heap, &entry)) {
            if (entry.wFlags & PROCESS_HEAP_REGION) {
                regionCount++;
                printf("Region %d: Start=%p, Size=%lu bytes, Type=REGION\n", 
                       regionCount, entry.lpData, 
                       (unsigned long)entry.Region.dwCommittedSize);
            }
            else if (entry.wFlags & PROCESS_HEAP_ENTRY_BUSY) {
                busyCount++;
                totalBusy += entry.cbData;
                printf("Block: Start=%p, Size=%lu bytes, Type=BUSY\n", 
                       entry.lpData, (unsigned long)entry.cbData);
            }
            else if (entry.wFlags & PROCESS_HEAP_UNCOMMITTED_RANGE) {
                printf("Block: Start=%p, Size=%lu bytes, Type=UNCOMMITTED\n", 
                       entry.lpData, (unsigned long)entry.cbData);
            }
            else {
                freeCount++;
                totalFree += entry.cbData;
                printf("Block: Start=%p, Size=%lu bytes, Type=FREE\n", 
                       entry.lpData, (unsigned long)entry.cbData);
            }
        }
        HeapUnlock(heap);
    }
    
    printf("\nSummary:\n");
    printf("Total heap size: ~%lu bytes\n", 
           (unsigned long)((totalBusy + totalFree) / 1024) * 1024);
    printf("Busy blocks: %d, Total busy: %lu bytes\n", busyCount, (unsigned long)totalBusy);
    printf("Free blocks: %d, Total free: %lu bytes\n", freeCount, (unsigned long)totalFree);
    printf("Heap regions: %d\n", regionCount);
    printf("===========================\n");
}

int main() {
    printf("=== LAB-08C: Heap Management (5 blocks of 1 MiB) ===\n");
    printf("Initial heap size: 1 MiB, Maximum: 8 MiB\n");
    printf("Block size: 1 MiB, Number of blocks: 5\n");
    printf("Total to allocate: 5 MiB (within 8 MiB limit)\n\n");
    
    // ЭТАП 1: Создание кучи
    printf("STEP 1: Creating heap...\n");
    HANDLE heap = HeapCreate(0, INITIAL_HEAP_SIZE, MAX_HEAP_SIZE);
    if (!heap) {
        printf("Failed to create heap! Error: %lu\n", GetLastError());
        return 1;
    }
    printf("Heap created successfully!\n");
    
    HeapInfo(heap);
    system("pause & cls");
    
    // ЭТАП 2: Выделение блоков
    printf("STEP 2: Allocating %d blocks of %d bytes each\n", NUM_BLOCKS, BLOCK_SIZE);
    
    void* blocks[NUM_BLOCKS] = {0};
    
    for (int i = 0; i < NUM_BLOCKS; i++) {
        blocks[i] = HeapAlloc(heap, HEAP_ZERO_MEMORY, BLOCK_SIZE);
        if (!blocks[i]) {
            printf("\nERROR: Failed to allocate block %d! Error: %lu\n", i, GetLastError());
            printf("This may happen because:\n");
            printf("1. Heap fragmentation\n");
            printf("2. Not enough contiguous memory\n");
            printf("3. Heap management overhead\n");
            break;
        }
        
        printf("\nAllocated block %d at address %p\n", i, blocks[i]);
        
        // Заполнение массива
        int* data = (int*)blocks[i];
        int count = BLOCK_SIZE / sizeof(int);  // 262144 чисел
        for (int j = 0; j < count; j++) {
            data[j] = i * 1000000 + j;
        }
        printf("Filled with %d integers\n", count);
        
        HeapInfo(heap);
        if (i < NUM_BLOCKS - 1) {
            system("pause & cls");
        }
    }
    
    printf("\nAllocation completed.\n");
    system("pause & cls");
    
    // Освобождение и уничтожение
    printf("STEP 3: Freeing blocks...\n");
    for (int i = 0; i < NUM_BLOCKS; i++) {
        if (blocks[i] && HeapFree(heap, 0, blocks[i])) {
            printf("Freed block %d\n", i);
        }
    }
    
    printf("\nSTEP 4: Destroying heap...\n");
    HeapDestroy(heap);
    printf("Heap destroyed.\n\n");
    
    printf("=== Analysis for question ===\n");
    printf("With 5 blocks of 1 MiB each:\n");
    printf("- Total required: 5 MiB\n");
    printf("- Initial heap: 1 MiB\n");
    printf("- Max heap size: 8 MiB\n");
    printf("- Result: Should succeed as 5 MiB < 8 MiB\n");
    printf("- Heap will expand automatically as needed\n");
    printf("- Physical memory allocated on first touch (write)\n");
    
    system("pause");
    return 0;
}