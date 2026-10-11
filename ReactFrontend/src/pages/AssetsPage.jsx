import React, { useEffect, useState } from 'react';
import API from '../services/api';
import StatusBadge from '../components/StatusBadge';
import ConfirmModal from '../components/ConfirmModal';
import { Link } from 'react-router-dom';
import { Plus, Search, Filter, Laptop, UserCheck, RotateCcw, Wrench, Eye, Trash2 } from 'lucide-react';

const AssetsPage = () => {
  const [assets, setAssets] = useState([]);
  const [categories, setCategories] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);

  // Filters
  const [search, setSearch] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  // Modals
  const [showAddModal, setShowAddModal] = useState(false);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [showReturnModal, setShowReturnModal] = useState(false);
  const [showMaintModal, setShowMaintModal] = useState(false);
  const [retireId, setRetireId] = useState(null);

  const [selectedAsset, setSelectedAsset] = useState(null);

  // Forms
  const [assetForm, setAssetForm] = useState({
    assetTag: '',
    assetName: '',
    categoryId: '',
    brand: '',
    model: '',
    serialNumber: '',
    purchaseDate: new Date().toISOString().split('T')[0],
    purchaseCost: 0,
    warrantyExpiry: '',
    location: 'Main Office',
    description: ''
  });

  const [assignForm, setAssignForm] = useState({
    employeeId: '',
    expectedReturnDate: '',
    notes: ''
  });

  const [returnForm, setReturnForm] = useState({
    condition: 'Good',
    returnNotes: ''
  });

  const [maintForm, setMaintForm] = useState({
    issue: '',
    vendor: '',
    cost: 0
  });

  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    fetchInitialData();
  }, []);

  const fetchInitialData = async () => {
    try {
      setLoading(true);
      const [catsRes, empsRes] = await Promise.all([
        API.get('/categories'),
        API.get('/employees')
      ]);
      setCategories(catsRes.data);
      setEmployees(empsRes.data.filter(e => e.status === 'Active'));
      await fetchAssets();
    } catch (err) {
      console.error('Error loading assets initial data', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchAssets = async (s = search, c = categoryId, st = statusFilter) => {
    try {
      const res = await API.get('/assets', {
        params: { search: s, categoryId: c, status: st }
      });
      setAssets(res.data);
    } catch (err) {
      console.error('Failed to fetch assets', err);
    }
  };

  const handleFilterChange = (s, c, st) => {
    setSearch(s);
    setCategoryId(c);
    setStatusFilter(st);
    fetchAssets(s, c, st);
  };

  // Open Handlers
  const handleOpenAdd = () => {
    setAssetForm({
      assetTag: `AST-${1000 + assets.length + 1}`,
      assetName: '',
      categoryId: categories[0]?.id || '',
      brand: '',
      model: '',
      serialNumber: `SN-${Math.floor(100000 + Math.random() * 900000)}`,
      purchaseDate: new Date().toISOString().split('T')[0],
      purchaseCost: 999.00,
      warrantyExpiry: '',
      location: 'Building A - Floor 1',
      description: ''
    });
    setErrorMsg('');
    setShowAddModal(true);
  };

  const handleOpenAssign = (asset) => {
    setSelectedAsset(asset);
    setAssignForm({
      employeeId: employees[0]?.id || '',
      expectedReturnDate: '',
      notes: ''
    });
    setErrorMsg('');
    setShowAssignModal(true);
  };

  const handleOpenReturn = (asset) => {
    setSelectedAsset(asset);
    setReturnForm({ condition: 'Good', returnNotes: '' });
    setErrorMsg('');
    setShowReturnModal(true);
  };

  const handleOpenMaint = (asset) => {
    setSelectedAsset(asset);
    setMaintForm({ issue: '', vendor: '', cost: 0 });
    setErrorMsg('');
    setShowMaintModal(true);
  };

  // Submit Handlers
  const handleCreateAsset = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      await API.post('/assets', assetForm);
      setShowAddModal(false);
      fetchAssets();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error creating asset.');
    }
  };

  const handleAssignSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      await API.post(`/assets/${selectedAsset.assetId}/assign`, assignForm);
      setShowAssignModal(false);
      fetchAssets();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error assigning asset.');
    }
  };

  const handleReturnSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      await API.post(`/assets/${selectedAsset.assetId}/return`, returnForm);
      setShowReturnModal(false);
      fetchAssets();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error returning asset.');
    }
  };

  const handleMaintSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      await API.post(`/assets/${selectedAsset.assetId}/maintenance`, maintForm);
      setShowMaintModal(false);
      fetchAssets();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error sending asset to maintenance.');
    }
  };

  const handleRetireAsset = async () => {
    if (!retireId) return;
    try {
      await API.delete(`/assets/${retireId}`);
      setRetireId(null);
      fetchAssets();
    } catch (err) {
      alert('Error retiring asset');
    }
  };

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">IT Asset Management</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>Comprehensive hardware and software asset catalog</p>
        </div>
        <button onClick={handleOpenAdd} className="btn btn-primary d-flex align-items-center gap-2 fw-medium px-3 py-2 rounded-3">
          <Plus size={18} />
          <span>Add New Asset</span>
        </button>
      </div>

      {/* Search & Filter Toolbar */}
      <div className="card-custom p-3 mb-4">
        <div className="row g-2">
          <div className="col-12 col-md-5">
            <div className="input-group">
              <span className="input-group-text bg-light border-end-0 text-muted">
                <Search size={18} />
              </span>
              <input
                type="text"
                className="form-control bg-light border-start-0 ps-0"
                placeholder="Search tag, name, serial number, brand..."
                value={search}
                onChange={(e) => handleFilterChange(e.target.value, categoryId, statusFilter)}
              />
            </div>
          </div>

          <div className="col-12 col-sm-6 col-md-3">
            <select
              className="form-select bg-light"
              value={categoryId}
              onChange={(e) => handleFilterChange(search, e.target.value, statusFilter)}
            >
              <option value="">All Categories</option>
              {categories.map(c => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>

          <div className="col-12 col-sm-6 col-md-4">
            <select
              className="form-select bg-light"
              value={statusFilter}
              onChange={(e) => handleFilterChange(search, categoryId, e.target.value)}
            >
              <option value="">All Statuses</option>
              <option value="Available">Available</option>
              <option value="Assigned">Assigned</option>
              <option value="UnderMaintenance">Under Maintenance</option>
              <option value="Retired">Retired</option>
            </select>
          </div>
        </div>
      </div>

      {/* Asset Catalog Table */}
      <div className="card-custom p-4">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
            <thead className="table-light">
              <tr>
                <th>Asset Tag</th>
                <th>Asset Name</th>
                <th>Category</th>
                <th>Serial Number</th>
                <th>Location</th>
                <th>Assigned To</th>
                <th>Status</th>
                <th className="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-muted">Loading asset inventory...</td>
                </tr>
              ) : assets.length > 0 ? (
                assets.map(a => (
                  <tr key={a.assetId}>
                    <td><span className="badge bg-light text-dark border font-monospace">{a.assetTag}</span></td>
                    <td>
                      <Link to={`/admin/assets/${a.assetId}`} className="fw-semibold text-dark text-decoration-none">
                        {a.assetName}
                      </Link>
                      <div className="text-muted" style={{ fontSize: '0.75rem' }}>{a.brand} {a.model}</div>
                    </td>
                    <td>{a.categoryName}</td>
                    <td><span className="font-monospace text-secondary" style={{ fontSize: '0.8rem' }}>{a.serialNumber}</span></td>
                    <td>{a.location || '—'}</td>
                    <td>{a.currentEmployeeName ? (
                      <span className="fw-medium text-dark">{a.currentEmployeeName}</span>
                    ) : <span className="text-muted">—</span>}</td>
                    <td><StatusBadge status={a.status} type="asset" /></td>
                    <td className="text-end">
                      <div className="d-flex justify-content-end gap-1">
                        <Link to={`/admin/assets/${a.assetId}`} className="btn btn-light btn-sm text-secondary" title="View Audit Details">
                          <Eye size={16} />
                        </Link>

                        {a.status === 'Available' && (
                          <button onClick={() => handleOpenAssign(a)} className="btn btn-primary btn-sm px-2 py-1" title="Assign Asset">
                            <UserCheck size={14} className="me-1" /> Assign
                          </button>
                        )}

                        {a.status === 'Assigned' && (
                          <button onClick={() => handleOpenReturn(a)} className="btn btn-outline-secondary btn-sm px-2 py-1" title="Return Asset">
                            <RotateCcw size={14} className="me-1" /> Return
                          </button>
                        )}

                        {a.status !== 'UnderMaintenance' && a.status !== 'Retired' && (
                          <button onClick={() => handleOpenMaint(a)} className="btn btn-light btn-sm text-warning" title="Send to Maintenance">
                            <Wrench size={16} />
                          </button>
                        )}

                        {a.status !== 'Retired' && (
                          <button onClick={() => setRetireId(a.assetId)} className="btn btn-light btn-sm text-danger" title="Retire Asset">
                            <Trash2 size={16} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-muted">No assets found matching criteria.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Add Asset Modal */}
      {showAddModal && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered modal-lg">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">Register New IT Asset</h5>
                <button type="button" className="btn-close" onClick={() => setShowAddModal(false)}></button>
              </div>
              <form onSubmit={handleCreateAsset}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="row g-3">
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Asset Tag (Unique)</label>
                      <input
                        type="text"
                        className="form-control"
                        value={assetForm.assetTag}
                        onChange={(e) => setAssetForm({ ...assetForm, assetTag: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Asset Name</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Dell XPS 15 9530"
                        value={assetForm.assetName}
                        onChange={(e) => setAssetForm({ ...assetForm, assetName: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Category</label>
                      <select
                        className="form-select"
                        value={assetForm.categoryId}
                        onChange={(e) => setAssetForm({ ...assetForm, categoryId: parseInt(e.target.value) })}
                        required
                      >
                        {categories.map(c => (
                          <option key={c.id} value={c.id}>{c.name}</option>
                        ))}
                      </select>
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Serial Number (Unique)</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="SN-DLL-998811"
                        value={assetForm.serialNumber}
                        onChange={(e) => setAssetForm({ ...assetForm, serialNumber: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Brand / Manufacturer</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Dell / Apple / Lenovo"
                        value={assetForm.brand}
                        onChange={(e) => setAssetForm({ ...assetForm, brand: e.target.value })}
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Model</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="XPS 15 / MacBook Pro"
                        value={assetForm.model}
                        onChange={(e) => setAssetForm({ ...assetForm, model: e.target.value })}
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Purchase Date</label>
                      <input
                        type="date"
                        className="form-control"
                        value={assetForm.purchaseDate}
                        onChange={(e) => setAssetForm({ ...assetForm, purchaseDate: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Purchase Cost ($)</label>
                      <input
                        type="number"
                        step="0.01"
                        className="form-control"
                        value={assetForm.purchaseCost}
                        onChange={(e) => setAssetForm({ ...assetForm, purchaseCost: parseFloat(e.target.value) })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Warranty Expiry Date</label>
                      <input
                        type="date"
                        className="form-control"
                        value={assetForm.warrantyExpiry}
                        onChange={(e) => setAssetForm({ ...assetForm, warrantyExpiry: e.target.value })}
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Storage Location</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Building A - Room 302"
                        value={assetForm.location}
                        onChange={(e) => setAssetForm({ ...assetForm, location: e.target.value })}
                      />
                    </div>
                    <div className="col-12">
                      <label className="form-label fw-semibold">Description / Notes</label>
                      <textarea
                        className="form-control"
                        rows="2"
                        placeholder="Core specifications, accessory details..."
                        value={assetForm.description}
                        onChange={(e) => setAssetForm({ ...assetForm, description: e.target.value })}
                      />
                    </div>
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowAddModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-primary px-4">Save Asset</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Assign Asset Modal */}
      {showAssignModal && selectedAsset && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">Assign Asset: {selectedAsset.assetName}</h5>
                <button type="button" className="btn-close" onClick={() => setShowAssignModal(false)}></button>
              </div>
              <form onSubmit={handleAssignSubmit}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Select Employee</label>
                    <select
                      className="form-select"
                      value={assignForm.employeeId}
                      onChange={(e) => setAssignForm({ ...assignForm, employeeId: parseInt(e.target.value) })}
                      required
                    >
                      {employees.map(emp => (
                        <option key={emp.id} value={emp.id}>{emp.name} ({emp.employeeCode} - {emp.department})</option>
                      ))}
                    </select>
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Expected Return Date (Optional)</label>
                    <input
                      type="date"
                      className="form-control"
                      value={assignForm.expectedReturnDate}
                      onChange={(e) => setAssignForm({ ...assignForm, expectedReturnDate: e.target.value })}
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Assignment Notes</label>
                    <textarea
                      className="form-control"
                      rows="2"
                      placeholder="Primary workstation laptop, onboarding setup..."
                      value={assignForm.notes}
                      onChange={(e) => setAssignForm({ ...assignForm, notes: e.target.value })}
                    />
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowAssignModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-primary px-4">Confirm Assignment</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Return Asset Modal */}
      {showReturnModal && selectedAsset && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">Process Asset Return: {selectedAsset.assetName}</h5>
                <button type="button" className="btn-close" onClick={() => setShowReturnModal(false)}></button>
              </div>
              <form onSubmit={handleReturnSubmit}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Returned Condition</label>
                    <select
                      className="form-select"
                      value={returnForm.condition}
                      onChange={(e) => setReturnForm({ ...returnForm, condition: e.target.value })}
                    >
                      <option value="Good">Good / Excellent</option>
                      <option value="Fair">Fair / Used</option>
                      <option value="Damaged">Damaged (Needs Maintenance)</option>
                    </select>
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Return Notes</label>
                    <textarea
                      className="form-control"
                      rows="2"
                      placeholder="Returned upon offboarding, cable included..."
                      value={returnForm.returnNotes}
                      onChange={(e) => setReturnForm({ ...returnForm, returnNotes: e.target.value })}
                    />
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowReturnModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-success px-4">Confirm Return</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Maintenance Modal */}
      {showMaintModal && selectedAsset && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">Send to Maintenance: {selectedAsset.assetName}</h5>
                <button type="button" className="btn-close" onClick={() => setShowMaintModal(false)}></button>
              </div>
              <form onSubmit={handleMaintSubmit}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Issue Description</label>
                    <textarea
                      className="form-control"
                      rows="2"
                      placeholder="Screen flickering, battery replacement required..."
                      value={maintForm.issue}
                      onChange={(e) => setMaintForm({ ...maintForm, issue: e.target.value })}
                      required
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Vendor / Service Center</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Apple Service Provider / Dell Care"
                      value={maintForm.vendor}
                      onChange={(e) => setMaintForm({ ...maintForm, vendor: e.target.value })}
                    />
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowMaintModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-warning px-4">Start Maintenance</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Retire Confirm Modal */}
      <ConfirmModal
        show={!!retireId}
        title="Retire Asset"
        message="Are you sure you want to retire this asset? It will be marked as Retired while preserving full historical audit logs."
        confirmText="Retire Asset"
        confirmVariant="danger"
        onConfirm={handleRetireAsset}
        onClose={() => setRetireId(null)}
      />
    </div>
  );
};

export default AssetsPage;
