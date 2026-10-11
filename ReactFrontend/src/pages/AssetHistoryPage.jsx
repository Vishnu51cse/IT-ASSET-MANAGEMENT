import React, { useEffect, useState } from 'react';
import API from '../services/api';
import { History, User, Laptop } from 'lucide-react';
import { Link } from 'react-router-dom';

const AssetHistoryPage = () => {
  const [histories, setHistories] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchHistory();
  }, []);

  const fetchHistory = async () => {
    try {
      setLoading(true);
      const res = await API.get('/asset-history');
      setHistories(res.data);
    } catch (err) {
      console.error('Failed to load asset history audit log', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">System Audit Log</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>Comprehensive immutable audit trail of all IT asset events and actions</p>
        </div>
      </div>

      <div className="card-custom p-4">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
            <thead className="table-light">
              <tr>
                <th>Date & Time</th>
                <th>Asset Tag</th>
                <th>Asset Name</th>
                <th>Action</th>
                <th>Description</th>
                <th>Performed By</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="6" className="text-center py-4 text-muted">Loading system audit trail...</td>
                </tr>
              ) : histories.length > 0 ? (
                histories.map(h => (
                  <tr key={h.historyId}>
                    <td className="text-muted" style={{ fontSize: '0.8rem' }}>{new Date(h.date).toLocaleString()}</td>
                    <td><span className="badge bg-light text-dark border font-monospace">{h.assetTag}</span></td>
                    <td>
                      <Link to={`/admin/assets/${h.assetId}`} className="fw-semibold text-dark text-decoration-none">
                        {h.assetName}
                      </Link>
                    </td>
                    <td>
                      <span className="badge bg-primary-subtle text-primary border border-primary-subtle px-2.5 py-1 rounded-pill">
                        {h.action}
                      </span>
                    </td>
                    <td className="text-secondary">{h.description}</td>
                    <td className="fw-medium">{h.performedBy}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="6" className="text-center py-4 text-muted">No audit events recorded.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default AssetHistoryPage;
