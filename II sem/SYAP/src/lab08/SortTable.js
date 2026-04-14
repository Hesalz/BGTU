import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import './SortTable.css';
import lupa from './lupa.png';

const Catalog = () => {
  const products = [
    {
      id: 1,
      name: 'Rolex Collection MTP-1374D-1A',
      price: 350,
      quantity: 100,
      image: 'https://www.tempus.by/upload/iblock/089/s4tz2o0xhs0sgrnwe1xpwbun09c54icn.jpeg',
      description: 'Многофункциональные мужские наручные часы...',
      isNew: true,
      discount: 10,
      mass: 55
    },
    {
      id: 2,
      name: 'Casio Collection MTS-100L-1A',
      price: 400,
      quantity: 55,
      image: 'https://www.tempus.by/upload/resize_cache/iblock/c5f/2000_2000_140cd750bba9870f18aada2478b24840a/w5gq18nv0x1sund4jfhbczrqcs5h78jk.jpeg',
      description: 'Мужские наручные часы. Точный кварцевый механизм. Центральная секундная стрелка. Окошко даты. Черный циферблат. Сапфировое стекло. Круглый металлический корпус с полированной поверхностью. Кожаный ремешок черного цвета.',
      isNew: false,
      discount: 0,
      mass: 75
    },
    {
      id: 3,
      name: 'Casio Collection MTP-VD01D-1E2',
      price: 300,
      quantity: 200,
      image: 'https://www.tempus.by/upload/iblock/4a2/g57zyddeafy40xt3m4qx9fkg9oofykrv.jpeg',
      description: 'Мужские наручные часы Casio Collection MTP-VD01D-1E2. Точный кварцевый механизм с аналоговой индикацией. Корпус изготовлен из высококачественной часовой латуни с высокими антикоррозийными свойствами. На стрелки и отметки нанесен светонакопительный состав, благодаря этому считывать информацию с циферблата возможно и в темноте. На циферблате отображается текущее число месяца. Браслет из нержавеющей стали. Прочное, устойчивое к царапинам минеральное стекло защищает часы от повреждений.Часы являются водонепроницаемыми до 5 Бар.',
      isNew: false,
      discount: 0,
      mass: 85
    },
    {
      id: 4,
      name: 'Casio Collection MTP-V004D-1B2',
      price: 280,
      quantity: 120,
      image: 'https://www.tempus.by/upload/iblock/e86/e2hau87f0voknl3pszi5eo83rflx7qn4.jpeg',
      description: 'Мужские наручные часы Casio Collection MTP-V004D-1B2. Точный кварцевый механизм с аналоговой индикацией. Корпус изготовлен из высококачественной часовой латуни с высокими антикоррозийными свойствами. На стрелки и отметки нанесен светонакопительный состав, благодаря этому считывать информацию с циферблата возможно и в темноте. На циферблате отображается текущее число месяца. Браслет из нержавеющей стали. Прочное, устойчивое к царапинам минеральное стекло защищает часы от повреждений. Часы являются водонепроницаемыми до 3 Бар.',
      isNew: true,
      discount: 5,
      mass: 105
    },
    {
      id: 5,
      name: 'Casio Edifice EFV-560D-2A',
      price: 800,
      quantity: 145,
      image: 'https://www.tempus.by/upload/iblock/db4/u5doud8t89i9esva3p1mz6ybdfe4v2c2.jpeg',
      description: 'Мужские часы со спортивным дизайном. Круглый корпус и браслет из нерж. стали. Минеральное стекло. Водозащита 10 АТМ. Японский кварцевый механизм с функцией хронографа и индикатором времени суток. Японский кварцевый механизм, модуль 5451, с аналоговым отображением. Точность хода с погрешностью не более ±20 секунд в месяц. Механизм часов работает от элемента питания SR920SW, рассчитанного минимум на 3 лет работы.',
      isNew: false,
      discount: 0,
      mass: 90
    },
    {
      id: 6,
      name: 'Casio Edifice EFV-550GY-8A',
      price: 1000,
      quantity: 30,
      image: 'https://www.tempus.by/upload/iblock/6b8/ztd4qrq43sagh7ququntbli68kn7ldgr.jpeg',
      description: 'Мужские часы со спортивным дизайном. Круглый корпус и браслет из нерж. стали. Минеральное стекло. Водозащита 10 АТМ. Японский кварцевый механизм с функцией хронографа и индикатором времени суток. Японский кварцевый механизм, модуль 5451, с аналоговым отображением. Точность хода с погрешностью не более ±20 секунд в месяц. Механизм часов работает от элемента питания SR920SW, рассчитанного минимум на 3 лет работы.',
      isNew: false,
      discount: 12,
      mass: 70
    },
    {
      id: 7,
      name: 'Casio G-Shock GM-2100B-4A',
      price: 1000,
      quantity: 7,
      image: 'https://www.tempus.by/upload/iblock/c72/rj32rfiyd3j0ncb07r09i890en88ed62.jpeg',
      description: 'Мужские кварцевые часы из серии G-Shock. Комбинированный круглый корпус из нержавеющей стали и пластика. Красный аналого-цифровой циферблат. Минеральное стекло. Водозащита 200 м. Пластиковый черный ремешок с системой быстрой замены. Простая пряжка.',
      isNew: true,
      discount: 0,
      mass: 85
    }
  ];
  const [filteredProducts, setFilteredProducts] = useState(products);
  const dispatch = useDispatch();

  const handleSearch = (searchText) => {
    const filtered = products.filter(product =>
      product.name.toLowerCase().includes(searchText.toLowerCase())
    );
    setFilteredProducts(filtered);
  };

  const handleAddToCart = (product) => {
    dispatch({ type: 'ADD_TO_CART', payload: { ...product, quantity: 1 } });
  };

  const SortTable = ({ products }) => {
    const [sortColumn, setSortColumn] = useState(null);
    const [sortDirection, setSortDirection] = useState('asc');

    const handleSort = (column) => {
      if (sortColumn === column) {
        setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
      } else {
        setSortColumn(column);
        setSortDirection('asc');
      }
    };

    const sortedProducts = [...products].sort((a, b) => {
      if (a.isNew !== b.isNew) {
        return a.isNew ? -1 : 1;
      }
      if (sortColumn === 'name') {
        return sortDirection === 'asc'
          ? a.name.localeCompare(b.name)
          : b.name.localeCompare(a.name);
      }
      if (sortColumn === 'price') {
        return sortDirection === 'asc' ? a.price - b.price : b.price - a.price;
      }
      if (sortColumn === 'quantity') {
        return sortDirection === 'asc' ? a.quantity - b.quantity : b.quantity - a.quantity;
      }
      if (sortColumn === 'discount') {
        return sortDirection === 'asc' ? a.discount - b.discount : b.discount - a.discount;
      }
      return 0;
    });

    const getSortIndicator = (column) => {
      if (sortColumn === column) {
        return sortDirection === 'asc' ? '▲' : '▼';
      }
      return null;
    };
    return (
      <table>
        <thead>
          <tr>
            <th className='new' onClick={() => handleSort('isNew')}>
              Новинка {getSortIndicator('isNew')}
            </th>
            <th className='name' onClick={() => handleSort('name')}>
              Наименование {getSortIndicator('name')}
            </th>
            <th className='price' onClick={() => handleSort('price')}>
              Стоимость {getSortIndicator('price')}
            </th>
            <th className='pricedisc'>Стоимость со скидкой</th>
            <th className='amount' onClick={() => handleSort('quantity')}>
              Количество {getSortIndicator('quantity')}
            </th>
            <th>Изображение</th>
            <th>Описание</th>
            <th className='discount' onClick={() => handleSort('discount')}>
              Скидка {getSortIndicator('discount')}
            </th>
            <th className='mass'>Масса</th>
          </tr>
        </thead>
        <tbody>
          {sortedProducts.map((product, index) => (
            <tr key={index}>
              <td>{product.isNew ? 'Да' : 'Нет'}</td>
              <td>{product.name}</td>
              <td>{product.price}</td>
              <td>{product.price - (product.price * product.discount) / 100}</td>
              <td>{product.quantity}</td>
              <td>
                <img src={product.image} alt="Изображение" />
              </td>
              <td>{product.description}</td>
              <td>{product.discount}%</td>
              <td>{product.mass} г</td>
              <td>
                <button onClick={() => handleAddToCart(product)}>Добавить в корзину</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  };

  return (
    <div>
      <h2 style={{ marginLeft: '25px', marginBottom: '0' }}>Каталог товаров</h2>
      <Search products={products} onSearch={handleSearch} />
      <SortTable products={filteredProducts} />
    </div>
  );
};

const Search = ({ products, onSearch }) => {
  const [searchText, setSearchText] = useState('');

  const handleSearch = () => {
    onSearch(searchText);
  };

  return (
    <div className='search'>
      <input
        type="text"
        placeholder="Поиск по наименованию..."
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
      />
      <img className='lupa' src={lupa} onClick={handleSearch} alt='лупа' />
    </div>
  );
};

export default Catalog;
