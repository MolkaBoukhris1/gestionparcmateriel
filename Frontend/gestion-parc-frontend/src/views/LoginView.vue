<template>
  <div class="d-flex justify-content-center align-items-center" style="min-height:80vh">
    <div class="card shadow-sm" style="width:400px">
      <div class="card-body p-4">
        <h3 class="text-center mb-4">🔐 Connexion</h3>

        <form @submit.prevent="login">
          <div class="mb-3">
            <label class="form-label">Email</label>
            <input v-model="email" type="email" class="form-control" required>
          </div>
          <div class="mb-3">
            <label class="form-label">Mot de passe</label>
            <input v-model="motDePasse" type="password" class="form-control" required>
          </div>

          <div v-if="errorMsg" class="alert alert-danger py-2">{{ errorMsg }}</div>

          <button type="submit" class="btn btn-primary w-100" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-1"></span>
            Se connecter
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { authService } from '../services/api.js'

const email = ref('')
const motDePasse = ref('')
const loading = ref(false)
const errorMsg = ref('')

async function login() {
  loading.value = true
  errorMsg.value = ''
  try {
    const res = await authService.login({ email: email.value, motDePasse: motDePasse.value })
    localStorage.setItem('token', res.data.token)
    localStorage.setItem('role', res.data.role)
    localStorage.setItem('nomComplet', res.data.nomComplet || res.data.email)
window.location.href = '/dashboard'  } catch (err) {
    errorMsg.value = err.response?.data?.message || 'Erreur de connexion.'
  } finally {
    loading.value = false
  }
}
</script>