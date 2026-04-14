fetch('/json/json.json')
.then(response => response.json())
.then(data => {
    document.getElementById('json-content-display').textContent = JSON.stringify(data, null, 2);
})
.catch(error => console.error('Error fetching JSON:', error));

fetch('/xml/xml.xml')
.then(response => response.text())
.then(data => {
    document.getElementById('xml-content-display').textContent = data;
})
.catch(error => console.error('Error fetching XML:', error));