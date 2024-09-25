const imageUrls = [
    './img/bludo.jpg',
    './img/bludo2.jpg',
    './img/bludo3.jpg'
    ]
    
    const images = imageUrls.map(url => {
      const img = new Image()
      img.src = url
      return img
    });
    
    let index = 1;
    
    setInterval(()=>{
    let cont = document.querySelector('.container')
      cont.style.backgroundImage = `url(${images[index].src})`
      index = index >= images.length-1 ? 0 : index + 1
    }, 10000)