var zd1 = () => {
    alert("Вас приветствует учебный центр");
}
function zd2() {
    let name = prompt("Введите имя");
    alert("Добро пожаловать на наши курсы, " + name);
}
function zd3() {
    if (confirm("Хотите стать Web-дизайнером?")) { alert("Учите стили CSS и JavaScript!"); }
    else {
        alert("Упускаете время!");
    }
}
function zd4() {
    let x = 14 + 8;
    console.log(x);
    let a = "10";
    let b = "5";
    console.log(a + b);
    console.log(x + b)
}
function zd6() {
    alert("Результатом сложения строки и числа всегда будет строка");
}
function zd7() {
    if (1 == 1) console.log("true");
    else console.log("false");
    if (3 != 13) console.log("true");
    else console.log("false");
    if (6 > 11) console.log("true");
    else console.log("false");
    if (1 >= 12) console.log("true");
    else console.log("false");
    if (1 < 9) console.log("true");
    else console.log("false");
    if (1 <= 1) console.log("true");
    else console.log("false");
}
function zd8() {
    let z = 384;
    let c = 94;
    let q = ((35 * z - 25 * c) / 5 + 232);
    let w = (8 * z / (c + 5 * c / (z - 43 * 6)));
    console.log(q);
    console.log(w);
    alert("Остаток деления = " + q % w);
    document.write('<p style="color: red"><font size="8" style="font-family: Impact"> Остаток деления = ' + q % w + '</font></p>');
}
function zd9() {
    let num = prompt("Введите число");
    if ((num < 20 || num > 40) && num != 15 && num % 5 == 0.0) document.write('<p style="color: green"> Правильное значение.</p><br>');
    else document.write('<p style="color: green"><font size="8" style="font-family: Impact"> Не правильное значение.</font></p><br>');
}
function zd10() {
    let zaz = prompt("Введите число A");
    let zbz = prompt("Введите число Б");
    if (zaz > zbz) document.write('<p style="color: blue"><font size="8" style="font-family: Impact"> А больше Б.</font></p><br>');
    else document.write('<p style="color: blue"><font size="8" style="font-family: Impact"> А меньше Б.</font></p><br>');
    if (zaz % zbz == 0) document.write('<p style="color: blue"><font size="8" style="font-family: Impact"> А делится на Б без остатка.</font></p><br>');
    else document.write('<p style="color: blue"><font size="8" style="font-family: Impact">А не делится на Б без остатка.</font></p><br>');
}
function zd11() {
    switch (new Date().getDay()) {
        case 0: day = "Воскресенье. ";
            break;
        case 1: day = "Понедельник. ";
            break;
        case 2: day = "Вторник. ";
            break;
        case 3: day = "Среда. ";
            break;
        case 4: day = "Четверг. ";
            break;
        case 5: day = "Пятница. ";
            break;
        case 6: day = "Суббота. ";
            break;
    }
    document.write(day + "<br>");
}
function zd12() {
    let s = 12, t = 47, v = 61;
    alert("Число 12 в двоичной системе: " + s.toString(2) + ". Число 12 в восьмеричной системе: " + s.toString(8) + ". Число 12 в шестнадцатеричной системе: " + s.toString(16));
    alert("Число 47 в двоичной системе: " + t.toString(2) + ". Число 47 в восьмеричной системе: " + t.toString(8) + ". Число 47 в шестнадцатеричной системе: " + t.toString(16));
    alert("Число 61 в двоичной системе: " + v.toString(2) + ". Число 61 в восьмеричной системе: " + v.toString(8) + ". Число 61 в шестнадцатеричной системе: " + v.toString(16));
}
function zd13() {
    try {
        alert("Код прошёл без ошибок");
    }
    catch (err) {
        alert("В коде найдена ошибка - " + err.message);
    }
}