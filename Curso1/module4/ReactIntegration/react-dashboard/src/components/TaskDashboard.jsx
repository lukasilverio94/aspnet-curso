import React, { useState, useEffect } from 'react';
import './TaskDashboard.css';

const TaskDashboard = () => {
  const [tasks, setTasks] = useState([]);
  const [statusFilter, setStatusFilter] = useState('All');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch('/tasks')
      .then(res => res.json())
      .then(data => { setTasks(data); setLoading(false); })
      .catch(() => setLoading(false));
  }, []);

  const filtered = statusFilter === 'All' 
    ? tasks 
    : tasks.filter(t => t.status === statusFilter);

  if (loading) return <div className="loading">Loading tasks...</div>;

  return (
    <div className="task-dashboard">
      <h1>Project Dashboard</h1>
      <div className="controls">
        <label>Status:</label>
        <select value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
          <option value="All">All</option>
          <option value="Todo">To Do</option>
          <option value="InProgress">In Progress</option>
          <option value="Done">Done</option>
        </select>
      </div>
      <div className="grid">
        {filtered.map(task => (
          <div key={task.id} className="card">
            <h3>{task.title}</h3>
            <p>{task.description}</p>
            <p><strong>Status:</strong> {task.status}</p>
            <p><strong>Priority:</strong> {task.priority}</p>
            <p><strong>Assigned to:</strong> {task.assignedTo}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default TaskDashboard;