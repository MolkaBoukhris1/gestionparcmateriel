<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="fw-bold mb-0">📦 Parc Matériel</h2>
      <div class="d-flex gap-2">
        <button class="btn btn-success" @click="exporterExcel" :disabled="exporting">
          <span v-if="exporting" class="spinner-border spinner-border-sm me-1"></span>
          📥 Exporter Excel
        </button>
        <router-link v-if="isAdmin" to="/materiels/nouveau" class="btn btn-primary">
          ＋ Ajouter un matériel
        </router-link>
      </div>
    </div>

    <!-- Filtres -->
    <div class="card mb-3 shadow-sm">
      <div class="card-body py-2">
        <div class="row g-2 align-items-end">
          <div class="col-md-5">
            <label class="form-label small mb-1">Recherche</label>
            <input v-model="search" @input="onFilter" type="text"
              class="form-control form-control-sm"
              placeholder="Libellé, code-barres, marque, modèle...">
          </div>
          <div class="col-md-3">
            <label class="form-label small mb-1">État</label>
            <select v-model="etatFiltreId" @change="onFilter" class="form-select form-select-sm">
              <option value="">Tous les états</option>
              <option v-for="e in etats" :key="e.id" :value="e.id">{{ e.libelle }}</option>
            </select>
          </div>
          <div class="col-md-2">
            <div class="form-check mt-3">
              <input class="form-check-input" type="checkbox" id="chkInactif"
                v-model="inclureInactif" @change="onFilter">
              <label class="form-check-label small" for="chkInactif">Inclure inactifs</label>
            </div>
          </div>
          <div class="col-md-2">
            <button class="btn btn-outline-secondary btn-sm w-100" @click="resetFiltres">
              Réinitialiser
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary"></div>
    </div>

    <!-- Tableau -->
    <div v-else class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover table-striped mb-0">
            <thead class="table-dark">
              <tr>
                <th>Code-barres</th>
                <th>Libellé</th>
                <th>Type</th>
                <th>État</th>
                <th>Localisation</th>
                <th>Employé affecté</th>
                <th class="text-center">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="materiels.length === 0">
                <td colspan="7" class="text-center text-muted py-4">
                  Aucun matériel trouvé.
                </td>
              </tr>
              <tr v-for="m in materiels" :key="m.id" :class="{ 'table-secondary text-muted': !m.actif }">
                <td><code>{{ m.codeBarre }}</code></td>
                <td class="fw-semibold">{{ m.libelle }}</td>
                <td><span class="badge bg-light text-dark border">{{ m.typeMaterielLibelle }}</span></td>
                <td><EtatBadge :libelle="m.etatLibelle" /></td>
                <td>{{ m.localisationNom }}</td>
                <td>{{ m.employeNomComplet || '—' }}</td>
                <td class="text-center">
                  <div class="btn-group btn-group-sm">
                    <router-link :to="`/materiels/${m.id}`" class="btn btn-outline-info" title="Détail">👁</router-link>
                    <router-link v-if="isAdmin" :to="`/materiels/${m.id}/modifier`" class="btn btn-outline-warning" title="Modifier">✏️</router-link>
                    <button v-if="isAdmin" class="btn btn-outline-danger" title="Supprimer" @click="confirmDelete(m)">🗑</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">
          {{ materiels.length }} matériel(s) affiché(s)
        </div>
      </div>
    </div>

    <!-- Modal confirmation suppression -->
    <div v-if="materielASupprimer" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.4)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title">Confirmer la suppression</h5>
            <button type="button" class="btn-close btn-close-white" @click="materielASupprimer=null"></button>
          </div>
          <div class="modal-body">
            Désactiver le matériel <strong>{{ materielASupprimer.libelle }}</strong> ({{ materielASupprimer.codeBarre }}) ?
            <div class="text-muted small mt-1">Cette opération est logique — le matériel ne sera pas supprimé physiquement.</div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="materielASupprimer=null">Annuler</button>
            <button class="btn btn-danger" @click="doDelete">Désactiver</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api, { materielService, etatService } from '../services/api.js'
import EtatBadge from '../components/EtatBadge.vue'

const materiels = ref([])
const etats = ref([])
const loading = ref(false)
const search = ref('')
const etatFiltreId = ref('')
const inclureInactif = ref(false)
const materielASupprimer = ref(null)
const exporting = ref(false)
const isAdmin = ref(localStorage.getItem('role') === 'Admin')

async function charger() {
  loading.value = true
  try {
    const params = {}
    if (search.value) params.search = search.value
    if (etatFiltreId.value) params.etatId = etatFiltreId.value
    if (inclureInactif.value) params.includeInactif = true
    const res = await materielService.getAll(params)
    materiels.value = res.data
  } finally {
    loading.value = false
  }
}

function onFilter() {
  clearTimeout(window._filterTimer)
  window._filterTimer = setTimeout(charger, 300)
}

function resetFiltres() {
  search.value = ''
  etatFiltreId.value = ''
  inclureInactif.value = false
  charger()
}

function confirmDelete(m) { materielASupprimer.value = m }

async function doDelete() {
  try {
    await materielService.delete(materielASupprimer.value.id)
    materielASupprimer.value = null
    charger()
  } catch {}
}

async function exporterExcel() {
  exporting.value = true
  try {
    const response = await api.get('/Export/materiels', { responseType: 'blob' })
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `Materiel_${new Date().toISOString().slice(0,10)}.xlsx`)
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
  } catch (error) {
    console.error('Erreur export Excel', error)
  } finally {
    exporting.value = false
  }
}

onMounted(async () => {
  const [, e] = await Promise.allSettled([charger(), etatService.getAll()])
  if (e.status === 'fulfilled') etats.value = e.value.data
})
</script>