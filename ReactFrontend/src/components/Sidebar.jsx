import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { 
  LayoutDashboard, 
  Users, 
  Layers, 
  Laptop, 
  UserCheck, 
  FileText, 
  Wrench, 
  History, 
  Send,
  LogOut
} from 'lucide-react';

const Sidebar = () => {
  const { user, isAdmin, logout } = useAuth();

  return (
    <aside className="sidebar shadow-sm">
      <div className="p-4 border-bottom border-secondary border-opacity-25 d-flex align-items-center gap-3">
        <div className="bg-primary text-white p-2 rounded-3 d-flex align-items-center justify-content-center" style={{ width: '40px', height: '40px' }}>
          <Laptop size={22} />
        </div>
        <div>
          <h6 className="fw-bold mb-0 text-white">AssetTrack</h6>
          <small className="text-muted" style={{ fontSize: '0.75rem' }}>IT Asset Manager</small>
        </div>
      </div>

      <div className="py-3 px-2 flex-grow-1 overflow-y-auto">
        <div className="px-3 mb-2 text-uppercase text-muted fw-semibold" style={{ fontSize: '0.7rem', letterSpacing: '0.05em' }}>
          Menu
        </div>

        {isAdmin ? (
          <>
            <NavLink to="/admin/dashboard" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <LayoutDashboard size={18} />
              <span>Dashboard</span>
            </NavLink>
            <NavLink to="/admin/employees" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <Users size={18} />
              <span>Employees</span>
            </NavLink>
            <NavLink to="/admin/categories" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <Layers size={18} />
              <span>Categories</span>
            </NavLink>
            <NavLink to="/admin/assets" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <Laptop size={18} />
              <span>Assets</span>
            </NavLink>
            <NavLink to="/admin/assignments" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <UserCheck size={18} />
              <span>Assignments</span>
            </NavLink>
            <NavLink to="/admin/requests" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <FileText size={18} />
              <span>Asset Requests</span>
            </NavLink>
            <NavLink to="/admin/maintenance" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <Wrench size={18} />
              <span>Maintenance</span>
            </NavLink>
            <NavLink to="/admin/history" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <History size={18} />
              <span>Asset History</span>
            </NavLink>
          </>
        ) : (
          <>
            <NavLink to="/employee/dashboard" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <LayoutDashboard size={18} />
              <span>My Dashboard</span>
            </NavLink>
            <NavLink to="/employee/my-assets" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <Laptop size={18} />
              <span>My Assigned Assets</span>
            </NavLink>
            <NavLink to="/employee/request-asset" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <Send size={18} />
              <span>Request New Asset</span>
            </NavLink>
            <NavLink to="/employee/my-requests" className={({ isActive }) => `nav-item-link ${isActive ? 'active' : ''}`}>
              <FileText size={18} />
              <span>My Requests</span>
            </NavLink>
          </>
        )}
      </div>

      <div className="p-3 border-top border-secondary border-opacity-25">
        <div className="d-flex align-items-center justify-content-between">
          <div className="d-flex align-items-center gap-2 overflow-hidden">
            <div className="bg-secondary text-white rounded-circle d-flex align-items-center justify-content-center fw-bold" style={{ width: '36px', height: '36px', flexShrink: 0 }}>
              {user?.name ? user.name.charAt(0).toUpperCase() : 'U'}
            </div>
            <div className="text-truncate">
              <div className="fw-medium text-white text-truncate" style={{ fontSize: '0.85rem' }}>{user?.name}</div>
              <div className="text-muted text-truncate" style={{ fontSize: '0.75rem' }}>{user?.role}</div>
            </div>
          </div>
          <button onClick={logout} className="btn btn-link text-muted p-1 hover-text-white" title="Logout">
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </aside>
  );
};

export default Sidebar;
