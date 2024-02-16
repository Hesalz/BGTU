function onmouse(event) {
    var target = event.target;
    if (target.classList.contains('menu-item')) {
        var submenus = target.getElementsByClassName('submenu');

        closeMenu();

        for (var i = 0; i < submenus.length; i++) {
            submenus[i].style.height = '30px';
            submenus[i].style.visibility = 'visible';
            submenus[i].style.fontSize = '20px';
        }
    }
}

document.onmouseover = function (event) {
    var target = event.target;

    if (!target.classList.contains('menu-item') && !target.classList.contains('submenu') && !target.classList.contains('a')) {
        closeMenu();
    }
}

function closeMenu() {
    var submenus = document.getElementsByClassName('submenu');
    for (var i = 0; i < submenus.length; i++) {
        submenus[i].style.height = '0px';
        submenus[i].style.visibility = 'hidden';
        submenus[i].style.fontSize = '0';
    }
}