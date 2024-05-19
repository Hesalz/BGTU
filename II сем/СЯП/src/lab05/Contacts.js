import ContactForm from "./ContactForm";
import ContactTable from "./ContactTable";
import React from 'react';
import {useState} from "react";

function Contacts() {
    const [contacts, setContacts] = useState([]);

    const addContacts = (contact) => {
        setContacts([...contacts, contact]);
    }

  return (
    <div>
        <ContactForm addContacts={addContacts}></ContactForm>
        <ContactTable contacts={contacts}></ContactTable>
    </div>
  );
}

export default Contacts;
