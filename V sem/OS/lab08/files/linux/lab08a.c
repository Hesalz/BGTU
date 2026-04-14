#include <stdio.h>
#include <unistd.h>

// ----- глобальные переменные -----
int g_init = 42;          // .data
int g_uninit;             // .bss

static int gs_init = 84;  // .data
static int gs_uninit;     // .bss

void demo_function(void) {
    // пустая функция
}

int main(int argc, char* argv[]) {
    int l_init = 7;       // стек
    int l_uninit;         // стек

    static int ls_init = 128; // .data
    static int ls_uninit;     // .bss

    printf("PID: %d\n", getpid());

    printf("\n== FUNCTIONS ==\n");
    printf(" demo_function:  %p\n", (void*)demo_function);
    printf(" main:           %p\n", (void*)main);

    printf("\n== GLOBALS ==\n");
    printf(" &g_init:        %p\n", (void*)&g_init);
    printf(" &g_uninit:      %p\n", (void*)&g_uninit);
    printf(" &gs_init:       %p\n", (void*)&gs_init);
    printf(" &gs_uninit:     %p\n", (void*)&gs_uninit);

    printf("\n== LOCALS ==\n");
    printf(" &l_init:        %p\n", (void*)&l_init);
    printf(" &l_uninit:      %p\n", (void*)&l_uninit);

    printf("\n== LOCAL STATICS ==\n");
    printf(" &ls_init:       %p\n", (void*)&ls_init);
    printf(" &ls_uninit:     %p\n", (void*)&ls_uninit);

    printf("\n== ARGS ==\n");
    printf(" &argc:          %p\n", (void*)&argc);
    printf(" argv:           %p\n", (void*)argv);
    printf(" argv[0]:        %p\n", (void*)argv[0]);

    printf("\nPress ENTER to exit...\n");
    getchar();

    return 0;
}
