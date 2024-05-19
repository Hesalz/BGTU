import React, { useState } from 'react'; 
 
function ToDoForm({ onSubmit }) { 
  const [task, setTask] = useState(''); 
 
  const Change = (event) => { 
    setTask(event.target.value); 
  }; 
 
  const Submit = (event) => { 
    event.preventDefault(); 
    if (task.trim()) { 
      onSubmit(task);
      setTask('');
    } 
  }; 
 
  return ( 
    <form onSubmit={Submit}> 
      <input type="text" value={task} onChange={Change} placeholder="Введите задачу"/> 
      <button type="submit">Add</button> 
    </form> 
  ); 
} 

function ToDoItems({ tasks, onToggle }) { 
  return ( 
    <ul> 
      {tasks.map((task, index) => ( 
        <li key={index}> 
          <input type="checkbox" checked={task.completed} onChange={() => onToggle(index)}/> 
          <span style={{ textDecoration: task.completed ? 'line-through' : 'none' }}> 
            {task.title} 
          </span> 
        </li> 
      ))} 
    </ul> 
  ); 
} 
 
function ToDoList() { 
  const [tasks, setTasks] = useState([]); 
  const [showTasks, setShowTasks] = useState(false);
  const [filter, setFilter] = useState('all');
  const [filteredTasks, setFilteredTasks] = useState([]);
 
  const addTask = (title) => { 
    setTasks([...tasks, { title, completed: false }]); 
  }; 
 
  const toggleTask = (index) => { 
    const newTasks = [...tasks]; 
    newTasks[index].completed = !newTasks[index].completed; 
    setTasks(newTasks); 
  }; 
 
  const ShowTasks = () => { 
    setShowTasks(true); 
    filterTasks(); 
  }; 
 
  const FilterChange = (event) => { 
    setFilter(event.target.value); 
  }; 
 
  const filterTasks = () => { 
    let filteredTasks = tasks; 
    switch (filter) { 
      case 'Выполненные задачи': 
        filteredTasks = tasks.filter(task => task.completed); 
        break; 
      case 'Невыполненные задачи': 
        filteredTasks = tasks.filter(task => !task.completed); 
        break; 
      default: 
        break; 
    } 
    setFilteredTasks(filteredTasks); 
  }; 
 
  return ( 
    <div> 
      <ToDoForm onSubmit={addTask} /> 
      <button onClick={ShowTasks}>Submit</button> 
      <select value={filter} onChange={FilterChange}> 
        <option value="Все задачи">Все задачи</option> 
        <option value="Выполненные задачи">Выполненные задачи</option> 
        <option value="Невыполненные задачи">Невыполненные задачи</option> 
      </select> 
      {showTasks && <ToDoItems tasks={filteredTasks} onToggle={toggleTask} />} 
    </div> 
  ); 
} 
 
export default ToDoList;