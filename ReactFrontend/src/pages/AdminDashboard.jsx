import React, { useEffect, useState } from 'react';
import API from '../services/api';
import StatusBadge from '../components/StatusBadge';
import { Link } from 'react-router-dom';
import { 
  Laptop, 
  CheckCircle, 
  UserCheck, 
  Wrench, 
  FileText, 
  Users, 
  ArrowUpRight,
  Clock
} from 'lucide-react';

const AdminDashboard = () => {
  const [metrics, setMetrics] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchMetrics();
  }, []);

  const fetchMetrics = async () => {
    try {
      const res = await API.get('/dashboard/admin');
      setMetrics(res.data);
    } catch (err) {
      console.error('Failed to load admin metrics', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="text-center py-5 text-muted">Loading System Dashboard...</div>;
  }

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">System Overview</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>Real-time telemetry and management metrics</p>
        </div>
      </div>

      {/* Metric Cards Row */}
      <div className="row g-3 mb-4">
        <div className="col-12 col-sm-6 col-lg-4 col-xl-2">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.8rem' }}>Total Assets</span>
              <div className="stat-icon bg-primary bg-opacity-10 text-primary">
                <Laptop size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{metrics?.totalAssets || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-lg-4 col-xl-2">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.8rem' }}>Available</span>
              <div className="stat-icon bg-success bg-opacity-10 text-success">
                <CheckCircle size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{metrics?.availableAssets || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-lg-4 col-xl-2">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.8rem' }}>Assigned</span>
              <div className="stat-icon bg-indigo bg-opacity-10 text-indigo" style={{ color: '#4f46e5' }}>
                <UserCheck size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{metrics?.assignedAssets || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-lg-4 col-xl-2">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.8rem' }}>Maintenance</span>
              <div className="stat-icon bg-warning bg-opacity-10 text-warning">
                <Wrench size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{metrics?.underMaintenanceAssets || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-lg-4 col-xl-2">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.8rem' }}>Pending Req.</span>
              <div className="stat-icon bg-info bg-opacity-10 text-info">
                <FileText size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{metrics?.pendingRequests || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-lg-4 col-xl-2">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.8rem' }}>Employees</span>
              <div className="stat-icon bg-secondary bg-opacity-10 text-secondary">
                <Users size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{metrics?.totalEmployees || 0}</h3>
          </div>
        </div>
      </div>

      <div className="row g-4">
        {/* Recent Asset Activity & Requests */}
        <div className="col-12 col-lg-8">
          {/* Pending Requests Table */}
          <div className="card-custom p-4 mb-4">
            <div className="d-flex align-items-center justify-content-between mb-3">
              <h6 className="fw-bold mb-0">Pending Asset Requests</h6>
              <Link to="/admin/requests" className="text-primary text-decoration-none fw-semibold style-link" style={{ fontSize: '0.85rem' }}>
                View All <ArrowUpRight size={16} />
              </Link>
            </div>
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
                <thead className="table-light">
                  <tr>
                    <th>Employee</th>
                    <th>Category</th>
                    <th>Reason</th>
                    <th>Requested Date</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {metrics?.recentRequests?.length > 0 ? (
                    metrics.recentRequests.map(r => (
                      <tr key={r.requestId}>
                        <td>
                          <div className="fw-medium">{r.employeeName}</div>
                          <div className="text-muted" style={{ fontSize: '0.75rem' }}>{r.department}</div>
                        </td>
                        <td>{r.categoryName}</td>
                        <td className="text-truncate" style={{ maxWidth: '200px' }}>{r.reason}</td>
                        <td>{new Date(r.requestedDate).toLocaleDateString()}</td>
                        <td><StatusBadge status={r.status} type="request" /></td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="5" className="text-center py-3 text-muted">No pending requests</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          {/* Recent Assets Catalog */}
          <div className="card-custom p-4">
            <div className="d-flex align-items-center justify-content-between mb-3">
              <h6 className="fw-bold mb-0">Recent Assets</h6>
              <Link to="/admin/assets" className="text-primary text-decoration-none fw-semibold" style={{ fontSize: '0.85rem' }}>
                Asset Catalog <ArrowUpRight size={16} />
              </Link>
            </div>
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
                <thead className="table-light">
                  <tr>
                    <th>Asset Tag</th>
                    <th>Name</th>
                    <th>Category</th>
                    <th>Assigned To</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {metrics?.recentAssets?.map(a => (
                    <tr key={a.assetId}>
                      <td><span className="badge bg-light text-dark border font-monospace">{a.assetTag}</span></td>
                      <td>
                        <Link to={`/admin/assets/${a.assetId}`} className="fw-semibold text-dark text-decoration-none">
                          {a.assetName}
                        </Link>
                      </td>
                      <td>{a.categoryName}</td>
                      <td>{a.currentEmployeeName || <span className="text-muted">—</span>}</td>
                      <td><StatusBadge status={a.status} type="asset" /></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        {/* System Activity Stream */}
        <div className="col-12 col-lg-4">
          <div className="card-custom p-4">
            <div className="d-flex align-items-center justify-content-between mb-3">
              <h6 className="fw-bold mb-0">Recent Audit Logs</h6>
              <Clock size={18} className="text-muted" />
            </div>

            <div className="timeline">
              {metrics?.recentActivities?.map(h => (
                <div key={h.historyId} className="timeline-item">
                  <div className="timeline-dot"></div>
                  <div className="fw-semibold text-dark" style={{ fontSize: '0.85rem' }}>{h.action}</div>
                  <div className="text-secondary" style={{ fontSize: '0.8rem' }}>{h.description}</div>
                  <div className="text-muted mt-1" style={{ fontSize: '0.725rem' }}>
                    {new Date(h.date).toLocaleString()} • {h.performedBy}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
