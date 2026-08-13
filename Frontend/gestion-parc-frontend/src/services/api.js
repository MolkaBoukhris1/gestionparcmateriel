import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5109/api',
  headers: { 'Content-Type': 'application/json' }
})

// ── Intercepteur requête — ajoute automatiquement le token JWT ───────────────
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ── Intercepteur réponse — gestion centralisée des erreurs ───────────────────
api.interceptors.response.use(
  response => response,
  error => {
    const message =
      error.response?.data?.message ||
      error.response?.data?.title ||
      error.message ||
      'Une erreur inattendue est survenue.'
    window.dispatchEvent(new CustomEvent('api-error', { detail: message }))
    return Promise.reject(error)
  }
)

export default api

// ── Auth ──────────────────────────────────────────────────────────────────────
export const authService = {
  login: (data) => api.post('/Auth/login', data)
}

// ── Matériel ──────────────────────────────────────────────────────────────────
export const materielService = {
  getAll: (params) => api.get('/Materiel', { params }),
  getById: (id) => api.get(`/Materiel/${id}`),
  create: (data) => api.post('/Materiel', data),
  update: (id, data) => api.put(`/Materiel/${id}`, data),
  delete: (id) => api.delete(`/Materiel/${id}`),
  affecter: (id, data) => api.post(`/Materiel/${id}/affecter`, data),
  retour: (id, data) => api.post(`/Materiel/${id}/retour`, data),
  transfert: (id, data) => api.post(`/Materiel/${id}/transfert`, data),
  historique: (id) => api.get(`/Materiel/${id}/historique`)
}

// ── Référentiels ──────────────────────────────────────────────────────────────
export const typeService = {
  getAll: () => api.get('/TypeMateriel')
}

export const etatService = {
  getAll: () => api.get('/EtatMateriel')
}

export const localisationService = {
  getAll: () => api.get('/Localisation'),
  getById: (id) => api.get(`/Localisation/${id}`),
  create: (data) => api.post('/Localisation', data),
  update: (id, data) => api.put(`/Localisation/${id}`, data),
  delete: (id) => api.delete(`/Localisation/${id}`)
}

export const employeService = {
  getAll: (params) => api.get('/Employe', { params }),
  getById: (id) => api.get(`/Employe/${id}`),
  create: (data) => api.post('/Employe', data),
  update: (id, data) => api.put(`/Employe/${id}`, data),
  delete: (id) => api.delete(`/Employe/${id}`)
}