const xmlString = `
<products>
    <person>
        <name>Екатерина -повар молекулярной кухни</name>
        <image>./img/comand/katya.jpg</image>
    </person>
    <person>
        <name>Денис - Повар</name>
        <image>./img/comand/mask.jpg</image>
    </person>
    <person>
        <name>Федор - Повар</name>
        <image>./img/comand/fedor.jpg</image>
    </person>
    <person>
        <name>Константин - Бармен</name>
        <image>./img/comand/kostya.jpg</image>
    </person>
    <person>
        <name>Лев - СуШеф</name>
        <image>./img/comand/leva.jpg</image>
    </person>
    <person>
        <name>Луи - Кондитер-пекарь</name>
        <image>./img/comand/lui.jpg</image>
    </person>
    <person>
        <name>Арсений - Повар</name>
        <image>./img/comand/arsen.jpg</image>
    </person>
    <person>
        <name>Максим - Повар</name>
        <image>./img/comand/maks.jpg</image>
    </person>
</products>
`;


const parser = new DOMParser();


const xmlDoc = parser.parseFromString(xmlString, 'text/xml');


const products = xmlDoc.querySelectorAll('person');


const allCardElement = document.querySelector('.personal');





products.forEach((product) => {
    const name = product.querySelector('name').textContent;
    
    const imageURL = product.querySelector('image').textContent;

    const productCard = document.createElement('div');
    productCard.classList.add('product-card');



    const nameElement = document.createElement('p');
    nameElement.textContent = name;

    const imageElement = document.createElement('img');
    imageElement.src = imageURL;

   


    productCard.appendChild(imageElement);
    productCard.appendChild(nameElement);
   
    

    allCardElement.appendChild(productCard);
});