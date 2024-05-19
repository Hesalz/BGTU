import Catalog from './Catalog.js';
import React from 'react';
import './Catalog.css';

export default function CatalogApp() {
    const products = [
        { id: 1, name: 'Коленвал', price: 25, quantity: 10 },
        { id: 2, name: 'Тормоза', price: 50, quantity: 5 },
        { id: 3, name: 'Трансмиссия', price: 80, quantity: 3 },
        { id: 4, name: 'Инжектор', price: 65, quantity: 2 },
        { id: 5, name: 'Карбюратор', price: 45, quantity: 0 },
        { id: 6, name: 'Диски', price: 35, quantity: 1 },
    ];

    return (
        <Catalog products={products}/>
    )
}

