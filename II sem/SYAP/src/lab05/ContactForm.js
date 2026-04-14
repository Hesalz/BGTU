import React, {useEffect, useState} from 'react';
import './ContactFormStyle.css';

const ContactForm = ({addContacts}) => {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");
    const [gender, setGender] = useState("Не выбран");
    const [nameDirty, setNameDirty] = useState(false);
    const [nameError, setNameError] = useState("Имя не может быть пустым");
    const [emailDirty, setEmailDirty] = useState(false);
    const [emailError, setEmailError] = useState("Почта не может быть пустой");
    const [messageDirty, setMessageDirty] = useState(false);
    const [messageError, setMessageError] = useState("Сообщение не может быть пустым");
    const [formValid, setFormValid] = useState(false);
    const [formError, setFormError] = useState("");

    useEffect(() => {
        if(nameError && emailError && messageError) {
            setFormValid(false);
        }else{
            setFormValid(true); 
        }
    }, [nameError, emailError, messageError]);

    const nameHandler = (e) => {
        setName(e.target.value);
        const validate = /^[А-Я][а-яА-Я0-9]*$/.test(e.target.value);

        if(!validate){
            setNameError("Некорректное имя");
        }else {
            setNameError("");
        }
    }

    const emailHandler = (e) => {
        setEmail(e.target.value);
        const validate = String(e.target.value)
            .toLowerCase()
            .match(
                /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            );

        if(!validate){
            setEmailError("Некорректная почта");
        }else {
            setEmailError("");
        }
    }

    const messageHandler = (e) => {
        setMessage(e.target.value);
        const validate = /^[,.?!a-zA-Zа-яА-ЯёЁ0-9 \s]+$/u.test(e.target.value);

        if(!validate){
            setMessageError("Некорректное сообщение");
        }else {
            setMessageError("");
        }
    }
  
    const buttonHandler = (e) => {
        e.preventDefault();
        if(emailError || nameError || messageError || gender === "male") {
            setFormError("Форма заполнена некорректно");
            return;
        }
        addContacts({name: name, email: email, message: message, gender: gender});
        setGender("Не выбран");
        setMessage("");
        setEmail("");
        setName("");
        setEmailDirty(false);
        setNameDirty(false);
        setMessageDirty(false);
        setFormValid(false);
        setEmailError("Почта не может быть пустой");
        setNameError("Имя не может быть пустым");
        setMessageError("Сообщение не может быть пустым");
        setFormError("");
    }

    const blurNameHandler = (e) => {
        setNameDirty(true);
    }
    const blurEmailHandler = (e) => {
        setEmailDirty(true);
    }
    const blurMessageHandler = (e) => {
        setMessageDirty(true);
    }


    return (
        <form className='contact-form'>
            <div>
                <label htmlFor="name">Имя:</label>
                <input
                    id="name"
                    type="text"
                    value={name}
                    onBlur={e => blurNameHandler(e)}
                    onChange={e => nameHandler(e)}
                    required
                    />
                    {(nameDirty && nameError) && <div style={{color: "red"}}>{nameError}</div>}
            </div>
            <div>
                <label htmlFor="email">Почта:</label>
                <input
                    id="email"
                    type="email"
                    value={email}
                    onBlur={e => blurEmailHandler(e)}
                    onChange={e => emailHandler(e)}
                    required
                    />
                    {(emailDirty && emailError) && <div style={{color: "red"}}>{emailError}</div>}
            </div>
            <div>
                <label htmlFor="message">Сообщение:</label>
                <textarea
                    id="message"
                    value={message}
                    onBlur={e => blurMessageHandler(e)}
                    onChange={e => messageHandler(e)}
                    required
                    ></textarea>
                    {(messageDirty && messageError) && <div style={{color: "red"}}>{messageError}</div>}
            </div>
            <div>
                <label>Пол:</label>
                <div>
                    <label htmlFor="male">
                        <input
                            id="male"
                            name="gender"
                            type="radio"
                            value="Мужской"
                            checked={"Мужской" === gender}
                            onChange={e => setGender(e.target.value)}
                            required
                        />
                        Мужской
                    </label>
                    <label htmlFor="female">
                        <input
                            id="female"
                            name="gender"
                            type="radio"
                            value="Женский"
                            checked={"Женский" === gender}
                            onChange={e => setGender(e.target.value)}
                            required
                        />
                        Женский
                    </label>
                </div>
            </div>
            <button type="submit" disabled={!formValid} onClick={(e) => buttonHandler(e)}>Отправить</button>
            {formError && <div className="error" style={{color: "red"}}>{formError}</div>}
        </form>
    );
};

export default ContactForm;