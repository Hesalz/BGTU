import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { setCurrency } from './store';

const currencies = ['BYN', 'USD', 'EUR', 'RUB'];

export default function CurrencySwitcher() {
  const currentCurrency = useSelector((state) => state.currency);
  const dispatch = useDispatch();

  return (
    <div className='currency-switcher' style={{ display: 'flex' }}>
      {currencies.map((currency, index) => (
        <div
          key={currency}
          onClick={() => dispatch(setCurrency(currency))}
          style={{
            padding: '10px 20px',
            cursor: 'pointer',
            backgroundColor: currentCurrency === currency ? '#b5a9c6' : '#fff',
            color: currentCurrency === currency ? '#42305f' : '#333',
            borderRight: index !== currencies.length - 1 ? '1px solid #ddd' : 'none',
            borderTopLeftRadius: index === 0 ? '20px' : '0',
            borderTopRightRadius: index === currencies.length - 1 ? '20px' : '0',
            borderBottomLeftRadius: index === 0 ? '20px' : '0',
            borderBottomRightRadius: index === currencies.length - 1 ? '20px' : '0',
          }}
        >
          {currency}
        </div>
      ))}
    </div>
  );
}
