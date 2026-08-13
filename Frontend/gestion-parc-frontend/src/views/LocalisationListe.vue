<template>
  <div class="row">
    <!-- Liste -->
    <div class="col-lg-7">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h2 class="fw-bold mb-0">📍 Localisations</h2>
      </div>
      <div class="card shadow-sm">
        <div class="card-body p-0">
          <table class="table table-hover table-striped mb-0">
            <thead class="table-dark">
              <tr><th>ID</th><th>Nom</th><th class="text-center">Actions</th></tr>
            </thead>
            <tbody>
              <tr v-if="localisations.length === 0">
                <td colspan="3" class="text-center text-muted py-4">Aucune localisation.</td>
              </tr>
             <tr v-for="(l, index) in localisations" :key="l.id">
                <td>{{ index + 1 }}</td>
                <td>{{ l.nom }}</td>
                <td class="text-center">
                  <div class="btn-group btn-group-sm" v-if="isAdmin">
                    <button class="btn btn-outline-warning" @click="editer(l)">✏️</button>
                    <button class="btn btn-outline-danger" @click="confirmDelete(l)">🗑</button>
                  </div>
                  <span v-else class="text-muted small">—</span>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="card-footer text-muted small">{{ localisations.length }} localisation(s)</div>
        </div>
      </div>
    </div>

    <!-- Formulaire (Admin uniquement) -->
    <div class="col-lg-5" v-if="isAdmin">
      <div class="card shadow-sm">
        <div class="card-header fw-semibold">
          {{ editId ? '✏️ Modifier' : '➕ Nouvelle localisation' }}
        </div>
        <div class="card-body">
          <form @submit.prevent="soumettre">
            <div class="mb-3">
              <label class="form-label">Nom <span class="text-danger">*</span></label>
              <input v-model="form.nom" type="text" class="form-control" required maxlength="100"
                placeholder="Ex: Salle Serveur, Administration...">
            </div>
            <div v-if="erreur" class="alert alert-danger py-2 small">{{ erreur }}</div>
            <div class="d-flex gap-2">
              <button type="submit" class="btn btn-primary btn-sm" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
                💾 Enregistrer
              </button>
              <button v-if="editId" type="button" class="btn btn-outline-secondary btn-sm" @click="annuler">
                ✖ Annuler
              </button>
            </div>
          </form>
        </div>
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
          <div class="modal-body">Supprimer <strong>{{ aSupprimer.nom }}</strong> ?</div>
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
import { localisationService } from '../services/api.js'

const localisations = ref([])
const form = ref({ nom: '' })
const editId = ref(null)
const saving = ref(false)
const erreur = ref('')
const aSupprimer = ref(null)
const isAdmin = ref(localStorage.getItem('role') === 'Admin')

async function charger() {
  const res = await localisationService.getAll()
  localisations.value = res.data.sort((a, b) => a.nom.localeCompare(b.nom))
}

function editer(l) { editId.value = l.id; form.value = { nom: l.nom }; erreur.value = '' }
function annuler() { editId.value = null; form.value = { nom: '' }; erreur.value = '' }
function confirmDelete(l) { aSupprimer.value = l }

async function soumettre() {
  erreur.value = ''; saving.value = true
  try {
    if (editId.value) await localisationService.update(editId.value, form.value)
    else await localisationService.create(form.value)
    annuler(); charger()
  } catch (err) {
    erreur.value = err.response?.data?.message || 'Erreur lors de l\'enregistrement.'
  } finally { saving.value = false }
}

async function doDelete() {
  try { await localisationService.delete(aSupprimer.value.id); aSupprimer.value = null; charger() } catch {}
}

onMounted(charger)
</script>