import {useState} from 'react';
import React from 'react';

export default function App() {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState(null);

    function isValidEmail(email) {
        return /\S+@\S+\.\S+/.test(email);
    }

    const handleChange = event => {
        setEmail(event.target.value);
    };

    const handleSubmit = event => {
        event.preventDefault();

        setMessage(null);

        if (isValidEmail(email)) {
            setMessage('Почта валидна.');
        } else {
            setMessage('Почта не валидна.');
        }
    };

    return (
        <div>
            <form onSubmit={handleSubmit}>

                <input
                    id="message"
                    name="message"
                    value={email}
                    onChange={handleChange}
                />
            <br />
                <button type="submit" class="sub" >Submit</button>
                {message && <h2>{message}</h2>}

            </form>
        </div>
    );
}