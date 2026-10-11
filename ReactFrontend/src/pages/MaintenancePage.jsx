import React, { useEffect, useState } from 'react';
import API from '../services/api';
import StatusBadge from '../components/StatusBadge';
import { Wrench, CheckCircle2, DollarSign, Clock } from 'lucide-react';

const MaintenancePage = () => {
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(true);

  // Complete Maintenance Modal State
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [resolution, setResolution] = useState('Replaced faulty component, tested & certified operational.');
  const [cost, setCost] = useState(0);
  const [vendor, setVendor] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    fetchMaintenance();
  }, []);

  const fetchMaintenance = async () => {
    try {
      setLoading(true);
      const res = await API.get('/maintenance');
      setRecords(res.data);
    } catch (err) {
      console.error('Failed to load maintenance records', err);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenComplete = (record) => {
    setSelectedRecord(record);
    setResolution(record.resolution || 'Issue repaired, hardware passed diagnostics tests.');
    setCost(record.cost || 0);
    setVendor(record.vendor || '');
    setErrorMsg('');
    setShowModal(true);
  };

  const handleCompleteSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      await API.post(`/maintenance/${selectedRecord.maintenanceId}/complete`, {
        resolution,
        cost: parseFloat(cost),
        vendor
      });
      setShowModal(false);
      fetchMaintenance();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error completing maintenance.');
    }
  };

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">Asset Maintenance</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>Track repairs, service vendor logs, and maintenance completion</p>
        </div>
      </div>

      <div className="card-custom p-4">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
            <thead className="table-light">
              <tr>
                <th>Asset Tag</th>
                <th>Asset Name</th>
                <th>Reported Issue</th>
                <th>Vendor</th>
                <th>Reported Date</th>
                <th>Cost ($)</th>
                <th>Status</th>
                <th className="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-muted">Loading maintenance logs...</td>
                </tr>
              ) : records.length > 0 ? (
                records.map(r => (
                  <tr key={r.maintenanceId}>
                    <td><span className="badge bg-light text-dark border font-monospace">{r.assetTag}</span></td>
                    <td className="fw-semibold text-dark">{r.assetName}</td>
                    <td className="text-truncate" style={{ maxWidth: '220px' }}>{r.issue}</td>
                    <td>{r.vendor || <span className="text-muted">—</span>}</td>
                    <td>{new Date(r.reportedDate).toLocaleDateString()}</td>
                    <td className="fw-semibold">${r.cost.toFixed(2)}</td>
                    <td><StatusBadge status={r.status} type="maintenance" /></td>
                    <td className="text-end">
                      {r.status !== 'Completed' ? (
                        <button onClick={() => handleOpenComplete(r)} className="btn btn-success btn-sm px-2.5 py-1">
                          <CheckCircle2 size={14} className="me-1" /> Complete Repair
                        </button>
                      ) : (
                        <span className="text-muted" style={{ fontSize: '0.75rem' }}>
                          Done on {r.completedDate ? new Date(r.completedDate).toLocaleDateString() : ''}
                        </span>
                      )}
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-muted">No maintenance records found.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Complete Maintenance Modal */}
      {showModal && selectedRecord && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">Complete Repair: {selectedRecord.assetName}</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)}></button>
              </div>
              <form onSubmit={handleCompleteSubmit}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Resolution Summary</label>
                    <textarea
                      className="form-control"
                      rows="3"
                      value={resolution}
                      onChange={(e) => setResolution(e.target.value)}
                      required
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Service Vendor</label>
                    <input
                      type="text"
                      className="form-control"
                      value={vendor}
                      onChange={(e) => setVendor(e.target.value)}
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Total Service Cost ($)</label>
                    <input
                      type="number"
                      step="0.01"
                      className="form-control"
                      value={cost}
                      onChange={(e) => setCost(e.target.value)}
                    />
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-success px-4">Complete Maintenance</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default MaintenancePage;
