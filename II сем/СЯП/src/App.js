import './App.css';
import React, { useState } from 'react'
import Chess from './lab01/Chess.js';
import Table from './lab01/Table.js';
import Calendar from './lab01/Calendar.js';
import Clock from './lab02/Clock.js';
import JobMenu from './lab02/JobMenu';
import Validmail from './lab03/validmail.js';
import Catalog from './lab03/CatalogApp.js';
import ToDoList from './lab04/ToDoList.js';
import Contacts from './lab05/Contacts.js';
import Colors from './lab06/colors.js';
import Comments from './lab07/comments.js';
import SortTable from './lab08/SortTable.js';
import OrderForm from './lab08/orderForm.js';

function App() {
  const [isVisible1, setIsVisible1] = useState(false);
  const [isVisible2, setIsVisible2] = useState(false);
  const [isVisible3, setIsVisible3] = useState(false);
  const [isVisible4, setIsVisible4] = useState(false);
  const [isVisible5, setIsVisible5] = useState(false);
  const [isVisible6, setIsVisible6] = useState(false);
  const [isVisible7, setIsVisible7] = useState(false);
  const [isVisible8, setIsVisible8] = useState(false);

  const Click1 = () => {
    setIsVisible1(!isVisible1);
  }
  const Click2 = () => {
    setIsVisible2(!isVisible2);
  }
  const Click3 = () => {
    setIsVisible3(!isVisible3);
  }

  const Click4 = () => {
    setIsVisible4(!isVisible4);
  }

  const Click5 = () => {
    setIsVisible5(!isVisible5);
  }
  
  const Click6 = () => {
    setIsVisible6(!isVisible6);
  }

  const Click7 = () => {
    setIsVisible7(!isVisible7);
  }

  const Click8 = () => {
    setIsVisible8(!isVisible8);
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
          <div>
            <button onClick={Click3}>Лабараторная работа №3</button>
          </div>
          <div>
            <button onClick={Click4}>Лабараторная работа №4</button>
          </div>
          <div>
            <button onClick={Click5}>Лабараторная работа №5</button>
          </div>
          <div>
            <button onClick={Click6}>Лабараторная работа №6</button>
          </div>
          <div>
            <button onClick={Click7}>Лабараторная работа №7</button>
          </div>
          <div>
            <button onClick={Click8}>Лабараторная работа №8-10</button>
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
          <div>
            <h1>1 задание</h1>
            <Clock format='24' timezone='+3:00' />
            <Clock format='12' timezone='-5:00' />
            <h1>2 задание</h1>
            <JobMenu />
          </div>
        )}
      </div>


      <div>
        {isVisible3 && (
          <div>
            <h1>1 задание</h1>
            <Validmail />
            <h1>2 задание</h1>
            <Catalog />
          </div>
        )}
      </div>

      <div>
        {isVisible4 && (
          <div>
            <ToDoList />          
          </div>
        )}
      </div>
      

      <div>
        {isVisible5 && (
          <div>
            <Contacts />          
          </div>
        )}
      </div>

      <div>
        {isVisible6 && (
          <div>
            <Colors />          
          </div>
        )}
      </div>

      <div>
        {isVisible7 && (
          <div>
            <Comments />          
          </div>
        )}
      </div>
      
      <div>
        {isVisible8 && (
          <div>
            <SortTable />     
            <OrderForm />     
          </div>
        )}
      </div>
    </>
  );
}

export default App;