import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import './orderForm.css';

const OrderForm = () => {
  const cart = useSelector(state => state.cart);
  const order = useSelector(state => state.order);
  const dispatch = useDispatch();
  const [currentPage, setCurrentPage] = useState(1);

  const [courierCost, setCourierCost] = useState(0);
  const [postCost, setPostCost] = useState(0);
  const [pickupCost, setPickupCost] = useState(0);
  const [totalCost, setTotalCost] = useState(0);

  useEffect(() => {
    const totalPrice = cart.reduce((sum, item) => sum + item.quantity * item.price, 0);
    const totalWeight = cart.reduce((sum, item) => sum + item.mass * item.quantity, 0);

    const courierDeliveryCost = totalPrice <= 200 ? 10 : 0;
    const postDeliveryCost = totalWeight * 5;
    const pickupDeliveryCost = 0;

    setCourierCost(courierDeliveryCost);
    setPostCost(postDeliveryCost);
    setPickupCost(pickupDeliveryCost);

    let selectedDeliveryCost = 0;
    if (order.deliveryMethod === 'courier') {
      selectedDeliveryCost = courierDeliveryCost;
    } else if (order.deliveryMethod === 'post') {
      selectedDeliveryCost = postDeliveryCost;
    } else if (order.deliveryMethod === 'pickup') {
      selectedDeliveryCost = pickupDeliveryCost;
    }

    setTotalCost(totalPrice + selectedDeliveryCost);
  }, [cart, order.deliveryMethod]);

  const handleNextPage = () => setCurrentPage(currentPage + 1);
  const handlePrevPage = () => setCurrentPage(currentPage - 1);

  const handleCheckboxChange = (id) => {
    dispatch({ type: 'TOGGLE_CHECK', payload: { id } });
  };

  const handleQuantityChange = (id, quantity) => {
    dispatch({ type: 'UPDATE_QUANTITY', payload: { id, quantity } });
  };

  const handleRemoveFromCart = (id) => {
    dispatch({ type: 'REMOVE_FROM_CART', payload: id });
  };

  const handleDeliveryMethodChange = (method) => {
    dispatch({ type: 'SET_DELIVERY_METHOD', payload: method });
  };

  const handlePaymentMethodChange = (method) => {
    dispatch({ type: 'SET_PAYMENT_METHOD', payload: method });
  };

  const handleDeliveryAddressChange = (address) => {
    dispatch({ type: 'SET_DELIVERY_ADDRESS', payload: address });
  };

  return (
    <div>
      {currentPage === 1 && (
        <div className='cart'>
          <h2>Корзина</h2>
          <ul>
            {cart.map((product) => (
              <li key={product.id}>
                <input
                  type="checkbox"
                  checked={product.checked || false}
                  onChange={() => handleCheckboxChange(product.id)}
                />
                <span>{product.name}</span>
                <input
                  type="number"
                  value={product.quantity}
                  onChange={(e) => handleQuantityChange(product.id, parseInt(e.target.value))}
                  min="1"
                  max={product.stock}
                />
                <button onClick={() => handleRemoveFromCart(product.id)}>Удалить</button>
              </li>
            ))}
          </ul>
          <button onClick={handleNextPage}>Далее</button>
        </div>
      )}

      {currentPage === 2 && (
        <div className='cart2'>
          <h2>Доставка и Оплата</h2>
          <div className='delivery'>
            <h3>Способ доставки</h3>
            <label>
              <input
                type="radio"
                name="delivery"
                value="courier"
                checked={order.deliveryMethod === 'courier'}
                onChange={(e) => handleDeliveryMethodChange(e.target.value)}
              />
              Курьером (стоимость 10 рублей при заказе от 200 рублей) - {courierCost} рублей
            </label>
            <label>
              <input
                type="radio"
                name="delivery"
                value="post"
                checked={order.deliveryMethod === 'post'}
                onChange={(e) => handleDeliveryMethodChange(e.target.value)}
              />
              Почтой (5 рублей за каждый килограмм массы) - {postCost} рублей
            </label>
            <label>
              <input
                type="radio"
                name="delivery"
                value="pickup"
                checked={order.deliveryMethod === 'pickup'}
                onChange={(e) => handleDeliveryMethodChange(e.target.value)}
              />
              Самовывоз (бесплатно) - {pickupCost} рублей
            </label>
          </div>
          <div>
            <h3>Адрес доставки</h3>
            <input
              type="text"
              value={order.deliveryAddress}
              onChange={(e) => handleDeliveryAddressChange(e.target.value)}
              disabled={order.deliveryMethod === 'pickup'}
            />
          </div>
          <div className='payment'>
            <h3>Способ оплаты</h3>
            <label>
              <input
                type="radio"
                name="payment"
                value="cash"
                checked={order.paymentMethod === 'cash'}
                onChange={(e) => handlePaymentMethodChange(e.target.value)}
              />
              Наличными
            </label>
            <label>
              <input
                type="radio"
                name="payment"
                value="card"
                checked={order.paymentMethod === 'card'}
                onChange={(e) => handlePaymentMethodChange(e.target.value)}
              />
              Банковской картой
            </label>
            <label>
              <input
                type="radio"
                name="payment"
                value="bankTransfer"
                checked={order.paymentMethod === 'bankTransfer'}
                onChange={(e) => handlePaymentMethodChange(e.target.value)}
              />
              Банковским переводом
            </label>
          </div>
          <div className='total-cost'>
            <h3>Итого: {totalCost} рублей</h3>
          </div>
          <button onClick={handlePrevPage}>Назад</button>
          <button onClick={handleNextPage}>Далее</button>
        </div>
      )}
    </div>
  );
};

export default OrderForm;
