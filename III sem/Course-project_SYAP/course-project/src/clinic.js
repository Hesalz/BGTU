import React, { useState } from 'react';
import './clinic.css';
import Form from './Form';
import clinic from './images/clinic.png';
import toothIcon from './images/tooth.png';
import Footer from './footer';
import Header from './header';
const buttonNames = ["Имплантация зубов", "Виниры", "Удаление зубов"];
const buttonNamesBottom = ["Создание протезов", "Лечение кариеса", "Коронки"];
const commonLink = "/services";

const handleButtonClick = () => {
    window.location.href = commonLink;
};

export default function Clinic() {
    const [isFormVisible, setFormVisible] = useState(false);

  const handleOpenForm = () => {
    setFormVisible(true);
  };

  const handleCloseForm = () => {
    setFormVisible(false);
  };

  const [isModalOpen, setIsModalOpen] = useState(false);
  const handleOpenModal = () => {
    setIsModalOpen(true);
};

const handleCloseModal = () => {
    setIsModalOpen(false);
};

    return (
        <>
        <Header />
            <div className="implant">
                <div className='clinic-text'>
                    <h1>Новые зубы за пару дней!</h1>
                    <p id='clinic-text-1'> Невероятная реальноть, в которую сложно поверить</p>
                    <div className="clinic-text-1">
                        <a href="#appointment" onClick={handleOpenModal}>Подробнее</a>
                        <button className="clinic-button" onClick={handleOpenForm}>Записаться</button>
                    </div>
                </div>
                <img src={clinic} className="clinic-img" alt="clinic" />
            </div>

            {isModalOpen && (
                <div className="modal-overlay3" onClick={handleCloseModal}>
                    <div className="modal3" onClick={(e) => e.stopPropagation()}>
                        <button className="close-button" onClick={handleCloseModal}>X</button>
                        <h2>Подробнее о процессе имплантации</h2>
                        <p>
                            Современные методы имплантации позволяют получить новые зубы за пару дней.
                            Благодаря инновационным технологиям мы минимизируем дискомфорт и ускоряем процесс восстановления.
                        </p>
                        <p>
                            <strong>Преимущества:</strong>
                            <ul>
                                <li>Короткие сроки лечения</li>
                                <li>Минимальная травматичность</li>
                                <li>Естественная эстетика</li>
                                <li>Долговечный результат</li>
                            </ul>
                        </p>
                        <p>Свяжитесь с нами для получения бесплатной консультации.</p>
                    </div>
                </div>
            )}

            <div className="services">
                <h2>Популярные услуги</h2>
                <p>Стоматологические услуги, наиболее востребованные пациентами в наших стоматологических центрах</p>
            </div>
            <div className="button-container">
                {buttonNames.map((name, index) => (
                    <button
                        key={index}
                        className="button"
                        onClick={() => handleButtonClick()}
                    >
                        <img src={toothIcon} alt="tooth" className="button-icon" />
                        <div className="button-text">{name}</div>
                    </button>
                ))}
            </div>
            <div className="button-container">
                {buttonNamesBottom.map((name, index) => (
                    <button
                        key={index}
                        className="button"
                        onClick={() => handleButtonClick()}
                    >
                        <img src={toothIcon} alt="tooth" className="button-icon" />
                        <div className="button-text">{name}</div>
                    </button>
                ))}
            </div>
            <div className='dentist'>
                <h2>Стоматология в Беларуси «Cube»</h2>
                <p>Частная стоматология «Cube» оказывает весь спектр стоматологических услуг и делает это хорошо. Наш центр применяет единую ценовую политику для пациентов из Беларуси и иностранных граждан. Для удобства мы внедрили конвертер валют: Вы можете изучать цены нашей стоматологии в белорусских и российских рублях, долларах и евро.

                    <br /> <br /> Наши услуги можно оплатить онлайн и наличными, кредитными карточками, картами рассрочек, по ЕРИП и т.д.
                </p>
            </div>

            <div className='dentist'>
                <h2>Современная платная стоматологическая клиника в Беларуси</h2>
                <div className='dentist-list'>
                    <ul>
                        <li>эргономичные кресла минимизируют усталость пациента и доктора при длительном лечении;</li>
                        <li>интеллектуальная установка PLANMECA PRO MAX 3D позволяет проводить точное рентгенологическое исследование челюстно-лицевой области;</li>
                        <li>фрезерный станок, цифровой сканнер и специализированное программное обеспечение вывело протезирование в «Cube» на совершенно новый уровень, благодаря чему фраза «коронка за 1 день» перестала быть маркетинговой уловкой, а стала реальностью;</li>
                        <li>стоматология оснащена оборудованием, позволяющем проводить лечение во сне под наркозом, седацией;</li>
                        <li>применение микроскопа делает лечение еще более точным и качественным.</li>
                    </ul>
                </div>
            </div>
            {isFormVisible && <Form onClose={handleCloseForm} />}
            <Footer />
        </>
    );
}
