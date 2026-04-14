import ctypes
from ctypes import wintypes
import os

def list_process_threads():
    """
    Выводит список запущенных в процессе потоков.
    """
    print(f"\nТекущий процесс PID: {os.getpid()}")
    print("-" * 40)
    
    kernel32 = ctypes.windll.kernel32
    
    class THREADENTRY32(ctypes.Structure):
        _fields_ = [
            ("dwSize", wintypes.DWORD),
            ("cntUsage", wintypes.DWORD),
            ("th32ThreadID", wintypes.DWORD),
            ("th32OwnerProcessID", wintypes.DWORD),
            ("tpBasePri", wintypes.LONG),
            ("tpDeltaPri", wintypes.LONG),
            ("dwFlags", wintypes.DWORD)
        ]
    
    TH32CS_SNAPTHREAD = 0x00000004
    INVALID_HANDLE_VALUE = -1
    
    snapshot = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0)
    
    if snapshot == INVALID_HANDLE_VALUE:
        error_code = kernel32.GetLastError()
        print(f"Ошибка создания снимка: {error_code}")
        return
    
    try:
        thread_entry = THREADENTRY32()
        thread_entry.dwSize = ctypes.sizeof(THREADENTRY32)
        
        current_pid = os.getpid()
        thread_count = 0
        
        print("Идентификаторы потоков (TID):")
        print("-" * 20)
        
        if kernel32.Thread32First(snapshot, ctypes.byref(thread_entry)):
            while True:
                if thread_entry.th32OwnerProcessID == current_pid:
                    print(f"TID: {thread_entry.th32ThreadID}")
                    thread_count += 1

                if not kernel32.Thread32Next(snapshot, ctypes.byref(thread_entry)):
                    break
                
        print("-" * 20)
        print(f"Всего потоков: {thread_count}\n")
        
    finally:
        kernel32.CloseHandle(snapshot)

if __name__ == "__main__":
    list_process_threads()