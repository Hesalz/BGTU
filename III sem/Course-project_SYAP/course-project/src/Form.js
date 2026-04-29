import React, { useState } from 'react';
import './Form.css';

function Form({ onClose }) {
  const [formData, setFormData] = useState({
    name: '',
    phone: '',
    date: '',
    time: '',
    message: '',
    recipientEmail: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false); 

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
  
    const response = await fetch('http://localhost:5000/send-email', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(formData),
    });
  
    if (response.ok) {
      console.log('Письмо отправлено');
      onClose();
    } else {
      console.error('Ошибка при отправке данных');
    }

    setIsSubmitting(false);
  };

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h2>Записаться на прием</h2>
        <form onSubmit={handleSubmit}>
          <label>
            Имя:
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
            />
          </label>

          <label>
            Номер телефона:
            <input
              type="tel"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              required
              placeholder="+375 (___) ___-____"
            />
          </label>

          <label>
            Дата записи:
            <input
              type="date"
              name="date"
              value={formData.date}
              onChange={handleChange}
              required
            />
          </label>

          <label>
            Время записи:
            <input
              type="time"
              name="time"
              value={formData.time}
              onChange={handleChange}
              required
            />
          </label>

          <label>
            Комментарий:
            <input
              type="text"
              name="message"
              value={formData.message}
              onChange={handleChange}
              rows="4"
              placeholder="Укажите дополнительную информацию"
            />
          </label>

          <label>
            Email получателя:
            <input
              type="email"
              name="recipientEmail"
              value={formData.recipientEmail}
              onChange={handleChange}
              required
              placeholder="Введите email получателя"
            />
          </label>

          <div className="modal-buttons">
            <button 
              type="submit" 
              className="submit-button"
              disabled={isSubmitting} 
            >
              {isSubmitting ? 'Отправка...' : 'Записаться'}
            </button>
            <button type="button" className="close-button" onClick={onClose}>Закрыть</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default Form;
