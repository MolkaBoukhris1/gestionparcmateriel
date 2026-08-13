<template>
  <div v-if="loading" class="text-center py-5"><div class="spinner-border text-primary"></div></div>

  <div v-else-if="materiel">
    <!-- En-tête -->
    <div class="d-flex justify-content-between align-items-start mb-3">
      <div>
        <h2 class="fw-bold mb-1">{{ materiel.libelle }}</h2>
        <div class="text-muted small">
          <code>{{ materiel.codeBarre }}</code>
          <span class="mx-2">|</span>
          <EtatBadge :libelle="materiel.etatLibelle" />
          <span v-if="!materiel.actif" class="badge bg-secondary ms-2">Inactif</span>
        </div>
      </div>
      <div class="d-flex gap-2">
<router-link v-if="isAdmin" :to="`/materiels/${materiel.id}/modifier`" class="btn btn-outline-warning btn-sm">✏️ Modifier</router-link>        <router-link to="/" class="btn btn-outline-secondary btn-sm">← Retour liste</router-link>
      </div>
    </div>

    <div class="row g-3">
      <!-- Informations -->
      <div :class="isAdmin ? 'col-lg-5' : 'col-lg-12'">
        <div class="card shadow-sm h-100">
          <div class="card-header fw-semibold">📋 Informations</div>
          <div class="card-body">
            <table class="table table-sm table-borderless mb-0">
              <tbody>
                <tr><th class="text-muted w-40">Type</th><td>{{ materiel.typeMaterielLibelle }}</td></tr>
                <tr><th class="text-muted">Marque</th><td>{{ materiel.marque || '—' }}</td></tr>
                <tr><th class="text-muted">Modèle</th><td>{{ materiel.modele || '—' }}</td></tr>
                <tr><th class="text-muted">N° de série</th><td>{{ materiel.numeroSerie || '—' }}</td></tr>
                <tr><th class="text-muted">Acquisition</th><td>{{ formatDate(materiel.dateAcquisition) }}</td></tr>
                <tr><th class="text-muted">Localisation</th><td>{{ materiel.localisationNom }}</td></tr>
                <tr><th class="text-muted">Employé</th><td>{{ materiel.employeNomComplet || '—' }}</td></tr>
                <tr v-if="materiel.commentaires"><th class="text-muted">Commentaires</th><td>{{ materiel.commentaires }}</td></tr>
              </tbody>
            </table>
          </div>
           <div class="card-footer text-center bg-white">
            <div class="text-muted small mb-2">QR Code d'accès rapide</div>
            <img :src="qrCodeUrl" alt="QR Code" style="width:140px;height:140px" class="border rounded p-1">
          </div>
        </div>
      </div>

      <!-- Actions contextuelles -->
      <!-- Actions contextuelles (Admin uniquement) -->
      <div class="col-lg-7" v-if="isAdmin">
        <div class="card shadow-sm">
          <div class="card-header fw-semibold">⚡ Actions</div>
          <div class="card-body">
<!-- Non-Admin → message informatif -->
            <div v-if="!isAdmin" class="text-muted">
              🔒 Seul un administrateur peut effectuer ces actions.
            </div>
            <!-- EN STOCK → Affecter -->
            <div v-else-if="materiel.etatLibelle === 'En stock'">
              <h6 class="text-success">✅ Matériel disponible en stock</h6>
              <form @submit.prevent="affecter" class="mt-2">
                <div class="mb-2">
                  <label class="form-label small">Employé <span class="text-danger">*</span></label>
                  <select v-model="affectForm.employeId" class="form-select form-select-sm" required>
                    <option value="">-- Choisir un employé --</option>
                    <option v-for="e in employes" :key="e.id" :value="e.id">
                      {{ e.prenom }} {{ e.nom }} ({{ e.matricule }})
                    </option>
                  </select>
                </div>
                <div class="mb-2">
                  <label class="form-label small">Localisation <span class="text-danger">*</span></label>
                  <select v-model="affectForm.localisationId" class="form-select form-select-sm" required>
                    <option value="">-- Choisir --</option>
                    <option v-for="l in localisations" :key="l.id" :value="l.id">{{ l.nom }}</option>
                  </select>
                </div>
                <div class="mb-2">
                  <label class="form-label small">Commentaire</label>
                  <input v-model="affectForm.commentaires" type="text" class="form-control form-control-sm">
                </div>
                <button type="submit" class="btn btn-success btn-sm" :disabled="actionLoading">
                  <span v-if="actionLoading" class="spinner-border spinner-border-sm me-1"></span>
                  📤 Affecter
                </button>
              </form>
            </div>

            <!-- AFFECTÉ → Retour + Transfert -->
            <div v-else-if="materiel.etatLibelle === 'Affecté'">
              <h6 class="text-primary">📌 Matériel affecté à {{ materiel.employeNomComplet }}</h6>

              <!-- Retour -->
              <div class="mb-3 mt-2">
                <label class="form-label small fw-semibold">Retour au stock</label>
                <div class="d-flex gap-2 align-items-end">
                  <select v-model="retourForm.localisationId" class="form-select form-select-sm">
                    <option value="">-- Localisation de retour --</option>
                    <option v-for="l in localisations" :key="l.id" :value="l.id">{{ l.nom }}</option>
                  </select>
                  <input v-model="retourForm.commentaires" type="text"
                    class="form-control form-control-sm" placeholder="Commentaire">
                  <button class="btn btn-warning btn-sm text-nowrap" :disabled="actionLoading || !retourForm.localisationId"
                    @click="retourner">
                    📥 Retourner
                  </button>
                </div>
              </div>

              <!-- Transfert -->
              <div>
                <label class="form-label small fw-semibold">Transfert de localisation</label>
                <div class="d-flex gap-2 align-items-end">
                  <select v-model="transfertForm.nouvelleLocalisationId" class="form-select form-select-sm">
                    <option value="">-- Nouvelle localisation --</option>
                    <option v-for="l in localisations" :key="l.id" :value="l.id">{{ l.nom }}</option>
                  </select>
                  <input v-model="transfertForm.commentaires" type="text"
                    class="form-control form-control-sm" placeholder="Commentaire">
                  <button class="btn btn-info btn-sm text-white text-nowrap"
                    :disabled="actionLoading || !transfertForm.nouvelleLocalisationId"
                    @click="transferer">
                    🔄 Transférer
                  </button>
                </div>
              </div>
            </div>

            <!-- Autres états -->
            <div v-else>
              <p class="text-muted">
                État actuel : <EtatBadge :libelle="materiel.etatLibelle" />
                — aucune action disponible.
              </p>
            </div>

            <!-- Message retour action -->
            <div v-if="actionMsg" class="alert mt-3" :class="actionOk ? 'alert-success' : 'alert-danger'">
              {{ actionMsg }}
            </div>
          </div>
        </div>
      </div>

      <!-- Historique -->
      <div class="col-12">
        <div class="card shadow-sm">
          <div class="card-header d-flex justify-content-between align-items-center">
            <span class="fw-semibold">📜 Historique des mouvements</span>
            <button class="btn btn-outline-secondary btn-sm" @click="chargerHistorique">🔄 Actualiser</button>
          </div>
          <div class="card-body p-0">
            <div v-if="histLoading" class="text-center py-3"><div class="spinner-border spinner-border-sm text-primary"></div></div>
            <div v-else class="table-responsive">
              <table class="table table-sm table-hover mb-0">
                <thead class="table-light">
                  <tr>
                    <th>Date</th>
                    <th>Type</th>
                    <th>Employé</th>
                    <th>Localisation</th>
                    <th>Ancien état</th>
                    <th>Nouvel état</th>
                    <th>Commentaire</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="historique.length === 0">
                    <td colspan="7" class="text-center text-muted py-3">Aucun mouvement enregistré.</td>
                  </tr>
                  <tr v-for="h in historique" :key="h.id">
                    <td class="text-nowrap">{{ formatDate(h.dateMouvement) }}</td>
                    <td><span class="badge bg-dark">{{ h.typeMouvement }}</span></td>
                    <td>{{ h.employeNomComplet || '—' }}</td>
                    <td>{{ h.localisationNom }}</td>
                    <td><EtatBadge :libelle="h.ancienEtatLibelle" /></td>
                    <td><EtatBadge :libelle="h.nouvelEtatLibelle" /></td>
                    <td>{{ h.commentaires || '—' }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { materielService, employeService, localisationService } from '../services/api.js'
import EtatBadge from '../components/EtatBadge.vue'

const route = useRoute()
const id = route.params.id

const materiel = ref(null)
const qrCodeUrl = ref(`http://localhost:5109/api/materiel/${id}/qrcode`)
const isAdmin = ref(localStorage.getItem('role') === 'Admin')
const historique = ref([])
const employes = ref([])
const localisations = ref([])
const loading = ref(false)
const histLoading = ref(false)
const actionLoading = ref(false)
const actionMsg = ref('')
const actionOk = ref(true)

const affectForm = ref({ employeId: '', localisationId: '', commentaires: '' })
const retourForm = ref({ localisationId: '', commentaires: '' })
const transfertForm = ref({ nouvelleLocalisationId: '', commentaires: '' })

async function charger() {
  loading.value = true
  try {
    const res = await materielService.getById(id)
    materiel.value = res.data
  } finally {
    loading.value = false
  }
}

async function chargerHistorique() {
  histLoading.value = true
  try {
    const res = await materielService.historique(id)
    historique.value = res.data
  } finally {
    histLoading.value = false
  }
}

async function affecter() {
  actionLoading.value = true; actionMsg.value = ''
  try {
    await materielService.affecter(id, {
      employeId: parseInt(affectForm.value.employeId),
      localisationId: parseInt(affectForm.value.localisationId),
      commentaires: affectForm.value.commentaires || null
    })
    actionOk.value = true
    actionMsg.value = '✅ Matériel affecté avec succès.'
    affectForm.value = { employeId: '', localisationId: '', commentaires: '' }
    await charger(); await chargerHistorique()
  } catch (err) {
    actionOk.value = false
    actionMsg.value = err.response?.data?.message || 'Erreur lors de l\'affectation.'
  } finally { actionLoading.value = false }
}

async function retourner() {
  actionLoading.value = true; actionMsg.value = ''
  try {
    await materielService.retour(id, {
      localisationId: parseInt(retourForm.value.localisationId),
      commentaires: retourForm.value.commentaires || null
    })
    actionOk.value = true
    actionMsg.value = '✅ Retour au stock effectué.'
    retourForm.value = { localisationId: '', commentaires: '' }
    await charger(); await chargerHistorique()
  } catch (err) {
    actionOk.value = false
    actionMsg.value = err.response?.data?.message || 'Erreur lors du retour.'
  } finally { actionLoading.value = false }
}

async function transferer() {
  actionLoading.value = true; actionMsg.value = ''
  try {
    await materielService.transfert(id, {
      nouvelleLocalisationId: parseInt(transfertForm.value.nouvelleLocalisationId),
      commentaires: transfertForm.value.commentaires || null
    })
    actionOk.value = true
    actionMsg.value = '✅ Transfert effectué.'
    transfertForm.value = { nouvelleLocalisationId: '', commentaires: '' }
    await charger(); await chargerHistorique()
  } catch (err) {
    actionOk.value = false
    actionMsg.value = err.response?.data?.message || 'Erreur lors du transfert.'
  } finally { actionLoading.value = false }
}

function formatDate(d) {
  if (!d) return '—'
  return new Date(d).toLocaleDateString('fr-FR', { day:'2-digit', month:'2-digit', year:'numeric', hour:'2-digit', minute:'2-digit' })
}

onMounted(async () => {
  await charger()
  chargerHistorique()
  const [e, l] = await Promise.all([employeService.getAll(), localisationService.getAll()])
  employes.value = e.data
  localisations.value = l.data
})
</script>
