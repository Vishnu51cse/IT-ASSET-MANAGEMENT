import React, { useEffect, useState } from 'react';
import API from '../services/api';
import { useAuth } from '../context/AuthContext';
import StatusBadge from '../components/StatusBadge';
import { Send, Check, X, FileText } from 'lucide-react';

const AssetRequestsPage = () => {
  const { isAdmin } = useAuth();
  const [requests, setRequests] = useState([]);
  const [categories, setCategories] = useState([]);
  const [availableAssets, setAvailableAssets] = useState([]);
  const [loading, setLoading] = useState(true);

  // Modal State for Admin Process
  const [selectedReq, setSelectedReq] = useState(null);
  const [processAction, setProcessAction] = useState('Approve');
  const [adminComment, setAdminComment] = useState('');
  const [assignAssetId, setAssignAssetId] = useState('');
  const [showProcessModal, setShowProcessModal] = useState(false);

  // Employee Form State
  const [reqCategoryId, setReqCategoryId] = useState('');
  const [reqReason, setReqReason] = useState('');
  const [submitSuccess, setSubmitSuccess] = useState('');
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [reqsRes, catsRes, assetsRes] = await Promise.all([
        API.get('/asset-requests'),
        API.get('/categories'),
        API.get('/assets', { params: { status: 'Available' } })
      ]);
      setRequests(reqsRes.data);
      setCategories(catsRes.data);
      setAvailableAssets(assetsRes.data);
      if (catsRes.data.length > 0) setReqCategoryId(catsRes.data[0].id);
    } catch (err) {
      console.error('Error loading requests data', err);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenProcess = (req, action) => {
    setSelectedReq(req);
    setProcessAction(action);
    setAdminComment(action === 'Approve' ? 'Approved by IT Admin.' : 'Request rejected.');
    setAssignAssetId('');
    setErrorMsg('');
    setShowProcessModal(true);
  };

  const handleProcessSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      const endpoint = processAction === 'Approve' ? `/asset-requests/${selectedReq.requestId}/approve` : `/asset-requests/${selectedReq.requestId}/reject`;
      await API.post(endpoint, {
        action: processAction,
        adminComment,
        assignAssetId: assignAssetId ? parseInt(assignAssetId) : null
      });
      setShowProcessModal(false);
      fetchData();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error processing request.');
    }
  };

  const handleCreateRequest = async (e) => {
    e.preventDefault();
    setSubmitSuccess('');
    setErrorMsg('');
    try {
      await API.post('/asset-requests', {
        categoryId: parseInt(reqCategoryId),
        reason: reqReason
      });
      setSubmitSuccess('Your asset request has been submitted successfully.');
      setReqReason('');
      fetchData();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Failed to submit asset request.');
    }
  };

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">{isAdmin ? 'Asset Request Approvals' : 'Request New IT Asset'}</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>
            {isAdmin ? 'Review and approve/reject hardware and software requests' : 'Submit hardware or software requests to IT administration'}
          </p>
        </div>
      </div>

      {!isAdmin && (
        <div className="card-custom p-4 mb-4">
          <h6 className="fw-bold mb-3">Submit Asset Request</h6>
          {submitSuccess && <div className="alert alert-success py-2 mb-3">{submitSuccess}</div>}
          {errorMsg && <div className="alert alert-danger py-2 mb-3">{errorMsg}</div>}
          <form onSubmit={handleCreateRequest}>
            <div className="row g-3">
              <div className="col-12 col-md-4">
                <label className="form-label fw-semibold">Asset Category</label>
                <select
                  className="form-select"
                  value={reqCategoryId}
                  onChange={(e) => setReqCategoryId(e.target.value)}
                  required
                >
                  {categories.map(c => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </div>
              <div className="col-12 col-md-8">
                <label className="form-label fw-semibold">Reason for Request</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Explain why this asset is required for your daily work..."
                  value={reqReason}
                  onChange={(e) => setReqReason(e.target.value)}
                  required
                />
              </div>
              <div className="col-12 text-end">
                <button type="submit" className="btn btn-primary px-4 fw-medium">
                  <Send size={16} className="me-1" /> Submit Request
                </button>
              </div>
            </div>
          </form>
        </div>
      )}

      {/* Requests Table */}
      <div className="card-custom p-4">
        <h6 className="fw-bold mb-3">{isAdmin ? 'All Employee Requests' : 'My Submitted Requests'}</h6>
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
            <thead className="table-light">
              <tr>
                <th>Req #</th>
                <th>Employee</th>
                <th>Category</th>
                <th>Reason</th>
                <th>Requested Date</th>
                <th>Status</th>
                <th>Admin Comments</th>
                {isAdmin && <th className="text-end">Actions</th>}
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={isAdmin ? "8" : "7"} className="text-center py-4 text-muted">Loading requests...</td>
                </tr>
              ) : requests.length > 0 ? (
                requests.map(r => (
                  <tr key={r.requestId}>
                    <td className="fw-medium font-monospace">#{r.requestId}</td>
                    <td>
                      <div className="fw-semibold">{r.employeeName}</div>
                      <div className="text-muted" style={{ fontSize: '0.75rem' }}>{r.department}</div>
                    </td>
                    <td>{r.categoryName}</td>
                    <td className="text-truncate" style={{ maxWidth: '220px' }}>{r.reason}</td>
                    <td>{new Date(r.requestedDate).toLocaleDateString()}</td>
                    <td><StatusBadge status={r.status} type="request" /></td>
                    <td className="text-muted">{r.adminComment || '—'}</td>
                    {isAdmin && (
                      <td className="text-end">
                        {r.status === 'Pending' ? (
                          <div className="d-flex justify-content-end gap-1">
                            <button onClick={() => handleOpenProcess(r, 'Approve')} className="btn btn-success btn-sm px-2 py-1">
                              <Check size={14} className="me-1" /> Approve
                            </button>
                            <button onClick={() => handleOpenProcess(r, 'Reject')} className="btn btn-outline-danger btn-sm px-2 py-1">
                              <X size={14} className="me-1" /> Reject
                            </button>
                          </div>
                        ) : (
                          <span className="text-muted text-capitalize" style={{ fontSize: '0.8rem' }}>Processed</span>
                        )}
                      </td>
                    )}
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={isAdmin ? "8" : "7"} className="text-center py-4 text-muted">No requests found.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Admin Process Modal */}
      {showProcessModal && selectedReq && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">
                  {processAction} Request #{selectedReq.requestId} ({selectedReq.employeeName})
                </h5>
                <button type="button" className="btn-close" onClick={() => setShowProcessModal(false)}></button>
              </div>
              <form onSubmit={handleProcessSubmit}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Admin Comment</label>
                    <textarea
                      className="form-control"
                      rows="2"
                      value={adminComment}
                      onChange={(e) => setAdminComment(e.target.value)}
                    />
                  </div>

                  {processAction === 'Approve' && (
                    <div className="mb-3">
                      <label className="form-label fw-semibold">Assign Available Asset Right Now (Optional)</label>
                      <select
                        className="form-select"
                        value={assignAssetId}
                        onChange={(e) => setAssignAssetId(e.target.value)}
                      >
                        <option value="">Do not assign yet (Approve request only)</option>
                        {availableAssets
                          .filter(a => a.categoryId === selectedReq.categoryId)
                          .map(a => (
                            <option key={a.assetId} value={a.assetId}>
                              {a.assetTag} - {a.assetName} ({a.brand} {a.model})
                            </option>
                          ))}
                      </select>
                      <div className="form-text">Selecting an asset will automatically set status to Completed and assign it to {selectedReq.employeeName}.</div>
                    </div>
                  )}
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowProcessModal(false)}>Cancel</button>
                  <button type="submit" className={`btn btn-${processAction === 'Approve' ? 'success' : 'danger'} px-4`}>
                    Confirm {processAction}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AssetRequestsPage;
