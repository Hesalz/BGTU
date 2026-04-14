document.addEventListener('DOMContentLoaded', () => {
    loadAllCelebrities();
});

function loadAllCelebrities() {
    fetch('/api/Celebrities')
    .then(r => r.json())
        .then(data => {
            console.log(data);
            const celDiv = document.getElementById('celDiv');
            data.forEach(cel => {
                const image = document.createElement('img');
                console.log(cel.reqPhotoPath);
                fetch(`/api/Celebrities/photopath/${cel.reqPhotoPath}`)
                    .then(response => {
                        return response.json();
                    })
                    .then(filePath => {
                        image.src = filePath;
                        image.addEventListener('click', () => { loadLifeEvents(cel.id, cel.fullName) });
                        celDiv.appendChild(image);
                    })
            });
        })
        .catch(error => console.error('Error fetching life events:', error));
}

function loadLifeEvents(celebrityId, fullname) {
    fetch(`/api/Lifeevents/Celebrities/${celebrityId}`) 
        .then(r => r.json())
        .then(e => {
            console.log(e);
            const eventsList = document.getElementById('events');
            eventsList.innerHTML = '';
            e.forEach(ev => {
                const li = document.createElement('li');
                console.log(fullname)
                li.textContent = fullname + "        " + ev.date + "      " + ev.description; 
                console.log(li);
                eventsList.appendChild(li);
            });
            document.getElementById('lifeDiv').style.display = 'block';
        })
}