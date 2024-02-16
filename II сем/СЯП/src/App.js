import './App.css';
import React, { useState } from 'react'
import Chess from './lab01/Chess.js';
import Table from './lab01/Table.js';
import Calendar from './lab01/Calendar.js';

function App() {
  const [isVisible1, setIsVisible1] = useState(false);
  const [isVisible2, setIsVisible2] = useState(false);

  const Click1 = () => {
    setIsVisible1(!isVisible1);
  }
  const Click2 = () => {
    setIsVisible2(!isVisible2);
  }

  return (
    <>
      <header>
        <div id='but-cont'>
          <div>
            <button onClick={Click1}>Лабараторная работа №1</button>
          </div>
          <div>
            <button onClick={Click2}>Лабараторная работа №2</button>
          </div>
        </div>
      </header>

      <div>
        {isVisible1 && (
          <div>
            <h1>1 задание</h1>
            <Calendar />
            <h1>2 задание</h1>
            <Table />
            <h1>3 задание</h1>
            <Chess />
          </div>
        )}
      </div>

      <div>
        {isVisible2 && (
          <div></div>
        )
        }
      </div>

    </>
  );
}

export default App;