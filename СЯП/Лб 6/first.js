let windo = [];

function wind1(){
    windo.unshift(window.open("", "", "width=200, height=200,"));
}

function close1(){
    for (i = 0; i < windo.length; i++) {
        windo[i].window.close();
      }
}

function doc() {
    for (i = 0; i < windo.length; i++) {
      windo[i].document.write("Текст в вызванном окне");
    }
  }

  function table(){
    document.write("<table border = 1 style='background-color: lightblue'>");
    document.write("<th>Свойство</th>");
    document.write("<th>Значение</th>");
    document.write("<tr><td>1. Вся информация о браузере пользователя</td><td>" + navigator.userAgent + "</td></tr>");
    document.write("<tr><td>2. Версия браузера</td><td>" + navigator.appVersion + "</td></tr>");
    document.write("<tr><td>3. Название браузера</td><td>" + navigator.appName + "</td></tr>");
    document.write("<tr><td>4. Кодовое название браузера</td><td>" + navigator.appCodeName + "</td></tr>");
    document.write("<tr><td>5. ОС, которую использует пользователь</td><td>" + navigator.platform + "</td></tr>");
    document.write("<tr><td>6. Включена ли поддержка Java в браузере)</td><td>" + navigator.javaEnabled() + "</td></tr>");
    document.write("<tr><td>7. Ширина и высота экрана</td><td>ширина экрана: " + screen.width + ", высота экрана: " + screen.height + "</td></tr>");
    document.write("<tr><td>8. Глубина цвета</td><td>" + screen.colorDepth + "</td></tr>");
    document.write("<tr><td>9. URL, которые были посещены в данном окне браузера</td><td>" + history.length + "</td></tr>");
    document.write("<tr><td>10. URL текущего документа</td><td>" + location.href + "</td></tr>");
    document.write("<tr><td>11. Путь к загруженному документу</td><td>" + location.pathname + "</td></tr>");
    document.write("<tr><td>12. Имя домена загруженного документа</td><td>" + location.host + "</td></tr></table>");
  }