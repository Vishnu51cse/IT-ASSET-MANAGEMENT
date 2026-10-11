import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Sidebar from './components/Sidebar';
import Navbar from './components/Navbar';

import Login from './pages/Login';
import AdminDashboard from './pages/AdminDashboard';
import EmployeeDashboard from './pages/EmployeeDashboard';
import EmployeesPage from './pages/EmployeesPage';
import CategoriesPage from './pages/CategoriesPage';
import AssetsPage from './pages/AssetsPage';
import AssetDetailPage from './pages/AssetDetailPage';
import AssetRequestsPage from './pages/AssetRequestsPage';
import MaintenancePage from './pages/MaintenancePage';
import AssetHistoryPage from './pages/AssetHistoryPage';

// Protected Route Component
const ProtectedRoute = ({ children, requireAdmin = false, pageTitle }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return <div className="d-flex align-items-center justify-content-center vh-100 text-muted">Loading Application...</div>;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (requireAdmin && user.role !== 'Admin') {
    return <Navigate to="/employee/dashboard" replace />;
  }

  return (
    <div className="app-layout">
      <Sidebar />
      <div className="main-wrapper">
        <Navbar title={pageTitle} />
        <main className="main-content">
          {children}
        </main>
      </div>
    </div>
  );
};

function AppRoutes() {
  const { user } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={user ? <Navigate to={user.role === 'Admin' ? "/admin/dashboard" : "/employee/dashboard"} replace /> : <Login />} />

      {/* Admin Routes */}
      <Route path="/admin/dashboard" element={<ProtectedRoute requireAdmin pageTitle="Admin Dashboard"><AdminDashboard /></ProtectedRoute>} />
      <Route path="/admin/employees" element={<ProtectedRoute requireAdmin pageTitle="Employee Directory"><EmployeesPage /></ProtectedRoute>} />
      <Route path="/admin/categories" element={<ProtectedRoute requireAdmin pageTitle="Asset Categories"><CategoriesPage /></ProtectedRoute>} />
      <Route path="/admin/assets" element={<ProtectedRoute requireAdmin pageTitle="Asset Catalog"><AssetsPage /></ProtectedRoute>} />
      <Route path="/admin/assets/:id" element={<ProtectedRoute requireAdmin pageTitle="Asset Audit Details"><AssetDetailPage /></ProtectedRoute>} />
      <Route path="/admin/assignments" element={<ProtectedRoute requireAdmin pageTitle="Asset Assignments"><AssetsPage /></ProtectedRoute>} />
      <Route path="/admin/requests" element={<ProtectedRoute requireAdmin pageTitle="Asset Requests"><AssetRequestsPage /></ProtectedRoute>} />
      <Route path="/admin/maintenance" element={<ProtectedRoute requireAdmin pageTitle="Asset Maintenance"><MaintenancePage /></ProtectedRoute>} />
      <Route path="/admin/history" element={<ProtectedRoute requireAdmin pageTitle="System Audit Log"><AssetHistoryPage /></ProtectedRoute>} />

      {/* Employee Routes */}
      <Route path="/employee/dashboard" element={<ProtectedRoute pageTitle="My Dashboard"><EmployeeDashboard /></ProtectedRoute>} />
      <Route path="/employee/my-assets" element={<ProtectedRoute pageTitle="My Assigned Assets"><EmployeeDashboard /></ProtectedRoute>} />
      <Route path="/employee/request-asset" element={<ProtectedRoute pageTitle="Request New Asset"><AssetRequestsPage /></ProtectedRoute>} />
      <Route path="/employee/my-requests" element={<ProtectedRoute pageTitle="My Requests"><AssetRequestsPage /></ProtectedRoute>} />

      {/* Fallback Catch-All */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <Router>
        <AppRoutes />
      </Router>
    </AuthProvider>
  );
}
