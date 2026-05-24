var knowledge = [
    ["эвакуатор", "является", "специальным транспортным средством для буксировки и перевозки неисправных или повреждённых автомобилей"],
    ["эвакуатор", "предназначен", "для транспортировки автомобилей, которые не могут двигаться самостоятельно"],
    ["эвакуатор", "используется", "для эвакуации повреждённых в ДТП машин и неправильно припаркованных автомобилей"],
    ["эвакуатор", "состоит из", "шасси, выдвижной платформы, гидравлической системы, лебёдки, системы крепления и световой сигнализации"],
    ["эвакуатор", "работает", "с помощью гидравлической системы, которая выдвигает и наклоняет платформу, и лебёдки, которая затягивает автомобиль"],
    ["эвакуатор", "выглядит", "как грузовой автомобиль с гидравлической платформой, лебёдкой и проблесковыми маячками"],
    ["эвакуатор", "оснащается", "проблесковыми маячками для безопасности на дороге"],
    ["эвакуатор", "называется", "также эвакуационной машиной или tow truck"],
    ["первый эвакуатор", "был создан", "в 1916 году Эрнестом Холмсом-старшим в городе Чаттануга, США"],
    ["эрнест холмс-старший", "создал", "первый в мире эвакуатор в 1916 году"],
    ["компания holmes", "производит", "эвакуаторы с 1916 года до настоящего времени"],
    ["типы эвакуаторов", "бывают", "с частичной погрузкой, с полной погрузкой (платформенные), с манипулятором и тяжёлые эвакуаторы"],
    ["эвакуатор с частичной погрузкой", "является", "самым распространённым типом эвакуатора"],
    ["эвакуатор с частичной погрузкой", "работает", "путём подъёма автомобиля за переднюю или заднюю ось и буксировки в полуподвешенном состоянии"],
    ["эвакуатор с частичной погрузкой", "подходит для", "легковых автомобилей на короткие и средние расстояния"],
    ["эвакуатор с частичной погрузкой", "не подходит для", "полноприводных автомобилей"],
    ["эвакуатор с полной погрузкой", "называется", "также платформенным эвакуатором"],
    ["эвакуатор с полной погрузкой", "является", "наиболее безопасным способом транспортировки автомобилей"],
    ["эвакуатор с полной погрузкой", "работает", "путём загрузки автомобиля на платформу с помощью лебёдки или гидравлического привода"],
    ["эвакуатор с полной погрузкой", "подходит для", "полноприводных автомобилей, спортивных машин и автомобилей после серьёзных аварий"],
    ["эвакуатор с манипулятором", "оснащён", "гидравлической стрелой-манипулятором, которая может вращаться на 360 градусов"],
    ["эвакуатор с манипулятором", "используется для", "извлечения автомобилей из кюветов, оврагов и водоёмов"],
    ["тяжёлый эвакуатор", "предназначен для", "транспортировки грузовиков, автобусов и спецтехники"],
    ["тяжёлый эвакуатор", "имеет грузоподъёмность", "от 50 до 100 тонн"],
    ["лебёдка", "является", "устройством с тросом для затягивания автомобиля на платформу"],
    ["лебёдка", "выглядит", "как барабан с намотанным тросом, приводимый в движение электродвигателем"],
    ["лебёдка", "нужна", "для затягивания автомобиля на платформу при эвакуации"],
    ["лебёдка", "предназначена", "для затягивания автомобиля на платформу"],
    ["лебёдка", "используется", "для затягивания автомобиля на платформу эвакуатора"],
    ["лебёдка с электроприводом", "затягивает", "автомобиль на платформу с помощью троса"],
    ["лебёдка", "работает", "путём наматывания троса на барабан, что создаёт тяговое усилие"],
    ["проблесковые маячки", "являются", "световыми сигнальными устройствами оранжевого или синего цвета"],
    ["проблесковые маячки", "выглядят", "как мигающие лампы на крыше эвакуатора"],
    ["проблесковые маячки", "нужны", "для предупреждения других водителей при проведении эвакуации"],
    ["проблесковые маячки", "используются", "для обозначения аварийного транспортного средства на дороге"],
    ["гидравлическая система", "является", "системой, использующей давление масла для перемещения платформы и лебёдки"],
    ["гидравлическая система", "состоит из", "маслостанции, гидроцилиндров, распределителей и шлангов высокого давления"],
    ["гидравлическая система", "работает", "за счёт давления масла, которое подаётся от маслостанции к гидроцилиндрам"],
    ["гидравлическая система", "нужна", "для подъёма, опускания и выдвижения платформы"],
    ["выдвижная платформа", "является", "грузовой площадкой с гидроприводом"],
    ["выдвижная платформа", "выглядит", "как металлическая площадка, которая может выдвигаться назад и наклоняться"],
    ["выдвижная платформа", "работает", "с помощью гидравлических цилиндров, которые выдвигают и наклоняют её"],
    ["выдвижная платформа", "предназначена", "для размещения транспортируемого автомобиля"],
    ["шасси", "является", "грузовой основой эвакуатора с усиленной рамой"],
    ["шасси", "выглядит", "как прочная металлическая несущая конструкция"],
    ["шасси", "объединяет", "все узлы и агрегаты эвакуатора в единое целое"],
    ["система крепления", "нужна", "для надёжной фиксации автомобиля на платформе"],
    ["система крепления", "состоит из", "ремней, цепей и замков"],
    ["автоматические системы крепления", "быстро фиксируют", "автомобиль без участия оператора"],
    ["стабилизаторы", "обеспечивают", "устойчивость эвакуатора во время погрузки"],
    ["стабилизаторы", "опускаются", "перед началом погрузки автомобиля"],
    ["платформа", "гидравлически сдвигается", "назад и наклоняется для образования пологого въезда"],
    ["автомобиль", "затягивается", "лебёдкой на платформу"],
    ["платформа", "возвращается", "в горизонтальное положение после загрузки автомобиля"],
    ["платформа", "фиксируется", "в транспортном положении"],
    ["стабилизаторы", "опускаются", "для обеспечения устойчивости во время погрузки"],
    ["система креплений", "фиксирует", "транспортируемый автомобиль"],
    ["системы дистанционного управления", "позволяют", "оператору управлять лебёдкой и платформой с безопасного расстояния"],
    ["компьютеризированные системы контроля нагрузки", "предотвращают", "опрокидывание эвакуатора"],
    ["gps-навигация", "оптимизирует", "маршруты эвакуатора"],
    ["телематика", "отслеживает", "местоположение эвакуаторов"],
    ["сенсоры приближения", "предотвращают", "повреждение автомобиля при заезде на платформу"],
    ["miller industries", "является", "крупнейшим мировым производителем эвакуаторов"],
    ["miller industries", "включает", "бренды Century, Vulcan, Chevron и Holmes"],
    ["jerr-dan", "известен", "инновационными решениями в области эвакуационной техники"],
    ["kässbohrer", "производит", "тяжёлые эвакуаторы для грузового транспорта"],
    ["nrc", "специализируется", "на эвакуаторах с роторным механизмом"],
    ["бонаум", "производит", "эвакуаторы на базе шасси MAN, Mercedes, Iveco"],
    ["тонар", "является", "российским производителем эвакуаторов"],
    ["имитационное моделирование", "является", "методом исследования, при котором система заменяется моделью"],
    ["имитационное моделирование", "используется", "когда невозможно или дорого экспериментировать на реальном объекте"],
    ["имитационное моделирование", "позволяет", "имитировать поведение системы во времени"],
    ["симулятор эвакуатора", "является", "примером имитационной модели для обучения"],
    ["имитационная модель", "позволяет имитировать", "поведение системы во времени"],
    ["временем в модели", "можно управлять", "замедляя или ускоряя его"],
    ["цель имитационного моделирования", "состоит в", "воспроизведении поведения исследуемой системы"],
    ["эвакуатор", "применяется для", "аварийной эвакуации после ДТП, перемещения на штрафстоянку и технической помощи"],
    ["аварийная эвакуация", "транспортирует", "повреждённые в ДТП автомобили"],
    ["штрафстоянка", "перемещает", "неправильно припаркованные автомобили"],
    ["техническая помощь", "буксирует", "автомобили с неисправностями"],
    ["эвакуаторы", "используются", "на дорогах, автострадах, в городах и на трассах"],
    ["лёгкий эвакуатор", "имеет грузоподъёмность", "до 2,5 тонн"],
    ["средний эвакуатор", "имеет грузоподъёмность", "от 2,5 до 5 тонн"],
    ["тяжёлый эвакуатор тип ц", "имеет грузоподъёмность", "от 5 до 15 тонн"],
    ["сверхтяжёлый эвакуатор", "имеет грузоподъёмность", "от 15 до 50 тонн"],
    ["длина платформы лёгкого эвакуатора", "составляет", "4-5 метров"],
    ["длина платформы среднего эвакуатора", "составляет", "5-6 метров"],
    ["длина платформы тяжёлого эвакуатора", "составляет", "6-8 метров"],
    ["длина платформы сверхтяжёлого эвакуатора", "составляет", "8-12 метров"],
    ["отличие эвакуатора с частичной погрузкой от эвакуатора с полной погрузкой", "заключается в", "способе транспортировки: при частичной погрузке автомобиль едет на двух колёсах, при полной — стоит на платформе"],
    ["эвакуатор с полной погрузкой", "безопаснее", "эвакуатора с частичной погрузкой, так как автомобиль полностью закреплён на платформе"],
    ["эвакуатор с манипулятором", "отличается от", "других типов наличием гидравлической стрелы, вращающейся на 360 градусов"],
    ["лебёдка", "нужна для", "затягивания автомобиля на платформу"],
    ["проблесковые маячки", "нужны для", "предупреждения других водителей при проведении эвакуации"],
    ["гидравлическая система", "нужна для", "перемещения платформы и работы лебёдки"],
    ["эвакуатор", "нужен для", "транспортировки неисправных и повреждённых автомобилей"],
    ["стабилизаторы", "нужны для", "обеспечения устойчивости эвакуатора во время погрузки"],
    ["эвакуатор", "это", "специальное транспортное средство для буксировки и перевозки неисправных автомобилей"],
    ["лебёдка", "это", "устройство с тросом для затягивания автомобиля на платформу"],
    ["гидравлическая система", "это", "система, использующая давление масла для перемещения механизмов"],
    ["выдвижная платформа", "это", "грузовая площадка с гидроприводом"],
    ["проблесковые маячки", "это", "световые сигнальные устройства оранжевого или синего цвета"],
    ["шасси", "это", "грузовая основа эвакуатора с усиленной рамой"],
    ["имитационное моделирование", "это", "метод исследования, при котором система заменяется модельой"],
    ["эвакуатор", "работает как", "гидравлическая система выдвигает платформу, лебёдка затягивает автомобиль"],
    ["лебёдка", "работает как", "электродвигатель вращает барабан, наматывая трос"],
    ["гидравлическая система", "работает как", "маслостанция создаёт давление, которое перемещает гидроцилиндры"],
    ["эвакуатор", "состоит из", "шасси, платформы, гидравлики, лебёдки, креплений и маячков"],
    ["гидравлическая система", "состоит из", "маслостанции, гидроцилиндров, распределителей и шлангов"],
    ["эвакуатор с манипулятором", "используется", "в местах, где автомобиль невозможно загрузить обычным способом: в кюветах, тесных дворах и после серьёзных ДТП"],
    ["эвакуатор с частичной погрузкой", "отличается от эвакуатора с полной погрузкой", "тем, что транспортирует автомобиль на двух колёсах, а не полностью на платформе"],
    ["эвакуатор", "нужен", "для транспортировки неисправных, повреждённых или неправильно припаркованных автомобилей"],
    ["проблесковые маячки", "нужны для", "предупреждения других участников движения и повышения безопасности при эвакуации"],
    ["лебёдка", "нужна для", "затягивания автомобиля на платформу эвакуатора"],
    ["эвакуаторы", "используются", "на дорогах, автострадах, в городах и на трассах"],
    ["первый эвакуатор", "был создан", "в 1916 году Эрнестом Холмсом-старшим в городе Чаттануга, США"],
    ["эвакуатор с частичной погрузкой", "не подходит для", "полноприводных автомобилей"],
    ["полноприводные автомобили", "не рекомендуется перевозить", "эвакуатором с частичной погрузкой"],
    ["эвакуатор с полной погрузкой", "считается безопаснее", "потому что автомобиль полностью закреплён на платформе"],
    ["проблесковые маячки", "повышают", "безопасность дорожного движения"],
    ["стабилизаторы", "предотвращают", "опрокидывание эвакуатора"],
    ["сенсоры приближения", "предотвращают", "повреждение автомобиля при погрузке"],
    ["лёгкий эвакуатор", "отличается от среднего", "меньшей грузоподъёмностью и длиной платформы"],
    ["эвакуатор с манипулятором", "отличается от платформенного", "наличием гидравлической стрелы"],
    ["тяжёлый эвакуатор", "отличается от лёгкого", "возможностью перевозки грузовиков и автобусов"],
    ["эвакуатор с полной погрузкой", "имеет преимущество", "высокой безопасности перевозки"],
    ["эвакуатор с частичной погрузкой", "имеет преимущество", "быстрой загрузки автомобиля"],
    ["эвакуатор с частичной погрузкой", "имеет недостаток", "неподходящести для полного привода"],
    ["эвакуатор с манипулятором", "имеет преимущество", "возможности подъёма автомобиля в труднодоступных местах"],
    ["оператор эвакуатора", "обязан", "проверять фиксацию автомобиля"],
    ["автомобиль", "должен быть", "надёжно закреплён на платформе"],
    ["эвакуатор", "требует", "регулярного технического обслуживания"],
    ["лебёдка", "требует", "проверки состояния троса"],
    ["эвакуатор с манипулятором", "применяется", "в тесных дворах и после серьёзных ДТП"],
    ["тяжёлый эвакуатор", "применяется", "для перевозки грузовиков и автобусов"],
    ["эвакуатор с полной погрузкой", "применяется", "для спортивных и дорогих автомобилей"],
    ["эвакуатор с частичной погрузкой", "применяется", "для быстрой городской эвакуации"],
    ["лебёдка", "имеет", "электрический или гидравлический привод"],
    ["гидравлическая система", "использует", "давление масла"],
    ["выдвижная платформа", "может", "наклоняться и выдвигаться"],
    ["проблесковые маячки", "бывают", "оранжевого и синего цвета"],
    ["эвакуатор", "называется", "tow truck"],
    ["эвакуатор с полной погрузкой", "называется", "платформенным эвакуатором"],
    ["гидравлическая стрела", "называется", "манипулятором"],
    ["световая сигнализация", "включает", "проблесковые маячки"],
    ["эвакуатор", "включает", "лебёдку"],
    ["эвакуатор", "включает", "гидравлическую систему"],
    ["эвакуатор", "включает", "выдвижную платформу"],
    ["эвакуатор", "включает", "систему крепления"],
    ["эвакуатор", "включает", "проблесковые маячки"]
];

console.log("База знаний загружена, количество триад:", knowledge.length);

function capitalize(str) {
    if (!str || str.length === 0) return str;
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function cleanQuestion(q) {
    var result = q.toLowerCase().trim();
    result = result.replace(/[?.,!;:()]/g, "");
    result = result.replace(/ё/g, "е");
    return result;
}

function extractSubject(question) {

    var patterns = [
        /для чего нужен (.+)/,
        /для чего нужна (.+)/,
        /для чего нужны (.+)/,
        /для чего вообще нужен (.+)/,
        /что такое (.+)/,
        /что представляет собой (.+)/,
        /кто создал (.+)/,
        /когда был создан (.+)/,
        /как работает (.+)/,
        /как именно работает (.+)/,
        /из чего состоит (.+)/,
        /из каких частей состоит (.+)/,
        /где используется (.+)/,
        /где используются (.+)/,
        /где обычно используется (.+)/,
        /чем отличается (.+)/,
        /в чем отличие (.+)/,
        /в чём отличие (.+)/,
        /какие типы (.+) бывают/,
        /какие виды (.+) бывают/,
        /какие существуют типы (.+)/,
        /какая грузоподъемность у (.+)/,
        /какая грузоподъёмность у (.+)/,
        /чем известен (.+)/,
        /почему (.+)/,
        /какие преимущества у (.+)/,
        /какое преимущество у (.+)/,
        /какие недостатки у (.+)/,
        /какой недостаток у (.+)/
    ];

    for (var i = 0; i < patterns.length; i++) {
        var match = question.match(patterns[i]);
        if (match && match[1]) {
            return match[1].trim();
        }
    }

    var stopWords = [
        "что", "такое", "кто", "какой", "какая", "какие",
        "какое", "где", "когда", "как", "зачем", "почему",
        "для", "чего", "чем", "кого", "кому", "о", "об",
        "про", "на", "в", "по", "с", "из", "от", "до",
        "у", "за", "под", "над", "нужен", "нужна",
        "нужны", "работает", "состоит", "отличается",
        "используется", "бывают"
    ];

    var words = question.split(/\s+/);

    for (var i = 0; i < words.length; i++) {
        var w = words[i];

        if (w.length > 2 && stopWords.indexOf(w) === -1) {
            return w;
        }
    }

    return null;
}

function normalizeText(text) {
    return text
        .toLowerCase()
        .replace(/ё/g, "е")
        .replace(/[^\w\sа-я]/gi, "")
        .replace(/\s+/g, " ")
        .trim();
}

function findByExactSubject(subject) {

    subject = normalizeText(subject);

    for (var i = 0; i < knowledge.length; i++) {

        var subj = normalizeText(knowledge[i][0]);

        if (subj === subject) {
            return i;
        }
    }

    return -1;
}

function findByPartialSubject(subject) {

    subject = normalizeText(subject);

    for (var i = 0; i < knowledge.length; i++) {

        var subj = normalizeText(knowledge[i][0]);

        if (
            subj.indexOf(subject) >= 0 ||
            subject.indexOf(subj) >= 0
        ) {
            return i;
        }
    }

    return -1;
}

function findBySubjectAndPredicate(subject, predicateList) {

    subject = subject.toLowerCase();

    for (var i = 0; i < knowledge.length; i++) {

        var subj = knowledge[i][0].toLowerCase();
        var pred = knowledge[i][1].toLowerCase();

        var subjectMatch =
            subj === subject ||
            subj.indexOf(subject) >= 0 ||
            subject.indexOf(subj) >= 0;

        if (subjectMatch) {

            for (var p = 0; p < predicateList.length; p++) {

                if (
                    pred === predicateList[p] ||
                    pred.indexOf(predicateList[p]) >= 0
                ) {
                    return i;
                }
            }
        }
    }

    return -1;
}

function containsMainWords(text, sample) {

    text = normalizeText(text);
    sample = normalizeText(sample);

    var words = sample.split(" ");

    for (var i = 0; i < words.length; i++) {

        var w = words[i];

        if (w.length < 4) continue;

        if (text.indexOf(w) === -1) {
            return false;
        }
    }

    return true;
}

function getAnswer(question) {
    var qRaw = question.trim();
    if (qRaw === "") {
        return "Пожалуйста, введите вопрос.";
    }

    var q = cleanQuestion(qRaw);

    var subject = extractSubject(q);
    if (!subject) {
        return "Не удалось определить ключевое понятие в вопросе.";
    }

    var isForWhat = (q.indexOf("для чего") >= 0 || q.indexOf("зачем") >= 0);
    var isWhatIs = (q.indexOf("что такое") >= 0 || q.indexOf("кто такой") >= 0);
    var isHowWorks = (q.indexOf("как работает") >= 0);
    var isWhatConsists = (q.indexOf("из чего состоит") >= 0);
    var isDifference = (q.indexOf("чем отличается") >= 0);
    var isWhereUsed = (q.indexOf("где используется") >= 0);
    var isWhoCreated = (q.indexOf("кто создал") >= 0);
    var isCapacity = (q.indexOf("грузоподъемность") >= 0 || q.indexOf("грузоподъёмность") >= 0);
    var isTypes = (q.indexOf("какие типы") >= 0 || q.indexOf("какие виды") >= 0 || q.indexOf("существуют типы") >= 0 || q.indexOf("типы эвакуаторов") >= 0);
    var isWhenCreated = (q.indexOf("когда был создан") >= 0);
    var isFamous = (q.indexOf("чем известен") >= 0);
    var isWherePlural = (q.indexOf("где используются") >= 0);
    var isAlternativeWhatIs = (q.indexOf("представляет собой") >= 0);
    var isAlternativeHow = (q.indexOf("как именно работает") >= 0);
    var isAlternativeConsists = (q.indexOf("из каких частей состоит") >= 0);
    var isWhy = (q.indexOf("почему") >= 0);
    var isAdvantages = (q.indexOf("преимущества") >= 0 || q.indexOf("преимущество") >= 0);
    var isDisadvantages = (q.indexOf("недостатки") >= 0 || q.indexOf("недостаток") >= 0);
    var isDifferenceAdvanced = (q.indexOf("чем отличается") >= 0 || q.indexOf("в чем отличие") >= 0 || q.indexOf("в чём отличие") >= 0);
    var idx = -1;

    if (isForWhat) {
        var predicates = ["нужен", "нужна", "нужны", "предназначен", "предназначена", "предназначены", "нужен для", "нужна для", "нужны для"];
        idx = findBySubjectAndPredicate(subject, predicates);
        if (idx >= 0) {
            var triad = knowledge[idx];
            return capitalize(triad[0]) + " " + triad[1] + " " + triad[2] + ".";
        }
    }

    if (isWhatIs || isAlternativeWhatIs) {
        idx = findByExactSubject(subject);
        if (idx < 0) idx = findByPartialSubject(subject);
        if (idx >= 0) {
            var triad = knowledge[idx];
            return capitalize(triad[0]) + " " + triad[1] + " " + triad[2] + ".";
        }
    }

    if (isHowWorks || isAlternativeHow) {
        idx = findBySubjectAndPredicate(subject, ["работает", "работает как"]);
        if (idx < 0) idx = findByPartialSubject(subject);
        if (idx >= 0) {
            var triad = knowledge[idx];
            return capitalize(triad[0]) + " " + triad[1] + " " + triad[2] + ".";
        }
    }

    if (isWhatConsists || isAlternativeConsists) {
        idx = findBySubjectAndPredicate(subject, ["состоит из", "состоит"]);
        if (idx < 0) idx = findByPartialSubject(subject);
        if (idx >= 0) {
            var triad = knowledge[idx];
            return capitalize(triad[0]) + " " + triad[1] + " " + triad[2] + ".";
        }
    }

    if (isDifference) {

        for (var i = 0; i < knowledge.length; i++) {

            var subj = knowledge[i][0].toLowerCase();

            if (
                subj.indexOf("отличие") >= 0 &&
                q.indexOf("частичной погрузкой") >= 0 &&
                q.indexOf("полной погрузкой") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }
        }
    }

    if (isWhereUsed) {

        idx = findBySubjectAndPredicate(subject, [
            "используется",
            "используется для"
        ]);

        if (idx >= 0) {
            var triad = knowledge[idx];

            return capitalize(triad[0]) +
                " " +
                triad[1] +
                " " +
                triad[2] + ".";
        }
    }

    if (isWhoCreated) {

        idx = findBySubjectAndPredicate(subject, [
            "создал",
            "был создан"
        ]);

        if (idx >= 0) {

            var triad = knowledge[idx];

            return capitalize(triad[0]) +
                " " +
                triad[1] +
                " " +
                triad[2] + ".";
        }
    }

    if (isCapacity) {

        for (var i = 0; i < knowledge.length; i++) {

            var subj = normalizeText(knowledge[i][0]);
            var pred = normalizeText(knowledge[i][1]);

            var isCapacityTriad =
                pred.indexOf("грузоподъемность") >= 0 ||
                pred.indexOf("грузоподъёмность") >= 0;

            if (!isCapacityTriad) continue;

            if (
                (q.indexOf("легкого") >= 0 ||
                    q.indexOf("лёгкого") >= 0 ||
                    q.indexOf("легкий") >= 0 ||
                    q.indexOf("лёгкий") >= 0)
                &&
                subj.indexOf("легк") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }

            if (
                q.indexOf("средн") >= 0 &&
                subj.indexOf("средн") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }

            if (
                (q.indexOf("тяжел") >= 0 ||
                    q.indexOf("тяжёл") >= 0)
                &&
                subj.indexOf("тяж") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }

            if (
                q.indexOf("сверх") >= 0 &&
                subj.indexOf("сверх") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }
        }
    }

    if (isTypes) {

        for (var i = 0; i < knowledge.length; i++) {

            var subj = normalizeText(knowledge[i][0]);
            var pred = normalizeText(knowledge[i][1]);

            if (
                subj.indexOf("типы эвакуаторов") >= 0 &&
                pred.indexOf("бывают") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }
        }
    }

    if (isWhenCreated) {

        idx = findBySubjectAndPredicate(subject, [
            "был создан",
            "создал"
        ]);

        if (idx >= 0) {

            var triad = knowledge[idx];

            return capitalize(triad[0]) +
                " " +
                triad[1] +
                " " +
                triad[2] + ".";
        }
    }

    if (isWherePlural) {

        idx = findBySubjectAndPredicate(subject, [
            "используются",
            "применяются"
        ]);

        if (idx >= 0) {

            var triad = knowledge[idx];

            return capitalize(triad[0]) +
                " " +
                triad[1] +
                " " +
                triad[2] + ".";
        }
    }

    if (isFamous) {

        idx = findBySubjectAndPredicate(subject, [
            "известен"
        ]);

        if (idx >= 0) {

            var triad = knowledge[idx];

            return capitalize(triad[0]) +
                " " +
                triad[1] +
                " " +
                triad[2] + ".";
        }
    }

    if (isWhy) {

        if (q.indexOf("маячк") >= 0) {

            idx = findBySubjectAndPredicate(
                "проблесковые маячки",
                [
                    "повышают",
                    "нужны для",
                    "предотвращают"
                ]
            );

            if (idx >= 0) {

                var triad = knowledge[idx];

                return "Потому что " +
                    triad[0] + " " +
                    triad[1] + " " +
                    triad[2] + ".";
            }
        }

        if (q.indexOf("стабилизатор") >= 0) {

            idx = findByPartialSubject("стабилизаторы");

            if (idx >= 0) {

                var triad = knowledge[idx];

                return "Потому что " +
                    triad[0] + " " +
                    triad[1] + " " +
                    triad[2] + ".";
            }
        }

        if (q.indexOf("полной погрузк") >= 0) {

            idx = findByPartialSubject(
                "эвакуатор с полной погрузкой"
            );

            if (idx >= 0) {

                var triad = knowledge[idx];

                return "Потому что " +
                    triad[0] + " " +
                    triad[1] + " " +
                    triad[2] + ".";
            }
        }
    }

    if (isDifferenceAdvanced) {

        for (var i = 0; i < knowledge.length; i++) {

            var subj = normalizeText(knowledge[i][0]);

            if (
                subj.indexOf(subject) >= 0 ||
                subject.indexOf(subj) >= 0
            ) {

                if (
                    subj.indexOf("отлич") >= 0 ||
                    knowledge[i][1].toLowerCase().indexOf("отлич") >= 0
                ) {

                    var triad = knowledge[i];

                    return capitalize(triad[0]) +
                        " " +
                        triad[1] +
                        " " +
                        triad[2] + ".";
                }
            }
        }

        for (var i = 0; i < knowledge.length; i++) {

            var subj = normalizeText(knowledge[i][0]);

            if (
                q.indexOf("манипулятор") >= 0 &&
                subj.indexOf("манипулятор") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }

            if (
                q.indexOf("тяж") >= 0 &&
                subj.indexOf("тяж") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }

            if (
                q.indexOf("легк") >= 0 &&
                q.indexOf("средн") >= 0 &&
                subj.indexOf("легк") >= 0
            ) {

                var triad = knowledge[i];

                return capitalize(triad[0]) +
                    " " +
                    triad[1] +
                    " " +
                    triad[2] + ".";
            }
        }
    }

    if (isAdvantages) {

        for (var i = 0; i < knowledge.length; i++) {

            var subj =
                normalizeText(knowledge[i][0]);

            var pred =
                normalizeText(knowledge[i][1]);

            if (
                pred.indexOf("преимуществ") >= 0 ||
                pred.indexOf("преимущество") >= 0
            ) {

                if (
                    q.indexOf("полной погрузк") >= 0 &&
                    subj.indexOf("полной погрузк") >= 0
                ) {

                    var triad = knowledge[i];

                    return capitalize(triad[0]) +
                        " " +
                        triad[1] +
                        " " +
                        triad[2] + ".";
                }
            }
        }
    }

    if (isDisadvantages) {

        for (var i = 0; i < knowledge.length; i++) {

            var subj =
                normalizeText(knowledge[i][0]);

            var pred =
                normalizeText(knowledge[i][1]);

            if (
                pred.indexOf("недостат") >= 0
            ) {

                if (
                    q.indexOf("частичной погрузк") >= 0 &&
                    subj.indexOf("частичной погрузк") >= 0
                ) {

                    var triad = knowledge[i];

                    return capitalize(triad[0]) +
                        " " +
                        triad[1] +
                        " " +
                        triad[2] + ".";
                }
            }
        }
    }

    idx = findByExactSubject(subject);
    if (idx < 0) idx = findByPartialSubject(subject);
    if (idx >= 0) {
        var triad = knowledge[idx];
        return capitalize(triad[0]) + " " + triad[1] + " " + triad[2] + ".";
    }

    for (var i = 0; i < knowledge.length; i++) {
        if (knowledge[i][2].toLowerCase().indexOf(subject) >= 0) {
            var triad = knowledge[i];
            return capitalize(triad[0]) + " " + triad[1] + " " + triad[2] + ".";
        }
    }

    return "Ответ не найден. Попробуйте переформулировать вопрос.";
}

function askQuestion() {
    var input = document.getElementById('userQuestion');
    var question = input.value.trim();
    if (question === "") {
        document.getElementById('answerOutput').innerHTML = "❓ Введите вопрос.";
        return;
    }
    var answer = getAnswer(question);
    document.getElementById('answerOutput').innerHTML = answer;

    var utterance =
        new SpeechSynthesisUtterance(
            answer
        );

    utterance.lang =
        "ru-RU";

    speechSynthesis.cancel();

    speechSynthesis.speak(
        utterance
    );
}

function toggleMenu() {
    var nav = document.getElementById('navLinks');
    if (nav) nav.classList.toggle('active');
}

document.addEventListener('DOMContentLoaded', function () {
    var input = document.getElementById('userQuestion');
    if (input) {
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') askQuestion();
        });
    }
});














var dialogOn = false;

function dialog_window() {

    var toggleBtn =
        document.createElement("button");

    toggleBtn.id =
        "dialogToggle";

    toggleBtn.innerHTML =
        "База знаний";

    toggleBtn.onclick =
        openDialog;

    document.body.appendChild(
        toggleBtn
    );

    var dialog =
        document.createElement("div");

    dialog.id = "dialog";

    dialog.style.display =
        "none";

    dialog.innerHTML =

        '<div class="dialog-header">' +

        '<span>Диалог с базой знаний</span>' +

        '<button id="closeDialog" onclick="openDialog()">✖</button>' +

        '</div>' +

        '<div id="history" class="dialog-history"></div>' +

        '<div class="dialog-input">' +

        '<input ' +
        'id="question" ' +
        'type="text" ' +
        'placeholder="Введите вопрос..." ' +
        'onkeypress="if(event.key===\'Enter\') ask()">' +

        '<button onclick="ask()">➤</button>' +

        '<button onclick="startVoiceInput()" ' +
        'class="voice-button">' +
        '🎤' +
        '</button>' +

        '</div>';

    document.body.appendChild(
        dialog
    );
}

function openDialog() {

    var dialog =
        document.getElementById(
            "dialog"
        );

    if (
        dialog.style.display
        === "none"
    ) {

        dialog.style.display =
            "block";

        $("#dialog")
            .hide()
            .slideDown(300);

    }
    else {

        $("#dialog")
            .slideUp(
                300,
                function () {

                    dialog.style.display =
                        "none";
                }
            );
    }
}

function ask() {

    var input =
        document.getElementById("question");

    var question =
        input.value.trim();

    if (question === "") return;

    dialogOn = true;

    var history =
        document.getElementById("history");

    var questionDiv =
        document.createElement("div");

    questionDiv.className =
        "question";

    questionDiv.innerHTML =
        question;

    history.appendChild(questionDiv);

    var answer =
        getAnswer(question);

    var answerDiv =
        document.createElement("div");

    answerDiv.className =
        "answer";

    answerDiv.innerHTML =
        answer;

    history.appendChild(answerDiv);

    var utterance =
        new SpeechSynthesisUtterance(
            answer
        );

    utterance.lang = "ru-RU";

    speechSynthesis.speak(
        utterance
    );

    history.scrollTop =
        history.scrollHeight;

    input.value = "";
}

function startVoiceInput() {

    if (
        !(
            'webkitSpeechRecognition'
            in window
        )
    ) {

        alert(
            "Голосовой ввод не поддерживается браузером"
        );

        return;
    }

    var recognition =
        new webkitSpeechRecognition();

    recognition.lang =
        "ru-RU";

    recognition.interimResults =
        false;

    recognition.maxAlternatives =
        1;

    recognition.start();

    recognition.onresult =
        function (event) {

            var text =
                event.results[0][0]
                    .transcript;

            var dialogInput =
                document.getElementById(
                    "question"
                );

            var pageInput =
                document.getElementById(
                    "userQuestion"
                );

            if (dialogInput) {

                dialogInput.value =
                    text;

                ask();
            }

            else if (pageInput) {

                pageInput.value =
                    text;

                askQuestion();
            }
        };

    recognition.onerror =
        function (event) {

            console.log(
                event.error
            );
        };
}