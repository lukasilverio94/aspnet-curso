import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import TaskDashboard from './components/TaskDashboard';
import './App.css';

function App() {
  return (
    <Router>
      <nav style={{ 
        padding: '1rem', 
        borderBottom: '1px solid #ccc', 
        marginBottom: '1rem',
        backgroundColor: '#f8f9fa'
      }}>
        <Link to="/" style={{ marginRight: '1rem', textDecoration: 'none' }}>Home</Link>
        <Link to="/dashboard" style={{ textDecoration: 'none' }}>Dashboard</Link>
      </nav>
      <div style={{ padding: '0 1rem' }}>
        <Routes>
          <Route path="/" element={<h2>Welcome to the Project Dashboard</h2>} />
          <Route path="/dashboard" element={<TaskDashboard />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;