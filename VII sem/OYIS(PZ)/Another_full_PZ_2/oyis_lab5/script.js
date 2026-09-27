<<<<<<< HEAD
const fs = require('fs');
const { performance } = require('perf_hooks');
const ExcelJS = require('exceljs');
const { ChartJSNodeCanvas } = require('chartjs-node-canvas');

const SpaceSteg = {
  encode: (cover, secret) => {
    const bits = secret.split('').map(c => c.charCodeAt(0).toString(2).padStart(8, '0')).join('');
    let out = '';
    let bi = 0;
    for (let i = 0; i < cover.length; i++) {
      if (cover[i] === ' ' && bi < bits.length) {
        out += bits[bi] === '1' ? '  ' : ' ';
        bi++;
      } else {
        out += cover[i];
      }
    }
    return out;
  },
  decode: (text) => {
    let bits = '';
    for (let i = 0; i < text.length; i++) {
      if (text[i] === ' ') {
        let count = 1;
        while (text[i + 1] === ' ') { count++; i++; }
        bits += (count > 1 ? '1' : '0');
      }
    }
    return bits.match(/.{8}/g)?.map(b => String.fromCharCode(parseInt(b, 2))).join('') || '';
  }
};

const ZWSteg = {
  encode: (cover, secret) => {
    const bits = secret.split('').map(c => c.charCodeAt(0).toString(2).padStart(8, '0')).join('');
    let encoded = cover;
    for (let b of bits) {
      encoded += b === '1' ? '\u200B' : '\u200C';
    }
    return encoded;
  },
  decode: (text) => {
    const bits = [...text].filter(ch => ch === '\u200B' || ch === '\u200C').map(ch => ch === '\u200B' ? '1' : '0').join('');
    return bits.match(/.{8}/g)?.map(b => String.fromCharCode(parseInt(b, 2))).join('') || '';
  }
};

const coverSamples = {
  eng: "The quick brown fox jumps over the lazy dog.",
  rus: "Съешь же ещё этих мягких французских булок, да выпей чаю.",
  html: "<p>Hello world! Welcome to <b>our</b> site.</p>",
  code: "function test(){ return 'ok'; }"
};

const messages = ["hi", "secret", "оченьдлинноесообщение123456", "LoremIpsumDolorSitAmet"];

const experiments = [];

function runExperiment(methodName, method, cover, secret) {
  const startEnc = performance.now();
  const encoded = method.encode(cover, secret);
  const encTime = performance.now() - startEnc;

  const startDec = performance.now();
  const decoded = method.decode(encoded);
  const decTime = performance.now() - startDec;

  return {
    method: methodName,
    coverLength: cover.length,
    secretLength: secret.length,
    encTime: encTime.toFixed(3),
    decTime: decTime.toFixed(3),
    success: decoded === secret
  };
}

for (let coverKey in coverSamples) {
  for (let msg of messages) {
    experiments.push(runExperiment("Space", SpaceSteg, coverSamples[coverKey], msg));
    experiments.push(runExperiment("ZeroWidth", ZWSteg, coverSamples[coverKey], msg));
  }
}

(async () => {
  const workbook = new ExcelJS.Workbook();
  const sheet = workbook.addWorksheet("Results");

  sheet.columns = [
    { header: 'Method', key: 'method' },
    { header: 'CoverLength', key: 'coverLength' },
    { header: 'SecretLength', key: 'secretLength' },
    { header: 'EncTime(ms)', key: 'encTime' },
    { header: 'DecTime(ms)', key: 'decTime' },
    { header: 'Success', key: 'success' }
  ];

  experiments.forEach(e => sheet.addRow(e));

  await workbook.xlsx.writeFile("steg_analysis.xlsx");
  console.log("Excel сохранён: steg_analysis.xlsx");

  const chartJSNodeCanvas = new ChartJSNodeCanvas({ width: 800, height: 600 });

  const methods = [...new Set(experiments.map(e => e.method))];
  for (let metric of ["encTime", "decTime"]) {
    const data = methods.map(m => {
      return experiments.filter(e => e.method === m).reduce((sum, e) => sum + parseFloat(e[metric]), 0) / experiments.filter(e => e.method === m).length;
    });

    const config = {
      type: 'bar',
      data: {
        labels: methods,
        datasets: [{ label: metric + " avg", data }]
      }
    };

    const img = await chartJSNodeCanvas.renderToBuffer(config);
    fs.writeFileSync(metric + ".png", img);
    console.log("График сохранён:", metric + ".png");
  }
})();
=======
const fs = require('fs');
const { performance } = require('perf_hooks');
const ExcelJS = require('exceljs');
const { ChartJSNodeCanvas } = require('chartjs-node-canvas');

const SpaceSteg = {
  encode: (cover, secret) => {
    const bits = secret.split('').map(c => c.charCodeAt(0).toString(2).padStart(8, '0')).join('');
    let out = '';
    let bi = 0;
    for (let i = 0; i < cover.length; i++) {
      if (cover[i] === ' ' && bi < bits.length) {
        out += bits[bi] === '1' ? '  ' : ' ';
        bi++;
      } else {
        out += cover[i];
      }
    }
    return out;
  },
  decode: (text) => {
    let bits = '';
    for (let i = 0; i < text.length; i++) {
      if (text[i] === ' ') {
        let count = 1;
        while (text[i + 1] === ' ') { count++; i++; }
        bits += (count > 1 ? '1' : '0');
      }
    }
    return bits.match(/.{8}/g)?.map(b => String.fromCharCode(parseInt(b, 2))).join('') || '';
  }
};

const ZWSteg = {
  encode: (cover, secret) => {
    const bits = secret.split('').map(c => c.charCodeAt(0).toString(2).padStart(8, '0')).join('');
    let encoded = cover;
    for (let b of bits) {
      encoded += b === '1' ? '\u200B' : '\u200C';
    }
    return encoded;
  },
  decode: (text) => {
    const bits = [...text].filter(ch => ch === '\u200B' || ch === '\u200C').map(ch => ch === '\u200B' ? '1' : '0').join('');
    return bits.match(/.{8}/g)?.map(b => String.fromCharCode(parseInt(b, 2))).join('') || '';
  }
};

const coverSamples = {
  eng: "The quick brown fox jumps over the lazy dog.",
  rus: "Съешь же ещё этих мягких французских булок, да выпей чаю.",
  html: "<p>Hello world! Welcome to <b>our</b> site.</p>",
  code: "function test(){ return 'ok'; }"
};

const messages = ["hi", "secret", "оченьдлинноесообщение123456", "LoremIpsumDolorSitAmet"];

const experiments = [];

function runExperiment(methodName, method, cover, secret) {
  const startEnc = performance.now();
  const encoded = method.encode(cover, secret);
  const encTime = performance.now() - startEnc;

  const startDec = performance.now();
  const decoded = method.decode(encoded);
  const decTime = performance.now() - startDec;

  return {
    method: methodName,
    coverLength: cover.length,
    secretLength: secret.length,
    encTime: encTime.toFixed(3),
    decTime: decTime.toFixed(3),
    success: decoded === secret
  };
}

for (let coverKey in coverSamples) {
  for (let msg of messages) {
    experiments.push(runExperiment("Space", SpaceSteg, coverSamples[coverKey], msg));
    experiments.push(runExperiment("ZeroWidth", ZWSteg, coverSamples[coverKey], msg));
  }
}

(async () => {
  const workbook = new ExcelJS.Workbook();
  const sheet = workbook.addWorksheet("Results");

  sheet.columns = [
    { header: 'Method', key: 'method' },
    { header: 'CoverLength', key: 'coverLength' },
    { header: 'SecretLength', key: 'secretLength' },
    { header: 'EncTime(ms)', key: 'encTime' },
    { header: 'DecTime(ms)', key: 'decTime' },
    { header: 'Success', key: 'success' }
  ];

  experiments.forEach(e => sheet.addRow(e));

  await workbook.xlsx.writeFile("steg_analysis.xlsx");
  console.log("Excel сохранён: steg_analysis.xlsx");

  const chartJSNodeCanvas = new ChartJSNodeCanvas({ width: 800, height: 600 });

  const methods = [...new Set(experiments.map(e => e.method))];
  for (let metric of ["encTime", "decTime"]) {
    const data = methods.map(m => {
      return experiments.filter(e => e.method === m).reduce((sum, e) => sum + parseFloat(e[metric]), 0) / experiments.filter(e => e.method === m).length;
    });

    const config = {
      type: 'bar',
      data: {
        labels: methods,
        datasets: [{ label: metric + " avg", data }]
      }
    };

    const img = await chartJSNodeCanvas.renderToBuffer(config);
    fs.writeFileSync(metric + ".png", img);
    console.log("График сохранён:", metric + ".png");
  }
})();
>>>>>>> bc8717460893f29e21da2be7d527854345ffee6b
