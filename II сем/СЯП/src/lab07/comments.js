import React, { useState } from 'react';
import './comments.css';

const Comments = () => {
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState({
    username: '',
    email: '',
    avatar: '',
    text: '',
  });
  const [editingIndex, setEditingIndex] = useState(null);
  const [editedComment, setEditedComment] = useState({
    username: '',
    email: '',
    avatar: '',
    text: '',
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setNewComment({ ...newComment, [name]: value });
  };

  const handleAddComment = () => {
    const secretWord = generateSecretWord();
    alert('Ваш секретный код: ' + secretWord);
    console.log('secretWord: ' + secretWord);
    const updatedComments = [...comments, { ...newComment, secretWord }];
    setComments(updatedComments);
    setNewComment({
      username: '',
      email: '',
      avatar: '',
      text: '',
    });
  };

  const handleEditComment = (index) => {
    setEditingIndex(index);
    setEditedComment(comments[index]);
  };

  const handleSaveEdit = () => {
    const updatedComments = [...comments];
    updatedComments[editingIndex] = editedComment;
    setComments(updatedComments);
    setEditingIndex(null);
    setEditedComment({
      username: '',
      email: '',
      avatar: '',
      text: '',
    });
  };

  const handleCancelEdit = () => {
    setEditingIndex(null);
    setEditedComment({
      username: '',
      email: '',
      avatar: '',
      text: '',
    });
  };

  const handleDeleteComment = (index) => {
    const confirmWord = prompt('Введите секретный код для удаления этого комментария:');
    if (confirmWord === comments[index].secretWord) {
      const updatedComments = [...comments];
      updatedComments.splice(index, 1);
      setComments(updatedComments);
    } else {
      alert('Секретный код неверный!');
    }
  };

  
const generateSecretWord = () => {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const length = 10;
    let secretWord = '';
    for (let i = 0; i < length; i++) {
      secretWord += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return secretWord;
  };

  return (
    <div className="comments">
        <div className="add-comment">
        <input
          type="text"
          name="username"
          placeholder="Имя пользователя"
          value={newComment.username}
          onChange={handleInputChange}
        />
        <input
          type="email"
          name="email"
          placeholder="Почта"
          value={newComment.email}
          onChange={handleInputChange}
        />
        <input
          type="text"
          name="avatar"
          placeholder="URL Аватара"
          value={newComment.avatar}
          onChange={handleInputChange}
        />
        <textarea
          name="text"
          placeholder="Текст комментария"
          value={newComment.text}
          onChange={handleInputChange}
        />
        <button onClick={handleAddComment}>Добавить комментарий</button>
      </div>

        <h3>Комментарии</h3>
      {comments.map((comment, index) => (
          <div className="com" key={index}>
            <div className='avatar-name-email'>
            <img className="avatar" src={comment.avatar} alt="Avatar" />
            <div className='name-email'>
            <strong>{comment.username}</strong>
            <p className="com-email">{comment.email}</p>
            </div></div>
           
          {editingIndex === index ? (
              <div>
              <input
                type="text"
                name="username"
                value={editedComment.username}
                onChange={(e) => setEditedComment({ ...editedComment, username: e.target.value })}
              />
              <input
                type="email"
                name="email"
                value={editedComment.email}
                onChange={(e) => setEditedComment({ ...editedComment, email: e.target.value })}
              />
              <input
                type="text"
                name="avatar"
                value={editedComment.avatar}
                onChange={(e) => setEditedComment({ ...editedComment, avatar: e.target.value })}
              />
              <textarea
                name="text"
                value={editedComment.text}
                onChange={(e) => setEditedComment({ ...editedComment, text: e.target.value })}
              />
            </div>
          ) : (
                <p className="com-text">{comment.text}</p>
          )}
           {editingIndex === index ? (
                <>
                <button className='save' onClick={handleSaveEdit}>Сохранить</button>
                <button className='cancel' onClick={handleCancelEdit}>Отмена</button>
              </>
            ) : (
                <button className='edit' onClick={() => handleEditComment(index)}>Редактировать</button>
                )}
            <button className='delete' onClick={() => handleDeleteComment(index)}>Удалить</button>
        </div>
        
      ))}
    </div>
  );
};

export default Comments;