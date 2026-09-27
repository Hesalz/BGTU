<<<<<<< HEAD
// npm install xlsx chartjs-node-canvas fs
const fs = require('fs');
const { ChartJSNodeCanvas } = require('chartjs-node-canvas');
const xlsx = require('xlsx');

const width = 800;
const height = 600;
const chartJSNodeCanvas = new ChartJSNodeCanvas({ width, height });

// --- Методы стеганографии ---
const zeroWidth = { '0': '\u200B', '1': '\u200C' };

function encodeSpaceStego(text, secret) {
    const binary = Array.from(secret).map(c => c.charCodeAt(0).toString(2).padStart(8,'0')).join('');
    const words = text.split(' ');
    let stegoText = words[0];
    for(let i=0;i<binary.length && i+1<words.length;i++){
        stegoText += binary[i]==='0'?' '+words[i+1]:'  '+words[i+1];
    }
    stegoText += ' '+words.slice(binary.length+1).join(' ');
    return stegoText;
}

function encodeZeroWidth(text, secret) {
    const binary = Array.from(secret).map(c=>c.charCodeAt(0).toString(2).padStart(8,'0')).join('');
    let stegoText = text;
    for(let bit of binary) stegoText += zeroWidth[bit];
    return stegoText;
}

// --- Метрики сравнения ---
// Степень заметности: нормализуем кол-во изменений относительно длины текста
function noticeableChanges(original, stego, method) {
    const textLen = original.length;
    if (method === 'Space') {
        const extraSpaces = (stego.match(/  /g) || []).length;
        return extraSpaces / textLen * 100; // % заметности
    } else {
        const zeroWidths = (stego.match(/[\u200B\u200C]/g) || []).length;
        return zeroWidths / textLen * 100; // % заметности
    }
}

// Качество текста: нормализованное расстояние Левенштейна
function levenshtein(a, b) {
    const matrix = Array.from({ length: a.length + 1 }, (_, i) => Array(b.length + 1).fill(0));
    for (let i = 0; i <= a.length; i++) matrix[i][0] = i;
    for (let j = 0; j <= b.length; j++) matrix[0][j] = j;
    for (let i = 1; i <= a.length; i++) {
        for (let j = 1; j <= b.length; j++) {
            if (a[i - 1] === b[j - 1]) matrix[i][j] = matrix[i - 1][j - 1];
            else matrix[i][j] = Math.min(
                matrix[i - 1][j] + 1,
                matrix[i][j - 1] + 1,
                matrix[i - 1][j - 1] + 1
            );
        }
    }
    return matrix[a.length][b.length];
}
function textQuality(original, stego) {
    return (levenshtein(original, stego) / original.length) * 100; // % искажения
}

// Безопасность: вероятность обнаружения как функция заметности + случайный шум (имитация анализа)
function detectability(score) {
    // Чем выше заметность, тем выше вероятность обнаружения
    const base = Math.min(score, 100);
    const noise = Math.random() * 10; // добавим случайность
    return Math.min(base + noise, 100);
}

// --- Генерация тестовых данных ---
const baseText = "Это тестовый текст для анализа методов стеганографии, который будет использоваться для экспериментов.";
const messageLengths = [2, 4, 6, 8, 10, 20, 30, 40];
let results = [];

for (let len of messageLengths) {
    const secret = "A".repeat(len);

    const stegoSpace = encodeSpaceStego(baseText, secret);
    const stegoZW = encodeZeroWidth(baseText, secret);

    const noticeableSpace = noticeableChanges(baseText, stegoSpace, 'Space');
    const noticeableZW = noticeableChanges(baseText, stegoZW, 'ZW');

    const qualitySpace = textQuality(baseText, stegoSpace);
    const qualityZW = textQuality(baseText, stegoZW);

    const detectSpace = detectability(noticeableSpace);
    const detectZW = detectability(noticeableZW);

    results.push({
        length: len,
        noticeableSpace,
        noticeableZW,
        qualitySpace,
        qualityZW,
        detectSpace,
        detectZW
    });
}

// --- Создание Excel файла ---
const wb = xlsx.utils.book_new();
const ws_data = [[
    "Message Length",
    "Space: Noticeability (%)",
    "ZeroWidth: Noticeability (%)",
    "Space: Quality Loss (%)",
    "ZeroWidth: Quality Loss (%)",
    "Space: Detectability (%)",
    "ZeroWidth: Detectability (%)"
]];
results.forEach(r=>ws_data.push([
    r.length,
    r.noticeableSpace.toFixed(2),
    r.noticeableZW.toFixed(2),
    r.qualitySpace.toFixed(2),
    r.qualityZW.toFixed(2),
    r.detectSpace.toFixed(2),
    r.detectZW.toFixed(2)
]));
const ws = xlsx.utils.aoa_to_sheet(ws_data);
xlsx.utils.book_append_sheet(wb, ws, "StegoAnalysis");
xlsx.writeFile(wb, "stego_comparison.xlsx");

// --- Генерация графиков ---
(async () => {
    const charts = [
        { metric: 'noticeable', label: 'Степень заметности', yTitle: 'Заметность (%)', fields: ['noticeableSpace', 'noticeableZW'] },
        { metric: 'quality', label: 'Качество текста', yTitle: 'Искажение текста (%)', fields: ['qualitySpace', 'qualityZW'] },
        { metric: 'detect', label: 'Безопасность', yTitle: 'Вероятность обнаружения (%)', fields: ['detectSpace', 'detectZW'] }
    ];

    for (let chart of charts) {
        const configuration = {
            type: 'line',
            data: {
                labels: results.map(r => r.length),
                datasets: [
                    { label: 'Space Stego', data: results.map(r => r[chart.fields[0]]), borderColor: 'red', fill: false },
                    { label: 'ZeroWidth Stego', data: results.map(r => r[chart.fields[1]]), borderColor: 'blue', fill: false }
                ]
            },
            options: {
                plugins: { title: { display: true, text: chart.label } },
                scales: {
                    x: { title: { display: true, text: 'Длина секретного сообщения' } },
                    y: { title: { display: true, text: chart.yTitle, beginAtZero: true, max: 110 } }
                }
            }
        };

        const image = await chartJSNodeCanvas.renderToBuffer(configuration);
        fs.writeFileSync(`${chart.metric}_chart.png`, image);
    }
=======
// npm install xlsx chartjs-node-canvas fs
const fs = require('fs');
const { ChartJSNodeCanvas } = require('chartjs-node-canvas');
const xlsx = require('xlsx');

const width = 800;
const height = 600;
const chartJSNodeCanvas = new ChartJSNodeCanvas({ width, height });

// --- Методы стеганографии ---
const zeroWidth = { '0': '\u200B', '1': '\u200C' };

function encodeSpaceStego(text, secret) {
    const binary = Array.from(secret).map(c => c.charCodeAt(0).toString(2).padStart(8,'0')).join('');
    const words = text.split(' ');
    let stegoText = words[0];
    for(let i=0;i<binary.length && i+1<words.length;i++){
        stegoText += binary[i]==='0'?' '+words[i+1]:'  '+words[i+1];
    }
    stegoText += ' '+words.slice(binary.length+1).join(' ');
    return stegoText;
}

function encodeZeroWidth(text, secret) {
    const binary = Array.from(secret).map(c=>c.charCodeAt(0).toString(2).padStart(8,'0')).join('');
    let stegoText = text;
    for(let bit of binary) stegoText += zeroWidth[bit];
    return stegoText;
}

// --- Метрики сравнения ---
// Степень заметности: нормализуем кол-во изменений относительно длины текста
function noticeableChanges(original, stego, method) {
    const textLen = original.length;
    if (method === 'Space') {
        const extraSpaces = (stego.match(/  /g) || []).length;
        return extraSpaces / textLen * 100; // % заметности
    } else {
        const zeroWidths = (stego.match(/[\u200B\u200C]/g) || []).length;
        return zeroWidths / textLen * 100; // % заметности
    }
}

// Качество текста: нормализованное расстояние Левенштейна
function levenshtein(a, b) {
    const matrix = Array.from({ length: a.length + 1 }, (_, i) => Array(b.length + 1).fill(0));
    for (let i = 0; i <= a.length; i++) matrix[i][0] = i;
    for (let j = 0; j <= b.length; j++) matrix[0][j] = j;
    for (let i = 1; i <= a.length; i++) {
        for (let j = 1; j <= b.length; j++) {
            if (a[i - 1] === b[j - 1]) matrix[i][j] = matrix[i - 1][j - 1];
            else matrix[i][j] = Math.min(
                matrix[i - 1][j] + 1,
                matrix[i][j - 1] + 1,
                matrix[i - 1][j - 1] + 1
            );
        }
    }
    return matrix[a.length][b.length];
}
function textQuality(original, stego) {
    return (levenshtein(original, stego) / original.length) * 100; // % искажения
}

// Безопасность: вероятность обнаружения как функция заметности + случайный шум (имитация анализа)
function detectability(score) {
    // Чем выше заметность, тем выше вероятность обнаружения
    const base = Math.min(score, 100);
    const noise = Math.random() * 10; // добавим случайность
    return Math.min(base + noise, 100);
}

// --- Генерация тестовых данных ---
const baseText = "Это тестовый текст для анализа методов стеганографии, который будет использоваться для экспериментов.";
const messageLengths = [2, 4, 6, 8, 10, 20, 30, 40];
let results = [];

for (let len of messageLengths) {
    const secret = "A".repeat(len);

    const stegoSpace = encodeSpaceStego(baseText, secret);
    const stegoZW = encodeZeroWidth(baseText, secret);

    const noticeableSpace = noticeableChanges(baseText, stegoSpace, 'Space');
    const noticeableZW = noticeableChanges(baseText, stegoZW, 'ZW');

    const qualitySpace = textQuality(baseText, stegoSpace);
    const qualityZW = textQuality(baseText, stegoZW);

    const detectSpace = detectability(noticeableSpace);
    const detectZW = detectability(noticeableZW);

    results.push({
        length: len,
        noticeableSpace,
        noticeableZW,
        qualitySpace,
        qualityZW,
        detectSpace,
        detectZW
    });
}

// --- Создание Excel файла ---
const wb = xlsx.utils.book_new();
const ws_data = [[
    "Message Length",
    "Space: Noticeability (%)",
    "ZeroWidth: Noticeability (%)",
    "Space: Quality Loss (%)",
    "ZeroWidth: Quality Loss (%)",
    "Space: Detectability (%)",
    "ZeroWidth: Detectability (%)"
]];
results.forEach(r=>ws_data.push([
    r.length,
    r.noticeableSpace.toFixed(2),
    r.noticeableZW.toFixed(2),
    r.qualitySpace.toFixed(2),
    r.qualityZW.toFixed(2),
    r.detectSpace.toFixed(2),
    r.detectZW.toFixed(2)
]));
const ws = xlsx.utils.aoa_to_sheet(ws_data);
xlsx.utils.book_append_sheet(wb, ws, "StegoAnalysis");
xlsx.writeFile(wb, "stego_comparison.xlsx");

// --- Генерация графиков ---
(async () => {
    const charts = [
        { metric: 'noticeable', label: 'Степень заметности', yTitle: 'Заметность (%)', fields: ['noticeableSpace', 'noticeableZW'] },
        { metric: 'quality', label: 'Качество текста', yTitle: 'Искажение текста (%)', fields: ['qualitySpace', 'qualityZW'] },
        { metric: 'detect', label: 'Безопасность', yTitle: 'Вероятность обнаружения (%)', fields: ['detectSpace', 'detectZW'] }
    ];

    for (let chart of charts) {
        const configuration = {
            type: 'line',
            data: {
                labels: results.map(r => r.length),
                datasets: [
                    { label: 'Space Stego', data: results.map(r => r[chart.fields[0]]), borderColor: 'red', fill: false },
                    { label: 'ZeroWidth Stego', data: results.map(r => r[chart.fields[1]]), borderColor: 'blue', fill: false }
                ]
            },
            options: {
                plugins: { title: { display: true, text: chart.label } },
                scales: {
                    x: { title: { display: true, text: 'Длина секретного сообщения' } },
                    y: { title: { display: true, text: chart.yTitle, beginAtZero: true, max: 110 } }
                }
            }
        };

        const image = await chartJSNodeCanvas.renderToBuffer(configuration);
        fs.writeFileSync(`${chart.metric}_chart.png`, image);
    }
>>>>>>> bc8717460893f29e21da2be7d527854345ffee6b
})();