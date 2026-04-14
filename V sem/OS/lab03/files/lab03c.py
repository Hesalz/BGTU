import psutil

def main():
    print(f"{'PID':<10} {'Parent PID':<12} {'Name'}")
    print("-" * 40)
    for proc in psutil.process_iter(['pid', 'ppid', 'name']):
        try:
            print(f"{proc.info['pid']:<10} {proc.info['ppid']:<12} {proc.info['name']}")
        except (psutil.NoSuchProcess, psutil.AccessDenied):
            pass

if __name__ == "__main__":
    main()
