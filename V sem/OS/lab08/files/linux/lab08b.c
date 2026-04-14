#define _GNU_SOURCE
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/mman.h>
#include <string.h>

int main() {
    // --- определяем размер страницы ---
    long page_size = sysconf(_SC_PAGESIZE);
    if (page_size <= 0) {
        perror("Ошибка получения размера страницы");
        return 1;
    }

    printf("Page size = %ld bytes\n", page_size);

    size_t total_pages = 256;
    size_t half_pages = total_pages / 2;

    size_t total_size = total_pages * page_size;
    size_t half_size = half_pages * page_size;

    // STEP 1: reserve (mmap with PROT_NONE)
    printf("STEP 1: Reserving %zu pages (%zu bytes)...\n", total_pages, total_size);

    void* region = mmap(NULL, total_size, PROT_NONE,
        MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
    if (region == MAP_FAILED) {
        perror("mmap failed");
        return 1;
    }

    printf("Reserved region: %p\n", region);
    printf("Press ENTER to continue...\n");
    getchar();

    // STEP 2: commit the 2nd half (PROT_READ | PROT_WRITE)
    printf("STEP 2: Commit 128 pages (2nd half)...\n");

    void* second_half = (char*)region + half_size;

    if (mprotect(second_half, half_size, PROT_READ | PROT_WRITE) != 0) {
        perror("mprotect commit failed");
        return 1;
    }

    printf("Committed 2nd half: %p\n", second_half);
    printf("Press ENTER to continue...\n");
    getchar();

    // STEP 3: fill committed memory
    printf("STEP 3: Filling committed memory...\n");

    int* arr = (int*)second_half;
    size_t count = half_size / sizeof(int);

    for (size_t i = 0; i < count; i++)
        arr[i] = (int)i;

    printf("Filled %zu integers. arr[0] = %d, arr[%zu] = %d\n",
        count, arr[0], count - 1, arr[count - 1]);

    printf("Press ENTER to continue...\n");
    getchar();

    // STEP 4: make pages READONLY
    printf("STEP 4: Make 2nd half pages READONLY...\n");

    if (mprotect(second_half, half_size, PROT_READ) != 0) {
        perror("mprotect readonly failed");
        return 1;
    }

    printf("Protection changed to READONLY.\n");
    printf("Press ENTER to continue...\n");
    getchar();

    // STEP 5: uncommit (free physical memory)
    printf("STEP 5: Decommit (free physical memory) 2nd half...\n");

    // munmap освобождает именно эту половину
    if (munmap(second_half, half_size) != 0) {
        perror("munmap (half) failed");
        return 1;
    }

    printf("Decommitted 2nd half.\n");
    printf("Press ENTER to continue...\n");
    getchar();

    // STEP 6: release remaining virtual memory
    printf("STEP 6: Release remaining virtual memory...\n");

    if (munmap(region, half_size) != 0) {
        perror("munmap (remaining) failed");
        return 1;
    }

    printf("Released region.\n");

    return 0;
}
