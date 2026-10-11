import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Laptop, Lock, Mail, ArrowRight, ShieldCheck, User } from 'lucide-react';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const authData = await login(email, password);
      if (authData.role === 'Admin') {
        navigate('/admin/dashboard');
      } else {
        navigate('/employee/dashboard');
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid email or password.');
    } finally {
      setLoading(false);
    }
  };

  const handleDemoFill = (demoRole) => {
    if (demoRole === 'Admin') {
      setEmail('admin@company.com');
      setPassword('Password123!');
    } else {
      setEmail('employee@company.com');
      setPassword('Password123!');
    }
  };

  return (
    <div className="login-bg">
      <div className="login-card">
        <div className="text-center mb-4">
          <div className="bg-primary text-white p-3 rounded-4 d-inline-flex mb-3 shadow-sm">
            <Laptop size={32} />
          </div>
          <h4 className="fw-bold text-dark mb-1">IT Asset Management</h4>
          <p className="text-muted" style={{ fontSize: '0.9rem' }}>Sign in to manage corporate hardware & software</p>
        </div>

        {error && (
          <div className="alert alert-danger py-2 px-3 mb-4 rounded-3 text-start" style={{ fontSize: '0.875rem' }}>
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label fw-semibold text-secondary" style={{ fontSize: '0.85rem' }}>Email Address</label>
            <div className="input-group">
              <span className="input-group-text bg-light text-muted border-end-0">
                <Mail size={18} />
              </span>
              <input
                type="email"
                className="form-control bg-light border-start-0 ps-0"
                placeholder="name@company.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="mb-4">
            <label className="form-label fw-semibold text-secondary" style={{ fontSize: '0.85rem' }}>Password</label>
            <div className="input-group">
              <span className="input-group-text bg-light text-muted border-end-0">
                <Lock size={18} />
              </span>
              <input
                type="password"
                className="form-control bg-light border-start-0 ps-0"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="btn btn-primary w-100 py-2.5 fw-semibold d-flex align-items-center justify-content-center gap-2 rounded-3"
          >
            {loading ? 'Authenticating...' : (
              <>
                <span>Sign In</span>
                <ArrowRight size={18} />
              </>
            )}
          </button>
        </form>

        <div className="mt-4 pt-3 border-top text-center">
          <div className="text-uppercase text-muted fw-bold mb-2" style={{ fontSize: '0.7rem', letterSpacing: '0.05em' }}>
            Demo Account 1-Click Login
          </div>
          <div className="d-flex gap-2">
            <button
              type="button"
              onClick={() => handleDemoFill('Admin')}
              className="btn btn-outline-primary btn-sm flex-fill d-flex align-items-center justify-content-center gap-1.5 py-1.5 rounded-2"
            >
              <ShieldCheck size={16} />
              <span>Admin Demo</span>
            </button>
            <button
              type="button"
              onClick={() => handleDemoFill('Employee')}
              className="btn btn-outline-secondary btn-sm flex-fill d-flex align-items-center justify-content-center gap-1.5 py-1.5 rounded-2"
            >
              <User size={16} />
              <span>Employee Demo</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
