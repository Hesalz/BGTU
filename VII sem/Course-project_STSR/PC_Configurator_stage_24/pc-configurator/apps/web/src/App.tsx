import { FormEvent, MouseEvent, useEffect, useRef, useState } from 'react';
import {
  Activity, ArrowRight, Box, CircuitBoard, Cpu, Fan, HardDrive, MemoryStick,
  Plus, Search, ShieldCheck, SlidersHorizontal, Zap, X, Pencil, Trash2, LogIn, LogOut, Save, Bookmark, Minus, AlertTriangle, CheckCircle2, Info, ChevronLeft, ChevronRight, Image as ImageIcon,
} from 'lucide-react';

type HealthResponse = { status: string; service: string; timestamp: string };
type Category = { id: string; slug: string; name: string; description?: string | null };
type ComponentItem = {
  id: string; category: string; categoryId: string | null; categoryEntity?: Category | null;
  manufacturer: string; model: string; description: string | null; price: number | null;
  powerW: number | null; specifications: string | null; imageUrl: string | null; imageUrls?: string | null; imagePreviewUrl?: string | null; imagePreviewUrls?: string | null; priceOffers?: string | null;
  manufacturerPartNumber?: string | null; priceSource?: string | null; priceUrl?: string | null; priceUpdatedAt?: string | null; imageSource?: string | null;
};
type AuthUser = { id: string; email: string; displayName: string | null; role: 'USER' | 'ADMIN' };
type SavedConfiguration = { id: string; name: string; createdAt: string; updatedAt?: string; components: ComponentItem[]; totalPrice: number; totalPowerW: number };
type SpecEntry = { label: string; value: string };
type ComponentForm = {
  categoryId: string; manufacturer: string; model: string; description: string;
  price: string; powerW: string; specifications: string; imageUrl: string; imageUrls: string;
};
type CompatibilityFinding = { level: 'error' | 'warning' | 'info'; rule: string; message: string };
type CompatibilityResult = { status: 'compatible' | 'incompatible' | 'needs-data'; findings: CompatibilityFinding[] };
type CharacteristicFilter = { label: string; unit: string; labels: RegExp; fromPower?: boolean };

const characteristicFilters: Record<string, CharacteristicFilter> = {
  cpu: { label: 'Частота от', unit: 'ГГц', labels: /базов.*частот/i },
  // OpenDB stores GPU VRAM under `memory`, which is rendered as «Объём памяти».
  gpu: { label: 'Видеопамять от', unit: 'ГБ', labels: /^(память видеокарты|объём памяти)$/i },
  motherboard: { label: 'Слотов памяти от', unit: 'шт.', labels: /слот.*памят/i },
  ram: { label: 'Объём памяти от', unit: 'ГБ', labels: /объём|ёмкост|capacity/i },
  storage: { label: 'Ёмкость от', unit: 'ГБ', labels: /ёмкост|capacity/i },
  cooling: { label: 'Высота до', unit: 'мм', labels: /^высота$/i },
  psu: { label: 'Мощность от', unit: 'Вт', labels: /мощност|wattage/i, fromPower: true },
  case: { label: 'Место для GPU от', unit: 'мм', labels: /максимальн.*длин.*видеокарт/i },
};

const specificationLabels: Record<string, string> = {
  socket: 'Сокет', cores: 'Ядра', total: 'Количество ядер', threads: 'Потоки', chipset: 'Чипсет', chipset_manufacturer: 'Производитель чипсета',
  form_factor: 'Форм-фактор', supported_motherboard_form_factors: 'Поддерживаемые форм-факторы плат', memory_type: 'Тип памяти', memory: 'Объём памяти', capacity: 'Ёмкость',
  speed: 'Скорость памяти', memory_speed: 'Скорость памяти', memory_slots: 'Слоты памяти', max_memory: 'Максимальный объём памяти', tdp: 'Тепловыделение (TDP)',
  wattage: 'Мощность', efficiency: 'Сертификат эффективности', modular: 'Модульные кабели', storage_type: 'Тип накопителя', interface: 'Интерфейс',
  sequential_read: 'Последовательное чтение', sequential_write: 'Последовательная запись', max_video_card_length: 'Максимальная длина видеокарты',
  max_cpu_cooler_height: 'Максимальная высота кулера', internal_3_5_bays: 'Отсеки 3,5 дюйма', internal_2_5_bays: 'Отсеки 2,5 дюйма', expansion_slots: 'Слоты расширения',
  side_panel: 'Боковая панель', has_transparent_side_panel: 'Прозрачная боковая панель', front_panel_usb: 'Разъёмы на передней панели', dimensions: 'Размеры', volume: 'Объём корпуса',
  color: 'Цвет', lighting: 'Подсветка', backlight: 'Подсветка', fan_size: 'Размер вентилятора', noise_level: 'Уровень шума', length: 'Длина', height: 'Высота', width: 'Ширина',
  base_clock: 'Базовая частота', boost_clock: 'Максимальная частота', series: 'Серия', variant: 'Вариант', releaseyear: 'Год выпуска', release_year: 'Год выпуска', part_numbers: 'Артикулы',
  cas_latency: 'CAS Latency', timings: 'Тайминги', ecc: 'Поддержка ECC', registered: 'Тип модулей', heat_spreader: 'Радиатор охлаждения', rgb: 'RGB-подсветка',
  profile_support: 'Поддерживаемые профили разгона', ram_type: 'Тип памяти',
  supported_power_supply_form_factors: 'Поддерживаемые форм-факторы блока питания', power_supply: 'Блок питания',
};
const hiddenSpecificationKeys = new Set(['opendb_id', 'identifiers', 'general_product_information', 'retailer_listings', 'version', 'region', 'pricing_enabled', 'verified', 'source_product_id', 'channel', 'source', 'type', 'value', 'manufacturer_url', 'url', 'website', 'product_url']);
const hiddenValuePattern = /https?:\/\//i;

function normalizeSpecKey(key: string): string {
  return key.replace(/([a-z])([A-Z])/g, '$1_$2').replace(/[\s-]+/g, '_').toLowerCase();
}

function getSpecificationEntries(item: ComponentItem): SpecEntry[] {
  if (!item.specifications?.trim()) return [];
  let source: unknown;
  try { source = JSON.parse(item.specifications); } catch { source = null; }
  const entries: SpecEntry[] = [];
  const add = (key: string, value: unknown, parent = '') => {
    const normalized = normalizeSpecKey(key);
    if (hiddenSpecificationKeys.has(normalized) || value == null || value === '') return;
    if (typeof value === 'object') {
      if (Array.isArray(value)) {
        const printable = value.filter((v) => v != null && typeof v !== 'object' && !hiddenValuePattern.test(String(v))).map(String);
        if (printable.length) entries.push({ label: specificationLabels[normalized] ?? keyToLabel(key), value: printable.join(', ') });
      } else {
        const obj = value as Record<string, unknown>;
        if (normalized === 'metadata') {
          for (const metaKey of ['series', 'variant', 'releaseYear', 'part_numbers']) if (obj[metaKey] != null) add(metaKey, obj[metaKey]);
        } else if (normalized === 'cores') {
          if (obj.total != null) add('total', obj.total);
          if (obj.threads != null) add('threads', obj.threads);
          Object.entries(obj).filter(([child]) => !['total', 'threads'].includes(child)).forEach(([child, childValue]) => add(child, childValue, key));
        } else {
          Object.entries(obj).forEach(([childKey, childValue]) => add(childKey, childValue, key));
        }
      }
      return;
    }
    let shown = String(value).trim();
    if (!shown || hiddenValuePattern.test(shown)) return;
    if (typeof value === 'boolean') shown = value ? 'Да' : 'Нет';
    if (normalized === 'capacity' && item.category === 'ram' && !/гб|gb/i.test(shown)) shown += ' ГБ';
    if (['max_video_card_length', 'max_cpu_cooler_height', 'length', 'height', 'width'].includes(normalized) && /^\d+(\.\d+)?$/.test(shown)) shown += ' мм';
    if (['wattage', 'tdp'].includes(normalized) && /^\d+(\.\d+)?$/.test(shown)) shown += ' Вт';
    if (normalized === 'memory' && typeof value === 'number') shown += ' ГБ';
    const label = specificationLabels[normalized] ?? (parent ? keyToLabel(key) : keyToLabel(key));
    entries.push({ label, value: shown });
  };
  if (source && typeof source === 'object' && !Array.isArray(source)) {
    Object.entries(source as Record<string, unknown>).forEach(([key, value]) => add(key, value));
  } else {
    item.specifications.split(/\r?\n/).forEach((line) => {
      const match = line.match(/^\s*([^:：]+)\s*[:：]\s*(.+)\s*$/);
      if (match) add(match[1].trim(), match[2].trim());
    });
  }
  const seen = new Set<string>();
  return entries.filter((entry) => {
    const key = `${entry.label.toLowerCase()}|${entry.value.toLowerCase()}`;
    if (seen.has(key)) return false;
    seen.add(key); return true;
  });
}

function keyToLabel(key: string): string {
  const normalized = normalizeSpecKey(key);
  return specificationLabels[normalized] ?? normalized.split('_').filter(Boolean).map((part) => part.charAt(0).toUpperCase() + part.slice(1)).join(' ');
}

function descriptionText(value: string | null): string {
  return (value ?? '')
    .replace(/^Импортировано из BuildCores OpenDB\.\s*/i, '')
    .replace(/(?:Страница|Сайт) производителя\s*:\s*https?:\/\/\S+/gi, '')
    .replace(/https?:\/\/\S+/gi, '')
    .replace(/\s*[·|]\s*[·|]\s*/g, ' · ')
    .replace(/^\s*[·|]\s*|\s*[·|]\s*$/g, '')
    .trim();
}

function formatSpecificationsForEdit(item: ComponentItem): string {
  return getSpecificationEntries(item).map((entry) => `${entry.label}: ${entry.value}`).join('\n');
}

function specificationGroup(label: string): string {
  const value = label.toLowerCase();
  if (/ядра|потоки|частот|тепловыдел|tdp|энергопотреб|мощност|скорость памяти|cas latency|тайминг|чтени|записи|профил/.test(value)) return 'Производительность';
  if (/сокет|чипсет|тип памяти|слоты памяти|максимальный объём памяти|поддерживаем|интерфейс|ecc|registered/.test(value)) return 'Совместимость и подключение';
  if (/форм-фактор|длина|высота|ширина|размер|отсек|цвет|подсвет|радиатор|вентилятор|боковая панель|корпус|объём корпуса|rgb/.test(value)) return 'Конструкция и внешний вид';
  if (/серия|вариант|год выпуска|артикул|производитель|сертификат эффективности|модульные кабели/.test(value)) return 'Дополнительные сведения';
  return 'Основные характеристики';
}

function groupSpecifications(entries: SpecEntry[]): { title: string; entries: SpecEntry[] }[] {
  const order = ['Основные характеристики', 'Производительность', 'Совместимость и подключение', 'Конструкция и внешний вид', 'Дополнительные сведения'];
  const groups = new Map<string, SpecEntry[]>();
  entries.forEach((entry) => {
    const group = specificationGroup(entry.label);
    if (!groups.has(group)) groups.set(group, []);
    groups.get(group)!.push(entry);
  });
  return order.filter((title) => groups.has(title)).map((title) => ({ title, entries: groups.get(title)! }));
}

const emptyForm: ComponentForm = {
  categoryId: '', manufacturer: '', model: '', description: '', price: '', powerW: '', specifications: '', imageUrl: '', imageUrls: '',
};
const categoryIcons: Record<string, typeof Cpu> = {
  cpu: Cpu, gpu: CircuitBoard, motherboard: SlidersHorizontal, ram: MemoryStick,
  storage: HardDrive, cooling: Fan, psu: Zap, case: Box,
};
const multiQuantityCategories = new Set(['ram', 'cooling', 'storage']);

function getComponentIcon(item: ComponentItem): typeof Cpu {
  const category = (item.categoryEntity?.slug ?? item.category ?? '').toLowerCase();
  const categoryName = (item.categoryEntity?.name ?? '').toLowerCase();
  if (category === 'gpu' || /видеокарт|графическ/.test(categoryName)) return CircuitBoard;
  if (category === 'motherboard' || /материнск/.test(categoryName)) return SlidersHorizontal;
  if (category === 'ram' || /оперативн.*памят/.test(categoryName)) return MemoryStick;
  if (category === 'storage' || /накопител|ssd|hdd/.test(categoryName)) return HardDrive;
  if (category === 'cooling' || /охлажден|кулер/.test(categoryName)) return Fan;
  if (category === 'psu' || /блок питания/.test(categoryName)) return Zap;
  if (category === 'case' || /корпус/.test(categoryName)) return Box;
  return categoryIcons[category] ?? Cpu;
}

function getComponentImages(item: ComponentItem): string[] {
  const values: string[] = [];
  if (item.imageUrls) {
    try {
      const parsed: unknown = JSON.parse(item.imageUrls);
      if (Array.isArray(parsed)) values.push(...parsed.filter((value): value is string => typeof value === 'string'));
    } catch { /* Older records may not have a gallery yet. */ }
  }
  if (item.imageUrl) values.unshift(item.imageUrl);
  return [...new Set(values.filter((url) => /^https?:\/\//i.test(url)))];
}

function getComponentPreviewImages(item: ComponentItem): string[] {
  const values: string[] = [];
  if (item.imagePreviewUrls) {
    try {
      const parsed: unknown = JSON.parse(item.imagePreviewUrls);
      if (Array.isArray(parsed)) values.push(...parsed.filter((value): value is string => typeof value === 'string'));
    } catch { /* Older records may not have preview images yet. */ }
  }
  if (item.imagePreviewUrl) values.unshift(item.imagePreviewUrl);
  if (!values.length) return getComponentImages(item);
  return [...new Set(values.filter((url) => /^https?:\/\//i.test(url)))];
}

function getPriceOffers(item: ComponentItem): { shop: string; price: number; url: string | null }[] {
  if (!item.priceOffers) return [];
  try {
    const parsed: unknown = JSON.parse(item.priceOffers);
    if (!Array.isArray(parsed)) return [];
    return parsed.filter((offer): offer is { shop: string; price: number; url: string | null } =>
      Boolean(offer) && typeof offer.shop === 'string' && Number.isFinite(Number(offer.price)));
  } catch { return []; }
}

function App() {
  const [apiStatus, setApiStatus] = useState<'checking' | 'online' | 'offline'>('checking');
  const [categories, setCategories] = useState<Category[]>([]);
  const [components, setComponents] = useState<ComponentItem[]>([]);
  const [categoryCounts, setCategoryCounts] = useState<Record<string, number>>({});
  const [detailsItem, setDetailsItem] = useState<ComponentItem | null>(null);
  const [detailsImageIndex, setDetailsImageIndex] = useState(0);
  const [search, setSearch] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [characteristicMin, setCharacteristicMin] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ComponentItem | null>(null);
  const [form, setForm] = useState<ComponentForm>(emptyForm);
  const [saving, setSaving] = useState(false);
  const [authUser, setAuthUser] = useState<AuthUser | null>(() => { try { const raw = localStorage.getItem('pc-configurator-user'); return raw ? JSON.parse(raw) as AuthUser : null; } catch { return null; } });
  const [authToken, setAuthToken] = useState<string>(() => localStorage.getItem('pc-configurator-token') ?? '');
  const [authModalOpen, setAuthModalOpen] = useState(false);
  const [authMode, setAuthMode] = useState<'login' | 'register'>('login');
  const [authEmail, setAuthEmail] = useState('');
  const [authPassword, setAuthPassword] = useState('');
  const [authDisplayName, setAuthDisplayName] = useState('');
  const [authError, setAuthError] = useState('');
  const [selectedComponents, setSelectedComponents] = useState<ComponentItem[]>([]);
  const [compatibility, setCompatibility] = useState<CompatibilityResult | null>(null);
  const [compatibilityError, setCompatibilityError] = useState('');
  const [configurationName, setConfigurationName] = useState('Моя сборка');
  const [savedConfigurations, setSavedConfigurations] = useState<SavedConfiguration[]>([]);
  const [pendingConfigurationConfirmation, setPendingConfigurationConfirmation] = useState<{ action: 'delete' | 'replace'; configuration: SavedConfiguration } | null>(null);
  const [configurationMessage, setConfigurationMessage] = useState('');
  const [configurationBusy, setConfigurationBusy] = useState(false);
  const [openDbImportBusy, setOpenDbImportBusy] = useState(false);
  const [catalogEnrichmentBusy, setCatalogEnrichmentBusy] = useState(false);
  const [importMessage, setImportMessage] = useState('');
  const [catalogDataImportOpen, setCatalogDataImportOpen] = useState(false);
  const [catalogDataSource, setCatalogDataSource] = useState('');
  const [catalogDataFile, setCatalogDataFile] = useState<File | null>(null);
  const [catalogDataBusy, setCatalogDataBusy] = useState(false);
  const [catalogDataError, setCatalogDataError] = useState('');
  const [activeNav, setActiveNav] = useState<'catalog' | 'builder' | 'saved-configurations'>('catalog');
  const navigationLockRef = useRef(false);
  const navigationTimerRef = useRef<number | null>(null);
  const isAdmin = authUser?.role === 'ADMIN';
  const activeCategory = categories.find((category) => category.id === categoryId);
  const activeCharacteristicFilter = activeCategory ? characteristicFilters[activeCategory.slug] : undefined;
  const filteredComponents = (() => {
    const filter = activeCharacteristicFilter;
    if (!filter || characteristicMin === '') return components;
    return components.filter((item) => {
      const entries = getSpecificationEntries(item);
      const raw = filter.fromPower && item.powerW != null
        ? String(item.powerW)
        : entries.find((entry) => filter.labels.test(entry.label))?.value;
      const numeric = Number(raw?.replace(',', '.').match(/\d+(?:\.\d+)?/)?.[0]);
      return Number.isFinite(numeric) && numeric >= Number(characteristicMin);
    });
  })();

  useEffect(() => {
    const updateActiveSection = () => {
      // While smooth navigation is in progress, keep the clicked item active.
      // Otherwise intermediate section positions can flicker the active class.
      if (navigationLockRef.current) return;
      // Determine the active link from section start positions rather than their
      // heights: the build list is an inner scrollable element inside the builder.
      const marker = window.scrollY + 110;
      const topOf = (id: string) => {
        const element = document.getElementById(id);
        return element ? element.getBoundingClientRect().top + window.scrollY : Number.POSITIVE_INFINITY;
      };
      const catalogTop = topOf('catalog');
      const builderTop = topOf('builder');
      const savedConfigurationsTop = topOf('saved-configurations');
      if (marker >= savedConfigurationsTop - 40) setActiveNav('saved-configurations');
      else if (marker >= builderTop - 40) setActiveNav('builder');
      else setActiveNav('catalog');
    };
    window.addEventListener('scroll', updateActiveSection, { passive: true });
    window.addEventListener('hashchange', updateActiveSection);
    updateActiveSection();
    return () => {
      window.removeEventListener('scroll', updateActiveSection);
      window.removeEventListener('hashchange', updateActiveSection);
      if (navigationTimerRef.current !== null) window.clearTimeout(navigationTimerRef.current);
    };
  }, []);

  function navigateToSection(event: MouseEvent<HTMLAnchorElement>, sectionId: 'catalog' | 'builder' | 'saved-configurations') {
    event.preventDefault();
    const target = document.getElementById(sectionId);
    if (!target) return;
    setActiveNav(sectionId);
    navigationLockRef.current = true;
    if (navigationTimerRef.current !== null) window.clearTimeout(navigationTimerRef.current);
    // Explicit scrolling is more reliable than hash-only navigation when the
    // page contains nested scrollable panels and a fixed sidebar.
    window.history.replaceState(null, '', `#${sectionId}`);
    window.scrollTo({ top: Math.max(0, target.getBoundingClientRect().top + window.scrollY - 24), behavior: 'smooth' });
    // Release the lock after the smooth-scroll animation has settled.
    navigationTimerRef.current = window.setTimeout(() => {
      navigationLockRef.current = false;
      navigationTimerRef.current = null;
    }, 900);
  }

  function authHeaders(json = false): HeadersInit {
    return { ...(json ? { 'Content-Type': 'application/json' } : {}), ...(authToken ? { Authorization: `Bearer ${authToken}` } : {}) };
  }

  async function submitAuth(event: FormEvent) {
    event.preventDefault(); setAuthError('');
    try {
      const response = await fetch(`/api/v1/auth/${authMode === 'login' ? 'login' : 'register'}`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(authMode === 'login' ? { email: authEmail, password: authPassword } : { email: authEmail, password: authPassword, displayName: authDisplayName }) });
      const body = await response.json().catch(() => null);
      if (!response.ok) throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || `Не удалось выполнить ${authMode === 'login' ? 'вход' : 'регистрацию'} (HTTP ${response.status})`);
      if (!body?.accessToken || !body?.user) throw new Error('Сервер вернул некорректный ответ. Проверьте журнал API.');
      setAuthToken(body.accessToken); setAuthUser(body.user); localStorage.setItem('pc-configurator-token', body.accessToken); localStorage.setItem('pc-configurator-user', JSON.stringify(body.user)); setAuthModalOpen(false); setAuthPassword('');
    } catch (e) { setAuthError(e instanceof Error ? e.message : 'Ошибка авторизации'); }
  }

  function logout() { setAuthToken(''); setAuthUser(null); setSavedConfigurations([]); localStorage.removeItem('pc-configurator-token'); localStorage.removeItem('pc-configurator-user'); setModalOpen(false); }


  async function loadSavedConfigurations() {
    if (!authToken) { setSavedConfigurations([]); return; }
    try {
      const response = await fetch('/api/v1/configurations', { headers: authHeaders() });
      if (!response.ok) throw new Error('Не удалось загрузить сохранённые конфигурации');
      setSavedConfigurations(await response.json() as SavedConfiguration[]);
    } catch (e) {
      setConfigurationMessage(e instanceof Error ? e.message : 'Ошибка загрузки конфигураций');
    }
  }

  useEffect(() => { void loadSavedConfigurations(); }, [authToken]);

  useEffect(() => {
    if (!configurationMessage) return;
    const timeoutId = window.setTimeout(() => setConfigurationMessage(''), 4500);
    return () => window.clearTimeout(timeoutId);
  }, [configurationMessage]);

  useEffect(() => {
    if (!importMessage) return;
    const timeoutId = window.setTimeout(() => setImportMessage(''), 6000);
    return () => window.clearTimeout(timeoutId);
  }, [importMessage]);

  useEffect(() => { setCharacteristicMin(''); }, [categoryId]);

  useEffect(() => {
    if (!selectedComponents.length) { setCompatibility(null); setCompatibilityError(''); return; }
    let active = true;
    const check = async () => {
      try {
        setCompatibilityError('');
        const response = await fetch('/api/v1/components/compatibility', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ componentIds: selectedComponents.map((item) => item.id) }) });
        const body = await response.json().catch(() => null);
        if (!response.ok) throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || 'Не удалось проверить совместимость.');
        if (active) setCompatibility(body as CompatibilityResult);
      } catch (e) { if (active) { setCompatibility(null); setCompatibilityError(e instanceof Error ? e.message : 'Не удалось проверить совместимость.'); } }
    };
    void check();
    return () => { active = false; };
  }, [selectedComponents]);

  function toggleSelectedComponent(item: ComponentItem) {
    setSelectedComponents((current) => current.some((component) => component.id === item.id)
      ? current.filter((component) => component.id !== item.id)
      : [...current, item]);
    setConfigurationMessage('');
  }

  function setComponentQuantity(item: ComponentItem, quantity: number) {
    const nextQuantity = Math.max(0, Math.min(10, Math.floor(quantity)));
    setSelectedComponents((current) => {
      const withoutItem = current.filter((component) => component.id !== item.id);
      return nextQuantity ? [...withoutItem, ...Array.from({ length: nextQuantity }, () => item)] : withoutItem;
    });
    setConfigurationMessage('');
  }

  async function saveConfiguration(existingOverride?: SavedConfiguration) {
    if (!authToken || !authUser) { setAuthMode('login'); setAuthError('Войди или зарегистрируйся, чтобы сохранить сборку.'); setAuthModalOpen(true); return; }
    if (!selectedComponents.length) { setConfigurationMessage('Сначала добавь в сборку хотя бы одну комплектующую.'); return; }
    if (!configurationName.trim()) { setConfigurationMessage('Укажи название конфигурации.'); return; }
    const normalizedName = configurationName.trim().toLocaleLowerCase('ru-RU');
    const existing = existingOverride ?? savedConfigurations.find((item) => item.name.trim().toLocaleLowerCase('ru-RU') === normalizedName);
    if (existing && !existingOverride) {
      setPendingConfigurationConfirmation({ action: 'replace', configuration: existing });
      return;
    }
    setConfigurationBusy(true); setConfigurationMessage('');
    try {
      const payload = JSON.stringify({ name: configurationName.trim(), componentIds: selectedComponents.map((item) => item.id) });
      const response = await fetch(existing ? `/api/v1/configurations/${existing.id}` : '/api/v1/configurations', {
        method: existing ? 'PATCH' : 'POST', headers: authHeaders(true), body: payload,
      });
      const body = await response.json().catch(() => null);
      if (!response.ok) throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || 'Не удалось сохранить конфигурацию');
      setSavedConfigurations((current) => existing
        ? [body as SavedConfiguration, ...current.filter((item) => item.id !== existing.id)]
        : [body as SavedConfiguration, ...current]);
      setConfigurationMessage(existing ? `Сборка «${existing.name}» обновлена.` : 'Конфигурация сохранена в аккаунте.');
    } catch (e) {
      setConfigurationMessage(e instanceof Error ? e.message : 'Ошибка сохранения конфигурации');
    } finally { setConfigurationBusy(false); }
  }

  async function deleteSavedConfiguration(id: string) {
    setConfigurationBusy(true);
    setConfigurationMessage('');
    try {
      const response = await fetch(`/api/v1/configurations/${id}`, { method: 'DELETE', headers: authHeaders() });
      if (!response.ok) throw new Error('Не удалось удалить конфигурацию');
      const deleted = savedConfigurations.find((item) => item.id === id);
      setSavedConfigurations((current) => current.filter((item) => item.id !== id));
      setPendingConfigurationConfirmation(null);
      setConfigurationMessage(deleted ? `Сборка «${deleted.name}» удалена.` : 'Сборка удалена.');
    } catch (e) { setPendingConfigurationConfirmation(null); setConfigurationMessage(e instanceof Error ? e.message : 'Ошибка удаления конфигурации'); }
    finally { setConfigurationBusy(false); }
  }

  function confirmPendingConfigurationAction() {
    if (!pendingConfigurationConfirmation || configurationBusy) return;
    const pending = pendingConfigurationConfirmation;
    if (pending.action === 'delete') {
      void deleteSavedConfiguration(pending.configuration.id);
    } else {
      setPendingConfigurationConfirmation(null);
      void saveConfiguration(pending.configuration);
    }
  }

  function loadConfigurationIntoBuilder(configuration: SavedConfiguration) {
    setSelectedComponents(configuration.components);
    setConfigurationName(configuration.name);
    setConfigurationMessage(`Загружена сборка «${configuration.name}» в текущий редактор.`);
    window.requestAnimationFrame(() => {
      document.getElementById('builder')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

  async function importOpenDb() {
    if (!authToken || !isAdmin) return;
    if (!window.confirm('Импортировать реальные характеристики из BuildCores OpenDB? Повторный импорт обновит ранее импортированные записи. Цены источник не предоставляет.')) return;
    setOpenDbImportBusy(true);
    setError('');
    setImportMessage('');
    try {
      const response = await fetch('/api/v1/components/import/opendb', { method: 'POST', headers: authHeaders(true) });
      const body = await response.json().catch(() => null);
      if (!response.ok) throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || `Импорт не выполнен (HTTP ${response.status})`);
      await loadCategories();
      await loadComponents();
      setImportMessage(`Импорт завершён: добавлено ${body.inserted}, обновлено ${body.updated}, пропущено ${body.skipped}. Цены в BuildCores OpenDB отсутствуют.`);
      document.getElementById('catalog')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось импортировать каталог');
    } finally { setOpenDbImportBusy(false); }
  }

  async function enrichCatalog() {
    if (!authToken || !isAdmin) return;
    if (!window.confirm('Обновить цены из Onliner и изображения из Open Icecat? Будут изменены только точно сопоставленные модели.')) return;
    setCatalogEnrichmentBusy(true); setError(''); setImportMessage('');
    try {
      const response = await fetch('/api/v1/components/import/catalog-enrichment', { method: 'POST', headers: authHeaders(true) });
      const body = await response.json().catch(() => null);
      if (!response.ok) throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || 'Не удалось обновить цены и изображения.');
      await loadComponents();
      const onliner = body.onliner ?? {}; const icecat = body.icecat ?? {};
      setImportMessage(`Цены Onliner: ${onliner.updatedPrices ?? 0}; предложений магазинов: ${onliner.updatedOffers ?? 0}; галерей Onliner: ${onliner.updatedImages ?? 0}; товаров с фото Icecat: ${icecat.updated ?? 0}; фотографий Icecat: ${icecat.imagesFound ?? 0}; очищено нулевых цен: ${onliner.clearedZeroPrices ?? 0}; пропущено: ${(onliner.skipped ?? 0) + (icecat.skipped ?? 0)}.${icecat.error ? ` Icecat: ${icecat.error}` : ''}`);
    } catch (e) { setError(e instanceof Error ? e.message : 'Не удалось обновить цены и изображения.'); }
    finally { setCatalogEnrichmentBusy(false); }
  }

  async function importCatalogData(event: FormEvent) {
    event.preventDefault();
    if (!catalogDataFile) { setCatalogDataError('Выберите CSV- или XLSX-файл.'); return; }
    if (!catalogDataSource.trim()) { setCatalogDataError('Укажите источник прайс-листа.'); return; }
    setCatalogDataBusy(true); setCatalogDataError('');
    try {
      const formData = new FormData();
      formData.append('file', catalogDataFile);
      formData.append('source', catalogDataSource.trim());
      const response = await fetch('/api/v1/components/import/catalog-file', {
        method: 'POST',
        headers: authHeaders(),
        body: formData,
      });
      const body = await response.json().catch(() => null);
      if (!response.ok) throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || 'Не удалось импортировать прайс-лист.');
      await loadComponents(); await refreshCategoryCounts();
      setImportMessage(`Прайс-лист обработан: обновлено ${body.updated}, не сопоставлено ${body.unmatched}, некорректных строк ${body.invalid}.`);
      setCatalogDataImportOpen(false); setCatalogDataFile(null); setCatalogDataSource('');
    } catch (e) { setCatalogDataError(e instanceof Error ? e.message : 'Не удалось обработать файл прайс-листа.'); }
    finally { setCatalogDataBusy(false); }
  }

  async function refreshCategoryCounts() {
    const response = await fetch('/api/v1/components');
    if (!response.ok) return;
    const items = await response.json() as ComponentItem[];
    const next: Record<string, number> = {};
    items.forEach((item) => { const key = item.categoryId ?? item.category; next[key] = (next[key] ?? 0) + 1; });
    setCategoryCounts(next);
  }

  async function loadCategories() {
    const response = await fetch('/api/v1/categories');
    if (!response.ok) throw new Error('Не удалось загрузить категории');
    const data = await response.json() as Category[];
    setCategories(data);
    await refreshCategoryCounts();
  }

  async function loadComponents() {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      if (search.trim()) params.set('search', search.trim());
      if (categoryId) params.set('categoryId', categoryId);
      if (minPrice !== '') params.set('minPrice', minPrice);
      if (maxPrice !== '') params.set('maxPrice', maxPrice);
      const response = await fetch(`/api/v1/components?${params.toString()}`);
      if (!response.ok) throw new Error('Не удалось загрузить каталог');
      setComponents(await response.json() as ComponentItem[]);
      setError('');
      setApiStatus('online');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Ошибка подключения к серверу');
      setApiStatus('offline');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (!authToken) return;
    fetch('/api/v1/auth/me', { headers: { Authorization: `Bearer ${authToken}` } })
      .then(async (response) => {
        if (!response.ok) throw new Error('Сеанс истёк');
        const user = await response.json() as AuthUser;
        setAuthUser(user); localStorage.setItem('pc-configurator-user', JSON.stringify(user));
      })
      .catch(() => { setAuthToken(''); setAuthUser(null); localStorage.removeItem('pc-configurator-token'); localStorage.removeItem('pc-configurator-user'); });
  }, []);

  useEffect(() => {
    let active = true;
    Promise.all([
      fetch('/api/v1/health').then((r) => { if (!r.ok) throw new Error(); return r.json() as Promise<HealthResponse>; }),
      fetch('/api/v1/categories').then((r) => { if (!r.ok) throw new Error(); return r.json() as Promise<Category[]>; }),
    ]).then(([, cats]) => {
      if (active) { setApiStatus('online'); setCategories(cats); fetch('/api/v1/components').then((r) => r.ok ? r.json() as Promise<ComponentItem[]> : []).then((items) => { if (!active) return; const next: Record<string, number> = {}; items.forEach((item) => { const key = item.categoryId ?? item.category; next[key] = (next[key] ?? 0) + 1; }); setCategoryCounts(next); }).catch(() => undefined); }
    }).catch(() => { if (active) setApiStatus('offline'); });
    return () => { active = false; };
  }, []);

  useEffect(() => {
    const timeout = window.setTimeout(() => { void loadComponents(); }, 250);
    return () => window.clearTimeout(timeout);
  }, [search, categoryId, minPrice, maxPrice]);



  function openCreate() {
    setEditing(null);
    setForm({ ...emptyForm, categoryId: categoryId || categories[0]?.id || '' });
    setModalOpen(true);
  }

  function openEdit(item: ComponentItem) {
    setEditing(item);
    setForm({
      categoryId: item.categoryId ?? item.categoryEntity?.id ?? '',
      manufacturer: item.manufacturer, model: item.model, description: descriptionText(item.description),
      price: item.price == null ? '' : String(item.price), powerW: item.powerW == null ? '' : String(item.powerW),
      specifications: formatSpecificationsForEdit(item), imageUrl: item.imageUrl ?? '', imageUrls: getComponentImages(item).slice(1).join('\n'),
    });
    setModalOpen(true);
  }

  async function saveComponent(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError('');
    const payload = {
      categoryId: form.categoryId, manufacturer: form.manufacturer.trim(), model: form.model.trim(),
      description: form.description || null, price: form.price === '' ? null : Number(form.price),
      powerW: categories.find((category) => category.id === form.categoryId)?.slug === 'psu' && form.powerW !== '' ? Number(form.powerW) : null,
      specifications: form.specifications || null,
      imageUrl: form.imageUrl.trim() || null,
      imageUrls: JSON.stringify([...new Set([form.imageUrl.trim(), ...form.imageUrls.split(/\r?\n/).map((url) => url.trim())].filter((url) => /^https?:\/\//i.test(url)))]),
    };
    try {
      const response = await fetch(editing ? `/api/v1/components/${editing.id}` : '/api/v1/components', {
        method: editing ? 'PATCH' : 'POST',
        headers: authHeaders(true), body: JSON.stringify(payload),
      });
      if (!response.ok) {
        const body = await response.json().catch(() => null);
        throw new Error(Array.isArray(body?.message) ? body.message.join(', ') : body?.message || 'Не удалось сохранить комплектующую');
      }
      setModalOpen(false);
      await loadComponents();
      await refreshCategoryCounts();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Ошибка сохранения');
    } finally { setSaving(false); }
  }

  async function deleteComponent(item: ComponentItem) {
    if (!window.confirm(`Удалить «${item.manufacturer} ${item.model}»?`)) return;
    try {
      const response = await fetch(`/api/v1/components/${item.id}`, { method: 'DELETE', headers: authHeaders() });
      if (!response.ok) throw new Error('Не удалось удалить комплектующую');
      await loadComponents();
      await refreshCategoryCounts();
    } catch (e) { setError(e instanceof Error ? e.message : 'Ошибка удаления'); }
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <a className="brand" href="/" aria-label="PC Configurator"><span className="brand-mark">PC<span>.</span></span><span className="brand-caption">CONFIGURATOR</span></a>
        <div className="side-label">РАБОЧАЯ ОБЛАСТЬ</div>
        <a className={`nav-link ${activeNav === 'catalog' ? 'active' : ''}`} href="#catalog" onClick={(event) => navigateToSection(event, 'catalog')}><Cpu size={18} /> Комплектующие</a>
        <a className={`nav-link ${activeNav === 'builder' ? 'active' : ''}`} href="#builder" onClick={(event) => navigateToSection(event, 'builder')}><SlidersHorizontal size={18} /> Конфигуратор</a>
        <a className={`nav-link ${activeNav === 'saved-configurations' ? 'active' : ''}`} href="#saved-configurations" onClick={(event) => navigateToSection(event, 'saved-configurations')}><Bookmark size={18} /> Сохранённые сборки</a>
        <div className="sidebar-bottom">
          <div className="api-indicator"><span className={`status-dot ${apiStatus}`} /><div><strong>Сервер API</strong><small>{apiStatus === 'online' ? 'Подключение установлено' : apiStatus === 'checking' ? 'Проверка соединения…' : 'Нет соединения'}</small></div></div>
          <div className="version">PC CONFIGURATOR <span>v0.2.0</span></div>
        </div>
      </aside>

      <main className="main-content">
        <header className="topbar"><div className="breadcrumb">Рабочая область <span>/</span> Каталог комплектующих</div><div className="topbar-right"><span className="status-pill"><span className={`status-dot ${apiStatus}`} /> {apiStatus === 'online' ? 'SYSTEM READY' : apiStatus === 'checking' ? 'CONNECTING' : 'API OFFLINE'}</span>{authUser ? <><span className="user-label">{authUser.displayName || authUser.email} · {authUser.role}</span><button className="auth-button" type="button" onClick={logout} title="Выйти"><LogOut size={15} /> Выйти</button></> : <button className="auth-button" type="button" onClick={() => { setAuthMode('login'); setAuthModalOpen(true); }}><LogIn size={15} /> Войти</button>}<div className="avatar">{authUser?.displayName?.[0]?.toUpperCase() ?? 'G'}</div></div></header>
        <section className="hero" id="home">
          <div className="hero-copy"><div className="eyebrow"><span /> PC BUILDING PLATFORM</div><h1>Собери ПК.<br /><em>Без компромиссов.</em></h1><p>Подбери комплектующие, отфильтруй каталог и создай основу своей конфигурации.</p><a className="primary-button" href="#catalog">Открыть каталог <ArrowRight size={17} /></a></div>
          <div className="hero-graphic" aria-hidden="true"><div className="graphic-ring ring-one" /><div className="graphic-ring ring-two" /><div className="graphic-core"><Cpu size={76} strokeWidth={1.1} /></div><div className="graphic-tag tag-top">COMPONENTS <b>02</b></div><div className="graphic-tag tag-bottom"><span /> BUILD YOUR SYSTEM</div><div className="graphic-cross cross-one">+</div><div className="graphic-cross cross-two">+</div></div>
          <div className="hero-index"><strong>{selectedComponents.length}</strong> <span>КОМПЛЕКТУЮЩИХ В СБОРКЕ</span></div>
        </section>

        <section className="section" id="catalog">
          <div className="section-heading"><div><div className="eyebrow">COMPONENT SELECTOR</div><h2>Каталог комплектующих</h2><p>Выбирай категорию, сравнивай основные параметры и открывай карточку комплектующей, чтобы изучить подробные характеристики перед добавлением в сборку.</p></div>{isAdmin && <div className="catalog-admin-actions"><button className="outline-button" type="button" onClick={() => void importOpenDb()} disabled={openDbImportBusy || catalogEnrichmentBusy}><ArrowRight size={15} />{openDbImportBusy ? 'Импорт…' : 'Импортировать комплектующие'}</button><button className="outline-button" type="button" onClick={() => void enrichCatalog()} disabled={openDbImportBusy || catalogEnrichmentBusy}>{catalogEnrichmentBusy ? 'Обновление…' : 'Обновить цены и изображения'}</button><button className="outline-button" type="button" onClick={() => { setCatalogDataImportOpen(true); setCatalogDataError(''); }} disabled={catalogDataBusy}><ArrowRight size={15} /> Импортировать прайс-лист</button><button className="primary-button catalog-add" type="button" onClick={openCreate}><Plus size={17} /> Добавить компонент</button></div>}</div>
          <div className="category-grid">
            {categories.map((category, index) => {
              const Icon = categoryIcons[category.slug] ?? Cpu;
              return <button className={`category-card ${categoryId === category.id ? 'selected' : ''}`} key={category.id} type="button" onClick={() => setCategoryId(categoryId === category.id ? '' : category.id)}>
                <div className="category-top"><span>{String(index + 1).padStart(2, '0')}</span><ArrowRight size={16} /></div><div className="category-content"><div className="category-icon"><Icon size={30} strokeWidth={1.5} /></div><div className="category-copy"><strong>{category.name}</strong><small>{categoryCounts[category.id] ?? 0} компонентов</small></div></div><span className="card-accent" />
              </button>;
            })}
          </div>

          <div className="catalog-toolbar">
            <label className="catalog-search"><Search size={18} /><input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Поиск по модели, производителю..." /></label>
            {activeCharacteristicFilter ? <input aria-label={activeCharacteristicFilter.label} type="number" min="0" step="0.1" placeholder={`${activeCharacteristicFilter.label}, ${activeCharacteristicFilter.unit}`} value={characteristicMin} onChange={(e) => setCharacteristicMin(e.target.value)} /> : <div className="catalog-filter-hint">Выберите категорию выше, чтобы фильтровать по характеристике.</div>}
            <input aria-label="Минимальная цена" type="number" min="0" placeholder="Цена от" value={minPrice} onChange={(e) => setMinPrice(e.target.value)} />
            <input aria-label="Максимальная цена" type="number" min="0" placeholder="Цена до" value={maxPrice} onChange={(e) => setMaxPrice(e.target.value)} />
          </div>
          {error && <div className="catalog-error" role="alert">{error}</div>}
          {importMessage && <div className="import-success" role="status">{importMessage}</div>}
          <div className="catalog-results-heading"><strong>Найдено компонентов: {filteredComponents.length}</strong><button className="text-button" type="button" onClick={() => { setSearch(''); setCategoryId(''); setCharacteristicMin(''); setMinPrice(''); setMaxPrice(''); }}>Сбросить фильтры <X size={15} /></button></div>
          {loading ? <div className="catalog-empty">Загружаем каталог…</div> : filteredComponents.length === 0 ? <div className="catalog-empty"><Cpu size={30} /><strong>Комплектующие не найдены</strong><span>{isAdmin ? 'Измени параметры поиска или добавь первый компонент.' : 'Измени параметры поиска или загляни позже — каталог будет пополняться.'}</span>{isAdmin && <button className="outline-button" type="button" onClick={openCreate}><Plus size={16} /> Добавить компонент</button>}</div> : <div className="component-grid">
            {filteredComponents.map((item) => {
              const images = getComponentImages(item);
              const Icon = getComponentIcon(item);
              const isSelected = selectedComponents.some((selected) => selected.id === item.id);
              const specs = getSpecificationEntries(item).slice(0, 3);
              return <article className="component-card" key={item.id}>
                <button className="component-card-visual" type="button" onClick={() => { setDetailsImageIndex(0); setDetailsItem(item); }} aria-label={`Подробнее: ${item.manufacturer} ${item.model}`}>
                  {getComponentPreviewImages(item)[0] ? <img src={getComponentPreviewImages(item)[0]} alt={`${item.manufacturer} ${item.model}`} loading="lazy" /> : <Icon size={42} strokeWidth={1.35} />}
                  {images.length > 1 && <span className="card-image-count"><ImageIcon size={12} /> {images.length}</span>}
                </button>
                <div className="component-card-body">
                  <span className="component-category">{item.categoryEntity?.name ?? categories.find((c) => c.id === item.categoryId)?.name ?? item.category}</span>
                  <h3>{item.manufacturer} {item.model}</h3>
                  {specs.length > 0 && <div className="card-specs">{specs.map((entry, index) => <span key={`${entry.label}-${index}`}><b>{entry.label}:</b> {entry.value}</span>)}</div>}
                  <div className="component-card-bottom">
                    <div className="component-price">{item.price != null ? <><strong>{Number(item.price).toLocaleString('ru-RU', { maximumFractionDigits: 2 })}</strong><span>BYN</span></> : <span>Цена не указана</span>}</div>
                    <div className="component-actions">
                      <button type="button" title="Подробнее" aria-label="Подробнее" onClick={() => { setDetailsImageIndex(0); setDetailsItem(item); }}><Search size={16} /></button>
                      <button type="button" className={isSelected ? 'component-selected' : ''} title={isSelected ? 'Убрать из сборки' : 'Добавить в сборку'} aria-label={isSelected ? 'Убрать из сборки' : 'Добавить в сборку'} onClick={() => toggleSelectedComponent(item)}>{isSelected ? <Minus size={16} /> : <Plus size={16} />}</button>
                      {isAdmin && <><button type="button" title="Редактировать" aria-label="Редактировать" onClick={() => openEdit(item)}><Pencil size={16} /></button><button type="button" title="Удалить" aria-label="Удалить" onClick={() => void deleteComponent(item)}><Trash2 size={16} /></button></>}
                    </div>
                  </div>
                </div>
              </article>;
            })}
          </div>}
        </section>

        <section className="builder-panel" id="builder"><div className="builder-panel-copy"><div className="eyebrow">YOUR PC BUILD</div><h2>Твоя конфигурация<br /><em>начинается здесь.</em></h2><p>Добавляй комплектующие из каталога, оценивай общую стоимость и сохраняй сборки в своём аккаунте.</p><label className="configuration-name-label">Название сборки<input value={configurationName} maxLength={100} onChange={(event) => setConfigurationName(event.target.value)} placeholder="Например, игровой ПК" /></label><button className="primary-button save-configuration-button" type="button" onClick={() => void saveConfiguration()} disabled={configurationBusy}><Save size={16} />{configurationBusy ? 'Сохранение…' : 'Сохранить конфигурацию'}</button>{!authUser && <p className="configuration-hint">Собирать компьютер можно без регистрации. Вход нужен только для сохранения.</p>}{configurationMessage && <div className={`configuration-message ${configurationMessage.startsWith('Конфигурация сохранена') || configurationMessage.startsWith('Сборка «') || configurationMessage.startsWith('Загружена сборка') ? 'configuration-message-success' : 'configuration-message-error'}`} role="status" aria-live="polite">{configurationMessage}</div>}</div><div className="build-summary"><div className="summary-header"><span>ТЕКУЩАЯ СБОРКА</span><span className="draft-label">{selectedComponents.length} шт.</span></div>{compatibilityError && <div className="compatibility-panel compatibility-error" role="alert"><AlertTriangle size={15} />{compatibilityError}</div>}{compatibility && <div className={`compatibility-panel compatibility-${compatibility.status}`} role="status" aria-live="polite">{compatibility.findings.map((finding, index) => <div className="compatibility-finding" key={`${finding.rule}-${index}`}>{finding.level === 'error' ? <AlertTriangle size={15} /> : finding.level === 'info' ? <CheckCircle2 size={15} /> : <Info size={15} />}<span>{finding.message}</span></div>)}</div>}<div className="build-category-slots" id="compatibility"><div className="build-slots-heading">ВЫБРАННЫЕ КОМПЛЕКТУЮЩИЕ ПО КАТЕГОРИЯМ</div>{categories.map((category) => { const matches = selectedComponents.filter((item) => (item.categoryId ?? item.categoryEntity?.id) === category.id || item.category === category.slug); const uniqueMatches = matches.filter((item, index) => matches.findIndex((candidate) => candidate.id === item.id) === index); return <div className="build-category-slot" key={category.id}><span className="build-category-name">{category.name}</span><div className="build-category-items">{matches.length ? uniqueMatches.map((item) => { const quantity = matches.filter((selected) => selected.id === item.id).length; const canSetQuantity = multiQuantityCategories.has(item.category); return <div className="build-slot-item" key={item.id}><button type="button" className="selected-component-name" onClick={() => { setDetailsImageIndex(0); setDetailsItem(item); }} title="Посмотреть характеристики">{item.manufacturer} {item.model}</button>{canSetQuantity && <span className="component-quantity"><button type="button" aria-label={`Уменьшить количество ${item.model}`} disabled={quantity <= 1} onClick={() => setComponentQuantity(item, quantity - 1)}><Minus size={12} /></button><output>×{quantity}</output><button type="button" aria-label={`Увеличить количество ${item.model}`} disabled={quantity >= 10} onClick={() => setComponentQuantity(item, quantity + 1)}><Plus size={12} /></button></span>}<strong>{item.price == null ? '—' : `${(Number(item.price) * quantity).toLocaleString('ru-RU', { maximumFractionDigits: 2 })} BYN`}</strong><button type="button" aria-label={`Убрать ${item.model}`} onClick={() => setComponentQuantity(item, 0)}><X size={14} /></button></div>; }) : <span className="build-slot-empty">Не выбрано</span>}</div></div>})}</div><div className="summary-footer"><span>ОБЩАЯ СТОИМОСТЬ</span><strong>{selectedComponents.reduce((sum, item) => sum + Number(item.price ?? 0), 0).toLocaleString('ru-RU', { maximumFractionDigits: 2 })} <small>BYN</small></strong></div></div></section>
        <section className="section saved-configurations" id="saved-configurations"><div className="section-heading"><div><div className="eyebrow">YOUR SAVED BUILDS</div><h2>Сохранённые конфигурации</h2><p>Твои сборки доступны после повторного входа в аккаунт.</p></div></div>{!authUser ? <div className="catalog-empty"><Bookmark size={26} /><strong>Войди, чтобы увидеть сохранённые сборки</strong><button className="outline-button" type="button" onClick={() => { setAuthMode('login'); setAuthModalOpen(true); }}>Войти в аккаунт</button></div> : savedConfigurations.length === 0 ? <div className="catalog-empty"><Bookmark size={26} /><strong>Сохранённых сборок пока нет</strong><span>Добавь комплектующие и сохрани первую конфигурацию.</span></div> : <div className="saved-config-list">{savedConfigurations.map((configuration) => <article className="saved-config-card" key={configuration.id}><div><strong>{configuration.name}</strong><span>{configuration.components.length} комплектующих · {Number(configuration.totalPrice).toLocaleString('ru-RU', { maximumFractionDigits: 2 })} BYN · {configuration.totalPowerW} Вт</span><small>{new Date(configuration.updatedAt || configuration.createdAt).toLocaleDateString('ru-RU')}</small></div><div className="saved-config-actions"><button className="outline-button" type="button" onClick={() => loadConfigurationIntoBuilder(configuration)}>Открыть</button><button className="text-button" type="button" onClick={() => setPendingConfigurationConfirmation({ action: 'delete', configuration })}>Удалить</button></div></article>)}</div>}</section>
        <footer className="footer"><span>PC<span className="red">.</span> CONFIGURATOR</span><span>BUILT FOR YOUR NEXT BUILD <Activity size={13} /></span><span>2026</span></footer>
      </main>

      {pendingConfigurationConfirmation && <div className="modal-backdrop confirmation-modal-backdrop" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget && !configurationBusy) setPendingConfigurationConfirmation(null); }}><section className="component-modal action-confirmation-modal" role="dialog" aria-modal="true" aria-labelledby="configuration-confirmation-title"><div className="modal-heading"><div><div className="eyebrow">PC CONFIGURATOR</div><h2 id="configuration-confirmation-title">{pendingConfigurationConfirmation.action === 'delete' ? 'Удалить сборку?' : 'Обновить сборку?'}</h2></div><button type="button" className="modal-close" aria-label="Закрыть" disabled={configurationBusy} onClick={() => setPendingConfigurationConfirmation(null)}><X size={20} /></button></div><p>{pendingConfigurationConfirmation.action === 'delete' ? <>Вы действительно хотите удалить сборку <strong>«{pendingConfigurationConfirmation.configuration.name}»</strong>?</> : <>Сборка с названием <strong>«{pendingConfigurationConfirmation.configuration.name}»</strong> уже существует. Заменить её текущей конфигурацией? Старый состав сборки будет перезаписан.</>}</p>{pendingConfigurationConfirmation.action === 'delete' && <p className="confirmation-warning">Это действие нельзя отменить.</p>}<div className="confirmation-modal-actions"><button className="outline-button" type="button" disabled={configurationBusy} onClick={() => setPendingConfigurationConfirmation(null)}>Отмена</button><button className="primary-button" type="button" disabled={configurationBusy} onClick={confirmPendingConfigurationAction}>{configurationBusy ? (pendingConfigurationConfirmation.action === 'delete' ? 'Удаление…' : 'Сохранение…') : (pendingConfigurationConfirmation.action === 'delete' ? 'Да, удалить' : 'Да, обновить')}</button></div></section></div>}

      {authModalOpen && <div className="modal-backdrop" role="presentation" onMouseDown={(e) => { if (e.target === e.currentTarget) setAuthModalOpen(false); }}><form className="component-modal auth-modal" onSubmit={submitAuth}>
        <div className="modal-heading"><div><div className="eyebrow">PC CONFIGURATOR ACCOUNT</div><h2>{authMode === 'login' ? 'Вход в систему' : 'Регистрация'}</h2></div><button type="button" className="modal-close" aria-label="Закрыть" onClick={() => setAuthModalOpen(false)}><X size={20} /></button></div>
        {authMode === 'register' && <label>Имя<input required minLength={2} maxLength={80} value={authDisplayName} onChange={(e) => setAuthDisplayName(e.target.value)} /></label>}
        <label>Email<input required type="email" maxLength={254} value={authEmail} onChange={(e) => setAuthEmail(e.target.value)} /></label>
        <label>Пароль<input required type="password" minLength={authMode === 'register' ? 8 : 1} maxLength={72} value={authPassword} onChange={(e) => setAuthPassword(e.target.value)} /></label>
        {authError && <div className="catalog-error" role="alert">{authError}</div>}
        <div className="modal-actions"><button className="text-button" type="button" onClick={() => { setAuthMode(authMode === 'login' ? 'register' : 'login'); setAuthError(''); }}>{authMode === 'login' ? 'Создать аккаунт' : 'Уже есть аккаунт? Войти'}</button><button className="primary-button" type="submit">{authMode === 'login' ? 'Войти' : 'Зарегистрироваться'}</button></div>
      </form></div>}

      {catalogDataImportOpen && <div className="modal-backdrop" role="presentation" onMouseDown={(e) => { if (e.target === e.currentTarget && !catalogDataBusy) setCatalogDataImportOpen(false); }}><form className="component-modal catalog-import-modal" onSubmit={importCatalogData}>
        <div className="modal-heading"><div><div className="eyebrow">CATALOG DATA IMPORT</div><h2>Цены и изображения</h2></div><button type="button" className="modal-close" aria-label="Закрыть" disabled={catalogDataBusy} onClick={() => setCatalogDataImportOpen(false)}><X size={20} /></button></div>
        <p className="details-description">Импорт сопоставляет позиции только по артикулу производителя (MPN), поэтому не подменяет цену или фото у похожей, но другой модели.</p>
        <label>Название источника<input required maxLength={120} value={catalogDataSource} placeholder="Например, OnLife / Open Icecat" onChange={(e) => setCatalogDataSource(e.target.value)} /></label>
        <label>Файл прайс-листа<input required type="file" accept=".csv,.xlsx,text/csv,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" onChange={(e) => setCatalogDataFile(e.target.files?.[0] ?? null)} /><small className="field-hint">Столбцы: partNumber (или mpn/артикул), manufacturer, price, productUrl, imageUrl, imageUrls, shop. Если указан shop, price импортируется как отдельное предложение магазина. Цена — в BYN; поддерживаются CSV и XLSX.</small></label>
        {catalogDataError && <div className="catalog-error" role="alert">{catalogDataError}</div>}
        <div className="modal-actions"><button className="text-button" type="button" disabled={catalogDataBusy} onClick={() => setCatalogDataImportOpen(false)}>Отмена</button><button className="primary-button" type="submit" disabled={catalogDataBusy}>{catalogDataBusy ? 'Импорт…' : 'Загрузить прайс-лист'}</button></div>
      </form></div>}

      {detailsItem && <div className="modal-backdrop" role="presentation" onMouseDown={(e) => { if (e.target === e.currentTarget) setDetailsItem(null); }}><div className="component-modal details-modal" role="dialog" aria-modal="true" aria-label="Характеристики комплектующей">
        <div className="modal-heading"><div><div className="eyebrow">COMPONENT DETAILS</div><h2>{detailsItem.manufacturer} {detailsItem.model}</h2></div><button type="button" className="modal-close" aria-label="Закрыть" onClick={() => setDetailsItem(null)}><X size={20} /></button></div>
        <div className="details-meta"><span>{detailsItem.categoryEntity?.name ?? categories.find((c) => c.id === detailsItem.categoryId)?.name ?? detailsItem.category}</span><strong>{detailsItem.price == null ? 'Цена не указана' : `${Number(detailsItem.price).toLocaleString('ru-RU', { maximumFractionDigits: 2 })} BYN`}</strong></div>
        {getComponentImages(detailsItem).length > 0 && <div className="details-gallery">
          <div className="details-gallery-main">
            <img src={getComponentImages(detailsItem)[Math.min(detailsImageIndex, getComponentImages(detailsItem).length - 1)]} alt={`${detailsItem.manufacturer} ${detailsItem.model}, фото ${detailsImageIndex + 1}`} />
            {getComponentImages(detailsItem).length > 1 && <>
              <button type="button" className="gallery-arrow gallery-prev" aria-label="Предыдущее изображение" onClick={() => setDetailsImageIndex((index) => (index - 1 + getComponentImages(detailsItem).length) % getComponentImages(detailsItem).length)}><ChevronLeft size={20} /></button>
              <button type="button" className="gallery-arrow gallery-next" aria-label="Следующее изображение" onClick={() => setDetailsImageIndex((index) => (index + 1) % getComponentImages(detailsItem).length)}><ChevronRight size={20} /></button>
              <span className="gallery-counter">{detailsImageIndex + 1} / {getComponentImages(detailsItem).length}</span>
            </>}
          </div>
          {getComponentImages(detailsItem).length > 1 && <div className="details-gallery-thumbs">{getComponentImages(detailsItem).map((url, index) => <button type="button" key={url} className={index === detailsImageIndex ? 'active' : ''} onClick={() => setDetailsImageIndex(index)} aria-label={`Показать фото ${index + 1}`}><img src={url} alt="" /></button>)}</div>}
        </div>}{detailsItem.manufacturerPartNumber && <p className="details-source">Артикул производителя: {detailsItem.manufacturerPartNumber}</p>}{detailsItem.priceSource && <p className="details-source">Источник цены: {detailsItem.priceUrl ? <a href={detailsItem.priceUrl} target="_blank" rel="noreferrer">{detailsItem.priceSource}</a> : detailsItem.priceSource}{detailsItem.priceUpdatedAt ? ` · обновлено ${new Date(detailsItem.priceUpdatedAt).toLocaleDateString('ru-RU')}` : ''}</p>} 
        {getPriceOffers(detailsItem).length > 0 && <section className="price-offers"><h3>Предложения магазинов</h3>{getPriceOffers(detailsItem).map((offer, index) => <div className="price-offer" key={`${offer.shop}-${offer.price}-${index}`}><span>{offer.shop}</span><strong>{offer.price.toLocaleString('ru-RU', { maximumFractionDigits: 2 })} BYN</strong>{offer.url && <a href={offer.url} target="_blank" rel="noreferrer">В магазин</a>}</div>)}</section>}{detailsItem.imageSource && <p className="details-source">Источник изображения: {detailsItem.imageSource === 'Open Icecat' ? <a href="https://icecat.biz" target="_blank" rel="noreferrer">Specs Icecat</a> : detailsItem.imageSource}</p>}
        {getSpecificationEntries(detailsItem).length === 0 && descriptionText(detailsItem.description) && <p className="details-description">{descriptionText(detailsItem.description)}</p>}
        {getSpecificationEntries(detailsItem).length ? <div className="specification-groups">{groupSpecifications(getSpecificationEntries(detailsItem)).map((group) => <section className="specification-group" key={group.title}><h3>{group.title}</h3><div className="specification-grid">{group.entries.map((entry, index) => <div className="specification-item" key={`${entry.label}-${index}`}><span>{entry.label}</span><strong>{entry.value}</strong></div>)}</div></section>)}</div> : <div className="catalog-empty details-empty">Для этой комплектующей подробные характеристики пока не загружены.</div>}
        <div className="modal-actions"><button className="outline-button" type="button" onClick={() => { toggleSelectedComponent(detailsItem); setDetailsItem(null); }}><Plus size={15} /> Добавить в сборку</button>{isAdmin && <button className="primary-button" type="button" onClick={() => { const item = detailsItem; setDetailsItem(null); openEdit(item); }}>Редактировать</button>}</div>
      </div></div>}

      {modalOpen && <div className="modal-backdrop" role="presentation" onMouseDown={(e) => { if (e.target === e.currentTarget) setModalOpen(false); }}><form className="component-modal" onSubmit={saveComponent}>
        <div className="modal-heading"><div><div className="eyebrow">{editing ? 'EDIT COMPONENT' : 'NEW COMPONENT'}</div><h2>{editing ? 'Редактирование' : 'Новая комплектующая'}</h2></div><button type="button" className="modal-close" aria-label="Закрыть" onClick={() => setModalOpen(false)}><X size={20} /></button></div>
        <label>Категория<select required value={form.categoryId} onChange={(e) => setForm({ ...form, categoryId: e.target.value })}><option value="">Выберите категорию</option>{categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}</select></label>
        <div className="modal-two-columns"><label>Производитель<input required maxLength={120} value={form.manufacturer} onChange={(e) => setForm({ ...form, manufacturer: e.target.value })} /></label><label>Модель<input required maxLength={180} value={form.model} onChange={(e) => setForm({ ...form, model: e.target.value })} /></label></div>
        <div className="modal-two-columns"><label>Цена, BYN<input type="number" min="0" step="0.01" value={form.price} onChange={(e) => setForm({ ...form, price: e.target.value })} /></label>{categories.find((category) => category.id === form.categoryId)?.slug === 'psu' && <label>Мощность блока питания, Вт<input type="number" min="0" step="1" value={form.powerW} onChange={(e) => setForm({ ...form, powerW: e.target.value })} /></label>}</div>
        <label>Описание<textarea rows={3} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} /></label>
        <label>Характеристики<textarea rows={6} placeholder="Например: Сокет: AM5\nЯдра: 8\nПотоки: 16" value={form.specifications} onChange={(e) => setForm({ ...form, specifications: e.target.value })} /><small className="field-hint">Указывай по одной характеристике на строку в формате «Название: значение».</small></label>
        <label>Ссылка на изображение<input type="url" maxLength={1000} value={form.imageUrl} onChange={(e) => setForm({ ...form, imageUrl: e.target.value })} /></label>
        <label>Дополнительные изображения<textarea rows={3} placeholder="https://...\nhttps://..." value={form.imageUrls} onChange={(e) => setForm({ ...form, imageUrls: e.target.value })} /><small className="field-hint">Добавляй по одной HTTPS-ссылке на строку. Они появятся в галерее комплектующей.</small></label>
        <div className="modal-actions"><button className="text-button" type="button" onClick={() => setModalOpen(false)}>Отмена</button><button className="primary-button" type="submit" disabled={saving}>{saving ? 'Сохранение…' : editing ? 'Сохранить' : 'Создать компонент'}</button></div>
      </form></div>}
    </div>
  );
}
export default App;
