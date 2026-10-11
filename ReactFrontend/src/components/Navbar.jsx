import React from 'react';
import { useAuth } from '../context/AuthContext';
import { Bell, Search } from 'lucide-react';

const Navbar = ({ title = "Dashboard" }) => {
  const { user } = useAuth();

  return (
    <header className="top-navbar">
      <div className="d-flex align-items-center gap-3">
        <h5 className="fw-bold mb-0 text-dark">{title}</h5>
      </div>

      <div className="d-flex align-items-center gap-3">
        <span className="badge bg-primary-subtle text-primary border border-primary-subtle px-3 py-1.5 rounded-pill fw-medium">
          {user?.department || 'IT Operations'}
        </span>

        <div className="d-flex align-items-center gap-2 ps-3 border-start">
          <div className="text-end d-none d-sm-block">
            <div className="fw-semibold text-dark" style={{ fontSize: '0.9rem' }}>{user?.name}</div>
            <div className="text-muted" style={{ fontSize: '0.75rem' }}>{user?.email}</div>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Navbar;
