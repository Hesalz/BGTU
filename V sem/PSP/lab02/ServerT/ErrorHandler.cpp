#include "ErrorHandler.h"
#include <winsock2.h>

string GetErrorMsgText(int code) {
    string msgText;
    switch (code) {
    case WSAEINTR: msgText = "Работа функции прервана"; break;
    case WSAEACCES: msgText = "Разрешение отвергнуто"; break;
    case WSAEFAULT: msgText = "Ошибочный адрес"; break;
    case WSAEINVAL: msgText = "Ошибка в аргументе"; break;
    case WSAEWOULDBLOCK: msgText = "Ресурс временно недоступен"; break;
    default: msgText = "Неизвестная ошибка"; break;
    }
    return msgText;
}

string SetErrorMsgText(string msgText, int code) {
    return msgText + " " + GetErrorMsgText(code);
}
