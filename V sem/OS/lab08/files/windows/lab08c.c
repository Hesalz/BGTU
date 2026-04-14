#define _WIN32_WINNT 0x0600
#include <windows.h>
#include <stdio.h>

// Параметры ПО ЗАДАНИЮ
#define INITIAL_HEAP_SIZE (1 * 1024 * 1024)   // 1 MiB
#define MAX_HEAP_SIZE     (8 * 1024 * 1024)   // 8 MiB
#define BLOCK_SIZE        (512 * 1024)        // 512 KiB (ЗАДАНИЕ!)
#define NUM_BLOCKS        10                  // 10 блоков (ЗАДАНИЕ!)

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
    printf("=== LAB-08C: Heap Management (MAIN TASK) ===\n");
    printf("Initial heap size: 1 MiB, Maximum: 8 MiB\n");
    printf("Block size: 512 KiB, Number of blocks: 10\n");
    printf("Total to allocate: 5 MiB (10 * 512 KiB)\n\n");
    
    // ========== ЭТАП 1: Создание кучи ==========
    printf("STEP 1: Creating heap...\n");
    HANDLE heap = HeapCreate(0, INITIAL_HEAP_SIZE, MAX_HEAP_SIZE);
    if (!heap) {
        printf("Failed to create heap! Error: %lu\n", GetLastError());
        return 1;
    }
    printf("Heap created successfully!\n");
    
    HeapInfo(heap);
    system("pause & cls");
    
    // ========== ЭТАП 2: Выделение 10 блоков по 512KB ==========
    printf("STEP 2: Allocating %d blocks of %d bytes each\n", NUM_BLOCKS, BLOCK_SIZE);
    printf("Each block will contain %d integers (512KB / 4 bytes)\n\n", BLOCK_SIZE / sizeof(int));
    
    void* blocks[NUM_BLOCKS] = {0};
    
    for (int i = 0; i < NUM_BLOCKS; i++) {
        // Выделение блока памяти
        blocks[i] = HeapAlloc(heap, HEAP_ZERO_MEMORY, BLOCK_SIZE);
        if (!blocks[i]) {
            printf("ERROR: Failed to allocate block %d! Error: %lu\n", i, GetLastError());
            break;
        }
        
        printf("Allocated block %d at address %p\n", i, blocks[i]);
        
        // ЗАПОЛНЕНИЕ МАССИВОМ ЦЕЛЫХ ЧИСЕЛ (требование задания)
        int* data = (int*)blocks[i];
        int count = BLOCK_SIZE / sizeof(int);  // 512KB / 4 = 131072 чисел
        for (int j = 0; j < count; j++) {
            data[j] = i * 1000000 + j;  // Уникальные значения
        }
        printf("Filled with %d integers (first: %d, last: %d)\n", 
               count, data[0], data[count-1]);
        
        // HeapInfo после каждой итерации (требование задания)
        HeapInfo(heap);
        
        if (i < NUM_BLOCKS - 1) {
            system("pause & cls");
        }
    }
    
    printf("\nSuccessfully allocated all %d blocks!\n", NUM_BLOCKS);
    system("pause & cls");
    
    // ========== ЭТАП 3: Освобождение блоков ==========
    printf("STEP 3: Freeing all blocks...\n\n");
    for (int i = 0; i < NUM_BLOCKS; i++) {
        if (blocks[i]) {
            if (HeapFree(heap, 0, blocks[i])) {
                printf("Freed block %d from address %p\n", i, blocks[i]);
                blocks[i] = NULL;
            } else {
                printf("Failed to free block %d! Error: %lu\n", i, GetLastError());
            }
        }
    }
    
    HeapInfo(heap);
    system("pause & cls");
    
    // ========== ЭТАП 4: Уничтожение кучи ==========
    printf("STEP 4: Destroying heap...\n");
    if (HeapDestroy(heap)) {
        printf("Heap destroyed successfully!\n");
    } else {
        printf("Failed to destroy heap! Error: %lu\n", GetLastError());
        return 1;
    }
    
    printf("\n=== MAIN TASK COMPLETED ===\n");
    printf("Press any key to exit...");
    getchar();
    
    return 0;
}