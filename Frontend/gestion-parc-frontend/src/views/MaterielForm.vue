<template>
  <div class="row justify-content-center">
    <div class="col-lg-8">
      <div class="card shadow-sm">
        <div class="card-header bg-primary text-white">
          <h4 class="mb-0">{{ isEdit ? '✏️ Modifier le matériel' : '➕ Nouveau matériel' }}</h4>
        </div>
        <div class="card-body">
          <div v-if="loading" class="text-center py-4"><div class="spinner-border text-primary"></div></div>
          <form v-else @submit.prevent="soumettre">
            <div class="row g-3">
              <!-- CodeBarre -->
              <div class="col-md-6" v-if="!isEdit">
                <label class="form-label">Code-barres <span class="text-danger">*</span></label>
                <input v-model="form.codeBarre" type="text" class="form-control"
                  :class="{'is-invalid': erreurs.codeBarre}" required maxlength="50">
                <div class="invalid-feedback">{{ erreurs.codeBarre }}</div>
              </div>

              <!-- Libellé -->
              <div class="col-md-6">
                <label class="form-label">Libellé <span class="text-danger">*</span></label>
                <input v-model="form.libelle" type="text" class="form-control"
                  :class="{'is-invalid': erreurs.libelle}" required maxlength="100">
                <div class="invalid-feedback">{{ erreurs.libelle }}</div>
              </div>

              <!-- Type -->
              <div class="col-md-4">
                <label class="form-label">Type de matériel <span class="text-danger">*</span></label>
                <select v-model="form.typeMaterielId" class="form-select" required>
                  <option value="">-- Choisir --</option>
                  <option v-for="t in types" :key="t.id" :value="t.id">{{ t.libelle }}</option>
                </select>
              </div>

              <!-- Marque -->
              <div class="col-md-4">
                <label class="form-label">Marque</label>
                <input v-model="form.marque" type="text" class="form-control" maxlength="50">
              </div>

              <!-- Modèle -->
              <div class="col-md-4">
                <label class="form-label">Modèle</label>
                <input v-model="form.modele" type="text" class="form-control" maxlength="50">
              </div>

              <!-- Numéro de série -->
              <div class="col-md-6">
                <label class="form-label">Numéro de série</label>
                <input v-model="form.numeroSerie" type="text" class="form-control" maxlength="100"
                  placeholder="Unique si renseigné">
              </div>

              <!-- Date acquisition -->
              <div class="col-md-6">
                <label class="form-label">Date d'acquisition <span class="text-danger">*</span></label>
                <input v-model="form.dateAcquisition" type="date" class="form-control" required>
              </div>

              <!-- État (création uniquement) -->
              <div class="col-md-4" v-if="!isEdit">
                <label class="form-label">État initial <span class="text-danger">*</span></label>
                <select v-model="form.etatId" class="form-select" required>
                  <option value="">-- Choisir --</option>
                  <option v-for="e in etats" :key="e.id" :value="e.id">{{ e.libelle }}</option>
                </select>
              </div>

              <!-- Localisation -->
              <div class="col-md-4">
                <label class="form-label">Localisation <span class="text-danger">*</span></label>
                <select v-model="form.localisationId" class="form-select" required>
                  <option value="">-- Choisir --</option>
                  <option v-for="l in localisations" :key="l.id" :value="l.id">{{ l.nom }}</option>
                </select>
              </div>

              <!-- Commentaires -->
              <div class="col-12">
                <label class="form-label">Commentaires</label>
                <textarea v-model="form.commentaires" class="form-control" rows="2"></textarea>
              </div>
            </div>

            <!-- Erreur globale -->
            <div v-if="erreurGlobal" class="alert alert-danger mt-3">{{ erreurGlobal }}</div>

            <div class="d-flex gap-2 mt-4">
              <button type="submit" class="btn btn-primary" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
                💾 Enregistrer
              </button>
              <button type="button" class="btn btn-outline-secondary" @click="annuler">
                ✖ Annuler
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { materielService, typeService, etatService, localisationService } from '../services/api.js'

const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)

const form = ref({
  codeBarre: '', libelle: '', typeMaterielId: '', marque: '', modele: '',
  numeroSerie: '', dateAcquisition: '', etatId: '', localisationId: '', commentaires: ''
})
const types = ref([])
const etats = ref([])
const localisations = ref([])
const loading = ref(false)
const saving = ref(false)
const erreurs = ref({})
const erreurGlobal = ref('')

onMounted(async () => {
  loading.value = true
  try {
    const [t, e, l] = await Promise.all([
      typeService.getAll(), etatService.getAll(), localisationService.getAll()
    ])
    types.value = t.data
    etats.value = e.data
    localisations.value = l.data

    if (isEdit.value) {
      const res = await materielService.getById(route.params.id)
      const m = res.data
      form.value = {
        codeBarre: m.codeBarre,
        libelle: m.libelle,
        typeMaterielId: m.typeMaterielId,
        marque: m.marque || '',
        modele: m.modele || '',
        numeroSerie: m.numeroSerie || '',
        dateAcquisition: m.dateAcquisition?.substring(0, 10),
        etatId: m.etatId,
        localisationId: m.localisationId,
        commentaires: m.commentaires || ''
      }
    }
  } finally {
    loading.value = false
  }
})

async function soumettre() {
  erreurs.value = {}
  erreurGlobal.value = ''
  saving.value = true
  try {
    const payload = {
      ...form.value,
      typeMaterielId: parseInt(form.value.typeMaterielId),
      etatId: isEdit.value ? form.value.etatId : parseInt(form.value.etatId),
      localisationId: parseInt(form.value.localisationId),
      dateAcquisition: new Date(form.value.dateAcquisition).toISOString(),
      numeroSerie: form.value.numeroSerie || null,
      marque: form.value.marque || null,
      modele: form.value.modele || null,
      commentaires: form.value.commentaires || null
    }
    if (isEdit.value) {
      await materielService.update(route.params.id, payload)
      router.push(`/materiels/${route.params.id}`)
    } else {
      const res = await materielService.create(payload)
      router.push(`/materiels/${res.data.id}`)
    }
  } catch (err) {
    erreurGlobal.value = err.response?.data?.message || 'Erreur lors de l\'enregistrement.'
  } finally {
    saving.value = false
  }
}

function annuler() {
  if (isEdit.value) router.push(`/materiels/${route.params.id}`)
  else router.push('/')
}
</script>
