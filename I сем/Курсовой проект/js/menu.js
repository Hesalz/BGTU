var el = document.getElementsByClassName('menu-item');

for(var i = 0; i < el.length; i++) {
    el[i].addEventListener("mouseenter", showSub, false);
    el[i].addEventListener("mouseleave", hideSub, false);
}

var hideTimer;
var currentSubmenu;

function showSub(e) {
    clearTimeout(hideTimer);
    if(this.children.length > 1) {
       hideOtherSubmenus(this);
       this.children[1].style.height = "auto";
       this.children[1].style.display = "block";
       this.children[1].style.opacity = "1";
       currentSubmenu = this.children[1];
    } else {
       return false;
    }
}

function hideSub(e) {
    if(this.children.length > 1) {
        var submenu = this.children[1];
        hideTimer = setTimeout(function() {
            submenu.style.height = "0px";
            submenu.style.display = "none";
            submenu.style.opacity = "0";
        }, 200);
    } else {
        return false;
    }
}

function hideOtherSubmenus(currentElement) {
    for(var i = 0; i < el.length; i++) {
        if(el[i] !== currentElement && el[i].children.length > 1) {
            var submenu = el[i].children[1];
            submenu.style.height = "0px";
            submenu.style.display = "none";
            submenu.style.opacity = "0";
        }
    }
}
