import React, {useEffect} from 'react';
import './ContactTableStyle.css';

const ContactTable = ({contacts}) => {

    useEffect(() => {
        console.log(contacts);
    }, [contacts]);

    return (
        <table className='contact-table'>
            <tr>
                <th>№</th>
                <th>Имя</th>
                <th>Почта</th>
                <th>Сообщение</th>
                <th>Пол</th>
            </tr>
            {contacts.length > 0 ? (
                contacts.map((contact, index) => (
                        <tr>
                            <th>{index}</th>
                            <td>{contact.name}</td>
                            <td>{contact.email}</td>
                            <td>{contact.message}</td>
                            <td>{contact.gender}</td>
                        </tr>
                ))
            ) : null}

        </table>
    );
};

export default ContactTable;