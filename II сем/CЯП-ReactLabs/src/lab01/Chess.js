import './Chess.css'
import React from 'react'
function Chess() {

  const letters = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']
  const numbers = ['1', '2', '3', '4', '5', '6', '7', '8']

  const board = []

  for (let j = numbers.length - 1; j >= 0; j--) {
      for (let i = 0; i < letters.length; i++) {
          const number = (i + j) % 2 === 0 ? 'black' : 'white'
          board.push(
              <div className={number}></div>
          )
      }
  }

  return (
      <div className='chess'>
          
          <div className='letters'>
              {letters.map((letter, index) => (
                  <div key={index}>{letter}</div>
              ))}
          </div>

      <div className='board-container'>

          <div className="numbers">
              {numbers.map((number, index) => (
                  <div key={index}>{number}</div>
              ))}
          </div>


          <div className="board">{board}</div>
          

          <div className="numbers">
              {numbers.map((number, index) => (
                  <div key={index}>{number}</div>
              ))}
          </div>

      </div>

      <div className='letters'>
              {letters.map((letter, index) => (
                  <div key={index}>{letter}</div>
              ))}
          </div>
          
          
      </div>
  )
}

export default Chess;