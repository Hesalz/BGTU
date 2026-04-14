import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Clinic from './clinic';
import Contacts from './contacts';
import Services from './services';
import Specialists from './specialists';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Clinic />} />
        <Route path="/clinic" element={<Clinic />} />
        <Route path="/services" element={<Services />} />
        <Route path="/specialists" element={<Specialists />} />
        <Route path="/contacts" element={<Contacts />} />
      </Routes>
    </Router>
  );
}

export default App;
