#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/stat.h>
#include <time.h>

#define BUFFER_SIZE 4096

void print_time(const char *title, time_t t)
{
    struct tm *tm_info = localtime(&t);
    printf("%s: %02d.%02d.%04d %02d:%02d:%02d\n",
           title,
           tm_info->tm_mday,
           tm_info->tm_mon + 1,
           tm_info->tm_year + 1900,
           tm_info->tm_hour,
           tm_info->tm_min,
           tm_info->tm_sec);
}

int is_text_file(int fd)
{
    unsigned char buffer[BUFFER_SIZE];
    ssize_t bytes;

    lseek(fd, 0, SEEK_SET);
    bytes = read(fd, buffer, BUFFER_SIZE);
    if (bytes < 0)
        return 0;

    for (ssize_t i = 0; i < bytes; i++)
    {
        unsigned char b = buffer[i];
        if (b < 9 || (b > 13 && b < 32))
            return 0;
    }
    return 1;
}

void print_info(const char *filename)
{
    int fd = open(filename, O_RDONLY);
    if (fd < 0)
    {
        perror("Не удалось открыть файл");
        return;
    }

    struct stat st;
    if (stat(filename, &st) < 0)
    {
        perror("stat");
        close(fd);
        return;
    }

    printf("Имя файла: %s\n", filename);

    printf("Размер файла: %ld Б (%.2f КиБ, %.2f МиБ)\n",
           st.st_size,
           st.st_size / 1024.0,
           st.st_size / (1024.0 * 1024.0));

    print_time("Создан (ctime)", st.st_ctime);
    print_time("Последний доступ", st.st_atime);
    print_time("Последнее изменение", st.st_mtime);

    if (is_text_file(fd))
        printf("Тип файла: Текстовый\n");
    else
        printf("Тип файла: Бинарный\n");

    close(fd);
}

void print_text(const char *filename)
{
    int fd = open(filename, O_RDONLY);
    if (fd < 0)
    {
        perror("Не удалось открыть файл");
        return;
    }

    if (!is_text_file(fd))
    {
        printf("Файл не является текстовым\n");
        close(fd);
        return;
    }

    lseek(fd, 0, SEEK_SET);

    char buffer[BUFFER_SIZE];
    ssize_t bytes;

    while ((bytes = read(fd, buffer, BUFFER_SIZE)) > 0)
    {
        write(STDOUT_FILENO, buffer, bytes);
    }

    close(fd);
}

int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        printf("lab09a <путь_к_файлу>\n");
        return 1;
    }

    print_info(argv[1]);

    printf("\n----- Содержимое файла -----\n");
    print_text(argv[1]);

    return 0;
}
