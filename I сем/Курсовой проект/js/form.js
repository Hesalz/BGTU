function toggleForm() {
    var background = document.getElementById("background");
    var form = document.getElementById("form");
    if (background.style.display === "none") {
        background.style.display = "block";
        form.style.display = "block";
        document.body.style.overflow = "hidden";
    } else {
        background.style.display = "none";
        form.style.display = "none";
        document.body.style.overflow = "";
    }
}

function submitForm(event) {
    event.preventDefault();
    var name = document.getElementById("name").value;
    var phone = document.getElementById("phone").value;
    var date = document.getElementById("date").value;
    var time = document.getElementById("time").value;
    var people = document.getElementById("people").value;

    console.log("Имя:", name);
    console.log("Номер телефона:", phone);
    console.log("Дата:", date);
    console.log("Время:", time);
    console.log("Количество человек:", people);
    
    var xmlString = `
    <reservation>
        <name>${name}</name>
        <phone>${phone}</phone>
        <date>${date}</date>
        <time>${time}</time>
        <people>${people}</people>
    </reservation>
        
    `;
    
    var currentData = localStorage.getItem('formData');
    
    var newData = currentData ? currentData + xmlString : xmlString;
    
    localStorage.setItem('formData', newData);
    
    document.getElementById("myForm").reset();
    toggleForm();
    downloadXML();
}

function downloadXML() {
    var formData = localStorage.getItem('formData');
    var blob = new Blob([formData], { type: "text/xml" });
    var url = URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = 'data.xml';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}