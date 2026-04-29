import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import logo from './images/logo.png';
import './header.css';
import Form from './Form';

function Header() {
    const [isFormVisible, setFormVisible] = useState(false);
    const [isMenuOpen, setMenuOpen] = useState(false);

    const handleOpenForm = () => {
        setFormVisible(true);
    };

    const handleCloseForm = () => {
        setFormVisible(false);
    };

    const toggleMenu = () => {
        setMenuOpen(!isMenuOpen);
    };

    return (
        <>
            <header>
                <div className='logo'>
                    <img src={logo} className="App-logo" alt="logo" />
                    <div className='logo-text'>
                        <p className='logo-text-1'>Cube</p>
                        <p>СТОМАТОЛОГИЧЕСКАЯ КЛИНИКА</p>
                    </div>
                </div>
                <button className="burger-menu" onClick={toggleMenu}>
                    ☰
                </button>
                <nav className={`nav ${isMenuOpen ? 'nav-open' : ''}`} >
                    <Link to="/clinic">Клиника</Link>
                    <Link to="/services">Услуги</Link>
                    <Link to="/specialists">Специалисты</Link>
                    <Link to="/contacts">Контакты</Link>
                <button className="appointment-button" onClick={handleOpenForm}>Записаться</button>
                </nav>
            </header>
            {isFormVisible && <Form onClose={handleCloseForm} />}
        </>
    );
}

export default Header;
