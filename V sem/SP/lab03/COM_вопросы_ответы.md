# Ответы на вопросы по COM с примерами из кода

## 19. Что такое COM? COM-программирование?

**COM (Component Object Model)** - это модель компонентов Microsoft, стандарт для создания переиспользуемых программных компонентов.

**COM-программирование** - это разработка компонентов и клиентов, использующих COM для взаимодействия. В вашем проекте:
- `OS12_COM.dll` - COM-сервер (компонент)
- `OS12_COM_2.exe` - COM-клиент (использует компонент напрямую)
- `OS12_04.exe` - COM-клиент (использует компонент через обертку)

---

## 20. Что такое COM-объект(компонент)? CLSID?

**COM-объект (компонент)** - это экземпляр класса, реализующего один или несколько COM-интерфейсов. В вашем проекте это класс `Math`:

```cpp
// OS12_COM/Math.h
class Math : public IAdder, public IMultiplier
{
    // Реализует интерфейсы IAdder и IMultiplier
};
```

**CLSID (Class Identifier)** - уникальный идентификатор класса COM-компонента. В вашем коде:

```cpp
// OS12_COM/Interface.h, строки 5-7
// {e4ffc6c9-e6ca-4cfe-84f0-7b656b8fd825}
DEFINE_GUID(CLSID_Math,
    0xe4ffc6c9, 0xe6ca, 0x4cfe, 0x84, 0xf0, 0x7b, 0x65, 0x6b, 0x8f, 0xd8, 0x25);
```

Используется при создании объекта:
```cpp
// OS12_COM_2/main.cpp, строка 20
CoCreateInstance(CLSID_Math, NULL, CLSCTX_INPROC_SERVER, IID_IUnknown, (void**)&pIUnknown);
```

---

## 21. Что такое GUID? Где применяется GUID? Размер GUID-идентификатора?

**GUID (Globally Unique Identifier)** - глобально уникальный идентификатор (128 бит = 16 байт).

**Применение в проекте:**

1. **CLSID** - идентификатор класса:
```cpp
// OS12_COM/Interface.h, строки 5-7
DEFINE_GUID(CLSID_Math, ...);
```

2. **IID** - идентификатор интерфейса:
```cpp
// OS12_COM/Interface.h, строки 9-11
DEFINE_GUID(IID_IAdder,
    0xf8db8e0a, 0x60eb, 0x46f1, 0x85, 0x2f, 0x14, 0xb9, 0x96, 0xee, 0x74, 0x95);

// Строки 13-15
DEFINE_GUID(IID_IMultiplier,
    0xbebd0c4b, 0xcd13, 0x4773, 0xb2, 0x37, 0x06, 0xfa, 0xbf, 0xd4, 0xa2, 0x58);
```

---

## 22. Какие типы COM-контейнеров бывают?

1. **DLL (In-Process)** - компонент в DLL, работает в процессе клиента (ваш случай - `OS12_COM.dll`)
2. **EXE (Out-of-Process)** - компонент в отдельном процессе
3. **Сервис Windows** - компонент как системная служба
4. **Remote (DCOM)** - компонент на удаленном компьютере

В вашем проекте используется **DLL-контейнер**.

---

## 23. Что является клиентом и сервером в COM?

**COM-сервер** - компонент, предоставляющий функциональность:
- В вашем проекте: `OS12_COM.dll` (класс `Math`, фабрика `MathFactory`)

**COM-клиент** - приложение, использующее компонент:
- `OS12_COM_2.exe` - прямой клиент
- `OS12_04.exe` - клиент через обертку `OS12_LIB`

Пример использования клиентом:
```cpp
// OS12_COM_2/main.cpp, строка 20
CoCreateInstance(CLSID_Math, ...); // Клиент запрашивает сервер создать объект
```

---

## 24. Поясните понятия «однокомпонентный» и «многокомпонентный» COM-сервер.

**Однокомпонентный сервер** - DLL/EXE содержит один COM-класс.

**Многокомпонентный сервер** - DLL/EXE содержит несколько COM-классов.

В вашем проекте - **однокомпонентный сервер** (только класс `Math`):
```cpp
// OS12_COM/dllmain.cpp, строки 58-67
STDAPI DllGetClassObject(const CLSID& clsid, const IID& iid, void** ppv) {
    // Проверяет только CLSID_Math
    if (clsid != CLSID_Math) rc = CLASS_E_CLASSNOTAVAILABLE;
    // ...
}
```

---

## 25. Поясните типы COM-серверов: CLSCTX_INPROC_SERVER, CLSCTX_LOCAL_SERVER, CLSCTX_REMOTE_SERVER.

**CLSCTX_INPROC_SERVER** - сервер в DLL, работает в процессе клиента (ваш случай):
```cpp
// OS12_COM_2/main.cpp, строка 20
CoCreateInstance(CLSID_Math, NULL, CLSCTX_INPROC_SERVER, ...);
```

**CLSCTX_LOCAL_SERVER** - сервер в отдельном процессе на том же компьютере.

**CLSCTX_REMOTE_SERVER** - сервер на удаленном компьютере (DCOM).

---

## 26. Как называется имя библиотеки, обеспечивающей работу COM-приложений?

**OLE32.dll** - основная библиотека COM.

Также используются:
- `OLEAUT32.dll` - автоматизация COM
- `COMSVCS.dll` - дополнительные сервисы COM

В вашем коде:
```cpp
// OS12_COM_2/main.cpp, строка 14
CoInitialize(NULL); // Инициализация COM из OLE32.dll
```

---

## 27. Поясните назначение типа и структуру HRESULT.

**HRESULT** - тип результата операций COM (32-битное значение).

**Структура HRESULT (32 бита):**
```
31 30 29 | 28-27 | 26-16 | 15-0
    S    |   R   |   F   | Code
```
- **S (31 бит)** - знак (0 = успех, 1 = ошибка)
- **R (27-28 биты)** - резерв
- **Facility (16-27 биты)** - категория кода
- **Code (0-15 биты)** - код ошибки

**Примеры в вашем коде:**
```cpp
// Успешные коды
return S_OK;                    // 0x00000000
return S_FALSE;                 // 0x00000001

// Коды ошибок
return E_NOINTERFACE;           // Интерфейс не поддерживается
return CLASS_E_CLASSNOTAVAILABLE; // Класс недоступен
return E_OUTOFMEMORY;           // Нехватка памяти

// Проверка результата
// OS12_COM_2/main.cpp, строка 22
if (SUCCEEDED(hr0) && pIUnknown != NULL) // SUCCEEDED проверяет бит S
```

---

## 28. Что такое COM-интерфейс?

**COM-интерфейс** - это контракт (абстрактный класс) с чисто виртуальными функциями, определяющий методы, которые должен реализовать компонент.

В вашем проекте:
```cpp
// OS12_COM/Interface.h, строки 21-25
interface IAdder : IUnknown
{
    STDMETHOD(Add(const double x,const double y,double& z))PURE;
    STDMETHOD(Sub(const double x,const double y,double& z))PURE;
};
```

Интерфейс наследуется от `IUnknown` и содержит только объявления методов (PURE = чисто виртуальные).

---

## 29. Чем характеризуется COM-интерфейс?

1. **Наследует IUnknown** - базовый интерфейс
2. **Имеет уникальный IID** - идентификатор интерфейса
3. **Неизменяемость** - после публикации не изменяется (можно только расширять)
4. **Полиморфизм** - один объект может реализовывать несколько интерфейсов
5. **Стандартизация** - методы используют соглашение `__stdcall`

В вашем коде класс реализует два интерфейса:
```cpp
// OS12_COM/Math.h, строка 11
class Math : public IAdder, public IMultiplier
{
    // Реализует оба интерфейса
};
```

---

## 30. Что значит «стандартный» COM-интерфейс?

**Стандартный COM-интерфейс** - интерфейс, определенный Microsoft и обязательный для всех COM-компонентов.

**Примеры стандартных интерфейсов:**
- `IUnknown` - базовый (обязателен для всех)
- `IClassFactory` - фабрика классов (для создания объектов)

В вашем проекте `MathFactory` реализует стандартный `IClassFactory`:
```cpp
// OS12_COM/MathFactory.h, строка 5
class MathFactory : public IClassFactory
{
    // Стандартный интерфейс от Microsoft
};
```

---

## 31. Назовите два стандартных COM-интерфейса.

1. **IUnknown** - базовый интерфейс (наследуют все COM-интерфейсы):
```cpp
// Все интерфейсы наследуют IUnknown
interface IAdder : IUnknown { ... }
interface IMultiplier : IUnknown { ... }
```

2. **IClassFactory** - фабрика классов:
```cpp
// OS12_COM/MathFactory.h, строка 5
class MathFactory : public IClassFactory
```

---

## 32. Перечислите методы интерфейса IUnknown и поясните их назначение.

**IUnknown** имеет 3 метода:

1. **QueryInterface** - получение указателя на другой интерфейс того же объекта:
```cpp
// OS12_COM/Math.cpp, строки 16-31
HRESULT STDMETHODCALLTYPE Math::QueryInterface(REFIID riid, void** ppv)
{
    if (riid == IID_IUnknown || riid == IID_IAdder)
        *ppv = (IAdder*)this;
    else if (riid == IID_IMultiplier)
        *ppv = (IMultiplier*)this;
    else
        return E_NOINTERFACE;
    
    if (*ppv) {
        AddRef(); // Увеличивает счетчик при успехе
        return (S_OK);
    }
}
```

2. **AddRef** - увеличивает счетчик ссылок:
```cpp
// OS12_COM/Math.cpp, строки 33-37
STDMETHODIMP_(ULONG) Math::AddRef()
{
    InterlockedIncrement(&m_lRef); // Атомарное увеличение
    return m_lRef;
}
```

3. **Release** - уменьшает счетчик ссылок, удаляет объект при 0:
```cpp
// OS12_COM/Math.cpp, строки 39-49
STDMETHODIMP_(ULONG) Math::Release()
{
    InterlockedDecrement(&m_lRef);
    if (m_lRef == 0)
    {
        delete this; // Удаление объекта при счетчике = 0
        return 0;
    }
    else
        return m_lRef;
}
```

**Использование в клиенте:**
```cpp
// OS12_COM_2/main.cpp, строка 25
pIUnknown->QueryInterface(IID_IAdder, (void**)&pIAdder); // Получение интерфейса
// ...
pIAdder->Release(); // Освобождение ссылки
```

---

## 33. Что такое «фабрика классов» и для чего она нужна?

**Фабрика классов** - объект, реализующий `IClassFactory`, который создает экземпляры COM-объектов.

**Назначение:**
- Инкапсулирует логику создания объектов
- Позволяет контролировать создание экземпляров
- Позволяет блокировать/разблокировать сервер

В вашем проекте:
```cpp
// OS12_COM/MathFactory.h
class MathFactory : public IClassFactory
{
    // Создает объекты класса Math
};
```

Создание фабрики:
```cpp
// OS12_COM/dllmain.cpp, строки 58-67
STDAPI DllGetClassObject(const CLSID& clsid, const IID& iid, void** ppv) {
    MathFactory* pF;
    if ((pF = new MathFactory()) == NULL) 
        rc = E_OUTOFMEMORY;
    else {
        rc = pF->QueryInterface(iid, ppv); // Возвращает фабрику
        pF->Release();
    }
}
```

---

## 34. Перечислите методы интерфейса IClassFactory и поясните их назначение.

**IClassFactory** наследует `IUnknown` + имеет 2 метода:

1. **CreateInstance** - создает экземпляр COM-объекта:
```cpp
// OS12_COM/MathFactory.cpp, строки 46-65
STDMETHODIMP MathFactory::CreateInstance(LPUNKNOWN pUnkOuter, REFIID riid, void** ppvObj) {
    Math* pMath = nullptr;
    
    if (pUnkOuter != NULL)
        hr = CLASS_E_NOAGGREGATION; // Агрегация не поддерживается
    else if ((pMath = new Math()) == NULL)
        hr = E_OUTOFMEMORY;
    else {
        hr = pMath->QueryInterface(riid, ppvObj); // Получаем нужный интерфейс
        pMath->Release();
    }
    
    if (FAILED(hr))
        delete pMath;
    
    return hr;
}
```

2. **LockServer** - блокирует/разблокирует сервер в памяти:
```cpp
// OS12_COM/MathFactory.cpp, строки 67-74
STDMETHODIMP MathFactory::LockServer(BOOL fLock) {
    if (fLock)
        InterlockedIncrement(&g_lLocks); // Блокировка
    else
        InterlockedDecrement(&g_lLocks); // Разблокировка
    
    return S_OK;
}
```

**Использование:**
```cpp
// OS12_COM_2/main.cpp - CoCreateInstance внутренне вызывает:
// 1. DllGetClassObject -> получает фабрику
// 2. IClassFactory::CreateInstance -> создает объект
// 3. QueryInterface -> получает нужный интерфейс
```

---

## 35. Что такое «счетчик ссылок на интерфейсы»? Для чего он нужен? Каким образом и когда этот счетчик увеличивается и уменьшается?

**Счетчик ссылок** - переменная, отслеживающая количество активных ссылок на объект.

**Назначение:** Автоматическое управление временем жизни объекта.

**В вашем коде:**
```cpp
// OS12_COM/Math.h, строка 14
volatile ULONG m_lRef; // Счетчик ссылок на объект
```

**Увеличение счетчика:**
1. При создании объекта (инициализация = 1):
```cpp
// OS12_COM/Math.cpp, строки 5-9
Math::Math()
{
    m_lRef = 1; // Начальное значение
    InterlockedIncrement(&g_lObjs);
}
```

2. При `QueryInterface` (успешном):
```cpp
// OS12_COM/Math.cpp, строки 25-26
if (*ppv) {
    AddRef(); // Увеличивает счетчик
}
```

3. При `AddRef`:
```cpp
// OS12_COM/Math.cpp, строки 33-37
STDMETHODIMP_(ULONG) Math::AddRef()
{
    InterlockedIncrement(&m_lRef); // Увеличение
    return m_lRef;
}
```

**Уменьшение счетчика:**
1. При `Release`:
```cpp
// OS12_COM/Math.cpp, строки 39-49
STDMETHODIMP_(ULONG) Math::Release()
{
    InterlockedDecrement(&m_lRef); // Уменьшение
    if (m_lRef == 0)
    {
        delete this; // Удаление при счетчике = 0
        return 0;
    }
    return m_lRef;
}
```

**Пример использования в клиенте:**
```cpp
// OS12_COM_2/main.cpp
pIUnknown->QueryInterface(IID_IAdder, (void**)&pIAdder); // AddRef вызывается
// ...
pIAdder->Release(); // Уменьшает счетчик
pIUnknown->Release(); // Уменьшает счетчик, при 0 объект удаляется
```

---

## 36. Какое соглашение о вызове и возврате должен обеспечивать метод COM-объекта? Какие методы являются исключением?

**Соглашение о вызове:** `__stdcall` (STDCALL)

**Тип возврата:** Методы возвращают `HRESULT` (кроме `AddRef` и `Release`)

**В вашем коде:**
```cpp
// Обычные методы возвращают HRESULT
STDMETHODIMP Math::Add(const double x, const double y, double& z) {
    z = x + y;
    return S_OK; // HRESULT
}

// STDMETHOD = HRESULT __stdcall
// STDMETHODIMP = HRESULT STDMETHODCALLTYPE
```

**Исключения:**
1. **AddRef** - возвращает `ULONG`:
```cpp
// OS12_COM/Math.cpp, строка 33
STDMETHODIMP_(ULONG) Math::AddRef() // ULONG вместо HRESULT
```

2. **Release** - возвращает `ULONG`:
```cpp
// OS12_COM/Math.cpp, строка 39
STDMETHODIMP_(ULONG) Math::Release() // ULONG вместо HRESULT
```

---

## 37. Что должен «знать» COM-клиент, чтобы использовать COM-объект?

1. **CLSID** - идентификатор класса:
```cpp
// OS12_COM_2/main.cpp, строка 20
CoCreateInstance(CLSID_Math, ...); // Клиент должен знать CLSID
```

2. **IID** - идентификатор интерфейса:
```cpp
// OS12_COM_2/main.cpp, строка 25
pIUnknown->QueryInterface(IID_IAdder, (void**)&pIAdder);
```

3. **Определение интерфейса** - методы и их сигнатуры:
```cpp
// OS12_COM/Interface.h - клиент должен видеть определение
interface IAdder : IUnknown
{
    STDMETHOD(Add(const double x,const double y,double& z))PURE;
    STDMETHOD(Sub(const double x,const double y,double& z))PURE;
};
```

4. **Как инициализировать COM**:
```cpp
// OS12_COM_2/main.cpp, строка 14
CoInitialize(NULL); // Инициализация COM
```

---

## 38. Объясните в чем заключается процесс регистрации COM-объекта?

**Регистрация** - запись информации о COM-компоненте в реестр Windows.

**Процесс регистрации в вашем коде:**

1. Функция регистрации:
```cpp
// OS12_COM/dllmain.cpp, строки 31-40
STDAPI DllRegisterServer()
{
    return RegisterServer(
        hmodule,        // Модуль DLL
        CLSID_Math,     // CLSID класса
        FriendlyName,   // Понятное имя
        VerIndProg,     // Версия-независимый ProgID
        ProgID          // ProgID
    );
}
```

2. Запись в реестр:
```cpp
// OS12_COM/Registry.cpp, строки 18-47
HRESULT RegisterServer(...) {
    // Получает полный путь к DLL
    GetModuleFileName(hModule, szModule, ...);
    
    // Записывает в реестр:
    // 1. CLSID\{CLSID} = FriendlyName
    setKeyAndValue(szKey, NULL, szFriendlyName);
    
    // 2. CLSID\{CLSID}\InprocServer32 = путь к DLL
    setKeyAndValue(szKey, L"InprocServer32", szModule);
    
    // 3. CLSID\{CLSID}\ProgID = ProgID
    setKeyAndValue(szKey, L"ProgID", szProgID);
    
    // 4. ProgID\CLSID = CLSID
    setKeyAndValue(szProgID, L"CLSID", szCLSID);
}
```

**Запуск регистрации:**
```cmd
regsvr32 OS12_COM.dll
```

---

## 39. Поясните назначение утилиты regsvr32 и принцип ее работы.

**Назначение:** Регистрация/раскомментирование COM-компонентов в реестре Windows.

**Принцип работы:**
1. Загружает указанную DLL
2. Вызывает `DllRegisterServer()` для регистрации
3. Вызывает `DllUnregisterServer()` для удаления (с флагом `/u`)

**В вашем коде:**
```cpp
// OS12_COM/dllmain.cpp, строка 31
STDAPI DllRegisterServer() // Вызывается regsvr32
{
    return RegisterServer(...);
}
```

**Команды:**
- `regsvr32 OS12_COM.dll` - регистрация
- `regsvr32 /u OS12_COM.dll` - удаление регистрации

---

## 40. Поясните назначение утилиты regedit.

**regedit** - редактор реестра Windows для просмотра и редактирования.

**Использование:** Просмотр зарегистрированных COM-компонентов:
```
HKEY_CLASSES_ROOT\CLSID\{e4ffc6c9-e6ca-4cfe-84f0-7b656b8fd825}\InprocServer32
```

В вашем README:
```markdown
## Проверка реестра
### regedit
### Компьютер\HKEY_CLASSES_ROOT\OS12_secxndary.1
```

---

## 41. Перечислите пять функций, которые экспортируются COM/DLL-контейнером. Поясните назначение этих функций.

Функции экспортируются в `OS12_COM/OS_COM.def` и реализованы в `dllmain.cpp`:

1. **DllGetClassObject** - возвращает фабрику классов:
```cpp
// OS12_COM/dllmain.cpp, строки 58-67
STDAPI DllGetClassObject(const CLSID& clsid, const IID& iid, void** ppv) {
    MathFactory* pF;
    if (clsid != CLSID_Math) 
        rc = CLASS_E_CLASSNOTAVAILABLE;
    else if ((pF = new MathFactory()) == NULL) 
        rc = E_OUTOFMEMORY;
    else {
        rc = pF->QueryInterface(iid, ppv); // Возвращает фабрику
        pF->Release();
    }
    return rc;
}
```

2. **DllRegisterServer** - регистрирует компонент в реестре:
```cpp
// OS12_COM/dllmain.cpp, строки 31-40
STDAPI DllRegisterServer()
{
    return RegisterServer(hmodule, CLSID_Math, FriendlyName, VerIndProg, ProgID);
}
```

3. **DllUnregisterServer** - удаляет регистрацию:
```cpp
// OS12_COM/dllmain.cpp, строки 42-49
STDAPI DllUnregisterServer()
{
    return UnregisterServer(CLSID_Math, VerIndProg, ProgID);
}
```

4. **DllCanUnloadNow** - проверяет, можно ли выгрузить DLL:
```cpp
// OS12_COM/dllmain.cpp, строки 51-57
STDAPI DllCanUnloadNow() 
{
    if ((g_lLocks == 0) && (g_lObjs == 0))
        return S_OK;  // Можно выгрузить
    else
        return S_FALSE; // Нельзя выгрузить
}
```

5. **DllInstall** - дополнительная установка (опциональная):
```cpp
// OS12_COM/dllmain.cpp, строки 26-29
STDAPI DllInstall(BOOL b, PCWSTR s) 
{
    return S_OK;
}
```

**Экспорт в .def файле:**
```def
// OS12_COM/OS_COM.def
LIBRARY "OS_COM"
EXPORTS
    DllCanUnloadNow      PRIVATE
    DllGetClassObject    PRIVATE
    DllInstall           PRIVATE
    DllRegisterServer    PRIVATE
    DllUnregisterServer  PRIVATE
```

---

## 42. Назовите функцию COM-контейнера, которая вызывается OLE32 для получения указателя на фабрику классов.

**DllGetClassObject**

```cpp
// OS12_COM/dllmain.cpp, строки 58-67
STDAPI DllGetClassObject(const CLSID& clsid, const IID& iid, void** ppv) {
    // OLE32 вызывает эту функцию когда клиент вызывает CoCreateInstance
    MathFactory* pF;
    if ((pF = new MathFactory()) == NULL) 
        rc = E_OUTOFMEMORY;
    else {
        rc = pF->QueryInterface(iid, ppv); // Возвращает IClassFactory*
    }
    return rc;
}
```

**Цепочка вызовов:**
```cpp
// Клиент:
CoCreateInstance(CLSID_Math, ...)
    ↓
// OLE32.dll:
DllGetClassObject(CLSID_Math, IID_IClassFactory, &pFactory)
    ↓
// Ваша DLL возвращает фабрику
    ↓
// OLE32 вызывает:
pFactory->CreateInstance(..., &pObject)
```

---

## 43. Назовите функцию фабрики классов, в которой создается объект компонента.

**CreateInstance**

```cpp
// OS12_COM/MathFactory.cpp, строки 46-65
STDMETHODIMP MathFactory::CreateInstance(LPUNKNOWN pUnkOuter, REFIID riid, void** ppvObj) {
    Math* pMath = nullptr;
    
    if (pUnkOuter != NULL)
        hr = CLASS_E_NOAGGREGATION;
    else if ((pMath = new Math()) == NULL) // Создание объекта
        hr = E_OUTOFMEMORY;
    else {
        hr = pMath->QueryInterface(riid, ppvObj); // Возврат нужного интерфейса
        pMath->Release();
    }
    
    return hr;
}
```

---

## 44. Поясните назначение «счетчика экземпляров компонент». Где этот счетчик увеличивается и где уменьшается?

**Назначение:** Отслеживание количества активных экземпляров COM-объектов для управления выгрузкой DLL.

**Объявление:**
```cpp
// OS12_COM/Math.cpp, строка 2
long g_lObjs = 0; // Глобальный счетчик экземпляров
```

**Увеличение:** При создании объекта (в конструкторе):
```cpp
// OS12_COM/Math.cpp, строки 5-9
Math::Math()
{
    m_lRef = 1;
    InterlockedIncrement(&g_lObjs); // УВЕЛИЧЕНИЕ при создании
}
```

**Уменьшение:** При уничтожении объекта (в деструкторе):
```cpp
// OS12_COM/Math.cpp, строки 11-14
Math::~Math() 
{
    InterlockedDecrement(&g_lObjs); // УМЕНЬШЕНИЕ при удалении
}
```

**Использование:**
```cpp
// OS12_COM/dllmain.cpp, строки 51-57
STDAPI DllCanUnloadNow() 
{
    if ((g_lLocks == 0) && (g_lObjs == 0)) // Проверяет счетчик
        return S_OK;  // Можно выгрузить DLL
    else
        return S_FALSE; // Есть активные объекты
}
```

---

## 45. Назовите условие, при котором объект компонента удаляется.

**Объект удаляется, когда счетчик ссылок (`m_lRef`) становится равным 0.**

```cpp
// OS12_COM/Math.cpp, строки 39-49
STDMETHODIMP_(ULONG) Math::Release()
{
    InterlockedDecrement(&m_lRef);
    if (m_lRef == 0)  // УСЛОВИЕ УДАЛЕНИЯ
    {
        delete this;  // УДАЛЕНИЕ ОБЪЕКТА
        return 0;
    }
    else
        return m_lRef;
}
```

**Пример жизненного цикла:**
```cpp
// 1. Создание (m_lRef = 1)
Math* pMath = new Math(); // m_lRef = 1

// 2. QueryInterface увеличивает счетчик
pMath->AddRef(); // m_lRef = 2

// 3. Release уменьшает счетчик
pMath->Release(); // m_lRef = 1

// 4. Последний Release удаляет объект
pMath->Release(); // m_lRef = 0 -> delete this
```

---

## 46. Объясните механизм блокировки COM-сервера (функция LockServer фабрики классов).

**Механизм блокировки** - удержание DLL в памяти даже при отсутствии активных объектов.

**Реализация:**
```cpp
// OS12_COM/MathFactory.cpp, строки 67-74
STDMETHODIMP MathFactory::LockServer(BOOL fLock) {
    if (fLock)
        InterlockedIncrement(&g_lLocks); // БЛОКИРОВКА
    else
        InterlockedDecrement(&g_lLocks); // РАЗБЛОКИРОВКА
    
    return S_OK;
}
```

**Глобальный счетчик блокировок:**
```cpp
// OS12_COM/Math.cpp, строка 3
long g_lLocks = 0; // Счетчик блокировок сервера
```

**Использование:**
```cpp
// OS12_COM/dllmain.cpp, строки 51-57
STDAPI DllCanUnloadNow() 
{
    if ((g_lLocks == 0) && (g_lObjs == 0))
        return S_OK;  // Можно выгрузить (нет блокировок и объектов)
    else
        return S_FALSE; // Нельзя выгрузить (есть блокировки или объекты)
}
```

**Назначение:**
- Оптимизация производительности (избежание частой загрузки/выгрузки DLL)
- Предотвращение выгрузки во время активного использования
- Используется клиентами для оптимизации работы с сервером

**Пример использования:**
```cpp
// Клиент может заблокировать сервер:
pFactory->LockServer(TRUE);  // g_lLocks++
// ... работа с объектами ...
pFactory->LockServer(FALSE); // g_lLocks--
```

---

## Итоговая схема работы COM в проекте:

```
1. Клиент: CoCreateInstance(CLSID_Math, ...)
   ↓
2. OLE32: DllGetClassObject(CLSID_Math, IID_IClassFactory, ...)
   ↓
3. DLL: MathFactory* pF = new MathFactory()
   ↓
4. OLE32: pFactory->CreateInstance(..., IID_IUnknown, ...)
   ↓
5. Factory: Math* pMath = new Math() → g_lObjs++
   ↓
6. Factory: pMath->QueryInterface(IID_IUnknown, ...) → m_lRef = 1
   ↓
7. Клиент: pUnknown->QueryInterface(IID_IAdder, ...) → m_lRef = 2
   ↓
8. Клиент: pAdder->Add(2, 3, z)
   ↓
9. Клиент: pAdder->Release() → m_lRef = 1
   ↓
10. Клиент: pUnknown->Release() → m_lRef = 0 → delete this → g_lObjs--
```

