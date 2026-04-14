import React, { useState } from 'react';
import './Catalog.css';

const Catalog = ({ products }) => {

    const [sortColumn, setSortColumn] = useState(null);
    const [sortOrder, setSortOrder] = useState('asc');

    const handleSort = (column) => {
        if (sortColumn === column) {
            setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
        } else {
            setSortColumn(column);
            setSortOrder('asc');
        }
    };

    const sortedProducts = products.sort((a, b) => {
        if (sortColumn === 'name') {
            return sortOrder === 'asc' ? a.name.localeCompare(b.name) : b.name.localeCompare(a.name);
        } else if (sortColumn === 'price') {
            return sortOrder === 'asc' ? a.price - b.price : b.price - a.price;
        }
        else if (sortColumn === 'quantity') {
            return sortOrder === 'asc' ? a.quantity - b.quantity : b.quantity - a.quantity;
        }
        else {
            return a.id - b.id;
        }
    });

    const totalQuantity = products.reduce((total, product) => total + product.quantity, 0);
    const totalPrice = products.reduce((total, product) => total + product.price * product.quantity, 0);

    return (
        <div>
            <table>
                <thead>
                    <tr>
                        <th>Номер строки</th>
                        <th onClick={() => handleSort('name')}>Название товара</th>
                        <th onClick={() => handleSort('price')}>Цена</th>
                        <th onClick={() => handleSort('quantity')}>Количество</th>
                    </tr>
                </thead>
                <tbody>
                    {sortedProducts.map((product, index) => (
                        <tr key={product.id} style={{ backgroundColor: product.quantity === 0 ? 'red' : (product.quantity < 3 ? 'yellow' : 'white') }}>
                            <td>{index + 1}</td>
                            <td>{product.name}</td>
                            <td>{product.price}</td>
                            <td>{product.quantity}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div>
                Общее количество товаров: {totalQuantity}
                <br />
                Общая стоимость: {totalPrice}
            </div>
        </div>
    );
};

export default Catalog;