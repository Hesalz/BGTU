import React from "react"
import './Calendar.css'

function Calendar(){
    let data = new Date();
    return (
        <div><p>Текущая дата: {data.getDate()}.{data.getMonth() + 1}.{data.getFullYear()}</p></div>
    )
}
export default Calendar;