import { configureStore, createSlice } from '@reduxjs/toolkit';

const currencySlice = createSlice({
  name: 'currency',
  initialState: 'BYN',
  reducers: {
    setCurrency: (state, action) => action.payload,
  },
});

export const { setCurrency } = currencySlice.actions;

const store = configureStore({
  reducer: {
    currency: currencySlice.reducer,
  },
});

export default store;
