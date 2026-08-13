<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="fw-bold mb-0">👤 Employés</h2>
      <router-link v-if="isAdmin" to="/employes/nouveau" class="btn btn-primary">＋ Ajouter</router-link>
    </div>

    <div class="card shadow-sm mb-3">
      <div class="card-body py-2">
        <input v-model="search" @input="onSearch" type="text"
          class="form-control form-control-sm"
          placeholder="Rechercher par nom, prénom, matricule, service...">
      </div>
    </div>

    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover table-striped mb-0">
            <thead class="table-dark">
              <tr>
                <th>Matricule</th><th>Nom</th><th>Prénom</th>
                <th>Service</th><th>Poste</th><th>Email</th>
                <th class="text-center">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="employes.length === 0">
                <td colspan="7" class="text-center text-muted py-4">Aucun employé.</td>
              </tr>
              <tr v-for="e in employes" :key="e.id">
                <td><code>{{ e.matricule }}</code></td>
                <td class="fw-semibold">{{ e.nom }}</td>
                <td>{{ e.prenom }}</td>
                <td>{{ e.service || '—' }}</td>
                <td>{{ e.poste || '—' }}</td>
                <td>{{ e.email || '—' }}</td>
                <td class="text-center">
                  <div class="btn-group btn-group-sm">
                    <router-link v-if="isAdmin" :to="`/employes/${e.id}/modifier`" class="btn btn-outline-warning">✏️</router-link>
                    <button v-if="isAdmin" class="btn btn-outline-danger" @click="confirmDelete(e)">🗑</button>
                    <span v-if="!isAdmin" class="text-muted small">—</span>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="card-footer text-muted small">{{ employes.length }} employé(s)</div>
      </div>
    </div>

    <!-- Modal suppression -->
    <div v-if="aSupprimer" class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,.4)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title">Confirmer la suppression</h5>
            <button type="button" class="btn-close btn-close-white" @click="aSupprimer=null"></button>
          </div>
          <div class="modal-body">
            Supprimer <strong>{{ aSupprimer.prenom }} {{ aSupprimer.nom }}</strong> ?
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="aSupprimer=null">Annuler</button>
            <button class="btn btn-danger" @click="doDelete">Supprimer</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { employeService } from '../services/api.js'

const employes = ref([])
const search = ref('')
const aSupprimer = ref(null)
const isAdmin = ref(localStorage.getItem('role') === 'Admin')

async function charger() {
  const params = {}
  if (search.value) params.search = search.value
  const res = await employeService.getAll(params)
  employes.value = res.data
}

function onSearch() {
  clearTimeout(window._empTimer)
  window._empTimer = setTimeout(charger, 300)
}

function confirmDelete(e) { aSupprimer.value = e }

async function doDelete() {
  try { await employeService.delete(aSupprimer.value.id); aSupprimer.value = null; charger() } catch {}
}

onMounted(charger)
</script>