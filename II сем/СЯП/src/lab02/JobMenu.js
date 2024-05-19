import { useState } from 'react';
import React from 'react';
import './JobMenu.css';
import Button from './Button';

function JobMenu() {
    const [content, setContent] = useState(<Frontend />)

    function buttonClick(Component) {
        setContent(Component);
    }

    return (
        <main>
            <div className='ContainerJobMenu'>
                <Button onClick={() => buttonClick(<Frontend />)}>Frontend developer </Button>
                <Button onClick={() => buttonClick(<Backend />)}>Backend developer</Button>
                <Button onClick={() => buttonClick(<IOS />)}>IOS developer</Button>
                <Button onClick={() => buttonClick(<JAVA />)}>JAVA developer</Button>
                <Button onClick={() => buttonClick(<PYTHON />)}>PHYTON developer</Button>
            </div>
            {content}
        </main>
    )
}

function Frontend() {
    return (
            <ul className='listFrontend'>
                <li>Курсы HTML и CSS</li>
                <li>Курсы JS</li>
                <li>Курсы React JS</li>
                <li>Языки</li>
                <li>Фреймворки</li>
                <li>Онлайн-курсы</li>
                <li>Время на сон</li>
            </ul>
    )
}

function Backend() {
    return (
        <ul className='listBackend'>
            <li>Куры nodeJS</li>
            <li>Книги</li>
            <li>Вакансии</li>
            <li>Языки</li>
            <li>Фреймворки</li>
            <li>Онлайн-курсы</li>
            <li>Время на сон</li>
        </ul>
    )
}
function IOS() {
    return (
        <ul className='listIOS'>
            <li>Куры SWIFT</li>
            <li>Книги</li>
            <li>Вакансии</li>
            <li>Статья "Что такое кроссплотферменная разработка?"</li>
            <li>Фреймворки</li>
            <li>Онлайн-курсы</li>
            <li>Время на сон</li>
        </ul>
    )
}

function JAVA() {
    return (
        <ul className='listJAVA'>
            <li>Куры JAVA</li>
            <li>Книги</li>
            <li>Вакансии</li>
            <li>Языки</li>
            <li>Фреймворки</li>
            <li>Онлайн-курсы</li>
            <li>Время на сон</li>
        </ul>
    )
}

function PYTHON() {
    return (
        <ul className='listPYTHON'>
            <li>Куры PHYTON</li>
            <li>Книги</li>
            <li>Вакансии</li>
            <li>Языки</li>
            <li>Фреймворки</li>
            <li>Онлайн-курсы</li>
            <li>Время на сон</li>
        </ul>
    )
}

export default JobMenu;