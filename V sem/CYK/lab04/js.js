document.addEventListener('DOMContentLoaded', function () {
    // Код для главной страницы - смена фильмов
    const movieList = document.getElementById('movieList');
    const btn = document.getElementById('changeMovies');

    if (movieList && btn) {
        const sets = [
            [
                {
                    title: "Джокер",
                    desc: "Красочный боевик с незабываемыми персонажами и спецэффектами.",
                    img: "http://labscyk/wordpress/wp-content/uploads/2025/10/poster.jpg"
                },
                {
                    title: "Чупапа",
                    desc: "Трогательная драма о поисках себя и настоящей любви.",
                    img: "http://labscyk/wordpress/wp-content/uploads/2025/09/9c0ada35-1ded-4152-9214-b0a85da9f152.webp"
                },
                {
                    title: "Фильм 3",
                    desc: "Увлекательный фантастический триллер с неожиданными поворотами.",
                    img: "poster.jpg"
                }
            ],
            [
                {
                    title: "Фильм 4",
                    desc: "Комедия о приключениях в большом городе.",
                    img: "poster.jpg"
                },
                {
                    title: "Фильм 5",
                    desc: "Криминальный триллер с неожиданным финалом.",
                    img: "poster.jpg"
                },
                {
                    title: "Фильм 6",
                    desc: "Историческая драма о судьбах великих людей.",
                    img: "poster.jpg"
                }
            ],
            [
                {
                    title: "Фильм 7",
                    desc: "Мистический хоррор с пугающей атмосферой.",
                    img: "poster.jpg"
                },
                {
                    title: "Фильм 8",
                    desc: "Анимация для всей семьи с яркими героями.",
                    img: "poster.jpg"
                },
                {
                    title: "Фильм 9",
                    desc: "Документальный фильм об открытии новых технологий.",
                    img: "poster.jpg"
                }
            ]
        ];

        let currentSet = 0;

        function loadImage(src) {
            return new Promise((resolve, reject) => {
                const img = new Image();
                img.onload = () => resolve(src);
                img.onerror = () => reject(src);
                img.src = scriptData.templateUrl + '/' + src;
            });
        }

        async function renderMovies(setIndex) {
            movieList.innerHTML = '';
            const movies = sets[setIndex];

            for (const movie of movies) {
                let src = movie.img;
                try {
                    await loadImage(src);
                } catch (e) {
                    console.warn(`Не удалось загрузить изображение ${src}`);
                }

                const movieEl = document.createElement('div');
                movieEl.classList.add('movie');
                movieEl.innerHTML = `
                    <img src="${scriptData.templateUrl}/${src}" alt="${movie.title}" />
                    <div class="movie-info">
                        <h3>${movie.title}</h3>
                        <p>${movie.desc}</p>
                    </div>
                `;
                movieList.appendChild(movieEl);
            }
        }

        renderMovies(currentSet);

        btn.addEventListener('click', function () {
            currentSet = (currentSet + 1) % sets.length;
            renderMovies(currentSet);
        });
    }

    // Код для страницы контактов - форма
    const form = document.getElementById('contactForm');
    const successMsg = document.getElementById('successMessage');

    if (form && successMsg) {
        successMsg.style.display = 'none';
        
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            successMsg.style.display = 'block';
            form.reset();
            setTimeout(() => successMsg.style.display = 'none', 3000);
        });
    }

    // Код для страницы "О проекте" - слайдер
    const slides = document.getElementById('slides');
    
    if (slides) {
        let currentIndex = 0;
        const totalSlides = slides.children.length;

        function showSlide(index) {
            slides.style.transform = `translateX(-${index * 100}%)`;
        }

        setInterval(() => {
            currentIndex = (currentIndex + 1) % totalSlides;
            showSlide(currentIndex);
        }, 5000);
    }
});
