<template>
  <div class="row justify-content-center">
    <div class="col-lg-6">
      <div class="card shadow-sm">
        <div class="card-header bg-primary text-white">
          <h4 class="mb-0">{{ isEdit ? '✏️ Modifier employé' : '➕ Nouvel employé' }}</h4>
        </div>
        <div class="card-body">
          <div v-if="loading" class="text-center py-4"><div class="spinner-border text-primary"></div></div>
          <form v-else @submit.prevent="soumettre">
            <div class="row g-3">
              <div class="col-md-6" v-if="!isEdit">
                <label class="form-label">Matricule <span class="text-danger">*</span></label>
                <input v-model="form.matricule" type="text" class="form-control" required maxlength="20">
              </div>
              <div class="col-md-6">
                <label class="form-label">Nom <span class="text-danger">*</span></label>
                <input v-model="form.nom" type="text" class="form-control" required maxlength="100">
              </div>
              <div class="col-md-6">
                <label class="form-label">Prénom <span class="text-danger">*</span></label>
                <input v-model="form.prenom" type="text" class="form-control" required maxlength="100">
              </div>
              <div class="col-md-6">
                <label class="form-label">Email</label>
                <input v-model="form.email" type="email" class="form-control" maxlength="150">
              </div>
              <div class="col-md-6">
                <label class="form-label">Service</label>
                <input v-model="form.service" type="text" class="form-control" maxlength="100">
              </div>
              <div class="col-md-6">
                <label class="form-label">Poste</label>
                <input v-model="form.poste" type="text" class="form-control" maxlength="100">
              </div>
            </div>
            <div v-if="erreur" class="alert alert-danger mt-3">{{ erreur }}</div>
            <div class="d-flex gap-2 mt-4">
              <button type="submit" class="btn btn-primary" :disabled="saving">
                <span v-if="saving" class="spinner-border spinner-border-sm me-1"></span>
                💾 Enregistrer
              </button>
              <router-link to="/employes" class="btn btn-outline-secondary">✖ Annuler</router-link>
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
import { employeService } from '../services/api.js'

const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)

const form = ref({ matricule:'', nom:'', prenom:'', email:'', service:'', poste:'' })
const loading = ref(false)
const saving = ref(false)
const erreur = ref('')

onMounted(async () => {
  if (isEdit.value) {
    loading.value = true
    try {
      const res = await employeService.getById(route.params.id)
      const e = res.data
      form.value = { matricule: e.matricule, nom: e.nom, prenom: e.prenom, email: e.email||'', service: e.service||'', poste: e.poste||'' }
    } finally { loading.value = false }
  }
})

async function soumettre() {
  erreur.value = ''; saving.value = true
  try {
    const payload = { ...form.value, email: form.value.email||null, service: form.value.service||null, poste: form.value.poste||null }
    if (isEdit.value) await employeService.update(route.params.id, payload)
    else await employeService.create(payload)
    router.push('/employes')
  } catch (err) {
    erreur.value = err.response?.data?.message || 'Erreur lors de l\'enregistrement.'
  } finally { saving.value = false }
}
</script>
