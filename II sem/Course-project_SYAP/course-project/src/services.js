import React, { useState } from "react";
import "./services.css";
import Header from "./header";
import Footer from "./footer";
import { useSelector } from "react-redux";
import CurrencySwitcher from "./CurrencySwitcher";

const prices = {
    BYN: [
      998.3, 1091.06, 1115.77, 1274.05, 1677.52, 1134.11, 1141.64, 3311.62, // Имплантация
      455.29, 571.04, 532.29, 532.29, 838.51, 538.61, 1097.92, 1168.46, 1213.67, 1284.21, 1174.92, 1245.46, 1481.14, 1551.68, 1181.24, 1251.78,
      885.85, 1001.73, 962.98, // Протезирование
      11002.73, 10897.69, 11530.81, 14233.53, 11220.57, // All-on-4
      8598.25, 8822.17, 8786.65, 9370.51, 8776.57, 8747.53, 8723.85, 8735.69, 8692.45, // All-on-6
      8723.85, 8747.53, 8661.05, 8735.69, 8692.45, // All-on-6 Скуловая
      46.34, 442.26, 420.42, 91.18, 70.85, 848.23, 797.85, 784.98, 742.34, 1054.32, 244.42, 176.31, 309.49, 786.96, 689.97, 890.63, 1194.47, 1240.8, 1136.45, 1270.61, 1195.9, 120.14, 111.26, 35.96, 47.09, 86.93, 93.51, 133.51, 126.91,
      69.53, 221.96, 71.02, 334.75, 1430.2, 2013.9, 297.18, 3004.01, 286.55, 294.73, 365.07, 344.6, 333.16, 228.46, 440.92,
      44.99, 151.91, 181.48, 149.43, 166.73, 195.63, 333.45, 244.42, 174.66, 510.65, 41.79, 142.59, 361.57, 75.09,
      1113.17, 333.45, // Виниры
    ],
    USD: [
      375, 410, 420, 480, 630, 420, 425, 1230, // Имплантация
      165, 205, 190, 190, 300, 193, 393, 420, 435, 460, 420, 445, 520, 545, 415, 440,
      310, 350, 335, // Протезирование
      3865, 3825, 4050, 5000, 3950, // All-on-4
      3020, 3100, 3085, 3290, 3080, 3070, 3060, 3065, 3050, // All-on-6
      3060, 3070, 3035, 3065, 3050, // All-on-6 Скуловая
      16, 155, 147, 32, 25, 300, 282, 277, 262, 375, 87, 63, 111, 283, 248, 320, 430, 447, 410, 460, 430, 43, 40, 13, 17, 31, 33, 47, 45,
      25, 80, 26, 120, 515, 725, 107, 1085, 103, 107, 130, 123, 120, 85, 165,
      16, 55, 65, 54, 60, 70, 120, 87, 62, 183, 15, 51, 130, 27,
      400, 120, // Виниры
    ],
    EUR: [
      350, 380, 400, 450, 600, 400, 410, 1180, // Имплантация
      160, 200, 185, 185, 290, 188, 380, 405, 425, 450, 410, 435, 510, 535, 405, 430,
      300, 340, 325, // Протезирование
      3750, 3700, 3900, 4800, 3800, // All-on-4
      2950, 3020, 3005, 3200, 3000, 2990, 2980, 2990, 2980, // All-on-6
      2980, 2990, 2950, 2990, 2980, // All-on-6 Скуловая
      15, 150, 145, 30, 24, 290, 275, 270, 255, 365, 85, 62, 110, 275, 245, 315, 420, 435, 400, 450, 420, 42, 39, 12, 16, 30, 32, 45, 44,
      24, 78, 25, 115, 500, 700, 105, 1050, 100, 105, 125, 120, 115, 82, 160,
      15, 52, 62, 52, 58, 67, 115, 85, 60, 178, 14, 50, 127, 26,
      390, 115, // Виниры
    ],
    RUB: [
      37800, 41300, 42300, 48500, 63500, 42300, 42800, 124500, // Имплантация
      16600, 20600, 19100, 19100, 30200, 19400, 39500, 42200, 43700, 46300, 42300, 44800, 52500, 55000, 41800, 44300,
      31400, 35400, 33800, // Протезирование
      390000, 386000, 408000, 504000, 398000, // All-on-4
      306000, 314000, 312000, 333000, 312000, 311000, 310000, 311500, 310000, // All-on-6
      310000, 311000, 306000, 311500, 310000, // All-on-6 Скуловая
      1600, 15800, 15000, 3200, 2500, 30000, 28700, 28000, 26500, 38000, 8800, 6300, 11100, 28600, 25600, 33200, 45200, 46800, 42000, 47200, 44200, 4400, 3900, 1300, 1700, 3100, 3300, 4700, 4500,
      2500, 8000, 2600, 12000, 53000, 75000, 11000, 108500, 10400, 11000, 13200, 12600, 12200, 8600, 16500,
      1600, 5500, 6500, 5400, 5900, 6800, 11700, 8700, 6200, 18500, 1500, 5200, 13500, 2700,
      41000, 12500, // Виниры
    ],
  };
  

export default function Services() {
    const [openDropdown, setOpenDropdown] = useState(null);

    const toggleDropdown = (index) => {
        setOpenDropdown((prev) => (prev === index ? null : index));
    };
    const currentCurrency = useSelector((state) => state.currency);

    const services = [
        {
            title: "Имплантация",
            items: [
                `Установка имплантата NORIS MEDICAL (Израиль) — ${prices[currentCurrency][0]} ${currentCurrency}`,
                `Установка имплантата OSSTEM (Корея) — ${prices[currentCurrency][1]} ${currentCurrency}`,
                `Установка имплантата MEGAGEN Any One (Корея) — ${prices[currentCurrency][2]} ${currentCurrency}`,
                `Установка имплантата MEGAGEN Any Ridge (Корея) — ${prices[currentCurrency][3]} ${currentCurrency}`,
                `Установка имплантата STRAUMANN SLA (Швейцария) — ${prices[currentCurrency][4]} ${currentCurrency}`,
                `Установка имплантата NEODENT (Швейцария) — ${prices[currentCurrency][5]} ${currentCurrency}`,
                `Установка имплантата NORIS MEDICAL Ptery Fit (Израиль) — ${prices[currentCurrency][6]} ${currentCurrency}`,
                `Установка слулового имплантата ZYGOMATIC NORIS MEDICAL (Израиль) — ${prices[currentCurrency][7]} ${currentCurrency}`,
            ],
        },
        {
            title: "Протезирование на имплантах",
            items: [
                `Акриловая коронка на имплантат NORIS MEDICAL (Израиль) — ${prices[currentCurrency][8]} ${currentCurrency}`,
                `Акриловая коронка на имплантат OSSTEM (Корея) — ${prices[currentCurrency][9]} ${currentCurrency}`,
                `Акриловая коронка на имплантат MEGAGEN Any One (Корея) — ${prices[currentCurrency][10]} ${currentCurrency}`,
                `Акриловая коронка на имплантат MEGAGEN Any Ridge (Корея) — ${prices[currentCurrency][11]} ${currentCurrency}`,
                `Акриловая коронка на имплантат STRAUMANN SLA (Швейцария) — ${prices[currentCurrency][12]} ${currentCurrency}`,
                `Акриловая коронка на имплантат NEODENT (Швейцария) — ${prices[currentCurrency][13]} ${currentCurrency}`,
                `Безметалловая (Zr) циркониевая коронка с напеканием керамики на имплантат NORIS MEDICAL (Израиль) — ${prices[currentCurrency][14]} ${currentCurrency}`,
                `Безметалловая (Zr) циркониевая коронка с напеканием керамики на имплантат OSSTEM (Корея) — ${prices[currentCurrency][15]} ${currentCurrency}`,
                `Безметалловая (Zr) циркониевая коронка с напеканием керамики на имплантат MEGAGEN (Корея) — ${prices[currentCurrency][16]} ${currentCurrency}`,
                `Безметалловая (Zr) циркониевая коронка с напеканием керамики на имплантат STRAUMANN SLA (Швейцария) — ${prices[currentCurrency][17]} ${currentCurrency}`,
                `Безметалловая (Zr) циркониевая коронка с напеканием керамики на имплантат NEODENT (Швейцария) — ${prices[currentCurrency][18]} ${currentCurrency}`,
            ],
        },
        {
            title: "All-on-4",
            items: [
                `4 имплантата Osstem — ${prices[currentCurrency][19]} ${currentCurrency}`,
                `4 имплантата Mega Gen Any One — ${prices[currentCurrency][20]} ${currentCurrency}`,
                `4 имплантата Mega Gen Any Ridge — ${prices[currentCurrency][21]} ${currentCurrency}`,
                `4 имплантата Straumann SLActive — ${prices[currentCurrency][22]} ${currentCurrency}`,
                `4 имплантата Neodent — ${prices[currentCurrency][23]} ${currentCurrency}`,
            ],
        },
        {
            title: "All-on-6",
            items: [
                `6 имплантатов Noris — ${prices[currentCurrency][24]} ${currentCurrency}`,
                `6 имплантатов Osstem — ${prices[currentCurrency][25]} ${currentCurrency}`,
                `6 имплантатов Mega Gen Any One — ${prices[currentCurrency][26]} ${currentCurrency}`,
                `6 имплантатов Mega Gen Any Ridge — ${prices[currentCurrency][27]} ${currentCurrency}`,
                `6 имплантатов Straumann SLActive — ${prices[currentCurrency][28]} ${currentCurrency}`,
                `6 имплантатов Nеodent — ${prices[currentCurrency][29]} ${currentCurrency}`,
                `4 имплантата Osstem + 2 имплантата Noris — ${prices[currentCurrency][30]} ${currentCurrency}`,
                `4 имплантата Mega Gen Any Ridge + 2 имплантата Noris — ${prices[currentCurrency][31]} ${currentCurrency}`,
                `2 имплантата Osstem + 2 имплантата Noris + 2 имплантат Mega Gen Any Ridge — ${prices[currentCurrency][32]} ${currentCurrency}`,
                `3 Mega Gen Any Ridge + 3 Noris — ${prices[currentCurrency][33]} ${currentCurrency}`,
            ],
        },
        {
            title: "All-on-6 Скуловая",
            items: [
                `2 имплантата Zygomatic + 4 имплантата Mega Gen Any Ridge — ${prices[currentCurrency][34]} ${currentCurrency}`,
                `1 имплантат Zygomatic + 4 имплантата Mega Gen Any Ridge + 1 имплантат Noris — ${prices[currentCurrency][35]} ${currentCurrency}`,
                `2 Zygomatic + 4 Osstem — ${prices[currentCurrency][36]} ${currentCurrency}`,
                `2 Zygomatic + 2 Noris + 2 Mega Gen Any Ridge — ${prices[currentCurrency][36]} ${currentCurrency}`,
                `2 Zygomatic + 2 Noris + 2 Osstem — ${prices[currentCurrency][37]} ${currentCurrency}`,
                `1 Zygomatic + 4 Osstem + 1 Noris — ${prices[currentCurrency][38]} ${currentCurrency}`,
                `3 Zygomatic + 3 Mega Gen Any Ridge — ${prices[currentCurrency][39]} ${currentCurrency}`,

            ],
        },
        {
            title: "Ортопедия",
            items: [
                `Консультация ортопеда с анализом КЛКТ — ${prices[currentCurrency][40]} ${currentCurrency}`,
                `Металлокерамическая — ${prices[currentCurrency][41]} ${currentCurrency}`,
                `Металлокерамический зуб — ${prices[currentCurrency][42]} ${currentCurrency}`,
                `Акриловая(временная) коронка — ${prices[currentCurrency][43]} ${currentCurrency}`,
                `Акриловый(временный) зуб — ${prices[currentCurrency][44]} ${currentCurrency}`,
                `Безметалловая(Zr) циркониевая коронка с керамической облицовкой — ${prices[currentCurrency][45]} ${currentCurrency}`,
                `Безметалловая(Zr) циркониевая коронка полноанатомическая — ${prices[currentCurrency][46]} ${currentCurrency}`,
                `Безметалловый(Zr) циркониевый зуб с керамической облицовкой — ${prices[currentCurrency][47]} ${currentCurrency}`,
                `Безметалловый(Zr) циркониевый зуб полноанатомческий — ${prices[currentCurrency][48]} ${currentCurrency}`,
                `Керамический винир IPS E - max — ${prices[currentCurrency][49]} ${currentCurrency}`,
                `Безметалловая цельнокерамическая коронка E - max — ${prices[currentCurrency][50]} ${currentCurrency}`,
                `Восстановление зуба стекловолокном штифтом — ${prices[currentCurrency][51]} ${currentCurrency}`,
                `Вкладка литая металлическая многокорневая — ${prices[currentCurrency][52]} ${currentCurrency}`,
                `Вкладка безметалловая(Zr) — ${prices[currentCurrency][53]} ${currentCurrency}`,
                `ПСПП — ${prices[currentCurrency][54]} ${currentCurrency}`,
                `ЧСПП — ${prices[currentCurrency][55]} ${currentCurrency}`,
                `ЧСПП с ацеталовыми кламмерами — ${prices[currentCurrency][56]} ${currentCurrency}`,
                `Бюгель(литой) на кламмерной основе — ${prices[currentCurrency][57]} ${currentCurrency}`,
                `Бюгель(литой) на замках(аттачментах) — ${prices[currentCurrency][58]} ${currentCurrency}`,
                `Нейлон(ПСПП) — ${prices[currentCurrency][59]} ${currentCurrency}`,
                `Нейлон(ЧСПП) — ${prices[currentCurrency][60]} ${currentCurrency}`,
                `Ацетал(ЧСПП) — ${prices[currentCurrency][61]} ${currentCurrency}`,
                `Wax - UP — ${prices[currentCurrency][62]} ${currentCurrency}`,
                `Релаксирующая каппа — ${prices[currentCurrency][63]} ${currentCurrency}`,
                `Снятие ортопедической конструкции — ${prices[currentCurrency][64]} ${currentCurrency}`,
                `Фиксация ортопедической конструкции — ${prices[currentCurrency][65]} ${currentCurrency}`,
                `Адгезивная фиксация ортопедической конструкции(винир, коронка) — ${prices[currentCurrency][66]} ${currentCurrency}`,
                `Закрытие шахты винта пломбировочным материалом — ${prices[currentCurrency][67]} ${currentCurrency}`,
                `Закрытие шахты винта пломбировочным материалом + винт — ${prices[currentCurrency][68]} ${currentCurrency}`,
                `Извлечение штифта или вкладки из канала — ${prices[currentCurrency][69]} ${currentCurrency}`,
            ],
        },
        {
            title: "Хирургия",
            items: [
                `Консультация хирурга-имплантолога с анализом КЛКТ — ${prices[currentCurrency][70]} ${currentCurrency}`,
                `Удаление зуба однокорневого — ${prices[currentCurrency][71]} ${currentCurrency}`,
                `Удаление зуба многокорневого — ${prices[currentCurrency][72]} ${currentCurrency}`,
                `Удаление зуба мудрости — ${prices[currentCurrency][73]} ${currentCurrency}`,
                `Удаление ретинированного зуба — ${prices[currentCurrency][74]} ${currentCurrency}`,
                `Аугментация костной ткани — ${prices[currentCurrency][75]} ${currentCurrency}`,
                `Синус-лифтинг (закрытая методика) — ${prices[currentCurrency][76]} ${currentCurrency}`,
                `Синус-лифтинг (открытая методика) — ${prices[currentCurrency][77]} ${currentCurrency}`,
                `Мягкотканая аугментация (пластика десны) — ${prices[currentCurrency][78]} ${currentCurrency}`,
                `Увеличение АО с костным материалом — ${prices[currentCurrency][79]} ${currentCurrency}`,
                `Резекция верхушки корня — ${prices[currentCurrency][80]} ${currentCurrency}`,
                `Цистэктомия однокорневого зуба — ${prices[currentCurrency][81]} ${currentCurrency}`,
                `Цистэктомия многокорневого зуба — ${prices[currentCurrency][82]} ${currentCurrency}`,
                `Цистэктомия с удалением зуба — ${prices[currentCurrency][83]} ${currentCurrency}`,
                `Цистэктомия при корневой кисте — ${prices[currentCurrency][84]} ${currentCurrency}`,
                `Удаление экзостозов костной ткани — ${prices[currentCurrency][85]} ${currentCurrency}`,
                `Удаление имплантата винтового — ${prices[currentCurrency][86]} ${currentCurrency}`,
            ],
        },
        {
            title: "Терапия",
            items: [
                `Консультация терапевта — ${prices[currentCurrency][87]} ${currentCurrency}`,
                `Консультация терапевта с анализом КЛКТ — ${prices[currentCurrency][88]} ${currentCurrency}`,
                `Профессиональная гигиена (Ультразвуковая чистка зубов) — ${prices[currentCurrency][89]} ${currentCurrency}`,
                `Профессиональная гигиена (AIR FLOW) — ${prices[currentCurrency][90]} ${currentCurrency}`,
                `Лечение кариеса (пломба) до 1/3 — ${prices[currentCurrency][91]} ${currentCurrency}`,
                `Лечение кариеса (пломба) до 1/2 — ${prices[currentCurrency][92]} ${currentCurrency}`,
                `Лечение кариеса (пломба) более 1/2 — ${prices[currentCurrency][93]} ${currentCurrency}`,
                `Эстетическая реставрация (ВИНИР) — ${prices[currentCurrency][94]} ${currentCurrency}`,
                `Восстановление культи зуба на стекловолоконном штифте на композите — ${prices[currentCurrency][95]} ${currentCurrency}`,
                `Шинирование зубов стекловолоконной лентой (1 зуб) — ${prices[currentCurrency][96]} ${currentCurrency}`,
                `Адгезивный протез — ${prices[currentCurrency][97]} ${currentCurrency}`,
                `Лечение пульпита (наложение девитализирующей пасты) — ${prices[currentCurrency][98]} ${currentCurrency}`,
                `Эндодонтическое лечение (лечебная паста + без распломбирования) — ${prices[currentCurrency][99]} ${currentCurrency}`,
                `Герметизация фиссур — ${prices[currentCurrency][100]} ${currentCurrency}`,
            ],
        },
        {
            title: "Виниры",
            items: [
                `Керамический винир IPS E-max — ${prices[currentCurrency][101]} ${currentCurrency}`,
                `Эстетическая реставрация (ВИНИР композитный) — ${prices[currentCurrency][102]} ${currentCurrency}`,
            ],
        },
    ];

    return (
        <>
            <Header />
            <h2 className="services-h2">Платные услуги нашего стоматологического центра</h2>

            <CurrencySwitcher />

            <div className="clinic-services">
                {services.map((service, index) => (
                    <div key={index} className="dropdown">
                        <div
                            className="dropdown-header"
                            onClick={() => toggleDropdown(index)}
                        >
                            <span>{service.title}</span>
                            <span
                                className="arrow"
                                style={{
                                    transform: openDropdown === index ? "rotate(180deg)" : "rotate(0deg)",
                                }}
                            >
                                &#9660;
                            </span>
                        </div>
                        <div
                            className={`dropdown-content ${openDropdown === index ? "open" : "closed"
                                }`}
                        >
                            <ul>
                                {service.items.map((item, i) => (
                                    <li key={i}>{item}</li>
                                ))}
                            </ul>
                        </div>
                    </div>
                ))}
            </div>

            <Footer />
        </>
    );
}
