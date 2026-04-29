import React, { useState } from "react";
import './specialists.css';
import Header from "./header";
import Footer from "./footer";
import Doctor1 from "./images/doctor1.jpg";
import Doctor2 from "./images/doctor2.jpg";
import Doctor3 from "./images/doctor3.jpg";
import Doctor4 from "./images/doctor4.jpg";
import Doctor5 from "./images/doctor5.jpg";
import Doctor6 from "./images/doctor6.jpg";
import Doctor7 from "./images/doctor7.jpg";
import Doctor8 from "./images/doctor8.jpg";
import Doctor9 from "./images/doctor9.jpg";

const doctorsData = [
    {
        id: 1,
        name: "Шех Ольга Сергеевна",
        role: "Стоматолог-хирург-имплантолог",
        image: Doctor1,
        details: "Образование: Медицинский университет им. А.А. Богомольца.",
        experience: "Стаж: 15 лет.",
        achievements: "Достижения: Автор 10 научных публикаций по хирургической стоматологии.",
    },
    {
        id: 2,
        name: "Шех Сергей Петрович",
        role: "Анестезиолог-реаниматолог",
        image: Doctor2,
        details: "Образование: Московский медицинский университет.",
        experience: "Стаж: 20 лет.",
        achievements: "Достижения: Руководитель кафедры анестезиологии.",
    },
    {
        id: 3,
        name: "Забело Светлана Геннадьевна",
        role: "Рентгенолаборант",
        image: Doctor3,
        details: "Образование: Минский медицинский колледж.",
        experience: "Стаж: 10 лет.",
        achievements: "Достижения: Участник международных конференций по рентгенологии.",
    },
    {
        id: 4,
        name: "Тихонов Андрей Петрович",
        role: "Стоматолог-хирург-имплантолог",
        image: Doctor4,
        details: "Образование: Первый Санкт-Петербургский медицинский университет им. академика И.П. Павлова.",
        experience: "Стаж: 18 лет.",
        achievements: "Достижения: Участие в международных имплантологических форумах.",
    },
    {
        id: 5,
        name: "Миренко Дарина Викторовна",
        role: "Стоматолог-терапевт",
        image: Doctor5,
        details: "Образование: Белорусский государственный медицинский университет.", 
        experience: "Стаж: 12 лет.",
        achievements: "Достижения: Специалист по эстетической реставрации зубов.",
    },
    {
        id: 6,
        name: "Русецкая Мария Андреевна",
        role: "Анестезиолог-реаниматолог детский",
        image: Doctor6,
        details: "Образование: Российский национальный исследовательский медицинский университет имени Н.И. Пирогова.",
        experience: "Стаж: 14 лет.",
        achievements: "Достижения: Эксперт в области детской анестезиологии.",
    },
    {
        id: 7,
        name: "Межицкая Оксана Сергеевна",
        role: "Стоматолог-терапевт детский",
        image: Doctor7,
        details: "Образование: Московский государственный медико-стоматологический университет.",
        experience: "Стаж: 10 лет.",
        achievements: "Достижения: Сертификаты международного уровня в детской стоматологии.",
    },
    {
        id: 8,
        name: "Вахранова Елена Денисовна",
        role: "Стоматолог-ортопед",
        image: Doctor8,
        details: "Образование: Воронежский государственный медицинский университет.",
        experience: "Стаж: 16 лет.",
        achievements: "Достижения: Разработка авторской методики протезирования зубов.",
    },
    {
        id: 9,
        name: "Балан Евгений Олегович",
        role: "Стоматолог-пародонтолог",
        image: Doctor9,
        details: "Образование: Казанский государственный медицинский университет.", 
        experience: "Стаж: 11 лет.",
        achievements: "Достижения: Проведение более 500 успешных операций по лечению пародонтита.",
    }
];

function Specialists() {
    const [selectedDoctor, setSelectedDoctor] = useState(null);

    const handleDoctorClick = (doctor) => {
        setSelectedDoctor(doctor);
    };

    const closeModal = () => {
        setSelectedDoctor(null);
    };

    return (
        <>
            <Header />
            <h2 id="h2spec">Стоматологи в "Cube"</h2>
            <div className="doctors">
                {doctorsData.map((doctor) => (
                    <div
                        key={doctor.id}
                        className="doctor-container"
                        onClick={() => handleDoctorClick(doctor)}
                    >
                        <img className="doctor" src={doctor.image} alt={doctor.name} />
                        <p id="f">{doctor.name}</p>
                        <p>{doctor.role}</p>
                    </div>
                ))}
            </div>
            {selectedDoctor && (
                <div className="modal-overlay2" onClick={closeModal}>
                    <div className="modal2" onClick={(e) => e.stopPropagation()}>
                        <h2>{selectedDoctor.name}</h2>
                        <p>{selectedDoctor.role}</p>
                        <p>{selectedDoctor.details}</p>
                        <p>{selectedDoctor.experience}</p>
                        <p>{selectedDoctor.achievements}</p>
                        <button onClick={closeModal}>Закрыть</button>
                    </div>
                </div>
            )}
            <Footer />
        </>
    );
}

export default Specialists;
