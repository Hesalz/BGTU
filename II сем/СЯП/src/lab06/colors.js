import React, { useState, useEffect } from 'react'; 
import './colors.css'
 
const Colors = () => { 
  const [selectedColor, setColor] = useState(''); 
  const [savedColors, setSave] = useState(() => { 
    const Local = localStorage.getItem('savedColors'); 
    const saved = (JSON.parse(Local)); 
    return saved ? saved : [];  
  }); 
 
  useEffect(() => { 
    localStorage.setItem('savedColors', JSON.stringify(savedColors)); 
  }, [savedColors]); 
 
  const ChangeColor = (color) => { 
    setColor(color); 
    document.getElementById('colordiv').style.backgroundColor = color; 
  }; 
 
  const Submit = (event) => { 
    event.preventDefault(); 
  }; 
 
  const Save = (selectedColor) => { 
    setSave([...savedColors, selectedColor]); 
  } 
 
  return ( 
    <div className="color"> 
      <form onSubmit={Submit}> 
        <button type="submit" className="submit" id="red" onClick={() => ChangeColor('red')}></button> 
        <button type="submit" className="submit" id="yellow" onClick={() => ChangeColor('yellow')}></button> 
        <button type="submit" className="submit" id="purple" onClick={() => ChangeColor('purple')}></button> 
        <button type="submit" className="submit" id="green" onClick={() => ChangeColor('green')}></button> 
        <button type="submit" className="submit" id="brown" onClick={() => ChangeColor('brown')}></button> 
        <button type="submit" className="submit" id="pink" onClick={() => ChangeColor('pink')}></button> 
      </form> 
      <button type="submit" className="submit" id="color" onClick={() => ChangeColor('')}>Сброс</button> 
      <button type="submit" className="submit" id="color" onClick={() => Save(selectedColor)}>Сохранить</button> 

      <div id="colordiv"></div>
      {savedColors.length > 0 && ( 
        <div id='divdiv'> 
          <h3>Сохранённые цвета: </h3> 
            {savedColors.map((color, index) => (<div key={index} style={{ backgroundColor: color, color: color}} id='div'> 
                {color} 
              </div> 
            ))} 
        </div> 
      )} 
    </div> 
  ); 
}; 
 
export default Colors;