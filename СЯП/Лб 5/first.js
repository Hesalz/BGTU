let all = document.all; // Задание 1

function t1() {
    let result = "";
    for (let i = 0; i < all.length; i++) {
        result += all[i].tagName + " [" + i + "]" + "\n";
    }
    alert(result);
}

function t2() { // Задание 2
    let tags2 = "length = " + document.body.childNodes.length + "\n";
    for (let i = 1; i < (document.body.childNodes.length); i++) {
      tags2 += "Tag " + document.body.childNodes[i].tagName + " [" + i + "]" + "\n";
    }
    alert(tags2);
  }

  function t3() { // Задание 3
    let all_1 = document.querySelectorAll('span');
    alert("Первый элемент span " + all_1[0].innerHTML);
    let all_2 = document.getElementsByTagName('span');
    alert("Первый элемент span " + all_2[0].innerHTML);
    let all_3 = document.getElementById('ida');
    alert("Первый элемент span " + all_3.innerHTML);
  }

  function t4() { // Задание 4
    let marks = 0;
    let all_mark = document.getElementsByClassName('mark');
    for (let i = 0; i < all_mark.length; i++) {
      marks += parseInt(all_mark[i].innerHTML);
    }
    marks = marks / 3;
    document.getElementById('abc').innerHTML = "Cредний балл: " + marks;
    document.getElementById('ida').innerHTML = " MARK = " + marks;
  }
