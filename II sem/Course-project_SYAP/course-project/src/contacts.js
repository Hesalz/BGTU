import React from "react";
import './contacts.css';
import Header from "./header";
import Footer from "./footer";
import Tooth from "./images/tooth2.png";

export default function Contacts() {
    return (
        <>
            <Header />
            <h2 id="contacts">Контакты стоматологического центра "Cube"</h2>
            <div className="contacts">
                <div>
                    <h3>Адрес</h3>
                    <p>г. Минск, ул. Свердлова, 13А, корп. 4, Республика Беларусь.
                    </p>
                </div>
                <div>
                    <h3>Телефоны</h3>
                    <p><span>Мобильный</span> <br />
                        +375 (29) 331-05-05<br />
                        <span>Городской</span><br />
                        +375 (17) 726-05-05<br />
                        <span>Городской</span><br />
                        +375 (17) 555-56-55<br />
                        <span>Короткий</span><br />
                        7758 (A1, МТС)</p>
                </div>
                <div>
                    <h3>Время работы</h3>
                    <p><span>Понедельник — Суббота </span><br />
                        09:00 - 21:00<br />
                        <span>Воскресенье</span><br />
                        09:00 - 18:00</p>
                </div>
                <img className="tooth" src={Tooth} alt="tooth" />
                <div>
                    <h3>Email</h3>
                    <p>awdefin@gmail.com</p>
                </div>
            </div>
            <div id="map">
                <img className="tooth-2" src={Tooth} alt="tooth" />
                <h3>Мы на карте</h3>
                <div className="map-container">
                    <iframe
                        src="https://yandex.ru/map-widget/v1/?um=constructor%3A56ab1d0bd5e58084b948c41c1fb3cd8629fdd944b2900757305be7be65e13753&amp;source=constructor"
                        width="1190"
                        height="504"
                        frameBorder="0"
                        title="map"
                    ></iframe>
                </div>
            </div>
            <Footer />
        </>
    );
}