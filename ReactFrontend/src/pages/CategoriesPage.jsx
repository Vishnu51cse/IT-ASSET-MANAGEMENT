import React, { useEffect, useState } from 'react';
import API from '../services/api';
import { Layers, Plus, Edit2, CheckCircle2, XCircle } from 'lucide-react';

const CategoriesPage = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingCat, setEditingCat] = useState(null);
  const [formData, setFormData] = useState({ name: '', description: '' });
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    fetchCategories();
  }, []);

  const fetchCategories = async () => {
    try {
      setLoading(true);
      const res = await API.get('/categories');
      setCategories(res.data);
    } catch (err) {
      console.error('Failed to fetch categories', err);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenAdd = () => {
    setEditingCat(null);
    setFormData({ name: '', description: '' });
    setErrorMsg('');
    setShowModal(true);
  };

  const handleOpenEdit = (cat) => {
    setEditingCat(cat);
    setFormData({ name: cat.name, description: cat.description });
    setErrorMsg('');
    setShowModal(true);
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      if (editingCat) {
        await API.put(`/categories/${editingCat.id}`, formData);
      } else {
        await API.post('/categories', formData);
      }
      setShowModal(false);
      fetchCategories();
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error saving category.');
    }
  };

  const handleToggleActive = async (id) => {
    try {
      await API.delete(`/categories/${id}`);
      fetchCategories();
    } catch (err) {
      alert('Error updating category status');
    }
  };

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">Asset Categories</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>Manage asset types (Laptops, Monitors, Mobile, Software, Printers, etc.)</p>
        </div>
        <button onClick={handleOpenAdd} className="btn btn-primary d-flex align-items-center gap-2 fw-medium px-3 py-2 rounded-3">
          <Plus size={18} />
          <span>Add New Category</span>
        </button>
      </div>

      <div className="row g-3">
        {loading ? (
          <div className="col-12 text-center py-5 text-muted">Loading categories...</div>
        ) : categories.map(cat => (
          <div key={cat.id} className="col-12 col-md-6 col-lg-4">
            <div className="card-custom p-4 h-100 d-flex flex-column justify-content-between">
              <div>
                <div className="d-flex align-items-center justify-content-between mb-2">
                  <div className="d-flex align-items-center gap-2">
                    <div className="bg-primary bg-opacity-10 text-primary p-2 rounded-3">
                      <Layers size={20} />
                    </div>
                    <h6 className="fw-bold mb-0 text-dark">{cat.name}</h6>
                  </div>
                  <span className={`badge ${cat.isActive ? 'badge-available' : 'badge-retired'} px-2.5 py-1 rounded-pill`}>
                    {cat.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
                <p className="text-muted mb-3" style={{ fontSize: '0.85rem' }}>{cat.description || 'No description provided.'}</p>
              </div>

              <div className="pt-3 border-top d-flex align-items-center justify-content-between">
                <span className="text-secondary fw-semibold" style={{ fontSize: '0.85rem' }}>
                  {cat.totalAssetsCount} Registered Assets
                </span>
                <div className="d-flex gap-1">
                  <button onClick={() => handleOpenEdit(cat)} className="btn btn-light btn-sm text-secondary" title="Edit">
                    <Edit2 size={16} />
                  </button>
                  <button onClick={() => handleToggleActive(cat.id)} className={`btn btn-light btn-sm ${cat.isActive ? 'text-danger' : 'text-success'}`} title="Toggle Active">
                    {cat.isActive ? <XCircle size={16} /> : <CheckCircle2 size={16} />}
                  </button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Add / Edit Modal */}
      {showModal && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">{editingCat ? 'Edit Category' : 'Add New Category'}</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)}></button>
              </div>
              <form onSubmit={handleSave}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Category Name</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Laptop, Desktop, Monitor, etc."
                      value={formData.name}
                      onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                      required
                    />
                  </div>
                  <div className="mb-3">
                    <label className="form-label fw-semibold">Description</label>
                    <textarea
                      className="form-control"
                      rows="3"
                      placeholder="Brief details about hardware/software types in this category..."
                      value={formData.description}
                      onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    />
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-primary px-4">Save Category</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CategoriesPage;
