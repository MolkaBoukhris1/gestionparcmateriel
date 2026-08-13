<template>
  <div>
    <!-- Navbar Bootstrap -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container-fluid">
        <router-link class="navbar-brand fw-bold" to="/">
          🖥️ Gestion Parc Matériel
        </router-link>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navMenu">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navMenu">
          <ul class="navbar-nav ms-auto align-items-lg-center">
            <li class="nav-item">
              <router-link class="nav-link" active-class="active" to="/dashboard">
                📊 Dashboard
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" active-class="active" to="/insights">
                🧠 Analyses
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" active-class="active" to="/">
                📦 Matériels
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" active-class="active" to="/employes">
                👤 Employés
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" active-class="active" to="/localisations">
                📍 Localisations
              </router-link>
            </li>

            <!-- Utilisateur connecté -->
            <li v-if="isLoggedIn" class="nav-item ms-lg-3 d-flex align-items-center">
              <span class="badge bg-light text-primary me-2">
                {{ role }} — {{ nomComplet }}
              </span>
              <button class="btn btn-sm btn-outline-light" @click="logout">Déconnexion</button>
            </li>
            <li v-else class="nav-item ms-lg-3">
              <router-link class="btn btn-sm btn-light" to="/login">
                🔐 Connexion
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Toast erreur API -->
    <div v-if="errorMsg" class="position-fixed top-0 end-0 p-3" style="z-index:9999">
      <div class="toast show align-items-center text-white bg-danger border-0">
        <div class="d-flex">
          <div class="toast-body fw-semibold">⚠️ {{ errorMsg }}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" @click="errorMsg=''"></button>
        </div>
      </div>
    </div>

    <!-- Contenu principal -->
    <div class="container-fluid py-4 px-4">
      <router-view />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const errorMsg = ref('')
const isLoggedIn = ref(!!localStorage.getItem('token'))
const role = ref(localStorage.getItem('role') || '')
const nomComplet = ref(localStorage.getItem('nomComplet') || '')
let timer = null

function handleApiError(e) {
  errorMsg.value = e.detail
  clearTimeout(timer)
  timer = setTimeout(() => { errorMsg.value = '' }, 5000)
}

function logout() {
  localStorage.removeItem('token')
  localStorage.removeItem('role')
  localStorage.removeItem('nomComplet')
  window.location.href = '/login'
}

onMounted(() => window.addEventListener('api-error', handleApiError))
onUnmounted(() => window.removeEventListener('api-error', handleApiError))
</script>