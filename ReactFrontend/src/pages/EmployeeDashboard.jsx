import React, { useEffect, useState } from 'react';
import API from '../services/api';
import StatusBadge from '../components/StatusBadge';
import { Link } from 'react-router-dom';
import { Laptop, Send, Clock, CheckCircle2, RotateCcw, AlertTriangle } from 'lucide-react';

const EmployeeDashboard = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchMetrics();
  }, []);

  const fetchMetrics = async () => {
    try {
      const res = await API.get('/dashboard/employee');
      setData(res.data);
    } catch (err) {
      console.error('Failed to load employee metrics', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="text-center py-5 text-muted">Loading Employee Portal...</div>;
  }

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">My Dashboard</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>View your hardware, software, and active asset requests</p>
        </div>
        <Link to="/employee/request-asset" className="btn btn-primary d-flex align-items-center gap-2 fw-medium px-3 py-2 rounded-3">
          <Send size={18} />
          <span>Request New Asset</span>
        </Link>
      </div>

      {/* Metrics Row */}
      <div className="row g-3 mb-4">
        <div className="col-12 col-sm-6 col-md-3">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.85rem' }}>My Assigned Assets</span>
              <div className="stat-icon bg-primary bg-opacity-10 text-primary">
                <Laptop size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{data?.myAssignedAssetsCount || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-md-3">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.85rem' }}>Pending Requests</span>
              <div className="stat-icon bg-warning bg-opacity-10 text-warning">
                <Clock size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{data?.pendingRequestsCount || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-md-3">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.85rem' }}>Approved Requests</span>
              <div className="stat-icon bg-success bg-opacity-10 text-success">
                <CheckCircle2 size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{data?.approvedRequestsCount || 0}</h3>
          </div>
        </div>

        <div className="col-12 col-sm-6 col-md-3">
          <div className="stat-card">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="text-muted fw-medium" style={{ fontSize: '0.85rem' }}>Under Maintenance</span>
              <div className="stat-icon bg-danger bg-opacity-10 text-danger">
                <AlertTriangle size={20} />
              </div>
            </div>
            <h3 className="fw-bold mb-0 text-dark">{data?.underMaintenanceCount || 0}</h3>
          </div>
        </div>
      </div>

      {/* My Assigned Assets */}
      <div className="card-custom p-4 mb-4">
        <h6 className="fw-bold mb-3">My Currently Assigned Assets</h6>
        {data?.myAssignedAssets?.length > 0 ? (
          <div className="row g-3">
            {data.myAssignedAssets.map(asset => (
              <div key={asset.assetId} className="col-12 col-md-6 col-lg-4">
                <div className="border rounded-3 p-3 bg-white shadow-sm h-100 d-flex flex-column justify-content-between">
                  <div>
                    <div className="d-flex align-items-center justify-content-between mb-2">
                      <span className="badge bg-light text-dark border font-monospace">{asset.assetTag}</span>
                      <StatusBadge status={asset.status} type="asset" />
                    </div>
                    <h6 className="fw-bold text-dark mb-1">{asset.assetName}</h6>
                    <div className="text-muted mb-2" style={{ fontSize: '0.8rem' }}>
                      {asset.brand} {asset.model} • {asset.categoryName}
                    </div>
                    <div className="text-secondary" style={{ fontSize: '0.8rem' }}>
                      S/N: <span className="font-monospace">{asset.serialNumber}</span>
                    </div>
                  </div>
                  <div className="mt-3 pt-2 border-top d-flex gap-2">
                    <Link to="/employee/my-assets" className="btn btn-outline-primary btn-sm flex-fill d-flex align-items-center justify-content-center gap-1">
                      <RotateCcw size={14} />
                      <span>Request Return</span>
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="text-center py-4 text-muted">
            <Laptop size={36} className="mb-2 text-secondary opacity-50" />
            <p className="mb-0">You currently have no IT assets assigned to you.</p>
          </div>
        )}
      </div>

      {/* My Asset Requests */}
      <div className="card-custom p-4">
        <h6 className="fw-bold mb-3">My Recent Requests</h6>
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
            <thead className="table-light">
              <tr>
                <th>Req #</th>
                <th>Category</th>
                <th>Reason</th>
                <th>Requested Date</th>
                <th>Status</th>
                <th>Admin Response</th>
              </tr>
            </thead>
            <tbody>
              {data?.myRequests?.length > 0 ? (
                data.myRequests.map(r => (
                  <tr key={r.requestId}>
                    <td className="fw-medium">#{r.requestId}</td>
                    <td>{r.categoryName}</td>
                    <td>{r.reason}</td>
                    <td>{new Date(r.requestedDate).toLocaleDateString()}</td>
                    <td><StatusBadge status={r.status} type="request" /></td>
                    <td className="text-muted">{r.adminComment || '—'}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="6" className="text-center py-3 text-muted">No request history found.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default EmployeeDashboard;
