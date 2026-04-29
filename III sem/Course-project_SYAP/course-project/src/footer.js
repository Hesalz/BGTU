import './footer.css';
import logo from './images/logo.png';

export default function Footer(){
    return(
        <>
            <footer>
                <div className='logo'>
                    <img src={logo} className="App-logo" alt="logo" />
                    <div className='logo-text'>
                        <p className='logo-text-1'>Cube</p>
                        <p>СТОМАТОЛОГИЧЕСКАЯ КЛИНИКА</p>
                    </div>
                </div>
                <h1>Курсовой проект</h1>
                <h3>Выполнил: <br/> Бабашинский Глеб Александрович <br/> ФИТ 2-1</h3>
            </footer>
        </>
    )
}