import { createStore, combineReducers } from 'redux';

const initialState = {
  cart: [],
  deliveryMethod: '',
  paymentMethod: '',
  deliveryAddress: '',
};

const cartReducer = (state = initialState.cart, action) => {
  switch (action.type) {
    case 'ADD_TO_CART':
      return [...state, action.payload];
    case 'REMOVE_FROM_CART':
      return state.filter(item => item.id !== action.payload);
    case 'UPDATE_QUANTITY':
      return state.map(item =>
        item.id === action.payload.id
          ? { ...item, quantity: action.payload.quantity }
          : item
      );
    case 'TOGGLE_CHECK':
      return state.map(item =>
        item.id === action.payload.id
          ? { ...item, checked: !item.checked }
          : item
      );
    default:
      return state;
  }
};

// Редьюсер для оформления заказа
const orderReducer = (state = initialState, action) => {
  switch (action.type) {
    case 'SET_DELIVERY_METHOD':
      return { ...state, deliveryMethod: action.payload };
    case 'SET_PAYMENT_METHOD':
      return { ...state, paymentMethod: action.payload };
    case 'SET_DELIVERY_ADDRESS':
      return { ...state, deliveryAddress: action.payload };
    default:
      return state;
  }
};

const rootReducer = combineReducers({
  cart: cartReducer,
  order: orderReducer,
});

const store = createStore(rootReducer);

export default store;
