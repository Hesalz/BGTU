#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/inotify.h>
#include <sys/stat.h>
#include <dirent.h>
#include <string.h>

#define EVENT_BUF_LEN (1024 * (sizeof(struct inotify_event) + 16))

typedef struct {
    uint32_t cookie;
    char name[256];
} RenameEvent;

int is_temp_file(const char *name)
{
    return (
        strstr(name, ".goutputstream-") != NULL ||
        strstr(name, ".swp") != NULL ||
        name[strlen(name) - 1] == '~'
    );
}

void PrintDirectoryContents(const char *path)
{
    DIR *dir = opendir(path);
    if (!dir)
    {
        perror("opendir");
        return;
    }

    printf("Содержимое каталога %s:\n", path);

    struct dirent *e;
    while ((e = readdir(dir)) != NULL)
    {
        if (!strcmp(e->d_name, ".") || !strcmp(e->d_name, ".."))
            continue;

        if (e->d_type == DT_DIR)
            printf("[DIR]  %s\n", e->d_name);
        else
            printf("[FILE] %s\n", e->d_name);
    }

    closedir(dir);
}

int main(int argc, char *argv[])
{
    if (argc < 2)
    {
        printf("Использование: %s <каталог>\n", argv[0]);
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
        IN_CREATE | IN_DELETE | IN_MODIFY | IN_ATTRIB |
        IN_MOVED_FROM | IN_MOVED_TO |
        IN_DELETE_SELF | IN_MOVE_SELF
    );

    if (wd < 0)
    {
        perror("inotify_add_watch");
        return 1;
    }

    printf("\nОтслеживание изменений (Ctrl+C для выхода)...\n");

    char buffer[EVENT_BUF_LEN];
    RenameEvent lastRename = {0};

    while (1)
    {
        int length = read(fd, buffer, EVENT_BUF_LEN);
        if (length < 0) break;

        int i = 0;
        while (i < length)
        {
            struct inotify_event *ev =
                (struct inotify_event *)&buffer[i];

            if (ev->len > 0 && !is_temp_file(ev->name))
            {
                if (ev->mask & IN_MOVED_FROM)
                {
                    lastRename.cookie = ev->cookie;
                    strncpy(lastRename.name, ev->name, 255);
                }
                else if (ev->mask & IN_MOVED_TO)
                {
                    if (ev->cookie == lastRename.cookie)
                    {
                        printf("[RENAMED] %s -> %s\n",
                               lastRename.name, ev->name);
                        lastRename.cookie = 0;
                    }
                    else
                        printf("[ADDED] %s\n", ev->name);
                }
                else if (ev->mask & IN_CREATE)
                    printf("[ADDED] %s\n", ev->name);

                else if (ev->mask & IN_DELETE)
                    printf("[REMOVED] %s\n", ev->name);

                else if (ev->mask & IN_MODIFY)
                    printf("[MODIFIED] %s\n", ev->name);

                else if (ev->mask & IN_ATTRIB)
                    printf("[ATTRIB] %s\n", ev->name);
            }

            if (ev->mask & IN_DELETE_SELF)
                printf("[DIR DELETED]\n");

            if (ev->mask & IN_MOVE_SELF)
                printf("[DIR MOVED]\n");

            i += sizeof(struct inotify_event) + ev->len;
        }
    }

    close(fd);
    return 0;
}
