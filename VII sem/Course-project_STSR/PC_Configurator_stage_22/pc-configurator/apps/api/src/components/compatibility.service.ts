import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { In, Repository } from 'typeorm';
import { Component } from './entities/component.entity';

type Level = 'error' | 'warning' | 'info';
type Finding = { level: Level; rule: string; message: string };
type SpecMap = Record<string, string>;

@Injectable()
export class CompatibilityService {
  constructor(@InjectRepository(Component) private readonly components: Repository<Component>) {}

  async check(componentIds: string[]) {
    const uniqueIds = [...new Set(componentIds)];
    const selected = await this.components.find({ where: { id: In(uniqueIds) }, relations: { categoryEntity: true } });
    const missing = uniqueIds.length - selected.length;
    const findings: Finding[] = [];
    if (missing) findings.push({ level: 'error', rule: 'catalog', message: 'Часть комплектующих не найдена в каталоге.' });
    const byCategory = (name: string) => selected.filter((item) => item.category === name);
    const one = (name: string) => byCategory(name)[0];
    const specs = new Map(selected.map((item) => [item.id, this.specifications(item)]));
    const value = (item: Component | undefined, ...keys: string[]) => item ? keys.map((key) => specs.get(item.id)?.[key]).find(Boolean) : undefined;
    const compare = (left: Component | undefined, leftKeys: string[], right: Component | undefined, rightKeys: string[], rule: string, label: string) => {
      if (!left || !right) return;
      const a = value(left, ...leftKeys); const b = value(right, ...rightKeys);
      if (!a || !b) { findings.push({ level: 'warning', rule, message: `Нельзя проверить ${label}: в характеристиках недостаточно данных.` }); return; }
      if (!this.same(a, b)) findings.push({ level: 'error', rule, message: `${label}: «${a}» и «${b}» не совпадают.` });
    };

    const cpu = one('cpu'); const board = one('motherboard'); const ram = byCategory('ram'); const gpu = one('gpu'); const pcCase = one('case'); const cooler = one('cooling'); const psu = one('psu');
    compare(cpu, ['socket'], board, ['socket'], 'cpu-motherboard', 'сокет процессора и материнской платы');
    if (board && ram.length) ram.forEach((module) => compare(module, ['memory_type', 'ram_type', 'type'], board, ['memory_type', 'ram_type'], 'memory', 'тип оперативной памяти и материнской платы'));
    if (board && pcCase) {
      const boardForm = value(board, 'form_factor'); const supported = value(pcCase, 'supported_motherboard_form_factors', 'motherboard_form_factors');
      if (!boardForm || !supported) findings.push({ level: 'warning', rule: 'case-motherboard', message: 'Нельзя проверить форм-фактор корпуса и материнской платы: в характеристиках недостаточно данных.' });
      else if (!this.includes(supported, boardForm)) findings.push({ level: 'error', rule: 'case-motherboard', message: `Корпус не поддерживает форм-фактор материнской платы «${boardForm}».` });
    }
    this.sizeCheck(gpu, ['length', 'gpu_length'], pcCase, ['max_video_card_length'], 'gpu-case', 'Длина видеокарты превышает доступное место в корпусе.', 'длина видеокарты', 'максимальная длина видеокарты у корпуса', findings, value);
    this.sizeCheck(cooler, ['height', 'cooler_height'], pcCase, ['max_cpu_cooler_height'], 'cooler-case', 'Высота кулера превышает допустимую высоту корпуса.', 'высота кулера', 'максимальная высота кулера у корпуса', findings, value);
    if (cpu && cooler) {
      const socket = value(cpu, 'socket'); const supported = value(cooler, 'socket', 'supported_sockets');
      if (!socket || !supported) findings.push({ level: 'warning', rule: 'cpu-cooler', message: 'Нельзя проверить совместимость кулера с сокетом процессора: в характеристиках недостаточно данных.' });
      else if (!this.includes(supported, socket)) findings.push({ level: 'error', rule: 'cpu-cooler', message: `Кулер не заявлен для сокета процессора «${socket}».` });
    }
    if (psu && (cpu || gpu)) {
      const output = Number(psu.powerW ?? this.number(value(psu, 'wattage'))); const required = this.number(value(cpu, 'tdp', 'power')) + this.number(value(gpu, 'tdp', 'power'));
      if (!output || !required) findings.push({ level: 'warning', rule: 'power', message: 'Нельзя надёжно проверить мощность блока питания: отсутствует его мощность или TDP CPU/GPU.' });
      else if (output < required + 100) findings.push({ level: 'error', rule: 'power', message: `Блок питания ${output} Вт недостаточен: CPU и GPU требуют около ${required} Вт, необходим запас не менее 100 Вт.` });
    }
    if (!findings.some((item) => item.level === 'error') && selected.length) findings.push({ level: findings.some((item) => item.level === 'warning') ? 'warning' : 'info', rule: 'summary', message: findings.some((item) => item.level === 'warning') ? 'Критических конфликтов не найдено, но часть проверок требует полных характеристик.' : 'По доступным характеристикам критических конфликтов не найдено.' });
    return { status: findings.some((item) => item.level === 'error') ? 'incompatible' : findings.some((item) => item.level === 'warning') ? 'needs-data' : 'compatible', checkedComponentIds: selected.map((item) => item.id), findings };
  }

  private specifications(item: Component): SpecMap {
    const result: SpecMap = {}; const add = (key: string, raw: unknown) => { if (raw == null || raw === '') return; const normalized = key.toLowerCase().replace(/([a-z])([A-Z])/g, '$1_$2').replace(/[\s-]+/g, '_'); if (typeof raw === 'object') { if (Array.isArray(raw)) result[normalized] = raw.join(', '); else Object.entries(raw as Record<string, unknown>).forEach(([child, value]) => add(child, value)); } else result[normalized] = String(raw).trim(); };
    try { const parsed: unknown = JSON.parse(item.specifications ?? ''); if (parsed && typeof parsed === 'object') Object.entries(parsed as Record<string, unknown>).forEach(([key, raw]) => add(key, raw)); }
    catch { (item.specifications ?? '').split(/\r?\n/).forEach((line) => { const pair = line.split(/[:：]/, 2); if (pair.length === 2) add(pair[0], pair[1]); }); }
    const aliases: Record<string, string> = { 'сокет': 'socket', 'сокет_процессора': 'socket', 'тип_памяти': 'memory_type', 'форм_фактор': 'form_factor', 'длина': 'length', 'высота': 'height', 'мощность': 'wattage', 'теплопакет': 'tdp', 'энергопотребление': 'tdp', 'максимальная_длина_видеокарты': 'max_video_card_length', 'максимальная_высота_кулера': 'max_cpu_cooler_height', 'поддерживаемые_платы': 'supported_motherboard_form_factors', 'совместимые_сокеты': 'supported_sockets' };
    Object.entries(aliases).forEach(([from, to]) => { if (result[from] && !result[to]) result[to] = result[from]; }); return result;
  }
  private same(a: string, b: string) { return this.clean(a) === this.clean(b); }
  private includes(list: string, value: string) { return list.split(/[,;/|]/).some((item) => this.clean(item) === this.clean(value)); }
  private clean(value: string) { return value.toLowerCase().replace(/[^a-zа-я0-9]/gi, ''); }
  private number(value: string | undefined) { const match = value?.replace(',', '.').match(/\d+(?:\.\d+)?/); return match ? Number(match[0]) : 0; }
  private sizeCheck(a: Component | undefined, aKeys: string[], b: Component | undefined, bKeys: string[], rule: string, message: string, actualLabel: string, maximumLabel: string, findings: Finding[], value: (item: Component | undefined, ...keys: string[]) => string | undefined) { if (!a || !b) return; const actual = this.number(value(a, ...aKeys)); const maximum = this.number(value(b, ...bKeys)); if (!actual || !maximum) { const missing = [!actual && actualLabel, !maximum && maximumLabel].filter(Boolean).join(' и '); findings.push({ level: 'warning', rule, message: `Нельзя проверить габариты: не указана ${missing}.` }); } else if (actual > maximum) findings.push({ level: 'error', rule, message: `${message} (${actual} мм > ${maximum} мм)` }); }
}
