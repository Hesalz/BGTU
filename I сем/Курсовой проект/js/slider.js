let slideIndex = 0;
let timeout;

showSlides();

function showSlides() {
  let i;
  let slides = document.getElementsByClassName("mySlides");
  let dots = document.getElementsByClassName("dot");
  for (i = 0; i < slides.length; i++) {
    slides[i].style.display = "none";  
  }
  for (i = 0; i < dots.length; i++) {
    dots[i].classList.remove("activedot");
  }
  slideIndex++;
  if (slideIndex > slides.length) {slideIndex = 1}    
  slides[slideIndex-1].style.display = "block";  
  dots[slideIndex-1].classList.add("activedot");
  timeout = setTimeout(showSlides, 8000);
}

function currentSlide(n) {
  slideIndex = n;
  clearTimeout(timeout);
  showSlides();
}

let slideIndexb = 0;
let timeoutb;

showSlidesb();

function showSlidesb() {
  let i;
  let slidesb = document.getElementsByClassName("mySlidesb");
  let dotsb = document.getElementsByClassName("dotb");
  for (i = 0; i < slidesb.length; i++) {
    slidesb[i].style.display = "none";  
  }
  for (i = 0; i < dotsb.length; i++) {
    dotsb[i].classList.remove("activedot");
  }
  slideIndexb++;
  if (slideIndexb > slidesb.length) {slideIndexb = 1}    
  slidesb[slideIndexb-1].style.display = "block";  
  dotsb[slideIndexb-1].classList.add("activedot");
  timeoutb = setTimeout(showSlidesb, 8000);
}

function currentSlideb(n) {
  slideIndexb = n;
  clearTimeout(timeoutb);
  showSlidesb();
}
