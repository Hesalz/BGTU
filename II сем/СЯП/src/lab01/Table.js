import React from 'react'

function Table() {

    const stocksData = [{
        stock_name: "EFX",
        company_name: "Equifax Inc",
        price: 163.55,
        currency: "USD",
        change: "+9.03"
    }, {
        stock_name: "IRM",
        company_name: "Iron Mountain Inc",
        price: 33.21,
        currency: "USD",
        change: "+1.42"
    }, {
        stock_name: "NTAP",
        company_name: "NetApp Inc",
        price: 54.81,
        currency: "USD",
        change: "-6.01"
    }, {
        stock_name: "CTL",
        company_name: "Centurylink Inc",
        price: 13.79,
        currency: "USD",
        change: "-1.37"
    }];
    return (
        <table border="1" style={{ borderCollapse: "collapse", textAlign: "center" }}>
            <thead>
                <tr>
                    <th> Stock Name </th>
                    <th> Company Name </th>
                    <th> Price </th>
                    <th> Currency </th>
                    <th> Change </th>
                </tr>
            </thead>
            <tbody>
                {stocksData.map((stock, index) => (
                    <tr key={index}>
                        <td>{stock.stock_name}</td>
                        <td>{stock.company_name}</td>
                        <td>{stock.price} </td>
                        <td>{stock.currency}</td>
                        <td style={{ color: parseFloat(stock.change) >= 0 ? "green" : "red" }} >{stock.change}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
};
export default Table;