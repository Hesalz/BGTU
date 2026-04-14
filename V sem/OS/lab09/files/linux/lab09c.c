#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/inotify.h>
#include <sys/stat.h>
#include <dirent.h>
#include <string.h>
#include <errno.h>

#define EVENT_BUF_LEN (1024 * (sizeof(struct inotify_event) + 16))

void PrintDirectoryContents(const char *path)
{
    DIR *dir = opendir(path);
    if (!dir)
    {
        perror("Ошибка открытия каталога");
        return;
    }

    printf("Содержимое каталога %s:\n", path);

    struct dirent *entry;
    while ((entry = readdir(dir)) != NULL)
    {
        if (!strcmp(entry->d_name, ".") || !strcmp(entry->d_name, ".."))
            continue;

        if (entry->d_type == DT_DIR)
            printf("[DIR]  %s\n", entry->d_name);
        else
            printf("[FILE] %s\n", entry->d_name);
    }

    closedir(dir);
}

void PrintEvent(struct inotify_event *event)
{
    if (event->mask & IN_CREATE)
        printf("[ADDED] %s\n", event->name);

    if (event->mask & IN_DELETE)
        printf("[REMOVED] %s\n", event->name);

    if (event->mask & IN_MODIFY)
        printf("[MODIFIED] %s\n", event->name);

    if (event->mask & IN_ATTRIB)
        printf("[ATTRIB] %s\n", event->name);

    if (event->mask & IN_MOVED_FROM)
        printf("[RENAMED_OLD] %s\n", event->name);

    if (event->mask & IN_MOVED_TO)
        printf("[RENAMED_NEW] %s\n", event->name);

    if (event->mask & IN_DELETE_SELF)
        printf("[DIR DELETED]\n");

    if (event->mask & IN_MOVE_SELF)
        printf("[DIR MOVED]\n");
}

int main(int argc, char *argv[])
{
    if (argc < 2)
    {
        printf("Использование: %s <путь_к_каталогу>\n", argv[0]);
        return 1;
    }

    const char *path = argv[1];

    struct stat st;
    if (stat(path, &st) != 0 || !S_ISDIR(st.st_mode))
    {
        printf("Каталог не существует: %s\n", path);
        return 1;
    }

    PrintDirectoryContents(path);

    int fd = inotify_init();
    if (fd < 0)
    {
        perror("inotify_init");
        return 1;
    }

    int wd = inotify_add_watch(
        fd,
        path,
        IN_CREATE |
        IN_DELETE |
        IN_MODIFY |
        IN_ATTRIB |
        IN_MOVED_FROM |
        IN_MOVED_TO |
        IN_DELETE_SELF |
        IN_MOVE_SELF
    );

    if (wd < 0)
    {
        perror("inotify_add_watch");
        close(fd);
        return 1;
    }

    printf("\nОтслеживание изменений (Ctrl+C для выхода)...\n");

    char buffer[EVENT_BUF_LEN];

    while (1)
    {
        int length = read(fd, buffer, EVENT_BUF_LEN);
        if (length < 0)
        {
            perror("read");
            break;
        }

        int i = 0;
        while (i < length)
        {
            struct inotify_event *event =
                (struct inotify_event *)&buffer[i];

            if (event->len > 0)
                PrintEvent(event);

            i += sizeof(struct inotify_event) + event->len;
        }
    }

    inotify_rm_watch(fd, wd);
    close(fd);
    return 0;
}
